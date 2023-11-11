using System;
using STHLua;
using UnityEngine;

public class MetalSonic : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		JumpDash = 4,
		LightDash = 5,
		SlowFall = 6,
		Boost = 7,
		Homing = 8,
		AfterHoming = 9,
		SpinDash = 10,
		Hurt = 11,
		EdgeDanger = 12,
		WaterSlide = 13,
		Grinding = 14,
		Death = 15,
		FallDeath = 16,
		DrownDeath = 17,
		SnowBallDeath = 18,
		TornadoDeath = 19,
		Talk = 20,
		Path = 21,
		WarpHole = 22,
		Result = 23,
		Cutscene = 24,
		DashPanel = 25,
		Spring = 26,
		WideSpring = 27,
		JumpPanel = 28,
		DashRing = 29,
		RainbowRing = 30,
		ChainJump = 31,
		Orca = 32,
		Pole = 33,
		Float = 34,
		Rope = 35,
		Hold = 36,
		UpReel = 37,
		Balancer = 38
	}

	[Header("Player Framework")]
	public MetalSonicEffects MetalSonicEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private bool UsingDPad;

	private string LaunchMode;

	[Header("Player Models")]
	public SkinnedMeshRenderer PlayerRenderer;

	private float BodyDirDot;

	private float BlinkTimer;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip JumpDashKickback;

	public AudioClip SpinDashShoot;

	public AudioClip BrakeSound;

	[Header("Audio Sources")]
	public AudioSource[] SpinDashSources;

	public AudioSource WaterRunSource;

	public AudioSource[] Jump123Sources;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private float JumpDashStartTime;

	private float JumpDashLength;

	private float JumpDashSpeed;

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

	private Vector3 SpindashForward;

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

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Metal_Sonic_Lua.c_player_name;
		PlayerNameShort = Metal_Sonic_Lua.c_player_name_short;
		WalkSpeed = Metal_Sonic_Lua.c_walk_speed_max;
		TopSpeed = Metal_Sonic_Lua.c_run_speed_max;
		BrakeSpeed = Metal_Sonic_Lua.c_brake_acc;
		GrindSpeedOrg = Metal_Sonic_Lua.c_grind_speed_org;
		GrindAcc = Metal_Sonic_Lua.c_grind_acc;
		GrindSpeedMax = Metal_Sonic_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Metal_Sonic_Lua.OpenGauge(), Metal_Sonic_Lua.c_gauge_max, 0f, 0f);
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Metal_Sonic_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Metal_Sonic_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Metal_Sonic_Lua.c_run_speed_max) / Metal_Sonic_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Metal_Sonic_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Metal_Sonic_Lua.c_run_speed_max;
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
		JumpTime = Time.time;
		JumpAnimation = 0;
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Metal_Sonic_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Metal_Sonic_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Metal_Sonic_Lua.c_jump_time_min) ? 1 : 0));
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

	private void StateJumpDashStart()
	{
		PlayerState = State.JumpDash;
		MaxRayLenght = 0.75f;
		JumpDashLength = ((Singleton<Settings>.Instance.settings.JumpdashType == 0) ? Metal_Sonic_Lua.c_homing_time : Metal_Sonic_Lua.c_homing_e3_time);
		JumpDashStartTime = Time.time;
		JumpDashSpeed = Metal_Sonic_Lua.c_homing_spd;
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
		JumpDashSpeed -= Metal_Sonic_Lua.c_homing_brake * 0.75f * Time.deltaTime;
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
		if (AttackSphere(base.transform.position, Metal_Sonic_Lua.c_collision_homing(), base.transform.forward * CurSpeed, Metal_Sonic_Lua.c_homing_damage))
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
		Speed = Mathf.Min(Speed + Metal_Sonic_Lua.c_lightdash_speed * Time.fixedDeltaTime * 10f, Metal_Sonic_Lua.c_lightdash_speed);
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
				CurSpeed = (Metal_Sonic_Lua.c_run_speed_max + Metal_Sonic_Lua.c_lightdash_speed) / 1.75f;
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

	private void StateBoostStart()
	{
		PlayerState = State.Boost;
	}

	private void StateBoost()
	{
		PlayerState = State.Boost;
		PlayAnimation("Boost", "On Boost");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = Metal_Sonic_Lua.c_boost_spd;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (!IsSlopePhys && Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Metal_Sonic_Lua.c_walk_speed_max * 1.5f)
		{
			if (ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateBrake);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
		if (FrontalCollision || HUD.ActionDisplay < 0.1f || !Singleton<RInput>.Instance.P.GetButton("Button B"))
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateBoostEnd()
	{
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
		if (AttackSphere(base.transform.position + base.transform.forward * 0.25f, Metal_Sonic_Lua.c_collision_homing(), DirectionToTarget * Metal_Sonic_Lua.c_homing_power, Metal_Sonic_Lua.c_homing_damage))
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
		Audio.PlayOneShot(JumpDashKickback, Audio.volume);
		AHEnterTime = Time.time;
		HAPressed = true;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
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
	}

	private void StateSpinDashStart()
	{
		PlayerState = State.SpinDash;
		SpinDashState = 0;
		SpinDashSources[1].volume = 0f;
		SpinDashSources[0].Play();
		SpinDashSources[1].Play();
		SpindashForward = base.transform.forward;
		SpindashSpd = Mathf.Min(Mathf.Max(CurSpeed, Metal_Sonic_Lua.c_walk_speed_max * 4f), Metal_Sonic_Lua.c_spindash_spd * 1.5f);
	}

	private void StateSpinDash()
	{
		PlayerState = State.SpinDash;
		PlayAnimation((SpinDashState == 0) ? "Rolling" : "Spin", (SpinDashState == 0) ? "On Roll" : "On Spin");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (SpinDashState == 0)
		{
			CurSpeed = Mathf.MoveTowards(CurSpeed, 0f, Time.fixedDeltaTime * 75f);
			SpindashSpd = Mathf.MoveTowards(SpindashSpd, Metal_Sonic_Lua.c_spindash_spd * 1.5f, Time.fixedDeltaTime * 35f);
			if (CurSpeed <= 0f)
			{
				CurSpeed = 0f;
			}
			_Rigidbody.velocity = Vector3.ProjectOnPlane(SpindashForward, RaycastHit.normal) * CurSpeed;
			if (!SpinDashSources[0].isPlaying)
			{
				SpinDashSources[1].volume = 1f;
			}
		}
		else
		{
			SpinDashSources[0].Stop();
			SpinDashSources[1].Stop();
			CurSpeed += (Vector3.Dot(new Vector3(0f, -0.5f, 0f), base.transform.forward) - 0.05f) * 2f;
			CurSpeed = Mathf.Clamp(CurSpeed, 0f, Metal_Sonic_Lua.c_run_speed_max * 2f);
			_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (FrontalCollision)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		AttackSphere(base.transform.position, 1f, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, 1);
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
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Metal_Sonic_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Metal_Sonic_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Metal_Sonic_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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

	private void StateWaterSlideStart()
	{
		PlayerState = State.WaterSlide;
		WSpeed = Mathf.Min(CurSpeed, Metal_Sonic_Lua.c_run_speed_max * 1.5f);
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
		if (WSpeed > 0f)
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

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState != State.Homing)
		{
			HomingTarget = FindHomingTarget();
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Boost || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Metal_Sonic_Lua.c_rotation_speed);
				if (PlayerState != State.SpinDash)
				{
					AccelerationSystem((!HasSpeedUp) ? Metal_Sonic_Lua.c_run_acc : Metal_Sonic_Lua.c_speedup_acc);
				}
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Metal_Sonic_Lua.c_walk_speed_max : Metal_Sonic_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Metal_Sonic_Lua.c_speedup_speed_max : (IsGrounded() ? Metal_Sonic_Lua.c_run_speed_max : Metal_Sonic_Lua.c_jump_run));
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
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Boost || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
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
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !IsSinking)
				{
					StateMachine.ChangeState(StateSpinDash);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && HUD.ActionDisplay > 20f && !IsSinking)
				{
					StateMachine.ChangeState(StateBoost);
				}
			}
			if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if (PlayerState == State.WaterSlide && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.Rope && !LockControls))
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
			if (PlayerState == State.SpinDash)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
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
					Audio.PlayOneShot(SpinDashShoot, Audio.volume);
					CurSpeed = SpindashSpd;
					CurSpeed += _Rigidbody.velocity.magnitude * 0.6f;
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
		}
		WaterRunSource.volume = Mathf.Lerp(WaterRunSource.volume, (PlayerState == State.WaterSlide) ? 1f : 0f, Time.deltaTime * 6f);
		Jump123Sources[1].volume = Mathf.Lerp(Jump123Sources[1].volume, (PlayerState == State.ChainJump && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wait")) ? 1f : 0f, Time.deltaTime * 8f);
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
		if (PlayerState == State.Jump || PlayerState == State.JumpDash)
		{
			return 0;
		}
		if (PlayerState == State.Homing || PlayerState == State.AfterHoming || (PlayerState == State.SpinDash && SpinDashState == 1))
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
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Boost || (PlayerState == State.SpinDash && SpinDashState == 1)) && !LockControls && CurSpeed > 0f && !WalkSwitch) || flag || PlayerState == State.Balancer)) ? ((0f - num) * ((!flag) ? num2 : 8f)) : 0f, 10f * Time.fixedDeltaTime);
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
		PlayerRenderer.enabled = (Singleton<Settings>.Instance.settings.SpinEffect != 1 || PlayerState != State.SpinDash || SpinDashState != 0) && (ImmunityTime - Time.time <= 0f || ((ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
	}

	private void UpdateCollider()
	{
		if (PlayerState == State.SpinDash)
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
		if (!HomingTarget)
		{
			StateMachine.ChangeState(StateJumpDash);
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
		if (!IsDead && PlayerState != State.WaterSlide && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && PlayerState != State.Hold)
		{
			IsDead = true;
			Camera.OnPlayerDeath();
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
