using STHLua;
using UnityEngine;

public class AmigoAI : AmigoAIBase
{
	public enum State
	{
		Ground = 0,
		Jump = 1,
		VehicleSub = 2,
		Air = 3,
		Death = 4,
		FallDeath = 5,
		DrownDeath = 6,
		Path = 7,
		Result = 8,
		DashPanel = 9,
		Spring = 10,
		WideSpring = 11,
		JumpPanel = 12
	}

	internal State AmigoState;

	internal Vector3 AirMotionVelocity;

	private bool ResultsFaceMode;

	private bool ReachedGoal;

	private bool DoDeathState;

	private float DeathTime;

	private bool IdleAnimPlayed;

	private float IdleTimer;

	private int IdleAnim;

	private float JumpTime;

	private bool ReleasedJump;

	private bool ReachedApex;

	private VehicleBase Vehicle;

	private Collider[] VehicleCols;

	private Collider[] PlayerCols;

	private Transform PlayerSubPoint;

	private Vector3 JumpOffVel;

	private bool VehicleSubReverse;

	private int VehicleSubState;

	private float VehicleSubTime;

	private int FallDeathType;

	internal LinearBez LinearBezier;

	internal float PathSpeed;

	internal float PathTime;

	internal int PathMoveDir;

	private Vector3 PathNormal;

	private float PathYOffset;

	private bool PathDashpanel;

	private bool PlayedVictoryVoice;

	private Quaternion DashPanelRot;

	private float DashPanelStartTime;

	private float DashPanelSpeed;

	private float DashPanelTimer;

	private Vector3 SpringPos;

	private Vector3 SpringVel;

	private Vector3 SpringLaunchVelocity;

	private Vector3 SpringStartLaunchVelocity;

	private Quaternion SpringMeshLaunchRot;

	private float SpringStartTime;

	private float SpringTimer;

	private bool SpringFalling;

	private bool SpringOnStop;

	private bool SpringUseTimerToExit;

	private bool SpringUseTimerToRelease;

	private bool SpringAlwaysLocked;

	private string SpringLaunchAnimMode;

	private Vector3 WideSpringPos;

	private Vector3 WideSpringVel;

	private Vector3 WideSpringLaunchVelocity;

	private Vector3 WideSpringLastForward;

	private float WideSpringStartTime;

	private float WideSpringTimer;

	private bool WideSpringFalling;

	private Vector3 JumpPanelPos;

	private Vector3 JumpPanelVel;

	private Vector3 JumpPanelLaunchVelocity;

	private Vector3 JumpPanelStartLaunchVelocity;

	private Quaternion JumpPanelMeshLaunchRot;

	private float JumpPanelStartTime;

	private float JumpPanelTimer;

	private bool JumpPanelFalling;

	private bool JumpPanelOnCStop;

	private bool JumpPanelUseTimerToExit;

	private bool JumpPanelUseTimerToRelease;

	private bool JumpPanelAlwaysLocked;

	private string JumpPanelLaunchAnimMode;

	internal PathSystem PathSystem;

	internal PathData PathData;

	public override void Awake()
	{
		base.Awake();
		MaxRayLenght = 0.75f;
	}

