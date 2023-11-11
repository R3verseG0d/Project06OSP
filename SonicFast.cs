using System;
using STHLua;
using UnityEngine;

public class SonicFast : PlayerBase
{
	public enum State
	{
		Land = 0,
		Ground = 1,
		Jump = 2,
		Air = 3,
		LightDash = 4,
		Slide = 5,
		BoundAttack = 6,
		Hurt = 7,
		WallSlam = 8,
		AirSlam = 9,
		Death = 10,
		FallDeath = 11,
		DrownDeath = 12,
		SnowBallDeath = 13,
		TornadoDeath = 14,
		Talk = 15,
		Path = 16,
		WarpHole = 17,
		Result = 18,
		Cutscene = 19,
		DashPanel = 20,
		Spring = 21,
		WideSpring = 22,
		JumpPanel = 23,
		DashRing = 24,
		RainbowRing = 25,
		ChainJump = 26,
		Hold = 27,
		UpReel = 28,
		Balancer = 29
	}

	[Header("Player Framework")]
	public SonicFastEffects SonicFastEffects;

	public AnimationCurve RotationOverSpeed;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	internal bool UseSpeedBarrier;

	private float Steering;

	[Header("Player Models")]
	public RuntimeAnimatorController TGSAnimator;

	public SkinnedMeshRenderer[] PlayerRenderers;

	public UpgradeModels Upgrades;

	internal bool IsSuper;

	private float BodyDirDot;

	private float BlinkTimer;

	private float RingDrainTimer;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip SlideSound;

	public AudioClip BoundStart;

	public AudioClip SlamSound;

	[Header("Audio Sources")]
	public AudioSource Jump123Source;

	public AudioSource[] WindSpeedSources;

	public AudioSource SuperAuraSource;

	private float LandTimer;

	private float JumpTime;

	private bool ReleasedKey;

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

	private float SlideEndTimer;

	private float SlideSpeed;

	private bool SlideReleasedKey;

	internal int BoundState;

	private float BoundTime;

	private float HurtTime;

	private float HurtStartSpeed;

	private float SlamTime;

