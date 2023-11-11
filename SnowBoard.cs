using System;
using STHLua;
using UnityEngine;

public class SnowBoard : PlayerBase
{
	public enum State
	{
		Ground = 0,
		Jump = 1,
		SlowFall = 2,
		Hurt = 3,
		HurtBoard = 4,
		RampJump = 5,
		Grinding = 6,
		Death = 7,
		FallDeath = 8,
		DrownDeath = 9,
		SnowBallDeath = 10,
		TornadoDeath = 11,
		Talk = 12,
		Path = 13,
		WarpHole = 14,
		Result = 15,
		Cutscene = 16,
		DashPanel = 17
	}

	public enum Type
	{
		Metal = 0,
		Snow = 1
	}

	[Header("Player Framework")]
	public SnowBoardEffects SnowBoardEffects;

	public Transform BoardPoint;

	public GameObject MetalBoardObj;

	public GameObject SnowBoardObj;

	internal Type BoardType;

	internal GameObject Board;

	internal State PlayerState;

	internal bool IsOnRamp;

	private bool Dash;

	private bool EndDash;

	private bool IsLookBack;

	private float DashTime;

	private float LookBackTime;

	private float LookBackAnim;

	private float JumpWalk;

	private float JumpRun;

	private float BaseJump;

	private float GrindAnim;

	[Header("Player Models")]
	public SkinnedMeshRenderer[] PlayerRenderers;

	public UpgradeModels Upgrades;

	private float BodyDirDot;

	private float BlinkTimer;

	[Header("-----------------------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Sounds")]
	public AudioClip JumpSound;

	public AudioClip[] WindSounds;

	public AudioClip PerfectRampSound;

	[Header("Audio Sources")]
	public AudioSource[] MetalSources;

	public AudioSource[] SnowSources;

	public AudioSource AirSource;

	private Vector3 Direction;

	private Vector3 Speed;

	private float SteeringSpeed;

	private Vector3 GroundAcc;

	private Vector3 GroundForward;

	private Vector3 GroundRight;

	private Vector3 v_grav = new Vector3(0f, -0.6f, 0f);

	private float BoardSpeed_Forward;

	private float BoardSpeed_Side;

	private float BoardFriction_Forward = 0.0004f;

	private float BoardFriction_Side = 0.13f;

	private float BoardDecel = 5f;

	private Vector3 AirMotion;

	private float JumpTime;

	private float JumpStrenght;

	private float JumpMaxSpeed;

	private float HurtTime;

	private float HurtBoardTime;

	private Vector3 RampVelocity;

	private float RampTime;

	private float RampRate;

	private float RampBPower_Rate;

	private float RampJumpTime;

