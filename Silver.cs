using System;
using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Silver : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		SlowFall = 4,
		Levitate = 5,
		Upheave = 6,
		UpheaveSmash = 7,
		Hotspot = 8,
		PsychoSmash = 9,
		PsychicShot = 10,
		GrabAll = 11,
		Dash = 12,
		PsychoShock = 13,
		ESPAwaken = 14,
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
		IronSpring = 30,
		JumpPanel = 31,
		DashRing = 32,
		RainbowRing = 33,
		Float = 34,
		Hold = 35,
		Lotus = 36,
		Tarzan = 37,
		UpReel = 38,
		Balancer = 39
	}

	[Header("Player Framework")]
	public SilverEffects SilverEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	internal float AirActionTime;

	internal float BalancerFactor;

	internal bool IsAwakened;

	internal bool HasLotusOfResilience;

	internal bool HasFlameOfControl;

	internal bool HasSigilOfAwakening;

	internal bool ChargingPsychicKnife;

	internal bool FullyChargedPsychicKnife;

	private int TriggerCount;

	private int AttackState;

	private float HoldTime;

	private float TriggerCooler;

	private float ThrowForce;

	private float PsychoAnimation;

	private float PsychoShockTime;

	private float PsychicKnifeTime;

	private bool PsychoReleasedKey;

	private bool GrabAllReleasedKey;

	private bool SmashFullCharge;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	public UpgradeModels Upgrades;

	private float BlinkTimer;

	private float BodyDirDot;

	internal List<GameObject> PickedObjects = new List<GameObject>();

	internal List<GameObject> PickedObjectPoints = new List<GameObject>();

	internal bool UsingPsychokinesis;

	internal bool ManipulateObjects;

	internal bool UsePsiElements;

	internal bool PsiFX;

	[Header("Psycho Abilities")]
	public GameObject SmashBall;

	public GameObject KnifeSlash;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip[] LevitateSounds;

	public AudioClip[] PsychoSounds;

	public AudioClip UpheaveSound;

	public AudioClip[] UpheaveRotSounds;

	public AudioClip BrakeSound;

	public AudioClip KnifeSlashSound;

	public AudioClip ESPAwakenSound;

	[Header("Audio Sources")]
	public AudioSource PsiLoopSource;

	public AudioSource UpheaveSource;

	public AudioSource[] PsiChargeSources;

	public AudioSource KnifeChargeSource;

	public AudioSource UpgradeHoldSource;

	public AudioSource ESPAuraSource;

	private bool PlayKnifeChargeSound;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool ReachedApex;

	private bool StartUpheave;

	internal float UpheaveTime;

	internal int UpheaveState;

	private float UpheaveSmashTime;

	private bool ReleasedUpheaveKey;

	private bool UpheadSmashCanUseMoves;

	private bool StartHotspot;

	private float HotspotTime;

	internal int HotspotState;

	private bool PSmashGrounded;

	internal int PSmashState;

	private float PSmashTime;

	private bool PSmashSwitch;

	private int PSmashSwitchIndex;

	private bool IsKnife;

	private bool PShotGrounded;

	internal bool FirePsychicShot;

	private float PShotTime;

	private bool ChargeGrabAll;

	private float GrabAllTime;

	private bool CanTeleDash;

	private float DashTime;

	private bool ShockRelease;

	private float ShockTime;

	internal int ShockState;

	private bool HasAwakened;

	private bool AwakeDmg;

	internal float AwakenTimer;

	private float HurtTime;

	private GameObject UpheaveObj;

	private Common_PsiMarkSphere HotspotObj;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Silver_Lua.c_player_name;
		PlayerNameShort = Silver_Lua.c_player_name_short;
		WalkSpeed = Silver_Lua.c_walk_speed_max;
		TopSpeed = Silver_Lua.c_run_speed_max;
		BrakeSpeed = Silver_Lua.c_brake_acc;
		GrindSpeedOrg = Silver_Lua.c_grind_speed_org;
		GrindAcc = Silver_Lua.c_grind_acc;
		GrindSpeedMax = Silver_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		HasLotusOfResilience = gameData.HasFlag(Game.LotusOfResilience);
		HasFlameOfControl = gameData.HasFlag(Game.FlameOfControl);
		HasSigilOfAwakening = gameData.HasFlag(Game.SigilOfAwakening);
		Animator.SetBool("Has Lotus", HasLotusOfResilience);
		HUD.OpenGauge(Silver_Lua.OpenGauge(), Silver_Lua.psi_power, Silver_Lua.c_psi_gauge_heal, Silver_Lua.c_psi_gauge_heal_delay);
	}

	private void StateGroundStart()
	{
		PlayerState = State.Ground;
		PsychoReleasedKey = false;
		Animator.ResetTrigger("Additive Idle");
		MaxRayLenght = 0.75f;
		IdleAnimPlayed = false;
		IdleTimer = Time.time;
	}

	private void StateGround()
	{
		PlayerState = State.Ground;
		if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
		{
			PsychoReleasedKey = true;
		}
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (TargetDirection.magnitude == 0f && !UsePsiElements)
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Silver_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Silver_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Silver_Lua.c_run_speed_max) / Silver_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Silver_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Silver_Lua.c_run_speed_max;
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
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		if (HasLotusOfResilience)
		{
			JumpAnimation = 0;
		}
		else
		{
			ReachedApex = false;
			PlayAnimation("Jump Up", "On Jump");
		}
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Silver_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Silver_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateJump()
	{
		PlayerState = State.Jump;
		if (HasLotusOfResilience)
		{
			PlayAnimation("Jump Up Upgrade", "On Jump");
			JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Silver_Lua.c_jump_time_min) ? 1 : 0));
			if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, 1))
			{
				JumpTime = Time.time;
				JumpAnimation = 0;
				AirMotionVelocity.y = 12f;
			}
		}
		else if (!ReachedApex && (ReleasedKey || AirMotionVelocity.y < 3f))
		{
			PlayAnimation("Jump Down", "On Jump");
			ReachedApex = true;
		}
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
		Animator.SetTrigger("Additive Idle");
	}

	private void StateLevitateStart()
	{
		PlayerState = State.Levitate;
		if (CurSpeed > 0f)
		{
			Audio.PlayOneShot(LevitateSounds[1], Audio.volume);
		}
		else
		{
			Audio.PlayOneShot(LevitateSounds[0], Audio.volume);
			Audio.PlayOneShot(LevitateSounds[1], Audio.volume);
		}
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateLevitate()
	{
		PlayerState = State.Levitate;
		PlayAnimation("Levitate", "On Levitate");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, -0.001f, Time.fixedDeltaTime * 6f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (!Singleton<RInput>.Instance.P.GetButton("Button A") || HUD.ActionDisplay < 0.1f)
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

	private void StateLevitateEnd()
	{
	}

	private void StateUpheaveStart()
	{
		PlayerState = State.Upheave;
		PlayAnimation("Upheave Start", "On Upheave");
		UpheaveTime = Time.time;
		UpheaveState = 0;
		_Rigidbody.velocity = Vector3.zero;
		StartUpheave = false;
	}

	private void StateUpheave()
	{
		PlayerState = State.Upheave;
		LockControls = true;
		CurSpeed = 0f;
		if (UpheaveState == 0)
		{
			if (Time.time - UpheaveTime > 0.45f)
			{
				if (!StartUpheave)
				{
					StartUpheave = true;
					_Rigidbody.velocity = Vector3.zero;
					SilverEffects.CreateUpheaveFX();
				}
				DoUpheave();
			}
			else
			{
				_Rigidbody.velocity = Vector3.zero;
			}
			if ((!Singleton<RInput>.Instance.P.GetButton("Right Trigger") && Time.time - UpheaveTime > 0.55f) || HUD.ActionDisplay < 0.1f)
			{
				Audio.PlayOneShot(UpheaveRotSounds[(!(HUD.ActionDisplay < 0.1f)) ? 1u : 0u], Audio.volume * ((HUD.ActionDisplay < 0.1f) ? 1f : 5f));
				if (HUD.ActionDisplay > 0.1f)
				{
					SilverEffects.CreateUpheaveFX();
				}
				UpheaveState = 1;
				UpheaveTime = Time.time;
				PlayAnimation("Upheave Start", "On Upheave End");
				DoUpheave(Raise: false);
				base.transform.SetParent(null);
			}
		}
		else if (Time.time - UpheaveTime > 0.5f)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
			AirMotionVelocity = _Rigidbody.velocity;
			AirMotionVelocity.y = 0f;
			_Rigidbody.velocity = AirMotionVelocity;
			UpheaveDetach();
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
	}

	private void StateUpheaveEnd()
	{
		UpheaveDetach();
	}

	private void StateUpheaveSmashStart()
	{
		PlayerState = State.UpheaveSmash;
		PickedObjects.Add(UpheaveObj);
		PickedObjectPoints.Add(new GameObject("PickedObjectPoint" + PickedObjectPoints.Count));
		PlayerVoice.PlayRandom(6, RandomPlayChance: true);
		ThrowForce = Silver_Lua.c_psychosmash_power;
		DoPsychoThrow(base.transform.position + base.transform.up * 0.65f + base.transform.forward * 1.25f, 0.8f);
		SilverEffects.CreatePsychoSmashFX();
		UpheaveSmashTime = Time.time;
		ReleasedUpheaveKey = false;
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity = Vector3.up * 6f;
		_Rigidbody.velocity = AirMotionVelocity;
		Animator.SetTrigger("On Upheave Smash");
		DoUpheave(Raise: false);
		base.transform.SetParent(null);
		Debug.Log("Upheave Smash State");
		UpheadSmashCanUseMoves = false;
		CanTeleDash = true;
	}

	private void StateUpheaveSmash()
	{
		PlayerState = State.UpheaveSmash;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y, 12f);
		if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
		{
			ReleasedUpheaveKey = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedUpheaveKey && Singleton<RInput>.Instance.P.GetButton("Button X") && Time.time - UpheaveSmashTime < 0.5f)
		{
			AirMotionVelocity += Vector3.up * 6f * Time.fixedDeltaTime * 4f;
		}
		if (Time.time - UpheaveSmashTime > 0.15f && !UpheadSmashCanUseMoves)
		{
			UpheadSmashCanUseMoves = true;
		}
		else
		{
			AirActionTime = Time.time;
		}
		MaxRayLenght = ((Time.time - UpheaveSmashTime > 0.25f) ? 0.75f : 0f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false) && Time.time - UpheaveSmashTime > 0.25f)
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateUpheaveSmashEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateHotspotStart()
	{
		PlayerState = State.Hotspot;
		PlayAnimation("Upheave Start", "On Upheave");
		HotspotTime = Time.time;
		HotspotState = 0;
		StartHotspot = false;
	}

	private void StateHotspot()
	{
		PlayerState = State.Hotspot;
		LockControls = true;
		CurSpeed = 0f;
		if (HotspotState == 0)
		{
			if (!StartHotspot && Time.time - HotspotTime > 0.45f)
			{
				SilverEffects.CreateUpheaveFX();
				if ((bool)HotspotObj)
				{
					HotspotObj.OnHotspot();
				}
				Audio.PlayOneShot(UpheaveSound, Audio.volume);
				StartHotspot = true;
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger") && Time.time - HotspotTime > 0.55f)
			{
				HotspotState = 1;
				if ((bool)HotspotObj)
				{
					HotspotObj.OnRelease();
				}
				HotspotTime = Time.time;
				PlayAnimation("Upheave Start", "On Upheave End");
			}
		}
		else if (Time.time - HotspotTime > 0.5f)
		{
			StateMachine.ChangeState(StateGround);
		}
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateHotspotEnd()
	{
		if ((bool)HotspotObj && HotspotState != 1)
		{
			HotspotObj.OnRelease();
		}
	}

	private void StatePsychoSmashStart()
	{
		PlayerState = State.PsychoSmash;
		PSmashGrounded = IsGrounded();
		PSmashSwitch = false;
		PSmashSwitchIndex = 0;
		Animator.SetInteger("Smash Type", (!IsGrounded()) ? 1 : 0);
		Animator.SetTrigger("On Psycho Smash");
		Animator.SetInteger("Psycho Smash Switch Index", PSmashSwitchIndex);
		PSmashState = 0;
		ThrowForce = Silver_Lua.c_psychosmash_begin;
		PsiChargeSources[1].volume = 0f;
		PsiChargeSources[0].Play();
		PsiChargeSources[1].Play();
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		if (PSmashGrounded)
		{
			PlayerVoice.PlayRandom(8);
		}
		SmashFullCharge = false;
	}

	private void StatePsychoSmash()
	{
		PlayerState = State.PsychoSmash;
		LockControls = PSmashGrounded && PSmashState == 1;
		if (PSmashState == 0)
		{
			ThrowForce = Mathf.MoveTowards(ThrowForce, Silver_Lua.c_psychosmash_power, 1.5f);
			if (ThrowForce == Silver_Lua.c_psychosmash_power && !SmashFullCharge)
			{
				SilverEffects.PlayFlash(0.4f);
				SilverEffects.PlaySmashFullChargeFX();
				SmashFullCharge = true;
			}
			if (!PsiChargeSources[0].isPlaying)
			{
				PsiChargeSources[1].volume = 1f;
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				Animator.SetTrigger((!PSmashSwitch) ? "On Psycho Smash Throw" : "On Psycho Smash Switch Throw");
				PSmashTime = Time.time;
				PlayerVoice.PlayRandom(6, RandomPlayChance: true);
				DoPsychoThrow(base.transform.position + base.transform.up * 0.65f + base.transform.forward * 1.25f, 0.8f, CapsuleRadius: true);
				SilverEffects.CreatePsychoSmashFX();
				PSmashState = 1;
			}
		}
		else
		{
			PsiChargeSources[0].Stop();
			PsiChargeSources[1].Stop();
			if (Time.time - PSmashTime > (PSmashGrounded ? 0.65f : 0.4f))
			{
				if (PSmashGrounded)
				{
					StateMachine.ChangeState(StateGround);
				}
				else
				{
					StateMachine.ChangeState(StateAir);
				}
			}
		}
		if (PSmashGrounded)
		{
			CurSpeed = 0f;
			AirMotionVelocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (!IsGrounded() || ShouldAlignOrFall(Align: true))
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
			if (PickedObjects.Count == 0)
			{
				AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			}
			else
			{
				CurSpeed = 0f;
				AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, 0f, Time.fixedDeltaTime * 6f);
			}
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		_Rigidbody.velocity = AirMotionVelocity;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
	}

	private void StatePsychoSmashEnd()
	{
		PsiChargeSources[0].Stop();
		PsiChargeSources[1].Stop();
	}

	private void StatePsychicShotStart()
	{
		PlayerState = State.PsychicShot;
		PShotTime = Time.time;
		PShotGrounded = IsGrounded();
		PsiChargeSources[1].volume = 0f;
		PsiChargeSources[0].Play();
		PsiChargeSources[1].Play();
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		FirePsychicShot = false;
		if (PShotGrounded)
		{
			PlayAnimation("Movement (Blend Tree)", "On Ground");
		}
		else
		{
			PlayAnimation("Air Falling", "On Air Fall");
		}
	}

	private void StatePsychicShot()
	{
		PlayerState = State.PsychicShot;
		if (!IsKnife)
		{
			PlayAnimation(IsGrounded() ? "Psychic Shot" : "Air Psychic Shot", IsGrounded() ? "On Psychic Shot" : "On Air Psychic Shot");
		}
		else
		{
			PlayAnimation(IsGrounded() ? "Psychic Knife" : "Air Psychic Knife", IsGrounded() ? "On Psychic Knife" : "On Air Psychic Knife");
		}
		LockControls = PShotGrounded && FirePsychicShot;
		if (Time.time - PShotTime > ((!IsKnife) ? 0.2f : 0.25f) && !FirePsychicShot)
		{
			FirePsychicShot = true;
			PsiChargeSources[0].Stop();
			PsiChargeSources[1].Stop();
			PlayerVoice.PlayRandom(6, RandomPlayChance: true);
			if (!IsKnife)
			{
				SmashBall component = UnityEngine.Object.Instantiate(SmashBall, base.transform.position + base.transform.up * 0.35f + base.transform.forward * 0.25f, base.transform.rotation).GetComponent<SmashBall>();
				component.Player = base.transform;
				component.Awakened = IsAwakened;
				HUD.DrainActionGauge(Silver_Lua.c_psi_gauge_psi_shot * ESPMult());
			}
			else
			{
				UnityEngine.Object.Instantiate(KnifeSlash, base.transform.position + base.transform.up * 0.35f + base.transform.forward * 0.25f, base.transform.rotation).GetComponent<KnifeSlash>().Player = base.transform;
				HUD.DrainActionGauge(Silver_Lua.c_psi_gauge_psi_knife * ESPMult());
				Audio.PlayOneShot(KnifeSlashSound, Audio.volume * 0.75f);
			}
			SilverEffects.CreatePsychoSmashFX();
		}
		else if (Time.time - PShotTime < ((!IsKnife) ? 0.2f : 0.25f) && !PsiChargeSources[0].isPlaying)
		{
			PsiChargeSources[1].volume = 1f;
		}
		if (Time.time - PShotTime > ((!IsKnife) ? 0.9f : 0.65f))
		{
			if (PShotGrounded)
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		if (PShotGrounded)
		{
			CurSpeed = 0f;
			AirMotionVelocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			if (!IsGrounded())
			{
				StateMachine.ChangeState(StateAir);
			}
		}
		else
		{
			Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
			if (_Rigidbody.velocity.magnitude != 0f)
			{
				vector = base.transform.forward * CurSpeed * 0.5f;
				AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
			}
			if (FirePsychicShot)
			{
				AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
			}
			else
			{
				AirMotionVelocity.y -= 12.5f * Time.fixedDeltaTime;
			}
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateGround);
				DoLandAnim();
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
			}
		}
		_Rigidbody.velocity = AirMotionVelocity;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
	}

	private void StatePsychicShotEnd()
	{
		PsiChargeSources[0].Stop();
		PsiChargeSources[1].Stop();
	}

	private void StateGrabAllStart()
	{
		PlayerState = State.GrabAll;
		GrabAllReleasedKey = false;
		PlayAnimation("Grab All Start", "On Grab All");
		ThrowForce = Silver_Lua.c_psychosmash_begin;
		ChargeGrabAll = false;
		SilverEffects.CreateGrabAllActivateFX();
		SmashFullCharge = false;
	}

	private void StateGrabAll()
	{
		PlayerState = State.GrabAll;
		if (!Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
		{
			GrabAllReleasedKey = true;
		}
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (PickedObjects.Count != 0 && Singleton<RInput>.Instance.P.GetButton("Button X"))
		{
			ThrowForce = Mathf.MoveTowards(ThrowForce, Silver_Lua.c_psychosmash_power, 1.5f);
			if (ThrowForce == Silver_Lua.c_psychosmash_power && !SmashFullCharge)
			{
				SilverEffects.PlayFlash(0.4f);
				SilverEffects.PlaySmashFullChargeFX();
				SmashFullCharge = true;
			}
			if (!ChargeGrabAll)
			{
				ChargeGrabAll = true;
				PsiChargeSources[1].volume = 0f;
				PsiChargeSources[0].Play();
				PsiChargeSources[1].Play();
				PlayerVoice.PlayRandom(8);
				PlayAnimation("Grab All Charge", "On Grab All Charge");
			}
			if (!PsiChargeSources[0].isPlaying)
			{
				PsiChargeSources[1].volume = 1f;
			}
		}
		else
		{
			PsiChargeSources[0].Stop();
			PsiChargeSources[1].Stop();
			if (ChargeGrabAll)
			{
				DoPsychoThrow(base.transform.position + base.transform.up * 0.5f + base.transform.forward * 1.25f, 0.65f);
				ThrowForce = Silver_Lua.c_psychosmash_begin;
				ChargeGrabAll = false;
				GrabAllTime = Time.time;
				SilverEffects.CreateGrabAllFX();
				PlayerVoice.PlayRandom(7, RandomPlayChance: true);
				PlayAnimation("Grab All Charge End", "On Grab All Throw");
			}
		}
		if (PickedObjects.Count == 0 && !ChargeGrabAll && Time.time - GrabAllTime > 0.6f)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateGrabAllEnd()
	{
		PsiChargeSources[0].Stop();
		PsiChargeSources[1].Stop();
	}

	private void StateDashStart()
	{
		PlayerState = State.Dash;
		DashTime = Time.time;
		SilverEffects.CreateTeleDashFX();
		HUD.DrainActionGauge(Silver_Lua.c_psi_gauge_teleport_dash * ESPMult());
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateDash()
	{
		PlayerState = State.Dash;
		PlayAnimation("Dash", "On Dash");
		CurSpeed = Silver_Lua.c_tele_dash_speed;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, -0.001f, Time.fixedDeltaTime * 8f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if ((Time.time - DashTime > Silver_Lua.c_tele_dash_time && ((HasLotusOfResilience && !Singleton<RInput>.Instance.P.GetButton("Button A")) || (!HasLotusOfResilience && ((Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - DashTime > Silver_Lua.c_tele_dash_time + 0.25f) || !Singleton<RInput>.Instance.P.GetButton("Button A"))))) || HUD.ActionDisplay < 1f || FrontalCollision)
		{
			CurSpeed = TopSpeed;
			StateMachine.ChangeState(StateAir);
		}
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateDashEnd()
	{
	}

	private void StatePsychoShockStart()
	{
		PlayerState = State.PsychoShock;
		ShockRelease = false;
		ShockState = 0;
		Audio.PlayOneShot(LevitateSounds[1], Audio.volume);
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = -20f;
		_Rigidbody.velocity = AirMotionVelocity;
		PlayAnimation("Shock Fall", "On Shock");
	}

	private void StatePsychoShock()
	{
		PlayerState = State.PsychoShock;
		LockControls = ShockState == 1;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, (ShockState == 0) ? Vector3.up : RaycastHit.normal) * base.transform.rotation;
		if (ShockState == 0)
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
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				PlayAnimation("Shock Land", "On Shock");
				ShockTime = Time.time;
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
				ShockState = 1;
			}
		}
		else
		{
			CurSpeed = 0f;
			AirMotionVelocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
			_Rigidbody.velocity = AirMotionVelocity;
			if (Time.time - ShockTime > 0.15f && !ShockRelease)
			{
				SilverEffects.CreatePsychoShockFX();
				DoPsychoShock(base.transform.position - base.transform.up * 0.1f, 7f);
				ShockRelease = true;
			}
			if (Time.time - ShockTime > 0.8f)
			{
				StateMachine.ChangeState(StateGround);
			}
			if (!IsGrounded())
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StatePsychoShockEnd()
	{
	}

	private void StateESPAwakenStart()
	{
		PlayerState = State.ESPAwaken;
		HasAwakened = false;
		AwakeDmg = false;
		MaxRayLenght = 0.75f;
		AwakenTimer = Time.time;
		Audio.PlayOneShot(ESPAwakenSound, Audio.volume);
		Audio.PlayOneShot(LevitateSounds[1], Audio.volume);
	}

	private void StateESPAwaken()
	{
		PlayerState = State.ESPAwaken;
		PlayAnimation("ESP Awaken", "On Awaken");
		LockControls = true;
		CurSpeed = 0f;
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - AwakenTimer > 0.4f && !HasAwakened)
		{
			HasAwakened = true;
			IsAwakened = true;
			PlayerVoice.Play(7);
			Camera.PlayShakeMotion(0f, 0.5f);
			SilverEffects.CreateESPActivateFX();
			DoPsychoShock(base.transform.position - base.transform.up * 0.1f, 12f);
		}
		if (Time.time - AwakenTimer > 1.25f && !AwakeDmg)
		{
			AwakeDmg = true;
			AttackSphere_Dir(base.transform.position + base.transform.up * 0.25f, 12f, 12f, 10);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		if (Time.time - AwakenTimer > 1.65f)
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

	private void StateESPAwakenEnd()
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Silver_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Silver_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Silver_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		HomingTarget = FindPsychoTarget();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Upheave || PlayerState == State.Hotspot || PlayerState == State.PsychoSmash || PlayerState == State.PsychicShot || PlayerState == State.PsychoShock || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Silver_Lua.c_rotation_speed);
				AccelerationSystem((!HasSpeedUp) ? Silver_Lua.c_run_acc : Silver_Lua.c_speedup_acc);
				if (PlayerState != State.Levitate && WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Silver_Lua.c_walk_speed_max : Silver_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = ((PlayerState == State.Levitate) ? Silver_Lua.c_float_walk_speed : (HasSpeedUp ? Silver_Lua.c_speedup_speed_max : (IsGrounded() ? Silver_Lua.c_run_speed_max : Silver_Lua.c_jump_run)));
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
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Upheave || PlayerState == State.Hotspot || PlayerState == State.PsychoSmash || PlayerState == State.PsychicShot || PlayerState == State.PsychoShock || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if ((((PlayerState == State.Ground || PlayerState == State.DashPanel || PlayerState == State.Path) && CurSpeed >= Silver_Lua.c_run_speed_max - 2f) || PlayerState == State.Levitate || (PlayerState == State.Upheave && UpheaveState == 0) || (PlayerState == State.Hotspot && HotspotState == 0) || PlayerState == State.PsychoSmash || PlayerState == State.GrabAll || PlayerState == State.Dash || PlayerState == State.PsychoShock || (PlayerState == State.ESPAwaken && Time.time - AwakenTimer > 0.4f) || (PlayerState == State.PsychicShot && !FirePsychicShot) || (PlayerState == State.Result && Time.time - ResultTime > 1f) || (PlayerState == State.Cutscene && PsiFX) || (PlayerState == State.IronSpring && PsiFX) || (PlayerState == State.Lotus && PsiFX) || (UsingPsychokinesis && PickedObjects.Count == 0) || PickedObjects.Count != 0) && !IsDead)
		{
			if (!UsePsiElements)
			{
				UsePsiElements = true;
				Audio.PlayOneShot(PsychoSounds[0], Audio.volume * 0.5f);
				SilverEffects.CreatePsiTrailFX();
			}
		}
		else if (UsePsiElements)
		{
			UsePsiElements = false;
			Audio.PlayOneShot(PsychoSounds[1], Audio.volume * 0.5f);
		}
		PsiLoopSource.volume = Mathf.Lerp(PsiLoopSource.volume, UsePsiElements ? 0.5f : 0f, Time.deltaTime * 10f);
		UpheaveSource.volume = Mathf.Lerp(UpheaveSource.volume, (PlayerState == State.Upheave && UpheaveState == 0 && StartUpheave) ? 0.5f : 0f, Time.deltaTime * 5f);
		if (PlayerState == State.UpReel)
		{
			AirActionTime = Time.time;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (!LockControls && PlayerState != State.Levitate && PlayerState != State.PsychoSmash && PlayerState != State.Upheave && PlayerState != State.Hotspot && PlayerState != State.GrabAll && PlayerState != State.PsychicShot)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
				{
					if (!ManipulateObjects && !OnUpheave())
					{
						ManipulateObjects = true;
					}
					else if (((PlayerState == State.Ground && TriggerCount == 0) || PlayerState != 0) && PickedObjects.Count != 0 && !HotspotObj && PlayerState != State.Hotspot)
					{
						ManipulateObjects = false;
						DoPsychokinesis();
					}
				}
			}
			else if ((PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.SnowBallDeath || PlayerState == State.Talk) && ManipulateObjects)
			{
				ManipulateObjects = false;
				DoPsychokinesis();
			}
			UsingPsychokinesis = !LockControls && !IsDead && Singleton<RInput>.Instance.P.GetButton("Right Trigger") && ManipulateObjects && !OnUpheave() && PlayerState != State.Levitate && PlayerState != State.Upheave && PlayerState != State.Hotspot && PlayerState != State.Upheave && PlayerState != State.UpheaveSmash;
			if (ManipulateObjects && UsingPsychokinesis)
			{
				DoPsychokinesis();
			}
			for (int i = 0; i < PickedObjects.Count; i++)
			{
				if (PickedObjects[i] == null || !PickedObjects[i].activeSelf)
				{
					UnityEngine.Object.Destroy(PickedObjectPoints[i]);
					PickedObjects.RemoveAt(i);
					PickedObjectPoints.RemoveAt(i);
				}
			}
			if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && PickedObjects.Count == 0 && ((PlayerState == State.Ground && ShouldAlignOrFall(Align: false) && !IsSinking) || PlayerState == State.Jump || PlayerState == State.Air || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.IronSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Lotus && !LockControls) || (PlayerState == State.Tarzan && !LockControls) || PlayerState == State.Levitate || (PlayerState == State.Upheave && UpheaveState == 1) || (PlayerState == State.Hotspot && HotspotState == 1) || (PlayerState == State.UpheaveSmash && UpheadSmashCanUseMoves) || (PlayerState == State.PsychoShock && ShockState == 1 && Time.time - ShockTime > 0.175f)) && HasFlameOfControl)
			{
				if (Singleton<RInput>.Instance.P.GetButton("Button B"))
				{
					ChargingPsychicKnife = true;
					if (Time.time - PsychicKnifeTime > Silver_Lua.c_psi_gauge_psi_knife_charge)
					{
						FullyChargedPsychicKnife = true;
					}
				}
				else
				{
					PsychicKnifeTime = Time.time;
				}
			}
			else
			{
				ChargingPsychicKnife = false;
				FullyChargedPsychicKnife = false;
				PsychicKnifeTime = Time.time;
			}
			if (PlayerState == State.Ground)
			{
				CanTeleDash = true;
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
				{
					AirActionTime = Time.time;
					StateMachine.ChangeState(StateJump);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StatePsychoSmash);
				}
				if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && ShouldAlignOrFall(Align: false) && PickedObjects.Count == 0 && !IsSinking)
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
				if (PsychoReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && ShouldAlignOrFall(Align: false) && !IsSinking && HUD.ActionDisplay != 0f)
				{
					if (TriggerCooler > 0f && TriggerCount == 1 && !OnUpheave())
					{
						OnGrabAll();
					}
					else
					{
						if (!UsingPsychokinesis)
						{
							OnUpheave();
						}
						TriggerCooler = 0.25f;
						TriggerCount++;
					}
				}
				if (TriggerCooler > 0f)
				{
					TriggerCooler -= Time.deltaTime;
				}
				else
				{
					TriggerCount = 0;
				}
			}
			if (PickedObjects.Count == 0)
			{
				if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					HoldTime = Time.time;
					PsychoShockTime = Time.time;
				}
				else if (AttackState == 1)
				{
					AttackState = 2;
				}
			}
			if (PlayerState == State.Jump || PlayerState == State.Air || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.IronSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Lotus && !LockControls) || (PlayerState == State.Tarzan && !LockControls))
			{
				if (((PlayerState == State.Jump && ReleasedKey) || PlayerState != State.Jump) && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					if (Time.time - AirActionTime < Silver_Lua.c_tele_dash_input && HUD.ActionDisplay > Silver_Lua.c_psi_gauge_teleport_dash && !FrontalCollision && CanTeleDash)
					{
						CanTeleDash = false;
						StateMachine.ChangeState(StateDash);
					}
					else if (HUD.ActionDisplay != 0f)
					{
						StateMachine.ChangeState(StateLevitate);
					}
				}
				if (PickedObjects.Count == 0)
				{
					if (AttackState == 2)
					{
						if (Time.time - HoldTime > 0.25f && Time.time - PsychoShockTime > 0.25f)
						{
							StateMachine.ChangeState(StatePsychoShock);
						}
						else if (Singleton<RInput>.Instance.P.GetButtonUp("Button X"))
						{
							StateMachine.ChangeState(StatePsychoSmash);
						}
					}
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StatePsychoSmash);
				}
				if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && PickedObjects.Count == 0)
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
			}
			else
			{
				PsychoShockTime = Time.time;
			}
			if (PlayerState == State.Levitate)
			{
				if (PickedObjects.Count == 0)
				{
					if (AttackState == 2)
					{
						if (Time.time - HoldTime > 0.25f && Time.time - PsychoShockTime > 0.25f)
						{
							StateMachine.ChangeState(StatePsychoShock);
						}
						else if (Singleton<RInput>.Instance.P.GetButtonUp("Button X"))
						{
							StateMachine.ChangeState(StatePsychoSmash);
						}
					}
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StatePsychoSmash);
				}
				if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && PickedObjects.Count == 0)
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
			}
			if (PickedObjects.Count == 0 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				AttackState = 1;
			}
			if (PlayerState == State.Upheave || PlayerState == State.Hotspot)
			{
				if (((PlayerState == State.Upheave && UpheaveState == 1) || (PlayerState == State.Hotspot && HotspotState == 1)) && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					AirActionTime = Time.time;
					StateMachine.ChangeState(StateJump);
				}
				if (((PlayerState == State.Upheave && UpheaveState == 1) || (PlayerState == State.Hotspot && HotspotState == 1)) && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StatePsychoSmash);
				}
				if (PlayerState == State.Upheave && UpheaveState == 0 && Time.time - UpheaveTime > 0.45f && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateUpheaveSmash);
				}
				if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && PickedObjects.Count == 0 && ((PlayerState == State.Upheave && UpheaveState == 1) || (PlayerState == State.Hotspot && HotspotState == 1)))
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
			}
			if (PlayerState == State.UpheaveSmash && UpheadSmashCanUseMoves)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					if (Time.time - AirActionTime < Silver_Lua.c_tele_dash_input && HUD.ActionDisplay > Silver_Lua.c_psi_gauge_teleport_dash && !FrontalCollision && CanTeleDash)
					{
						CanTeleDash = false;
						StateMachine.ChangeState(StateDash);
					}
					else if (HUD.ActionDisplay != 0f)
					{
						StateMachine.ChangeState(StateLevitate);
					}
				}
				if (HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && PickedObjects.Count == 0)
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
			}
			if (PlayerState == State.PsychoSmash && PSmashState == 1)
			{
				if (PickedObjects.Count != 0 && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					ThrowForce = Silver_Lua.c_psychosmash_begin;
					PSmashSwitch = true;
					PSmashSwitchIndex++;
					if (PSmashSwitchIndex > 1)
					{
						PSmashSwitchIndex = 0;
					}
					Animator.SetInteger("Psycho Smash Switch Index", PSmashSwitchIndex);
					Animator.SetTrigger("On Psycho Smash Switch");
					PsiChargeSources[1].volume = 0f;
					PsiChargeSources[0].Play();
					PsiChargeSources[1].Play();
					if (PSmashGrounded)
					{
						PlayerVoice.PlayRandom(8);
					}
					PSmashState = 0;
				}
				if (Time.time - PSmashTime > 0.175f && PSmashGrounded && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					AirActionTime = Time.time;
					StateMachine.ChangeState(StateJump);
				}
				if (Time.time - PSmashTime > 0.3f && (TargetDirection != Vector3.zero || Singleton<RInput>.Instance.P.GetButton("Right Trigger")))
				{
					if (Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
					{
						ManipulateObjects = true;
					}
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
			if (PlayerState == State.PsychicShot)
			{
				if (HUD.ActionDisplay > ((!IsKnife) ? Silver_Lua.c_psi_gauge_psi_shot : Silver_Lua.c_psi_gauge_psi_knife) && Time.time - PShotTime > ((!IsKnife) ? 0.35f : 0.4f) && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StatePsychicShot);
				}
				if (PShotGrounded && FirePsychicShot && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					AirActionTime = Time.time;
					StateMachine.ChangeState(StateJump);
				}
				if (FirePsychicShot && Time.time - PShotTime > ((!IsKnife) ? 0.5f : 0.55f) && (TargetDirection != Vector3.zero || Singleton<RInput>.Instance.P.GetButton("Right Trigger")))
				{
					if (Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
					{
						ManipulateObjects = true;
					}
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
			if (PlayerState == State.GrabAll && GrabAllReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				ManipulateObjects = false;
				DoPsychokinesis();
				StateMachine.ChangeState(StateGround);
			}
			if (PlayerState == State.PsychoShock && ShockState == 1)
			{
				if (Time.time - ShockTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					AirActionTime = Time.time;
					StateMachine.ChangeState(StateJump);
				}
				if (Time.time - ShockTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StatePsychoSmash);
				}
				if (Time.time - ShockTime > 0.175f && HUD.ActionDisplay > Silver_Lua.c_psi_gauge_psi_shot && ShouldAlignOrFall(Align: false) && PickedObjects.Count == 0 && !IsSinking)
				{
					if ((!HasFlameOfControl && Singleton<RInput>.Instance.P.GetButtonDown("Button B")) || (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && !FullyChargedPsychicKnife))
					{
						PsychicShot(Knife: false);
					}
					else if (HasFlameOfControl && ChargingPsychicKnife && !Singleton<RInput>.Instance.P.GetButton("Button B") && FullyChargedPsychicKnife)
					{
						PsychicShot(Knife: true);
					}
				}
				if (Time.time - ShockTime > 0.3f && (TargetDirection != Vector3.zero || Singleton<RInput>.Instance.P.GetButton("Right Trigger")))
				{
					if (Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
					{
						ManipulateObjects = true;
					}
					StateMachine.ChangeState(StateGround);
				}
			}
			HUD.LevelAnimator.SetBool("Is Upgrade Limit", HasSigilOfAwakening && !IsAwakened && HUD.ESPMaturityDisplay >= 30f);
			if (PlayerState == State.Ground && HasSigilOfAwakening && !IsAwakened && HUD.ESPMaturityDisplay >= 30f && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger"))
			{
				StateMachine.ChangeState(StateESPAwaken);
			}
			if (IsAwakened)
			{
				DoPsychoShock(base.transform.position + base.transform.up * 0.25f, 1.75f, Awaken: true);
				if (HUD.MaturityDisplay <= 0f || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.SnowBallDeath || PlayerState == State.TornadoDeath)
				{
					IsAwakened = false;
				}
			}
		}
		if (ChargingPsychicKnife)
		{
			if (!PlayKnifeChargeSound)
			{
				PlayKnifeChargeSound = true;
				KnifeChargeSource.Play();
				KnifeChargeSource.volume = 1f;
			}
		}
		else
		{
			if (PlayKnifeChargeSound)
			{
				PlayKnifeChargeSound = false;
			}
			KnifeChargeSource.volume = Mathf.Lerp(KnifeChargeSource.volume, 0f, Time.deltaTime * 5f);
		}
		UpgradeHoldSource.volume = Mathf.Lerp(UpgradeHoldSource.volume, (!IsAwakened && HUD.ESPMaturityDisplay >= 30f && HasSigilOfAwakening && PlayerState == State.Ground && PlayerState != State.ESPAwaken && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f) ? 1f : 0f, Time.deltaTime * 5f);
		ESPAuraSource.volume = Mathf.Lerp(ESPAuraSource.volume, IsAwakened ? 1f : 0f, Time.deltaTime * 5f);
		if (IsDead || PlayerState == State.Result || PlayerState == State.Cutscene || StageManager.StageState == StageManager.State.Event)
		{
			UsingPsychokinesis = false;
			if (ManipulateObjects)
			{
				ManipulateObjects = false;
				DoPsychokinesis();
			}
			ChargingPsychicKnife = false;
			FullyChargedPsychicKnife = false;
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
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "ESPGeneral")
			{
				HUD.ActionDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloat;
				IsAwakened = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool;
			}
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName == "ESPDisplay")
			{
				HUD.ESPMaturityDisplay = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarFloat;
			}
		}
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump && HasLotusOfResilience)
		{
			return 0;
		}
		if ((PlayerState == State.PsychoSmash && (PickedObjects.Count != 0 || PSmashState == 1)) || PlayerState == State.Dash || PlayerState == State.PsychoShock || PlayerState == State.Upheave)
		{
			return 1;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if (PlayerState == State.Hurt || PlayerState == State.Talk || PlayerState == State.WarpHole || PlayerState == State.ESPAwaken || PlayerState == State.Result || PlayerState == State.Cutscene || HasInvincibility || IsDead)
		{
			return true;
		}
		return result;
	}

	private void DoPsychokinesis(string RadiusType = "Psychokinesis")
	{
		if (ManipulateObjects)
		{
			if (!(HUD.ActionDisplay > 0f))
			{
				return;
			}
			Collider[] array = ((RadiusType == "GrabAll") ? Physics.OverlapSphere(base.transform.position + base.transform.up * 0.25f, 7.5f) : Physics.OverlapCapsule(base.transform.position + base.transform.up * 0.25f + base.transform.forward * 1f, base.transform.position + base.transform.up * 0.25f + base.transform.forward * 6.5f, 1f));
			for (int i = 0; i < array.Length; i++)
			{
				if ((((bool)array[i].GetComponent<PhysicsObj>() && (bool)array[i].GetComponent<PhysicsObj>().brokenPrefab && !array[i].GetComponent<PhysicsObj>().PsychoThrown && (bool)array[i].GetComponent<PhysicsObj>().Renderer) || ((bool)array[i].GetComponent<PhysicsObj>() && !array[i].GetComponent<PhysicsObj>().brokenPrefab && (bool)array[i].GetComponent<PhysicsObj>().Renderer) || ((bool)array[i].GetComponent<BrokenSnowBall>() && !array[i].GetComponent<BrokenSnowBall>().PsychoThrown) || ((bool)array[i].GetComponent<Parasol>() && !array[i].GetComponent<Parasol>().PsychoThrown) || ((bool)array[i].GetComponent<Missile>() && !array[i].GetComponent<Missile>().PsychoThrown) || ((bool)array[i].GetComponent<TimedBomb>() && array[i].GetComponent<TimedBomb>().enabled && !array[i].GetComponent<TimedBomb>().PsychoThrown) || ((bool)array[i].GetComponent<Forearm>() && array[i].GetComponent<Forearm>().enabled && !array[i].GetComponent<Forearm>().PsychoThrown) || ((bool)array[i].GetComponent<EnemyBase>() && !array[i].GetComponent<EnemyBase>().PsychoThrown && array[i].GetComponent<EnemyBase>().Stuned) || ((bool)array[i].GetComponent<Aqa_Mercury_Small>() && !array[i].GetComponent<Aqa_Mercury_Small>().PsychoThrown && !array[i].GetComponent<Aqa_Mercury_Small>().Appear) || (bool)array[i].GetComponent<Pendulum>()) && PickedObjects.IndexOf(array[i].gameObject) < 0)
				{
					HUD.DrainActionGauge(Silver_Lua.c_psi_gauge_catch_one);
					PickedObjects.Add(array[i].gameObject);
					GameObject gameObject = new GameObject("PickedObjectPoint" + PickedObjectPoints.Count);
					gameObject.transform.position = base.transform.position + base.transform.up * 1.75f;
					gameObject.transform.rotation = Quaternion.LookRotation((base.transform.position - array[i].transform.position).MakePlanar());
					gameObject.transform.SetParent(base.transform);
					PickedObjectPoints.Add(gameObject);
					array[i].SendMessage("OnPsychokinesis", gameObject.transform, SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponent<Gate>() || (bool)array[i].GetComponent<Bullet>())
				{
					array[i].SendMessage("OnPsychokinesis", base.transform, SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponentInParent<Fruit>())
				{
					array[i].SendMessageUpwards("OnPsychokinesis", base.transform, SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponentInParent<HangingRock>())
				{
					array[i].SendMessageUpwards("OnPsychokinesis", base.transform, SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponent<Dtd_Billiard>())
				{
					array[i].GetComponent<Dtd_Billiard>().IsGrabbed = true;
					HUD.DrainActionGauge(1.25f * ESPMult());
				}
				if ((bool)array[i].GetComponentInParent<FlameSingle>())
				{
					array[i].SendMessageUpwards("Activate", SendMessageOptions.DontRequireReceiver);
				}
			}
			return;
		}
		for (int j = 0; j < PickedObjects.Count; j++)
		{
			if (!PickedObjects[j].GetComponent<Parasol>() && !PickedObjects[j].GetComponent<Missile>() && !PickedObjects[j].GetComponent<TimedBomb>() && !PickedObjects[j].GetComponent<Forearm>() && !PickedObjects[j].GetComponent<EnemyBase>())
			{
				PickedObjects[j].SendMessage("PsychoThrowSetup", new List<GameObject>(PickedObjects), SendMessageOptions.DontRequireReceiver);
			}
			PickedObjects[j].SendMessage("OnReleasePsycho", SendMessageOptions.DontRequireReceiver);
		}
		PickedObjects.Clear();
		for (int k = 0; k < PickedObjectPoints.Count; k++)
		{
			UnityEngine.Object.Destroy(PickedObjectPoints[k]);
		}
		PickedObjectPoints.Clear();
	}

	private void DoUpheave(bool Raise = true)
	{
		if ((bool)UpheaveObj)
		{
			if (Raise)
			{
				base.transform.SetParent(UpheaveObj.transform);
				Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.localEulerAngles.y : 1f));
				Vector3 vector = quaternion * Vector3.forward;
				vector.y = 0f;
				vector.Normalize();
				Vector3 vector2 = quaternion * Vector3.right;
				vector2.y = 0f;
				vector2.Normalize();
				Vector3 normalized = (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * vector2 + Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * vector).normalized;
				Vector3 velocity = UpheaveObj.GetComponent<Rigidbody>().velocity;
				velocity = Vector3.Lerp(velocity, Vector3.up * ((Time.time - UpheaveTime < 1.75f) ? 5f : 0f) + normalized * 8f, Time.fixedDeltaTime * 6f);
				UpheaveObj.SendMessage("OnUpheave", new HitInfo(base.transform, velocity), SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				UpheaveObj.SendMessage("OnUpheaveRelease", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void DoPsychoThrow(Vector3 Position, float Radius, bool CapsuleRadius = false)
	{
		if (PickedObjects.Count == 0)
		{
			Collider[] array = (CapsuleRadius ? Physics.OverlapCapsule(Position, Position + base.transform.forward * 1f, Radius) : Physics.OverlapSphere(Position, Radius));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.layer == LayerMask.NameToLayer("BreakableObj") && (bool)array[i].GetComponentInChildren<Rigidbody>())
				{
					if (ThrowForce < Silver_Lua.c_psychosmash_power)
					{
						if (!array[i].GetComponent<Rigidbody>().isKinematic)
						{
							array[i].GetComponent<Rigidbody>().AddForce(base.transform.forward * ThrowForce * Silver_Lua.c_psychosmash_power, ForceMode.Impulse);
						}
						else if (ThrowForce > Silver_Lua.c_psychosmash_power / 2f)
						{
							array[i].GetComponent<Rigidbody>().useGravity = true;
							array[i].GetComponent<Rigidbody>().isKinematic = false;
							array[i].GetComponent<Rigidbody>().AddForce(base.transform.forward * ThrowForce * Silver_Lua.c_psychosmash_power, ForceMode.Impulse);
						}
					}
					else
					{
						array[i].SendMessage("OnHit", new HitInfo(base.transform, base.transform.forward * ThrowForce, 10), SendMessageOptions.DontRequireReceiver);
					}
				}
				if ((bool)array[i].GetComponent<BreakWall>() || ((bool)array[i].GetComponent<PhysicsObj>() && (bool)array[i].GetComponent<PhysicsObj>().brokenPrefab && !array[i].GetComponentInChildren<Rigidbody>()))
				{
					array[i].SendMessage("OnHit", new HitInfo(base.transform, base.transform.forward * ThrowForce, 10), SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponent<Aqa_Mercury_Small>())
				{
					array[i].GetComponent<Rigidbody>().AddForce(base.transform.forward * ThrowForce * Silver_Lua.c_psychosmash_power * 0.025f, ForceMode.Impulse);
				}
				if ((bool)array[i].GetComponent<EnemyBase>())
				{
					if (array[i].GetComponent<EnemyBase>().Stuned && (IsAwakened || (!IsAwakened && ThrowForce >= Silver_Lua.c_psychosmash_power)))
					{
						int damage = (IsAwakened ? 10 : (HasFlameOfControl ? 5 : 2));
						array[i].SendMessage("OnHit", new HitInfo(base.transform, base.transform.forward * ThrowForce, damage), SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						array[i].SendMessage("OnFlash", SendMessageOptions.DontRequireReceiver);
					}
				}
				else if ((bool)array[i].GetComponent<Dtd_Billiard>())
				{
					array[i].SendMessage("OnPsychoThrow", new HitInfo(null, base.transform.forward * ThrowForce, 0), SendMessageOptions.DontRequireReceiver);
				}
				else if ((bool)array[i].GetComponentInParent<FlameSingle>())
				{
					array[i].SendMessageUpwards("Activate", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					array[i].SendMessage("OnFlash", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if (ThrowForce == Silver_Lua.c_psychosmash_power)
		{
			List<Collider> list = new List<Collider>();
			List<GameObject> value = new List<GameObject>(PickedObjects);
			for (int j = 0; j < PickedObjects.Count; j++)
			{
				Collider[] componentsInChildren = PickedObjects[j].GetComponentsInChildren<Collider>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					list.Add(componentsInChildren[k]);
				}
			}
			for (int l = 0; l < PickedObjects.Count; l++)
			{
				Vector3 vector = (HomingTarget ? (HomingTarget.transform.position - PickedObjects[l].transform.position).normalized : base.transform.forward);
				Collider[] componentsInChildren2 = PickedObjects[l].GetComponentsInChildren<Collider>();
				for (int m = 0; m < list.Count; m++)
				{
					Collider[] array2 = componentsInChildren2;
					foreach (Collider collider in array2)
					{
						Physics.IgnoreCollision(list[m], collider);
					}
				}
				if (!PickedObjects[l].GetComponent<Parasol>() && !PickedObjects[l].GetComponent<Missile>() && !PickedObjects[l].GetComponent<TimedBomb>() && !PickedObjects[l].GetComponent<Forearm>() && !PickedObjects[l].GetComponent<EnemyBase>())
				{
					PickedObjects[l].SendMessage("PsychoThrowSetup", value, SendMessageOptions.DontRequireReceiver);
				}
				PickedObjects[l].SendMessage("OnPsychoThrow", new HitInfo(base.transform, vector * Common_Lua.c_psi_throw_speed, 10), SendMessageOptions.DontRequireReceiver);
				PickedObjects[l] = null;
			}
		}
		else
		{
			if (PickedObjects.Count == 0)
			{
				return;
			}
			Vector3 vector2 = (HomingTarget ? (HomingTarget.transform.position - PickedObjects[0].transform.position).normalized : base.transform.forward);
			Collider[] componentsInChildren3 = PickedObjects[0].GetComponentsInChildren<Collider>();
			List<Collider> list2 = new List<Collider>();
			for (int num = 0; num < PickedObjects.Count; num++)
			{
				Collider[] componentsInChildren4 = PickedObjects[num].GetComponentsInChildren<Collider>();
				for (int num2 = 0; num2 < componentsInChildren4.Length; num2++)
				{
					list2.Add(componentsInChildren4[num2]);
				}
			}
			for (int num3 = 0; num3 < list2.Count; num3++)
			{
				Collider[] array2 = componentsInChildren3;
				foreach (Collider collider2 in array2)
				{
					Physics.IgnoreCollision(list2[num3], collider2);
				}
			}
			if (!PickedObjects[0].GetComponent<Parasol>() && !PickedObjects[0].GetComponent<Missile>() && !PickedObjects[0].GetComponent<TimedBomb>() && !PickedObjects[0].GetComponent<Forearm>() && !PickedObjects[0].GetComponent<EnemyBase>())
			{
				PickedObjects[0].SendMessage("PsychoThrowSetup", new List<GameObject>(PickedObjects), SendMessageOptions.DontRequireReceiver);
			}
			PickedObjects[0].SendMessage("OnPsychoThrow", new HitInfo(base.transform, vector2 * Common_Lua.c_psi_throw_speed, (!IsAwakened) ? 1 : 10), SendMessageOptions.DontRequireReceiver);
			PickedObjects[0] = null;
		}
	}

	private void DoPsychoShock(Vector3 Position, float Radius, bool Awaken = false)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (!Awaken)
			{
				if ((array[i].gameObject.layer == LayerMask.NameToLayer("BreakableObj") || array[i].gameObject.layer == LayerMask.NameToLayer("AttackableSolid")) && array[i].gameObject.layer != LayerMask.NameToLayer("PlayerPushCol") && (bool)array[i].GetComponentInChildren<Rigidbody>() && !array[i].GetComponent<Fruit>())
				{
					if (array[i].GetComponent<Rigidbody>().isKinematic)
					{
						array[i].GetComponent<Rigidbody>().useGravity = true;
						array[i].GetComponent<Rigidbody>().isKinematic = false;
					}
					Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
					if (vector == Vector3.zero)
					{
						vector = base.transform.forward.MakePlanar();
					}
					Vector3 vector2 = (vector + Vector3.up * UnityEngine.Random.Range(0.1f, 0.25f)).normalized * 20f;
					array[i].GetComponent<Rigidbody>().AddForce(vector2 * ((!array[i].GetComponent<Pendulum>()) ? 40f : 20f), ForceMode.Impulse);
				}
				if ((bool)array[i].GetComponent<Aqa_Mercury_Small>())
				{
					Vector3 vector3 = (array[i].transform.position - base.transform.position).MakePlanar();
					if (vector3 == Vector3.zero)
					{
						vector3 = base.transform.forward.MakePlanar();
					}
					Vector3 vector4 = (vector3 + Vector3.up * UnityEngine.Random.Range(0.1f, 0.25f)).normalized * 2.5f;
					array[i].GetComponent<Rigidbody>().AddForce(vector4 * 5f, ForceMode.Impulse);
				}
				if ((bool)array[i].GetComponentInParent<Common_Switch>())
				{
					array[i].SendMessageUpwards("OnSwitch", SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponentInParent<ItemBox>())
				{
					array[i].SendMessageUpwards("OnHit", new HitInfo(base.transform, Vector3.zero), SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponentInParent<FlameSingle>())
				{
					array[i].SendMessageUpwards("OnFlash", SendMessageOptions.DontRequireReceiver);
				}
			}
			array[i].SendMessage("OnDeflect", base.transform, SendMessageOptions.DontRequireReceiver);
			string methodName = (array[i].GetComponentInParent<eCannon>() ? "FullStun" : "OnFlash");
			array[i].SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void UpheaveDetach()
	{
		if (PlayerState == State.Upheave && UpheaveState == 0 && Time.time - UpheaveTime > 0.45f)
		{
			Audio.PlayOneShot(UpheaveRotSounds[(!(HUD.ActionDisplay < 0.1f)) ? 1u : 0u], Audio.volume * ((HUD.ActionDisplay < 0.1f) ? 1f : 5f));
			if (HUD.ActionDisplay > 0.1f)
			{
				SilverEffects.CreateUpheaveFX();
			}
			DoUpheave(Raise: false);
			base.transform.SetParent(null);
		}
	}

	private void PsychicShot(bool Knife)
	{
		IsKnife = Knife;
		StateMachine.ChangeState(StatePsychicShot);
	}

	private bool OnUpheave()
	{
		if (HUD.ActionDisplay > 33.333332f && !UsingPsychokinesis && Physics.Raycast(base.transform.position + base.transform.up * 0.25f, -base.transform.up, out var hitInfo, 0.6f) && (bool)hitInfo.transform.GetComponent<Rigidbody>() && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj") && !hitInfo.transform.GetComponent<Parasol>())
		{
			UpheaveObj = hitInfo.transform.gameObject;
			StateMachine.ChangeState(StateUpheave);
			return true;
		}
		if ((bool)HotspotObj && HotspotObj.Appear && PlayerState == State.Ground)
		{
			StateMachine.ChangeState(StateHotspot);
			return true;
		}
		return false;
	}

	private bool OnGrabAll()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + base.transform.up * 0.25f, 7.5f);
		for (int i = 0; i < array.Length; i++)
		{
			if (((bool)array[i].GetComponent<PhysicsObj>() && (bool)array[i].GetComponent<PhysicsObj>().brokenPrefab && !array[i].GetComponent<PhysicsObj>().PsychoThrown && (bool)array[i].GetComponent<PhysicsObj>().Renderer) || ((bool)array[i].GetComponent<PhysicsObj>() && !array[i].GetComponent<PhysicsObj>().brokenPrefab && (bool)array[i].GetComponent<PhysicsObj>().Renderer) || ((bool)array[i].GetComponent<BrokenSnowBall>() && !array[i].GetComponent<BrokenSnowBall>().PsychoThrown) || ((bool)array[i].GetComponent<Missile>() && !array[i].GetComponent<Missile>().PsychoThrown) || ((bool)array[i].GetComponent<TimedBomb>() && array[i].GetComponent<TimedBomb>().enabled && !array[i].GetComponent<TimedBomb>().PsychoThrown) || ((bool)array[i].GetComponent<Forearm>() && array[i].GetComponent<Forearm>().enabled && !array[i].GetComponent<Forearm>().PsychoThrown) || ((bool)array[i].GetComponent<EnemyBase>() && !array[i].GetComponent<EnemyBase>().PsychoThrown && array[i].GetComponent<EnemyBase>().Stuned))
			{
				DoPsychokinesis("GrabAll");
				if (!IsAwakened)
				{
					StateMachine.ChangeState(StateGrabAll);
				}
				else
				{
					ThrowForce = Silver_Lua.c_psychosmash_begin;
					SilverEffects.CreateGrabAllActivateFX();
				}
				return true;
			}
		}
		return false;
	}

	public float ESPMult()
	{
		if (!IsAwakened)
		{
			return 1f;
		}
		return 0.65f;
	}

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		PsychoAnimation = Mathf.MoveTowards(PsychoAnimation, (!UsingPsychokinesis && PickedObjects.Count == 0) ? 0f : ((PickedObjects.Count != 0) ? (-1f) : 1f), Time.deltaTime * 6f);
		Animator.SetFloat("Psycho Animation", PsychoAnimation);
		Animator.SetFloat("Balancer Factor", BalancerFactor);
		Animator.SetBool("Using Psychokinesis", UsePsiElements);
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Levitate || PlayerState == State.Dash) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Hotspot")
		{
			HotspotObj = collider.gameObject.GetComponent<Common_PsiMarkSphere>();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Hotspot")
		{
			HotspotObj = null;
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
			if (PlayerState != State.Grinding && PlayerState != State.Balancer && PlayerState != State.IronSpring)
			{
				UpheaveDetach();
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
		if (!IsDead && PlayerState != State.Death && PlayerState != State.FallDeath && PlayerState != State.DrownDeath && PlayerState != State.SnowBallDeath && PlayerState != State.TornadoDeath && PlayerState != State.Path && PlayerState != State.WarpHole && (PlayerState != State.Lotus || !LockControls) && PlayerState != State.Hold)
		{
			IsDead = true;
			UpheaveDetach();
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
		case "StateHotspot":
			StateMachine.ChangeState(StateHotspot);
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
