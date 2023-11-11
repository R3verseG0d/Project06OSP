using System;
using System.Collections;
using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Omega : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		SlowFall = 4,
		Hover = 5,
		OmegaShot = 6,
		OmegaLauncher = 7,
		GatlingGun = 8,
		Hurt = 9,
		EdgeDanger = 10,
		Grinding = 11,
		Death = 12,
		FallDeath = 13,
		DrownDeath = 14,
		SnowBallDeath = 15,
		TornadoDeath = 16,
		Talk = 17,
		Path = 18,
		WarpHole = 19,
		Result = 20,
		Cutscene = 21,
		DashPanel = 22,
		Spring = 23,
		WideSpring = 24,
		JumpPanel = 25,
		DashRing = 26,
		RainbowRing = 27,
		Balancer = 28
	}

	[Header("Player Framework")]
	public OmegaEffects OmegaEffects;

	public Transform LeftArmBone;

	public Transform RightArmBone;

	public Transform[] GunPoints;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	internal bool LockOnTargets;

	private bool CanOmegaLauncher;

	private bool IsLockOnShot;

	private float ReloadLauncher;

	private float LaserTime;

	private float HoverAnim;

	private int LaserMax;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	[Header("Projectiles")]
	public GameObject FireBall;

	public GameObject HomingLaser;

	public GameObject GatlingGunBullet;

	internal List<GameObject> LaserTargets;

	private bool Aim;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip[] OmegaShotSounds;

	public AudioClip LockOnShotSound;

	public AudioClip BrakeSound;

	public AudioClip GunModeSound;

	public AudioClip GunModeExitSound;

	public AudioClip GunEmptySound;

	public AudioClip GunSpinEndSound;

	[Header("Audio Sources")]
	public AudioSource HoverSource;

	public AudioSource[] GunSources;

	private bool PlayHoverSound;

	private bool PlayGunSound;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private bool ReachedApex;

	internal float ComboShotTimer;

	internal int ComboShotCount;

	private bool ComboShotDamage;

	private bool StartShotFlurry;

	private float ComboShotSpeed;

	private float FlurryTime;

	private int ShotCount;

	private int MaxShots;

	private int ShotArmID;

	private float LauncherTime;

	internal float LauncherSide;

	private Vector3 GunVel;

	private int GunState;

	private int GunIndex;

	private float GunTime;

	private float GunTimeStamp;

	private float GunMoveSpeed;

	private int GunBulletCount;

	private float HurtTime;

	private float EdgeDangerTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Omega_Lua.c_player_name;
		PlayerNameShort = Omega_Lua.c_player_name_short;
		WalkSpeed = Omega_Lua.c_walk_speed_max;
		TopSpeed = Omega_Lua.c_run_speed_max;
		BrakeSpeed = Omega_Lua.c_brake_acc;
		GrindSpeedOrg = Omega_Lua.c_grind_speed_org;
		GrindAcc = Omega_Lua.c_grind_acc;
		GrindSpeedMax = Omega_Lua.c_grind_speed_max;
		CanOmegaLauncher = true;
	}

	public override void Start()
	{
		base.Start();
		HUD.AmmoDisplay = Omega_Lua.c_ammo_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Omega_Lua.OpenGauge(), Omega_Lua.c_gauge_max, Omega_Lua.c_gauge_heal, Omega_Lua.c_gauge_heal_delay);
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
		if (TargetDirection.magnitude == 0f && !LockOnTargets)
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Omega_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Omega_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Omega_Lua.c_run_speed_max) / Omega_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Omega_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Omega_Lua.c_run_speed_max;
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
		HalveSinkJump = IsSinking && ColName != "2820000d";
		ReleasedKey = false;
		ReachedApex = false;
		PlayAnimation("Jump Up", "On Jump");
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Omega_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Omega_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
		OmegaEffects.CreateJetFireFX();
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
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && !ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && Time.time - JumpTime < ((!HalveSinkJump) ? 0.7f : 0.4f))
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

	private void StateHoverStart()
	{
		PlayerState = State.Hover;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateHover()
	{
		PlayerState = State.Hover;
		PlayAnimation("Air Falling", "On Air Fall");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, -1.25f, Time.fixedDeltaTime * 5f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (!Singleton<RInput>.Instance.P.GetButton("Button A") || HUD.ActionDisplay <= 0f)
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

	private void StateHoverEnd()
	{
	}

	private void StateOmegaShotStart()
	{
		PlayerState = State.OmegaShot;
		Animator.SetInteger("Combo ID", ComboShotCount);
		Animator.SetTrigger("On Omega Shot");
		ComboShotTimer = Time.time;
		ComboShotCount++;
		ComboShotSpeed = ((ComboShotCount < 3) ? (Omega_Lua.c_run_speed_max * ((TargetDirection != Vector3.zero) ? 0.85f : 0.45f)) : 0f);
		ComboShotDamage = true;
		QueuedPress = false;
		if (ComboShotCount < 3)
		{
			Audio.PlayOneShot(OmegaShotSounds[ComboShotCount - 1], Audio.volume);
		}
		else
		{
			Audio.PlayOneShot(OmegaShotSounds[4], Audio.volume);
		}
		ShotCount = 0;
		MaxShots = 8;
		StartShotFlurry = false;
	}

	private void StateOmegaShot()
	{
		PlayerState = State.OmegaShot;
		LockControls = ComboShotCount == 3 && Time.time - ComboShotTimer < 1.5f;
		if (ComboShotCount < 3)
		{
			ComboShotSpeed = Mathf.MoveTowards(ComboShotSpeed, 0f, Time.fixedDeltaTime * 25f);
			float radius = ((ComboShotCount == 1) ? 2f : 2.4f);
			if (ComboShotDamage && AttackSphere(base.transform.position + base.transform.up * 0.25f, radius, base.transform.forward * Omega_Lua.c_omega_shot_power * 0.5f, Omega_Lua.c_omega_shot_damage))
			{
				ComboShotDamage = false;
			}
			if (ComboShotCount == 1)
			{
				OmegaEffects.OnOmegaShotTrailsFX(Time.time - ComboShotTimer > 0.055f && Time.time - ComboShotTimer < 0.175f);
			}
		}
		else
		{
			if (!StartShotFlurry && Time.time - ComboShotTimer > 0.175f)
			{
				FlurryTime = Time.time + 0.1f;
				Audio.PlayOneShot(OmegaShotSounds[3], Audio.volume);
				StartShotFlurry = true;
			}
			if (StartShotFlurry && Time.time > FlurryTime + (float)ShotCount * 0.1325f && ShotCount < MaxShots)
			{
				OmegaEffects.CreateOmegaShotFX(ShotArmID);
				AttackSphere(OmegaEffects.ShotBones[ShotArmID].position + OmegaEffects.ShotBones[ShotArmID].forward * 1.25f, Omega_Lua.c_omega_shot.radius, OmegaEffects.ShotBones[ShotArmID].forward * Omega_Lua.c_omega_shot_power, Omega_Lua.c_omega_shot_damage);
				Audio.PlayOneShot(OmegaShotSounds[2], Audio.volume * 0.5f);
				ShotArmID++;
				if (ShotArmID > 1)
				{
					ShotArmID = 0;
				}
				ShotCount++;
			}
		}
		CurSpeed = ComboShotSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (Time.time - ComboShotTimer > ((ComboShotCount < 3) ? 0.8f : 1.95f))
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateOmegaShotEnd()
	{
		OmegaEffects.OnOmegaShotTrailsFX(Enable: false);
	}

	private void StateOmegaLauncherStart()
	{
		PlayerState = State.OmegaLauncher;
		LauncherTime = Time.time;
		LauncherSide += 1f;
		if (LauncherSide > 1f)
		{
			LauncherSide = 0f;
		}
		PlayAnimation("Air Falling", "On Air Fall");
		Animator.SetFloat("Launcher Side", LauncherSide);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		FireBall component = UnityEngine.Object.Instantiate(FireBall, base.transform.position + base.transform.up * 1f + base.transform.forward * 1f + base.transform.right * ((LauncherSide == 1f) ? (-0.75f) : 0.75f), base.transform.rotation).GetComponent<FireBall>();
		component.Player = base.transform;
		component.ClosestTarget = FindHomingTarget();
		OmegaEffects.CreateOmegaLauncherFX();
	}

	private void StateOmegaLauncher()
	{
		PlayerState = State.OmegaLauncher;
		PlayAnimation("Omega Launcher", "On Omega Launcher");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, -0.45f, Time.fixedDeltaTime * 8f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
		if (Time.time - LauncherTime > 0.6f)
		{
			if (Singleton<RInput>.Instance.P.GetButton("Button A") && HUD.ActionDisplay > 0f)
			{
				StateMachine.ChangeState(StateHover);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateOmegaLauncherEnd()
	{
	}

	private void StateGatlingGunStart()
	{
		PlayerState = State.GatlingGun;
		GunState = 0;
		GunTime = Time.time;
		GunIndex = 0;
		Audio.PlayOneShot(GunModeSound, Audio.volume * 0.6f);
		PlayGunSound = false;
		GunMoveSpeed = 0f;
		GunVel = Vector3.zero;
		GunBulletCount = 0;
	}

	private void StateGatlingGun()
	{
		PlayerState = State.GatlingGun;
		CurSpeed = 0f;
		GeneralMeshRotation = base.transform.rotation;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation, Time.deltaTime * 10f);
		if (GunState == 0 || GunState == 1)
		{
			LockControls = true;
			Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.localEulerAngles.y : 1f));
			Vector3 vector = quaternion * Vector3.forward;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = quaternion * Vector3.right;
			vector2.y = 0f;
			vector2.Normalize();
			TargetDirection = (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * vector2 + Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * vector).normalized;
			TargetDirection = Quaternion.FromToRotation(Vector3.up, base.transform.up) * TargetDirection;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((base.transform.position - Camera.transform.position).MakePlanar()), Time.fixedDeltaTime * Omega_Lua.c_rotation_speed);
			if (Time.time - GunTime > 0.6f && !PlayGunSound)
			{
				PlayGunSound = true;
				GunSources[1].volume = 0f;
				GunSources[0].Play();
				GunSources[1].Play();
			}
			if (PlayGunSound && !GunSources[0].isPlaying)
			{
				GunSources[1].volume = 0.4f;
			}
		}
		else if (PlayGunSound)
		{
			PlayGunSound = false;
			Audio.PlayOneShot(GunSpinEndSound, Audio.volume * 0.6f);
			GunSources[0].Stop();
			GunSources[1].Stop();
		}
		if (GunState == 0)
		{
			_Rigidbody.velocity = Vector3.zero;
			PlayAnimation("Gatling Gun", "On Gatling Gun");
			if (Time.time - GunTime > 0.9f)
			{
				HUD.UseCrosshair();
				HUD.OpenWeapons(Omega_Lua.c_ammo_max, "GadgetVulcan");
				GunState = 1;
			}
		}
		else if (GunState == 1)
		{
			if (!FrontalCollision && TargetDirection.magnitude != 0f)
			{
				if (GunMoveSpeed > 6f)
				{
					GunMoveSpeed = 6f;
				}
				else
				{
					GunMoveSpeed += 8f * Time.fixedDeltaTime;
				}
			}
			else if (GunMoveSpeed > 0f)
			{
				GunMoveSpeed -= 8f * Time.fixedDeltaTime;
			}
			if (GunMoveSpeed <= 0f)
			{
				GunMoveSpeed = 0f;
			}
			GunVel = Vector3.Lerp(GunVel, TargetDirection, Time.fixedDeltaTime * 5f);
			_Rigidbody.velocity = GunVel * GunMoveSpeed;
			if (Singleton<RInput>.Instance.P.GetButton("Right Trigger") && Time.time > GunTimeStamp)
			{
				GunTimeStamp = Time.time + 0.075f;
				if (HUD.AmmoDisplay > 0)
				{
					Ray ray = Camera.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
					RaycastHit hitInfo;
					Vector3 vector3 = ((!Physics.Raycast(ray, out hitInfo)) ? ray.GetPoint(500f) : hitInfo.point);
					Vector3 forward = vector3 - GunPoints[GunIndex].position;
					forward.x += UnityEngine.Random.Range(-0.025f, 0.025f);
					forward.y += UnityEngine.Random.Range(-0.025f, 0.025f);
					forward.z += UnityEngine.Random.Range(-0.025f, 0.025f);
					forward.Normalize();
					GunBulletCount++;
					if (GunBulletCount > 1)
					{
						GunBulletCount = 0;
					}
					GadgetBullet componentInChildren = UnityEngine.Object.Instantiate(GatlingGunBullet, GunPoints[GunIndex].position, Quaternion.LookRotation(forward)).GetComponentInChildren<GadgetBullet>();
					componentInChildren.Player = base.transform;
					componentInChildren.DealDamage = GunBulletCount == 1;
					OmegaEffects.CreateGunMuzzleFX(GunPoints[GunIndex].position, Quaternion.LookRotation(forward));
					Animator.SetInteger("Gun Index", GunIndex);
					Animator.SetTrigger("On Gatling Gun Shoot");
					HUD.AmmoDisplay--;
				}
				else
				{
					Audio.PlayOneShot(GunEmptySound, Audio.volume * 0.6f);
				}
				GunIndex++;
				if (GunIndex > GunPoints.Length - 1)
				{
					GunIndex = 0;
				}
			}
		}
		else
		{
			_Rigidbody.velocity = Vector3.zero;
			if (Time.time - GunTime > 1.5f)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
			{
				Camera.UncancelableEvent = false;
				Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
			}
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateGatlingGunEnd()
	{
		HUD.UseCrosshair(EndCrosshair: true);
		HUD.CloseWeapons();
		if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
		{
			Camera.UncancelableEvent = false;
			Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
		}
		PlayGunSound = false;
		GunMoveSpeed = 0f;
		GunSources[0].Stop();
		GunSources[1].Stop();
		Animator.SetTrigger("Additive Idle");
	}

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		HurtTime = Time.time;
		OmegaEffects.CreateHurtFX();
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Omega_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Omega_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Omega_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		if ((PlayerState == State.Ground || PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.Hover || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls)) && !IsLockOnShot)
		{
			if (Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				if (!LockOnTargets)
				{
					LaserTime = Time.time;
					LaserMax = 1;
					Aim = true;
					LaserClosestTargets.Clear();
					LockOnTargets = true;
				}
				if (Time.time - LaserTime > 0.1f && LaserMax < 5)
				{
					LaserTime = Time.time;
					LaserMax++;
				}
				LaserTargets = FindLaserTarget(LaserMax);
			}
			else if (LockOnTargets)
			{
				Aim = false;
				LockOnTargets = false;
			}
		}
		else if (LockOnTargets)
		{
			Aim = false;
			LockOnTargets = false;
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.OmegaShot || PlayerState == State.GatlingGun || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Omega_Lua.c_rotation_speed * ((PlayerState != State.OmegaLauncher) ? 1f : 0.625f));
				AccelerationSystem((!HasSpeedUp) ? Omega_Lua.c_run_acc : Omega_Lua.c_speedup_acc);
				if (PlayerState != State.OmegaLauncher)
				{
					if (PlayerState != State.Hover)
					{
						if (WalkSwitch)
						{
							MaximumSpeed = (IsGrounded() ? Omega_Lua.c_walk_speed_max : Omega_Lua.c_jump_walk);
						}
						else
						{
							MaximumSpeed = (HasSpeedUp ? Omega_Lua.c_speedup_speed_max : (IsGrounded() ? Omega_Lua.c_run_speed_max : Omega_Lua.c_jump_run));
						}
					}
					else
					{
						MaximumSpeed = Omega_Lua.c_jump_run * 1.3f;
					}
				}
				else
				{
					MaximumSpeed = Omega_Lua.c_jump_run * 0.25f;
				}
			}
			else
			{
				LockControls = false;
			}
		}
		SlopePhysics(PlayerState == State.Ground && CurSpeed < 20f);
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.OmegaShot || PlayerState == State.GatlingGun || PlayerState == State.Hurt || PlayerState == State.EdgeDanger || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if ((PlayerState == State.Ground && CurSpeed >= 20f) || PlayerState == State.Hover || (PlayerState == State.OmegaLauncher && (!IsGrounded() || (IsGrounded() && CurSpeed >= 20f))) || (PlayerState == State.DashPanel && CurSpeed >= 20f) || (PlayerState == State.Path && CurSpeed >= 20f))
		{
			if (!PlayHoverSound)
			{
				PlayHoverSound = true;
				HoverSource.Play();
				HoverSource.volume = 1f;
			}
		}
		else
		{
			if (PlayHoverSound)
			{
				PlayHoverSound = false;
			}
			HoverSource.volume = Mathf.Lerp(HoverSource.volume, 0f, Time.deltaTime * 2.5f);
		}
		if (Singleton<GameManager>.Instance.GameState == GameManager.State.Paused || Singleton<GameManager>.Instance.GameState == GameManager.State.Result || StageManager.StageState == StageManager.State.Event || IsDead || PlayerState == State.Talk)
		{
			return;
		}
		if (PlayerState == State.Ground)
		{
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
			{
				StateMachine.ChangeState(StateJump);
			}
			if (!IsLockOnShot && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking)
			{
				ComboShotCount = 0;
				StateMachine.ChangeState(StateOmegaShot);
			}
			if (!IsLockOnShot && LockOnTargets && LaserTargets.Count > 0 && Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
			{
				StartCoroutine(OnLockOnShot());
			}
			if (!IsLockOnShot && Singleton<RInput>.Instance.P.GetButtonDown("Button Y") && ShouldAlignOrFall(Align: false) && !IsSinking)
			{
				StateMachine.ChangeState(StateGatlingGun);
				Camera.StateMachine.ChangeState(Camera.StateOverTheShoulderFadeIn);
				Camera.UncancelableEvent = true;
			}
		}
		if (PlayerState == State.EdgeDanger && EdgeDangerTime > 1.5f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			StateMachine.ChangeState(StateJump);
		}
		if (PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.SlowFall || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
		{
			if (((PlayerState == State.Jump && ReleasedKey) || PlayerState != State.Jump) && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && HUD.ActionDisplay > 0f)
			{
				StateMachine.ChangeState(StateHover);
			}
			if (!IsLockOnShot && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && HUD.ActionDisplay > 0f && (CanOmegaLauncher || (!CanOmegaLauncher && Time.time - ReloadLauncher > Omega_Lua.c_reload_launcher)))
			{
				CanOmegaLauncher = false;
				ReloadLauncher = Time.time;
				StateMachine.ChangeState(StateOmegaLauncher);
			}
			if (!IsLockOnShot && LockOnTargets && LaserTargets.Count > 0 && Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
			{
				StartCoroutine(OnLockOnShot());
			}
		}
		else if (PlayerState != State.Hover && Time.time - ReloadLauncher > Omega_Lua.c_reload_launcher)
		{
			CanOmegaLauncher = true;
		}
		if (PlayerState == State.Hover)
		{
			if (!IsLockOnShot && LockOnTargets && LaserTargets.Count > 0 && Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
			{
				StartCoroutine(OnLockOnShot());
			}
			if (!IsLockOnShot && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && HUD.ActionDisplay > 0f && (CanOmegaLauncher || (!CanOmegaLauncher && Time.time - ReloadLauncher > Omega_Lua.c_reload_launcher)))
			{
				CanOmegaLauncher = false;
				ReloadLauncher = Time.time;
				StateMachine.ChangeState(StateOmegaLauncher);
			}
		}
		if (PlayerState == State.OmegaShot)
		{
			if (!QueuedPress && OnButtonPressed("Button X"))
			{
				QueuedPress = Time.time - ComboShotTimer > 0.15f;
			}
			if (Time.time - ComboShotTimer > ((ComboShotCount < 2) ? 0.25f : 0.35f) && ShouldAlignOrFall(Align: false) && ComboShotCount < 3 && OnCombo("Button X"))
			{
				if (Time.time - ComboShotTimer > 0.5f && ComboShotCount != 0)
				{
					ComboShotCount = 0;
				}
				StateMachine.ChangeState(StateOmegaShot);
			}
			if (Time.time - ComboShotTimer > ((ComboShotCount < 3) ? 0.45f : 1.4f) && TargetDirection.magnitude != 0f && !LockControls)
			{
				StateMachine.ChangeState(StateGround);
			}
			if (Time.time - ComboShotTimer > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
		}
		else if (ComboShotCount != 0)
		{
			ComboShotCount = 0;
		}
		if (PlayerState == State.OmegaLauncher && Time.time - LauncherTime > 0.35f && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && HUD.ActionDisplay > 0f)
		{
			StateMachine.ChangeState(StateOmegaLauncher);
		}
		if (PlayerState != State.GatlingGun)
		{
			return;
		}
		if (GunState == 1 && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
		{
			PlayAnimation("Gatling Gun End", "On Gatling Gun End");
			HUD.UseCrosshair(EndCrosshair: true);
			HUD.CloseWeapons();
			Audio.PlayOneShot(GunModeExitSound, Audio.volume * 0.6f);
			if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
			{
				Camera.UncancelableEvent = false;
				Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
			}
			GunTime = Time.time;
			GunMoveSpeed = 0f;
			GunState = 2;
		}
		if (GunState == 2 && TargetDirection != Vector3.zero)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (Time.time - GunTime > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			StateMachine.ChangeState(StateJump);
			if (GunState == 1)
			{
				HUD.UseCrosshair(EndCrosshair: true);
				HUD.CloseWeapons();
			}
			Audio.PlayOneShot(GunModeExitSound, Audio.volume * 0.6f);
			if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
			{
				Camera.UncancelableEvent = false;
				Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
			}
		}
	}

	private void LateUpdate()
	{
		if (PlayerState == State.GatlingGun && ((GunState == 0 && Time.time - GunTime > 1.175f) || GunState == 1))
		{
			LeftArmBone.eulerAngles = Quaternion.LookRotation(Camera.transform.forward).eulerAngles;
			LeftArmBone.localEulerAngles += new Vector3(92.5f, 0f, -15f);
			RightArmBone.eulerAngles = Quaternion.LookRotation(Camera.transform.forward).eulerAngles;
			RightArmBone.localEulerAngles += new Vector3(78f, 0f, -5f);
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

	private IEnumerator OnLockOnShot()
	{
		IsLockOnShot = true;
		float Timer = 0f;
		float LockShotTime = Time.time;
		float LaserCooldown = Time.time;
		int LaserShotCount = 0;
		Animator.SetTrigger("On Lock On Shot");
		while (Timer <= 0.675f)
		{
			Timer = Time.time - LockShotTime;
			if (LaserShotCount < LaserTargets.Count && Time.time - LaserCooldown > 0.05f)
			{
				LaserShotCount++;
				LaserCooldown = Time.time;
				HomingLaser component = UnityEngine.Object.Instantiate(HomingLaser, base.transform.position + base.transform.up * 1f + base.transform.right * -0.5f, base.transform.rotation).GetComponent<HomingLaser>();
				component.Player = base.transform;
				component.ClosestTarget = LaserTargets[LaserShotCount - 1];
				Audio.PlayOneShot(LockOnShotSound, Audio.volume);
			}
			yield return new WaitForFixedUpdate();
		}
		IsLockOnShot = false;
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.OmegaShot)
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
		HoverAnim = Mathf.MoveTowards(HoverAnim, (PlayerState == State.Hover) ? 1f : 0f, Time.deltaTime * 10f);
		Animator.SetFloat("Gun Move Speed", GunMoveSpeed);
		Animator.SetFloat("Lock On Blend", Mathf.Lerp(Animator.GetFloat("Lock On Blend"), (CurSpeed > 0f || !IsGrounded()) ? 1f : 0f, Time.deltaTime * 10f));
		Animator.SetFloat("Hover Anim", HoverAnim);
		Animator.SetBool("Is Aim", Aim);
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Hover) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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
			if (PlayerState != State.Grinding && PlayerState != State.Balancer && PlayerState != State.GatlingGun)
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
