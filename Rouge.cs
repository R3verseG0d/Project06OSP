using System;
using STHLua;
using UnityEngine;

public class Rouge : PlayerBase
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
		Kick = 9,
		FlyingKick = 10,
		KickDive = 11,
		Bomb = 12,
		BombScatter = 13,
		BombSnipe = 14,
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
		Lotus = 33,
		Tarzan = 34,
		Balancer = 35
	}

	[Header("Player Framework")]
	public RougeEffects RougeEffects;

	internal State PlayerState;

	internal Vector3 AirMotionVelocity;

	private Vector3 FrontNormal;

	private bool CanGlide;

	private int AttackState;

	private float CrackermineTime;

	private float HoldTime;

	private float GlideLockTime;

	private float LeftStickX;

	private float LeftStickY;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	private float BlinkTimer;

	private float BodyDirDot;

	[Header("Bomb Object")]
	public Transform HandPoint;

	public GameObject BombItem;

	public GameObject HeartBombItem;

	public GameObject CrackermineBombItem;

	private GameObject ThrowItem;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip BatBombSound;

	public AudioClip BombThrowSound;

	public AudioClip HeartBombSetSound;

	public AudioClip BombScatterSound;

	public AudioClip BrakeSound;

	public AudioClip KickDiveSound;

	[Header("Audio Sources")]
	public AudioSource[] GlideSources;

	public AudioSource ClimbSource;

	public AudioSource[] BombScatterSources;

	private bool BrakeStopped;

	private float BrakeSpd;

	private float BrakeDecelSpeed;

	private float BrakeTime;

	private float JumpTime;

	private bool ReleasedKey;

	private float GlideSpeed;

	private float GlideYGrav;

	private float GlideSpeedInc;

	private bool SetBomb;

	private float SetBombTimer;

	private float ClimbUpWait;

	private RaycastHit ClimbHit;

	private float ClimbUpTime;

	private float ClimbUpSpd;

	private bool KickDamage;

	private float KickTimer;

	private float KickSpeed;

	private int KickCount;

	private float FlyingKickTime;

	internal int KickDiveState;

	private float KickDiveLaunchTime;

	private bool DealLandDamage;

	private int BombState;

	private float BombTimer;

	private bool BombGrounded;

	internal int ScatterState;

	private int BombCount;

	private float ThrowCooldown;

	private float ScatterTimer;

	private int BombSnipeState;

	private float BombSnipeTimer;

	private float HurtTime;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		PlayerName = Rouge_Lua.c_player_name;
		PlayerNameShort = Rouge_Lua.c_player_name_short;
		WalkSpeed = Rouge_Lua.c_walk_speed_max;
		TopSpeed = Rouge_Lua.c_run_speed_max;
		BrakeSpeed = Rouge_Lua.c_brake_acc;
		GrindSpeedOrg = Rouge_Lua.c_grind_speed_org;
		GrindAcc = Rouge_Lua.c_grind_acc;
		GrindSpeedMax = Rouge_Lua.c_grind_speed_max;
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
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && CurSpeed > Rouge_Lua.c_walk_speed_max * 1.5f)
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
		if (!IsGrounded() || (CurSpeed <= Rouge_Lua.c_walk_speed_max && ShouldAlignOrFall(Align: true)))
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
		BrakeDecelSpeed = BrakeSpeed - Mathf.Min(CurSpeed, Rouge_Lua.c_run_speed_max) / Rouge_Lua.c_run_speed_max * 20f;
		if (BrakeSpd > Rouge_Lua.c_run_speed_max)
		{
			BrakeDecelSpeed += BrakeSpd - Rouge_Lua.c_run_speed_max;
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
			AirMotionVelocity += Vector3.up * Rouge_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Rouge_Lua.c_jump_speed;
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
		JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Rouge_Lua.c_jump_time_min) ? 1 : 0));
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
		Animator.SetTrigger("Additive Idle");
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
		GlideSpeedInc = Rouge_Lua.c_flight_speed_max;
		_Rigidbody.velocity = AirMotionVelocity;
		RougeEffects.CreateGlideFX();
		PlayerManager.PlayerEvents.WingFlap.enabled = true;
	}

	private void StateGlide()
	{
		PlayerState = State.Glide;
		PlayAnimation("Glide", "On Glide");
		GlideSpeedInc = Mathf.Lerp(GlideSpeedInc, Rouge_Lua.c_flight_speed_max * ((TargetDirection.magnitude != 0f && GlideSpeed >= Rouge_Lua.c_flight_speed_max) ? 1.5f : 1f), Time.fixedDeltaTime * 0.25f);
		if (TargetDirection.magnitude != 0f)
		{
			GlideSpeed += Rouge_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		else if (GlideSpeed > Rouge_Lua.c_flight_speed_min)
		{
			GlideSpeed -= Rouge_Lua.c_flight_acc * Time.fixedDeltaTime;
		}
		GlideSpeed = Mathf.Clamp(GlideSpeed, Rouge_Lua.c_flight_speed_min, GlideSpeedInc);
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
		PlayerManager.PlayerEvents.WingFlap.enabled = false;
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
		SetBomb = false;
		ClimbUpWait = Time.time;
	}

	private void StateClimb()
	{
		PlayerState = State.Climb;
		LockControls = true;
		PlayAnimation((!SetBomb) ? "Climb Move" : "Climb Bomb Set", (!SetBomb) ? "On Climb" : "On Bomb Set");
		CurSpeed = 0f;
		Physics.Raycast(base.transform.position + base.transform.up * 0.25f, base.transform.forward, out ClimbHit, 0.5f, base.FrontalCol_Mask);
		LeftStickX = ((!SetBomb) ? Mathf.MoveTowards(LeftStickX, Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (Rouge_Lua.c_climb_speed * 0.75f), Time.fixedDeltaTime * 50f) : 0f);
		LeftStickY = ((!SetBomb) ? Mathf.MoveTowards(LeftStickY, Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * (Rouge_Lua.c_climb_speed * 0.75f), Time.fixedDeltaTime * 50f) : 0f);
		Vector3 velocity = base.transform.right * LeftStickX + base.transform.up * LeftStickY;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(-base.transform.forward, ClimbHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = velocity;
		if ((_Rigidbody.velocity == Vector3.zero && ClimbSource.isPlaying) || SetBomb)
		{
			ClimbSource.Stop();
		}
		else if (_Rigidbody.velocity != Vector3.zero && !ClimbSource.isPlaying)
		{
			ClimbSource.Play();
		}
		if (SetBomb)
		{
			if (!ThrowItem && Time.time - SetBombTimer > 0.15f && Time.time - SetBombTimer < 0.2f)
			{
				Audio.PlayOneShot(BatBombSound, Audio.volume);
				ThrowItem = UnityEngine.Object.Instantiate(HeartBombItem, HandPoint.position + HandPoint.forward * -0.15f, HandPoint.rotation * Quaternion.Euler(0f, 0f, 95f));
				ThrowItem.transform.SetParent(HandPoint);
			}
			if ((bool)ThrowItem && Time.time - SetBombTimer > 0.625f)
			{
				Audio.PlayOneShot(HeartBombSetSound, Audio.volume);
				ThrowItem.GetComponent<Bomb>().enabled = true;
				ThrowItem.GetComponent<Bomb>().Player = base.transform;
				ThrowItem.GetComponent<Bomb>().AttachedObj = ClimbHit.collider.gameObject;
				ThrowItem.transform.SetParent(null);
				ThrowItem.transform.position = base.transform.position + base.transform.up * 0.565f + base.transform.forward * 0.25f + base.transform.right * 0.1f;
				ThrowItem.transform.rotation = base.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
				ThrowItem = null;
			}
			if (Time.time - SetBombTimer > 1f)
			{
				SetBomb = false;
			}
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
		SetBomb = false;
		ClimbSource.Stop();
		if ((bool)ThrowItem)
		{
			UnityEngine.Object.Destroy(ThrowItem.gameObject);
			ThrowItem = null;
		}
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

	private void StateKickStart()
	{
		PlayerState = State.Kick;
		Animator.SetInteger("Kick ID", KickCount);
		Animator.SetTrigger("On Kick");
		KickTimer = Time.time;
		KickCount++;
		KickSpeed = Rouge_Lua.c_run_speed_max * ((TargetDirection != Vector3.zero) ? 1f : 0.5f);
		KickDamage = true;
		QueuedPress = false;
		PlayerVoice.PlayRandom((KickCount == 1) ? 1 : 5, RandomPlayChance: true);
		RougeEffects.CreateKickFX(KickCount);
	}

	private void StateKick()
	{
		PlayerState = State.Kick;
		KickSpeed = Mathf.MoveTowards(KickSpeed, 0f, Time.fixedDeltaTime * 30f);
		if (KickDamage && AttackSphere(base.transform.position + base.transform.up * 0.25f, (KickCount == 1) ? 0.9f : 1.3f, base.transform.forward * CurSpeed + base.transform.forward * 5f, 1))
		{
			KickDamage = false;
		}
		CurSpeed = KickSpeed;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		if (Time.time - KickTimer > 0.4f)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded() || ShouldAlignOrFall(Align: true))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateKickEnd()
	{
	}

	private void StateFlyingKickStart()
	{
		PlayerState = State.FlyingKick;
		Animator.SetInteger("Kick ID", 2);
		Animator.SetTrigger("On Kick");
		FlyingKickTime = Time.time;
		Audio.PlayOneShot(JumpSound, Audio.volume);
		PlayerVoice.PlayRandom(6, RandomPlayChance: true);
		RougeEffects.CreateFlyingKickFX();
		CurSpeed = Rouge_Lua.c_run_speed_max * 1.5f * ((TargetDirection != Vector3.zero) ? 1f : 0.5f);
		AirMotionVelocity = _Rigidbody.velocity;
		AirMotionVelocity.y = 7.5f;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateFlyingKick()
	{
		PlayerState = State.FlyingKick;
		if (Time.time - FlyingKickTime < 0.55f)
		{
			CurSpeed = Mathf.Lerp(CurSpeed, 0f, Time.fixedDeltaTime * 5f);
		}
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (Time.time - FlyingKickTime > 0.3f)
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		if (AirMotionVelocity.y > 0f)
		{
			AttackSphere_Dir(base.transform.position + base.transform.up * 0.25f, 1.5f, 5f, 1);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
		if (Time.time - FlyingKickTime > 0.55f)
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

	private void StateFlyingKickEnd()
	{
		Animator.SetTrigger("Additive Idle");
	}

	private void StateKickDiveStart()
	{
		PlayerState = State.KickDive;
		KickDiveState = 0;
		DealLandDamage = false;
		Audio.PlayOneShot(KickDiveSound, Audio.volume);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateKickDive()
	{
		PlayerState = State.KickDive;
		LockControls = KickDiveState == 1;
		PlayAnimation((KickDiveState == 0) ? "Kick Dive" : "Kick Dive Land", (KickDiveState == 0) ? "On Kick Dive" : "On Kick Dive Land");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		if (KickDiveState == 0)
		{
			AirMotionVelocity.y = -25f;
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
			{
				KickDiveLaunchTime = Time.time;
				KickDiveState = 1;
			}
			else
			{
				AttackSphere_Dir(base.transform.position, 1f, 5f, 1);
			}
		}
		else
		{
			if (!DealLandDamage && Time.time - KickDiveLaunchTime > 0.075f)
			{
				Audio.Stop();
				RougeEffects.CreateKickDiveLandFX();
				StunSphere(base.transform.position + base.transform.forward * 0.35f + base.transform.up * -0.25f, 7f, AffectObjs: true);
				DealLandDamage = true;
			}
			CurSpeed = 0f;
			AirMotionVelocity.y = 0f;
			if (Time.time - KickDiveLaunchTime > 0.75f)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		GeneralMeshRotation = ((KickDiveState == 1) ? Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation) : (Quaternion.LookRotation(_Rigidbody.velocity.normalized) * Quaternion.Euler(-90f, 0f, 0f)));
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		DoWallNormal();
	}

	private void StateKickDiveEnd()
	{
	}

	private void StateBombStart()
	{
		PlayerState = State.Bomb;
		BombTimer = Time.time;
		BombState = 0;
		BombGrounded = IsGrounded();
		ThrowItem = UnityEngine.Object.Instantiate(BombGrounded ? BombItem : CrackermineBombItem, HandPoint.position + HandPoint.forward * -0.17f, HandPoint.rotation);
		ThrowItem.transform.SetParent(HandPoint);
		Collider[] colliders = ThrowItem.GetComponent<Bomb>().Colliders;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = colliders;
		foreach (Collider collider in array)
		{
			Collider[] array2 = componentsInChildren;
			for (int j = 0; j < array2.Length; j++)
			{
				Physics.IgnoreCollision(array2[j], collider);
			}
		}
		Audio.PlayOneShot(BombThrowSound, Audio.volume);
		PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		if (!BombGrounded)
		{
			AirMotionVelocity = _Rigidbody.velocity;
			_Rigidbody.velocity = AirMotionVelocity;
			PlayAnimation("Air Falling", "On Air Fall");
		}
		else
		{
			PlayAnimation("Movement (Blend Tree)", "On Ground");
		}
	}

	private void StateBomb()
	{
		PlayerState = State.Bomb;
		PlayAnimation(BombGrounded ? "Bomb Throw" : "Jump Bomb Throw", "On Bomb Throw");
		if (BombGrounded)
		{
			CurSpeed = 0f;
			LockControls = BombState == 1;
		}
		else if (CurSpeed > Rouge_Lua.c_walk_speed_max)
		{
			CurSpeed -= Rouge_Lua.c_walk_speed_max * Time.fixedDeltaTime * 5f;
		}
		if (!BombGrounded)
		{
			Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
			if (_Rigidbody.velocity.magnitude != 0f)
			{
				vector = base.transform.forward * CurSpeed * 0.5f;
				AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
			}
			AirMotionVelocity.y -= 12.5f * Time.fixedDeltaTime;
			AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
			_Rigidbody.velocity = AirMotionVelocity;
			if (IsGrounded() && BombState == 1)
			{
				StateMachine.ChangeState(StateGround);
			}
		}
		else
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
			_Rigidbody.velocity = Vector3.zero;
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		float num = Time.time - BombTimer;
		if (BombState == 0)
		{
			if (num > (BombGrounded ? 0.1f : 0.125f))
			{
				ThrowItem.transform.SetParent(null);
				ThrowItem.transform.forward = base.transform.forward;
				ThrowItem.transform.GetComponent<Rigidbody>().AddForce(base.transform.forward * (Rouge_Lua.c_throw_bomb * 0.35f) + ThrowItem.transform.up * 10f, ForceMode.Impulse);
				ThrowItem.GetComponent<Bomb>().enabled = true;
				ThrowItem.GetComponent<Bomb>().Player = base.transform;
				ThrowItem.GetComponent<Bomb>().ClosestTarget = HomingTarget;
				ThrowItem = null;
				BombTimer = Time.time;
				BombState = 1;
			}
			else
			{
				ThrowItem.GetComponent<Rigidbody>().position = HandPoint.position + HandPoint.forward * -0.17f;
				ThrowItem.GetComponent<Rigidbody>().rotation = base.transform.rotation;
				ThrowItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
		else if (Time.time - BombTimer > (BombGrounded ? 0.275f : 0.26f))
		{
			if (BombGrounded)
			{
				StateMachine.ChangeState(StateGround);
			}
			else
			{
				StateMachine.ChangeState(StateAir);
			}
		}
	}

	private void StateBombEnd()
	{
	}

	private void StateBombScatterStart()
	{
		PlayerState = State.BombScatter;
		Animator.ResetTrigger("On Bomb Scatter");
		PlayAnimation("Scatter Charge", "On Bomb Scatter");
		ScatterTimer = Time.time;
		ScatterState = 0;
		BombCount = 0;
		BombScatterSources[1].volume = 0f;
		BombScatterSources[0].Play();
		BombScatterSources[1].Play();
		PlayerVoice.PlayRandom(8);
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateBombScatter()
	{
		PlayerState = State.BombScatter;
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
		float num = Time.time - ScatterTimer;
		if (ScatterState == 0)
		{
			if (!BombScatterSources[0].isPlaying)
			{
				BombScatterSources[1].volume = 1f;
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button B") && num > 0.25f)
			{
				PlayAnimation("Scatter Throw", "On Bomb Scatter");
				BombScatterSources[0].Stop();
				BombScatterSources[1].Stop();
				Audio.PlayOneShot(BombScatterSound, Audio.volume);
				PlayerVoice.PlayRandom(6);
				ScatterTimer = Time.time;
				ThrowCooldown = Time.time;
				ScatterState = 1;
			}
			return;
		}
		if (BombCount < 5 && Time.time - ThrowCooldown > 0.1f)
		{
			BombCount++;
			ThrowCooldown = Time.time;
			ThrowItem = UnityEngine.Object.Instantiate(IsGrounded() ? BombItem : CrackermineBombItem, base.transform.position + base.transform.forward * 0.25f + base.transform.up * 0.45f, base.transform.rotation);
			Collider[] colliders = ThrowItem.GetComponent<Bomb>().Colliders;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			Collider[] array = colliders;
			foreach (Collider collider in array)
			{
				Collider[] array2 = componentsInChildren;
				for (int j = 0; j < array2.Length; j++)
				{
					Physics.IgnoreCollision(array2[j], collider);
				}
			}
			ThrowItem.transform.forward = base.transform.forward;
			ThrowItem.transform.GetComponent<Rigidbody>().AddForce(base.transform.forward * (Rouge_Lua.c_throw_bomb * 0.25f) + ThrowItem.transform.up * 10f, ForceMode.Impulse);
			ThrowItem.GetComponent<Bomb>().enabled = true;
			ThrowItem.GetComponent<Bomb>().Player = base.transform;
			ThrowItem.GetComponent<Bomb>().ClosestTarget = FindHomingTarget(OnlyEnemy: true);
			ThrowItem = null;
		}
		if (Time.time - ScatterTimer > 0.75f)
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateBombScatterEnd()
	{
		BombScatterSources[0].Stop();
		BombScatterSources[1].Stop();
	}

	private void StateBombSnipeStart()
	{
		PlayerState = State.BombSnipe;
		PlayAnimation("Snipe Charge", "On Snipe Charge");
		HUD.UseCrosshair();
		BombSnipeTimer = Time.time;
		BombSnipeState = 0;
		BombScatterSources[1].volume = 0f;
		BombScatterSources[0].Play();
		BombScatterSources[1].Play();
		Audio.PlayOneShot(BatBombSound, Audio.volume);
		ThrowItem = UnityEngine.Object.Instantiate(BombItem, HandPoint.position + HandPoint.forward * -0.17f, HandPoint.rotation);
		ThrowItem.transform.SetParent(HandPoint);
	}

	private void StateBombSnipe()
	{
		PlayerState = State.BombSnipe;
		LockControls = true;
		CurSpeed = 0f;
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.zero;
		GeneralMeshRotation = base.transform.rotation;
		float num = Time.time - BombSnipeTimer;
		if (BombSnipeState == 0)
		{
			if (!BombScatterSources[0].isPlaying)
			{
				BombScatterSources[1].volume = 1f;
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((base.transform.position - Camera.transform.position).MakePlanar()), Time.fixedDeltaTime * Rouge_Lua.c_rotation_speed);
			if (num > 0.5f && !Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				PlayAnimation("Snipe Throw", "On Snipe Throw");
				HUD.UseCrosshair(EndCrosshair: true);
				Audio.PlayOneShot(BombThrowSound, Audio.volume);
				PlayerVoice.PlayRandom(5, RandomPlayChance: true);
				BombScatterSources[0].Stop();
				BombScatterSources[1].Stop();
				ThrowItem.GetComponent<Rigidbody>().isKinematic = false;
				ThrowItem.transform.SetParent(null);
				ThrowItem.transform.forward = base.transform.forward;
				ThrowItem.transform.GetComponent<Rigidbody>().AddForce(Camera.transform.forward * (Rouge_Lua.c_throw_bomb * 0.75f) + ThrowItem.transform.up * 17.5f, ForceMode.Impulse);
				ThrowItem.GetComponent<Bomb>().enabled = true;
				ThrowItem.GetComponent<Bomb>().Player = base.transform;
				ThrowItem = null;
				BombSnipeTimer = Time.time;
				BombSnipeState = 1;
			}
			else
			{
				ThrowItem.GetComponent<Rigidbody>().position = HandPoint.position + HandPoint.forward * -0.17f;
				ThrowItem.GetComponent<Rigidbody>().rotation = base.transform.rotation;
				ThrowItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
				ThrowItem.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		else if (Time.time - BombSnipeTimer > 0.325f)
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StateBombSnipeEnd()
	{
		if (BombSnipeState == 0 && (bool)ThrowItem && !ThrowItem.GetComponent<Bomb>().enabled)
		{
			UnityEngine.Object.Destroy(ThrowItem);
			ThrowItem = null;
		}
		HUD.UseCrosshair(EndCrosshair: true);
		BombScatterSources[0].Stop();
		BombScatterSources[1].Stop();
		if (Camera.CameraState == PlayerCamera.State.FirstPerson)
		{
			Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
		}
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
		CurSpeed = ((Time.time - HurtTime < 0.325f) ? (Rouge_Lua.c_damage_speed * 8f) : 0f);
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		PlayAnimation((Time.time - HurtTime > Rouge_Lua.c_damage_time) ? "Hurt Get Up" : "Hurt", (Time.time - HurtTime > Rouge_Lua.c_damage_time) ? "On Hurt Get Up" : "On Hurt");
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
		if (PlayerState == State.Ground || PlayerState == State.Jump || PlayerState == State.Glide)
		{
			HomingTarget = FindHomingTarget(OnlyEnemy: true);
		}
		FrontNormal = FrontalHit.normal;
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Kick || PlayerState == State.KickDive || PlayerState == State.Bomb || PlayerState == State.BombSnipe || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event)
		{
			if (!LockControls)
			{
				RotatePlayer(Rouge_Lua.c_rotation_speed, Override: false, PlayerState == State.Glide);
				if (PlayerState != State.Glide)
				{
					AccelerationSystem((!HasSpeedUp) ? Rouge_Lua.c_run_acc : Rouge_Lua.c_speedup_acc);
				}
				if (WalkSwitch)
				{
					MaximumSpeed = (IsGrounded() ? Rouge_Lua.c_walk_speed_max : Rouge_Lua.c_jump_walk);
				}
				else
				{
					MaximumSpeed = (HasSpeedUp ? Rouge_Lua.c_speedup_speed_max : (IsGrounded() ? Rouge_Lua.c_run_speed_max : Rouge_Lua.c_jump_run));
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
		if (PlayerState == State.Ground || PlayerState == State.Brake || PlayerState == State.Kick || PlayerState == State.KickDive || PlayerState == State.Bomb || PlayerState == State.BombSnipe || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && StageManager.StageState != StageManager.State.Event && !IsDead && PlayerState != State.Talk)
		{
			if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
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
				if (AttackState == 2 && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					if (Time.time - HoldTime > 0.25f)
					{
						StateMachine.ChangeState(StateBombSnipe);
						Camera.StateMachine.ChangeState(Camera.StateFirstPerson);
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
					{
						StateMachine.ChangeState(StateBomb);
					}
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && ShouldAlignOrFall(Align: false) && !IsSinking)
				{
					KickCount = 0;
					StateMachine.ChangeState(StateKick);
				}
			}
			if (PlayerState == State.Jump || PlayerState == State.Air || PlayerState == State.SlowFall || PlayerState == State.BallFall || (PlayerState == State.FlyingKick && Time.time - FlyingKickTime > 0.55f) || (PlayerState == State.WideSpring && !LockControls) || (PlayerState == State.RainbowRing && !LockControls) || (PlayerState == State.Lotus && !LockControls) || (PlayerState == State.Tarzan && !LockControls))
			{
				if (((PlayerState == State.Jump && ReleasedKey && Singleton<RInput>.Instance.P.GetButtonDown("Button A")) || (PlayerState != State.Jump && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))) && (CanGlide || (!CanGlide && Time.time - GlideLockTime > 0.3f)))
				{
					CanGlide = false;
					GlideLockTime = Time.time;
					StateMachine.ChangeState(StateGlide);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateKickDive);
				}
				if (AttackState == 2)
				{
					if (Time.time - HoldTime > 0.25f)
					{
						StateMachine.ChangeState(StateBombScatter);
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
					{
						StateMachine.ChangeState(StateBomb);
					}
				}
			}
			if (PlayerState == State.Glide)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateKickDive);
				}
				if (AttackState == 2)
				{
					if (Time.time - HoldTime > 0.25f)
					{
						StateMachine.ChangeState(StateBombScatter);
					}
					else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
					{
						StateMachine.ChangeState(StateBomb);
					}
				}
			}
			if (PlayerState == State.Bomb && BombState == 1)
			{
				if (Time.time - BombTimer > 0.225f && TargetDirection != Vector3.zero)
				{
					StateMachine.ChangeState(StateGround);
				}
				if (Time.time - BombTimer > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					StateMachine.ChangeState(StateBomb);
				}
			}
			if (PlayerState == State.BombSnipe && BombSnipeState == 1 && Time.time - BombSnipeTimer > 0.235f && Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				BombSnipeState = 0;
				StateBombSnipeStart();
			}
			if (PlayerState == State.Climb)
			{
				if (!SetBomb && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					if (Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") < -0f)
					{
						base.transform.forward = -base.transform.forward;
					}
					CurSpeed = Rouge_Lua.c_run_speed_max / 1.5f;
					StateMachine.ChangeState(StateJump);
				}
				if (!SetBomb && Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					SetBomb = true;
					SetBombTimer = Time.time;
					PlayerVoice.PlayRandom(7, RandomPlayChance: true);
				}
			}
			if (PlayerState == State.ClimbUp && Time.time - ClimbUpTime > 0.25f)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateGlide);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					StateMachine.ChangeState(StateKickDive);
				}
			}
			if (PlayerState == State.Kick)
			{
				if (!QueuedPress && OnButtonPressed("Button X"))
				{
					QueuedPress = Time.time - KickTimer > 0.1f;
				}
				if (Time.time - KickTimer > ((KickCount < 2) ? 0.2f : 0.3f) && ShouldAlignOrFall(Align: false) && KickCount < 3)
				{
					if (KickCount < 2 && OnCombo("Button X"))
					{
						StateMachine.ChangeState(StateKick);
					}
					else if (KickCount > 1 && OnCombo("Button X"))
					{
						StateMachine.ChangeState(StateFlyingKick);
					}
				}
				if (Time.time - KickTimer > 0.1f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					StateMachine.ChangeState(StateJump);
				}
			}
			else if (KickCount != 0)
			{
				KickCount = 0;
			}
			if (PlayerState == State.KickDive)
			{
				if (KickDiveState == 1)
				{
					if (Time.time - KickDiveLaunchTime > 0.225f && TargetDirection != Vector3.zero)
					{
						StateMachine.ChangeState(StateGround);
					}
					if (Time.time - KickDiveLaunchTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
					{
						StateMachine.ChangeState(StateJump);
					}
					if (Time.time - KickDiveLaunchTime > 0.175f && Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
					{
						KickCount = 0;
						StateMachine.ChangeState(StateKick);
					}
				}
				else
				{
					if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && (CanGlide || (!CanGlide && Time.time - GlideLockTime > 0.3f)))
					{
						CanGlide = false;
						GlideLockTime = Time.time;
						StateMachine.ChangeState(StateGlide);
					}
					if (AttackState == 2)
					{
						if (Time.time - HoldTime > 0.25f)
						{
							StateMachine.ChangeState(StateBombScatter);
						}
						else if (Singleton<RInput>.Instance.P.GetButtonUp("Button B"))
						{
							StateMachine.ChangeState(StateBomb);
						}
					}
				}
			}
			if (!Singleton<RInput>.Instance.P.GetButton("Button B"))
			{
				AttackState = 1;
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
		if (PlayerState == State.Kick || PlayerState == State.FlyingKick || PlayerState == State.KickDive)
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

	private void CanClimb(bool CannotAttach = false)
	{
		if (!CannotAttach && FrontalCollision && FrontalHit.transform.tag == "ClimbableWall" && FrontNormal != Vector3.zero && 0.75f > FrontNormal.y && -0.75f < FrontNormal.y)
		{
			StateMachine.ChangeState(StateClimb);
		}
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
			if (PlayerState != State.Grinding && PlayerState != State.Balancer && PlayerState != State.BombSnipe)
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
}
