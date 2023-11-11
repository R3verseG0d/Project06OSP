using System;
using System.Collections;
using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Shadow : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		JumpDash = 4,
		JumpDashSTH = 5,
		SlowFall = 6,
		Homing = 7,
		AfterHoming = 8,
		ChaosAttack = 9,
		Tornado = 10,
		TornadoReturn = 11,
		TrickJump = 12,
		ChaosSpear = 13,
		LightDash = 14,
		SpinDash = 15,
		ChaosBoost = 16,
		Uninhibit = 17,
		UninhibitBreak = 18,
		ChaosBlast = 19,
		Hurt = 20,
		EdgeDanger = 21,
		Grinding = 22,
		Death = 23,
		FallDeath = 24,
		DrownDeath = 25,
		SnowBallDeath = 26,
		TornadoDeath = 27,
		Talk = 28,
		Path = 29,
		WarpHole = 30,
		Result = 31,
		Cutscene = 32,
		Vehicle = 33,
		DashPanel = 34,
		Spring = 35,
		WideSpring = 36,
		JumpPanel = 37,
		DashRing = 38,
		RainbowRing = 39,
		ChainJump = 40,
		Pole = 41,
		Float = 42,
		Rope = 43,
		Hold = 44,
		UpReel = 45,
		Balancer = 46,
		TrainRepel = 47
	}

	[Header("Player Framework")]
	public ShadowEffects ShadowEffects;

	internal VehicleBase CurVehicle;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	internal bool IsChaosBoost;

	internal bool IsFullPower;

	internal bool IsRestrained;

	internal bool UseChaosSnap;

	internal bool HasLightMemoryShard;

	internal int ChaosBoostLevel;

	private bool TriggerPressed;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BodyDirDot;

	private float BlinkTimer;

	[Header("Chaos Abilities")]
	public GameObject ChaosSpear;

	public GameObject ChaosBlast;

	internal List<GameObject> SpearTargets;

	[Header("Full Power Mode")]
	public Animator[] LimiterAnimators;

	public Transform[] LimiterPoints;

	public Collider[] LimiterCols;

	public Rigidbody[] LimiterRBs;

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

	public AudioClip AwakeSound;

	public AudioClip WarpSound;

	public AudioClip SmashSound;

	public AudioClip SnapSound;

	public AudioClip SpearShootSound;

	public AudioClip[] LimiterSounds;

	public AudioClip FullPowerTransformSound;

	public AudioClip[] RestrainSounds;

	[Header("Audio Sources")]
	public AudioSource[] ChaosSpearSources;

	public AudioSource[] SpinDashSources;

	public AudioSource Jump123Source;

	public AudioSource ChaosBoostSource;

	public AudioSource UpgradeHoldSource;

	public AudioSource FullPowerSource;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private float JumpDashStartTime;

	private float JumpDashLength;

	private float JumpDashSpeed;

	private int JumpdashIndex;

	private bool HoldSnapDash;

	private float JumpDashSTHStartTime;

	private float JumpDashSTHLength;

	private Vector3 HomingDirection;

	private Vector3 DirectionToTarget;

	private Vector3 HAForward;

	private Vector3 HAStartPos;

	private float HAStartTime;

	private float HomingTime;

	private string HATag;

	private bool IsEnemyAndHasHealth;

	private bool IsEnemy;

	private bool IsEggmanTrain;

	private bool EnemyHasHealth;

	private bool DontSnap;

	private bool IsCA;

	private float AHEnterTime;

	private bool HAPressed;

	private int AfterHomingTrick;

	private bool AttackDealDamage;

	private bool ChaosAttackDamage;

	private bool SnapChainReleased;

	private bool CancelSnapChain;

	private bool ContinueAttackCount;

	private int AttackCount;

	private int ChainCount;

	private float ChaosAttackTime;

	private GameObject ChaosAttackTarget;

	private bool TornadoPressed;

	private int TornadoCount;

	private float TornadoRadius;

	private float TornadoTime;

	private float TornadoTimeAmount;

	private float TornadoWaitTimeAmount;

	internal float FirstKickSpeed;

	private float TornadoReturnTime;

	private float TrickJumpTime;

	private bool ReleasedTrickKey;

	private float SpearReleaseTime;

	internal int SpearState;

	private int SpearAmount;

	private int SpearCount;

	private bool DontAddJumpdashIndex;

	private bool DoAirRecoil;

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

	private float SpindashTime;

	private float SpindashSpd;

	internal int SpinDashState;

	private Vector3 BoostVel;

	private bool BoostActivate;

	private float BoostTime;

	internal int BoostState;

	private Vector3 UninhibitVel;

	private bool UninhibitActivate;

	private float UninhibitTime;

	internal int UninhibitState;

	private float UninhibitBreakTime;

	private bool RestrainLand;

	private bool RestrainStandUp;

	private bool BlastExplode;

	private bool BlastFromGround;

	private float BlastTime;

	private float BlastLerpTime;

	private Vector3 BlastPos;

	private Vector3 BlastNewPos;

	private float HurtTime;

	private float EdgeDangerTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Shadow_Lua.c_player_name;
		PlayerNameShort = Shadow_Lua.c_player_name_short;
		WalkSpeed = Shadow_Lua.c_walk_speed_max;
		TopSpeed = Shadow_Lua.c_run_speed_max;
		BrakeSpeed = Shadow_Lua.c_brake_acc;
		GrindSpeedOrg = Shadow_Lua.c_grind_speed_org;
		GrindAcc = Shadow_Lua.c_grind_acc;
		GrindSpeedMax = Shadow_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Shadow_Lua.OpenGauge(), Shadow_Lua.c_gauge_max, 0f, Shadow_Lua.c_gauge_heal_wait);
		HasLightMemoryShard = Singleton<GameManager>.Instance.GetGameData().HasFlag(Game.MemoryShardLight);
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
		if (!IsSlopePhys && Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Shadow_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Shadow_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeSpd = CurSpeed;
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Shadow_Lua.c_run_speed_max) / Shadow_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Shadow_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Shadow_Lua.c_run_speed_max;
		}
		BrakeStopped = false;
		ShadowEffects.CreateShoeJetCombustFX();
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
			AirMotionVelocity += Vector3.up * Shadow_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Shadow_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Shadow_Lua.c_jump_time_min) ? 1 : 0));
		if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			ReleasedKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!HalveSinkJump) ? 0.7f : 0.45f))
		{
			AirMotionVelocity += Vector3.up * ((!HalveSinkJump) ? 4.25f : 3f) * Time.fixedDeltaTime * 4f;
		}
		int damage = ((!IsFullPower) ? 1 : 10);
		if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, damage))
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
		if (DoAirRecoil)
		{
			AirMotionVelocity.y = 7f;
			if (AfterHomingTrick > 2)
			{
				AfterHomingTrick = 0;
			}
			Animator.SetInteger("HomingAnimID", AfterHomingTrick);
			AfterHomingTrick++;
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAir()
	{
		PlayerState = State.Air;
		if (!DoAirRecoil)
		{
			PlayAnimation("Air Falling", "On Air Fall");
		}
		else
		{
			Animator.SetTrigger("On After Homing");
		}
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
		DoAirRecoil = false;
	}

	private void StateJumpDashStart()
	{
		PlayerState = State.JumpDash;
		MaxRayLenght = 0.75f;
		JumpDashLength = ((Singleton<Settings>.Instance.settings.JumpdashType == 0) ? Shadow_Lua.c_homing_time : Shadow_Lua.c_homing_e3_time);
		JumpDashStartTime = Time.time;
		JumpDashSpeed = Shadow_Lua.c_homing_spd;
		if (Singleton<Settings>.Instance.settings.JumpdashType == 1)
		{
			AirMotionVelocity = _Rigidbody.velocity;
			AirMotionVelocity.y = 0f;
			_Rigidbody.velocity = AirMotionVelocity;
		}
		HoldSnapDash = true;
		JumpdashIndex += 2;
		DontAddJumpdashIndex = false;
	}

	private void StateJumpDash()
	{
		PlayerState = State.JumpDash;
		PlayAnimation("Rolling", "On Roll");
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
		if (HoldSnapDash && !Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			HoldSnapDash = false;
		}
		if (Time.time - JumpDashStartTime > JumpDashLength * ((IsChaosBoost && HoldSnapDash) ? 0.75f : 1f) || FrontalCollision)
		{
			if (IsChaosBoost && HoldSnapDash)
			{
				if (!HomingTarget)
				{
					Audio.PlayOneShot(SnapSound, Audio.volume);
					ShadowEffects.CreateSnapDashFX();
					StateMachine.ChangeState(StateAir);
				}
				else
				{
					OnChaosSnap();
				}
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		if (AttackSphere(base.transform.position, Shadow_Lua.c_collision_homing(), base.transform.forward * CurSpeed, (!IsFullPower) ? Shadow_Lua.c_homing_damage : 10))
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
		CurSpeed = Shadow_Lua.c_homing_spd;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		HoldSnapDash = true;
		JumpdashIndex += 2;
		DontAddJumpdashIndex = false;
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
		if (HoldSnapDash && !Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			HoldSnapDash = false;
		}
		if (Time.time - JumpDashSTHStartTime > JumpDashSTHLength * ((IsChaosBoost && HoldSnapDash) ? 0.75f : 1f) || FrontalCollision)
		{
			if (IsChaosBoost && HoldSnapDash)
			{
				if (!HomingTarget)
				{
					Audio.PlayOneShot(SnapSound, Audio.volume);
					ShadowEffects.CreateSnapDashFX();
					CurSpeed *= 0.75f;
					StateMachine.ChangeState(StateAir);
				}
				else
				{
					OnChaosSnap();
				}
			}
			else
			{
				CurSpeed *= 0.75f;
				StateMachine.ChangeState(StateAir);
			}
		}
		if (AttackSphere(base.transform.position, Shadow_Lua.c_collision_homing(), base.transform.forward * Shadow_Lua.c_homing_spd, (!IsFullPower) ? Shadow_Lua.c_homing_damage : 10))
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
		if (UseChaosSnap)
		{
			Audio.PlayOneShot(WarpSound, Audio.volume);
			ShadowEffects.CreateChaosSnapFX();
		}
		IsEnemyAndHasHealth = HomingTarget.layer == LayerMask.NameToLayer("Enemy") && HomingTarget.GetComponentInParent<EnemyBase>().GetCurHealth() > 0;
		IsEnemy = HomingTarget.layer == LayerMask.NameToLayer("Enemy");
		IsEggmanTrain = HomingTarget.GetComponentInParent<Eggtrain>();
		if (IsEnemy)
		{
			EnemyHasHealth = HomingTarget.GetComponentInParent<EnemyBase>().GetCurHealth() > 0 && !IsFullPower;
		}
		else
		{
			EnemyHasHealth = false;
		}
		DontSnap = (bool)HomingTarget.GetComponentInParent<FlameSingle>() || (bool)HomingTarget.GetComponentInParent<Common_Switch>();
		IsCA = (!UseChaosSnap && (IsEnemyAndHasHealth || IsEggmanTrain)) || (UseChaosSnap && !DontSnap && (IsFullPower ? Singleton<RInput>.Instance.P.GetButton("Button A") : (IsEnemyAndHasHealth || IsEggmanTrain || (!IsEnemyAndHasHealth && !IsEggmanTrain && Singleton<RInput>.Instance.P.GetButton("Button A")))));
		JumpdashIndex += 2;
		DontAddJumpdashIndex = false;
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
		float num2 = Mathf.Clamp(num, 0f, 1f) * (UseChaosSnap ? 1.5f : 1f);
		if (!UseChaosSnap)
		{
			num2 *= num2;
		}
		_Rigidbody.velocity = Vector3.zero;
		Vector3 vector = HomingDirection.MakePlanar();
		if (vector == Vector3.zero)
		{
			vector = HAForward.MakePlanar();
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.LookRotation(vector);
		_Rigidbody.MovePosition(Vector3.Lerp(HAStartPos, HomingTarget.transform.position, num2));
		if (ChaosAttackSphere())
		{
			if (IsCA)
			{
				ChaosAttackTarget = HomingTarget;
				StateMachine.ChangeState(StateChaosAttack);
				if (!UseChaosSnap)
				{
					OnChaosAttack(IsChaosSnap: false, IsFirstHit: true);
				}
			}
			else
			{
				StateMachine.ChangeState(StateAfterHoming);
			}
		}
		else
		{
			IsCA = (!UseChaosSnap && (IsEnemyAndHasHealth || IsEggmanTrain)) || (UseChaosSnap && !DontSnap && (IsFullPower ? Singleton<RInput>.Instance.P.GetButton("Button A") : (IsEnemyAndHasHealth || IsEggmanTrain || (!IsEnemyAndHasHealth && !IsEggmanTrain && Singleton<RInput>.Instance.P.GetButton("Button A")))));
		}
		if (num > 5f || (FrontalCollision && !IsEggmanTrain) || DirectionToTarget.magnitude > 15f)
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHomingEnd()
	{
		MaxRayLenght = 0.75f;
		if (UseChaosSnap)
		{
			ShadowEffects.CreateChaosSnapFX();
		}
		if (PlayerState != State.Grinding)
		{
			HomingDirection = Vector3.zero;
		}
	}

	public bool ChaosAttackSphere()
	{
		HitInfo value = new HitInfo(base.transform, DirectionToTarget * Shadow_Lua.c_homing_power, (!IsFullPower) ? Shadow_Lua.c_homing_damage : 10);
		Collider[] array = Physics.OverlapSphere(base.transform.position + base.transform.forward * 0.25f, Shadow_Lua.c_collision_homing(), AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				if (Vector3.Distance(HomingTarget.transform.position, base.transform.position) > 4f)
				{
					IsCA = false;
				}
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
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

	private void StateChaosAttackStart()
	{
		PlayerState = State.ChaosAttack;
		ChaosAttackTime = Time.time;
		ChaosAttackDamage = true;
		AttackDealDamage = false;
		AttackCount = 0;
		if (ContinueAttackCount)
		{
			AttackCount++;
			ContinueAttackCount = false;
		}
		QueuedPress = false;
		Animator.SetInteger("Attack ID", AttackCount);
		Animator.SetTrigger("On Chaos Attack");
		SnapChainReleased = false;
		if (IsEnemy)
		{
			Audio.PlayOneShot(SmashSound, Audio.volume * 1.5f);
		}
		if (!UseChaosSnap)
		{
			return;
		}
		ChainCount++;
		if (IsEnemy)
		{
			if (!EnemyHasHealth)
			{
				OnChaosAttack(IsChaosSnap: true);
			}
			else
			{
				CancelSnapChain = true;
			}
		}
	}

	private void StateChaosAttack()
	{
		PlayerState = State.ChaosAttack;
		LockControls = ChaosAttackTarget;
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up) * CurSpeed;
		if (AttackDealDamage && ChaosAttackDamage && AttackSphere(ChaosAttackTarget.transform.position, (!UseChaosSnap) ? 1.5f : 3f, base.transform.forward * 20f, IsFullPower ? 10 : ((!UseChaosSnap) ? 1 : 2)))
		{
			ChaosAttackDamage = false;
		}
		if (UseChaosSnap && !CancelSnapChain)
		{
			if (!SnapChainReleased)
			{
				if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
				{
					SnapChainReleased = true;
				}
				if (ChainCount < Shadow_Lua.c_chaos_snap_count && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - ChaosAttackTime > 0.2f && (bool)HomingTarget)
				{
					Audio.PlayOneShot((Singleton<Settings>.Instance.settings.JumpdashType != 2) ? JumpDashSound : JumpDashSTHSound, Audio.volume);
					if (!IsFullPower)
					{
						HUD.DrainActionGauge(Shadow_Lua.c_chaossnap);
					}
					StateMachine.ChangeState(StateHoming);
				}
			}
			else
			{
				StateMachine.ChangeState(StateAfterHoming);
			}
		}
		if (((!UseChaosSnap || (UseChaosSnap && CancelSnapChain && !ChaosAttackTarget)) && Time.time - ChaosAttackTime > 0.5f) || (Time.time - ChaosAttackTime > 0.5f && AttackCount > 4))
		{
			ChainCount = 0;
			StateMachine.ChangeState(StateAfterHoming);
		}
	}

	private void StateChaosAttackEnd()
	{
		CancelSnapChain = false;
	}

	private void StateTornadoStart()
	{
		PlayerState = State.Tornado;
		PlayAnimation("Tornado", "On Tornado");
		TornadoTime = Time.time;
		TornadoCount = 0;
		TornadoRadius = 1f;
		TornadoTimeAmount = 0.375f;
		TornadoWaitTimeAmount = 0.15f;
		TornadoPressed = false;
		FirstKickSpeed = CurSpeed;
		PlayerVoice.PlayRandom(6, RandomPlayChance: true);
		ShadowEffects.CreateTornadoFX(TornadoCount);
	}

	private void StateTornado()
	{
		PlayerState = State.Tornado;
		if (!IsSlopePhys)
		{
			FirstKickSpeed -= Shadow_Lua.c_run_speed_max * (Time.fixedDeltaTime * ((TargetDirection == Vector3.zero) ? 0.75f : 0.2f));
		}
		FirstKickSpeed = Mathf.Clamp(FirstKickSpeed, 0f, FirstKickSpeed);
		CurSpeed = FirstKickSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (TornadoCount == 0)
		{
			TornadoRadius = 1.35f;
			TornadoTimeAmount = 0.375f;
			TornadoWaitTimeAmount = 0.175f;
		}
		else if (TornadoCount == 1)
		{
			TornadoRadius = 2.275f;
			TornadoTimeAmount = 0.35f;
			TornadoWaitTimeAmount = 0.2f;
		}
		else if (TornadoCount == 2)
		{
			TornadoRadius = 3.2f;
			TornadoTimeAmount = ((CurSpeed > 0f) ? 0.55f : 0.6f);
			TornadoWaitTimeAmount = 0.4f;
		}
		AttackSphere_Dir(base.transform.position - base.transform.up * 0.25f, TornadoRadius, 10f, (!IsFullPower) ? 2 : 10);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (Time.time - TornadoTime > TornadoTimeAmount)
		{
			if (!TornadoPressed)
			{
				if (TornadoCount < 2)
				{
					StateMachine.ChangeState(StateTornadoReturn);
				}
				else
				{
					StateMachine.ChangeState(StateGround);
				}
			}
			else
			{
				TornadoTime = Time.time;
				TornadoPressed = false;
			}
		}
		ShadowEffects.ManageTornadoTrail(Time.time - TornadoTime < TornadoTimeAmount * 0.75f);
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateTornadoEnd()
	{
		ShadowEffects.ManageTornadoTrail(Enable: false);
	}

	private void StateTornadoReturnStart()
	{
		PlayerState = State.TornadoReturn;
		TornadoReturnTime = Time.time;
	}

	private void StateTornadoReturn()
	{
		PlayerState = State.TornadoReturn;
		PlayAnimation("Tornado Return", "On Tornado Return");
		if (!IsSlopePhys)
		{
			FirstKickSpeed -= Shadow_Lua.c_run_speed_max * (Time.fixedDeltaTime * ((TargetDirection == Vector3.zero) ? 0.75f : 0.2f));
		}
		FirstKickSpeed = Mathf.Clamp(FirstKickSpeed, 0f, FirstKickSpeed);
		CurSpeed = FirstKickSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		if (!IsGrounded() || Time.time - TornadoReturnTime > ((CurSpeed > 0f) ? 0.525f : 0.65f))
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateTornadoReturnEnd()
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
			AirMotionVelocity += Vector3.up * Shadow_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Shadow_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
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
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedTrickKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - TrickJumpTime < 0.5f)
		{
			AirMotionVelocity += Vector3.up * 4.25f * Time.fixedDeltaTime * 4f;
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

	private void StateChaosSpearStart()
	{
		PlayerState = State.ChaosSpear;
		PlayAnimation("Chaos Spear Start", "On Chaos Spear");
		SpearReleaseTime = Time.time;
		SpearState = 0;
		ChaosSpearSources[1].volume = 0f;
		ChaosSpearSources[0].Play();
		ChaosSpearSources[1].Play();
		PlayerVoice.PlayRandom(8, RandomPlayChance: true);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		SpearCount++;
		if (JumpdashIndex > 1 && !DontAddJumpdashIndex)
		{
			JumpdashIndex--;
			DontAddJumpdashIndex = true;
		}
	}

	private void StateChaosSpear()
	{
		PlayerState = State.ChaosSpear;
		CurSpeed = 0f;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, 0f, Time.fixedDeltaTime * 8f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (SpearState == 0)
		{
			if (Time.time - SpearReleaseTime < 0.2f)
			{
				PlayAnimation("Chaos Spear Start", "On Chaos Spear");
			}
			if (Time.time - SpearReleaseTime <= Shadow_Lua.c_chaos_spear_accumulate_wait && SpearTargets.Count == 0)
			{
				SpearAmount = 1;
			}
			else if (IsChaosBoost && ChaosBoostLevel > 1)
			{
				if (!IsFullPower)
				{
					if (Time.time - SpearReleaseTime <= Shadow_Lua.c_chaos_spear_accumulate_wait && SpearTargets.Count > 0)
					{
						SpearAmount = SpearTargets.Count;
					}
					else
					{
						SpearAmount = 1;
					}
				}
				else if (SpearTargets.Count > 0)
				{
					SpearAmount = SpearTargets.Count;
				}
				else
				{
					SpearAmount = 1;
				}
			}
			else if (SpearTargets.Count > 0)
			{
				SpearAmount = SpearTargets.Count;
			}
			else
			{
				SpearAmount = Shadow_Lua.c_chaos_spread_count;
			}
			if (Time.time - SpearReleaseTime > 0.1f && !Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				PlayAnimation("Chaos Spear Shoot", "On Chaos Spear Shoot");
				Audio.PlayOneShot(SpearShootSound, Audio.volume);
				PlayerVoice.PlayRandom(2, RandomPlayChance: true);
				for (int i = 0; i < SpearAmount; i++)
				{
					Vector3 euler = new Vector3(10f, 0f, 0f);
					if (SpearAmount > 1)
					{
						euler.x += UnityEngine.Random.Range(-7.5f, 7.5f);
						euler.y += UnityEngine.Random.Range(-7.5f, 7.5f);
					}
					ChaosSpear component = UnityEngine.Object.Instantiate(ChaosSpear, base.transform.position + base.transform.up * 0.5f + base.transform.forward * 0.25f, base.transform.rotation * Quaternion.Euler(euler)).GetComponent<ChaosSpear>();
					component.Player = base.transform;
					component.ChaosLance = (!IsFullPower && IsChaosBoost && ChaosBoostLevel > 1) || IsFullPower;
					component.ClosestTarget = ((SpearTargets.Count > 0) ? SpearTargets[i] : null);
					component.FullPower = IsFullPower;
					if (Time.time - SpearReleaseTime >= Shadow_Lua.c_chaos_spear_accumulate_wait)
					{
						component.DamageEnemies = true;
						if (component.ChaosLance && !IsFullPower)
						{
							HUD.DrainActionGauge(Shadow_Lua.c_chaoslance);
							component.MaxPower = true;
						}
					}
					if (IsFullPower)
					{
						component.MaxPower = true;
					}
				}
				SpearReleaseTime = Time.time;
				SpearState = 1;
			}
			if (!ChaosSpearSources[0].isPlaying)
			{
				ChaosSpearSources[1].volume = 1f;
			}
		}
		else
		{
			ChaosSpearSources[0].Stop();
			ChaosSpearSources[1].Stop();
			if (Time.time - SpearReleaseTime > 0.3f)
			{
				DoAirRecoil = true;
				StateMachine.ChangeState(StateAir);
			}
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateChaosSpearEnd()
	{
		ChaosSpearSources[0].Stop();
		ChaosSpearSources[1].Stop();
		SpearTargets = null;
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
		Speed = Mathf.Min(Speed + Shadow_Lua.c_lightdash_speed * Time.fixedDeltaTime * 10f, Shadow_Lua.c_lightdash_speed);
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
				CurSpeed = (Shadow_Lua.c_run_speed_max + Shadow_Lua.c_lightdash_speed) / 1.75f;
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

	private void StateSpinDashStart()
	{
		PlayerState = State.SpinDash;
		SpinDashState = 0;
		if (!ShadowEffects.DashPadRoll)
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
			ShadowEffects.DashPadRoll = false;
		}
		SpindashSpd = Mathf.Min(Mathf.Max(CurSpeed, Shadow_Lua.c_walk_speed_max * 6f), Shadow_Lua.c_spindash_spd * 1.5f);
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
			SpindashSpd = Mathf.MoveTowards(SpindashSpd, Shadow_Lua.c_spindash_spd * 1.5f, Time.fixedDeltaTime * 35f);
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
			CurSpeed = Mathf.Clamp(CurSpeed, 0f, Shadow_Lua.c_run_speed_max * 2f);
			_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (FrontalCollision)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		AttackSphere(base.transform.position, 1.1f, _Rigidbody.velocity.normalized * 10f + _Rigidbody.velocity, (!IsFullPower) ? 1 : 10);
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

	private void StateChaosBoostStart()
	{
		PlayerState = State.ChaosBoost;
		BoostState = ((!IsGrounded()) ? 1 : 0);
		BoostTime = Time.time;
		BoostActivate = false;
		if (BoostState == 1)
		{
			PlayerVoice.PlayRandom(7);
			PlayAnimation("Chaos Boost", "On Chaos Boost");
			Camera.PlayShakeMotion(0.2f);
		}
		else
		{
			Audio.PlayOneShot(JumpSound, Audio.volume);
			PlayerVoice.PlayRandom(1, RandomPlayChance: true);
			BoostVel = Vector3.zero;
			BoostVel.y = 10f;
			_Rigidbody.velocity = BoostVel;
		}
	}

	private void StateChaosBoost()
	{
		PlayerState = State.ChaosBoost;
		LockControls = true;
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (BoostState == 0)
		{
			PlayAnimation("Jump Up", "On Jump");
			JumpAnimation = ((Time.time - BoostTime > Shadow_Lua.c_jump_time_min) ? 1 : 0);
			BoostVel.y = Mathf.Lerp(BoostVel.y, 0f, Mathf.Clamp01((Time.time - BoostTime) * 0.25f));
			_Rigidbody.velocity = BoostVel;
			if (Time.time - BoostTime > 0.5f)
			{
				BoostTime = Time.time;
				PlayerVoice.PlayRandom(7);
				PlayAnimation("Chaos Boost", "On Chaos Boost");
				Camera.PlayShakeMotion(0.2f);
				BoostState = 1;
			}
			return;
		}
		if (Time.time - BoostTime < 0.2f)
		{
			PlayAnimation("Chaos Boost", "On Chaos Boost");
		}
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - BoostTime > 0.3f && !BoostActivate)
		{
			BoostActivate = true;
			IsChaosBoost = true;
			ChaosBoostLevel = 1;
			ShadowEffects.CreateActivateBoostFX();
		}
		if (Time.time - BoostTime > 1.5f)
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

	private void StateChaosBoostEnd()
	{
	}

	private void StateUninhibitStart()
	{
		PlayerState = State.Uninhibit;
		UninhibitState = ((!IsGrounded()) ? 1 : 0);
		UninhibitTime = Time.time;
		UninhibitActivate = false;
		if (UninhibitState == 1)
		{
			PlayerVoice.PlayRandom(14);
			PlayAnimation("Uninhibit", "On Uninhibit");
			Camera.PlayShakeMotion(0.2f);
		}
		else
		{
			Audio.PlayOneShot(JumpSound, Audio.volume);
			PlayerVoice.PlayRandom(1, RandomPlayChance: true);
			UninhibitVel = Vector3.zero;
			UninhibitVel.y = 10f;
			_Rigidbody.velocity = UninhibitVel;
		}
	}

	private void StateUninhibit()
	{
		PlayerState = State.Uninhibit;
		LockControls = true;
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (UninhibitState == 0)
		{
			PlayAnimation("Jump Up", "On Jump");
			JumpAnimation = ((Time.time - UninhibitTime > Shadow_Lua.c_jump_time_min) ? 1 : 0);
			UninhibitVel.y = Mathf.Lerp(UninhibitVel.y, 0f, Mathf.Clamp01((Time.time - UninhibitTime) * 0.25f));
			_Rigidbody.velocity = UninhibitVel;
			if (Time.time - UninhibitTime > 0.5f)
			{
				UninhibitTime = Time.time;
				PlayerVoice.Play(14);
				PlayAnimation("Uninhibit", "On Uninhibit");
				Camera.PlayShakeMotion(0.2f);
				UninhibitState = 1;
			}
			return;
		}
		if (Time.time - UninhibitTime < 0.2f)
		{
			PlayAnimation("Uninhibit", "On Uninhibit");
		}
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - UninhibitTime > 0.3f && !UninhibitActivate)
		{
			UninhibitActivate = true;
			OnFullPower(Trigger: true);
			Audio.PlayOneShot(FullPowerTransformSound, Audio.volume * 0.5f);
			ShadowEffects.CreateUninhibitFX();
		}
		if (Time.time - UninhibitTime > 1.5f)
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

	private void StateUninhibitEnd()
	{
	}

	private void StateUninhibitBreakStart()
	{
		PlayerState = State.UninhibitBreak;
		PlayAnimation("Restrain Fall", "On Restrain Fall");
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		RestrainLand = false;
		RestrainStandUp = false;
		AirMotionVelocity = _Rigidbody.velocity;
		CurSpeed = 0f;
		AirMotionVelocity.y = 0f;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateUninhibitBreak()
	{
		PlayerState = State.UninhibitBreak;
		LockControls = true;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		CurSpeed = ((!IsGrounded() && ShouldAlignOrFall(Align: false)) ? Mathf.MoveTowards(CurSpeed, 0f, Time.fixedDeltaTime * 10f) : 0f);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, (IsGrounded() && ShouldAlignOrFall(Align: false)) ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (IsGrounded() && ShouldAlignOrFall(Align: false) && !RestrainLand)
		{
			PlayAnimation("Restrain Land", "On Restrain Land");
			UninhibitBreakTime = Time.time;
			RestrainLand = true;
		}
		if (IsGrounded() && RestrainLand && Time.time - UninhibitBreakTime > 6.5f)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded())
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			RestrainStandUp = false;
		}
		else
		{
			AirMotionVelocity.y = 0f;
			if (Time.time - UninhibitBreakTime > 5.5f && RestrainLand && !RestrainStandUp)
			{
				PlayAnimation("Restrain Stand", "On Restrain Stand");
				PlayerVoice.PlayRandom(3, RandomPlayChance: true);
				RestrainStandUp = true;
			}
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
	}

	private void StateUninhibitBreakEnd()
	{
	}

	private void StateChaosBlastStart()
	{
		PlayerState = State.ChaosBlast;
		BlastFromGround = IsGrounded();
		BlastTime = Time.time;
		BlastExplode = false;
		PlayAnimation("Chaos Blast", "On Chaos Blast");
		Camera.PlayCinematic(3.85f, "Chaos Blast");
		ShadowEffects.CreateBlastChargeFX();
		PlayerVoice.PlayRandom(12);
		if (BlastFromGround)
		{
			BlastLerpTime = 0f;
			BlastPos = base.transform.position;
			BlastNewPos = base.transform.position + Vector3.up * 2.5f;
		}
	}

	private void StateChaosBlast()
	{
		PlayerState = State.ChaosBlast;
		if (Time.time - BoostTime < 0.2f)
		{
			PlayAnimation("Chaos Blast", "On Chaos Blast");
		}
		LockControls = true;
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up) * CurSpeed;
		if (BlastFromGround)
		{
			BlastLerpTime += Time.fixedDeltaTime;
			if (BlastLerpTime > 2.35f)
			{
				BlastLerpTime = 2.35f;
			}
			float num = BlastLerpTime / 2.35f;
			num = num * num * (3f - 2f * num);
			base.transform.position = Vector3.Lerp(BlastPos, BlastNewPos, num);
		}
		if (Time.time - BlastTime > 2.35f && !BlastExplode)
		{
			BlastExplode = true;
			if (!IsFullPower)
			{
				HUD.DrainActionGauge(Shadow_Lua.c_chaosblast);
			}
			ChaosBlast component = UnityEngine.Object.Instantiate(ChaosBlast, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).GetComponent<ChaosBlast>();
			component.Player = base.transform;
			component.FullPower = IsFullPower;
			PlayerVoice.PlayRandom(13);
		}
		if (Time.time - BlastTime > 3.85f)
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateChaosBlastEnd()
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Shadow_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Shadow_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Shadow_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
			HomingTarget = ((PlayerState != State.ChaosAttack) ? FindHomingTarget() : FindHomingTarget());
		}
		if (PlayerState == State.ChaosSpear && SpearState == 0)
		{
			SpearTargets = FindSpearTarget((IsChaosBoost && ChaosBoostLevel > 1 && Time.time - SpearReleaseTime >= Shadow_Lua.c_chaos_spear_accumulate_wait && !IsFullPower) ? 1 : Shadow_Lua.c_chaos_spread_count);
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Tornado || PlayerState == State.TornadoReturn || PlayerState == State.UninhibitBreak || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (IsFullPower && PlayerState != State.Homing)
		{
			JumpAttackSphere(base.transform.position + base.transform.up * 0.25f, 0.8f, base.transform.forward * _Rigidbody.velocity.magnitude, 10);
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Shadow_Lua.c_rotation_speed, PlayerState == State.JumpDashSTH, DontLockOnAir: false, (PlayerState == State.JumpDashSTH) ? 3f : 2f);
				if (PlayerState != State.SpinDash)
				{
					AccelerationSystem((!HasSpeedUp) ? Shadow_Lua.c_run_acc : Shadow_Lua.c_speedup_acc);
				}
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Shadow_Lua.c_walk_speed_max : Shadow_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Shadow_Lua.c_speedup_speed_max : (IsGrounded() ? Shadow_Lua.c_run_speed_max : Shadow_Lua.c_jump_run));
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
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Tornado || PlayerState == State.TornadoReturn || PlayerState == State.UninhibitBreak || PlayerState == State.SpinDash || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		UseChaosSnap = IsChaosBoost;
		CameraFX.IsOnChaosBoost = IsChaosBoost;
		CameraFX.IsOnFullPower = IsFullPower;
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (UseChaosSnap && (IsGrounded() || PlayerState == State.Grinding) && ChainCount > 0)
			{
				ChainCount = 0;
			}
			if ((IsGrounded() || PlayerState == State.AfterHoming || PlayerState == State.Grinding || PlayerState == State.ChainJump || PlayerState == State.Vehicle || (PlayerState == State.Spring && LockControls) || (PlayerState == State.WideSpring && LockControls) || (PlayerState == State.JumpPanel && LockControls) || (PlayerState == State.RainbowRing && LockControls) || (PlayerState == State.Pole && LockControls) || (PlayerState == State.Rope && LockControls)) && JumpdashIndex != 0)
			{
				JumpdashIndex = 0;
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
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateTornado);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !IsSinking)
				{
					StateMachine.ChangeState(StateSpinDash);
				}
				if (IsChaosBoost && ChaosBoostLevel == Shadow_Lua.c_level_max && ((!IsFullPower && (!HasLightMemoryShard || (HasLightMemoryShard && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") == 0f))) || IsFullPower) && !TriggerPressed && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateChaosBlast);
				}
			}
			if (PlayerState == State.Jump || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls))
			{
				if ((PlayerState == State.Jump && ReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Button A")) || (PlayerState != State.Jump && Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
				{
					OnJumpDash();
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && SpearCount < Shadow_Lua.c_chaos_spear_count)
				{
					StateMachine.ChangeState(StateChaosSpear);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (IsChaosBoost && ChaosBoostLevel == Shadow_Lua.c_level_max && ((!IsFullPower && (!HasLightMemoryShard || (HasLightMemoryShard && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") == 0f))) || IsFullPower) && !TriggerPressed && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					StateMachine.ChangeState(StateChaosBlast);
				}
			}
			if (PlayerState == State.Air)
			{
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && SpearCount < Shadow_Lua.c_chaos_spear_count)
				{
					StateMachine.ChangeState(StateChaosSpear);
				}
				if (JumpdashIndex < 2 && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					OnJumpDash();
				}
			}
			if (PlayerState == State.SlowFall && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && SpearCount < Shadow_Lua.c_chaos_spear_count)
			{
				StateMachine.ChangeState(StateChaosSpear);
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
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && SpearCount < Shadow_Lua.c_chaos_spear_count)
				{
					StateMachine.ChangeState(StateChaosSpear);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.ChaosAttack)
			{
				if (!QueuedPress && OnButtonPressed("Button A"))
				{
					QueuedPress = Time.time - ChaosAttackTime > 0.125f;
				}
				if (AttackCount < 5 && Time.time - ChaosAttackTime > 0.2f && OnCombo("Button A") && (bool)ChaosAttackTarget)
				{
					AttackDealDamage = true;
					OnChaosAttack();
				}
				if (UseChaosSnap && (bool)HomingTarget && Time.time - ChaosAttackTime > 0.2f && (Singleton<RInput>.Instance.P.GetButton("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button A")) && !ChaosAttackTarget)
				{
					ContinueAttackCount = true;
					OnJumpDash();
				}
				if (UseChaosSnap && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateAfterHoming);
				}
			}
			if (PlayerState == State.Tornado)
			{
				if (Time.time - TornadoTime > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateTrickJump);
				}
				if (TornadoCount < 2 && Time.time - TornadoTime > TornadoWaitTimeAmount && !TornadoPressed && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					TornadoCount++;
					ShadowEffects.CreateTornadoFX(TornadoCount);
					ShadowEffects.PlayFlash();
					PlayerVoice.PlayRandom(6, RandomPlayChance: true);
					TornadoPressed = false;
					ShadowEffects.ManageTornadoTrail(Enable: true);
					TornadoPressed = true;
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState == State.TornadoReturn)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateTrickJump);
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
					OnJumpDash();
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && SpearCount < Shadow_Lua.c_chaos_spear_count)
				{
					StateMachine.ChangeState(StateChaosSpear);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
			}
			if (PlayerState != State.ChaosSpear && (IsGrounded() || PlayerState == State.AfterHoming || PlayerState == State.Grinding))
			{
				SpearCount = 0;
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
				if (SpinDashState == 1 && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StateTornado);
				}
				if (SpinDashState == 0 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					SpinDashState = 1;
					Audio.PlayOneShot((Singleton<Settings>.Instance.settings.SpinEffect == 0) ? SpinDashShoot : ((Singleton<Settings>.Instance.settings.SpinEffect == 1) ? SpinDashShootAdventure : SpinDashShootAdventure2), Audio.volume);
					CurSpeed = SpindashSpd;
					CurSpeed += _Rigidbody.velocity.magnitude * 0.6f;
					CurSpeed = Mathf.Clamp(CurSpeed, 0f, Sonic_New_Lua.c_run_speed_max * 2f);
				}
				if (CanLightDash() && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
				{
					StateMachine.ChangeState(StateLightDash);
				}
				if (IsChaosBoost && ChaosBoostLevel == Shadow_Lua.c_level_max && ((!IsFullPower && (!HasLightMemoryShard || (HasLightMemoryShard && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") == 0f))) || IsFullPower) && !TriggerPressed && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					StateMachine.ChangeState(StateChaosBlast);
				}
			}
			if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
			if ((PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.Jump || PlayerState == State.AfterHoming || PlayerState == State.Tornado || PlayerState == State.SpinDash || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && HUD.ActionDisplay == 100f && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateChaosBoost);
				TriggerPressed = true;
			}
			if (IsChaosBoost)
			{
				if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
				{
					TriggerPressed = false;
				}
				if ((PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.Jump || PlayerState == State.AfterHoming || PlayerState == State.Tornado || PlayerState == State.SpinDash || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && ChaosBoostLevel < Shadow_Lua.c_level_max && HUD.ChaosMaturityDisplay >= 100f && !TriggerPressed && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					ChaosBoostLevel++;
					ShadowEffects.CreateActivateBoostFX();
					Camera.PlayShakeMotion();
					if (ChaosBoostLevel < Shadow_Lua.c_level_max + (HasLightMemoryShard ? 1 : 0))
					{
						HUD.ChaosMaturityDisplay = 0f;
					}
				}
				if (!IsFullPower && (HUD.ActionDisplay <= 0f || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.SnowBallDeath || PlayerState == State.TornadoDeath))
				{
					DisableChaosBoost();
				}
				if (IsFullPower)
				{
					if (HUD.ActionDisplay <= 0f || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.SnowBallDeath || PlayerState == State.TornadoDeath)
					{
						OnFullPower(Trigger: false);
					}
					if (HUD.ActionDisplay <= 0f && PlayerState != State.ChaosBlast && PlayerState != State.EdgeDanger && PlayerState != State.Grinding && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Cutscene && PlayerState != State.Vehicle && PlayerState != State.Spring && PlayerState != State.WideSpring && PlayerState != State.JumpPanel && PlayerState != State.Death && PlayerState != State.DashRing && PlayerState != State.RainbowRing && PlayerState != State.ChainJump && PlayerState != State.Pole && PlayerState != State.Float && PlayerState != State.Rope && PlayerState != State.Hold && PlayerState != State.UpReel && PlayerState != State.Balancer && PlayerState != State.TrainRepel)
					{
						StateMachine.ChangeState(StateUninhibitBreak);
					}
				}
				if ((PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.Jump || PlayerState == State.AfterHoming || PlayerState == State.Tornado || PlayerState == State.SpinDash || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && HasLightMemoryShard && !IsFullPower && ChaosBoostLevel == Shadow_Lua.c_level_max && HUD.ChaosMaturityDisplay >= 100f && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f && !TriggerPressed && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					StateMachine.ChangeState(StateUninhibit);
					TriggerPressed = true;
				}
			}
		}
		ChaosBoostSource.volume = Mathf.Lerp(ChaosBoostSource.volume, (IsChaosBoost && !IsFullPower) ? 0.5f : 0f, Time.deltaTime * 5f);
		UpgradeHoldSource.volume = Mathf.Lerp(UpgradeHoldSource.volume, (!IsFullPower && IsChaosBoost && ChaosBoostLevel == Shadow_Lua.c_level_max && HUD.ChaosMaturityDisplay >= 100f && HasLightMemoryShard && (PlayerState == State.Ground || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.Jump || PlayerState == State.AfterHoming || PlayerState == State.Tornado || PlayerState == State.SpinDash || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Pole && !LockControls) || (PlayerState == State.Rope && !LockControls)) && PlayerState != State.Uninhibit && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f) ? 0.5f : 0f, Time.deltaTime * 5f);
		FullPowerSource.volume = Mathf.Lerp(FullPowerSource.volume, IsFullPower ? 1f : 0f, Time.deltaTime * 5f);
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
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "BoostGeneral")
			{
				HUD.ActionDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloat;
				ChaosBoostLevel = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarInt;
				IsChaosBoost = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool;
			}
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "BoostDisplay")
			{
				HUD.ChaosMaturityDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloat;
			}
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "UninhibitMode" && Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool)
			{
				IsFullPower = true;
				LimiterRBs[0].transform.SetParent(null);
				LimiterRBs[1].transform.SetParent(null);
				LimiterRBs[0].isKinematic = false;
				LimiterRBs[1].isKinematic = false;
				LimiterRBs[0].transform.position = base.transform.position - base.transform.up * 100f;
				LimiterRBs[1].transform.position = base.transform.position - base.transform.up * 100f;
				LimiterCols[0].enabled = true;
				LimiterCols[1].enabled = true;
				HUD.LevelAnimator.SetBool("Is Pulsate", value: true);
			}
		}
	}

	public IEnumerator MountVehicle(string Vehicle)
	{
		yield return new WaitForSeconds(0.025f);
		switch (Vehicle)
		{
		case "jeep":
			UnityEngine.Object.FindObjectOfType<Jeep>().Mount(PlayerManager, StartInVehicle: true);
			break;
		case "bike":
			UnityEngine.Object.FindObjectOfType<Bike>().Mount(PlayerManager, StartInVehicle: true);
			break;
		case "hover":
			UnityEngine.Object.FindObjectOfType<Hover>().Mount(PlayerManager, StartInVehicle: true);
			break;
		case "glider":
			UnityEngine.Object.FindObjectOfType<Glider>().Mount(PlayerManager, StartInVehicle: true);
			break;
		}
	}

	private IEnumerator SetRestrain()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		float FlashTime = Time.time;
		float FlashAmount = UnityEngine.Random.Range(0.25f, 1.5f);
		while (Timer <= 7.5f)
		{
			Timer = Time.time - StartTime;
			if (Time.time - FlashTime > FlashAmount)
			{
				FlashTime = Time.time;
				FlashAmount = UnityEngine.Random.Range(0.25f, 1.5f);
				ShadowEffects.PlayRestrainFX(UnityEngine.Random.Range(0.25f, 0.5f));
				Audio.PlayOneShot(RestrainSounds[UnityEngine.Random.Range(0, RestrainSounds.Length)], Audio.volume);
			}
			yield return new WaitForFixedUpdate();
		}
		IsRestrained = false;
	}

	private void OnChaosAttack(bool IsChaosSnap = false, bool IsFirstHit = false)
	{
		ChaosAttackTime = Time.time;
		QueuedPress = false;
		if (!IsChaosSnap)
		{
			if (!IsFirstHit)
			{
				AttackCount++;
			}
		}
		else
		{
			AttackCount = ChainCount;
		}
		ChaosAttackDamage = true;
		ShadowEffects.CreateChaosAttackFX(AttackCount);
		Animator.SetInteger("Attack ID", AttackCount);
		Animator.SetTrigger("On Chaos Attack");
		Camera.PlayImpactShakeMotion();
		if (AttackCount < 5)
		{
			PlayerVoice.PlayRandom(6);
		}
		else
		{
			PlayerVoice.Play(7);
		}
	}

	private void DisableChaosBoost()
	{
		Audio.PlayOneShot(AwakeSound, Audio.volume * 0.5f);
		ChaosBoostLevel = 0;
		HUD.ChaosMaturityDisplay = 0f;
		IsChaosBoost = false;
	}

	private void OnFullPower(bool Trigger)
	{
		ShadowEffects.PlayLimiterFX(Trigger);
		Audio.PlayOneShot(LimiterSounds[(!Trigger) ? 1 : 0], Audio.volume);
		if (Trigger)
		{
			IsFullPower = true;
			IsRestrained = false;
			LimiterRBs[0].transform.SetParent(null);
			LimiterRBs[1].transform.SetParent(null);
			LimiterRBs[0].isKinematic = false;
			LimiterRBs[1].isKinematic = false;
			LimiterRBs[0].AddForce(-base.transform.right * 5f, ForceMode.VelocityChange);
			LimiterRBs[1].AddForce(base.transform.right * 5f, ForceMode.VelocityChange);
			LimiterCols[0].enabled = true;
			LimiterCols[1].enabled = true;
			LimiterAnimators[0].SetTrigger("On Limiter Off");
			LimiterAnimators[1].SetTrigger("On Limiter Off");
			HUD.LevelAnimator.SetBool("Is Pulsate", value: true);
		}
		else
		{
			IsFullPower = false;
			IsRestrained = true;
			LimiterRBs[0].transform.SetParent(LimiterPoints[0]);
			LimiterRBs[1].transform.SetParent(LimiterPoints[1]);
			LimiterRBs[0].transform.localPosition = Vector3.zero;
			LimiterRBs[1].transform.localPosition = Vector3.zero;
			LimiterRBs[0].transform.localEulerAngles = Vector3.zero;
			LimiterRBs[1].transform.localEulerAngles = Vector3.zero;
			LimiterRBs[0].isKinematic = true;
			LimiterRBs[1].isKinematic = true;
			LimiterCols[0].enabled = false;
			LimiterCols[1].enabled = false;
			HUD.LevelAnimator.SetBool("Is Pulsate", value: false);
			PlayerVoice.PlayRandom(4);
			StartCoroutine(SetRestrain());
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
		if (PlayerState == State.Homing || PlayerState == State.AfterHoming || PlayerState == State.Tornado || PlayerState == State.ChaosAttack || (PlayerState == State.SpinDash && SpinDashState == 1))
		{
			return 1;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if ((PlayerState == State.Homing && UseChaosSnap) || PlayerState == State.LightDash || PlayerState == State.Hurt || PlayerState == State.ChaosBlast || PlayerState == State.ChaosBoost || PlayerState == State.Uninhibit || PlayerState == State.Talk || PlayerState == State.Vehicle || PlayerState == State.WarpHole || PlayerState == State.Result || PlayerState == State.Cutscene || HasInvincibility || IsFullPower || IsDead)
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
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.JumpDashSTH || (PlayerState == State.SpinDash && SpinDashState == 1)) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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
			PlayerRenderers[i].enabled = ((!UseChaosSnap) ? (ImmunityTime - Time.time <= 0f || ((ImmunityTime - Time.time >= 0f && BlinkTimer <= 0.5f) ? true : false)) : (PlayerState != State.Homing));
		}
	}

	private void UpdateCollider()
	{
		if (PlayerState == State.Tornado || PlayerState == State.TornadoReturn || PlayerState == State.SpinDash)
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
		if (!HomingTarget)
		{
			PlayerVoice.PlayRandom(5, RandomPlayChance: true);
			Audio.PlayOneShot((Singleton<Settings>.Instance.settings.JumpdashType != 2) ? JumpDashSound : JumpDashSTHSound, Audio.volume);
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
		if (!UseChaosSnap)
		{
			Audio.PlayOneShot((Singleton<Settings>.Instance.settings.JumpdashType != 2) ? JumpDashSound : JumpDashSTHSound, Audio.volume);
		}
		OnChaosSnap();
		return true;
	}

	private void OnChaosSnap()
	{
		PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		StateMachine.ChangeState(StateHoming);
	}

	private void OnDismountVehicle()
	{
		if ((bool)CurVehicle)
		{
			CurVehicle.OnImmediateDismount();
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
					DoAirRecoil = false;
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

	public override void OnBulletHit(Vector3 Direction, float Rate = 1f, int DeathType = 0)
	{
		if (!IsInvulnerable(1) && (!IsChaosBoost || (IsChaosBoost && IsSinking)))
		{
			base.OnBulletHit(Direction, Rate, DeathType);
		}
	}

	public override void OnDeathEnter(int DeathType)
	{
		if (!IsDead && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && PlayerState != State.Hold)
		{
			IsDead = true;
			Camera.OnPlayerDeath();
			OnDismountVehicle();
			DoDeath(DeathType);
		}
	}

	public override void OnGoal()
	{
		OnDismountVehicle();
		base.OnGoal();
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