	private float AirSlamTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 1.75f;
		PlayerName = Sonic_Fast_Lua.c_player_name;
		PlayerNameShort = Sonic_Fast_Lua.c_player_name_short;
		WalkSpeed = Sonic_Fast_Lua.c_slow_speed_max;
		TopSpeed = Sonic_Fast_Lua.c_walk_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.CloseGauge();
	}

	private void StateLandStart()
	{
		PlayerState = State.Land;
		if (StageManager._Stage == StageManager.Stage.csc && StageManager.StageSection == StageManager.Section.E)
		{
			PlayAnimation("Land CSC", "On Land CSC");
		}
		else if (StageManager._Stage == StageManager.Stage.kdv && StageManager.StageSection == StageManager.Section.C)
		{
			PlayAnimation("Land KDV", "On Land KDV");
		}
		else
		{
			PlayAnimation("Land", "On Land");
		}
		MaxRayLenght = 1.75f;
		LandTimer = Time.time;
	}

	private void StateLand()
	{
		PlayerState = State.Land;
		LockControls = true;
		CurSpeed = Sonic_Fast_Lua.c_slow_speed_max;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - LandTimer > ((StageManager._Stage == StageManager.Stage.csc && StageManager.StageSection == StageManager.Section.E) ? 1.5f : 1f))
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateLandEnd()
	{
	}

	private void StateGroundStart()
	{
		PlayerState = State.Ground;
		MaxRayLenght = 1.75f;
	}

	private void StateGround()
	{
		PlayerState = State.Ground;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (FrontalCollision)
		{
			if (!IsOnWall)
			{
				if (CurSpeed > 48.75f)
				{
					OnWallSlam(Grounded: true);
				}
				else
				{
					CurSpeed = 0f;
				}
			}
			else
			{
				OnWallSlam(Grounded: false);
			}
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateGroundEnd()
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
		AirMotionVelocity = Vector3.up * Sonic_Fast_Lua.c_jump_speed;
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
		if (AirMotionVelocity.y > Common_Lua.c_vel_y_max)
		{
			AirMotionVelocity.y = 0f;
		}
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Sonic_Fast_Lua.c_jump_time_min) ? 1 : 0));
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!HalveSinkJump) ? 0.4f : 0.25f))
		{
			AirMotionVelocity += Vector3.up * ((!HalveSinkJump) ? 12.25f : 11f) * Time.fixedDeltaTime * 2f;
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
		if (FrontalCollision)
		{
			if (CurSpeed > 48.75f)
			{
				OnWallSlam(Grounded: false);
			}
			else
			{
				CurSpeed = 0f;
			}
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
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
		if (AirMotionVelocity.y > Common_Lua.c_vel_y_max)
		{
			AirMotionVelocity.y = 0f;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (FrontalCollision)
		{
			if (CurSpeed > 48.75f)
			{
				OnWallSlam(Grounded: false);
			}
			else
			{
				CurSpeed = 0f;
			}
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateAirEnd()
	{
	}

	private void StateLightDashStart()
	{
		PlayerState = State.LightDash;
		MaxRayLenght = 2.75f;
		int num = ClosestLightDashRing(Sonic_Fast_Lua.c_lockon_lightdash.radius);
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
		Speed = Mathf.Min(Speed + Sonic_Fast_Lua.c_lightdash_speed * Time.fixedDeltaTime * 10f, Sonic_Fast_Lua.c_lightdash_speed);
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
			CurSpeed = ((!(VerticalDir() > 0f)) ? Sonic_Fast_Lua.c_run_speed_max : ((!IsSuper) ? Sonic_Fast_Lua.c_lightdash_mid_speed : Sonic_Fast_Lua.c_lightdash_mid_speed_super));
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
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
		MaxRayLenght = 1.75f;
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
		PlayAnimation("Sliding Attack", "On Sliding Attack");
		if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
		{
			SlideReleasedKey = true;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		SlideSpeed = Mathf.Clamp(SlideSpeed, Sonic_Fast_Lua.c_walk_speed_max, Sonic_Fast_Lua.c_lightdash_speed);
		SlideSpeed -= 12f * Time.fixedDeltaTime;
		CurSpeed = SlideSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position, Sonic_New_Lua.c_sliding_collision.radius, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, Sonic_New_Lua.c_sliding_damage);
		if (Time.time - SlideEndTimer > Sonic_New_Lua.c_sliding_time)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (FrontalCollision)
		{
			if (!IsOnWall)
			{
				if (CurSpeed > 48.75f)
				{
					OnWallSlam(Grounded: true);
				}
				else
				{
					CurSpeed = 0f;
				}
			}
			else
			{
				OnWallSlam(Grounded: false);
			}
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateSlideEnd()
	{
	}

	private void StateBoundAttackStart()
	{
		PlayerState = State.BoundAttack;
		BoundState = 0;
		BoundTime = 0f;
		AirMotionVelocity = _Rigidbody.velocity;
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
			AirMotionVelocity.y = Sonic_Fast_Lua.c_boundjump_jmp;
			if (IsGrounded())
			{
				SonicFastEffects.CreateBoundAttackFX();
				BoundTime = Time.time;
				BoundState = 1;
				AttackSphere_Dir(base.transform.position, Sonic_Fast_Lua.c_boundattack_collision.radius * 2f, 20f, Sonic_Fast_Lua.c_boundattack_damage);
			}
			else
			{
				AttackSphere_Dir(base.transform.position, Sonic_Fast_Lua.c_boundattack_collision.radius, 20f, Sonic_Fast_Lua.c_boundattack_damage);
			}
		}
		else if (BoundState == 1)
		{
			PlayAnimation((Time.time - BoundTime > 0.25f && AirMotionVelocity.y < 0.25f) ? "Falling" : "Rolling", (Time.time - BoundTime > 0.25f && AirMotionVelocity.y < 0.25f) ? "On Fall" : "On Roll");
			if (Time.time - BoundTime < 0.25f)
			{
				AirMotionVelocity.y = 4f;
				if (CurSpeed > Sonic_Fast_Lua.c_walk_speed_max * 0.8f)
				{
					CurSpeed = Sonic_Fast_Lua.c_walk_speed_max * 0.8f;
				}
				AttackSphere_Dir(base.transform.position, Sonic_Fast_Lua.c_boundattack_collision.radius, 10f, Sonic_Fast_Lua.c_boundattack_damage);
			}
			else if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				StateMachine.ChangeState(StateGround);
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

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		PlayAnimation("Hurt", "On Hurt");
		HurtTime = Time.time;
		HurtStartSpeed = CurSpeed;
		PlayerVoice.PlayRandom(4);
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		float maxDelta = Mathf.Clamp(HurtTime, 0f, 1.5f);
		HurtStartSpeed = Mathf.MoveTowards(HurtStartSpeed, Sonic_Fast_Lua.c_slow_speed_max, maxDelta);
		CurSpeed = HurtStartSpeed;
		_Rigidbody.velocity = base.transform.forward * CurSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (FrontalCollision)
		{
			if (!IsOnWall)
			{
				if (CurSpeed > 48.75f)
				{
					OnWallSlam(Grounded: true);
				}
				else
				{
					CurSpeed = 0f;
				}
			}
			else
			{
				OnWallSlam(Grounded: false);
			}
		}
		if (Time.time - HurtTime > 1.5f || !IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateHurtEnd()
	{
	}

	private void StateWallSlamStart()
	{
		PlayerState = State.WallSlam;
		PlayAnimation("Wall Slam", "On Wall Slam");
		SlamTime = Time.time;
		Audio.PlayOneShot(SlamSound, Audio.volume);
		PlayerVoice.PlayRandom(4);
	}

	private void StateWallSlam()
	{
		PlayerState = State.WallSlam;
		LockControls = true;
		float num = Mathf.Clamp(Time.time - SlamTime, 0f, 1f);
		CurSpeed = Mathf.Lerp(0f - Sonic_Fast_Lua.c_slow_speed_max, 0f, num * 1.25f);
		_Rigidbody.velocity = base.transform.forward * CurSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (Time.time - SlamTime > 0.75f || !IsGrounded())
		{
			CurSpeed = 17f;
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateWallSlamEnd()
	{
	}

	private void StateAirSlamStart()
	{
		PlayerState = State.AirSlam;
		PlayAnimation("Air Slam", "On Air Slam");
		AirSlamTime = Time.time;
		Audio.PlayOneShot(SlamSound, Audio.volume);
		PlayerVoice.PlayRandom(4);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAirSlam()
	{
		PlayerState = State.AirSlam;
		LockControls = true;
		float t = Mathf.Clamp(Time.time - AirSlamTime, 0f, 1f);
		CurSpeed = Mathf.Lerp(0f - Sonic_Fast_Lua.c_slow_speed_max, 0f, t);
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
			CurSpeed = 0f;
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateAirSlamEnd()
	{
	}

	private bool Invert()
	{
		if (Vector3.Dot(base.transform.forward, Camera.transform.forward) < 0f && StageManager._Stage == StageManager.Stage.csc)
		{
			if (Camera.CameraState != PlayerCamera.State.Event || (Camera.parameters.Mode != 5 && Camera.parameters.Mode != 50))
			{
				return Camera.CameraState == PlayerCamera.State.EventFadeOut;
			}
			return true;
		}
		return false;
	}

	public float HorizontalDir()
	{
		return Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (Invert() ? (-1f) : 1f);
	}

	private float VerticalDir()
	{
		return Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * (Invert() ? (-1f) : 1f);
	}

	private void ControlSpeed()
	{
		float num = Sonic_Fast_Lua.c_run_acc * Time.fixedDeltaTime;
		float num2 = Sonic_Fast_Lua.c_run_acc * 0.65f * Time.fixedDeltaTime;
		if (VerticalDir() > 0f)
		{
			if (CurSpeed > ((!IsSuper) ? Sonic_Fast_Lua.c_lightdash_mid_speed : Sonic_Fast_Lua.c_lightdash_mid_speed_super))
			{
				CurSpeed -= num2;
			}
			else if (CurSpeed < Sonic_Fast_Lua.c_run_speed_max)
			{
				CurSpeed += num;
			}
		}
		else if (VerticalDir() < 0f)
		{
			if (CurSpeed > Sonic_Fast_Lua.c_slow_speed_max)
			{
				CurSpeed -= num;
			}
			else if (CurSpeed < Sonic_Fast_Lua.c_slow_speed_max)
			{
				CurSpeed += num;
			}
		}
		else if (CurSpeed < Sonic_Fast_Lua.c_walk_speed_max)
		{
			CurSpeed += num;
		}
		else if (CurSpeed > Sonic_Fast_Lua.c_walk_speed_max)
		{
			CurSpeed -= num;
		}
	}

	private void ControlRotation()
	{
		float num = ((IsGrounded() && PlayerState != State.Slide) ? RotOverSpdCurve.Evaluate(CurSpeed) : 1f);
		Vector3.Dot(base.transform.forward, Camera.transform.forward);
		Quaternion quaternion = Quaternion.Euler(base.transform.eulerAngles + new Vector3(0f, HorizontalDir() * num, 0f));
		quaternion = Quaternion.FromToRotation(Vector3.up, RaycastHit.normal) * quaternion;
		base.transform.rotation = quaternion;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState == State.Land || PlayerState == State.Ground || PlayerState == State.Slide || PlayerState == State.Hurt || PlayerState == State.WallSlam || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (UseSpeedBarrier)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, (!IsSuper) ? 2f : 4f, base.transform.forward * CurSpeed, 1, "SuperSpeed");
		}
		if (PlayerState == State.ChainJump)
		{
			AttackSphere(base.transform.position + base.transform.up * 0.25f, 3f, base.transform.forward * CurSpeed, 1);
		}
		if (IsSuper)
		{
			JumpAttackSphere(base.transform.position + base.transform.up * 0.25f, 0.8f, base.transform.forward * _Rigidbody.velocity.magnitude, 5);
			AttractSphere((!UseSpeedBarrier) ? 1f : 3f);
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				ControlRotation();
				ControlSpeed();
			}
			else
			{
				LockControls = false;
			}
		}
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		UpdateCollider();
		if (PlayerState == State.Land || PlayerState == State.Ground || PlayerState == State.Slide || PlayerState == State.Hurt || PlayerState == State.WallSlam || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
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
		UseSpeedBarrier = (PlayerState == State.Ground || PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.LightDash) && CurSpeed >= 85f;
		CanSink = !IsSuper;
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (PlayerState == State.Ground && CanJumpFromSink())
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
				if (CanLightDash(Sonic_Fast_Lua.c_lockon_lightdash.radius) && Singleton<RInput>.Instance.P.GetButtonDown("Button Y") && !IsSinking)
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (CurSpeed > Sonic_Fast_Lua.c_slow_speed_max && Singleton<RInput>.Instance.P.GetButtonDown("Button B") && !IsSinking)
				{
					StateMachine.ChangeState(StateSlide);
				}
			}
			if (PlayerState == State.Slide)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
				if (CanLightDash(Sonic_Fast_Lua.c_lockon_lightdash.radius) && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (SlideReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StateGround);
				}
			}
			if (PlayerState == State.Air || PlayerState == State.Jump || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
			{
				if (CanLightDash(Sonic_Fast_Lua.c_lockon_lightdash.radius) && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateBoundAttack);
				}
			}
			if (PlayerState == State.BoundAttack && CanLightDash(Sonic_Fast_Lua.c_lockon_lightdash.radius) && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
			{
				StateMachine.ChangeState(StateLightDash);
			}
			if (IsSuper && HUD.Rings > 0 && PlayerState != State.Result)
			{
				RingDrainTimer += Time.deltaTime;
				if (RingDrainTimer >= 1f)
				{
					RingDrainTimer = 0f;
					Singleton<GameManager>.Instance._PlayerData.rings--;
				}
			}
			if (HUD.Rings <= 0)
			{
				Detransform();
			}
		}
		WindSpeedSources[0].volume = Mathf.Lerp(WindSpeedSources[0].volume, (!LockControls && CurSpeed >= 30f && CurSpeed <= 55f) ? 0.4f : 0f, Time.deltaTime * 4f);
		WindSpeedSources[1].volume = Mathf.Lerp(WindSpeedSources[1].volume, (!LockControls && CurSpeed >= 42.5f && CurSpeed <= 67.5f) ? 0.6f : 0f, Time.deltaTime * 4f);
		WindSpeedSources[2].volume = Mathf.Lerp(WindSpeedSources[2].volume, (!LockControls && CurSpeed >= 55f) ? 0.8f : 0f, Time.deltaTime * 4f);
		Jump123Source.volume = Mathf.Lerp(Jump123Source.volume, (PlayerState == State.ChainJump && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait") && !Animator.GetCurrentAnimatorStateInfo(0).IsName("Chain Jump Wall Wait Loop")) ? 1f : 0f, Time.deltaTime * 8f);
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
				IsSuper = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool;
			}
		}
	}

	public void Detransform()
	{
		if (IsSuper)
		{
			SonicFastEffects.CreateDetransformFX();
			IsSuper = false;
		}
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump)
		{
			return 0;
		}
		if (PlayerState == State.Slide || PlayerState == State.BoundAttack || UseSpeedBarrier)
		{
			return 1;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if (PlayerState == State.Hurt || PlayerState == State.Talk || PlayerState == State.WarpHole || PlayerState == State.Result || PlayerState == State.ChainJump || PlayerState == State.Cutscene || HasInvincibility || IsSuper || IsDead)
		{
			return true;
		}
		return result;
	}

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		Steering = Mathf.MoveTowards(Steering, UseSpeedBarrier ? HorizontalDir() : 0f, Time.deltaTime * 3.5f);
		Animator.SetFloat("Steering", Steering);
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
			float num = Vector3.Dot(normalized.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 10f, CurSpeed / WalkSpeed);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Slide) && !LockControls && CurSpeed > 0f) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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
			}
		}
		PlayerRenderers[0].enabled = (!IsSuper && ImmunityTime - Time.time <= 0f) || ((!IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[1].enabled = (IsSuper && ImmunityTime - Time.time <= 0f) || ((IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[2].enabled = (!IsSuper && ImmunityTime - Time.time <= 0f) || ((!IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false);
		PlayerRenderers[3].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 0 && ((IsSuper && ImmunityTime - Time.time <= 0f) || ((IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
		PlayerRenderers[4].enabled = Singleton<Settings>.Instance.settings.GemShoesType != 1 && ((IsSuper && ImmunityTime - Time.time <= 0f) || ((IsSuper && ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false));
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
			CapsuleCollider.center = Vector3.MoveTowards(CapsuleCollider.center, new Vector3(0f, 0.25f, 0f), Time.deltaTime * 2f);
			CapsuleCollider.height = Mathf.MoveTowards(CapsuleCollider.height, 1f, Time.deltaTime * 4f);
		}
	}

	public void OnWallSlam(bool Grounded)
	{
		int rings = Singleton<GameManager>.Instance._PlayerData.rings;
		if ((rings > 0 && ImmunityTime - Time.time <= 0f) || ImmunityTime - Time.time > 0f)
		{
			if (Grounded)
			{
				StateMachine.ChangeState(StateWallSlam);
			}
			else
			{
				StateMachine.ChangeState(StateAirSlam);
			}
		}
		if (!(CurSpeed >= Sonic_Fast_Lua.c_run_speed_max * 0.9f) || !(ImmunityTime - Time.time <= 0f))
		{
			return;
		}
		if (rings > 0 || HasShield)
		{
			if (!IsSuper && !HasInvincibility)
			{
				if (HasShield)
				{
					RemoveShield();
					return;
				}
				ImmunityTime = Time.time + Common_Lua.c_invincible_time;
				CreateRings(Mathf.Min(rings, 20), IsSuperSpeed: true);
				Singleton<GameManager>.Instance._PlayerData.rings = 0;
			}
		}
		else
		{
			OnDeathEnter(0);
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
			if (HasShield)
			{
				RemoveShield();
				return;
			}
			CreateRings(Mathf.Min(rings, 20), IsSuperSpeed: true);
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
			Detransform();
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
		case "StateLand":
			StateMachine.ChangeState(StateLand);
			break;
		case "StateGround":
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
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
		if (Singleton<Settings>.Instance.settings.TGSSonic == 1)
		{
			Animator.runtimeAnimatorController = TGSAnimator;
		}
		if (!TalkState)
		{
			StateLandStart();
			StateMachine.Initialize(StateLand);
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
