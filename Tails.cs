using System;
using STHLua;
using UnityEngine;

public class Tails : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Brake = 1,
		Jump = 2,
		Air = 3,
		SlowFall = 4,
		Fly = 5,
		TailSwipe = 6,
		AerialTailSwipe = 7,
		DummyRingBomb = 8,
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
		UpReel = 27,
		Balancer = 28
	}

	[Header("Player Framework")]
	public TailsEffects TailsEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	internal bool CanRegenFly;

	internal bool CanAirThrow;

	private bool PlayedFlySource;

	private int AttackState;

	private float HoldTime;

	private float AirThrowTime;

	private float AirThrowCD;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	private float BodyFwdDirDot;

	[Header("Dummy Ring Object")]
	public Transform HandPoint;

	public GameObject HandItem;

	public GameObject HandItemFX;

	private GameObject ThrowItem;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip BrakeSound;

	public AudioClip TiredFlySound;

	public AudioClip TailSwipeSound;

	public AudioClip BombThrowSound;

	public AudioClip BombSnipeSound;

	[Header("Audio Sources")]
	public AudioSource flySource;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	internal int DRBState;

	internal bool DRSnipe;

	private float DRBTimer;

	internal bool FlyReleasedKey;

	private bool FlyingTired;

	private Vector2 FlyAnimation;

	private float SwipeTimer;

	internal bool UseRythmBadge;

	private float AirSwipeTimer;

	private float HurtTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Tails_Lua.c_player_name;
		PlayerNameShort = Tails_Lua.c_player_name_short;
		WalkSpeed = Tails_Lua.c_walk_speed_max;
		TopSpeed = Tails_Lua.c_run_speed_max;
		BrakeSpeed = Tails_Lua.c_brake_acc;
		GrindSpeedOrg = Tails_Lua.c_grind_speed_org;
		GrindAcc = Tails_Lua.c_grind_acc;
		GrindSpeedMax = Tails_Lua.c_grind_speed_max;
	}

	public override void SetUIGauge()
	{
		HUD.OpenGauge(Tails_Lua.OpenGauge(), Tails_Lua.c_gauge_max, Tails_Lua.c_gauge_heal, Tails_Lua.c_gauge_heal_delay);
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Tails_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Tails_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Tails_Lua.c_run_speed_max) / Tails_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Tails_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Tails_Lua.c_run_speed_max;
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
			AirMotionVelocity += Vector3.up * Tails_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Tails_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Tails_Lua.c_jump_time_min) ? 1 : 0));
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

	private void StateDummyRingBombStart()
	{
		PlayerState = State.DummyRingBomb;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (DRSnipe)
		{
			PlayAnimation("Snipe Charge", "On Snipe Charge");
			HUD.UseCrosshair();
		}
		else
		{
			Audio.PlayOneShot(BombThrowSound, Audio.volume);
			PlayerVoice.PlayRandom(6, RandomPlayChance: true);
			ThrowItem = null;
			ThrowItem = UnityEngine.Object.Instantiate(HandItem, HandPoint.position + Vector3.up * 0.3f, HandPoint.rotation);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Physics.IgnoreCollision(componentsInChildren[i], ThrowItem.GetComponent<DummyRingBomb>().Collider);
			}
			UnityEngine.Object.Instantiate(HandItemFX, ThrowItem.transform.position, ThrowItem.transform.rotation);
		}
		DRBTimer = Time.time;
		DRBState = 0;
	}

	private void StateDummyRingBomb()
	{
		PlayerState = State.DummyRingBomb;
		if (!DRSnipe)
		{
			PlayAnimation("Bomb Throw", "On Bomb Throw");
		}
		LockControls = true;
		CurSpeed = 0f;
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.zero;
		GeneralMeshRotation = base.transform.rotation;
		float num = Time.time - DRBTimer;
		if (DRBState == 0)
		{
			if (DRSnipe)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((base.transform.position - Camera.transform.position).MakePlanar()), Time.fixedDeltaTime * Tails_Lua.c_rotation_speed);
			}
			if ((DRSnipe && num > 0.4f && !Singleton<RInput>.Instance.P.GetButton("Button X")) || (!DRSnipe && num > 0.15f))
			{
				if (DRSnipe)
				{
					PlayAnimation("Snipe Throw", "On Snipe Throw");
					Audio.PlayOneShot(BombSnipeSound, Audio.volume);
					PlayerVoice.PlayRandom(6, RandomPlayChance: true);
					HUD.UseCrosshair(EndCrosshair: true);
					ThrowItem = null;
					ThrowItem = UnityEngine.Object.Instantiate(HandItem, HandPoint.position + Vector3.up * 0.3f, HandPoint.rotation);
					Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Physics.IgnoreCollision(componentsInChildren[i], ThrowItem.GetComponent<DummyRingBomb>().Collider);
					}
					ThrowItem.GetComponent<Rigidbody>().rotation = base.transform.rotation;
					UnityEngine.Object.Instantiate(HandItemFX, ThrowItem.transform.position, ThrowItem.transform.rotation);
				}
				ThrowItem.transform.GetComponent<Rigidbody>().AddForce((DRSnipe ? Camera.transform.forward : base.transform.forward) * 600f + ThrowItem.transform.up * 10f, ForceMode.Impulse);
				ThrowItem.GetComponent<DummyRingBomb>().enabled = true;
				ThrowItem.GetComponent<DummyRingBomb>().Player = base.transform;
				if (!DRSnipe)
				{
					ThrowItem.GetComponent<DummyRingBomb>().ClosestTarget = FindHomingTarget(OnlyEnemy: true);
				}
				ThrowItem = null;
				DRBTimer = Time.time;
				DRBState = 1;
			}
			else if (!DRSnipe)
			{
				ThrowItem.GetComponent<Rigidbody>().position = HandPoint.position + HandPoint.forward * -0.3f + base.transform.right * -0.05f;
				ThrowItem.GetComponent<Rigidbody>().rotation = base.transform.rotation;
				ThrowItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
		else if (Time.time - DRBTimer > 0.55f)
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateDummyRingBombEnd()
	{
		if (DRBState == 0 && (bool)ThrowItem && !ThrowItem.GetComponent<DummyRingBomb>().enabled)
		{
			UnityEngine.Object.Destroy(ThrowItem);
			ThrowItem = null;
		}
		if (DRSnipe)
		{
			HUD.UseCrosshair(EndCrosshair: true);
		}
		if (Camera.CameraState == PlayerCamera.State.FirstPerson)
		{
			Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
		}
	}

	private void StateFlyStart()
	{
		PlayerState = State.Fly;
		PlayerVoice.PlayRandom(7, RandomPlayChance: true);
		FlyingTired = false;
		FlyReleasedKey = false;
		TailsEffects.CreateFlyFX();
		FlyAnimation = Vector2.zero;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateFly()
	{
		PlayerState = State.Fly;
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		if (!FlyReleasedKey)
		{
			AirMotionVelocity.y += Tails_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		else
		{
			AirMotionVelocity.y -= Tails_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = Mathf.Clamp(AirMotionVelocity.y, -25f, 5f);
		if (HUD.ActionDisplay <= 0f && !FlyingTired)
		{
			FlyingTired = true;
			Audio.PlayOneShot(TiredFlySound, Audio.volume);
			PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		}
		AttackSphere(base.transform.position + base.transform.up * 0.675f, 0.25f, base.transform.forward * CurSpeed + base.transform.forward * 20f, 1);
		PlayAnimation("Fly", "On Fly");
		_Rigidbody.velocity = AirMotionVelocity;
		if (IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateFlyEnd()
	{
		Animator.SetTrigger("Additive Idle");
	}

	private void StateTailSwipeStart()
	{
		PlayerState = State.TailSwipe;
		Audio.PlayOneShot(TailSwipeSound, Audio.volume * 0.5f);
		PlayerVoice.PlayRandom(new int[3] { 1, 6, 7 }, RandomPlayChance: true);
		SwipeTimer = Time.time;
		TailsEffects.CreateTailSwipeFX();
		UseRythmBadge = false;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
	}

	private void StateTailSwipe()
	{
		PlayerState = State.TailSwipe;
		PlayAnimation((!UseRythmBadge) ? "Tail Swipe" : "Tail Swipe Loop", (!UseRythmBadge) ? "On Swipe" : "On Swipe Loop");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		AttackSphere(base.transform.position + base.transform.up * 0.25f, 1.3f, base.transform.forward * CurSpeed + base.transform.forward * 20f, 1);
		if (Time.time - SwipeTimer > 0.25f && Singleton<RInput>.Instance.P.GetButton("Button B") && !UseRythmBadge)
		{
			UseRythmBadge = true;
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
		if ((!UseRythmBadge && Time.time - SwipeTimer > 0.35f) || (UseRythmBadge && !Singleton<RInput>.Instance.P.GetButton("Button B")))
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateTailSwipeEnd()
	{
		UseRythmBadge = false;
	}

	private void StateAerialTailSwipeStart()
	{
		PlayerState = State.AerialTailSwipe;
		Audio.PlayOneShot(TailSwipeSound, Audio.volume * 0.5f);
		PlayerVoice.PlayRandom(new int[3] { 1, 6, 7 }, RandomPlayChance: true);
		AirSwipeTimer = Time.time;
		TailsEffects.CreateTailSwipeFX();
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
		PlayAnimation("Air Falling", "On Air Fall");
	}

	private void StateAerialTailSwipe()
	{
		PlayerState = State.AerialTailSwipe;
		PlayAnimation("Air Tail Swipe", "On Air Swipe");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (AttackSphere(base.transform.position + base.transform.up * 0.25f, 1.3f, base.transform.forward * CurSpeed + base.transform.forward * 20f, 1))
		{
			AirMotionVelocity = Vector3.up * 6f * 2f;
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
		if (Time.time - AirSwipeTimer > 0.35f)
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateAerialTailSwipeEnd()
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Tails_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Tails_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Tails_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		if (PlayerState == State.Jump || PlayerState == State.Fly)
		{
			HomingTarget = FindHomingTarget(OnlyEnemy: true);
		}
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.DummyRingBomb || PlayerState == State.TailSwipe || PlayerState == State.Hurt || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Tails_Lua.c_rotation_speed);
				AccelerationSystem(Tails_Lua.c_run_acc);
				if (PlayerState == State.Fly)
				{
					MaximumSpeed = Tails_Lua.c_flight_speed_max;
				}
				else if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Tails_Lua.c_walk_speed_max : Tails_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Tails_Lua.c_speedup_speed_max : (IsGrounded() ? Tails_Lua.c_run_speed_max : Tails_Lua.c_jump_run));
				}
			}
			else
			{
				LockControls = false;
			}
		}
		SlopePhysics(PlayerState == State.Ground || PlayerState == State.TailSwipe);
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.DummyRingBomb || PlayerState == State.TailSwipe || PlayerState == State.Hurt || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (!CanRegenFly && (IsGrounded() || LockControls))
		{
			CanRegenFly = true;
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				HoldTime = Time.time;
			}
			else if (AttackState == 1)
			{
				AttackState = 2;
			}
			if (PlayerState == State.Ground)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && CanJumpFromSink())
				{
					StateMachine.ChangeState(StateJump);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					StateMachine.ChangeState(StateTailSwipe);
				}
				if (AttackState == 2 && !IsOnWall && !IsSinking)
				{
					if (Time.time - HoldTime > 0.25f)
					{
						DRSnipe = true;
						StateMachine.ChangeState(StateDummyRingBomb);
						Camera.StateMachine.ChangeState(Camera.StateFirstPerson);
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button X"))
					{
						DRSnipe = false;
						StateMachine.ChangeState(StateDummyRingBomb);
					}
				}
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				AttackState = 1;
			}
			if (PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.SlowFall || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
			{
				if (((PlayerState == State.Jump && ReleasedKey) || PlayerState != State.Jump) && (CanAirThrow || (!CanAirThrow && Time.time - AirThrowTime > 0.3f)) && HUD.ActionDisplay != 0f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateFly);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && (CanAirThrow || (!CanAirThrow && Time.time - AirThrowTime > 0.3f)))
				{
					CanRegenFly = false;
					StateMachine.ChangeState(StateAerialTailSwipe);
				}
			}
			if (PlayerState == State.Fly)
			{
				FlyReleasedKey = FlyingTired || !Singleton<RInput>.Instance.P.GetButton("Button A");
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && (CanAirThrow || (!CanAirThrow && Time.time - AirThrowTime > 0.3f)))
				{
					CanRegenFly = false;
					StateMachine.ChangeState(StateAerialTailSwipe);
				}
			}
			if (PlayerState == State.Jump || PlayerState == State.Fly || PlayerState == State.Air || (PlayerState == State.Spring && !LockControls) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.JumpPanel && !LockControls) || (PlayerState == State.RainbowRing && !LockControls))
			{
				if (Time.time - AirThrowCD > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button X") && (CanAirThrow || (!CanAirThrow && Time.time - AirThrowTime > 0.3f)))
				{
					CanAirThrow = false;
					Animator.SetTrigger("On Air Bomb Throw");
					AirThrowTime = Time.time;
					Audio.PlayOneShot(BombThrowSound, Audio.volume);
					PlayerVoice.PlayRandom(6, RandomPlayChance: true);
					ThrowItem = UnityEngine.Object.Instantiate(HandItem, HandPoint.position, Quaternion.identity);
					Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Physics.IgnoreCollision(componentsInChildren[i], ThrowItem.GetComponent<DummyRingBomb>().Collider);
					}
					ThrowItem.transform.GetComponent<Rigidbody>().AddForce(base.transform.forward * 300f, ForceMode.Impulse);
					ThrowItem.GetComponent<DummyRingBomb>().enabled = true;
					ThrowItem.GetComponent<DummyRingBomb>().Player = base.transform;
					ThrowItem.GetComponent<DummyRingBomb>().ClosestTarget = HomingTarget;
					UnityEngine.Object.Instantiate(HandItemFX, ThrowItem.transform.position, ThrowItem.transform.rotation);
					ThrowItem = null;
				}
			}
			else
			{
				CanAirThrow = true;
				AirThrowCD = Time.time;
			}
			if (PlayerState == State.DummyRingBomb && DRSnipe && DRBState == 1 && Time.time - DRBTimer > 0.45f && Singleton<RInput>.Instance.P.GetButton("Button X"))
			{
				DRBState = 0;
				StateDummyRingBombStart();
			}
			if (PlayerState == State.TailSwipe && Time.time - SwipeTimer > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				StateMachine.ChangeState(StateJump);
			}
		}
		if (PlayerState == State.Fly || ((PlayerState == State.Ground || PlayerState == State.DashPanel || PlayerState == State.Path) && CurSpeed >= Tails_Lua.c_run_speed_max - 2f))
		{
			if (!PlayedFlySource)
			{
				PlayedFlySource = true;
				flySource.volume = 1f;
			}
			flySource.volume = Mathf.Lerp(flySource.volume, 0f, Time.deltaTime);
		}
		else
		{
			PlayedFlySource = false;
			flySource.volume = Mathf.Lerp(flySource.volume, 0f, 5f * Time.deltaTime);
		}
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump || (PlayerState == State.TailSwipe && UseRythmBadge))
		{
			return 0;
		}
		if ((PlayerState == State.TailSwipe && !UseRythmBadge) || PlayerState == State.AerialTailSwipe)
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
		FlyAnimation.x = Mathf.MoveTowards(FlyAnimation.x, FlyReleasedKey ? 0f : 1f, Time.deltaTime * 3.5f);
		FlyAnimation.y = Mathf.MoveTowards(FlyAnimation.y, (!FlyingTired) ? 0f : 1f, Time.deltaTime * 2.5f);
		Animator.SetFloat("Fly Up Animation", FlyAnimation.x);
		Animator.SetFloat("Fly Tired Animation", FlyAnimation.y);
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			float num2 = Mathf.Lerp(0f, 20f, CurSpeed / WalkSpeed);
			if (PlayerState != State.TailSwipe)
			{
				BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && (((PlayerState == State.Ground || PlayerState == State.Fly) && !LockControls && CurSpeed > 0f && !WalkSwitch) || PlayerState == State.Balancer)) ? ((0f - num) * num2) : 0f, 10f * Time.fixedDeltaTime);
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
			if (PlayerState != State.Grinding && PlayerState != State.Balancer && (PlayerState != State.DummyRingBomb || (PlayerState == State.DummyRingBomb && !DRSnipe)))
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