	public Vector3 SafeDirection
	{
		get
		{
			Vector3 normalized = TargetDirection.normalized;
			if (normalized == Vector3.zero)
			{
				normalized = base.transform.forward.normalized;
			}
			if (normalized == Vector3.zero)
			{
				normalized = _Rigidbody.velocity.normalized;
			}
			return normalized;
		}
	}

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
		StageManager.Stage stage = UnityEngine.Object.FindObjectOfType<StageManager>()._Stage;
		if (stage == StageManager.Stage.wap)
		{
			PlayerName = Snow_Board_Wap_Lua.c_player_name;
			PlayerNameShort = Snow_Board_Wap_Lua.c_player_name_short;
			WalkSpeed = Snow_Board_Wap_Lua.c_min_speed;
			TopSpeed = Snow_Board_Wap_Lua.c_max_speed;
			GrindSpeedOrg = Snow_Board_Wap_Lua.c_grind_speed_org;
			GrindAcc = Snow_Board_Wap_Lua.c_grind_acc;
			GrindSpeedMax = Snow_Board_Wap_Lua.c_grind_speed_max;
			JumpWalk = Snow_Board_Wap_Lua.c_jump_walk;
			JumpRun = Snow_Board_Wap_Lua.c_jump_run;
			BaseJump = Snow_Board_Wap_Lua.l_base_jump;
		}
		else
		{
			PlayerName = Snow_Board_Lua.c_player_name;
			PlayerNameShort = Snow_Board_Lua.c_player_name_short;
			WalkSpeed = Snow_Board_Lua.c_min_speed;
			TopSpeed = Snow_Board_Lua.c_max_speed;
			GrindSpeedOrg = Snow_Board_Lua.c_grind_speed_org;
			GrindAcc = Snow_Board_Lua.c_grind_acc;
			GrindSpeedMax = Snow_Board_Lua.c_grind_speed_max;
			JumpWalk = Snow_Board_Lua.c_jump_walk;
			JumpRun = Snow_Board_Lua.c_jump_run;
			BaseJump = Snow_Board_Lua.l_base_jump;
		}
	}

	public override void SetUIGauge()
	{
		HUD.CloseGauge();
	}

	private void StateGroundStart()
	{
		PlayerState = State.Ground;
		MaxRayLenght = 0.75f;
		JumpStrenght = JumpWalk;
		BoardFriction_Forward = 0.0004f;
		BoardDecel = 5f;
		Speed = _Rigidbody.velocity * 0.95f;
		_Rigidbody.velocity = Speed;
		Direction = base.transform.forward;
		Direction.y = 0f;
	}

	private void StateGround()
	{
		PlayerState = State.Ground;
		if (!Dash)
		{
			PlayAnimation("Board", "On Board");
		}
		else
		{
			PlayAnimation("Dash", "On Dash");
			if (Time.time - DashTime > 0.3f && !EndDash)
			{
				PlayerVoice.PlayRandom(5, RandomPlayChance: true);
				if (Speed.magnitude < TopSpeed * 1.15f)
				{
					float num = Mathf.Lerp(0.75f, 0f, Speed.magnitude / TopSpeed);
					Speed += base.transform.forward * TopSpeed * num;
				}
				PlayerManager.PlayerEvents.CreateLandFXAndSound();
				EndDash = true;
			}
			if (Time.time - DashTime > 0.8f)
			{
				Dash = false;
				EndDash = false;
			}
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		float num2 = Mathf.Lerp(10f, 20f, _Rigidbody.velocity.magnitude / TopSpeed);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation, Time.deltaTime * num2);
		GroundRight = base.transform.right;
		GroundForward = Vector3.Cross(GroundRight, base.transform.up);
		SteeringSpeed = BoardSpeed_Forward / 35f;
		SteeringSpeed = Mathf.Clamp(SteeringSpeed, 0.25f, SteeringSpeed);
		Direction = (GroundForward + TargetDirection * SteeringSpeed).normalized;
		Direction.y = 0f;
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Direction, Vector3.up + TargetDirection * (0.25f + Speed.magnitude / 30f)), Time.fixedDeltaTime * BoardDecel);
		GroundAcc = Vector3.Dot(GroundForward, v_grav) * GroundForward + Vector3.Dot(GroundRight, v_grav) * GroundRight;
		if (Speed.magnitude < TopSpeed)
		{
			Speed += GroundAcc;
		}
		else
		{
			Speed -= GroundAcc;
		}
		BoardSpeed_Forward = Vector3.Dot(GroundForward, Speed);
		BoardSpeed_Side = Vector3.Dot(GroundRight, Speed);
		BoardSpeed_Forward *= 1f - BoardFriction_Forward;
		BoardSpeed_Side *= 1f - BoardFriction_Side;
		Speed = GroundForward * BoardSpeed_Forward + GroundRight * BoardSpeed_Side;
		_Rigidbody.velocity = Speed;
		if (FrontalCollision)
		{
			if (Speed.magnitude > 16f)
			{
				StateMachine.ChangeState(StateHurt);
			}
			else
			{
				Speed = -Speed.normalized * 2.5f;
			}
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateJump);
		}
	}

	private void StateGroundEnd()
	{
		Animator.SetTrigger("Additive Idle");
		Animator.ResetTrigger("Additive Idle");
		BoardFriction_Forward = 0.0004f;
		BoardDecel = 5f;
	}

	private void StateJumpStart()
	{
		PlayerState = State.Jump;
		JumpTime = Time.time;
		Direction = base.transform.forward;
		Speed = _Rigidbody.velocity;
		_Rigidbody.velocity = Speed;
		JumpMaxSpeed = TopSpeed;
	}

	private void StateJump()
	{
		PlayerState = State.Jump;
		PlayAnimation((JumpStrenght < ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? "Jump Small" : "Jump High", (JumpStrenght < ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? "On Jump" : "On Jump High");
		MaxRayLenght = ((Time.time - JumpTime > 0.25f) ? 0.75f : 0f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation, Time.deltaTime * 10f);
		Direction = Vector3.Slerp(Direction, SafeDirection, Time.fixedDeltaTime * 3f);
		base.transform.rotation = Quaternion.LookRotation(Direction.MakePlanar());
		JumpMaxSpeed = Mathf.Lerp(JumpMaxSpeed, JumpRun, Time.fixedDeltaTime);
		Speed += TargetDirection * Time.fixedDeltaTime * 5f;
		Speed.y -= 25f * Time.fixedDeltaTime;
		Speed.y = LimitVel(Speed.y);
		Speed.x *= 0.999f;
		Speed.x = Mathf.Clamp(Speed.x, 0f - JumpMaxSpeed, JumpMaxSpeed);
		Speed.z *= 0.999f;
		Speed.z = Mathf.Clamp(Speed.z, 0f - JumpMaxSpeed, JumpMaxSpeed);
		_Rigidbody.velocity = Speed;
		if (JumpAttackSphere(base.transform.position, 0.5f, base.transform.forward * _Rigidbody.velocity.magnitude, 1))
		{
			JumpTime = Time.time;
			Speed.y = 12f;
			Speed.x *= 0.75f;
			Speed.z *= 0.75f;
		}
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			Animator.SetTrigger((JumpStrenght < 12.5f) ? "On Land" : "On Land High");
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateJumpEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateHurtStart()
	{
		PlayerState = State.Hurt;
		HurtTime = Time.time;
		PlayerVoice.PlayRandom(4);
		Speed = _Rigidbody.velocity * -0.25f;
		_Rigidbody.velocity = Speed;
	}

	private void StateHurt()
	{
		PlayerState = State.Hurt;
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		float num = Mathf.Lerp(10f, 20f, _Rigidbody.velocity.magnitude / TopSpeed);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation, Time.deltaTime * num);
		GroundRight = base.transform.right;
		GroundForward = Vector3.Cross(GroundRight, base.transform.up);
		GroundAcc = Vector3.Dot(GroundForward, v_grav) * GroundForward + Vector3.Dot(GroundRight, v_grav) * GroundRight;
		if (Speed.magnitude > TopSpeed * 0.5f)
		{
			Speed -= GroundAcc;
		}
		BoardSpeed_Forward = Vector3.Dot(GroundForward, Speed);
		BoardSpeed_Side = Vector3.Dot(GroundRight, Speed);
		BoardSpeed_Forward *= 1f - BoardFriction_Forward;
		BoardSpeed_Side *= 1f - BoardFriction_Side;
		Speed = GroundForward * BoardSpeed_Forward + GroundRight * BoardSpeed_Side;
		_Rigidbody.velocity = Speed;
		PlayAnimation("Hurt", "On Hurt");
		if (Time.time - HurtTime > 0.65f)
		{
			StateMachine.ChangeState(StateGround);
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
		else if (!IsGrounded())
		{
			StateMachine.ChangeState(StateJump);
		}
	}

	private void StateHurtEnd()
	{
	}

	private void StateHurtBoardStart()
	{
		PlayerState = State.HurtBoard;
		HurtTime = Time.time;
		PlayerVoice.PlayRandom(4);
		Speed = _Rigidbody.velocity * 0.95f;
		_Rigidbody.velocity = Speed;
	}

	private void StateHurtBoard()
	{
		PlayerState = State.HurtBoard;
		PlayAnimation("Hurt Board", "On Hurt Board");
		LockControls = true;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		float num = Mathf.Lerp(10f, 20f, _Rigidbody.velocity.magnitude / TopSpeed);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation, Time.deltaTime * num);
		GroundRight = base.transform.right;
		GroundForward = Vector3.Cross(GroundRight, base.transform.up);
		GroundAcc = Vector3.Dot(GroundForward, v_grav) * GroundForward + Vector3.Dot(GroundRight, v_grav) * GroundRight;
		if (Speed.magnitude > TopSpeed * 0.5f)
		{
			Speed -= GroundAcc;
		}
		BoardSpeed_Forward = Vector3.Dot(GroundForward, Speed);
		BoardSpeed_Side = Vector3.Dot(GroundRight, Speed);
		BoardSpeed_Forward *= 1f - BoardFriction_Forward;
		BoardSpeed_Side *= 1f - BoardFriction_Side;
		Speed = GroundForward * BoardSpeed_Forward + GroundRight * BoardSpeed_Side;
		_Rigidbody.velocity = Speed;
		if (Time.time - HurtTime > 0.9f)
		{
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateJump);
		}
	}

	private void StateHurtBoardEnd()
	{
	}

	private void StateRampJumpStart()
	{
		PlayerState = State.RampJump;
		MaxRayLenght = 0.75f;
		RampJumpTime = Time.time;
		Speed = RampVelocity;
		_Rigidbody.velocity = Speed;
		base.transform.forward = Speed.MakePlanar();
		IsOnRamp = false;
		Audio.PlayOneShot(WindSounds[UnityEngine.Random.Range(0, WindSounds.Length)], Audio.volume);
		AddScore((JumpStrenght > ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? 187 : 300, InstantChain: true, (JumpStrenght > ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? 8 : 2);
		if (JumpStrenght > ((BoardType == Type.Metal) ? 12.5f : 9.5f))
		{
			SnowBoardEffects.CreatePerfectRampFX();
			Audio.PlayOneShot(PerfectRampSound, Audio.volume);
			PlayerVoice.PlayRandom(5, RandomPlayChance: true);
		}
		else
		{
			PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		}
	}

	private void StateRampJump()
	{
		PlayerState = State.RampJump;
		PlayAnimation((JumpStrenght < ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? "Jump High" : "Trick", (JumpStrenght < ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? "On Jump High" : "On Trick");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation, Time.deltaTime * 10f);
		if (Time.time - RampJumpTime < RampTime)
		{
			Speed = RampVelocity;
		}
		else
		{
			Speed.y -= 25f * Time.fixedDeltaTime;
			Speed.y = LimitVel(Speed.y);
			Speed.x = Mathf.Lerp(Speed.x, Speed.x * ((JumpStrenght > ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? RampBPower_Rate : RampRate), Time.fixedDeltaTime);
			Speed.z = Mathf.Lerp(Speed.z, Speed.z * ((JumpStrenght > ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? RampBPower_Rate : RampRate), Time.fixedDeltaTime);
		}
		Speed += base.transform.right * Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * 0.15f;
		_Rigidbody.velocity = Speed;
		DoWallNormal();
		if (Time.time - RampJumpTime > 0.1f && IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			Animator.SetTrigger((JumpStrenght < ((BoardType == Type.Metal) ? 12.5f : 9.5f)) ? "On Land" : "On Land High");
			StateMachine.ChangeState(StateGround);
			base.transform.forward = _Rigidbody.velocity.normalized.MakePlanar();
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateRampJumpEnd()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		UpdateMesh();
		if (PlayerState == State.Ground || PlayerState == State.Hurt || PlayerState == State.HurtBoard || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.eulerAngles.y : 1f));
		Vector3 vector = quaternion * Vector3.forward;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = quaternion * Vector3.right;
		vector2.y = 0f;
		vector2.Normalize();
		TargetDirection = (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * vector2 + Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * vector).normalized;
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		UpdateSound();
		if (PlayerState == State.Ground || PlayerState == State.Hurt || PlayerState == State.HurtBoard || PlayerState == State.Death || PlayerState == State.FallDeath || PlayerState == State.DrownDeath || PlayerState == State.Talk || PlayerState == State.DashPanel)
		{
			PositionToPoint();
		}
		if (Singleton<GameManager>.Instance.GameState == GameManager.State.Paused || Singleton<GameManager>.Instance.GameState == GameManager.State.Result || StageManager.StageState == StageManager.State.Event || IsDead || PlayerState == State.Talk)
		{
			return;
		}
		if (PlayerState == State.Ground)
		{
			if (Singleton<RInput>.Instance.P.GetButton("Button A"))
			{
				BoardFriction_Forward = 0.0002f;
				BoardDecel = 3f;
				JumpStrenght = Mathf.Min(JumpStrenght + BaseJump, JumpRun);
			}
			if (Singleton<RInput>.Instance.P.GetButtonUp("Button A"))
			{
				Audio.PlayOneShot(JumpSound, Audio.volume);
				if (!IsOnRamp)
				{
					PlayerVoice.PlayRandom(1, RandomPlayChance: true);
					_Rigidbody.velocity = (2f * WorldVelocity + _Rigidbody.velocity).normalized * WorldVelocity.magnitude;
					_Rigidbody.velocity += base.transform.up * JumpStrenght;
					StateMachine.ChangeState(StateJump);
				}
				else
				{
					StateMachine.ChangeState(StateRampJump);
				}
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && Time.time - DashTime > 1f && !Dash)
			{
				DashTime = Time.time;
				Dash = true;
			}
		}
		if (PlayerState != State.Grinding)
		{
			return;
		}
		if (Singleton<RInput>.Instance.P.GetButton("Button A"))
		{
			BoardFriction_Forward = 0.0002f;
			BoardDecel = 3f;
			JumpStrenght = Mathf.Min(JumpStrenght + BaseJump, JumpRun);
		}
		if (Singleton<RInput>.Instance.P.GetButtonUp("Button A"))
		{
			Audio.PlayOneShot(JumpSound, Audio.volume);
			if (!IsOnRamp)
			{
				PlayerVoice.PlayRandom(1, RandomPlayChance: true);
				_Rigidbody.velocity = base.transform.forward * CurSpeed;
				_Rigidbody.velocity += base.transform.up * JumpStrenght * 2f;
				StateMachine.ChangeState(StateJump);
			}
			else
			{
				StateMachine.ChangeState(StateRampJump);
			}
		}
	}

	public void ManageBoard(bool AddBoard)
	{
		if (AddBoard && !Board)
		{
			StageManager.Stage stage = StageManager._Stage;
			if (stage == StageManager.Stage.wap)
			{
				BoardType = Type.Snow;
			}
			else
			{
				BoardType = Type.Metal;
			}
			Board = UnityEngine.Object.Instantiate((BoardType == Type.Metal) ? MetalBoardObj : SnowBoardObj, BoardPoint.position + BoardPoint.forward * 0.025f, BoardPoint.rotation);
			Board.GetComponent<BoardFX>().PM = PlayerManager;
			Board.transform.SetParent(BoardPoint);
		}
		else if (!AddBoard && (bool)Board)
		{
			Board.transform.SetParent(null);
			Board.transform.GetComponent<Collider>().enabled = true;
			Board.transform.GetComponent<Rigidbody>().isKinematic = false;
			Board.transform.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			Board.transform.GetComponent<Rigidbody>().velocity = _Rigidbody.velocity * 1.1f;
			Board = null;
		}
	}

	public void LookBack()
	{
		IsLookBack = true;
		LookBackTime = Time.time;
	}

	public override int AttackLevel()
	{
		if (PlayerState == State.Jump)
		{
			return 0;
		}
		return -1;
	}

	public override bool IsInvulnerable(int HurtType)
	{
		bool result = AttackLevel() >= HurtType;
		if (PlayerState == State.Hurt || PlayerState == State.Talk || PlayerState == State.WarpHole || PlayerState == State.HurtBoard || PlayerState == State.Result || PlayerState == State.Cutscene || HasInvincibility || IsDead)
		{
			return true;
		}
		return result;
	}

	public override void UpdateAnimations()
	{
		base.UpdateAnimations();
		if (IsLookBack && Time.time - LookBackTime > 2.5f)
		{
			IsLookBack = false;
		}
		LookBackAnim = Mathf.Lerp(LookBackAnim, IsLookBack ? 1f : 0f, Time.deltaTime * 2f);
		GrindAnim = Mathf.MoveTowards(GrindAnim, (((PlayerState == State.Ground && !Dash) || PlayerState == State.Grinding) && Singleton<RInput>.Instance.P.GetButton("Button A")) ? 1f : 0f, Time.deltaTime * 10f);
		Animator.SetFloat("Steering", Mathf.Lerp(Animator.GetFloat("Steering"), (!Dash) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, 3f * Time.deltaTime));
		Animator.SetFloat("Look Back Anim", LookBackAnim);
		Animator.SetFloat("Grind Anim", GrindAnim);
	}

	private void UpdateMesh()
	{
		if (PlayerState != State.WarpHole)
		{
			float num = Vector3.Dot(TargetDirection.normalized, base.transform.right.normalized);
			BodyDirDot = Mathf.Lerp(BodyDirDot, (UseCharacterSway && PlayerState == State.Ground && !Dash) ? ((0f - num) * Mathf.Min(Speed.magnitude, TopSpeed) * 1.25f) : 0f, 4f * Time.fixedDeltaTime);
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

	private void UpdateSound()
	{
		if (BoardType == Type.Metal)
		{
			MetalSources[0].volume = ((PlayerState == State.Ground) ? Mathf.Lerp(MetalSources[0].volume, (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") != 0f || Singleton<RInput>.Instance.P.GetButton("Button A")) ? 0f : Mathf.Lerp(0.1f, 0.7f, Mathf.Clamp(_Rigidbody.velocity.magnitude, _Rigidbody.velocity.magnitude, TopSpeed) / TopSpeed), Time.deltaTime * 5f) : 0f);
			MetalSources[1].volume = ((PlayerState == State.Ground) ? Mathf.Lerp(MetalSources[1].volume, (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") != 0f && !Singleton<RInput>.Instance.P.GetButton("Button A")) ? Mathf.Lerp(0.1f, 0.7f, Mathf.Clamp(_Rigidbody.velocity.magnitude, _Rigidbody.velocity.magnitude, TopSpeed) / TopSpeed) : 0f, Time.deltaTime * 5f) : 0f);
			MetalSources[2].volume = ((PlayerState == State.Ground) ? Mathf.Lerp(MetalSources[2].volume, Singleton<RInput>.Instance.P.GetButton("Button A") ? Mathf.Lerp(0.1f, 0.7f, Mathf.Clamp(_Rigidbody.velocity.magnitude, _Rigidbody.velocity.magnitude, TopSpeed) / TopSpeed) : 0f, Time.deltaTime * 5f) : 0f);
		}
		else
		{
			SnowSources[0].volume = ((PlayerState == State.Ground) ? Mathf.Lerp(SnowSources[0].volume, (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") != 0f) ? 0f : Mathf.Lerp(0.1f, 0.9f, Mathf.Clamp(_Rigidbody.velocity.magnitude, _Rigidbody.velocity.magnitude, TopSpeed) / TopSpeed), Time.deltaTime * 5f) : 0f);
			SnowSources[1].volume = ((PlayerState == State.Ground) ? Mathf.Lerp(SnowSources[1].volume, (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") != 0f) ? Mathf.Lerp(0.1f, 0.75f, Mathf.Clamp(_Rigidbody.velocity.magnitude, _Rigidbody.velocity.magnitude, TopSpeed) / TopSpeed) : 0f, Time.deltaTime * 5f) : 0f);
		}
		AirSource.volume = Mathf.Lerp(AirSource.volume, (PlayerState == State.Jump || PlayerState == State.RampJump) ? 1f : 0f, Time.deltaTime * 2f);
	}

	public void OnRampEnter(Vector3 _RampVelocity, float _RampTime, float _RampRate, float _RampBPower_Rate)
	{
		RampVelocity = _RampVelocity;
		RampTime = _RampTime;
		RampRate = _RampRate;
		RampBPower_Rate = _RampBPower_Rate;
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
			if (PlayerState != State.Grinding)
			{
				if (IsGrounded() && ShouldAlignOrFall(Align: false))
				{
					StateMachine.ChangeState(StateHurtBoard);
				}
				else
				{
					Vector3 velocity = _Rigidbody.velocity;
					velocity.y = 0f;
					_Rigidbody.velocity = velocity;
					StateMachine.ChangeState(StateJump);
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
			ManageBoard(AddBoard: false);
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
		case "StateAir":
			StateMachine.ChangeState(StateJump);
			break;
		case "StateHurt":
			StateMachine.ChangeState(StateHurt);
			break;
		}
	}

	public override void StartPlayer(bool TalkState = false)
	{
		ManageBoard(AddBoard: true);
		if (!TalkState)
		{
			if (IsGrounded())
			{
				StateGroundStart();
				StateMachine.Initialize(StateGround);
			}
			else
			{
				StateJumpStart();
				StateMachine.Initialize(StateJump);
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
