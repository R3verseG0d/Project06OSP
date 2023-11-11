using System;
using STHLua;
using UnityEngine;

public class Princess : PlayerBase
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
		Slide = 10,
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
		ChainJump = 30,
		Lotus = 31,
		Bungee = 32,
		Tarzan = 33,
		UpReel = 34,
		Balancer = 35
	}

	[Header("Player Framework")]
	public Animator EliseAnimator;

	public PrincessEffects PrincessEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private RaycastHit WaterShieldHit;

	private bool UsingDPad;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	public UpgradeModels Upgrades;

	private float BodyDirDot;

	private float BlinkTimer;

	[Header("Shield")]
	public LayerMask PushableObjectMask;

	internal bool UsingShield;

	private static Collider[] ColliderPool = new Collider[8];

	private string WaterColName;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip JumpDashSound;

	public AudioClip JumpDashSTHSound;

	public AudioClip JumpDashKickback;

	public AudioClip SlideSound;

	public AudioClip BrakeSound;

	public AudioClip ShieldOffSound;

	[Header("Audio Sources")]
	public AudioSource[] ShieldSources;

	public AudioSource[] Jump123Sources;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool ReachedApex;

	private float JumpDashStartTime;

	private float JumpDashLength;

	private float JumpDashSpeed;

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

	private float SlideEndTimer;

	internal float SlideSpeed;

	private bool SlideReleasedKey;

	private float HurtTime;

	private float EdgeDangerTime;

	private LayerMask ShieldWater_Mask => LayerMask.GetMask("Water", "TriggerCollider");

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Princess_Lua.c_player_name;
		PlayerNameShort = Princess_Lua.c_player_name_short;
		WalkSpeed = Princess_Lua.c_walk_speed_max;
		TopSpeed = Princess_Lua.c_run_speed_max;
		BrakeSpeed = Princess_Lua.c_brake_acc;
		GrindSpeedOrg = Princess_Lua.c_grind_speed_org;
		GrindAcc = Princess_Lua.c_grind_acc;
		GrindSpeedMax = Princess_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Princess_Lua.OpenGauge(), Princess_Lua.c_gauge_max, Princess_Lua.c_gauge_heal, Princess_Lua.c_gauge_heal_delay);
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
				PlayIdleEvent(1);
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Princess_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Princess_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Princess_Lua.c_run_speed_max) / Princess_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Princess_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Princess_Lua.c_run_speed_max;
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
		HalveSinkJump = ColName == "40000009" || ColName == "2820000d" || WaterColName == "00080001" || WaterColName == "00080011";
		ReleasedKey = false;
		ReachedApex = false;
		PlayAnimation("Jump Up", "On Jump");
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Princess_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Princess_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateJump()
	{
		PlayerState = State.Jump;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y, 12f);
		if (!ReachedApex && (ReleasedKey || AirMotionVelocity.y < 3f))
		{
			PlayAnimation("Jump Down", "On Jump");
			ReachedApex = true;
		}
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!HalveSinkJump) ? 0.7f : 0.45f))
		{
			AirMotionVelocity += Vector3.up * ((!HalveSinkJump) ? 4.25f : 3f) * Time.fixedDeltaTime * 4f;
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
		JumpDashLength = ((Singleton<Settings>.Instance.settings.JumpdashType == 0) ? Princess_Lua.c_homing_time : Princess_Lua.c_homing_e3_time);
		JumpDashStartTime = Time.time;
		JumpDashSpeed = Princess_Lua.c_homing_spd;
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
		PlayAnimation("Homing", "On Homing");
		_ = (Time.time - JumpDashStartTime) / JumpDashLength;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		JumpDashSpeed -= Princess_Lua.c_homing_brake * 0.75f * Time.deltaTime;
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
		if (AttackSphere(base.transform.position, Princess_Lua.c_collision_homing(), base.transform.forward * CurSpeed, Princess_Lua.c_homing_damage))
		{
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
	}

	private void StateJumpDashSTHStart()
	{
		PlayerState = State.JumpDashSTH;
		MaxRayLenght = 0.75f;
		JumpDashSTHLength = 0.4f;
		JumpDashSTHStartTime = Time.time;
		CurSpeed = Princess_Lua.c_homing_spd;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateJumpDashSTH()
	{
		PlayerState = State.JumpDashSTH;
		PlayAnimation("Homing", "On Homing");
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
		if (AttackSphere(base.transform.position, Princess_Lua.c_collision_homing(), base.transform.forward * Princess_Lua.c_homing_spd, Princess_Lua.c_homing_damage))
		{
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
		Speed = Mathf.Min(Speed + Princess_Lua.c_lightdash_speed * Time.fixedDeltaTime * 10f, Princess_Lua.c_lightdash_speed);
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
				CurSpeed = (Princess_Lua.c_run_speed_max + Princess_Lua.c_lightdash_speed) / 1.75f;
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
		PlayAnimation("Homing", "On Homing");
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
		GeneralMeshRotation = Quaternion.LookRotation(HomingDirection);
		base.transform.rotation = Quaternion.LookRotation(vector);
		_Rigidbody.MovePosition(Vector3.Lerp(HAStartPos, HomingTarget.transform.position, num2));
		if (AttackSphere(base.transform.position + base.transform.forward * 0.25f, Princess_Lua.c_collision_homing(), DirectionToTarget * Princess_Lua.c_homing_power, Princess_Lua.c_homing_damage))
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
		if (AfterHomingTrick > 2)
		{
			AfterHomingTrick = 0;
		}
		Animator.SetTrigger("On After Homing");
		Audio.PlayOneShot(JumpDashKickback, Audio.volume);
		PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		AHEnterTime = Time.time;
		HAPressed = true;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAfterHoming()
	{
		PlayerState = State.AfterHoming;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - AHEnterTime <= 0.25f || (Time.time - AHEnterTime <= 0.45f && HAPressed))
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

	private void StateSlideStart()
	{
		PlayerState = State.Slide;
		SlideReleasedKey = false;
		Audio.PlayOneShot(SlideSound, Audio.volume);
		PlayerVoice.PlayRandom(7, RandomPlayChance: true);
		SlideEndTimer = Time.time;
		SlideSpeed = CurSpeed;
	}

	private void StateSlide()
	{
		PlayerState = State.Slide;
		if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
		{
			SlideReleasedKey = true;
		}
		PlayAnimation("Sliding Attack", "On Sliding Attack");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		SlideSpeed += Vector3.Dot(new Vector3(0f, -0.5f, 0f), base.transform.forward);
		SlideSpeed = Mathf.Clamp(SlideSpeed, Princess_Lua.c_sliding_speed_min, Princess_Lua.c_run_speed_max * 1.5f);
		CurSpeed = SlideSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position, Princess_Lua.c_sliding_collision.radius, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, Princess_Lua.c_sliding_damage);
		if (Time.time - SlideEndTimer > Princess_Lua.c_sliding_time || !IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
		}
		if (FrontalCollision)
		{
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateSlideEnd()
	{
	}

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		HurtTime = Time.time;
		PlayerVoice.PlayRandom(4, RandomPlayChance: false, RandomMulticast: true);
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Princess_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Princess_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Princess_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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

	public void SyncEliseAnimator()
	{
		if (!(EliseAnimator != null))
		{
			return;
		}
		if (Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != EliseAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash)
		{
			EliseAnimator.Play(Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
		}
		AnimatorControllerParameter[] parameters = EliseAnimator.parameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
		{
			switch (animatorControllerParameter.type)
			{
			case AnimatorControllerParameterType.Bool:
				EliseAnimator.SetBool(animatorControllerParameter.nameHash, Animator.GetBool(animatorControllerParameter.nameHash));
				break;
			case AnimatorControllerParameterType.Float:
				EliseAnimator.SetFloat(animatorControllerParameter.nameHash, Animator.GetFloat(animatorControllerParameter.nameHash));
				break;
			case AnimatorControllerParameterType.Int:
				EliseAnimator.SetInteger(animatorControllerParameter.nameHash, Animator.GetInteger(animatorControllerParameter.nameHash));
				break;
			case AnimatorControllerParameterType.Trigger:
				if (Animator.GetBool(animatorControllerParameter.nameHash))
				{
					EliseAnimator.SetTrigger(animatorControllerParameter.nameHash);
				}
				else
				{
					EliseAnimator.ResetTrigger(animatorControllerParameter.nameHash);
				}
				break;
			}
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState != State.Homing)
		{
			HomingTarget = FindHomingTarget();
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Slide || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (UsingShield && PlayerState != State.Homing)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 1.1f, WorldVelocity, 1);
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, 2f, ColliderPool, PushableObjectMask);
			for (int i = 0; i < num; i++)
			{
				Rigidbody attachedRigidbody = ColliderPool[i].attachedRigidbody;
				if (attachedRigidbody != null)
				{
					attachedRigidbody.AddForce((attachedRigidbody.transform.position - base.transform.position).normalized * 10f, ForceMode.Impulse);
				}
			}
		}
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out WaterShieldHit, 1f, ShieldWater_Mask))
		{
			Debug.DrawLine(base.transform.position + base.transform.up * 0.5f, WaterShieldHit.point, Color.yellow);
		}
		WaterColName = (WaterShieldHit.transform ? WaterShieldHit.transform.gameObject.name : "");
		if (WaterColName == "00080001" || WaterColName == "00080011")
		{
			WaterShieldHit.transform.gameObject.layer = LayerMask.NameToLayer((!CanSink && !IsDead) ? "Water" : "TriggerCollider");
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Princess_Lua.c_rotation_speed, PlayerState == State.JumpDashSTH, DontLockOnAir: false, 3f);
				if (PlayerState != State.Slide)
				{
					AccelerationSystem((!HasSpeedUp) ? Princess_Lua.c_run_acc : Princess_Lua.c_speedup_acc);
				}
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Princess_Lua.c_walk_speed_max : Princess_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Princess_Lua.c_speedup_speed_max : (IsGrounded() ? Princess_Lua.c_run_speed_max : Princess_Lua.c_jump_run));
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
		SyncEliseAnimator();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Slide || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (PlayerState == State.Ground && !IsSinking)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && CurSpeed > Princess_Lua.c_sliding_speed_min)
				{
					StateMachine.ChangeState(StateSlide);
				}
			}
			if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Lotus && !LockControls) || (PlayerState == State.Tarzan && !LockControls))
			{
				if ((PlayerState == State.Jump && ReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Button A")) || (PlayerState != State.Jump && Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
				{
					OnJumpDash();
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.Air && CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateLightDash);
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
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.Slide)
			{
				if (Time.time - SlideEndTimer > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !IsSinking)
				{
					StateMachine.ChangeState(StateJump);
				}
				if (SlideReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StateGround);
				}
			}
			if (!UsingShield && PlayerState != State.Result && PlayerState != State.Cutscene && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && HUD.ActionDisplay > 0f)
			{
				HUD.DrainActionGauge(10f);
				UsingShield = true;
				ShieldSources[0].Play();
				PrincessEffects.CreateShieldFX();
			}
			if (UsingShield && !ShieldSources[0].isPlaying && !ShieldSources[1].isPlaying)
			{
				ShieldSources[1].Play();
			}
			if (UsingShield && (HUD.ActionDisplay <= 0f || Singleton<RInput>.Instance.P.GetButtonUp("Right Trigger") || (PlayerState == State.Result && PlayerState == State.Cutscene)))
			{
				ShieldSources[0].Stop();
				ShieldSources[1].Stop();
				Audio.PlayOneShot(ShieldOffSound, Audio.volume);
				UsingShield = false;
			}
			CanSink = !UsingShield;
		}
		else if (UsingShield)
		{
			ShieldSources[0].Stop();
			ShieldSources[1].Stop();
			UsingShield = false;
		}
		Jump123Sources[1].volume = Mathf.Lerp(Jump123Sources[1].volume, (PlayerState == State.ChainJump && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait Loop")) ? 1f : 0f, Time.deltaTime * 8f);
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
		if (PlayerState == State.JumpDash || PlayerState == State.JumpDashSTH || PlayerState == State.Slide)
		{
			return 0;
		}
		if (PlayerState == State.Homing || PlayerState == State.AfterHoming)
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
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.JumpDashSTH || PlayerState == State.Slide) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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
		if (Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int j = 0; j < Upgrades.Renderers.Count; j++)
			{
				Upgrades.Renderers[j].enabled = ImmunityTime - Time.time <= 0f || ((ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
			}
		}
	}

	private void UpdateCollider()
	{
		if (PlayerState == State.Slide)
		{
			CapsuleCollider.center = Vector3.zero;
			CapsuleCollider.height = 0.5f;
		}
		else
		{
			CapsuleCollider.center = new Vector3(0f, 0.25f, 0f);
			CapsuleCollider.height = 1f;
		}
	}

	private bool OnJumpDash()
	{
		Audio.PlayOneShot((Singleton<Settings>.Instance.settings.JumpdashType != 2) ? JumpDashSound : JumpDashSTHSound, Audio.volume);
		PlayerVoice.PlayRandom(5, RandomPlayChance: true);
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
		if (!IsDead && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && (PlayerState != State.Lotus || !LockControls))
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

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position + base.transform.up * 0.25f, 1.1f);
	}
}
