using System;
using STHLua;
using UnityEngine;

public class Blaze : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		AccelJump = 4,
		FireClaw = 5,
		SlowFall = 6,
		Homing = 7,
		AfterHoming = 8,
		SpinningClaw = 9,
		CrowAttack = 10,
		Hurt = 11,
		EdgeDanger = 12,
		Grinding = 13,
		Death = 14,
		FallDeath = 15,
		DrownDeath = 16,
		SnowBallDeath = 17,
		TornadoDeath = 18,
		Talk = 19,
		Path = 20,
		WarpHole = 21,
		Result = 22,
		Cutscene = 23,
		DashPanel = 24,
		Spring = 25,
		WideSpring = 26,
		JumpPanel = 27,
		DashRing = 28,
		RainbowRing = 29,
		Hold = 30,
		UpReel = 31,
		Balancer = 32
	}

	[Header("Player Framework")]
	public BlazeEffects BlazeEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private bool CanAccelJump;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	private float BodyFwdDirDot;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip AccelTornadoSound;

	public AudioClip AttackSound;

	public AudioClip SpinningClawStopSound;

	public AudioClip BrakeSound;

	[Header("Audio Sources")]
	public AudioSource[] SpinningClawSources;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool ReleasedAccelJump;

	internal bool ReachedApex;

	private float AccelJumpTime;

	private bool StopClawTrail;

	private bool AirFireClaw;

	private float FireClawTime;

	internal Vector3 HomingDirection;

	private Vector3 DirectionToTarget;

	private Vector3 HAForward;

	private Vector3 HAStartPos;

	private float HAStartTime;

	private float HomingTime;

	private string HATag;

	private float AHEnterTime;

	private bool HAPressed;

	private bool SpinRelease;

	private float SpinTime;

	private bool IdleAttack;

	private float AttackTimer;

	private float AttackSpeed;

	private float HurtTime;

	private float EdgeDangerTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Blaze_Lua.c_player_name;
		PlayerNameShort = Blaze_Lua.c_player_name_short;
		WalkSpeed = Blaze_Lua.c_walk_speed_max;
		TopSpeed = Blaze_Lua.c_run_speed_max;
		BrakeSpeed = Blaze_Lua.c_brake_acc;
		GrindSpeedOrg = Blaze_Lua.c_grind_speed_org;
		GrindAcc = Blaze_Lua.c_grind_acc;
		GrindSpeedMax = Blaze_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.CloseGauge();
	}

	private void StateGroundStart()
	{
		PlayerState = State.Ground;
		Animator.ResetTrigger("Additive Idle");
		MaxRayLenght = 0.75f;
		IdleAnimPlayed = false;
		IdleTimer = Time.time;
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
				PlayIdleEvent(2);
				IdleAnimPlayed = false;
			}
		}
		else
		{
			Animator.SetTrigger("Additive Idle");
			IdleTimer = Time.time;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Blaze_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Blaze_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateGroundEnd()
	{
		Animator.SetTrigger("Additive Idle");
	}

	private void StateBrakeStart()
	{
		PlayerState = State.Brake;
		Audio.PlayOneShot(BrakeSound, Audio.volume);
		BrakeSpd = CurSpeed;
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Blaze_Lua.c_run_speed_max) / Blaze_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Blaze_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Blaze_Lua.c_run_speed_max;
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
		if (IsGrounded())
		{
			if (ShouldEdgeDanger())
			{
				StateMachine.ChangeState(StateEdgeDanger);
			}
		}
		else
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
		Audio.PlayOneShot(JumpSound, Audio.volume);
		PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		JumpTime = Time.time;
		JumpAnimation = 0;
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Blaze_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Blaze_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Blaze_Lua.c_jump_time_min) ? 1 : 0));
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!HalveSinkJump) ? 0.7f : 0.45f))
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
	}

	private void StateAccelJumpStart()
	{
		PlayerState = State.AccelJump;
		AccelJumpTime = Time.time;
		ReleasedAccelJump = false;
		ReachedApex = false;
		PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = 10f;
		_Rigidbody.velocity = AirMotionVelocity;
		BlazeEffects.CreateAccelJumpTrailFX();
		BlazeEffects.OnClawTrailFX(Enable: true);
	}

	private void StateAccelJump()
	{
		PlayerState = State.AccelJump;
		if (AirMotionVelocity.y < -4f)
		{
			PlayAnimation("Falling", "On Fall");
			if (!ReleasedAccelJump && !StopClawTrail)
			{
				BlazeEffects.OnClawTrailFX(Enable: false);
				StopClawTrail = true;
			}
		}
		else
		{
			PlayAnimation("Tornado", "On Tornado");
		}
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedAccelJump = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedAccelJump)
		{
			if (Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - AccelJumpTime < 0.25f)
			{
				AirMotionVelocity += Vector3.up * 8f * Time.fixedDeltaTime * 2f;
			}
			else if (Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - AccelJumpTime > 0.25f && !ReachedApex)
			{
				Audio.PlayOneShot(AccelTornadoSound, Audio.volume);
				BlazeEffects.OnClawTrailFX(Enable: false);
				ReachedApex = true;
			}
		}
		if (ReachedApex)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, _Rigidbody.velocity.normalized * (CurSpeed * 1.25f), 1);
		}
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

	private void StateAccelJumpEnd()
	{
		BlazeEffects.OnClawTrailFX(Enable: false);
	}

	private void StateFireClawStart()
	{
		PlayerState = State.FireClaw;
		FireClawTime = Time.time;
		AirFireClaw = !IsGrounded();
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateFireClaw()
	{
		PlayerState = State.FireClaw;
		PlayAnimation("Fire Claw", "On Fire Claw");
		CurSpeed = Blaze_Lua.c_homing_spd * 1.25f;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - FireClawTime < 0.25f && !AirFireClaw)
		{
			AirMotionVelocity.y = 8f;
		}
		else
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
			}
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (FrontalCollision)
		{
			AirMotionVelocity = _Rigidbody.velocity;
			AirMotionVelocity.y = 0f;
			_Rigidbody.velocity = AirMotionVelocity;
			StateMachine.ChangeState(StateAir);
		}
		if (AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, _Rigidbody.velocity.normalized * Blaze_Lua.c_homing_power, 1))
		{
			StateMachine.ChangeState(StateAfterHoming);
		}
		GeneralMeshRotation = Quaternion.LookRotation(_Rigidbody.velocity.normalized);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
	}

	private void StateFireClawEnd()
	{
		BlazeEffects.OnClawTrailFX(Enable: false);
	}

	private void StateHomingStart()
	{
		PlayerState = State.Homing;
		MaxRayLenght = 0f;
		DirectionToTarget = HomingTarget.transform.position - base.transform.position;
		HAStartTime = Time.time;
		float num = DirectionToTarget.magnitude / 15f;
		DirectionToTarget.Normalize();
		HomingTime = Mathf.Lerp(0.1f, 0.3f, num * num);
		HomingDirection = DirectionToTarget;
		HAForward = base.transform.forward;
		HAStartPos = base.transform.position;
		HATag = HomingTarget.tag;
	}

	private void StateHoming()
	{
		PlayerState = State.Homing;
		PlayAnimation("Fire Claw", "On Fire Claw");
		LockControls = true;
		if (!HomingTarget || !HomingTarget.CompareTag(HATag))
		{
			StateMachine.ChangeState(StateAir);
		}
		float num = (Time.time - HAStartTime) / HomingTime;
		float num2 = Mathf.Clamp(num, 0f, 1f);
		num2 *= num2;
		_Rigidbody.velocity = Vector3.zero;
		Vector3 vector = HomingDirection.MakePlanar();
		if (vector == Vector3.zero)
		{
			vector = HAForward.MakePlanar();
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.LookRotation(vector);
		_Rigidbody.MovePosition(Vector3.Lerp(HAStartPos, HomingTarget.transform.position, num2));
		if (AttackSphere(base.transform.position + base.transform.forward * 0.25f, Blaze_Lua.c_collision_homing(), DirectionToTarget * Blaze_Lua.c_homing_power, Blaze_Lua.c_homing_damage))
		{
			StateMachine.ChangeState(StateAfterHoming);
		}
		if (num > 5f || FrontalCollision || DirectionToTarget.magnitude > 15f)
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHomingEnd()
	{
		MaxRayLenght = 0.75f;
		if (PlayerState != State.Grinding)
		{
			HomingDirection = Vector3.zero;
		}
		BlazeEffects.OnClawTrailFX(Enable: false);
	}

	private void StateAfterHomingStart()
	{
		PlayerState = State.AfterHoming;
		MaxRayLenght = 0.75f;
		AHEnterTime = Time.time;
		HAPressed = true;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAfterHoming()
	{
		PlayerState = State.AfterHoming;
		PlayAnimation("After Homing", "On After Homing");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - AHEnterTime <= 0.15f || (Time.time - AHEnterTime <= 0.45f && HAPressed))
		{
			AirMotionVelocity.y = 6.3765f;
			CurSpeed = 0f;
		}
		else
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
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

	private void StateAfterHomingEnd()
	{
	}

	private void StateSpinningClawStart()
	{
		PlayerState = State.SpinningClaw;
		SpinTime = Time.time;
		SpinRelease = false;
		PlayerVoice.PlayRandom(7, RandomPlayChance: true);
		SpinningClawSources[1].volume = 0f;
		SpinningClawSources[0].Play();
		SpinningClawSources[1].Play();
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateSpinningClaw()
	{
		PlayerState = State.SpinningClaw;
		PlayAnimation("Spinning Claw", "On Spinning Claw");
		if (IsGrounded())
		{
			AirMotionVelocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (ShouldAlignOrFall(Align: true))
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		else
		{
			Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
			if (_Rigidbody.velocity.magnitude != 0f)
			{
				vector = base.transform.forward * CurSpeed;
				AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
			}
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		if (!SpinningClawSources[0].isPlaying)
		{
			SpinningClawSources[1].volume = 1f;
		}
		Vector3 force = _Rigidbody.velocity.normalized * CurSpeed;
		AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, force, 1);
		if (!Singleton<RInput>.Instance.P.GetButton("Button Y"))
		{
			SpinRelease = true;
		}
		if ((!Singleton<RInput>.Instance.P.GetButton("Button Y") && Time.time - SpinTime > Blaze_Lua.c_spinning_claw_min) || (Singleton<RInput>.Instance.P.GetButton("Button Y") && Time.time - SpinTime > Blaze_Lua.c_spinning_claw_max))
		{
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateSpinningClawEnd()
	{
		Audio.PlayOneShot(SpinningClawStopSound, Audio.volume);
		SpinningClawSources[0].Stop();
		SpinningClawSources[1].Stop();
	}

	private void StateCrowAttackStart()
	{
		PlayerState = State.CrowAttack;
		AttackTimer = Time.time;
		AttackSpeed = (IdleAttack ? Blaze_Lua.c_run_speed_max : CurSpeed);
		Audio.PlayOneShot(AttackSound, Audio.volume);
		PlayerVoice.PlayRandom(new int[3] { 2, 3, 6 }, RandomPlayChance: true);
		BlazeEffects.CreateCrowAttackTrailFX();
		PlayAnimation("Movement (Blend Tree)", "On Ground");
	}

	private void StateCrowAttack()
	{
		PlayerState = State.CrowAttack;
		PlayAnimation("Crow Attack", "On Crow Attack");
		AttackSpeed = (IdleAttack ? Mathf.MoveTowards(AttackSpeed, 0f, Time.fixedDeltaTime * 55f) : ((CurSpeed < 8f) ? 8f : AttackSpeed));
		CurSpeed = AttackSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position + base.transform.up * 0.25f, 0.9f, base.transform.forward * CurSpeed + base.transform.forward * 5f, 1);
		if (IsGrounded() && Time.time - AttackTimer > (IdleAttack ? 0.575f : 0.525f))
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateCrowAttackEnd()
	{
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Blaze_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Blaze_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Blaze_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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

	private void StateEdgeDangerStart()
	{
		PlayerState = State.EdgeDanger;
		EdgeDangerTime = Time.time;
	}

	private void StateEdgeDanger()
	{
		PlayerState = State.EdgeDanger;
		PlayAnimation("Edge Danger", "On Edge Danger");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		CurSpeed = 0f;
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - EdgeDangerTime < 1.5f)
		{
			LockControls = true;
		}
		else if (TargetDirection != Vector3.zero)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateEdgeDangerEnd()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState != State.Homing)
		{
			HomingTarget = FindHomingTarget();
		}
		if (PlayerState == State.Ground || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.SpinningClaw || PlayerState == State.CrowAttack || PlayerState == State.Brake || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Blaze_Lua.c_rotation_speed);
				AccelerationSystem((!HasSpeedUp) ? Blaze_Lua.c_run_acc : Blaze_Lua.c_speedup_acc);
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Blaze_Lua.c_walk_speed_max : Blaze_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Blaze_Lua.c_speedup_speed_max : (IsGrounded() ? Blaze_Lua.c_run_speed_max : Blaze_Lua.c_jump_run));
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
		if (PlayerState == State.Ground || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.SpinningClaw || PlayerState == State.CrowAttack || PlayerState == State.Brake || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState == GameManager.State.Paused || Singleton<GameManager>.Instance.GameState == GameManager.State.Result || StageManager.StageState == StageManager.State.Event || IsDead || PlayerState == State.Talk)
		{
			return;
		}
		if (!CanAccelJump && (IsGrounded() || PlayerState == State.Grinding || (PlayerState == State.Spring && LockControls) || (PlayerState == State.WideSpring && LockControls) || (PlayerState == State.JumpPanel && LockControls) || (PlayerState == State.RainbowRing && !LockControls)))
		{
			CanAccelJump = true;
		}
		if (PlayerState == State.Ground)
		{
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
			{
				StateMachine.ChangeState(StateJump);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking && !FrontalCollision)
			{
				OnFireClaw();
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button Y") && ShouldAlignOrFall(Align: false) && !IsSinking)
			{
				StateMachine.ChangeState(StateSpinningClaw);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && !IsSinking)
			{
				IdleAttack = CurSpeed == 0f;
				StateMachine.ChangeState(StateCrowAttack);
			}
		}
		if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			StateMachine.ChangeState(StateJump);
		}
		if (PlayerState == State.Air || PlayerState == State.SlowFall)
		{
			if (ReleasedKey && CanAccelJump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				CanAccelJump = false;
				StateMachine.ChangeState(StateAccelJump);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !FrontalCollision)
			{
				OnFireClaw();
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateSpinningClaw);
			}
		}
		if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
		{
			if (((ReleasedKey && PlayerState == State.Jump) || PlayerState != State.Jump) && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateAccelJump);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !FrontalCollision)
			{
				OnFireClaw();
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateSpinningClaw);
			}
		}
		if (PlayerState == State.AccelJump)
		{
			if (ReleasedAccelJump && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !FrontalCollision)
			{
				OnFireClaw();
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateSpinningClaw);
			}
		}
		if (PlayerState == State.AfterHoming)
		{
			if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				HAPressed = false;
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateAccelJump);
			}
			if (!HAPressed && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !FrontalCollision)
			{
				OnFireClaw();
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateSpinningClaw);
			}
		}
		if (PlayerState != State.SpinningClaw)
		{
			return;
		}
		if (SpinRelease && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
		{
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		if (IsGrounded() && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			StateMachine.ChangeState(StateJump);
		}
		if (!IsGrounded() && CanAccelJump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			CanAccelJump = false;
			StateMachine.ChangeState(StateAccelJump);
		}
	}

	private bool ShouldEdgeDanger()
	{
		if (ShouldAlignOrFall(Align: true) || !IsGrounded() || Physics.Linecast(base.transform.position + base.transform.up * 0.25f, base.transform.position + base.transform.up * 0.25f + base.transform.forward, base.Collision_Mask) || Physics.Raycast(base.transform.position + base.transform.up * 0.25f + base.transform.forward * 0.3f, -base.transform.up, out var _, 0.75f, base.Collision_Mask))
		{
			return false;
		}
		return true;
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump)
		{
			return 0;
		}
		if (PlayerState == State.Homing || PlayerState == State.AfterHoming || (PlayerState == State.AccelJump && ReachedApex) || PlayerState == State.SpinningClaw || PlayerState == State.CrowAttack)
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
			if (PlayerState != State.SpinningClaw)
			{
				BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && ((PlayerState == State.Ground && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
				BodyFwdDirDot = Mathf.Lerp(BodyFwdDirDot, 0f, 10f * Time.fixedDeltaTime);
			}
			else
			{
				BodyDirDot = Mathf.Lerp(BodyDirDot, UseCharacterSway ? ((0f - num) * (CurSpeed / 2f)) : 0f, 5f * Time.fixedDeltaTime);
				BodyFwdDirDot = Mathf.Lerp(BodyFwdDirDot, UseCharacterSway ? (CurSpeed / 2f) : 0f, 5f * Time.fixedDeltaTime);
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

	private bool OnFireClaw()
	{
		Audio.PlayOneShot(AttackSound, Audio.volume);
		PlayerVoice.PlayRandom(new int[3] { 2, 3, 6 }, RandomPlayChance: true);
		if (!HomingTarget)
		{
			StateMachine.ChangeState(StateFireClaw);
			BlazeEffects.OnClawTrailFX(Enable: true);
			return false;
		}
		StateMachine.ChangeState(StateHoming);
		BlazeEffects.OnClawTrailFX(Enable: true);
		return true;
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