	private void Start()
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
		if (Params.AmigoName == "silver")
		{
			SilverHasLotus = Singleton<GameManager>.Instance.GetGameData().HasFlag(Game.LotusOfResilience);
			Params.Animator.SetBool("Has Lotus", SilverHasLotus);
		}
	}

	private void StateGroundStart()
	{
		AmigoState = State.Ground;
		Params.Animator.ResetTrigger("Additive Idle");
		MaxRayLenght = 0.75f;
		IdleAnimPlayed = false;
		IdleTimer = Time.time;
	}

	private void StateGround()
	{
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (CurSpeed == 0f)
		{
			if (Time.time - IdleTimer > 7.5f && !IdleAnimPlayed)
			{
				IdleAnimPlayed = true;
				IdleTimer = Time.time;
				PlayIdleEvent(Params.IdleIDs);
				IdleAnimPlayed = false;
			}
		}
		else
		{
			Params.Animator.SetTrigger("Additive Idle");
			IdleTimer = Time.time;
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
		Params.Animator.SetTrigger("Additive Idle");
	}

	private void StateJumpStart()
	{
		AmigoState = State.Jump;
		Audio.PlayOneShot(Params.JumpSound, Audio.volume);
		if (Params.AmigoName != "amy" && Params.AmigoName != "omega" && Params.AmigoName != "metal")
		{
			Params.PlayRandomVoice(1, RandomPlayChance: true);
		}
		JumpTime = Time.time;
		if (Params.AmigoName != "princess" && (Params.AmigoName != "silver" || (Params.AmigoName == "silver" && SilverHasLotus)) && Params.AmigoName != "omega")
		{
			JumpAnimation = 0;
		}
		else
		{
			ReachedApex = false;
			PlayAnimation("Jump Up", "On Jump");
		}
		ReleasedJump = false;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 2.5f)
		{
			AirMotionVelocity += Vector3.up * Amigo_Lua.c_jump_speed;
		}
		else
		{
			AirMotionVelocity = Vector3.up * Amigo_Lua.c_jump_speed;
		}
		_Rigidbody.velocity = AirMotionVelocity;
		if (Params.AmigoName == "omega")
		{
			CreateOmegaJetFireFX();
		}
	}

	private void StateJump()
	{
		if (Params.AmigoName != "princess" && (Params.AmigoName != "silver" || (Params.AmigoName == "silver" && SilverHasLotus)) && Params.AmigoName != "omega")
		{
			PlayAnimation((Params.AmigoName != "silver") ? "Jump Up" : "Jump Up Upgrade", "On Jump");
			JumpAnimation = ((!(AirMotionVelocity.y > -3f)) ? 2 : ((Time.time - JumpTime > Amigo_Lua.c_jump_time_min) ? 1 : 0));
		}
		else if (!ReachedApex && (ReleasedJump || AirMotionVelocity.y < 3f))
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
		if (!FrontalCollision && !IsFrontalBump())
		{
			ReleasedJump = true;
		}
		if (!ReleasedJump && Time.time - JumpTime < 0.7f)
		{
			AirMotionVelocity += Vector3.up * 4.25f * Time.fixedDeltaTime * 4f;
		}
		MaxRayLenght = ((Time.time - JumpTime > 0.25f) ? 0.75f : 0f);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirMotionVelocity;
		if (IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			Params.CreateLandFXAndSound();
		}
	}

	private void StateJumpEnd()
	{
		MaxRayLenght = 0.75f;
	}

	private void StateVehicleSubStart()
	{
		AmigoState = State.VehicleSub;
		base.transform.SetParent(PlayerSubPoint);
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2);
			}
		}
		_Rigidbody.isKinematic = true;
		_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		_Rigidbody.velocity = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		VehicleSubTime = Time.time;
		VehicleSubState = 0;
		PlayAnimation("Roll And Fall", "On Roll And Fall");
		if (IsGrounded())
		{
			Audio.PlayOneShot(Vehicle.JumpOff, Audio.volume);
		}
	}

	private void StateVehicleSub()
	{
		LockControls = true;
		CurSpeed = 0f;
		if (VehicleSubState == 1)
		{
			GeneralMeshRotation = base.transform.rotation;
		}
		else
		{
			GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		}
		if (VehicleSubState == 0)
		{
			float num = Time.time - VehicleSubTime;
			_Rigidbody.MovePosition(Vector3.MoveTowards(base.transform.position + Vector3.up * 0.215f, PlayerSubPoint.position, num * 0.45f));
			if (num > 1f)
			{
				PlayAnimation("Vehicle Sub", "On Vehicle Sub");
				VehicleSubState = 1;
			}
			return;
		}
		if (VehicleSubState == 1)
		{
			Params.Animator.SetFloat("Vehicle Sub Steer X", Vehicle.MoveAxes.x);
			if (!Vehicle.VehicleDamage)
			{
				if (!Vehicle.GoingReverse)
				{
					if (VehicleSubReverse)
					{
						PlayAnimation("Vehicle Sub Reverse End", "On Vehicle Sub Reverse End");
						VehicleSubReverse = false;
					}
				}
				else if (!VehicleSubReverse)
				{
					PlayAnimation("Vehicle Sub Reverse Start", "On Vehicle Sub Reverse");
					VehicleSubReverse = true;
				}
			}
			if (base.transform.localPosition != Vector3.zero)
			{
				base.transform.localPosition = Vector3.zero;
			}
			if (base.transform.localRotation != Quaternion.identity)
			{
				base.transform.localRotation = Quaternion.identity;
			}
			return;
		}
		JumpOffVel.y = Mathf.Lerp(JumpOffVel.y, 0f, Mathf.Clamp01((Time.time - VehicleSubTime) * 0.25f));
		_Rigidbody.velocity = JumpOffVel;
		if (!(Time.time - VehicleSubTime > 0.5f))
		{
			return;
		}
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2, ignore: false);
			}
		}
		StateMachine.ChangeState(StateAir);
	}

	private void StateVehicleSubEnd()
	{
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2, ignore: false);
			}
		}
	}

	private void StateAirStart()
	{
		AmigoState = State.Air;
		AirMotionVelocity = _Rigidbody.velocity;
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateAir()
	{
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
		if (IsGrounded())
		{
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			Params.CreateLandFXAndSound();
		}
	}

	private void StateAirEnd()
	{
	}

	private void StateDeathStart()
	{
		AmigoState = State.Death;
		PlayAnimation("Death Normal", "On Death Ground");
		DeathTime = Time.time;
	}

	private void StateDeath()
	{
		_Rigidbody.velocity = Vector3.zero;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
	}

	private void StateDeathEnd()
	{
	}

	private void StateFallDeathStart()
	{
		AmigoState = State.FallDeath;
		DeathTime = Time.time;
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		AirMotionVelocity = _Rigidbody.velocity;
		if (FallDeathType == 1)
		{
			AirMotionVelocity.y = 0f;
			PlayAnimation("Flip Death Fall", "On Flip Death Air");
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateFallDeath()
	{
		new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (FallDeathType == 0)
		{
			PlayAnimation(IsGrounded() ? "Death Lay" : "Death Fall", IsGrounded() ? "On Death Ground" : "On Death Air");
		}
		else if (IsGrounded())
		{
			PlayAnimation("Death Lay", "On Death Ground");
		}
		if (!IsGrounded())
		{
			AirMotionVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		else
		{
			AirMotionVelocity.y = 0f;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateFallDeathEnd()
	{
	}

	private void StateDrownDeathStart()
	{
		AmigoState = State.DrownDeath;
		DeathTime = Time.time;
		AirMotionVelocity = _Rigidbody.velocity;
		if (AirMotionVelocity.y > 0f)
		{
			AirMotionVelocity.y = 0f;
		}
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateDrownDeath()
	{
		new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		PlayAnimation(IsGrounded() ? "Death Lay" : "Death Drown", IsGrounded() ? "On Death Ground" : "On Death Air");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (!IsGrounded())
		{
			AirMotionVelocity.y = Mathf.Lerp(AirMotionVelocity.y, -5f, 10f * Time.fixedDeltaTime);
		}
		else
		{
			AirMotionVelocity.y = 0f;
		}
		AirMotionVelocity.y = LimitVel(AirMotionVelocity.y);
		_Rigidbody.velocity = AirMotionVelocity;
	}

	private void StateDrownDeathEnd()
	{
	}

	private void StatePathStart()
	{
		AmigoState = State.Path;
		PathSpeed = CurSpeed;
		PathMoveDir = ((Vector3.Dot(LinearBezier.GetTangent(PathTime), base.transform.forward) >= 0f) ? 1 : (-1));
		MaxRayLenght = 1.75f;
	}

	private void StatePath()
	{
		LockControls = true;
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		if (PathSpeed < TopSpeed)
		{
			PathSpeed += TopSpeed * Time.fixedDeltaTime;
		}
		if (PathSpeed >= TopSpeed && PathSpeed < TopSpeed * 2f && !PathDashpanel)
		{
			PathSpeed += Vector3.Dot(new Vector3(0f, -0.5f, 0f), LinearBezier.GetTangent(PathTime).normalized * PathMoveDir);
		}
		CurSpeed = PathSpeed;
		PathTime += PathSpeed / LinearBezier.Length() * Time.fixedDeltaTime * (float)PathMoveDir;
		PathNormal = PathData.normal[LinearBezier.GetSegment(PathTime)];
		GeneralMeshRotation = Quaternion.LookRotation(LinearBezier.GetTangent(PathTime).normalized * PathMoveDir, PathNormal);
		_Rigidbody.velocity = Vector3.zero;
		if (PathYOffset == 0f)
		{
			_Rigidbody.MovePosition(LinearBezier.GetPosition(PathTime) + PathNormal * 0.25f);
		}
		else
		{
			_Rigidbody.MovePosition(LinearBezier.GetPosition(PathTime) + PathNormal * 0.25f + base.transform.up * PathYOffset);
		}
		base.transform.up = PathNormal;
		if (PathTime <= 0.001f || PathTime >= 0.999f)
		{
			StateMachine.ChangeState(StateGround);
		}
	}

	private void StatePathEnd()
	{
		base.transform.forward = LinearBezier.GetTangent(PathTime).normalized * PathMoveDir;
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
		MaxRayLenght = 0.75f;
		PositionToPoint();
	}

	private void StateResultStart()
	{
		AmigoState = State.Result;
		PlayAnimation("GoalPose Start", "On Results");
		ResultStartTime = Time.time;
		PlayedVictoryVoice = false;
		base.transform.position = AmigoPoint();
		if (Params.AmigoName == "silver")
		{
			base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
		}
		else if (GetAmigoState() == "Result" && FollowTarget.GetPrefab("silver"))
		{
			if (Params.AmigoName == "sonic")
			{
				base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, -110f, 0f);
			}
			else
			{
				base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, -90f, 0f);
			}
		}
		else
		{
			base.transform.rotation = FollowTarget.transform.rotation;
		}
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
	}

	private void StateResult()
	{
		if (!PlayedVictoryVoice && Time.time - ResultStartTime > ((PlayerAmigoIndex > 1) ? 1.5f : 0.75f))
		{
			Singleton<AudioManager>.Instance.PlayVoiceClip(VoicesAudio, Params.VictoryVoice);
			PlayedVictoryVoice = true;
		}
		CurSpeed = 0f;
		base.transform.position = AmigoPoint();
		if (Params.AmigoName == "silver")
		{
			base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
		}
		else if (GetAmigoState() == "Result" && FollowTarget.GetPrefab("silver"))
		{
			if (Params.AmigoName == "sonic")
			{
				base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, -110f, 0f);
			}
			else
			{
				base.transform.rotation = FollowTarget.transform.rotation * Quaternion.Euler(0f, -90f, 0f);
			}
		}
		else
		{
			base.transform.rotation = FollowTarget.transform.rotation;
		}
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateResultEnd()
	{
	}

	private void StateDashPanelStart()
	{
		AmigoState = State.DashPanel;
		DashPanelStartTime = Time.time;
		LockControls = true;
		base.transform.rotation = DashPanelRot;
	}

	private void StateDashPanel()
	{
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		LockControls = true;
		MaxRayLenght = 1.25f;
		float curSpeed = DashPanelSpeed * ((DashPanelSpeed < TopSpeed) ? 1.5f : 1f);
		CurSpeed = curSpeed;
		_Rigidbody.velocity = Vector3.ProjectOnPlane(base.transform.forward, RaycastHit.normal) * CurSpeed;
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		if (Time.time - DashPanelStartTime > DashPanelTimer * 0.5f || FrontalCollision)
		{
			MaxRayLenght = 1.25f;
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
		}
		if (!IsGrounded())
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateDashPanelEnd()
	{
	}

	private void StateSpringStart()
	{
		AmigoState = State.Spring;
		SpringStartTime = Time.time;
		SpringLaunchVelocity = SpringVel;
		SpringStartLaunchVelocity = SpringLaunchVelocity.normalized;
		base.transform.position = SpringPos;
		SpringMeshLaunchRot = Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(30f, 0f, 0f);
		base.transform.forward = SpringLaunchVelocity.MakePlanar();
		MaxRayLenght = 0.75f;
		SpringFalling = false;
		SpringOnStop = false;
	}

	private void StateSpring()
	{
		bool flag = Time.time - SpringStartTime < (SpringUseTimerToExit ? 0f : SpringTimer);
		if (flag)
		{
			if (SpringLaunchAnimMode == "spring_a" || SpringLaunchAnimMode == "spring_b" || SpringLaunchAnimMode == "spring_c")
			{
				PlayAnimation("Spring Jump", "On Spring");
			}
			else
			{
				PlayAnimation("Falling", "On Fall");
			}
			SpringFalling = false;
		}
		else if (SpringLaunchAnimMode == "spring_a" || SpringLaunchAnimMode == "spring_b" || SpringLaunchAnimMode == "spring_c")
		{
			if (_Rigidbody.velocity.y > -0.1f)
			{
				PlayAnimation("Spring Jump", "On Spring");
				SpringFalling = false;
			}
			else if (!SpringFalling)
			{
				SpringFalling = true;
				PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
		}
		if (flag)
		{
			SpringMeshLaunchRot = Quaternion.Slerp(SpringMeshLaunchRot, Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			_Rigidbody.velocity = SpringLaunchVelocity;
			base.transform.forward = SpringLaunchVelocity.MakePlanar();
			CurSpeed = _Rigidbody.velocity.magnitude;
			LockControls = true;
		}
		else
		{
			if (SpringLaunchAnimMode == "spring_a")
			{
				CurSpeed = _Rigidbody.velocity.magnitude;
				base.transform.forward = SpringLaunchVelocity.MakePlanar();
				SpringLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				LockControls = true;
				SpringMeshLaunchRot = Quaternion.Slerp(SpringMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(SpringLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
			}
			else if (SpringLaunchAnimMode == "spring_b")
			{
				if ((SpringUseTimerToExit || SpringUseTimerToRelease) && !SpringAlwaysLocked)
				{
					if (Time.time - SpringStartTime < SpringTimer)
					{
						CurSpeed = _Rigidbody.velocity.magnitude;
						base.transform.forward = SpringLaunchVelocity.MakePlanar();
						SpringLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
						LockControls = true;
						SpringMeshLaunchRot = Quaternion.Slerp(SpringMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(SpringLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					}
					else
					{
						if (!SpringOnStop && !SpringUseTimerToRelease)
						{
							if (CurSpeed > TopSpeed)
							{
								CurSpeed = TopSpeed;
							}
							SpringOnStop = true;
						}
						SpringMeshLaunchRot = Quaternion.RotateTowards(SpringMeshLaunchRot, Quaternion.LookRotation(SpringLaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 200f);
						GeneralMeshRotation = base.transform.rotation;
						GeneralMeshRotation.x = SpringMeshLaunchRot.x;
						GeneralMeshRotation.z = SpringMeshLaunchRot.z;
						Vector3 vector = new Vector3(SpringLaunchVelocity.x, 0f, SpringLaunchVelocity.z);
						if (_Rigidbody.velocity.magnitude != 0f)
						{
							vector = base.transform.forward * CurSpeed;
							SpringLaunchVelocity = new Vector3(vector.x, SpringLaunchVelocity.y, vector.z);
						}
						SpringLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						SpringLaunchVelocity.y = LimitVel(SpringLaunchVelocity.y);
						base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
					}
				}
				else
				{
					CurSpeed = _Rigidbody.velocity.magnitude;
					base.transform.forward = SpringLaunchVelocity.MakePlanar();
					SpringLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					LockControls = true;
					SpringMeshLaunchRot = Quaternion.Slerp(SpringMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(SpringLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (SpringLaunchAnimMode == "spring_c")
			{
				if (!SpringAlwaysLocked)
				{
					if (_Rigidbody.velocity.y > 0f)
					{
						CurSpeed = _Rigidbody.velocity.magnitude;
						base.transform.forward = SpringLaunchVelocity.MakePlanar();
						SpringLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						LockControls = true;
					}
					else
					{
						GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
						Vector3 vector2 = new Vector3(SpringLaunchVelocity.x, 0f, SpringLaunchVelocity.z);
						if (_Rigidbody.velocity.magnitude != 0f)
						{
							vector2 = base.transform.forward * CurSpeed;
							SpringLaunchVelocity = new Vector3(vector2.x, SpringLaunchVelocity.y, vector2.z);
						}
						SpringLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						SpringLaunchVelocity.y = LimitVel(SpringLaunchVelocity.y);
						base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
					}
				}
				else
				{
					CurSpeed = _Rigidbody.velocity.magnitude;
					base.transform.forward = SpringLaunchVelocity.MakePlanar();
					SpringLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					LockControls = true;
				}
				SpringMeshLaunchRot = Quaternion.Slerp(SpringMeshLaunchRot, (_Rigidbody.velocity.y > -0.1f) ? (Quaternion.LookRotation(SpringStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)) : Quaternion.LookRotation(SpringLaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 5f);
			}
			_Rigidbody.velocity = SpringLaunchVelocity;
		}
		if (LockControls)
		{
			GeneralMeshRotation = SpringMeshLaunchRot;
		}
		if (IsGrounded() && ((SpringTimer != 0f && !SpringUseTimerToExit) ? (!flag) : (Time.time - SpringStartTime > 0.1f)))
		{
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			Params.CreateLandFXAndSound();
		}
		if (FrontalCollision && ((SpringTimer != 0f && !SpringUseTimerToExit) ? (!flag) : (Time.time - SpringStartTime > 0.1f)))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateSpringEnd()
	{
	}

	private void StateWideSpringStart()
	{
		AmigoState = State.WideSpring;
		PlayAnimation("Spring Jump", "On Spring");
		WideSpringStartTime = Time.time;
		WideSpringLaunchVelocity = WideSpringVel;
		WideSpringLastForward = base.transform.forward;
		base.transform.position = WideSpringPos + Vector3.up * 0.25f;
		WideSpringFalling = false;
	}

	private void StateWideSpring()
	{
		bool flag = Time.time - WideSpringStartTime < WideSpringTimer;
		if (Time.time - WideSpringStartTime < WideSpringTimer)
		{
			CurSpeed = 0f;
			base.transform.up = Vector3.up;
			base.transform.forward = WideSpringLastForward;
			_Rigidbody.velocity = WideSpringLaunchVelocity;
			LockControls = true;
		}
		else
		{
			Vector3 vector = new Vector3(WideSpringLaunchVelocity.x, 0f, WideSpringLaunchVelocity.z);
			if (_Rigidbody.velocity.magnitude != 0f)
			{
				vector = base.transform.forward * CurSpeed;
				WideSpringLaunchVelocity = new Vector3(vector.x, WideSpringLaunchVelocity.y, vector.z);
			}
			WideSpringLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
			WideSpringLaunchVelocity.y = LimitVel(WideSpringLaunchVelocity.y);
			_Rigidbody.velocity = WideSpringLaunchVelocity;
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		}
		if (_Rigidbody.velocity.y < 0f)
		{
			if (!WideSpringFalling)
			{
				WideSpringFalling = true;
				PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
		}
		else
		{
			WideSpringFalling = false;
			PlayAnimation("Spring Jump", "On Spring");
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		if (IsGrounded() && !flag)
		{
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			Params.CreateLandFXAndSound();
		}
	}

	private void StateWideSpringEnd()
	{
	}

	private void StateJumpPanelStart()
	{
		AmigoState = State.JumpPanel;
		JumpPanelStartTime = Time.time;
		JumpPanelLaunchVelocity = JumpPanelVel;
		JumpPanelStartLaunchVelocity = JumpPanelLaunchVelocity.normalized;
		base.transform.position = JumpPanelPos + base.transform.up * 0.25f;
		JumpPanelMeshLaunchRot = Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(30f, 0f, 0f);
		base.transform.forward = JumpPanelLaunchVelocity.MakePlanar();
		MaxRayLenght = 0.75f;
		JumpPanelFalling = false;
		JumpPanelOnCStop = false;
	}

	private void StateJumpPanel()
	{
		bool flag = Time.time - JumpPanelStartTime < ((!JumpPanelUseTimerToExit) ? JumpPanelTimer : 0f);
		if (flag)
		{
			if (JumpPanelLaunchAnimMode == "jumppanel_a" || JumpPanelLaunchAnimMode == "jumppanel_c")
			{
				PlayAnimation("Spring Jump", "On Spring");
			}
			else if (Params.AmigoName == "silver" || Params.AmigoName == "omega")
			{
				PlayAnimation("Spring Jump", "On Spring");
			}
			else
			{
				PlayAnimation("Rolling", "On Roll");
			}
			JumpPanelFalling = false;
		}
		else if (JumpPanelLaunchAnimMode == "jumppanel_a" || JumpPanelLaunchAnimMode == "jumppanel_c")
		{
			if (_Rigidbody.velocity.y > -0.1f)
			{
				PlayAnimation("Spring Jump", "On Spring");
				JumpPanelFalling = false;
			}
			else if (!JumpPanelFalling)
			{
				JumpPanelFalling = true;
				PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
		}
		else if (JumpPanelLaunchAnimMode == "jumppanel_b" || JumpPanelLaunchAnimMode == "jumppanel_d")
		{
			if (Params.AmigoName == "silver" || Params.AmigoName == "omega")
			{
				if (_Rigidbody.velocity.y > -0.1f)
				{
					PlayAnimation("Spring Jump", "On Spring");
					JumpPanelFalling = false;
				}
				else if (!JumpPanelFalling)
				{
					JumpPanelFalling = true;
					PlayAnimation("Roll And Fall", "On Roll And Fall");
				}
			}
			else if (_Rigidbody.velocity.y > -0.1f)
			{
				PlayAnimation("Rolling", "On Roll");
			}
			else
			{
				PlayAnimation("Falling", "On Fall");
			}
		}
		if (flag)
		{
			if ((JumpPanelLaunchAnimMode == "jumppanel_b" || JumpPanelLaunchAnimMode == "jumppanel_d") && Time.time - JumpPanelStartTime > JumpPanelTimer * 0.3f)
			{
				JumpPanelLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			}
			else if (JumpPanelLaunchAnimMode == "jumppanel_a" || JumpPanelLaunchAnimMode == "jumppanel_c")
			{
				JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			}
			CurSpeed = _Rigidbody.velocity.magnitude;
			base.transform.forward = JumpPanelLaunchVelocity.MakePlanar();
			_Rigidbody.velocity = JumpPanelLaunchVelocity;
			LockControls = true;
		}
		else
		{
			if (JumpPanelLaunchAnimMode == "jumppanel_a" || JumpPanelLaunchAnimMode == "jumppanel_d")
			{
				if (JumpPanelUseTimerToRelease && !JumpPanelAlwaysLocked)
				{
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
					GeneralMeshRotation.x = JumpPanelMeshLaunchRot.x;
					GeneralMeshRotation.z = JumpPanelMeshLaunchRot.z;
					Vector3 vector = new Vector3(JumpPanelLaunchVelocity.x, 0f, JumpPanelLaunchVelocity.z);
					if (_Rigidbody.velocity.magnitude != 0f)
					{
						vector = base.transform.forward * CurSpeed;
						JumpPanelLaunchVelocity = new Vector3(vector.x, JumpPanelLaunchVelocity.y, vector.z);
					}
					JumpPanelLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					JumpPanelLaunchVelocity.y = LimitVel(JumpPanelLaunchVelocity.y);
					base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
				}
				else
				{
					CurSpeed = _Rigidbody.velocity.magnitude;
					base.transform.forward = JumpPanelLaunchVelocity.MakePlanar();
					JumpPanelLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					_Rigidbody.velocity = JumpPanelLaunchVelocity;
					LockControls = true;
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (JumpPanelLaunchAnimMode == "jumppanel_b")
			{
				if (!JumpPanelUseTimerToExit && !JumpPanelAlwaysLocked)
				{
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
					GeneralMeshRotation.x = JumpPanelMeshLaunchRot.x;
					GeneralMeshRotation.z = JumpPanelMeshLaunchRot.z;
					Vector3 vector2 = new Vector3(JumpPanelLaunchVelocity.x, 0f, JumpPanelLaunchVelocity.z);
					if (_Rigidbody.velocity.magnitude != 0f)
					{
						vector2 = base.transform.forward * CurSpeed;
						JumpPanelLaunchVelocity = new Vector3(vector2.x, JumpPanelLaunchVelocity.y, vector2.z);
					}
					JumpPanelLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					JumpPanelLaunchVelocity.y = LimitVel(JumpPanelLaunchVelocity.y);
					base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
				}
				else
				{
					CurSpeed = _Rigidbody.velocity.magnitude;
					base.transform.forward = JumpPanelLaunchVelocity.MakePlanar();
					JumpPanelLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					_Rigidbody.velocity = JumpPanelLaunchVelocity;
					LockControls = true;
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (JumpPanelLaunchAnimMode == "jumppanel_c")
			{
				if (_Rigidbody.velocity.y < 0f && !JumpPanelAlwaysLocked)
				{
					if (!JumpPanelOnCStop)
					{
						if (CurSpeed > TopSpeed)
						{
							CurSpeed = TopSpeed;
						}
						JumpPanelOnCStop = true;
					}
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
					GeneralMeshRotation.x = JumpPanelMeshLaunchRot.x;
					GeneralMeshRotation.z = JumpPanelMeshLaunchRot.z;
					Vector3 vector3 = new Vector3(JumpPanelLaunchVelocity.x, 0f, JumpPanelLaunchVelocity.z);
					if (_Rigidbody.velocity.magnitude != 0f)
					{
						vector3 = base.transform.forward * CurSpeed;
						JumpPanelLaunchVelocity = new Vector3(vector3.x, JumpPanelLaunchVelocity.y, vector3.z);
					}
					JumpPanelLaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					JumpPanelLaunchVelocity.y = LimitVel(JumpPanelLaunchVelocity.y);
					base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
				}
				else
				{
					CurSpeed = _Rigidbody.velocity.magnitude;
					base.transform.forward = JumpPanelLaunchVelocity.MakePlanar();
					JumpPanelLaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					_Rigidbody.velocity = JumpPanelLaunchVelocity;
					LockControls = true;
					JumpPanelMeshLaunchRot = Quaternion.Slerp(JumpPanelMeshLaunchRot, (_Rigidbody.velocity.y < 0f) ? Quaternion.LookRotation(JumpPanelLaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(JumpPanelStartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			_Rigidbody.velocity = JumpPanelLaunchVelocity;
		}
		if (LockControls)
		{
			GeneralMeshRotation = JumpPanelMeshLaunchRot;
		}
		if (IsGrounded() && ((JumpPanelTimer != 0f && !JumpPanelUseTimerToExit) ? (!flag) : (Time.time - JumpPanelStartTime > 0.1f)))
		{
			PositionToPoint();
			StateMachine.ChangeState(StateGround);
			DoLandAnim();
			Params.CreateLandFXAndSound();
		}
		if (FrontalCollision && ((JumpPanelTimer != 0f && !JumpPanelUseTimerToExit) ? (!flag) : (Time.time - JumpPanelStartTime > 0.1f)))
		{
			StateMachine.ChangeState(StateAir);
		}
	}

	private void StateJumpPanelEnd()
	{
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (AmigoState == State.Ground || AmigoState == State.Death || AmigoState == State.FallDeath || AmigoState == State.DrownDeath)
		{
			PositionToPoint();
		}
		if (LockControls)
		{
			LockControls = false;
		}
		StateMachine.UpdateStateMachine();
	}

	public override void Update()
	{
		base.Update();
		if (AmigoState == State.Ground || AmigoState == State.Death || AmigoState == State.FallDeath || AmigoState == State.DrownDeath)
		{
			PositionToPoint();
		}
		if (Params.AmigoName != "omega" && Params.AmigoName != "metal")
		{
			if (AmigoState != State.Result)
			{
				if (CurSpeed != 0f || AmigoState == State.VehicleSub)
				{
					PlayFaceAnim(1);
				}
				else if (Params.AmigoName == "sonic")
				{
					if (Singleton<Settings>.Instance.settings.TGSSonic == 0)
					{
						PlayFaceAnim((IsAnimState("Idle_Face2") || IsAnimState("Idle_Face4")) ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
					}
					else
					{
						PlayFaceAnim((IsAnimState("Idle_Face1") || IsAnimState("Idle_Face4")) ? 2 : 0);
					}
				}
				else if (Params.AmigoName == "princess")
				{
					PlayFaceAnim(IsAnimState("Idle_Face1") ? 1 : (IsAnimState("Idle_Face2") ? 2 : 0));
				}
				else if (Params.AmigoName == "tails")
				{
					PlayFaceAnim(IsAnimState("Idle_Face3") ? 2 : 0);
				}
				else if (Params.AmigoName == "silver")
				{
					PlayFaceAnim(IsAnimState("Idle_Face2") ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
				}
				else if (Params.AmigoName == "shadow")
				{
					PlayFaceAnim(IsAnimState("Idle_Face1") ? 2 : (IsAnimState("Idle_Face2") ? 1 : 0));
				}
				else if (Params.AmigoName == "rouge")
				{
					PlayFaceAnim(0);
				}
				else if (Params.AmigoName == "knuckles")
				{
					PlayFaceAnim(IsAnimState("Idle_Face1") ? 1 : (IsAnimState("Idle_Face2") ? 2 : 0));
				}
				else if (Params.AmigoName == "blaze")
				{
					PlayFaceAnim(IsAnimState("Idle_Face1") ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
				}
				else if (Params.AmigoName == "amy")
				{
					PlayFaceAnim(0);
				}
			}
			else
			{
				Params.Animator.SetBool("Face Results", value: true);
				if (!ResultsFaceMode)
				{
					Params.Animator.SetTrigger("On Face Results");
					ResultsFaceMode = true;
				}
			}
		}
		if (AmigoState == State.Ground && (((FrontalCollision || IsFrontalBump()) && OutOfDistance() && !TotalControlLock) || SinkPosition > 0.5f))
		{
			StateMachine.ChangeState(StateJump);
		}
		if (AmigoState == State.VehicleSub)
		{
			if (VehicleSubState != 2 && FollowTarget.GetState() != "Vehicle")
			{
				_Rigidbody.isKinematic = false;
				_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				base.transform.SetParent(null);
				VehicleSubState = 2;
				VehicleSubTime = Time.time;
				JumpOffVel = Vector3.zero;
				JumpOffVel.y = 10f;
				_Rigidbody.velocity = JumpOffVel;
				PlayAnimation("Rolling", "On Roll");
				Audio.PlayOneShot(Vehicle.JumpOff, Audio.volume);
			}
			if (Vehicle.CurHP == 0f)
			{
				_Rigidbody.isKinematic = false;
				_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
				base.transform.SetParent(null);
				StateMachine.ChangeState(StateAir);
			}
		}
		if (FollowTarget.GetState() == "Result" && !ReachedGoal)
		{
			StateMachine.ChangeState(StateResult);
			ReachedGoal = true;
		}
	}

	private void PlayFaceAnim(int AnimIndex)
	{
		Params.Animator.SetInteger("Face Index", AnimIndex);
	}

	public void PlayIdleEvent(int MaxIndex)
	{
		Params.Animator.SetTrigger("On Idle Wait");
		Params.Animator.SetInteger("Idle Anim ID", IdleAnim);
		IdleAnim++;
		if (IdleAnim > MaxIndex)
		{
			IdleAnim = 0;
		}
	}

	private bool IsAnimState(string StateName)
	{
		if (Params.Animator.GetCurrentAnimatorStateInfo(1).IsName(StateName))
		{
			return true;
		}
		return false;
	}

	public void OnVehicleSub(Transform _PlayerSubPoint, Collider[] _VehicleCols, VehicleBase _Vehicle)
	{
		if (Params.AmigoName == "sonic" || Params.AmigoName == "rouge")
		{
			PlayerSubPoint = _PlayerSubPoint;
			VehicleCols = _VehicleCols;
			if (PlayerCols == null)
			{
				PlayerCols = GetComponentsInChildren<Collider>();
			}
			Vehicle = _Vehicle;
			StateMachine.ChangeState(StateVehicleSub);
		}
	}

	public void OnPathEnter(PathSystem PathSys, bool CanJumpOff, float YOffset)
	{
		if (!(GetAmigoState() != "Path") || !(GetAmigoState() != "VehicleSub"))
		{
			return;
		}
		float num = PathSys.PathDist(base.transform.position, base.transform.forward);
		if (num != -1f)
		{
			PathSystem = PathSys;
			PathData = PathSystem.BuildPathData(num);
			PathYOffset = YOffset;
			LinearBezier = new LinearBez(PathData.position);
			PathTime = LinearBezier.GetTime(PathSystem.FindClosestPoint(PathData, base.transform.position, base.transform.forward));
			if (PathTime > 0.001f && PathTime < 0.999f)
			{
				StateMachine.ChangeState(StatePath);
			}
		}
	}

	public override void OnDashPanel(Quaternion Rot, Vector3 Normal, Vector3 Forward, float Speed, float Timer)
	{
		DashPanelRot = Rot;
		DashPanelSpeed = Speed;
		DashPanelTimer = Timer;
		base.transform.up = Normal;
		if (AmigoState == State.Path)
		{
			PathSpeed = Speed * ((Speed < TopSpeed) ? 1.5f : 1f);
			if (Vector3.Dot(LinearBezier.GetTangent(PathTime) * PathMoveDir, Forward) < 0f)
			{
				PathMoveDir *= -1;
			}
			if (!PathDashpanel)
			{
				PathDashpanel = true;
			}
		}
		else
		{
			StateMachine.ChangeState(StateDashPanel);
		}
	}

	public override void OnSpring(Vector3 Pos, Vector3 Vel, float Timer, string LaunchAnimMode, bool UseTimerToExit, bool UseTimerToRelease, bool AlwaysLocked)
	{
		SpringPos = Pos;
		SpringVel = Vel;
		SpringTimer = Timer;
		SpringLaunchAnimMode = LaunchAnimMode;
		SpringUseTimerToExit = UseTimerToExit;
		SpringUseTimerToRelease = UseTimerToRelease;
		SpringAlwaysLocked = AlwaysLocked;
		StateMachine.ChangeState(StateSpring);
	}

	public override void OnWideSpring(Vector3 Pos, Vector3 Vel, float Timer)
	{
		WideSpringPos = Pos;
		WideSpringVel = Vel;
		WideSpringTimer = Timer;
		StateMachine.ChangeState(StateWideSpring);
	}

	public override void OnJumpPanel(Vector3 Pos, Vector3 Vel, float Timer, string LaunchAnimMode, bool UseTimerToExit, bool UseTimerToRelease, bool AlwaysLocked)
	{
		JumpPanelPos = Pos;
		JumpPanelVel = Vel;
		JumpPanelTimer = Timer;
		JumpPanelLaunchAnimMode = LaunchAnimMode;
		JumpPanelUseTimerToExit = UseTimerToExit;
		JumpPanelUseTimerToRelease = UseTimerToRelease;
		JumpPanelAlwaysLocked = AlwaysLocked;
		StateMachine.ChangeState(StateJumpPanel);
	}

	public override void OnDeathEnter(int DeathType)
	{
		if (IsDead || AmigoState == State.Death || AmigoState == State.FallDeath || AmigoState == State.DrownDeath)
		{
			return;
		}
		IsDead = true;
		switch (DeathType)
		{
		case 0:
			if (IsGrounded())
			{
				StateMachine.ChangeState(StateDeath);
				break;
			}
			FallDeathType = 1;
			StateMachine.ChangeState(StateFallDeath);
			break;
		case 1:
			FallDeathType = 0;
			StateMachine.ChangeState(StateFallDeath);
			break;
		case 2:
			StateMachine.ChangeState(StateDrownDeath);
			break;
		}
	}

	public override string GetAmigoState()
	{
		return AmigoState.ToString();
	}

	public override void SetAmigoMachineState(string StateName)
	{
		if (StateName == "StateReappear")
		{
			StateMachine.ChangeState(StateAir);
		}
	}
}
