using System;
using STHLua;
using UnityEngine;

public class SonicNew : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		JumpDash = 4,
		JumpDashSTH = 5,
		LightDash = 6,
		SlowFall = 7,
		Homing = 8,
		AfterHoming = 9,
		Kick = 10,
		Slide = 11,
		TrickJump = 12,
		GetUpA = 13,
		BoundAttack = 14,
		SpinDash = 15,
		Hurt = 16,
		EdgeDanger = 17,
		WaterSlide = 18,
		Grinding = 19,
		Death = 20,
		FallDeath = 21,
		DrownDeath = 22,
		SnowBallDeath = 23,
		TornadoDeath = 24,
		Talk = 25,
		Path = 26,
		WarpHole = 27,
		Result = 28,
		Tornado = 29,
		GunDrive = 30,
		GunDriveMove = 31,
		HomingSmash = 32,
		Transform = 33,
		LightAttack = 34,
		Cutscene = 35,
		DashPanel = 36,
		Spring = 37,
		WideSpring = 38,
		JumpPanel = 39,
		DashRing = 40,
		RainbowRing = 41,
		ChainJump = 42,
		Orca = 43,
		Pole = 44,
		Float = 45,
		Rope = 46,
		Hold = 47,
		UpReel = 48,
		Balancer = 49
	}

	public enum Gem
	{
		None = 0,
		Blue = 1,
		Red = 2,
		Green = 3,
		Purple = 4,
		Sky = 5,
		White = 6,
		Yellow = 7,
		Rainbow = 8
	}

	[Header("Player Framework")]
	public SonicEffects SonicEffects;

	public State PlayerState;

	internal Gem ActiveGem;

	internal Vector3 AirMotionVelocity;

	private bool UsingDPad;

	private int[] AttackState = new int[2];

	private float[] HoldTime = new float[2];

	private string LaunchMode;

	[Header("Player Models")]
	public RuntimeAnimatorController TGSAnimator;

	public SkinnedMeshRenderer[] PlayerRenderers;

	public UpgradeModels Upgrades;

	private float BodyDirDot;

	private float BlinkTimer;

	[Header("Gems")]
	public GameObject HomingSmashChain;

	public GameObject ThunderShield;

	public Transform HandPoint;

	public GameObject HandItem;

	internal GameData.GlobalData GemData;

	internal GameObject ThunderShieldObject;

	internal GameObject SkyGemObject;

	internal int GemSelector;

	internal int ObtainedGemIndex;

	internal int JumpLimit;

	internal bool IsSuper;

	internal bool UsingBlueGem;

	internal bool UsingRedGem;

	internal bool UsingGreenGem;

	internal bool UsingPurpleGem;

	internal bool UsingSkyGem;

	internal bool UsingWhiteGem;

	internal bool UsingYellowGem;

	internal bool UsingRainbowGem;

	private bool CanGunDrive;

	private float MachTimer;

	private float JumpBlock;

	private float RingDrainTimer;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip JumpDashSound;

	public AudioClip JumpDashSTHSound;

	public AudioClip JumpDashKickback;

	public AudioClip SpinDashChargeAdventure;

	public AudioClip SpinDashShoot;

	public AudioClip SpinDashShootAdventure;

	public AudioClip SpinDashShootAdventure2;

	public AudioClip SlideSound;

	public AudioClip BoundStart;

	public AudioClip BrakeSound;

	public AudioClip GemChange;

	public AudioClip HomingSmashLoop;

	public AudioClip HomingSmashShoot;

	public AudioClip SuperSound;

	[Header("Audio Sources")]
	public AudioSource[] SpinDashSources;

	public AudioSource WaterRunSource;

	public AudioSource[] GunDriveSources;

	public AudioSource[] Jump123Sources;

	public AudioSource SuperAuraSource;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool PurpleGemJump;

	private float JumpDashStartTime;

	private float JumpDashLength;

	private float JumpDashSpeed;

	internal bool CanJumpdash;

	private bool CanFallJumpdash;

	private float JumpDashSTHStartTime;

	private float JumpDashSTHLength;

	private BezierCurve LDBezierCurves;

	private Vector3 LDStartDirection;

	private Vector3 LDTargetDirection;

	private Vector3 LDStartPosition;

	private Vector3 LDTargetPosition;

	private bool GroundDash;

	private float LDSplineLength;

	private float SplineTime;

	private float LDStartTime;

	private float LDDistance;

	private float MinTime;

	private float MaxTime;

	private float Speed;

	private int LDState;

	private int LDDirection;

	private Vector3 HomingDirection;

	private Vector3 DirectionToTarget;

	private Vector3 HAForward;

	private Vector3 HAStartPos;

	private float HAStartTime;

	private float HomingTime;

	private string HATag;

	private float AHEnterTime;

	private bool HAPressed;

	private int AfterHomingTrick;

	private float KickTime;

	internal float FirstKickSpeed;

	private float SlideEndTimer;

	internal float SlideSpeed;

	private bool UsedSlide;

	private float TrickJumpTime;

	private bool ReleasedTrickKey;

	internal int JumpType;

	private float GetUpTime;

	internal int BoundState;

	private int BoundLevel;

	private float BoundHeight;

	private float BoundTime;

	private float SpindashTime;

	private float SpindashSpd;

	internal int SpinDashState;

	private float HurtTime;

	private float EdgeDangerTime;

	private Vector3 WSDirection;

	private BezierCurve WSSpline;

	private float WSpeed;

	private float WSTime;

	private float WSPositionShift;

	private float WSSmoothPos;

	internal bool GroundTornado;

	internal bool SpawnedAirTornado;

	private float TornadoTimer;

	private float NextTornadoHit;

	private float TornadoHitFreq;

	private float TornadoRadius;

	private float TornadoForce;

	private bool GunDriveSnipe;

	internal bool ThrewGem;

	internal int GunDriveState;

	private float GunDriveTimer;

	private float ThrowPower;

	private Vector3 DriveVelocity;

	private Vector3 DrivePosition;

	private Vector3 DriveStartDirection;

	private Vector3 DrivePlaneDir;

	private Quaternion DriveMeshLaunchRot;

	private bool MoveEnded;

	internal bool GunDriveAttack;

	private float DriveMoveTimer;

	private float DriveMoveSpeed;

	private float DriveStartTime;

	private bool HasTransformed;

	private bool TransformGrounded;

	private float TransformTimer;

	internal bool HasDashed;

	private float LATimer;

	private bool ThunderguardHurt;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Sonic_New_Lua.c_player_name;
		PlayerNameShort = Sonic_New_Lua.c_player_name_short;
		WalkSpeed = Sonic_New_Lua.c_walk_speed_max;
		TopSpeed = Sonic_New_Lua.c_run_speed_max;
		BrakeSpeed = Sonic_New_Lua.c_brake_acc;
		GrindSpeedOrg = Sonic_New_Lua.c_grind_speed_org;
		GrindAcc = Sonic_New_Lua.c_grind_acc;
		GrindSpeedMax = Sonic_New_Lua.c_grind_speed_max;
	}

	public override void Start()
	{
		base.Start();
		GemData = Singleton<GameManager>.Instance.GetGameData();
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Sonic_New_Lua.OpenGauge(), Sonic_New_Lua.c_gauge_max, Sonic_New_Lua.c_gauge_heal, Sonic_New_Lua.c_gauge_heal_delay);
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
		if (TargetDirection.magnitude == 0f && !IsSuper && !UsingBlueGem)
		{
			if (Time.time - IdleTimer > 7.5f && !IdleAnimPlayed)
			{
				IdleAnimPlayed = true;
				IdleTimer = Time.time;
				PlayIdleEvent(3);
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Sonic_New_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Sonic_New_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Sonic_New_Lua.c_run_speed_max) / Sonic_New_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Sonic_New_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Sonic_New_Lua.c_run_speed_max;
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
		PurpleGemJump = UsingPurpleGem;
		AirMotionVelocity = _Rigidbody.velocity;
		if (PurpleGemJump)
		{
			AirMotionVelocity.y = 0f;
		}
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Sonic_New_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Sonic_New_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
		JumpBlock = Time.time;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Sonic_New_Lua.c_jump_time_min) ? 1 : 0));
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!PurpleGemJump && !HalveSinkJump) ? 0.7f : 0.45f))
		{
			float num = ((!IsSuper || HUD.ActiveGemLevel[GemSelector] <= 0) ? ((!HalveSinkJump) ? 4.25f : 3f) : ((!HalveSinkJump) ? 6.25f : 5f));
			AirMotionVelocity += Vector3.up * num * Time.fixedDeltaTime * 4f;
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

	private void StateJumpDashStart()
	{
		PlayerState = State.JumpDash;
		MaxRayLenght = 0.75f;
		JumpDashLength = ((Singleton<Settings>.Instance.settings.JumpdashType == 0) ? Sonic_New_Lua.c_homing_time : Sonic_New_Lua.c_homing_e3_time);
		JumpDashStartTime = Time.time;
		JumpDashSpeed = Sonic_New_Lua.c_homing_spd;
		if (Singleton<Settings>.Instance.settings.JumpdashType == 1)
		{
			AirMotionVelocity = _Rigidbody.velocity;
			AirMotionVelocity.y = 0f;
			_Rigidbody.velocity = AirMotionVelocity;
		}
	}

	private void StateJumpDash()
	{
		PlayerState = State.JumpDash;
		PlayAnimation("Rolling", "On Roll");
		_ = (Time.time - JumpDashStartTime) / JumpDashLength;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		JumpDashSpeed -= Sonic_New_Lua.c_homing_brake * 0.75f * Time.deltaTime;
		CurSpeed = JumpDashSpeed;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Singleton<Settings>.Instance.settings.JumpdashType == 0)
		{
			AirMotionVelocity.y = 0.001f;
		}
		else
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		_Rigidbody.velocity = AirMotionVelocity;
		if (Time.time - JumpDashStartTime > JumpDashLength || FrontalCollision)
		{
			StateMachine.ChangeState(StateAir);
		}
		int num = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 2 : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 3 : 4));
		if (HomingAttackSphere(base.transform.position, Sonic_New_Lua.c_collision_homing(), base.transform.forward * CurSpeed, IsSuper ? 10 : (UsingWhiteGem ? num : Sonic_New_Lua.c_homing_damage), "SetChainProperties", (!UsingWhiteGem) ? 5 : HUD.ActiveGemLevel[GemSelector]))
		{
			if (UsingWhiteGem)
			{
				SonicEffects.CreateHomingSmashFX();
			}
			StateMachine.ChangeState(StateAfterHoming);
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateJumpDashEnd()
	{
		UsingWhiteGem = false;
	}

	private void StateJumpDashSTHStart()
	{
		PlayerState = State.JumpDashSTH;
		MaxRayLenght = 0.75f;
		JumpDashSTHLength = 0.4f;
		JumpDashSTHStartTime = Time.time;
		CurSpeed = Sonic_New_Lua.c_homing_spd;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateJumpDashSTH()
	{
		PlayerState = State.JumpDashSTH;
		PlayAnimation("Jumpdash", "On Jumpdash");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		_Rigidbody.velocity = AirMotionVelocity;
		if (Time.time - JumpDashSTHStartTime > JumpDashSTHLength || FrontalCollision)
		{
			CurSpeed *= 0.75f;
			StateMachine.ChangeState(StateAir);
		}
		int num = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 2 : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 3 : 4));
		if (HomingAttackSphere(base.transform.position, Sonic_New_Lua.c_collision_homing(), base.transform.forward * Sonic_New_Lua.c_homing_spd, IsSuper ? 10 : (UsingWhiteGem ? num : Sonic_New_Lua.c_homing_damage), "SetChainProperties", (!UsingWhiteGem) ? 5 : HUD.ActiveGemLevel[GemSelector]))
		{
			if (UsingWhiteGem)
			{
				SonicEffects.CreateHomingSmashFX();
			}
			StateMachine.ChangeState(StateAfterHoming);
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			CurSpeed *= 0.75f;
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateJumpDashSTHEnd()
	{
		UsingWhiteGem = false;
	}

	private void StateLightDashStart()
	{
		PlayerState = State.LightDash;
		MaxRayLenght = 2.75f;
		int num = ClosestLightDashRing();
		LDBezierCurves = GetRingSpline(num);
		float splineTime = LightDashRings[num].GetComponent<Ring>().SplineTime;
		GroundDash = LightDashRings[num].GetComponent<Ring>().GroundLightDash;
		LDStartDirection = base.transform.forward;
		LDTargetDirection = LDBezierCurves.GetTangent(splineTime);
		LDDirection = ((Vector3.Dot(LDStartDirection, LDTargetDirection) >= 0f) ? 1 : (-1));
		LDSplineLength = LDBezierCurves.Length();
		splineTime = (SplineTime = ((LDDirection != 1) ? Mathf.Min(splineTime + 0.5f / LDSplineLength, 1f) : Mathf.Max(splineTime - 0.5f / LDSplineLength, 0f)));
		LDState = 0;
		LDTargetDirection *= (float)LDDirection * 1f;
		LDStartPosition = base.transform.position;
		LDTargetPosition = LDBezierCurves.GetPosition(splineTime);
		if (GroundDash)
		{
			LDTargetPosition = LightDashGroundPos(LDTargetPosition);
		}
		LDDistance = Vector3.Distance(LDStartPosition, LDTargetPosition);
		Speed = CurSpeed;
		LDStartTime = Time.time;
		MinTime = 0f;
		MaxTime = 1f;
	}

	private void StateLightDash()
	{
		PlayerState = State.LightDash;
		PlayAnimation("Light Dash", "On Light Dash");
		if (LDTargetDirection == Vector3.zero)
		{
			StateMachine.ChangeState(StateGround);
		}
		_Rigidbody.velocity = Vector3.zero;
		Speed = Mathf.Min(Speed + Sonic_New_Lua.c_lightdash_speed * Time.fixedDeltaTime * 10f, Sonic_New_Lua.c_lightdash_speed);
		CurSpeed = Speed;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		if (LDState == 0)
		{
			float num = (Time.time - LDStartTime) / LDDistance * Speed;
			if (num > 1f)
			{
				LDState = 1;
			}
			base.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(LDStartDirection, LDTargetDirection, num), GroundDash ? RaycastHit.normal : Vector3.up);
			_Rigidbody.MovePosition(Vector3.Lerp(LDStartPosition, LDTargetPosition, num));
			return;
		}
		SplineTime += ((LDDirection == 1) ? Time.fixedDeltaTime : (0f - Time.fixedDeltaTime)) / LDSplineLength * Speed;
		float splineTime = SplineTime;
		Vector3 vector = LDBezierCurves.GetTangent(splineTime) * ((float)LDDirection * 1f);
		if (LastLightDashRing(vector, out var Index))
		{
			Ring component = LightDashRings[Index].GetComponent<Ring>();
			if (LDDirection == 1)
			{
				MaxTime = Mathf.Max(component.SplineTime + 1f / LDSplineLength, 1f);
			}
			else
			{
				MinTime = Mathf.Max(component.SplineTime - 1f / LDSplineLength, 0f);
			}
		}
		if (splineTime < MinTime || splineTime > MaxTime)
		{
			if (IsGrounded())
			{
				CurSpeed = (Sonic_New_Lua.c_run_speed_max + Sonic_New_Lua.c_lightdash_speed) / 1.75f;
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(base.StateSlowFall);
			}
		}
		base.transform.rotation = Quaternion.LookRotation(vector, GroundDash ? RaycastHit.normal : Vector3.up);
		Vector3 vector2 = LDBezierCurves.GetPosition(splineTime);
		if (GroundDash)
		{
			vector2 = LightDashGroundPos(vector2);
		}
		AttackSphere(base.transform.position + base.transform.up * 0.25f, 0.75f, base.transform.forward * (Sonic_New_Lua.c_lightdash_speed / 2f), 1);
		_Rigidbody.MovePosition(vector2);
	}

	private void StateLightDashEnd()
	{
		MaxRayLenght = 0.75f;
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
		PlayAnimation("Rolling", "On Roll");
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
		int num3 = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 2 : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 3 : 4));
		if (HomingAttackSphere(base.transform.position + base.transform.forward * 0.25f, Sonic_New_Lua.c_collision_homing(), DirectionToTarget * Sonic_New_Lua.c_homing_power, IsSuper ? 10 : (UsingWhiteGem ? num3 : Sonic_New_Lua.c_homing_damage), "SetChainProperties", (!UsingWhiteGem) ? 5 : HUD.ActiveGemLevel[GemSelector]))
		{
			if (UsingWhiteGem)
			{
				SonicEffects.CreateHomingSmashFX();
			}
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
		UsingWhiteGem = false;
		if (PlayerState != State.Grinding)
		{
			HomingDirection = Vector3.zero;
		}
	}

	private void StateAfterHomingStart()
	{
		PlayerState = State.AfterHoming;
		MaxRayLenght = 0.75f;
		if (AfterHomingTrick > 2)
		{
			AfterHomingTrick = 0;
		}
		Animator.SetInteger("HomingAnimID", AfterHomingTrick);
		Audio.PlayOneShot(JumpDashKickback, Audio.volume);
		PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		AHEnterTime = Time.time;
		HAPressed = true;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		UsingWhiteGem = false;
	}

	private void StateAfterHoming()
	{
		PlayerState = State.AfterHoming;
		Animator.SetTrigger("On After Homing");
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
		AfterHomingTrick++;
	}

	private void StateKickStart()
	{
		PlayerState = State.Kick;
		KickTime = Time.time;
		FirstKickSpeed = CurSpeed;
		PlayerVoice.PlayRandom(6, RandomPlayChance: true);
		SonicEffects.CreateKickAttacFX();
		PlayAnimation("Movement (Blend Tree)", "On Ground");
	}

	private void StateKick()
	{
		PlayerState = State.Kick;
		PlayAnimation("Kick Attack", "On Kick Attack");
		if (TargetDirection == Vector3.zero && !IsSlopePhys)
		{
			FirstKickSpeed -= Sonic_New_Lua.c_run_speed_max * Time.fixedDeltaTime;
		}
		FirstKickSpeed = Mathf.Clamp(FirstKickSpeed, 0f, FirstKickSpeed);
		CurSpeed = FirstKickSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position - base.transform.up * 0.25f, 0.9f, base.transform.forward * CurSpeed + base.transform.forward * 20f, 1);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (Time.time - KickTime > 0.6f || !IsGrounded() || FrontalCollision)
		{
			if (_Rigidbody.velocity.magnitude <= 0.1f)
			{
				_Rigidbody.velocity = Vector3.zero;
				StateMachine.ChangeState(StateGetUpA);
			}
			else
			{
				StateMachine.ChangeState(StateGround);
			}
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

	private void StateKickEnd()
	{
	}

	private void StateSlideStart()
	{
		PlayerState = State.Slide;
		Audio.PlayOneShot(SlideSound, Audio.volume);
		PlayerVoice.PlayRandom(7, RandomPlayChance: true);
		SlideEndTimer = Time.time;
		SlideSpeed = CurSpeed;
		UsedSlide = true;
	}

	private void StateSlide()
	{
		PlayerState = State.Slide;
		PlayAnimation("Sliding Attack", "On Sliding Attack");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		SlideSpeed += Vector3.Dot(new Vector3(0f, -0.5f, 0f), base.transform.forward);
		SlideSpeed = Mathf.Clamp(SlideSpeed, Sonic_New_Lua.c_sliding_speed_min, Sonic_New_Lua.c_run_speed_max * 1.5f);
		CurSpeed = SlideSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position, Sonic_New_Lua.c_sliding_collision.radius, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, Sonic_New_Lua.c_sliding_damage);
		if (Time.time - SlideEndTimer > Sonic_New_Lua.c_sliding_time)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || (SlideSpeed <= Sonic_New_Lua.c_sliding_speed_min && ShouldAlignOrFall(Align: true)))
		{
			StateMachine.ChangeState(StateAir);
		}
		if (FrontalCollision)
		{
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateGetUpA);
		}
	}

	private void StateSlideEnd()
	{
	}

	private void StateTrickJumpStart()
	{
		PlayerState = State.TrickJump;
		Audio.PlayOneShot(JumpSound, Audio.volume);
		PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		TrickJumpTime = Time.time;
		ReleasedTrickKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Sonic_New_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Sonic_New_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
		Animator.SetInteger("Jump Type", JumpType);
		Animator.SetTrigger("On Trick Jump");
	}

	private void StateTrickJump()
	{
		PlayerState = State.TrickJump;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y, 12f);
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedTrickKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedTrickKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - TrickJumpTime < 0.8f)
		{
			float num = ((IsSuper && HUD.ActiveGemLevel[GemSelector] > 0) ? 6.25f : 4.25f);
			AirMotionVelocity += Vector3.up * num * Time.fixedDeltaTime * 4f;
		}
		if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, 1))
		{
			TrickJumpTime = Time.time;
			AirMotionVelocity.y = 12f;
		}
		MaxRayLenght = ((Time.time - TrickJumpTime > 0.25f) ? 0.75f : 0f);
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

	private void StateTrickJumpEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateGetUpAStart()
	{
		PlayerState = State.GetUpA;
		GetUpTime = Time.time;
	}

	private void StateGetUpA()
	{
		PlayerState = State.GetUpA;
		PlayAnimation("Get Up A", "On Get Up A");
		LockControls = true;
		_Rigidbody.velocity = Vector3.zero;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		CurSpeed = 0f;
		if (!IsGrounded() || Time.time - GetUpTime > 0.55f)
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateGetUpAEnd()
	{
	}

	private void StateBoundAttackStart()
	{
		PlayerState = State.BoundAttack;
		BoundState = 0;
		BoundTime = 0f;
		BoundLevel = 0;
		BoundHeight = Sonic_New_Lua.c_bound_jump_spd_0 * 0.825f;
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = Sonic_New_Lua.c_boundjump_jmp;
		_Rigidbody.velocity = AirMotionVelocity;
		Audio.PlayOneShot(BoundStart, Audio.volume * 0.35f);
	}

	private void StateBoundAttack()
	{
		PlayerState = State.BoundAttack;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (BoundState == 0)
		{
			PlayAnimation("Rolling", "On Roll");
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
			if (IsGrounded())
			{
				SonicEffects.CreateBoundAttackFX();
				BoundTime = Time.time;
				if (BoundLevel == 0)
				{
					BoundHeight = Sonic_New_Lua.c_bound_jump_spd_0 * 0.825f;
					BoundLevel++;
				}
				else
				{
					BoundHeight = Sonic_New_Lua.c_bound_jump_spd_1;
				}
				if (CurSpeed > Sonic_New_Lua.c_walk_speed_max * 0.75f)
				{
					CurSpeed -= 4f;
				}
				BoundState = 1;
				AttackSphere_Dir(base.transform.position, Sonic_New_Lua.c_boundattack_collision.radius * 2f, 20f, Sonic_New_Lua.c_boundattack_damage);
			}
			else
			{
				AttackSphere_Dir(base.transform.position, Sonic_New_Lua.c_boundattack_collision.radius, 20f, Sonic_New_Lua.c_boundattack_damage);
			}
		}
		else if (BoundState == 1)
		{
			if (Time.time - BoundTime > 0.5f && AirMotionVelocity.y < 0.25f)
			{
				PlayAnimation("Falling", "On Fall");
			}
			else
			{
				PlayAnimation("Rolling", "On Roll");
			}
			AttackSphere_Dir(base.transform.position, Sonic_New_Lua.c_boundattack_collision.radius, 10f, Sonic_New_Lua.c_boundattack_damage);
			if (Time.time - BoundTime < 0.5f)
			{
				AirMotionVelocity.y = BoundHeight;
			}
			else if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
				DoLandAnim();
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
			}
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		}
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
	}

	private void StateBoundAttackEnd()
	{
	}

	private void StateSpinDashStart()
	{
		PlayerState = State.SpinDash;
		SpinDashState = 0;
		if (!SonicEffects.DashPadRoll)
		{
			SpinDashSources[1].volume = 0f;
			SpinDashSources[0].Play();
			SpinDashSources[1].Play();
			if (Singleton<Settings>.Instance.settings.SpinEffect == 1)
			{
				Audio.Stop();
				Audio.PlayOneShot(SpinDashChargeAdventure, Audio.volume);
			}
		}
		else
		{
			SonicEffects.DashPadRoll = false;
		}
		SpindashSpd = Mathf.Min(Mathf.Max(CurSpeed, Sonic_New_Lua.c_walk_speed_max * 6f), Sonic_New_Lua.c_spindash_spd * 1.5f);
		SpindashTime = Time.time;
	}

	private void StateSpinDash()
	{
		PlayerState = State.SpinDash;
		PlayAnimation((SpinDashState == 0) ? "Spindash" : "Spin", (SpinDashState == 0) ? "On Spindash" : "On Spin");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (SpinDashState == 0)
		{
			CurSpeed = Mathf.MoveTowards(CurSpeed, 0f, Time.fixedDeltaTime * 75f);
			SpindashSpd = Mathf.MoveTowards(SpindashSpd, Sonic_New_Lua.c_spindash_spd * 1.5f, Time.fixedDeltaTime * 35f);
			if (CurSpeed == 0f)
			{
				CurSpeed = 0f;
			}
			_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (!SpinDashSources[0].isPlaying)
			{
				SpinDashSources[1].volume = 1f;
			}
		}
		else
		{
			SpinDashSources[0].Stop();
			SpinDashSources[1].Stop();
			CurSpeed += (Vector3.Dot(new Vector3(0f, -0.4f, 0f), base.transform.forward) - 0.05f) * 2f;
			CurSpeed = Mathf.Clamp(CurSpeed, 0f, Sonic_New_Lua.c_run_speed_max * 2f);
			_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (FrontalCollision)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		AttackSphere(base.transform.position, 1.1f, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, 1);
		if (!IsGrounded() || (CurSpeed <= 9f && ShouldAlignOrFall(Align: true)))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateSpinDashEnd()
	{
		SpinDashSources[0].Stop();
		SpinDashSources[1].Stop();
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Sonic_New_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Sonic_New_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Sonic_New_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		_Rigidbody.velocity = Vector3.zero;
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

	private void StateWaterSlideStart()
	{
		PlayerState = State.WaterSlide;
		WSpeed = Mathf.Min(CurSpeed, Sonic_New_Lua.c_run_speed_max * 1.5f);
		WSTime = 0f;
		WSDirection = base.transform.forward;
		WSSpline = GetSpline(LaunchMode);
		WSPositionShift = 0f;
		MaxRayLenght = 0.55f;
	}

	private void StateWaterSlide()
	{
		PlayerState = State.WaterSlide;
		PlayAnimation("Water Slide", "On Water Slide");
		LockControls = true;
		if (IsSuper)
		{
			if (WSpeed > Sonic_New_Lua.c_run_speed_max * 1.3f)
			{
				WSpeed -= 7.5f * Time.fixedDeltaTime;
			}
			else
			{
				WSpeed += 7.5f * Time.fixedDeltaTime;
			}
		}
		else if (WSpeed > 0f)
		{
			WSpeed -= 7.5f * Time.fixedDeltaTime;
		}
		CurSpeed = WSpeed;
		float num = Vector3.Dot(base.transform.forward, Camera.transform.forward);
		WSSmoothPos = Mathf.Lerp(WSSmoothPos, (0f - Singleton<RInput>.Instance.P.GetAxis("Left Stick X")) * ((num > 0f) ? 1f : (-1f)), Time.fixedDeltaTime * Common_Lua.c_waterslider_lr * 2f);
		WSPositionShift = Mathf.Clamp(WSPositionShift + WSSmoothPos * Time.fixedDeltaTime, -1f, 1f);
		WSTime += CurSpeed / WSSpline.Length() * Time.fixedDeltaTime;
		if (WSTime > 1f || (WSTime > 0.25f && IsGrounded()))
		{
			StateMachine.ChangeState(StateGround);
		}
		if (WSpeed <= 4f)
		{
			StateMachine.ChangeState(StateAir);
		}
		WSDirection = WSSpline.GetTangent(WSTime).normalized;
		Vector3 normalized = Vector3.Cross(WSDirection, Vector3.up).normalized;
		_Rigidbody.MovePosition(WSSpline.GetPosition(WSTime) + base.transform.up * 0.25f + normalized * WSPositionShift * 3f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.LookRotation(WSDirection.MakePlanar());
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateWaterSlideEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateTornadoStart()
	{
		PlayerState = State.Tornado;
		UsingGreenGem = true;
		SpawnedAirTornado = false;
		TornadoTimer = Time.time;
		GroundTornado = IsGrounded();
		CurSpeed = 0f;
		TornadoHitFreq = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 0.75f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 0.5f : 0.25f));
		TornadoRadius = Sonic_New_Lua.c_tornado_collision.radius;
		if (HUD.ActiveGemLevel[GemSelector] == 0)
		{
			TornadoRadius -= 2f;
		}
		else if (HUD.ActiveGemLevel[GemSelector] == 2)
		{
			TornadoRadius += 2f;
		}
		TornadoForce = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 5f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 10f : 15f));
		if (!GroundTornado)
		{
			SonicEffects.CreateTornadoFX(HUD.ActiveGemLevel[GemSelector]);
		}
		else
		{
			PlayAnimation("Tornado", "On Tornado");
		}
	}

	private void StateTornado()
	{
		PlayerState = State.Tornado;
		_Rigidbody.velocity = Vector3.zero;
		CurSpeed = 0f;
		if (GroundTornado)
		{
			if (Time.time - TornadoTimer > 0.3f)
			{
				DeflectSphere(base.transform.position + base.transform.up * Sonic_New_Lua.c_tornado_collision.height, TornadoRadius);
				if (Time.time > NextTornadoHit)
				{
					NextTornadoHit = Time.time + TornadoHitFreq;
					AttackSphere_Dir(base.transform.position + base.transform.up * Sonic_New_Lua.c_tornado_collision.height, TornadoRadius, TornadoForce, Sonic_New_Lua.c_tornado_damage);
				}
				if (!SpawnedAirTornado)
				{
					SonicEffects.CreateTornadoFX(HUD.ActiveGemLevel[GemSelector]);
					SpawnedAirTornado = true;
				}
			}
		}
		else
		{
			DeflectSphere(base.transform.position + base.transform.up * Sonic_New_Lua.c_tornado_collision.height, TornadoRadius);
			if (Time.time > NextTornadoHit)
			{
				NextTornadoHit = Time.time + TornadoHitFreq;
				AttackSphere_Dir(base.transform.position + base.transform.up * Sonic_New_Lua.c_tornado_collision.height, TornadoRadius, TornadoForce, Sonic_New_Lua.c_tornado_damage);
			}
			PlayAnimation("Air Tornado", "On Tornado");
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		if (Time.time - TornadoTimer > (GroundTornado ? 3f : 1f))
		{
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateTornadoEnd()
	{
		UsingGreenGem = false;
	}

	private void StateGunDriveStart()
	{
		PlayerState = State.GunDrive;
		if (IsGrounded())
		{
			PlayAnimation("Gun Drive", "On Gun Drive");
		}
		if (GunDriveSnipe)
		{
			HUD.UseCrosshair();
		}
		UsingSkyGem = true;
		ThrewGem = false;
		ThrowPower = 300f;
		GunDriveTimer = Time.time;
		GunDriveState = 0;
		GunDriveSources[0].volume = 1f;
		GunDriveSources[1].volume = 0f;
		GunDriveSources[0].Play();
		GunDriveSources[1].Play();
		SkyGemObject = UnityEngine.Object.Instantiate(HandItem, HandPoint.position, HandPoint.rotation);
		SkyGemObject.transform.SetParent(HandPoint);
		SkyGemObject.transform.localScale = Vector3.one;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Physics.IgnoreCollision(componentsInChildren[i], SkyGemObject.GetComponent<SkyGem>().Collider);
		}
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateGunDrive()
	{
		PlayerState = State.GunDrive;
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			LockControls = true;
			CurSpeed = 0f;
			if (GunDriveSnipe)
			{
				Vector3 forward = base.transform.position - Camera.transform.position;
				forward.y = 0f;
				Quaternion b = Quaternion.LookRotation(forward);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.fixedDeltaTime * Sonic_New_Lua.c_rotation_speed);
			}
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
			_Rigidbody.velocity = Vector3.zero;
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
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
			_Rigidbody.velocity = AirMotionVelocity;
			DoWallNormal();
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		}
		GeneralMeshRotation = base.transform.rotation;
		float num = Time.time - GunDriveTimer;
		if (GunDriveState == 0)
		{
			if ((IsGrounded() && ShouldAlignOrFall(Align: false) && num > 0.2f && !Singleton<RInput>.Instance.P.GetButton("Right Trigger")) || !IsGrounded())
			{
				GunDriveTimer = Time.time;
				GunDriveState = 1;
			}
			if (!GunDriveSources[0].isPlaying)
			{
				GunDriveSources[1].volume = 1f;
			}
			if (!IsGrounded())
			{
				PlayAnimation("Roll", "On Roll");
			}
			ThrowPower = ((IsGrounded() && GunDriveSnipe) ? Mathf.Lerp(200f, 300f, Mathf.Clamp(num, 0f, 2f) / 2f) : 200f);
			return;
		}
		PlayAnimation("Gun Drive End", "On Gun Drive");
		if (Time.time - GunDriveTimer > 0.125f && !ThrewGem)
		{
			ThrewGem = true;
			SkyGemObject.GetComponent<SkyGem>().enabled = true;
			SkyGemObject.GetComponent<SkyGem>().SetGem(this, ((IsGrounded() && GunDriveSnipe) ? Camera.transform.forward : base.transform.forward) * ThrowPower + Vector3.up * 20f);
			SonicEffects.CreateSkyGemFX();
			if (GunDriveSnipe)
			{
				HUD.UseCrosshair(EndCrosshair: true);
			}
			HUD.DrainActionGauge(Sonic_New_Lua.c_sky);
			GunDriveSources[0].Stop();
			GunDriveSources[1].Stop();
			GunDriveSnipe = false;
		}
		if (Time.time - GunDriveTimer > 0.35f)
		{
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
				DoLandAnim();
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateGunDriveEnd()
	{
		if ((bool)SkyGemObject && !ThrewGem)
		{
			UnityEngine.Object.Destroy(SkyGemObject);
			SkyGemObject = null;
		}
		if (GunDriveSnipe)
		{
			HUD.UseCrosshair(EndCrosshair: true);
		}
		if (Camera.CameraState == PlayerCamera.State.FirstPerson)
		{
			Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
		}
		UsingSkyGem = false;
		GunDriveSources[0].Stop();
		GunDriveSources[1].Stop();
	}

	private void StateGunDriveMoveStart()
	{
		PlayerState = State.GunDriveMove;
		DriveStartTime = Time.time;
		AirMotionVelocity = Vector3.up * 5f + (DrivePosition - base.transform.position).normalized * DriveMoveSpeed;
		DriveStartDirection = AirMotionVelocity.normalized;
		DrivePlaneDir = AirMotionVelocity.MakePlanar();
		base.transform.forward = DrivePlaneDir;
		DriveMeshLaunchRot = Quaternion.LookRotation(DriveStartDirection) * Quaternion.Euler(30f, 0f, 0f);
		for (int i = 0; i < Jump123Sources.Length; i++)
		{
			Jump123Sources[i].Play();
		}
		SonicEffects.CreateGunDriveMoveFX();
		CanGunDrive = false;
	}

	private void StateGunDriveMove()
	{
		PlayerState = State.GunDriveMove;
		CanGunDrive = false;
		bool num = Time.time - DriveStartTime < DriveMoveTimer;
		if (!MoveEnded)
		{
			if (HUD.ActiveGemLevel[GemSelector] == 1 && Time.time - DriveStartTime < 0.5f)
			{
				GunDriveAttack = true;
			}
			else if (HUD.ActiveGemLevel[GemSelector] == 2)
			{
				GunDriveAttack = true;
			}
			else
			{
				GunDriveAttack = false;
			}
		}
		else
		{
			GunDriveAttack = false;
		}
		if (GunDriveAttack)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 1.1f, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, 1);
		}
		if (num)
		{
			PlayAnimation("Spring Jump", "On Spring");
			MoveEnded = false;
			DriveMeshLaunchRot = Quaternion.Slerp(DriveMeshLaunchRot, Quaternion.LookRotation(DriveStartDirection) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			AirMotionVelocity = Vector3.up * 5f + (DrivePosition - base.transform.position).normalized * DriveMoveSpeed;
			base.transform.forward = DrivePlaneDir;
			_Rigidbody.velocity = AirMotionVelocity;
			LockControls = true;
			CurSpeed = _Rigidbody.velocity.magnitude;
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		}
		else
		{
			if (_Rigidbody.velocity.y > 0f)
			{
				PlayAnimation("Spring Jump", "On Spring");
				MoveEnded = false;
			}
			else if (!MoveEnded)
			{
				MoveEnded = true;
				PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
			DriveMeshLaunchRot = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
			Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
			if (_Rigidbody.velocity.magnitude != 0f)
			{
				vector = base.transform.forward * CurSpeed;
				AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
			}
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y, Common_Lua.c_vel_y_max);
			_Rigidbody.velocity = AirMotionVelocity;
			DoWallNormal();
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
			Jump123Sources[0].Stop();
		}
		GeneralMeshRotation = DriveMeshLaunchRot;
		if (!num && IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateGunDriveMoveEnd()
	{
		GunDriveAttack = false;
		Jump123Sources[0].Stop();
	}

	private void StateHomingSmashStart()
	{
		PlayerState = State.HomingSmash;
		UsingWhiteGem = true;
		Audio.PlayOneShot(HomingSmashLoop, Audio.volume);
	}

	private void StateHomingSmash()
	{
		PlayerState = State.HomingSmash;
		PlayAnimation("Rolling", "On Roll");
		CurSpeed = 0.001f;
		_Rigidbody.velocity = base.transform.forward * CurSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
	}

	private void StateHomingSmashEnd()
	{
		HUD.DrainActionGauge(Sonic_New_Lua.c_white);
	}

	private void StateTransformStart()
	{
		PlayerState = State.Transform;
		UsingRainbowGem = true;
		HasTransformed = false;
		MaxRayLenght = 0.75f;
		TransformTimer = Time.time;
		Audio.PlayOneShot(SuperSound, Audio.volume);
		TransformGrounded = IsGrounded();
		if (!Physics.Raycast(base.transform.position + base.transform.up * 0.25f, base.transform.forward, out FrontalHit, 2.5f, base.FrontalCol_Mask))
		{
			Camera.PlayCinematic(2.5f, "Sonic Transform");
		}
	}

	private void StateTransform()
	{
		PlayerState = State.Transform;
		PlayAnimation(TransformGrounded ? "Transform" : "Air Transform", "On Transform");
		LockControls = true;
		CurSpeed = 0f;
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - TransformTimer > 1.5f && !HasTransformed)
		{
			HasTransformed = true;
			IsSuper = true;
			SonicEffects.CreateTransformFX();
			AttackSphere_Dir(base.transform.position + base.transform.up * 0.25f, 5f, 10f, 2);
			PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		if (Time.time - TransformTimer > 2.5f)
		{
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateTransformEnd()
	{
		UsingRainbowGem = false;
	}

	private void StateLightAttackStart()
	{
		PlayerState = State.LightAttack;
		PlayAnimation("Light Attack", "On Light Attack");
		UsingRainbowGem = true;
		HasDashed = false;
		HUD.DrainActionGauge(Sonic_New_Lua.c_super);
		MaxRayLenght = 0.75f;
		LATimer = Time.time;
		PlayerVoice.Play(12);
	}

	private void StateLightAttack()
	{
		PlayerState = State.LightAttack;
		float num = Time.time - LATimer;
		if (num < 0.2f)
		{
			PlayAnimation("Light Attack", "On Light Attack");
		}
		CurSpeed = ((num > 0.25f) ? 46f : (-4f));
		if (num > 0.25f)
		{
			if (!HasDashed)
			{
				HasDashed = true;
				SonicEffects.CreateLightAttackFX();
			}
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 3f, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, 5, "SuperSonic");
		}
		if (!IsGrounded())
		{
			Vector3 velocity = base.transform.forward * CurSpeed;
			velocity.y = _Rigidbody.velocity.y;
			velocity.y = 0f;
			_Rigidbody.velocity = velocity;
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		}
		else
		{
			_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		if (num > 1f || FrontalCollision)
		{
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateLightAttackEnd()
	{
		UsingRainbowGem = false;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState != State.Homing)
		{
			HomingTarget = FindHomingTarget();
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Kick || PlayerState == State.Slide || PlayerState == State.Kick || PlayerState == State.GetUpA || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.Tornado || PlayerState == State.GunDrive || PlayerState == State.LightAttack || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (UsingBlueGem && HUD.ActiveGemLevel[GemSelector] > 1)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 1f, base.transform.forward * _Rigidbody.velocity.magnitude, 1);
		}
		if (UsingYellowGem)
		{
			float num = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 2f : 3f));
			AttractSphere(Sonic_New_Lua.c_thunderguard_collision.radius * num);
		}
		if (IsSuper)
		{
			if (PlayerState != State.LightAttack && PlayerState != State.Homing)
			{
				JumpAttackSphere(base.transform.position + base.transform.up * 0.25f, 0.8f, base.transform.forward * _Rigidbody.velocity.magnitude, 5);
			}
			AttractSphere(1f);
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Sonic_New_Lua.c_rotation_speed, PlayerState == State.Kick || PlayerState == State.JumpDashSTH, DontLockOnAir: false, (PlayerState == State.JumpDashSTH) ? 3f : 2f);
				bool flag = IsSuper && HUD.ActiveGemLevel[GemSelector] > 0;
				if (PlayerState != State.Kick && PlayerState != State.Slide && PlayerState != State.SpinDash)
				{
					AccelerationSystem((!HasSpeedUp) ? (Sonic_New_Lua.c_run_acc * ((!flag) ? 1f : 1.3f)) : (Sonic_New_Lua.c_speedup_acc * ((!flag) ? 1f : 1.3f)));
				}
				if (UsingBlueGem)
				{
					MaximumSpeed = Sonic_New_Lua.c_run_speed_max + Sonic_New_Lua.c_custom_action_machspeed_acc;
					CurSpeed = Sonic_New_Lua.c_run_speed_max + Sonic_New_Lua.c_custom_action_machspeed_acc;
				}
				else if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Sonic_New_Lua.c_walk_speed_max : Sonic_New_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? (Sonic_New_Lua.c_speedup_speed_max * ((!flag) ? 1f : 1.3f)) : (IsGrounded() ? (Sonic_New_Lua.c_run_speed_max * ((!flag) ? 1f : 1.3f)) : (Sonic_New_Lua.c_jump_run * ((!flag) ? 1f : 1.3f))));
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
		UpdateCollider();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Kick || PlayerState == State.Slide || PlayerState == State.Kick || PlayerState == State.GetUpA || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.Tornado || PlayerState == State.GunDrive || PlayerState == State.LightAttack || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		CameraFX.IsOnSlowdown = UsingRedGem;
		if (PlayerState != State.Cutscene)
		{
			IsVisible = !UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2);
		}
		CanSink = !IsSuper;
		LockSink = UsingYellowGem;
		SuperAuraSource.volume = Mathf.Lerp(SuperAuraSource.volume, (!IsSuper) ? 0f : 0.75f, Time.deltaTime * 5f);
		if (HasShield && (bool)ShieldObject)
		{
			if (IsSuper && ShieldObject.activeSelf)
			{
				ShieldObject.SetActive(value: false);
			}
			else if (!IsSuper && !ShieldObject.activeSelf)
			{
				ShieldObject.SetActive(value: true);
			}
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (PlayerState != State.Orca && PlayerState != State.Result && PlayerState != State.Transform && PlayerState != State.Cutscene && !UsingGreenGem && !UsingSkyGem && !UsingWhiteGem && !IsSuper && GemData.ObtainedGems != null && GemData.ObtainedGems.Count > 1)
			{
				if (Singleton<RInput>.Instance.P.GetAxis("D-Pad X") > 0f)
				{
					if (!UsingDPad)
					{
						UsingDPad = true;
						Audio.PlayOneShot(GemChange, Audio.volume);
						if (ObtainedGemIndex < GemData.ObtainedGems.Count - 1)
						{
							ObtainedGemIndex++;
						}
						else
						{
							ObtainedGemIndex = 0;
						}
						GemSelector = GemData.ObtainedGems[ObtainedGemIndex];
						ActiveGem = (Gem)GemSelector;
						HUD.TriggerGemPanel();
					}
				}
				else if (Singleton<RInput>.Instance.P.GetAxis("D-Pad X") < -0f)
				{
					if (!UsingDPad)
					{
						UsingDPad = true;
						Audio.PlayOneShot(GemChange, Audio.volume);
						if (ObtainedGemIndex > 0)
						{
							ObtainedGemIndex--;
						}
						else
						{
							ObtainedGemIndex = GemData.ObtainedGems.Count - 1;
						}
						GemSelector = GemData.ObtainedGems[ObtainedGemIndex];
						ActiveGem = (Gem)GemSelector;
						HUD.TriggerGemPanel();
					}
				}
				else if (Singleton<RInput>.Instance.P.GetAxis("D-Pad X") == 0f)
				{
					UsingDPad = false;
				}
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				HoldTime[0] = Time.time;
			}
			else if (AttackState[0] == 1)
			{
				AttackState[0] = 2;
			}
			if (PlayerState == State.Ground)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
				{
					StateMachine.ChangeState(StateJump);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y") && !IsSinking)
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (AttackState[0] == 2 && ShouldAlignOrFall(Align: false) && CurSpeed > Sonic_New_Lua.c_sliding_speed_min && !IsSinking)
				{
					if (Time.time - HoldTime[0] > 0.25f)
					{
						if (!UsedSlide)
						{
							StateMachine.ChangeState(StateSlide);
						}
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
					{
						StateMachine.ChangeState(StateKick);
					}
				}
				if (ShouldAlignOrFall(Align: false) && CurSpeed < Sonic_New_Lua.c_sliding_speed_min && Singleton<RInput>.Instance.P.GetButtonDown("Button B") && !IsSinking)
				{
					StateMachine.ChangeState(StateKick);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !IsSinking)
				{
					StateMachine.ChangeState(StateSpinDash);
				}
			}
			if (PlayerState == State.SpinDash)
			{
				if (Time.time - SpindashTime > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
				if (SpinDashState == 1 && (Singleton<RInput>.Instance.P.GetButtonDown("Button X") || _Rigidbody.velocity.magnitude < 3f))
				{
					StateMachine.ChangeState(StateGround);
				}
				if (SpinDashState == 0 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					SpinDashState = 1;
					Audio.PlayOneShot((Singleton<Settings>.Instance.settings.SpinEffect == 0) ? SpinDashShoot : ((Singleton<Settings>.Instance.settings.SpinEffect == 1) ? SpinDashShootAdventure : SpinDashShootAdventure2), Audio.volume);
					CurSpeed = SpindashSpd;
					CurSpeed += _Rigidbody.velocity.magnitude * 0.6f;
					CurSpeed = Mathf.Clamp(CurSpeed, 0f, Sonic_New_Lua.c_run_speed_max * 2f);
				}
				if (SpinDashState == 1 && AttackState[0] == 2 && CurSpeed > Sonic_New_Lua.c_sliding_speed_min)
				{
					if (Time.time - HoldTime[0] > 0.25f)
					{
						if (!UsedSlide)
						{
							StateMachine.ChangeState(StateSlide);
						}
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
					{
						StateMachine.ChangeState(StateKick);
					}
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				AttackState[0] = 1;
				UsedSlide = false;
			}
			if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if (PlayerState == State.WaterSlide && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if ((PlayerState == State.Air || PlayerState == State.SlowFall) && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				StateMachine.ChangeState(StateBoundAttack);
			}
			if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls))
			{
				if (((PlayerState == State.Jump && ReleasedKey) || PlayerState != State.Jump) && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					if (UsingPurpleGem && PlayerState != State.GunDriveMove)
					{
						int num = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1 : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 2 : 3));
						if (Time.time - JumpBlock > ((JumpLimit < 1) ? 0f : Sonic_New_Lua.c_scale_jump_block) && JumpLimit < num)
						{
							StateMachine.ChangeState(StateJump);
							JumpLimit++;
						}
					}
					else if (JumpLimit < 1)
					{
						OnJumpDash();
					}
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateBoundAttack);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.Air)
			{
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (CanFallJumpdash && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					OnJumpDash();
				}
			}
			if (IsGrounded() && !CanFallJumpdash)
			{
				CanFallJumpdash = true;
			}
			if (PlayerState == State.AfterHoming)
			{
				if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
				{
					HAPressed = false;
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					OnJumpDash();
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateBoundAttack);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.BoundAttack)
			{
				if (((BoundState == 0 && CanJumpdash) || BoundState == 1) && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					CanJumpdash = false;
					OnJumpDash();
				}
				if (BoundState == 1 && Time.time - BoundTime > Sonic_New_Lua.c_boundjump_block && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					BoundState = 0;
					AirMotionVelocity.y = Sonic_New_Lua.c_boundjump_jmp;
					Audio.PlayOneShot(BoundStart, Audio.volume * 0.35f);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState != State.BoundAttack && (IsGrounded() || PlayerState == State.AfterHoming || PlayerState == State.Grinding || PlayerState == State.ChainJump || (PlayerState == State.Spring && LockControls) || (PlayerState == State.WideSpring && LockControls) || (PlayerState == State.JumpPanel && LockControls) || (PlayerState == State.RainbowRing && LockControls) || (PlayerState == State.Pole && LockControls) || (PlayerState == State.Rope && LockControls)))
			{
				CanJumpdash = true;
			}
			if (PlayerState == State.Kick)
			{
				if (Time.time - KickTime > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					JumpType = 1;
					StateMachine.ChangeState(StateTrickJump);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.Slide)
			{
				if (Time.time - SlideEndTimer > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					JumpType = 0;
					StateMachine.ChangeState(StateTrickJump);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StateGround);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.TrickJump)
			{
				if (ReleasedTrickKey && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					if (UsingPurpleGem)
					{
						int num2 = ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1 : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 2 : 3));
						if (Time.time - JumpBlock > ((JumpLimit < 1) ? 0f : Sonic_New_Lua.c_scale_jump_block) && JumpLimit < num2)
						{
							StateMachine.ChangeState(StateJump);
							JumpLimit++;
						}
					}
					else if (JumpLimit < 1)
					{
						OnJumpDash();
					}
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateBoundAttack);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.GetUpA && !FrontalCollision && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				StateMachine.ChangeState(StateKick);
			}
			float num3 = Sonic_New_Lua.c_green * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1.25f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 1f : 0.75f));
			if ((PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.Jump || PlayerState == State.SlowFall || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && ActiveGem == Gem.Green && HUD.ActionDisplay > num3 && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateTornado);
				HUD.DrainActionGauge(num3);
			}
			if (UsingGreenGem && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				if (IsGrounded())
				{
					StateMachine.ChangeState(StateJump);
				}
				else
				{
					StateMachine.ChangeState(StateAir);
				}
			}
			if (ActiveGem == Gem.Red)
			{
				float num4 = Sonic_New_Lua.c_red * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1.25f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 1f : 0.75f));
				if (PlayerState != State.Orca && PlayerState != State.Result && PlayerState != State.Cutscene && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && HUD.ActionDisplay > num4)
				{
					UsingRedGem = true;
					SonicEffects.CreateSlowdownFX(HUD.ActiveGemLevel[GemSelector]);
				}
				if (UsingRedGem && (HUD.ActionDisplay <= 0f || Singleton<RInput>.Instance.P.GetButtonUp("Right Trigger") || (PlayerState == State.Orca && PlayerState == State.Result && PlayerState == State.Cutscene)))
				{
					UsingRedGem = false;
				}
			}
			else
			{
				UsingRedGem = false;
			}
			if (ActiveGem == Gem.Blue && !FrontalCollision && PlayerState == State.Ground)
			{
				float num5 = Sonic_New_Lua.c_blue * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1.25f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 1f : 0.75f));
				if (!LockControls && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && !UsingBlueGem && HUD.ActionDisplay >= num5)
				{
					UsingBlueGem = true;
					MachTimer = Time.time;
					Animator.SetTrigger("Additive Idle");
					SonicEffects.CreateMachSpeedFX(HUD.ActiveGemLevel[GemSelector]);
					HUD.DrainActionGauge(num5);
				}
			}
			else
			{
				UsingBlueGem = false;
			}
			if (UsingBlueGem && Time.time - MachTimer > ((HUD.ActiveGemLevel[GemSelector] == 0) ? Sonic_New_Lua.c_custom_action_machspeed_time : ((HUD.ActiveGemLevel[GemSelector] == 1) ? Sonic_New_Lua.c_custom_action_machspeed2_time : Sonic_New_Lua.c_custom_action_machspeed3_time)))
			{
				UsingBlueGem = false;
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
			{
				HoldTime[1] = Time.time;
			}
			else if (AttackState[1] == 1)
			{
				AttackState[1] = 2;
			}
			if (PlayerState == State.Ground)
			{
				if (AttackState[1] == 2 && ShouldAlignOrFall(Align: false) && ActiveGem == Gem.Sky && !SkyGemObject && HUD.ActionDisplay > 30f)
				{
					if (Time.time - HoldTime[1] > 0.25f)
					{
						GunDriveSnipe = true;
						StateMachine.ChangeState(StateGunDrive);
						Camera.StateMachine.ChangeState(Camera.StateFirstPerson);
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Right Trigger"))
					{
						StateMachine.ChangeState(StateGunDrive);
					}
				}
			}
			else if ((PlayerState == State.Air || PlayerState == State.Jump || PlayerState == State.SlowFall || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && ActiveGem == Gem.Sky && CanGunDrive && !SkyGemObject && HUD.ActionDisplay > 30f && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateGunDrive);
			}
			if (IsGrounded() && PlayerState != State.Grinding)
			{
				CanGunDrive = true;
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
			{
				AttackState[1] = 1;
			}
			if (ActiveGem == Gem.Yellow)
			{
				if (PlayerState != State.Orca && PlayerState != State.Result && PlayerState != State.Cutscene && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					UsingYellowGem = !UsingYellowGem;
					if (UsingYellowGem)
					{
						float num6 = Sonic_New_Lua.c_yellow * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 0.75f : 0.5f));
						if (HUD.ActionDisplay > num6)
						{
							AddThunderShield();
							RemoveShield();
						}
						else
						{
							UsingYellowGem = false;
						}
					}
					else
					{
						RemoveThunderShield();
					}
				}
				if (UsingYellowGem && (HUD.ActionDisplay <= 0f || (PlayerState == State.Orca && PlayerState == State.Result)))
				{
					UsingYellowGem = false;
					RemoveThunderShield();
				}
			}
			else
			{
				UsingYellowGem = false;
				RemoveThunderShield();
			}
			if (ActiveGem == Gem.Purple)
			{
				float num7 = Sonic_New_Lua.c_purple * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 0.75f : 0.5f));
				if (PlayerState != State.Orca && PlayerState != State.Result && PlayerState != State.Cutscene && PlayerState != State.GunDriveMove && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && HUD.ActionDisplay > num7)
				{
					UsingPurpleGem = true;
					SonicEffects.CreateScaleFX();
				}
				if (UsingPurpleGem && (HUD.ActionDisplay <= 0f || Singleton<RInput>.Instance.P.GetButtonUp("Right Trigger") || PlayerState == State.Orca || PlayerState == State.Result || PlayerState == State.Cutscene))
				{
					UsingPurpleGem = false;
				}
			}
			else
			{
				UsingPurpleGem = false;
			}
			if (IsGrounded() || PlayerState == State.Grinding || PlayerState == State.Rope || PlayerState == State.Hold)
			{
				JumpLimit = 0;
			}
			if ((PlayerState == State.Jump || PlayerState == State.TrickJump || PlayerState == State.AfterHoming || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && ActiveGem == Gem.White && JumpLimit < 1 && HUD.ActionDisplay > Sonic_New_Lua.c_white && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateHomingSmash);
			}
			if (PlayerState == State.BoundAttack && ((BoundState == 0 && CanJumpdash) || BoundState == 1) && ActiveGem == Gem.White && HUD.ActionDisplay > Sonic_New_Lua.c_white && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				CanJumpdash = false;
				StateMachine.ChangeState(StateHomingSmash);
			}
			if (PlayerState == State.HomingSmash && !Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
			{
				OnJumpDash();
				Audio.Stop();
				Audio.PlayOneShot(HomingSmashShoot, Audio.volume);
			}
			if (IsGrounded() && UsingWhiteGem)
			{
				UsingWhiteGem = false;
			}
			if ((PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.Jump || PlayerState == State.SlowFall || PlayerState == State.AfterHoming || PlayerState == State.SpinDash || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && ActiveGem == Gem.Rainbow && !IsSuper && HUD.Rings >= 50 && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateTransform);
			}
			if (ActiveGem == Gem.Rainbow && IsSuper && StageManager.Player == StageManager.PlayerName.Sonic_New && HUD.Rings > 0 && PlayerState != State.Result)
			{
				RingDrainTimer += Time.deltaTime;
				if (RingDrainTimer >= 1f)
				{
					RingDrainTimer = 0f;
					Singleton<GameManager>.Instance._PlayerData.rings--;
				}
				if (!LockControls && HUD.ActionDisplay == 100f && HUD.ActiveGemLevel[GemSelector] == 2 && !FrontalCollision && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					StateMachine.ChangeState(StateLightAttack);
				}
			}
			if ((ActiveGem == Gem.Rainbow && HUD.Rings <= 0) || ActiveGem != Gem.Rainbow)
			{
				Detransform();
			}
		}
		else
		{
			if (UsingRedGem)
			{
				UsingRedGem = false;
			}
			if (UsingPurpleGem)
			{
				UsingPurpleGem = false;
			}
		}
		WaterRunSource.volume = Mathf.Lerp(WaterRunSource.volume, (PlayerState == State.WaterSlide) ? 1f : 0f, Time.deltaTime * 6f);
		Jump123Sources[1].volume = Mathf.Lerp(Jump123Sources[1].volume, ((PlayerState == State.GunDriveMove && !LockControls) || (PlayerState == State.ChainJump && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait Loop"))) ? 1f : 0f, Time.deltaTime * 8f);
		if (LockControls)
		{
			UsingBlueGem = false;
		}
	}

	public override void SetStoredAttributes()
	{
		base.SetStoredAttributes();
		if (Singleton<GameManager>.Instance.StoredPlayerVars == null)
		{
			return;
		}
		for (int i = 0; i < Singleton<GameManager>.Instance.StoredPlayerVars.Length; i++)
		{
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "GemGeneral")
			{
				HUD.ActionDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloat;
				GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
				for (int j = 0; j < gameData.ObtainedGems.Count; j++)
				{
					if (gameData.ObtainedGems[j] == Singleton<GameManager>.Instance.StoredPlayerVars[i].VarInt)
					{
						GemSelector = gameData.ObtainedGems[j];
					}
					if (gameData.ObtainedGems[j] == GemSelector)
					{
						ObtainedGemIndex = j;
					}
				}
				ActiveGem = (Gem)GemSelector;
				IsSuper = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool;
			}
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "GemLevels")
			{
				HUD.ActiveGemLevel = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarIntArray;
			}
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "GemDisplay")
			{
				HUD.GemDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloatArray;
			}
		}
	}

	public void Detransform()
	{
		if (IsSuper)
		{
			SonicEffects.CreateTransformFX(Super: false);
			IsSuper = false;
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
		if (PlayerState == State.Jump || PlayerState == State.JumpDash || PlayerState == State.JumpDashSTH || PlayerState == State.TrickJump)
		{
			return 0;
		}
		if (PlayerState == State.BoundAttack || PlayerState == State.Homing || PlayerState == State.AfterHoming || PlayerState == State.Kick || PlayerState == State.Slide || (PlayerState == State.SpinDash && SpinDashState == 1) || PlayerState == State.HomingSmash)
		{
			return 1;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if (PlayerState == State.LightDash || PlayerState == State.Hurt || PlayerState == State.Talk || PlayerState == State.WarpHole || PlayerState == State.Result || (PlayerState == State.GunDriveMove && GunDriveAttack) || (UsingBlueGem && HUD.ActiveGemLevel[GemSelector] > 1) || PlayerState == State.Transform || PlayerState == State.Cutscene || PlayerState == State.Orca || HasInvincibility || IsSuper || IsDead)
		{
			return true;
		}
		return result;
	}

	private void AddThunderShield()
	{
		if (ThunderShieldObject == null)
		{
			float amount = Sonic_New_Lua.c_yellow * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 0.75f : 0.5f));
			HUD.DrainActionGauge(amount);
			ThunderShieldObject = UnityEngine.Object.Instantiate(ThunderShield, base.transform.position + base.transform.up * 0.25f, Quaternion.identity);
			ThunderShieldObject.transform.SetParent(base.transform);
		}
	}

	private void RemoveThunderShield()
	{
		if (ThunderShieldObject != null)
		{
			UnityEngine.Object.Destroy(ThunderShieldObject);
			if (IsSinking && ColName == "2820000d")
			{
				ImmunityTime = Time.time + Common_Lua.c_invincible_time;
				StateMachine.ChangeState(StateHurt);
			}
		}
	}

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		Animator.SetFloat("Super Anim Float", IsSuper ? 1f : 0f);
	}

	private void UpdateMesh()
	{
		Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.localEulerAngles.y : 1f));
		Vector3 vector = quaternion * Vector3.forward;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = quaternion * Vector3.right;
		vector2.y = 0f;
		vector2.Normalize();
		Vector3 normalized = (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * vector2 + Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * vector).normalized;
		normalized = Quaternion.FromToRotation(Vector3.up, base.transform.up) * normalized;
		if (PlayerState != State.WarpHole)
		{
			bool flag = PlayerState == State.WaterSlide;
			float num = Vector3.Dot((!flag) ? TargetDirection.normalized : normalized.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.JumpDashSTH || PlayerState == State.Slide || (PlayerState == State.SpinDash && SpinDashState == 1)) && !LockControls && CurSpeed > 0f && !WalkSwitch) || flag || PlayerState == State.Balancer)) ? ((0f - num) * ((!flag) ? num2 : 8f)) : 0f, 10f * Time.deltaTime);
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
		if (Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int i = 0; i < Upgrades.Renderers.Count; i++)
			{
				Upgrades.Renderers[i].enabled = ImmunityTime - Time.time <= 0f || ((ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
				if (!UsingPurpleGem && !Upgrades.Renderers[i].transform.parent.GetChild(1).gameObject.activeSelf)
				{
					Upgrades.Renderers[i].transform.parent.GetChild(1).gameObject.SetActive(value: true);
					Upgrades.Renderers[i].transform.parent.GetChild(2).gameObject.SetActive(value: false);
				}
				else if (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && Upgrades.Renderers[i].transform.parent.GetChild(1).gameObject.activeSelf)
				{
					Upgrades.Renderers[i].transform.parent.GetChild(1).gameObject.SetActive(value: false);
					Upgrades.Renderers[i].transform.parent.GetChild(2).gameObject.SetActive(value: true);
				}
			}
		}
		PlayerRenderers[0].enabled = (!IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)) && ImmunityTime - Time.time <= 0f) || ((!IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)) && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[1].enabled = (IsSuper && ImmunityTime - Time.time <= 0f) || ((IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[2].enabled = ((GemSelector == 0 || (Singleton<Settings>.Instance.settings.GemShoesType == 1 && !IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)))) && ImmunityTime - Time.time <= 0f) || (((GemSelector == 0 || (Singleton<Settings>.Instance.settings.GemShoesType == 1 && !IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)))) && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[3].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 1 && ((!IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)) && GemSelector != 0 && ImmunityTime - Time.time <= 0f) || ((!IsSuper && (!UsingPurpleGem || (UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] < 2)) && GemSelector != 0 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
		PlayerRenderers[4].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 0 && ((IsSuper && GemSelector != 0 && ImmunityTime - Time.time <= 0f) || ((IsSuper && GemSelector != 0 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
		PlayerRenderers[5].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 1 && ((IsSuper && GemSelector != 0 && ImmunityTime - Time.time <= 0f) || ((IsSuper && GemSelector != 0 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
		PlayerRenderers[6].enabled = (!IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && ImmunityTime - Time.time <= 0f) || ((!IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[7].enabled = (GemSelector != 0 && Singleton<Settings>.Instance.settings.GemShoesType == 1 && !IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && ImmunityTime - Time.time <= 0f) || ((GemSelector != 0 && Singleton<Settings>.Instance.settings.GemShoesType == 1 && !IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[8].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 1 && ((!IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && GemSelector != 0 && ImmunityTime - Time.time <= 0f) || ((!IsSuper && UsingPurpleGem && HUD.ActiveGemLevel[GemSelector] > 1 && GemSelector != 0 && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
	}

	private void UpdateCollider()
	{
		if (PlayerState == State.Kick || PlayerState == State.Slide || PlayerState == State.SpinDash)
		{
			CapsuleCollider.center = Vector3.zero;
			CapsuleCollider.height = 0.5f;
		}
		else
		{
			CapsuleCollider.center = Vector3.MoveTowards(CapsuleCollider.center, new Vector3(0f, 0.25f, 0f), Time.deltaTime * 2f);
			CapsuleCollider.height = Mathf.MoveTowards(CapsuleCollider.height, 1f, Time.deltaTime * 4f);
		}
	}

	private bool OnJumpDash()
	{
		Audio.PlayOneShot((Singleton<Settings>.Instance.settings.JumpdashType != 2) ? JumpDashSound : JumpDashSTHSound, Audio.volume);
		PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		CanFallJumpdash = false;
		if (!HomingTarget)
		{
			if (Singleton<Settings>.Instance.settings.JumpdashType != 2)
			{
				StateMachine.ChangeState(StateJumpDash);
			}
			else
			{
				StateMachine.ChangeState(StateJumpDashSTH);
			}
			return false;
		}
		StateMachine.ChangeState(StateHoming);
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
		if (rings > 0 || HasShield || UsingYellowGem)
		{
			ImmunityTime = Time.time + (UsingYellowGem ? 0.25f : Common_Lua.c_invincible_time);
			if (!UsingYellowGem)
			{
				if (PlayerState != State.Grinding && PlayerState != State.Balancer && (PlayerState != State.GunDrive || (PlayerState == State.GunDrive && !GunDriveSnipe)))
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
			}
			else
			{
				float amount = Sonic_New_Lua.c_yellow * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1.5f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 1f : 0.5f));
				HUD.DrainActionGauge(amount);
				ThunderguardHurt = true;
			}
			if (HasShield || UsingYellowGem)
			{
				RemoveShield();
			}
			else if (!ThunderguardHurt)
			{
				CreateRings(Mathf.Min(rings, 20));
				Singleton<GameManager>.Instance._PlayerData.rings = 0;
			}
			if (!UsingYellowGem)
			{
				ThunderguardHurt = false;
			}
		}
		else
		{
			OnDeathEnter(0);
		}
	}

	public override void OnBulletHit(Vector3 Direction, float Rate = 1f, int DeathType = 0)
	{
		if (!IsInvulnerable(1) && !UsingYellowGem)
		{
			base.OnBulletHit(Direction, Rate, DeathType);
		}
		else if (ImmunityTime - Time.time <= 0f && UsingYellowGem)
		{
			float amount = Sonic_New_Lua.c_yellow * ((HUD.ActiveGemLevel[GemSelector] == 0) ? 1f : ((HUD.ActiveGemLevel[GemSelector] == 1) ? 0.75f : 0.5f));
			HUD.DrainActionGauge(amount);
			ImmunityTime = Time.time + 0.25f;
		}
	}

	public override void OnDeathEnter(int DeathType)
	{
		if (!IsDead && PlayerState != State.WaterSlide && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && PlayerState != State.Hold)
		{
			IsDead = true;
			Camera.OnPlayerDeath();
			Detransform();
			if (UsingYellowGem)
			{
				UsingYellowGem = false;
				RemoveThunderShield();
			}
			DoDeath(DeathType);
		}
	}

	public override void OnWaterSlideEnter(string Spline = "", bool TriggerState = true, float Speed = 0f)
	{
		if (Speed != 0f)
		{
			if (PlayerState == State.WaterSlide)
			{
				WSpeed = Speed;
			}
			else
			{
				CurSpeed = Speed;
			}
		}
		if (TriggerState)
		{
			LaunchMode = Spline;
			StateMachine.ChangeState(StateWaterSlide);
		}
	}

	public void OnGunDriveMove(Vector3 Position, float Timer, float Speed)
	{
		if (PlayerState != State.Result && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath)
		{
			DrivePosition = Position;
			DriveMoveTimer = Timer;
			DriveMoveSpeed = Speed;
			StateMachine.ChangeState(StateGunDriveMove);
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
		case "StateSpinDashUncurl":
			StateMachine.ChangeState(StateSpinDash);
			SpinDashState = 1;
			break;
		}
	}

	public override void StartPlayer(bool TalkState = false)
	{
		if (Singleton<Settings>.Instance.settings.TGSSonic == 1)
		{
			Animator.runtimeAnimatorController = TGSAnimator;
		}
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
