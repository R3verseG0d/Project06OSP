using System;
using STHLua;
using UnityEngine;

public class Knuckles : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		SlowFall = 4,
		Glide = 5,
		BallFall = 6,
		Climb = 7,
		ClimbUp = 8,
		Punch = 9,
		Uppercut = 10,
		Quake = 11,
		Screwdriver = 12,
		Homing = 13,
		AfterHoming = 14,
		Hurt = 15,
		Grinding = 16,
		Death = 17,
		FallDeath = 18,
		DrownDeath = 19,
		SnowBallDeath = 20,
		TornadoDeath = 21,
		Talk = 22,
		Path = 23,
		WarpHole = 24,
		Result = 25,
		Cutscene = 26,
		DashPanel = 27,
		Spring = 28,
		WideSpring = 29,
		JumpPanel = 30,
		DashRing = 31,
		RainbowRing = 32,
		UpReel = 33,
		Balancer = 34
	}

	[Header("Player Framework")]
	public KnucklesEffects KnucklesEffects;

	public Transform UppercutPoint;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private Vector3 FrontNormal;

	private bool CanGlide;

	private float GlideLockTime;

	private float LeftStickX;

	private float LeftStickY;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip HomingKickback;

	public AudioClip[] PunchSounds;

	public AudioClip QuakeMoveSound;

	public AudioClip[] ScrewdriverSounds;

	public AudioClip BrakeSound;

	[Header("Audio Sources")]
	public AudioSource[] GlideSources;

	public AudioSource ClimbSource;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private float GlideSpeed;

	private float GlideYGrav;

	private float GlideSpeedInc;

	private float ClimbUpWait;

	private RaycastHit ClimbHit;

	private float ClimbUpTime;

	private float ClimbUpSpd;

	private bool PunchDamage;

	private float PunchTimer;

	private float PunchSpeed;

	private int PunchCount;

	private int FinisherType;

	private float UppercutTime;

	internal int QuakeState;

	private float QuakeLaunchTime;

	private float ScrewDrillTime;

	internal int ScrewState;

	private bool FullCharge;

	internal Vector3 HomingDirection;

	private Vector3 DirectionToTarget;

	private Vector3 HAForward;

	private Vector3 HAStartPos;

	private float HAStartTime;

	private float HomingTime;

	private string HATag;

	private float AHEnterTime;

	private float HurtTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Knuckles_Lua.c_player_name;
		PlayerNameShort = Knuckles_Lua.c_player_name_short;
		WalkSpeed = Knuckles_Lua.c_walk_speed_max;
		TopSpeed = Knuckles_Lua.c_run_speed_max;
		BrakeSpeed = Knuckles_Lua.c_brake_acc;
		GrindSpeedOrg = Knuckles_Lua.c_grind_speed_org;
		GrindAcc = Knuckles_Lua.c_grind_acc;
		GrindSpeedMax = Knuckles_Lua.c_grind_speed_max;
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Knuckles_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Knuckles_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Knuckles_Lua.c_run_speed_max) / Knuckles_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Knuckles_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Knuckles_Lua.c_run_speed_max;
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
		Audio.PlayOneShot(JumpSound, Audio.volume);
		PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		JumpTime = Time.time;
		JumpAnimation = 0;
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Knuckles_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Knuckles_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Knuckles_Lua.c_jump_time_min) ? 1 : 0));
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
		CanClimb(AirMotionVelocity.y > 0f);
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

	private void StateGlideStart()
	{
		PlayerState = State.Glide;
		Animator.SetTrigger("On Glide Start");
		GlideSources[1].Stop();
		GlideSources[1].volume = 0f;
		GlideSources[0].Play();
		GlideSources[1].Play();
		GlideSpeed = CurSpeed;
		GlideYGrav = 0f;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 0f)
		{
			AirMotionVelocity.y = 0f;
		}
		GlideSpeedInc = Knuckles_Lua.c_flight_speed_max;
		_Rigidbody.velocity = AirMotionVelocity;
		KnucklesEffects.CreateGlideFX();
	}

	private void StateGlide()
	{
		PlayerState = State.Glide;
		PlayAnimation("Glide", "On Glide");
		GlideSpeedInc = Mathf.Lerp(GlideSpeedInc, Knuckles_Lua.c_flight_speed_max * ((TargetDirection.magnitude != 0f && GlideSpeed >= Knuckles_Lua.c_flight_speed_max) ? 1.5f : 1f), Time.fixedDeltaTime * 0.25f);
		if (TargetDirection.magnitude != 0f)
		{
			GlideSpeed += Knuckles_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		else if (GlideSpeed > Knuckles_Lua.c_flight_speed_min)
		{
			GlideSpeed -= Knuckles_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		GlideSpeed = Mathf.Clamp(GlideSpeed, Knuckles_Lua.c_flight_speed_min, GlideSpeedInc);
		GlideYGrav = Mathf.Lerp(GlideYGrav, (AirMotionVelocity.y < 0f) ? 2f : 8f, Time.fixedDeltaTime * 0.5f);
		CurSpeed = GlideSpeed;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= GlideYGrav * Time.fixedDeltaTime;
		if (AirMotionVelocity.y < 0f - GlideYGrav)
		{
			AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, 0f - GlideYGrav, Time.fixedDeltaTime * 6f);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		if (JumpAttackSphere(base.transform.position + base.transform.up * 0.5f, 0.9f, base.transform.forward * CurSpeed + base.transform.forward * 10f, 1))
		{
			GlideSpeed = Knuckles_Lua.c_flight_speed_min;
			AirMotionVelocity.y = 4f;
		}
		if (!GlideSources[0].isPlaying)
		{
			GlideSources[1].volume = 1f;
		}
		DoWallNormal();
		CanClimb();
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			StateMachine.ChangeState(StateBallFall);
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateGlideEnd()
	{
		Animator.SetTrigger("Additive Idle");
		GlideSources[0].Stop();
	}

	private void StateBallFallStart()
	{
		PlayerState = State.BallFall;
		JumpAnimation = 1;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateBallFall()
	{
		PlayerState = State.BallFall;
		PlayAnimation("Rolling", "On Roll");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, 1))
		{
			AirMotionVelocity.y = 12f;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		CanClimb(AirMotionVelocity.y > 0f);
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateBallFallEnd()
	{
	}

	private void StateClimbStart()
	{
		PlayerState = State.Climb;
		LeftStickX = 0f;
		LeftStickY = 0f;
		ClimbUpWait = Time.time;
	}

	private void StateClimb()
	{
		PlayerState = State.Climb;
		LockControls = true;
		PlayAnimation("Climb Move", "On Climb");
		CurSpeed = 0f;
		Physics.Raycast(base.transform.position + base.transform.up * 0.25f, base.transform.forward, out ClimbHit, 0.5f, base.FrontalCol_Mask);
		LeftStickX = Mathf.MoveTowards(LeftStickX, Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (Knuckles_Lua.c_climb_speed * 0.75f), Time.fixedDeltaTime * 50f);
		LeftStickY = Mathf.MoveTowards(LeftStickY, Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * (Knuckles_Lua.c_climb_speed * 0.75f), Time.fixedDeltaTime * 50f);
		Vector3 velocity = base.transform.right * LeftStickX + base.transform.up * LeftStickY;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(-base.transform.forward, ClimbHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = velocity;
		if (_Rigidbody.velocity == Vector3.zero && ClimbSource.isPlaying)
		{
			ClimbSource.Stop();
		}
		else if (_Rigidbody.velocity != Vector3.zero && !ClimbSource.isPlaying)
		{
			ClimbSource.Play();
		}
		if (ClimbHit.normal == Vector3.zero || ClimbHit.transform.gameObject.tag != "ClimbableWall" || 0.75f < ClimbHit.normal.y)
		{
			StateMachine.ChangeState(StateClimbUp);
		}
		else
		{
			base.transform.position = ClimbHit.point - (base.transform.up * 0.25f + base.transform.forward * 0.25f);
		}
		if (!Physics.Raycast(base.transform.position + base.transform.up * 0.5f, base.transform.forward, out ClimbHit, 0.5f, base.FrontalCol_Mask))
		{
			if (Time.time - ClimbUpWait > 0.1f)
			{
				StateMachine.ChangeState(StateClimbUp);
			}
			else
			{
				_Rigidbody.velocity = Vector3.zero;
			}
		}
		if (!Physics.Raycast(base.transform.position + -base.transform.right * 0.25f, base.transform.forward, out ClimbHit, 0.5f, base.FrontalCol_Mask) || !Physics.Raycast(base.transform.position + base.transform.right * 0.25f, base.transform.forward, out ClimbHit, 0.5f, base.FrontalCol_Mask))
		{
			StateMachine.ChangeState(StateAir);
		}
		if (IsGrounded() || -0.75f > ClimbHit.normal.y)
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateClimbEnd()
	{
		ClimbSource.Stop();
	}

	private void StateClimbUpStart()
	{
		PlayerState = State.ClimbUp;
		ClimbUpTime = Time.time;
		ClimbUpSpd = 3f;
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = 10f;
		_Rigidbody.velocity = AirMotionVelocity;
		PlayAnimation("Roll And Fall", "On Roll And Fall");
		Audio.PlayOneShot(JumpSound, Audio.volume);
	}

	private void StateClimbUp()
	{
		PlayerState = State.ClimbUp;
		LockControls = Time.time - ClimbUpTime < 0.25f;
		CurSpeed = ClimbUpSpd;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * ClimbUpSpd;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (Time.time - ClimbUpTime > 0.5f && IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateClimbUpEnd()
	{
	}

	private void StatePunchStart()
	{
		PlayerState = State.Punch;
		Animator.SetInteger("Punch ID", PunchCount);
		Animator.SetInteger("Finisher ID", FinisherType);
		Animator.SetTrigger("On Punch");
		PunchTimer = Time.time;
		PunchCount++;
		PunchSpeed = ((PunchCount != 3) ? Knuckles_Lua.c_run_speed_max : ((FinisherType == 0) ? Knuckles_Lua.c_run_speed_max : 0f)) * ((TargetDirection != Vector3.zero) ? 1f : 0.5f);
		PunchDamage = true;
		QueuedPress = false;
		Audio.PlayOneShot(PunchSounds[PunchCount], Audio.volume);
		PlayerVoice.PlayRandom((PunchCount == 1) ? 5 : ((PunchCount == 2) ? 6 : 7), RandomPlayChance: true);
		KnucklesEffects.CreatePunchFX((PunchCount == 1 || PunchCount != 2) ? 1 : 0, PunchCount);
	}

	private void StatePunch()
	{
		PlayerState = State.Punch;
		LockControls = PunchCount > 2 && (((FinisherType != 0 || !(Time.time - PunchTimer < 0.6f)) && FinisherType != 1) ? true : false);
		if (PunchCount < 3)
		{
			PunchSpeed = Mathf.MoveTowards(PunchSpeed, 0f, Time.fixedDeltaTime * 27.5f);
			if (PunchDamage && AttackSphere(base.transform.position + base.transform.up * 0.25f, (PunchCount == 1) ? 0.9f : 1.3f, base.transform.forward * CurSpeed + base.transform.forward * 5f, 1))
			{
				PunchDamage = false;
			}
		}
		else
		{
			if (FinisherType == 0)
			{
				PunchSpeed = ((Time.time - PunchTimer < 0.6f) ? Mathf.Lerp(PunchSpeed, 0f, Time.fixedDeltaTime * 2f) : 0f);
			}
			else if (Time.time - PunchTimer < 0.25f)
			{
				PunchSpeed = -2f;
			}
			else
			{
				PunchSpeed = ((Time.time - PunchTimer < 0.5f) ? (Knuckles_Lua.c_run_speed_max * 2f) : Mathf.Lerp(PunchSpeed, 0f, Time.fixedDeltaTime * 25f));
				AttackSphere(base.transform.position + base.transform.up * 0.25f, 1.5f, base.transform.forward * CurSpeed + base.transform.forward * 5f, 1);
			}
			if (PunchDamage && Time.time - PunchTimer > ((FinisherType == 0) ? 0.6f : 0.25f))
			{
				PunchDamage = false;
				if (FinisherType == 0)
				{
					Camera.PlayShakeMotion(0f, 0.25f, FastShake: true);
					AttackSphere_Dir(base.transform.position + base.transform.up * -0.25f + base.transform.forward * 0.15f, 5f, 5f, 3);
				}
				KnucklesEffects.CreateFinisherFX(FinisherType);
			}
		}
		CurSpeed = PunchSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (Time.time - PunchTimer > ((PunchCount != 3) ? ((PunchCount == 1) ? 0.35f : 0.5f) : ((FinisherType == 0) ? 1f : 0.65f)))
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StatePunchEnd()
	{
	}

	private void StateUppercutStart()
	{
		PlayerState = State.Uppercut;
		Animator.SetInteger("Punch ID", 2);
		Animator.SetInteger("Finisher ID", 2);
		Animator.SetTrigger("On Punch");
		UppercutTime = Time.time;
		Audio.PlayOneShot(PunchSounds[3], Audio.volume);
		PlayerVoice.PlayRandom(7, RandomPlayChance: true);
		KnucklesEffects.CreatePunchFX(1, 3);
		KnucklesEffects.CreateFinisherFX(2);
		CurSpeed = Knuckles_Lua.c_run_speed_max * 1.5f * ((TargetDirection != Vector3.zero) ? 1f : 0.5f);
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = 7.5f;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateUppercut()
	{
		PlayerState = State.Uppercut;
		if (Time.time - UppercutTime < 0.55f)
		{
			CurSpeed = Mathf.Lerp(CurSpeed, 0f, Time.fixedDeltaTime * 5f);
		}
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - UppercutTime > 0.3f)
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (AirMotionVelocity.y > 0f)
		{
			AttackSphere_Dir(UppercutPoint.position, 1.5f, 5f, 1);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (Time.time - UppercutTime > 0.55f)
		{
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
				DoLandAnim();
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
			}
			if (FrontalCollision)
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateUppercutEnd()
	{
		Animator.SetTrigger("Additive Idle");
	}

	private void StateQuakeStart()
	{
		PlayerState = State.Quake;
		QuakeState = 0;
		Audio.PlayOneShot(QuakeMoveSound, Audio.volume);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateQuake()
	{
		PlayerState = State.Quake;
		LockControls = QuakeState == 1;
		PlayAnimation((QuakeState == 0) ? "Quake Down" : "Quake End", (QuakeState == 0) ? "On Quake" : "On Quake End");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (QuakeState == 0)
		{
			AirMotionVelocity.y = -25f;
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				QuakeLaunchTime = Time.time;
				StunSphere(base.transform.position + base.transform.up * -0.25f, 7f, AffectObjs: true);
				KnucklesEffects.CreateQuakeFX();
				Audio.Stop();
				QuakeState = 1;
			}
			else
			{
				AttackSphere_Dir(base.transform.position, 1f, 5f, 1);
			}
		}
		else
		{
			CurSpeed = 0f;
			AirMotionVelocity.y = 0f;
			if (Time.time - QuakeLaunchTime > 0.6f)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		GeneralMeshRotation = ((QuakeState == 1) ? Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation) : (Quaternion.LookRotation(_Rigidbody.velocity.normalized) * Quaternion.Euler(-90f, 0f, 0f)));
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
	}

	private void StateQuakeEnd()
	{
	}

	private void StateScrewdriverStart()
	{
		PlayerState = State.Screwdriver;
		PlayAnimation("Screw Charge", "On Screw Charge");
		ScrewDrillTime = Time.time;
		ScrewState = 0;
		Audio.PlayOneShot(ScrewdriverSounds[0], Audio.volume);
		PlayerVoice.PlayRandom(8, RandomPlayChance: true);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		FullCharge = false;
	}

	private void StateScrewdriver()
	{
		PlayerState = State.Screwdriver;
		LockControls = ScrewState == 0;
		CurSpeed = ((ScrewState == 0) ? 0f : Knuckles_Lua.c_homing_spd);
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (ScrewState == 0)
		{
			AirMotionVelocity.y = 0f;
			if (Time.time - ScrewDrillTime > 0.125f && !Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				if (FrontalCollision)
				{
					StateMachine.ChangeState(StateGround);
				}
				else
				{
					ScrewDrillTime = Time.time;
					Audio.Stop();
					Audio.PlayOneShot(ScrewdriverSounds[1], Audio.volume * 0.6f);
					PlayerVoice.PlayRandom(7, RandomPlayChance: true);
					OnScrewDriver();
					ScrewState = 1;
				}
			}
			if (!FullCharge && Time.time - ScrewDrillTime > 1f)
			{
				Singleton<AudioManager>.Instance.PlayClip(ScrewdriverSounds[2], 1.25f);
				FullCharge = true;
			}
		}
		else
		{
			PlayAnimation("Screw", "On Screw");
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, base.transform.forward * CurSpeed + base.transform.forward * 10f, 1);
			if (Time.time - ScrewDrillTime < 0.25f)
			{
				AirMotionVelocity.y = 8f;
			}
			else
			{
				AirMotionVelocity.y -= 15f * Time.fixedDeltaTime;
				if (IsGrounded())
				{
					StateMachine.ChangeState(StateGround);
					PlayerManager.PlayerEvents.CreateLandFXAndSound();
				}
			}
			if (Time.time - ScrewDrillTime > Knuckles_Lua.c_homing_time || FrontalCollision)
			{
				StateMachine.ChangeState(StateBallFall);
			}
		}
		GeneralMeshRotation = ((ScrewState == 0) ? Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation) : Quaternion.LookRotation(_Rigidbody.velocity.normalized));
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		CanClimb();
		DoWallNormal();
	}

	private void StateScrewdriverEnd()
	{
	}

	private void StateHomingStart()
	{
		PlayerState = State.Homing;
		MaxRayLenght = 0f;
		DirectionToTarget = HomingTarget.transform.position + HomingTarget.transform.up * -0.25f - base.transform.position;
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
		PlayAnimation("Screw", "On Screw");
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
		_Rigidbody.MovePosition(Vector3.Lerp(HAStartPos, HomingTarget.transform.position + HomingTarget.transform.up * -0.25f, num2));
		if (AttackSphere(base.transform.position + base.transform.up * 0.25f + base.transform.forward * 0.25f, Knuckles_Lua.c_collision_homing(), DirectionToTarget * Knuckles_Lua.c_homing_power, Knuckles_Lua.c_homing_damage + (FullCharge ? 10 : 0)))
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
	}

	private void StateAfterHomingStart()
	{
		PlayerState = State.AfterHoming;
		MaxRayLenght = 0.75f;
		AHEnterTime = Time.time;
		Audio.PlayOneShot(HomingKickback, Audio.volume);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAfterHoming()
	{
		PlayerState = State.AfterHoming;
		PlayAnimation("Flip", "On After Homing");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - AHEnterTime <= 0.25f)
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

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		HurtTime = Time.time;
		PlayerVoice.PlayRandom(4);
	}

	public void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Knuckles_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Knuckles_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Knuckles_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		if (PlayerState == State.Screwdriver && ScrewState != 1)
		{
			HomingTarget = FindHomingTarget();
		}
		FrontNormal = FrontalHit.normal;
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Punch || PlayerState == State.Quake || (PlayerState == State.Screwdriver && ScrewState == 0) || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Knuckles_Lua.c_rotation_speed, Override: false, PlayerState == State.Glide);
				if (PlayerState != State.Glide)
				{
					AccelerationSystem((!HasSpeedUp) ? Knuckles_Lua.c_run_acc : Knuckles_Lua.c_speedup_acc);
				}
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Knuckles_Lua.c_walk_speed_max : Knuckles_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Knuckles_Lua.c_speedup_speed_max : (IsGrounded() ? Knuckles_Lua.c_run_speed_max : Knuckles_Lua.c_jump_run));
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
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Punch || PlayerState == State.Quake || (PlayerState == State.Screwdriver && ScrewState == 0) || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (PlayerState == State.Ground)
			{
				CanGlide = true;
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
				{
					StateMachine.ChangeState(StateJump);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					PunchCount = 0;
					StateMachine.ChangeState(StatePunch);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateScrewdriver);
				}
			}
			if (PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.BallFall || (PlayerState == State.Uppercut && Time.time - UppercutTime > 0.55f) || PlayerState == State.AfterHoming || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
			{
				if (((PlayerState == State.Jump && ReleasedKey) || PlayerState != State.Jump) && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && (CanGlide || (!CanGlide && Time.time - GlideLockTime > 0.3f)))
				{
					CanGlide = false;
					GlideLockTime = Time.time;
					StateMachine.ChangeState(StateGlide);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateQuake);
				}
			}
			if (PlayerState == State.Glide && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				StateMachine.ChangeState(StateQuake);
			}
			if (PlayerState == State.Climb && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				if (Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") < -0f)
				{
					base.transform.forward = -base.transform.forward;
				}
				CurSpeed = Knuckles_Lua.c_run_speed_max / 1.5f;
				StateMachine.ChangeState(StateJump);
			}
			if (PlayerState == State.ClimbUp && Time.time - ClimbUpTime > 0.25f)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateGlide);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateQuake);
				}
			}
			if (PlayerState == State.Punch)
			{
				if (!QueuedPress)
				{
					if (PunchCount < 2)
					{
						if (OnButtonPressed("Button X"))
						{
							QueuedPress = Time.time - PunchTimer > 0.1f;
						}
					}
					else if (OnButtonPressed("Button X") || OnButtonPressed("Button B") || OnButtonPressed("Button Y"))
					{
						QueuedPress = Time.time - PunchTimer > 0.175f;
					}
				}
				if (Time.time - PunchTimer > ((PunchCount < 2) ? 0.2f : 0.375f) && ShouldAlignOrFall(Align: false) && PunchCount < 3)
				{
					if (PunchCount < 2 && OnCombo("Button X"))
					{
						StateMachine.ChangeState(StatePunch);
					}
					else if (PunchCount > 1)
					{
						if (OnCombo("Button X"))
						{
							FinisherType = 0;
							StateMachine.ChangeState(StatePunch);
						}
						else if (OnCombo("Button B"))
						{
							FinisherType = 1;
							StateMachine.ChangeState(StatePunch);
						}
						else if (OnCombo("Button Y"))
						{
							StateMachine.ChangeState(StateUppercut);
						}
					}
				}
				if (Time.time - PunchTimer > 0.1f && PunchCount < 3 && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
			}
			else if (PunchCount != 0)
			{
				PunchCount = 0;
			}
			if (PlayerState == State.Quake)
			{
				if (QuakeState == 1)
				{
					if (Time.time - QuakeLaunchTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
					{
						StateMachine.ChangeState(StateJump);
					}
					if (Time.time - QuakeLaunchTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
					{
						PunchCount = 0;
						StateMachine.ChangeState(StatePunch);
					}
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && (CanGlide || (!CanGlide && Time.time - GlideLockTime > 0.3f)))
				{
					CanGlide = false;
					GlideLockTime = Time.time;
					StateMachine.ChangeState(StateGlide);
				}
			}
		}
		GlideSources[1].volume = ((PlayerState == State.Glide) ? 1f : Mathf.Lerp(GlideSources[1].volume, 0f, Time.deltaTime * 2f));
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump || PlayerState == State.Glide || PlayerState == State.BallFall)
		{
			return 0;
		}
		if (PlayerState == State.Punch || PlayerState == State.Uppercut || PlayerState == State.Quake || PlayerState == State.Screwdriver || PlayerState == State.Homing || PlayerState == State.AfterHoming)
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

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		Animator.SetFloat("Climb X", LeftStickX);
		Animator.SetFloat("Climb Y", LeftStickY);
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Glide) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * ((PlayerState == State.Glide) ? 30f : num2)) : 0f, 10f * Time.fixedDeltaTime);
			BodyTransform.localEulerAngles = new Vector3(BodyTransform.localEulerAngles.x, BodyTransform.localEulerAngles.y, BodyDirDot);
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

	private bool OnScrewDriver()
	{
		if (HomingTarget != null)
		{
			StateMachine.ChangeState(StateHoming);
			return true;
		}
		return false;
	}

	private void CanClimb(bool CannotAttach = false)
	{
		if (!CannotAttach && FrontalCollision && FrontalHit.transform.tag == "ClimbableWall" && FrontNormal != Vector3.zero && 0.75f > FrontNormal.y && -0.75f < FrontNormal.y)
		{
			StateMachine.ChangeState(StateClimb);
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
		if (!IsDead && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole)
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
