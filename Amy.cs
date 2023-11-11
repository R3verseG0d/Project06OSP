using System;
using STHLua;
using UnityEngine;

public class Amy : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		SlowFall = 4,
		HammerAttack = 5,
		HammerSpin = 6,
		HammerJump = 7,
		AirHammerSpin = 8,
		Hurt = 9,
		Grinding = 10,
		Death = 11,
		FallDeath = 12,
		DrownDeath = 13,
		SnowBallDeath = 14,
		TornadoDeath = 15,
		Talk = 16,
		Path = 17,
		WarpHole = 18,
		Result = 19,
		Cutscene = 20,
		DashPanel = 21,
		Spring = 22,
		WideSpring = 23,
		JumpPanel = 24,
		DashRing = 25,
		RainbowRing = 26,
		Hold = 27,
		UpReel = 28,
		Balancer = 29
	}

	[Header("Player Framework")]
	public AmyEffects AmyEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private bool CanDoubleJump;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	private float BodyFwdDirDot;

	[Header("Hammer Attack")]
	public Renderer HammerRenderer;

	private bool SwitchHammer;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip HammerAttackSound;

	public AudioClip BrakeSound;

	[Header("Audio Sources")]
	public AudioSource TarotAudioSource;

	public AudioSource HammerWindLoopSource;

	private bool StopTarotEffect;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool IsDoubleJump;

	private bool AttackDamage;

	private bool HammerSmash;

	private bool StandHammerAttack;

	private float AttackTimer;

	private float HammerSpeed;

	internal int HammerSpinState;

	private float HammerSpinTimer;

	private float HammerSpinSpeed;

	private float HammerSpinAnim;

	private float HammerSpinDizzyAnim;

	private float AlternateVel;

	private float HammerJumpTime;

	private bool HammerJumpApex;

	private bool AirHammerSpinDamage;

	private float AirHammerSpinTime;

	private float HurtTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Amy_Lua.c_player_name;
		PlayerNameShort = Amy_Lua.c_player_name_short;
		WalkSpeed = Amy_Lua.c_walk_speed_max;
		TopSpeed = Amy_Lua.c_run_speed_max;
		BrakeSpeed = Amy_Lua.c_brake_acc;
		GrindSpeedOrg = Amy_Lua.c_grind_speed_org;
		GrindAcc = Amy_Lua.c_grind_acc;
		GrindSpeedMax = Amy_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Amy_Lua.OpenGauge(), Amy_Lua.c_gauge_max, 0f, Amy_Lua.c_gauge_heal_wait);
	}

	private void StateGroundStart()
	{
		PlayerState = State.Ground;
		Animator.ResetTrigger("Additive Idle");
		MaxRayLenght = 0.75f;
		IdleAnimPlayed = false;
		IdleTimer = Time.time;
		StopTarotEffect = false;
	}

	private void StateGround()
	{
		PlayerState = State.Ground;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (TargetDirection.magnitude == 0f)
		{
			if (Time.time - IdleTimer > 7.5f && !IdleAnimPlayed)
			{
				IdleAnimPlayed = true;
				IdleTimer = Time.time;
				PlayIdleEvent(3);
				IdleAnimPlayed = false;
				StopTarotEffect = false;
			}
		}
		else
		{
			Animator.SetTrigger("Additive Idle");
			IdleTimer = Time.time;
			if (!StopTarotEffect)
			{
				AmyEffects.PlayTarotFX(Enable: false);
				StopTarotEffect = true;
			}
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Amy_Lua.c_walk_speed_max * 1.5f)
		{
			if (ShouldAlignOrFall(Align: false))
			{
				if (!IsSlopePhys)
				{
					StateMachine.ChangeState(StateBrake);
				}
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		if (!IsGrounded() || (CurSpeed <= Amy_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateGroundEnd()
	{
		Animator.SetTrigger("Additive Idle");
		AmyEffects.PlayTarotFX(Enable: false);
	}

	private void StateBrakeStart()
	{
		PlayerState = State.Brake;
		Audio.PlayOneShot(BrakeSound, Audio.volume);
		BrakeSpd = CurSpeed;
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Amy_Lua.c_run_speed_max) / Amy_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Amy_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Amy_Lua.c_run_speed_max;
		}
		BrakeStopped = false;
	}

	private void StateBrake()
	{
		PlayerState = State.Brake;
		LockControls = true;
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		BrakeSpd = Mathf.Max(0f, BrakeSpd - BrakeDecelSpeed * 2f * Time.fixedDeltaTime);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * BrakeSpd;
		CurSpeed = BrakeSpd;
		if ((BrakeSpd == 0f || FrontalCollision) && !BrakeStopped)
		{
			BrakeSpd = 0f;
			BrakeTime = Time.time;
			BrakeStopped = true;
		}
		if (BrakeStopped)
		{
			if (Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") < 0f)
			{
				float num = (Time.time - BrakeTime) / 0.5165f;
				PlayAnimation("Brake Stop", "On Brake");
				if (num > 1f)
				{
					TargetDirection = -base.transform.forward;
					base.transform.rotation = Quaternion.LookRotation(TargetDirection);
					GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
					StateMachine.ChangeState(StateGround);
				}
				else
				{
					GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
				}
			}
			else
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		else
		{
			GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
			PlayAnimation("Brake", "On Brake");
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateBrakeEnd()
	{
	}

	private void StateJumpStart()
	{
		PlayerState = State.Jump;
		JumpTime = Time.time;
		JumpAnimation = 0;
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Amy_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Amy_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateJump()
	{
		PlayerState = State.Jump;
		PlayAnimation("Jump Up", "On Jump");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y, 12f);
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Amy_Lua.c_jump_time_min) ? 1 : 0));
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!IsDoubleJump && !HalveSinkJump) ? 0.7f : 0.45f))
		{
			AirMotionVelocity += Vector3.up * ((!HalveSinkJump) ? 4.25f : 3f) * Time.fixedDeltaTime * 4f;
		}
		if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, 1))
		{
			JumpTime = Time.time;
			JumpAnimation = 0;
			AirMotionVelocity.y = 12f;
		}
		MaxRayLenght = ((Time.time - JumpTime > 0.25f) ? 0.75f : 0f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateJumpEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateAirStart()
	{
		PlayerState = State.Air;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAir()
	{
		PlayerState = State.Air;
		PlayAnimation("Air Falling", "On Air Fall");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateAirEnd()
	{
		Animator.SetTrigger("Additive Idle");
	}

	private void StateHammerAttackStart()
	{
		PlayerState = State.HammerAttack;
		AttackTimer = Time.time;
		HammerSpeed = Mathf.Max(Amy_Lua.c_walk_speed_max * 2f, CurSpeed);
		AttackDamage = true;
		HammerSmash = false;
		Audio.PlayOneShot(HammerAttackSound, Audio.volume);
		PlayerVoice.PlayRandom(6, RandomPlayChance: true);
		AmyEffects.OnHammerTrailFX(Enable: true);
		StandHammerAttack = CurSpeed < Amy_Lua.c_walk_speed_max * 3f;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
	}

	private void StateHammerAttack()
	{
		PlayerState = State.HammerAttack;
		PlayAnimation((!StandHammerAttack) ? "Hammer Attack" : "Hammer Attack Stand", (!StandHammerAttack) ? "On Attack" : "On Attack Stand");
		CurSpeed = ((Time.time - AttackTimer < 0.3f && !StandHammerAttack) ? HammerSpeed : Mathf.Lerp(CurSpeed, 0f, Time.fixedDeltaTime * 3f));
		if (Time.time - AttackTimer > (StandHammerAttack ? ((TargetDirection != Vector3.zero) ? 0.5f : 0.65f) : ((TargetDirection != Vector3.zero) ? 0.75f : 1.1f)))
		{
			StateMachine.ChangeState(StateGround);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if ((!StandHammerAttack && AttackDamage && AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, base.transform.forward * CurSpeed, (!IsVisible) ? 1 : 2)) || Time.time - AttackTimer > 0.3f)
		{
			AttackDamage = false;
		}
		if (Time.time - AttackTimer > ((!StandHammerAttack) ? 0.3f : 0.25f) && !HammerSmash)
		{
			HammerSmash = true;
			AttackSphere_Dir(base.transform.position + base.transform.up * -0.25f + base.transform.forward * ((!StandHammerAttack) ? 0.9f : 1.1f), 2.1f, 5f, 1);
			AmyEffects.CreateHammerAttackFX((!StandHammerAttack) ? 0.9f : 1.1f);
			AmyEffects.OnHammerTrailFX(Enable: false);
			if (!StandHammerAttack)
			{
				if (Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					CurSpeed = HammerSpeed * 1.5f;
					StateMachine.ChangeState(StateHammerJump);
				}
				else
				{
					Animator.SetTrigger("On Hammer Bump");
				}
			}
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHammerAttackEnd()
	{
		AmyEffects.OnHammerTrailFX(Enable: false);
	}

	private void StateHammerSpinStart()
	{
		PlayerState = State.HammerSpin;
		HammerSpinTimer = Time.time;
		HammerSpeed = Mathf.Min(CurSpeed, Amy_Lua.c_walk_speed_max * 6f);
		HammerSpinAnim = 0f;
		HammerSpinDizzyAnim = 1f;
		HammerSpinState = 0;
		Audio.PlayOneShot(BrakeSound, Audio.volume * 0.5f);
		Audio.PlayOneShot(HammerAttackSound, Audio.volume);
	}

	private void StateHammerSpin()
	{
		PlayerState = State.HammerSpin;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (HammerSpinState == 0)
		{
			PlayAnimation("Hammer Spin Wind Up", "On Hammer Spin Wind Up");
			CurSpeed = Mathf.Lerp(CurSpeed, 0f, Time.fixedDeltaTime * 5f);
			if (Time.time - HammerSpinTimer > 0.25f)
			{
				HammerSpinTimer = Time.time;
				CurSpeed = HammerSpeed;
				PlayerVoice.PlayRandom(new int[3] { 5, 6, 12 });
				AmyEffects.OnHammerTrailFX(Enable: true);
				HammerSpinState = 1;
			}
		}
		else if (HammerSpinState == 1)
		{
			PlayAnimation("Hammer Spin", "On Hammer Spin");
			AttackSphere_Dir(base.transform.position, 1.5f, 15f, (!IsVisible) ? 1 : 2);
		}
		else
		{
			if (Time.time - HammerSpinTimer > 6.25f)
			{
				HammerSpinDizzyAnim = Mathf.MoveTowards(HammerSpinDizzyAnim, 2f, Time.fixedDeltaTime);
				AlternateVel = Mathf.Lerp(AlternateVel, 0f, Time.fixedDeltaTime * 2f);
			}
			else
			{
				AlternateVel = ((CurSpeed > 0f) ? Mathf.SmoothStep(-0.75f, 0.75f, Mathf.PingPong(Time.time / 1f, 1f)) : 0f);
			}
			HammerSpinAnim = Mathf.MoveTowards(HammerSpinAnim, HammerSpinDizzyAnim, Time.fixedDeltaTime * 2.5f);
			PlayAnimation("Hammer Spin", "On Hammer Spin");
			_Rigidbody.velocity += base.transform.right * AlternateVel;
			if (Time.time - HammerSpinTimer > 7.5f)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHammerSpinEnd()
	{
		AmyEffects.OnHammerTrailFX(Enable: false);
	}

	private void StateHammerJumpStart()
	{
		PlayerState = State.HammerJump;
		HammerJumpTime = Time.time;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		HammerJumpApex = false;
		AmyEffects.CreateHammerJumpTrailFX();
		AmyEffects.OnHammerTrailFX(Enable: true);
	}

	private void StateHammerJump()
	{
		PlayerState = State.HammerJump;
		PlayAnimation("Hammer Jump", "On Attack");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - HammerJumpTime < 0.35f)
		{
			AirMotionVelocity.y = 6f;
		}
		else
		{
			if (!HammerJumpApex)
			{
				AmyEffects.OnHammerTrailFX(Enable: false);
				HammerJumpApex = true;
			}
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		if (Time.time - HammerJumpTime < 0.5f)
		{
			AttackSphere(base.transform.position, 1f, base.transform.forward * CurSpeed, (!IsVisible) ? 1 : 2);
		}
		DoWallNormal();
		if (Time.time - HammerJumpTime > 0.25f && IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateHammerJumpEnd()
	{
		AmyEffects.OnHammerTrailFX(Enable: false);
	}

	private void StateAirHammerSpinStart()
	{
		PlayerState = State.AirHammerSpin;
		AirHammerSpinDamage = true;
		AirHammerSpinTime = Time.time;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		AmyEffects.CreateHammerJumpTrailFX();
		AmyEffects.OnHammerTrailFX(Enable: true);
		PlayerVoice.PlayRandom(8, RandomPlayChance: true);
		PlayAnimation("Air Falling", "On Air Fall");
	}

	private void StateAirHammerSpin()
	{
		PlayerState = State.AirHammerSpin;
		PlayAnimation("Air Hammer Spin", "On Air Hammer");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		if (AirHammerSpinDamage && AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, base.transform.forward * CurSpeed, (!IsVisible) ? 1 : 2))
		{
			AirHammerSpinDamage = false;
			AirMotionVelocity.y = 0f;
			AirMotionVelocity.y = 16f;
		}
		DoWallNormal();
		if (Time.time - AirHammerSpinTime > 0.5f)
		{
			StateMachine.ChangeState(StateAir);
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateAirHammerSpinEnd()
	{
		AmyEffects.OnHammerTrailFX(Enable: false);
	}

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		HurtTime = Time.time;
		PlayerVoice.PlayRandom(4);
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Amy_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Amy_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Amy_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
		if (Time.time - HurtTime > 1.2f)
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
		else if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHurtEnd()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Hurt || PlayerState == State.HammerAttack || PlayerState == State.HammerSpin || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Amy_Lua.c_rotation_speed, PlayerState == State.HammerSpin && (HammerSpinState == 0 || HammerSpinState == 2), DontLockOnAir: false, (HammerSpinState == 0) ? 0f : 2f);
				if (PlayerState != State.HammerAttack && ((PlayerState == State.HammerSpin && HammerSpinState != 0) || PlayerState != State.HammerSpin))
				{
					AccelerationSystem((!HasSpeedUp) ? Amy_Lua.c_run_acc : Amy_Lua.c_speedup_acc);
				}
				if (PlayerState == State.HammerJump)
				{
					MaximumSpeed = ((!HasSpeedUp) ? (Amy_Lua.c_run_speed_max * 1.25f) : Amy_Lua.c_speedup_speed_max);
				}
				else if (PlayerState == State.HammerSpin && HammerSpinState > 0)
				{
					MaximumSpeed = Amy_Lua.c_walk_speed_max * ((HammerSpinState == 1) ? 8f : 1f);
				}
				else if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Amy_Lua.c_walk_speed_max : Amy_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Amy_Lua.c_speedup_speed_max : (IsGrounded() ? Amy_Lua.c_run_speed_max : Amy_Lua.c_jump_run));
				}
			}
			else
			{
				LockControls = false;
			}
		}
		SlopePhysics(PlayerState == State.Ground);
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Hurt || PlayerState == State.HammerAttack || PlayerState == State.HammerSpin || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (PlayerState == State.HammerAttack || (PlayerState == State.HammerSpin && HammerSpinState < 2) || PlayerState == State.HammerJump || PlayerState == State.AirHammerSpin)
		{
			if (!SwitchHammer)
			{
				AmyEffects.PlayHammerInvokeFX();
				SwitchHammer = true;
			}
			HammerRenderer.enabled = true;
		}
		else if (SwitchHammer)
		{
			HammerRenderer.enabled = false;
			AmyEffects.PlayHammerInvokeFX();
			SwitchHammer = false;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (!CanDoubleJump && (IsGrounded() || PlayerState == State.Grinding || (PlayerState == State.Spring && LockControls) || (PlayerState == State.WideSpring && LockControls) || (PlayerState == State.JumpPanel && LockControls) || (PlayerState == State.RainbowRing && !LockControls)))
			{
				CanDoubleJump = true;
			}
			if (PlayerState == State.Ground)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
				{
					OnJump();
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateHammerAttack);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateHammerSpin);
				}
			}
			if (PlayerState == State.Air || PlayerState == State.SlowFall)
			{
				if (ReleasedKey && CanDoubleJump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					CanDoubleJump = false;
					OnJump(DoubleJump: true);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateAirHammerSpin);
				}
			}
			if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
			{
				if ((PlayerState != State.Jump || (PlayerState == State.Jump && ReleasedKey)) && CanDoubleJump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					CanDoubleJump = false;
					OnJump(DoubleJump: true);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateAirHammerSpin);
				}
			}
			if (PlayerState == State.HammerAttack)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && HammerSmash)
				{
					OnJump();
				}
				if (StandHammerAttack && Time.time - AttackTimer > 0.4f && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateHammerAttack);
				}
			}
			if (PlayerState == State.HammerSpin)
			{
				if (HammerSpinState == 1)
				{
					if (Time.time - HammerSpinTimer > 0.5f && Time.time - HammerSpinTimer < 4f && !Singleton<RInput>.Instance.P.GetButton("Button B"))
					{
						StateMachine.ChangeState(StateGround);
					}
					else if (Time.time - HammerSpinTimer >= 4f && Singleton<RInput>.Instance.P.GetButton("Button B"))
					{
						HammerSpinTimer = Time.time;
						PlayerVoice.PlayRandom(new int[2] { 13, 7 });
						AmyEffects.OnHammerTrailFX(Enable: false);
						HammerSpinState = 2;
					}
				}
				if (Time.time - HammerSpinTimer > 0.1f && HammerSpinState < 2 && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
			}
			if (PlayerState == State.HammerJump && Time.time - HammerJumpTime > 0.25f)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateAirHammerSpin);
				}
				if (CanDoubleJump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					CanDoubleJump = false;
					OnJump(DoubleJump: true);
				}
			}
		}
		HammerWindLoopSource.volume = Mathf.Lerp(HammerWindLoopSource.volume, (PlayerState == State.HammerSpin && HammerSpinState == 1) ? 1f : 0f, Time.deltaTime * 3f);
	}

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		Animator.SetFloat("Hammer Spin Anim", HammerSpinAnim);
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump)
		{
			return 0;
		}
		if (PlayerState == State.HammerAttack || (PlayerState == State.HammerSpin && HammerSpinState == 1) || (PlayerState == State.HammerJump && Time.time - HammerJumpTime < 0.5f) || PlayerState == State.AirHammerSpin)
		{
			return 1;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if (PlayerState == State.Hurt || PlayerState == State.Talk || PlayerState == State.WarpHole || PlayerState == State.Result || PlayerState == State.Cutscene || HasInvincibility || IsDead)
		{
			return true;
		}
		return result;
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			if (PlayerState == State.HammerSpin && HammerSpinState == 1)
			{
				BodyDirDot = Mathf.Lerp(BodyDirDot, UseCharacterSway ? ((0f - num) * CurSpeed * 1.5f) : 0f, 5f * Time.fixedDeltaTime);
				BodyFwdDirDot = Mathf.Lerp(BodyFwdDirDot, UseCharacterSway ? (CurSpeed * 1.5f) : 0f, 5f * Time.fixedDeltaTime);
			}
			else
			{
				BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && ((PlayerState == State.Ground && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
				BodyFwdDirDot = Mathf.Lerp(BodyFwdDirDot, 0f, 10f * Time.fixedDeltaTime);
			}
			BodyTransform.localEulerAngles = new Vector3(BodyFwdDirDot, BodyTransform.localEulerAngles.y, BodyDirDot);
		}
		if (ImmunityTime - Time.time >= 0f)
		{
			BlinkTimer += Time.fixedDeltaTime * 15f;
			if (BlinkTimer >= 1f)
			{
				BlinkTimer = 0f;
			}
		}
		for (int i = 0; i < PlayerRenderers.Length; i++)
		{
			PlayerRenderers[i].enabled = ImmunityTime - Time.time <= 0f || ((ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		}
	}

	private void OnJump(bool DoubleJump = false)
	{
		IsDoubleJump = DoubleJump;
		StateMachine.ChangeState(StateJump);
		if (!DoubleJump)
		{
			Audio.PlayOneShot(JumpSound, Audio.volume);
			PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		}
		else
		{
			PlayerVoice.PlayRandom(5, RandomPlayChance: true);
			AmyEffects.CreateDoubleJumpFX();
		}
	}

	public override void OnHurtEnter(int HurtType = 0)
	{
		base.OnHurtEnter(HurtType);
		if (!(ImmunityTime - Time.time <= 0f) || IsInvulnerable(HurtType))
		{
			return;
		}
		int rings = Singleton<GameManager>.Instance._PlayerData.rings;
		if (rings > 0 || HasShield)
		{
			ImmunityTime = Time.time + Common_Lua.c_invincible_time;
			if (PlayerState != State.Grinding && PlayerState != State.Balancer)
			{
				if (IsGrounded() && ShouldAlignOrFall(Align: false))
				{
					StateMachine.ChangeState(StateHurt);
				}
				else
				{
					Vector3 velocity = _Rigidbody.velocity;
					velocity.y = 0f;
					_Rigidbody.velocity = velocity;
					StateMachine.ChangeState(StateAir);
				}
			}
			if (HasShield)
			{
				RemoveShield();
				return;
			}
			CreateRings(Mathf.Min(rings, 20));
			Singleton<GameManager>.Instance._PlayerData.rings = 0;
		}
		else
		{
			OnDeathEnter(0);
		}
	}

	public override void OnDeathEnter(int DeathType)
	{
		if (!IsDead && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && PlayerState != State.Hold)
		{
			IsDead = true;
			Camera.OnPlayerDeath();
			DoDeath(DeathType);
		}
	}

	public override string GetState()
	{
		return PlayerState.ToString();
	}

	public override void SetMachineState(string StateName)
	{
		switch (StateName)
		{
		case "StateGround":
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
			break;
		case "StateJump":
			StateMachine.ChangeState(StateJump);
			break;
		case "StateAir":
			StateMachine.ChangeState(StateAir);
			break;
		case "StateHurt":
			StateMachine.ChangeState(StateHurt);
			break;
		}
	}

	public override void StartPlayer(bool TalkState = false)
	{
		if (!TalkState)
		{
			if (IsGrounded())
			{
				StateGroundStart();
				StateMachine.Initialize(StateGround);
			}
			else
			{
				StateAirStart();
				StateMachine.Initialize(StateAir);
			}
		}
		else
		{
			StateTalkStart();
			StateMachine.Initialize(base.StateTalk);
		}
	}

	public override void SetState(string StateName)
	{
		PlayerState = (State)Enum.Parse(typeof(State), StateName);
	}
}
