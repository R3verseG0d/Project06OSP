using System.Collections;
using System.Collections.Generic;
using STHEngine;
using STHLua;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
	public enum Prefab
	{
		player_null = 0,
		sonic_new = 1,
		sonic_fast = 2,
		princess = 3,
		snow_board = 4,
		shadow = 5,
		silver = 6,
		tails = 7,
		amy = 8,
		knuckles = 9,
		blaze = 10,
		rouge = 11,
		omega = 12,
		metal_sonic = 13
	}

	[Header("Player Base")]
	public Prefab PlayerPrefab;

	public Rigidbody _Rigidbody;

	public StateMachine StateMachine;

	public PlayerManager PlayerManager;

	public Animator Animator;

	public CapsuleCollider CapsuleCollider;

	public Transform Mesh;

	public Transform BodyTransform;

	public Transform CameraTarget;

	public LayerMask AttackMask;

	public GameObject Shield;

	public GameObject InvincibleStars;

	public GameObject SpeedUp;

	public GameObject LostRingPrefab;

	public PlayerVoice PlayerVoice;

	public AudioClip[] OneUPSounds;

	public AudioSource Audio;

	public AudioSource VoicesAudio;

	public AudioClip GrindWind;

	internal PlayerCamera Camera;

	internal CameraEffects CameraFX;

	internal StageManager StageManager;

	internal UI HUD;

	internal RaycastHit RaycastHit;

	internal RaycastHit FrontalHit;

	internal AnimationCurve RotOverSpdCurve;

	internal GameObject HomingTarget;

	internal GameObject ShieldObject;

	internal Vector3 CamOffset;

	internal Vector3 TargetDirection;

	internal Vector3 WallNormal;

	internal Vector3 GroundNormal;

	internal Vector3 UpMeshRotation;

	internal Vector3 ForwardMeshRotation;

	internal Quaternion GeneralMeshRotation;

	internal float MaxRayLenght;

	internal float CurSpeed;

	internal float WalkSpeed;

	internal float TopSpeed;

	internal float BrakeSpeed;

	internal float MaximumSpeed;

	internal float GrindSpeedOrg;

	internal float GrindAcc;

	internal float GrindSpeedMax;

	internal float GroundDot = 0.65f;

	internal float ImmunityTime;

	internal int PlayerNo;

	internal int JumpAnimation;

	internal bool IsVisible;

	internal bool UseCharacterSway;

	internal bool LockControls;

	internal bool FrontalCollision;

	internal bool IsOnWall;

	internal bool IsDead;

	internal bool GrindTrick;

	internal bool WalkSwitch;

	internal bool HasShield;

	internal bool HasInvincibility;

	internal bool HasSpeedUp;

	internal string PlayerName;

	internal string PlayerNameShort;

	internal int AmigoIndex;

	private List<ContactPoint> Contacts;

	private GameObject InvStarsObject;

	private GameObject SpeedUpObject;

	private Vector3 MachCamOffset;

	private float InvincibleTime;

	private float SpeedUpTime;

	private float CamDirDot;

	private float RunWalkAnimation;

	internal bool IdleAnimPlayed;

	internal int IdleAnim;

	internal float IdleTimer;

	private bool TrickCombo;

	private int ComboState;

	private int ComboScoreDiff;

	private int ComboLevel;

	private int ComboMinScore = 200;

	private int NewComboScoreDiff;

	private int NewComboLevel;

	private float ComboMaxMultiplier = 10f;

	internal int ComboBonus;

	internal int NewComboBonus;

	internal float ComboTime;

	internal Vector3 WorldVelocity = Vector3.zero;

	private Vector3 LastPosition = Vector3.zero;

	internal bool CanSink;

	internal bool LockSink;

	internal bool IsSinking;

	internal bool CanSinkJump;

	internal bool HalveSinkJump;

	internal float SinkPosition;

	internal float SinkStartTime;

	internal string ColName;

	internal bool IsSlopePhys;

	private bool IsApplyDrag;

	private float Angle;

	private float SlopeFactor = 10f;

	private Keyframe[] Keys;

	private Vector3 AirVelocity;

	private float SlowFallTime;

	private float SlowFallSpeed;

	private RailSystem RailSys;

	internal RailData RailData;

	private Vector3 Normal;

	private Vector3 SwitchPos;

	private Collider WindRoadCol;

	private float GrindStartTime;

	private float GrindFlipTime;

	private float GrindSplineLength;

	private float GrindTime;

	private float SwitchTime;

	internal float GrindDir;

	internal float GrindSpeed;

	private float GrindNormalTime;

	private float GrindUpdateTime;

	private float GrindTilt;

	internal bool RailSwitch;

	private bool LeftSwitching;

	private bool IsWindRoad;

	private float DeathTime;

	private int FallDeathType;

	private float TalkTime;

	internal LinearBez LinearBezier;

	internal float PathSpeed;

	internal float PathTime;

	internal int PathMoveDir;

	private Vector3 PathNormal;

	private float PathYOffset;

	internal bool PathDashpanel;

	private Vector3 WarpPos;

	private int WarpType;

	private float WarpTime;

	private bool WarpAppear;

	internal float ResultTime;

	private bool HasWaitQuote;

	private int IdleIndex;

	private int TriggerIndex;

	private AudioSource LastRingSource;

	internal PathSystem PathSystem;

	internal PathData PathData;

	private bool PathJumpOff;

	private float BrakeSpdStore;

	internal PlayerGoal PlayerGoal;

	internal RailSystem RailSystem;

	internal RailSystem SwitchRailSystem;

	internal int RailType;

	private string ComboInput;

	internal bool QueuedPress;

	private Vector3 HalfExtents = new Vector3(0.3f, 0.6f, 0.3f);

	private Vector3 BigExtents = new Vector3(0.4f, 0.8f, 0.4f);

	private Collider[] Triggers;

	private Collider[] LastTriggers = new Collider[0];

	internal List<GameObject> LaserClosestTargets = new List<GameObject>();

	internal GameObject[] LightDashRings;

	internal LayerMask Collision_Mask => LayerMask.GetMask("Ignore Raycast", "Water", "UI", "PlayerCollision", "BreakableObj", "Object/PlayerOnlyCol", "Vehicle");

	internal LayerMask FrontalCol_Mask => LayerMask.GetMask("Default", "PlayerCollision", "BreakableObj", "Object/PlayerOnlyCol", "AttackableSolid", "Vehicle");

	private LayerMask FastFrontalCol_Mask => LayerMask.GetMask("Default", "PlayerCollision", "Object/PlayerOnlyCol");

	private LayerMask JumpAttack_Mask => LayerMask.GetMask("EnemyTrigger");

	internal LayerMask Trigger_Mask => LayerMask.GetMask("TriggerCollider");

	public void SetPlayer(int ID, string Player)
	{
		IsVisible = true;
		if (!Camera)
		{
			Camera = Object.FindObjectOfType<PlayerCamera>();
		}
		Camera.PlayerBase = this;
		Camera.Target = CameraTarget;
		if (!GetPrefab("snow_board"))
		{
			CamOffset.y = ((!GetPrefab("omega")) ? (Common_Lua.c_camera.y - 0.2f) : (Common_Lua.c_camera.y + 0.2f));
		}
		else
		{
			CamOffset.y = Common_Lua.c_camera.y - (IsGrounded() ? (-0.125f) : 0.2f);
		}
		MachCamOffset.y = Common_Lua.c_camera.y - 0.2f;
		if (GetPrefab("sonic_fast"))
		{
			UpdateCameraTarget_Mach();
		}
		else
		{
			UpdateCameraTarget();
		}
		if (!HUD)
		{
			HUD = Object.FindObjectOfType<UI>();
		}
		HUD.PM = PlayerManager;
		SetUIGauge();
		if ((bool)PlayerVoice)
		{
			PlayerVoice.Player = this;
		}
		if (!StageManager)
		{
			StageManager = Camera.StageManager;
		}
		StageManager.SetPlayer(Player);
		if (!CameraFX)
		{
			CameraFX = Camera.CameraEffects;
		}
		ObjectManager[] array = Object.FindObjectsOfType<ObjectManager>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].FindNewClosestPlayer(base.transform);
		}
		PlayerNo = ID;
	}

	public virtual void StartPlayer(bool TalkStart = false)
	{
	}

	public virtual void SetStoredAttributes()
	{
		if (Singleton<GameManager>.Instance.StoredPlayerVars == null)
		{
			return;
		}
		for (int i = 0; i < Singleton<GameManager>.Instance.StoredPlayerVars.Length; i++)
		{
			if (!Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName.Contains("Shield"))
			{
				continue;
			}
			string text = Singleton<GameManager>.Instance.StoredPlayerVars[i].VarName.Split("/"[0])[1];
			int num;
			if (!(text != "sonic_new") || !(text != "sonic_fast") || !(text != "princess") || !(text != "snow_board") || !GetPrefab(text))
			{
				switch (text)
				{
				case "sonic_new":
				case "sonic_fast":
				case "princess":
				case "snow_board":
					num = ((GetPrefab("sonic_new") || GetPrefab("sonic_fast") || GetPrefab("princess") || GetPrefab("snow_board")) ? 1 : 0);
					break;
				default:
					num = 0;
					break;
				}
			}
			else
			{
				num = 1;
			}
			bool flag = (byte)num != 0;
			if (Singleton<GameManager>.Instance.StoredPlayerVars[i].VarBool && flag)
			{
				HasShield = true;
				ShieldObject = Object.Instantiate(Shield, base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f), Quaternion.identity);
				ShieldObject.transform.SetParent(base.transform);
				ShieldObject.transform.localScale = Vector3.one * ((!GetPrefab("omega")) ? 1f : 1.5f);
				ShieldObject.transform.GetChild(0).gameObject.SetActive(value: false);
				ShieldObject.GetComponent<AudioSource>().enabled = false;
			}
		}
	}

	public virtual void Awake()
	{
		_Rigidbody.sleepThreshold = 0f;
		_Rigidbody.solverIterations = 20;
		_Rigidbody.maxAngularVelocity = float.PositiveInfinity;
	}

	public virtual void Start()
	{
		UseCharacterSway = Singleton<Settings>.Instance.settings.CharacterSway == 1;
		Contacts = new List<ContactPoint>();
		ForwardMeshRotation = base.transform.forward;
		UpMeshRotation = base.transform.up;
		CanSink = true;
		Singleton<GameManager>.Instance.ResetTimeScaleAndSoundPitch();
		StageManager.BGMPlayer.mute = false;
		GenerateRotationCurve();
	}

	private void GenerateRotationCurve()
	{
		RotOverSpdCurve = new AnimationCurve();
		Prefab playerPrefab = PlayerPrefab;
		if (playerPrefab == Prefab.sonic_fast)
		{
			Keys = new Keyframe[5];
			Keys[0].time = 0f;
			Keys[0].value = 3f;
			Keys[1].time = 30f;
			Keys[1].value = 1.5f;
			Keys[2].time = 55f;
			Keys[2].value = 1f;
			Keys[3].time = 80f;
			Keys[3].value = 0.875f;
			Keys[4].time = 120f;
			Keys[4].value = 0.875f;
			for (int i = 0; i < Keys.Length; i++)
			{
				Keys[i].weightedMode = WeightedMode.Both;
				Keys[i].inWeight = 0f;
				Keys[i].outWeight = 0f;
			}
			RotOverSpdCurve.keys = Keys;
		}
		else
		{
			Keys = new Keyframe[3];
			Keys[0].time = 0f;
			Keys[0].value = 10f;
			Keys[1].time = TopSpeed;
			Keys[1].value = 4f;
			Keys[2].time = TopSpeed * 2f;
			Keys[2].value = 3f;
			RotOverSpdCurve.keys = Keys;
			RotOverSpdCurve.SmoothTangents(1, 0f);
		}
	}

	private void SaveContactPoints(Collision collision)
	{
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			Contacts.Add(collision.contacts[i]);
		}
	}

	private void ClearContactPoints()
	{
		Contacts.Clear();
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		SaveContactPoints(collision);
	}

	protected virtual void OnCollisionStay(Collision collision)
	{
		SaveContactPoints(collision);
	}

	protected virtual void OnCollisionExit(Collision collision)
	{
		SaveContactPoints(collision);
	}

	public Vector3 GetPerpendicularDirection(Vector3 SurfaceNormal)
	{
		return Vector3.Cross(Vector3.Cross(Vector3.up, SurfaceNormal).normalized, SurfaceNormal).normalized;
	}

	private void UpdateWallCollision()
	{
		WallNormal = Vector3.zero;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < Contacts.Count; i++)
		{
			ContactPoint contactPoint = Contacts[i];
			if (!(contactPoint.otherCollider == null) && !IsGrounded())
			{
				num2 = Vector3.Dot(Vector3.up, contactPoint.normal);
				if (num2 > num)
				{
					WallNormal = contactPoint.normal;
					num = num2;
				}
			}
		}
		ClearContactPoints();
	}

	public void DoWallNormal()
	{
		Vector3 velocity = _Rigidbody.velocity;
		if (velocity.y < 0f && WallNormal != Vector3.zero)
		{
			float y = velocity.y;
			velocity.y = 0f;
			Vector3 perpendicularDirection = GetPerpendicularDirection(WallNormal);
			Debug.DrawRay(base.transform.position, perpendicularDirection, Color.green);
			velocity -= perpendicularDirection * y;
			WallNormal = Vector3.zero;
		}
		_Rigidbody.velocity = velocity;
	}

	private void StateSlowFallStart()
	{
		SetState("SlowFall");
		AirVelocity = _Rigidbody.velocity;
		AirVelocity.y = 0f;
		SlowFallSpeed = Mathf.Max(WalkSpeed * 2f, CurSpeed);
		_Rigidbody.velocity = AirVelocity;
		SlowFallTime = Time.time;
	}

	public void StateSlowFall()
	{
		SetState("SlowFall");
		PlayAnimation("Air Falling", "On Air Fall");
		Vector3 vector = new Vector3(AirVelocity.x, 0f, AirVelocity.z);
		SlowFallSpeed = Mathf.Lerp(SlowFallSpeed, WalkSpeed * 2f, Time.fixedDeltaTime * 10f);
		CurSpeed = SlowFallSpeed;
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirVelocity = new Vector3(vector.x, AirVelocity.y, vector.z);
		}
		AirVelocity.y = Mathf.Lerp(AirVelocity.y, -19.62f, Time.fixedDeltaTime);
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		_Rigidbody.velocity = AirVelocity;
		DoWallNormal();
		if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			SetMachineState("StateGround");
			PlayerManager.PlayerEvents.CreateLandFXAndSound();
		}
		if (Time.time - SlowFallTime > 1f)
		{
			SetMachineState("StateAir");
		}
	}

	private void StateSlowFallEnd()
	{
	}

	private void StateGrindingStart()
	{
		SetState("Grinding");
		GrindStartTime = Time.time;
		GrindFlipTime = 0f;
		SwitchTime = 1f;
		GrindTrick = false;
		RailSys = GetRailSystem();
		GrindSplineLength = RailSys.Length();
		float[] array = GrindData(base.transform.position + base.transform.up * 0.25f, base.transform.forward);
		GrindTime = 0f;
		GrindDir = 0f;
		if (array.Length == 2)
		{
			GrindTime = array[0];
			GrindDir = array[1];
		}
		else
		{
			Vector3 vector = new Vector3(array[0], array[1], array[2]);
			Vector3 vector2 = new Vector3(array[3], array[4], array[5]);
			Vector3 vector3 = Math3D.ClosestPointOnLine(vector, vector2, base.transform.position + base.transform.up * 0.25f);
			float t = Vector3.Distance(vector, vector3) / Vector3.Distance(vector, vector2);
			GrindDir = array[6];
			float num = (float)GetRailPathData().Length - 1f;
			GrindTime = Mathf.Lerp(array[7] / num, array[8] / num, t);
			Debug.DrawLine(vector, vector2, Color.white, 10f);
			Debug.DrawLine(base.transform.position - base.transform.up * 0.25f, vector3, Color.yellow, 10f);
		}
		RailData = RailSys.GetRailData(GrindTime);
		GrindSpeed = Mathf.Max(Vector3.Dot(RailData.tangent * GrindDir, WorldVelocity.normalized), 0f) * WorldVelocity.magnitude;
		GrindSpeed = Mathf.Min(GrindSpeed, GrindSpeedMax);
		if (Normal == Vector3.zero)
		{
			Normal = (base.transform.position + base.transform.up * 0.25f - RailData.position).normalized;
		}
		PlayerManager.FXBase.CreateRailFX(RailType);
		GrindNormalTime = 0f;
		RailSwitch = false;
		GrindTilt = 0f;
		IsWindRoad = RailType == 1;
		if (IsWindRoad)
		{
			WindRoadCol = RailSys.GetComponentInChildren<Collider>();
		}
	}

	private void StateGrinding()
	{
		SetState("Grinding");
		LockControls = true;
		GrindTilt = Mathf.Lerp(GrindTilt, (!GrindTrick && !RailSwitch) ? GetRailInput() : 0f, Time.fixedDeltaTime * 7.5f);
		GrindUpdateTime = Mathf.Clamp(Time.time - GrindStartTime, 0f, 1f);
		if (GrindSpeed < GrindSpeedOrg)
		{
			GrindSpeed = Mathf.MoveTowards(GrindSpeed, GrindSpeedOrg, Time.fixedDeltaTime * 10f);
		}
		else
		{
			GrindSpeed += Vector3.Dot(new Vector3(0f, -0.3f, 0f), RailData.tangent * GrindDir);
			GrindSpeed = Mathf.Clamp(GrindSpeed, 0f, GrindSpeedMax);
		}
		CurSpeed = GrindSpeed;
		GrindTime += GrindSpeed / GrindSplineLength * GrindDir * Time.fixedDeltaTime;
		if (GrindTime > 1f || GrindTime < 0f)
		{
			GrindDir *= ((GrindSpeed < 0f) ? (-1f) : 1f);
			_Rigidbody.velocity = RailData.tangent * GrindDir * GrindSpeed;
			if (!GetPrefab("snow_board"))
			{
				StateMachine.ChangeState(StateSlowFall);
			}
			else
			{
				SetMachineState("StateAir");
			}
		}
		if (IsWindRoad && !WindRoadCol.enabled)
		{
			WindRoadCol = null;
			SetMachineState("StateAir");
		}
		RailData = RailSys.GetRailData(GrindTime);
		GrindNormalTime += Mathf.Lerp(GrindNormalTime, 1f, Time.fixedDeltaTime / 0.3f * Mathf.Lerp(2f, 0.5f, GrindUpdateTime));
		Normal = Vector3.Slerp(Normal, RailData.normal, Mathf.Min(GrindNormalTime, 1f)).normalized;
		base.transform.rotation = Quaternion.LookRotation(RailData.tangent * GrindDir, Normal);
		_Rigidbody.velocity = Vector3.zero;
		if (RailSwitch)
		{
			GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up) * Quaternion.Euler(0f, 0f, 0f);
			SwitchTime += Time.fixedDeltaTime;
			if (SwitchTime > 1f)
			{
				SwitchTime = 1f;
			}
			float t = SwitchTime / 1f;
			SwitchPos = Vector3.Lerp(SwitchPos + base.transform.up * 0.25f, RailData.position - base.transform.up * 0.125f, t);
			_Rigidbody.MovePosition(SwitchPos);
		}
		else
		{
			GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
			_Rigidbody.MovePosition(RailData.position + base.transform.up * 0.25f);
		}
		if (!GetPrefab("snow_board"))
		{
			if (GrindTrick)
			{
				PlayAnimation("Grind Trick", "On Grind Trick");
			}
			else if (RailSwitch)
			{
				if (!LeftSwitching)
				{
					PlayAnimation("Grind Switch Right", "On Grind Switch");
				}
				else
				{
					PlayAnimation("Grind Switch Left", "On Grind Switch");
				}
			}
			else
			{
				PlayAnimation("Grind", "On Grind");
			}
		}
		else
		{
			PlayAnimation("Grind", "On Grind");
		}
		if (GrindTrick && Time.time - GrindFlipTime > 1f)
		{
			PlayAnimation("Grind", "On Grind");
			GrindTrick = false;
		}
		if (RailSwitch && SwitchTime > 0.425f)
		{
			PlayerManager.FXBase.CreateRailContactFX(RailType);
			RailSwitch = false;
		}
	}

	private void StateGrindingEnd()
	{
		PlayerManager.FXBase.DestroyRailFX();
		GrindTilt = 0f;
	}

	private void StateDeathStart()
	{
		SetState("Death");
		DeathTime = Time.time;
		PlayAnimation("Death Normal", "On Death Ground");
		if (!GetPrefab("metal_sonic"))
		{
			if (GetPrefab("princess"))
			{
				PlayerVoice.Play(9, RandomPlayChance: false, RandomMulticast: true, Uncut: true);
			}
			else
			{
				PlayerVoice.Play(9);
			}
		}
		StartCoroutine(RestartStage());
	}

	private void StateDeath()
	{
		SetState("Death");
		if (Time.time - DeathTime < 0.3f)
		{
			PlayAnimation("Death Normal", "On Death Ground");
		}
		LockControls = true;
		_Rigidbody.velocity = Vector3.zero;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
	}

	private void StateDeathEnd()
	{
	}

	private void StateFallDeathStart()
	{
		SetState("FallDeath");
		if (!GetPrefab("metal_sonic"))
		{
			if (GetPrefab("princess"))
			{
				PlayerVoice.Play(GetPrefab("omega") ? 9 : 0, RandomPlayChance: false, RandomMulticast: false, Uncut: true);
			}
			else
			{
				PlayerVoice.Play(GetPrefab("omega") ? 9 : 0);
			}
		}
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		AirVelocity = _Rigidbody.velocity;
		if (FallDeathType == 1)
		{
			CurSpeed = 0f;
			AirVelocity.y = 0f;
			PlayAnimation("Flip Death Fall", "On Flip Death Air");
		}
		_Rigidbody.velocity = AirVelocity;
		StartCoroutine(RestartStage());
	}

	private void StateFallDeath()
	{
		SetState("FallDeath");
		LockControls = true;
		Vector3 vector = new Vector3(AirVelocity.x, 0f, AirVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = (GetPrefab("snow_board") ? _Rigidbody.velocity.normalized : base.transform.forward) * CurSpeed;
			AirVelocity = new Vector3(vector.x, AirVelocity.y, vector.z);
		}
		CurSpeed = Mathf.MoveTowards(CurSpeed, 0f, Time.fixedDeltaTime * ((!IsGrounded() && ShouldAlignOrFall(Align: false)) ? 10f : 20f));
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, (IsGrounded() && ShouldAlignOrFall(Align: false)) ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (FallDeathType == 0)
		{
			PlayAnimation((IsGrounded() && ShouldAlignOrFall(Align: false)) ? "Death Lay" : "Death Fall", (IsGrounded() && ShouldAlignOrFall(Align: false)) ? "On Death Ground" : "On Death Air");
		}
		else if (IsGrounded() && ShouldAlignOrFall(Align: false))
		{
			PlayAnimation("Death Lay", "On Death Ground");
		}
		if (!IsGrounded())
		{
			AirVelocity.y -= 25f * Time.fixedDeltaTime;
		}
		else
		{
			AirVelocity.y = 0f;
		}
		AirVelocity.y = LimitVel(AirVelocity.y);
		_Rigidbody.velocity = AirVelocity;
		DoWallNormal();
	}

	private void StateFallDeathEnd()
	{
	}

	private void StateDrownDeathStart()
	{
		SetState("DrownDeath");
		if (!GetPrefab("metal_sonic"))
		{
			if (GetPrefab("princess"))
			{
				PlayerVoice.Play(GetPrefab("omega") ? 9 : 0, RandomPlayChance: false, RandomMulticast: false, Uncut: true);
			}
			else
			{
				PlayerVoice.Play(GetPrefab("omega") ? 9 : 0);
			}
		}
		AirVelocity = _Rigidbody.velocity;
		if (AirVelocity.y > 0f)
		{
			AirVelocity.y = 0f;
		}
		_Rigidbody.velocity = AirVelocity;
		StartCoroutine(RestartStage());
	}

	private void StateDrownDeath()
	{
		SetState("DrownDeath");
		LockControls = true;
		Vector3 vector = new Vector3(AirVelocity.x, 0f, AirVelocity.z);
		if (_Rigidbody.velocity.magnitude != 0f)
		{
			vector = base.transform.forward * CurSpeed;
			AirVelocity = new Vector3(vector.x, AirVelocity.y, vector.z);
		}
		CurSpeed = Mathf.Lerp(CurSpeed, 0f, Time.fixedDeltaTime * ((!IsGrounded()) ? 3f : 12f));
		PlayAnimation(IsGrounded() ? "Death Lay" : "Death Drown", IsGrounded() ? "On Death Ground" : "On Death Air");
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, IsGrounded() ? RaycastHit.normal : Vector3.up) * base.transform.rotation;
		if (!IsGrounded())
		{
			AirVelocity.y = Mathf.Lerp(AirVelocity.y, -5f, 10f * Time.fixedDeltaTime);
		}
		else
		{
			AirVelocity.y = 0f;
		}
		AirVelocity.y = LimitVel(AirVelocity.y);
		_Rigidbody.velocity = AirVelocity;
		DoWallNormal();
	}

	private void StateDrownDeathEnd()
	{
	}

	internal void StateTalkStart()
	{
		SetState("Talk");
		TalkTime = Time.time;
		LockControls = true;
		PlayAnimation("Talk Start", "On Talk");
		Camera.StateMachine.ChangeState(Camera.StateTalk);
	}

	internal void StateTalk()
	{
		SetState("Talk");
		LockControls = true;
		CurSpeed = 0f;
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, RaycastHit.normal) * base.transform.rotation;
		_Rigidbody.velocity = Vector3.zero;
		if (Time.time - TalkTime > 1f)
		{
			Camera.OnPlayerTalkEnd();
			SetMachineState("StateGround");
		}
		else if (Camera.CameraState != PlayerCamera.State.Talk)
		{
			Camera.StateMachine.ChangeState(Camera.StateTalk);
		}
	}

	internal void StateTalkEnd()
	{
	}

	private void StatePathStart()
	{
		SetState("Path");
		if (GetPrefab("snow_board"))
		{
			CurSpeed = _Rigidbody.velocity.magnitude;
		}
		PathSpeed = CurSpeed;
		PathMoveDir = ((Vector3.Dot(LinearBezier.GetTangent(PathTime), base.transform.forward) >= 0f) ? 1 : (-1));
		MaxRayLenght = 1.75f;
	}

	private void StatePath()
	{
		SetState("Path");
		if (!GetPrefab("snow_board") || (GetPrefab("snow_board") && !PlayerManager.snow_board.Board))
		{
			PlayAnimation("Movement (Blend Tree)", "On Ground");
		}
		else
		{
			PlayAnimation("Board", "On Board");
		}
		LockControls = true;
		TargetDirection = Vector3.zero;
		if (PathSpeed < TopSpeed)
		{
			PathSpeed += TopSpeed * Time.fixedDeltaTime;
		}
		if (PathSpeed >= TopSpeed && PathSpeed < TopSpeed * 2f && !PathDashpanel && !GetPrefab("sonic_fast"))
		{
			PathSpeed += Vector3.Dot(new Vector3(0f, -0.5f, 0f), LinearBezier.GetTangent(PathTime).normalized * PathMoveDir);
		}
		CurSpeed = PathSpeed;
		PathTime += PathSpeed / LinearBezier.Length() * Time.fixedDeltaTime * (float)PathMoveDir;
		PathTime = Mathf.Clamp01(PathTime);
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
			SetMachineState("StateGround");
			base.transform.forward = LinearBezier.GetTangent(PathTime).normalized * PathMoveDir;
			GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
			if (GetPrefab("snow_board"))
			{
				_Rigidbody.velocity = base.transform.forward * CurSpeed;
			}
			MaxRayLenght = 0.75f;
			PositionToPoint();
		}
	}

	private void StatePathEnd()
	{
		base.transform.forward = LinearBezier.GetTangent(PathTime).normalized * PathMoveDir;
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
		if (GetPrefab("snow_board"))
		{
			_Rigidbody.velocity = base.transform.forward * CurSpeed;
		}
		MaxRayLenght = 0.75f;
		PositionToPoint();
	}

	private void StateWarpHoleStart()
	{
		WarpTime = Time.time;
		SetCameraParams(new CameraParameters(30, Camera.transform.position, WarpPos));
		Camera.UncancelableEvent = true;
		WarpAppear = false;
	}

	public void StateWarpHole()
	{
		SetState("WarpHole");
		LockControls = true;
		PlayAnimation((!GetPrefab("princess")) ? "Float" : "Air Falling", (!GetPrefab("princess")) ? "On Float" : "On Air Fall");
		float num = Time.time - WarpTime;
		float num2 = Mathf.Clamp01(Time.time - WarpTime) / 3f;
		num2 *= num2;
		if (WarpType == 0 || (WarpType == 1 && !WarpAppear))
		{
			base.transform.position = Vector3.Lerp(base.transform.position + Vector3.up * 0.05f, WarpPos, num2);
		}
		GeneralMeshRotation = Quaternion.LookRotation(ForwardMeshRotation, UpMeshRotation);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		BodyTransform.localScale = Vector3.Lerp(BodyTransform.localScale, WarpAppear ? Vector3.one : Vector3.zero, num2);
		if ((bool)ShieldObject)
		{
			ShieldObject.transform.localPosition = Vector3.Lerp(ShieldObject.transform.localPosition, WarpAppear ? (base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f)) : Vector3.zero, num2);
			ShieldObject.transform.localScale = Vector3.Lerp(ShieldObject.transform.localScale, WarpAppear ? (Vector3.one * ((!GetPrefab("omega")) ? 1f : 1.5f)) : Vector3.zero, num2);
		}
		float num3 = Mathf.PerlinNoise(base.transform.position.x * 0.1f, base.transform.position.z * 0.1f) * 2f - 1f;
		float num4 = Mathf.PerlinNoise(base.transform.position.z * 0.1f, base.transform.position.y * 0.1f) * 2f - 1f;
		float num5 = Mathf.PerlinNoise(base.transform.position.z * 0.1f, base.transform.position.y * 0.1f) * 2f - 1f;
		Quaternion quaternion = Quaternion.AngleAxis(num3 * 25f, Vector3.left);
		Quaternion quaternion2 = Quaternion.AngleAxis(num4 * 25f, Vector3.up);
		Quaternion quaternion3 = Quaternion.AngleAxis(num5 * 25f, Vector3.forward);
		BodyTransform.rotation *= quaternion * quaternion2 * quaternion3;
		_Rigidbody.velocity = Vector3.zero;
		CurSpeed = 0f;
		if (WarpType == 1 && num >= 2.5f && !WarpAppear)
		{
			WarpTime = Time.time;
			WarpAppear = true;
		}
		if (WarpAppear && Time.time - WarpTime >= 2.5f)
		{
			Camera.UncancelableEvent = false;
			Camera.StateMachine.ChangeState(Camera.StateEventFadeOut);
			SetMachineState("StateAir");
		}
	}

	private void StateWarpHoleEnd()
	{
		BodyTransform.localRotation = Quaternion.identity;
		BodyTransform.localScale = Vector3.one;
	}

	private void StateResultStart()
	{
		SetState("Result");
		PlayAnimation("GoalPose Start", "On Results");
		PlayAnimation("Movement (Blend Tree)", "On Ground");
		ResultTime = Time.time;
		base.transform.position = PlayerGoal.transform.position + base.transform.up * 0.25f;
		base.transform.rotation = PlayerGoal.transform.rotation;
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
	}

	private void StateResult()
	{
		SetState("Result");
		LockControls = true;
		CurSpeed = 0f;
		base.transform.position = PlayerGoal.transform.position + base.transform.up * 0.25f;
		base.transform.rotation = PlayerGoal.transform.rotation;
		GeneralMeshRotation = Quaternion.LookRotation(base.transform.forward, base.transform.up);
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateResultEnd()
	{
	}

	public virtual void FixedUpdate()
	{
		UpdateTrigger();
		UpdateWallCollision();
		if (StageManager._Stage != StageManager.Stage.twn)
		{
			ComboUpdate();
		}
		IsOnWall = !(Vector3.Dot(base.transform.up, Vector3.up) >= GroundDot);
		GroundNormal = RaycastHit.normal;
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.25f, base.transform.forward, out FrontalHit, 0.325f, (!GetPrefab("sonic_fast")) ? FrontalCol_Mask : FastFrontalCol_Mask))
		{
			Debug.DrawLine(base.transform.position + base.transform.up * 0.25f, FrontalHit.point, Color.red);
			FrontalCollision = true;
			if (!GetPrefab("sonic_fast"))
			{
				CurSpeed = 0f;
			}
		}
		else
		{
			FrontalCollision = false;
		}
		if (HasInvincibility)
		{
			JumpAttackSphere(base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f), (!GetPrefab("omega")) ? 0.5f : 1f, base.transform.forward * CurSpeed, 1);
		}
		WorldVelocity = _Rigidbody.velocity;
		if (WorldVelocity == Vector3.zero)
		{
			WorldVelocity = (base.transform.position - LastPosition) / Time.fixedDeltaTime;
			LastPosition = base.transform.position;
		}
		UpdateEnvironmentalHazards();
	}

	public virtual void Update()
	{
		if (GetPrefab("sonic_fast"))
		{
			UpdateCameraTarget_Mach();
		}
		else
		{
			UpdateCameraTarget();
		}
		UpdateAnimations();
		if ((HasInvincibility && Time.time - InvincibleTime > Common_Lua.c_invincible_item) || Singleton<GameManager>.Instance.GameState == GameManager.State.Result)
		{
			HasInvincibility = false;
			if (!HasSpeedUp)
			{
				StageManager.BGMPlayer.mute = false;
			}
			Object.Destroy(InvStarsObject);
		}
		if ((HasSpeedUp && Time.time - SpeedUpTime > Common_Lua.c_speedup_time) || Singleton<GameManager>.Instance.GameState == GameManager.State.Result)
		{
			HasSpeedUp = false;
			if (!HasInvincibility)
			{
				StageManager.BGMPlayer.mute = false;
			}
			Object.Destroy(SpeedUpObject);
		}
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && StageManager.StageState != StageManager.State.Event && !IsDead && GetState() != "Talk")
		{
			if (GetState() == "Grinding" && !GetPrefab("snow_board"))
			{
				if ((GetRailInput() == 0f || (GetRailInput() < 0f && !CanChangeRail(Invert: true)) || (GetRailInput() > 0f && !CanChangeRail())) && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && GrindUpdateTime > 0.1f && !RailSwitch)
				{
					SetMachineState("StateJump");
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button X") && !GrindTrick && !RailSwitch)
				{
					GrindSpeed = Mathf.Min(GrindSpeed + GrindAcc * 0.75f, GrindSpeedMax);
					GrindTrick = true;
					GrindFlipTime = Time.time;
					Audio.PlayOneShot(GrindWind, Audio.volume);
				}
				if (((GetRailInput() < 0f && CanChangeRail(Invert: true)) || (GetRailInput() > 0f && CanChangeRail())) && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !GrindTrick && !RailSwitch)
				{
					OnChangeRail(GetRailInput() < 0f);
					RailSwitch = true;
				}
			}
			if (GetState() == "Path" && PathJumpOff && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				SetMachineState("StateJump");
			}
		}
		ForwardMeshRotation = Vector3.Slerp(ForwardMeshRotation, base.transform.forward, Time.deltaTime * (IsGrounded() ? 50f : 15f));
		UpMeshRotation = Vector3.Slerp(UpMeshRotation, IsGrounded() ? base.transform.up : Vector3.up, Time.deltaTime * 10f);
		Mesh.rotation = GeneralMeshRotation;
	}

	public virtual string GetState()
	{
		return "null";
	}

	public virtual void SetMachineState(string StateName)
	{
	}

	public virtual void SetState(string StateName)
	{
	}

	public void PlayAnimation(string AnimState, string TriggerName)
	{
		if (!Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimState))
		{
			Animator.SetTrigger(TriggerName);
		}
	}

	public void PlayIdleEvent(int MaxIndex)
	{
		Animator.SetTrigger("On Idle Wait");
		Animator.SetInteger("Idle Anim ID", IdleAnim);
		TriggerIndex++;
		if (TriggerIndex == 3)
		{
			if ((PlayerVoice.WaitVoiceClip.Count != 0 || (PlayerVoice.WaitVoiceClip.Count == 0 && (bool)PlayerVoice.MulticastTo && PlayerVoice.MulticastTo.WaitVoiceClip.Count != 0)) && !HasWaitQuote)
			{
				PlayerVoice.PlayWait();
				HasWaitQuote = true;
			}
			else
			{
				string clipName = "hint_all01_w0" + IdleIndex + "_" + PlayerNameShort;
				PlayerVoice.PlayGlobalWait(clipName);
				IdleIndex++;
			}
			if (IdleIndex > 1)
			{
				HasWaitQuote = false;
				IdleIndex = 0;
			}
			TriggerIndex = 0;
		}
		IdleAnim++;
		if (IdleAnim > MaxIndex)
		{
			IdleAnim = 0;
		}
	}

	public virtual void UpdateAnimations()
	{
		Animator.SetFloat("Speed", CurSpeed);
		if (GetState() != "Vehicle")
		{
			Animator.SetFloat("Y Vel", _Rigidbody.velocity.y);
		}
		if (!GetPrefab("princess") && !GetPrefab("snow_board") && !GetPrefab("omega"))
		{
			Animator.SetFloat("Jump Animation", (!IsGrounded()) ? Mathf.MoveTowards(Animator.GetFloat("Jump Animation"), JumpAnimation, Time.deltaTime * 10f) : 0f);
		}
		if (!GetPrefab("sonic_fast") && !GetPrefab("snow_board"))
		{
			Animator.SetFloat("Run Walk Animation", RunWalkAnimation);
			RunWalkAnimation = Mathf.MoveTowards(RunWalkAnimation, (WalkSwitch && !LockControls) ? 1f : 0f, Time.deltaTime * 4f);
			Animator.SetFloat("Grind Tilt", GrindTilt);
		}
		Animator.SetBool("Is Sinking", IsSinking && !LockSink && GetState() != "Hurt" && IsGrounded());
	}

	private void ComboUpdate()
	{
		if (ComboState == 1)
		{
			if (!TrickCombo)
			{
				ComboBonus = (int)(Mathf.Round((float)ComboScoreDiff * ((float)(ComboLevel + 1) / 50f * ComboMaxMultiplier) / 100f) * 100f);
			}
			else
			{
				NewComboBonus = (int)(Mathf.Round((float)NewComboScoreDiff * ((float)(NewComboLevel + 1) / 50f * ComboMaxMultiplier) / 100f) * 100f);
			}
			if ((GetState() == "Ground" && IsGrounded() && Time.time - ComboTime > 1f) || GetState() == "RainbowRing" || GetState() == "RampJump" || Time.time - ComboTime > 4f)
			{
				if (((!TrickCombo) ? ComboScoreDiff : NewComboScoreDiff) >= ComboMinScore)
				{
					Singleton<GameManager>.Instance._PlayerData.score += ((!TrickCombo) ? ComboBonus : 0);
					HUD.OnCombo((!TrickCombo) ? ComboBonus : NewComboBonus, ((!TrickCombo) ? ComboLevel : NewComboLevel) - 1);
				}
				ComboLevel = 0;
				ComboState = 0;
				ComboScoreDiff = 0;
				if (!TrickCombo)
				{
					NewComboLevel = 0;
					NewComboScoreDiff = 0;
				}
			}
		}
		if ((GetState() == "Ground" && IsGrounded() && Time.time - ComboTime > 1f) || GetState() == "Hurt")
		{
			ComboLevel = 0;
			ComboState = 0;
			ComboScoreDiff = 0;
			NewComboLevel = 0;
			NewComboScoreDiff = 0;
		}
		if (ComboState == 2 && Time.time - ComboTime > 4f)
		{
			ComboState = 0;
		}
	}

	public void AddScore(int Score, bool InstantChain = false, int CustomLevel = 429496)
	{
		if (StageManager._Stage == StageManager.Stage.twn)
		{
			return;
		}
		Singleton<GameManager>.Instance._PlayerData.score += ((!InstantChain) ? Score : 0);
		TrickCombo = InstantChain;
		ComboTime = Time.time;
		if (!InstantChain)
		{
			ComboScoreDiff += Score;
			if (ComboScoreDiff > Score && ComboScoreDiff > 99 && Score > 99)
			{
				ComboLevel++;
				ComboState = 1;
			}
			return;
		}
		if (NewComboLevel < 1)
		{
			NewComboScoreDiff = Score * 3;
			NewComboLevel = 1;
		}
		else
		{
			NewComboScoreDiff += Score;
			NewComboLevel += 2;
		}
		if (CustomLevel != 429496)
		{
			NewComboLevel = CustomLevel;
		}
		ComboBonus = (int)(Mathf.Round((float)NewComboScoreDiff * ((float)(NewComboLevel + 1) / 50f * ComboMaxMultiplier) / 100f) * 100f);
		Singleton<GameManager>.Instance._PlayerData.score += ComboBonus;
		ComboState = 1;
	}

	public void AddRing(int Amount = 1, AudioSource ThisRingSource = null)
	{
		GameManager.PlayerData playerData = Singleton<GameManager>.Instance._PlayerData;
		playerData.rings += Amount;
		if (playerData.rings > playerData.maxCollectedRings)
		{
			if (playerData.rings / 100 - playerData.maxCollectedRings / 100 > 0)
			{
				AddLife();
				SetUIItemBox(4);
			}
			playerData.maxCollectedRings = playerData.rings;
		}
		Singleton<GameManager>.Instance._PlayerData = playerData;
		HUD.RingAnimator.SetTrigger("On Play");
		if ((bool)ThisRingSource && LastRingSource != ThisRingSource)
		{
			if ((bool)LastRingSource)
			{
				LastRingSource.Stop();
			}
			LastRingSource = ThisRingSource;
		}
	}

	public void AddLife()
	{
		Singleton<AudioManager>.Instance.PlayClip(OneUPSounds[Singleton<Settings>.Instance.settings.E3XBLAMusic], 1.25f);
		if (!GetPrefab("omega"))
		{
			if (GetPrefab("princess"))
			{
				PlayerVoice.PlayRandom(new int[2] { 10, 11 }, RandomPlayChance: true, RandomMulticast: true);
			}
			else
			{
				PlayerVoice.PlayRandom(new int[2] { 10, 11 }, RandomPlayChance: true);
			}
		}
		else
		{
			PlayerVoice.Play(10, RandomPlayChance: true);
		}
		GameData.StoryData storyData = Singleton<GameManager>.Instance.GetStoryData();
		storyData.Lives++;
		Singleton<GameManager>.Instance.SetStoryData(storyData);
		HUD.LifeAnimator.SetTrigger("On Play");
	}

	public virtual void SetUIGauge()
	{
	}

	public void SetUIItemBox(int Index)
	{
		if (Singleton<Settings>.Instance.settings.ItemBoxType == 0)
		{
			HUD.ItemDisplay.GotItemBox(Index);
		}
	}

	public void GrantPowerUp(string Type)
	{
		switch (Type)
		{
		case "Shield":
			if (!HasShield)
			{
				HasShield = true;
				ShieldObject = Object.Instantiate(Shield, base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f), Quaternion.identity);
				ShieldObject.transform.SetParent(base.transform);
				ShieldObject.transform.localScale = Vector3.one * ((!GetPrefab("omega")) ? 1f : 1.5f);
			}
			break;
		case "Invincible":
			if (!HasInvincibility)
			{
				HasInvincibility = true;
				if (HasSpeedUp)
				{
					Object.Destroy(SpeedUpObject.GetComponent<AudioSource>());
				}
				InvincibleTime = Time.time;
				StageManager.BGMPlayer.mute = true;
				InvStarsObject = Object.Instantiate(InvincibleStars, base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f), Quaternion.identity);
				InvStarsObject.transform.SetParent(base.transform);
				InvStarsObject.transform.localScale = Vector3.one * ((!GetPrefab("omega")) ? 1f : 1.5f);
			}
			break;
		case "SpeedUp":
			if (!HasSpeedUp)
			{
				HasSpeedUp = true;
				if (HasInvincibility)
				{
					Object.Destroy(InvStarsObject.GetComponent<AudioSource>());
				}
				SpeedUpTime = Time.time;
				StageManager.BGMPlayer.mute = true;
				SpeedUpObject = Object.Instantiate(SpeedUp, base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.25f : 0.5f), Quaternion.identity);
				SpeedUpObject.transform.SetParent(base.transform);
				SpeedUpObject.transform.localScale = Vector3.one * ((!GetPrefab("omega")) ? 1f : 1.5f);
			}
			break;
		}
	}

	public void RemoveShield()
	{
		if (HasShield)
		{
			HasShield = false;
			Object.Destroy(ShieldObject);
		}
	}

	public void OnPathEnter(PathSystem PathSys, bool CanJumpOff, float YOffset)
	{
		if (!(GetState() != "Path") || !(GetState() != "Vehicle"))
		{
			return;
		}
		float num = PathSys.PathDist(base.transform.position, base.transform.forward);
		if (num != -1f)
		{
			PathSystem = PathSys;
			PathData = PathSystem.BuildPathData(num);
			PathJumpOff = CanJumpOff;
			PathYOffset = YOffset;
			LinearBezier = new LinearBez(PathData.position);
			PathTime = LinearBezier.GetTime(PathSystem.FindClosestPoint(PathData, base.transform.position, base.transform.forward));
			if (PathTime > 0.001f && PathTime < 0.999f)
			{
				StateMachine.ChangeState(StatePath);
			}
		}
	}

	public void OnWarpHoleEnter(int Type, Vector3 Pos)
	{
		WarpType = Type;
		WarpPos = Pos;
		StateMachine.ChangeState(StateWarpHole);
	}

	public virtual void OnHurtEnter(int HurtType = 0)
	{
		if (GetState() == "Hold" || GetState() == "UpReel" || GetState() == "Tarzan" || GetState() == "Orca")
		{
			_Rigidbody.isKinematic = false;
			_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			base.transform.SetParent(null);
		}
	}

	public virtual void OnBulletHit(Vector3 Direction, float Rate = 1f, int DeathType = 0)
	{
		if (ImmunityTime - Time.time <= 0f && GetState() != "Hurt" && GetState() != "Result" && !HasShield && !HasInvincibility && !IsInvulnerable(1))
		{
			if (Singleton<GameManager>.Instance._PlayerData.rings > 0)
			{
				Direction = Direction.MakePlanar();
				Direction.x += Random.Range(-0.15f, 0.15f);
				Direction.z += Random.Range(-0.15f, 0.15f);
				Direction.y += 1f;
				Direction.Normalize();
				LostRing component = Object.Instantiate(LostRingPrefab, base.transform.position + Direction, Quaternion.identity).GetComponent<LostRing>();
				component._Rigidbody.AddForce(Direction * 8f, ForceMode.VelocityChange);
				Singleton<AudioManager>.Instance.PlayClip(component.ClipPool[1]);
				ImmunityTime = Time.time + 0.5f / Rate;
				Singleton<GameManager>.Instance._PlayerData.rings--;
			}
			else
			{
				OnDeathEnter(DeathType);
			}
		}
	}

	public virtual void OnDeathEnter(int DeathType)
	{
	}

	public virtual void OnWaterSlideEnter(string Spline = "", bool TriggerState = true, float Speed = 0f)
	{
	}

	public virtual int AttackLevel()
	{
		return -1;
	}

	public virtual bool IsInvulnerable(int HurtType)
	{
		return AttackLevel() >= HurtType;
	}

	public void DoLandAnim()
	{
		if (CurSpeed <= 0f && _Rigidbody.velocity.y < -10f && !GetPrefab("sonic_fast") && !GetPrefab("snow_board"))
		{
			Animator.SetTrigger("On Land");
		}
	}

	public void DoDeath(int DeathType)
	{
		switch (DeathType)
		{
		case 0:
			if (IsGrounded() && ShouldAlignOrFall(Align: false))
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

	public IEnumerator RestartStage()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		bool DoDeathState = false;
		while (Timer <= 3f)
		{
			Timer = Time.time - StartTime;
			if (Timer > 2f)
			{
				HUD.PlayFadeOut();
			}
			yield return new WaitForFixedUpdate();
		}
		if (!DoDeathState)
		{
			Singleton<GameManager>.Instance.OnPlayerDeath();
		}
	}

	internal void AccelerationSystem(float AccFloat)
	{
		bool flag = IsSlopePhys || (!IsSlopePhys && Angle > SlopeFactor && CurSpeed > WalkSpeed);
		if (!FrontalCollision && TargetDirection.magnitude != 0f)
		{
			if (CurSpeed > MaximumSpeed)
			{
				CurSpeed = Mathf.MoveTowards(CurSpeed, MaximumSpeed, Time.fixedDeltaTime * (flag ? 5f : (IsGrounded() ? 20f : 35f)));
			}
			else
			{
				CurSpeed += AccFloat * Time.fixedDeltaTime;
			}
		}
		else if (CurSpeed > 0f)
		{
			CurSpeed -= BrakeSpeed * Time.fixedDeltaTime * 1.5f;
		}
		if (CurSpeed <= 0f)
		{
			CurSpeed = 0f;
		}
		if (Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.1f && CurSpeed > 0f)
		{
			CurSpeed -= BrakeSpeed * Time.fixedDeltaTime * (IsGrounded() ? 2f : 1.5f);
		}
	}

	internal void SlopePhysics(bool Use = false)
	{
		if (BrakeSpdStore == 0f)
		{
			BrakeSpdStore = BrakeSpeed;
		}
		if (Use)
		{
			Angle = Vector3.Angle(Vector3.up, base.transform.forward) - 90f;
			float num = Vector3.Dot(base.transform.up, Vector3.up);
			float num2 = 0.85f;
			float y = -0.325f;
			Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.localEulerAngles.y : 1f));
			Vector3 vector = quaternion * Vector3.forward;
			vector.y = 0f;
			vector.Normalize();
			Vector3 vector2 = quaternion * Vector3.right;
			vector2.y = 0f;
			vector2.Normalize();
			Vector3 normalized = (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * vector2 + Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * vector).normalized;
			normalized = Quaternion.FromToRotation(Vector3.up, base.transform.up) * normalized;
			IsApplyDrag = Angle < 0f - SlopeFactor || Angle > SlopeFactor;
			Vector3 vector3 = new Vector3(1f, 0f, 0f);
			Vector3 vector4 = new Vector3(base.transform.up.x, 0f, base.transform.up.z);
			vector3 = Vector3.Cross(base.transform.up, Vector3.Cross(base.transform.up, -vector4)).normalized;
			if (normalized != Vector3.zero)
			{
				if (IsApplyDrag && CurSpeed > WalkSpeed && CurSpeed < MaximumSpeed * 2f)
				{
					CurSpeed += Vector3.Dot(new Vector3(0f, y, 0f), base.transform.forward);
				}
			}
			else if (num > 0f && num < num2)
			{
				TargetDirection = vector3;
				if (CurSpeed > WalkSpeed && CurSpeed < MaximumSpeed * 2f)
				{
					CurSpeed += Vector3.Dot(new Vector3(0f, y, 0f), base.transform.forward);
				}
			}
			float num3 = Angle / 90f;
			float b = Mathf.Lerp(BrakeSpdStore, BrakeSpdStore * 0.25f, num3 * ((Angle / 90f < 0f) ? (-1f) : 1f));
			BrakeSpeed = Mathf.Lerp(BrakeSpeed, b, Time.fixedDeltaTime * 5f);
			IsSlopePhys = normalized == Vector3.zero && num > 0f && num < num2;
		}
		else
		{
			if (BrakeSpeed != BrakeSpdStore)
			{
				BrakeSpeed = BrakeSpdStore;
			}
			Angle = 0f;
			IsSlopePhys = false;
			IsApplyDrag = false;
		}
	}

	internal float LimitVel(float Result, float Positive = 0f)
	{
		Result = Mathf.Clamp(Result, 0f - Common_Lua.c_vel_y_max, (Positive == 0f) ? Result : Positive);
		return Result;
	}

	internal void RotatePlayer(float AirRotSpeed, bool Override = false, bool DontLockOnAir = false, float OverrideValue = 0f)
	{
		float axis = Singleton<RInput>.Instance.P.GetAxis("Left Stick X");
		float axis2 = Singleton<RInput>.Instance.P.GetAxis("Left Stick Y");
		float num = Mathf.Clamp(Mathf.Abs(axis) + Mathf.Abs(axis2), 0f, 1f);
		Quaternion quaternion = Quaternion.Euler(Vector3.up * ((Camera != null) ? Camera.transform.localEulerAngles.y : 1f));
		Vector3 vector = quaternion * Vector3.forward;
		vector.y = 0f;
		vector.Normalize();
		Vector3 vector2 = quaternion * Vector3.right;
		vector2.y = 0f;
		vector2.Normalize();
		if (!IsSlopePhys)
		{
			TargetDirection = (axis * vector2 + axis2 * vector).normalized;
			TargetDirection = Quaternion.FromToRotation(Vector3.up, base.transform.up) * TargetDirection;
		}
		WalkSwitch = (IsSlopePhys ? (CurSpeed <= WalkSpeed) : (TargetDirection != Vector3.zero && num < 0.75f && !IsOnWall && !LockControls));
		if (TargetDirection != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(TargetDirection, RaycastHit.normal);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, (Override ? OverrideValue : (IsGrounded() ? RotOverSpdCurve.Evaluate(CurSpeed) : ((Vector3.Dot(TargetDirection, _Rigidbody.velocity.normalized) < -0.75f && !DontLockOnAir && !LockControls) ? 0f : (AirRotSpeed * 0.75f)))) * Time.fixedDeltaTime);
		}
	}

	private void UpdateEnvironmentalHazards()
	{
		ColName = (RaycastHit.transform ? RaycastHit.transform.gameObject.name : "");
		if (CanSink && IsGrounded() && !HasInvincibility && (ColName == "40000009" || ColName == "2820000d"))
		{
			if (!IsSinking)
			{
				CurSpeed *= 0.5f;
				SinkStartTime = Time.time;
				CanSinkJump = false;
				IsSinking = true;
			}
		}
		else
		{
			IsSinking = false;
		}
		IsSinking = CanSink && IsGrounded() && !HasInvincibility && (ColName == "40000009" || ColName == "2820000d");
		if (IsSinking)
		{
			if (GetState() == "BoundAttack" || GetState() == "SpinDash" || GetState() == "PsychoShock" || GetState() == "Quake" || GetState() == "KickDive")
			{
				SetMachineState("StateGround");
			}
			if (!LockSink && GetState() != "Hurt")
			{
				SinkPosition += Time.fixedDeltaTime / 1.5f;
			}
			if (!CanSinkJump && Time.time - SinkStartTime > 0.15f)
			{
				CanSinkJump = true;
			}
			if (SinkPosition >= 1f)
			{
				if (ColName == "40000009")
				{
					PlayerManager.FXBase.RemoveSinkParticles("Sand");
				}
				SinkPosition = 1f;
				if (GetState() != "DrownDeath" && GetState() != "Death" && GetState() != "FallDeath")
				{
					OnDeathEnter(1);
				}
			}
			else if (ColName == "2820000d")
			{
				if (HasShield)
				{
					OnHurtEnter();
				}
				OnBulletHit(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 5f);
			}
			CurSpeed *= 1f - SinkPosition * 0.1f;
		}
		else
		{
			SinkPosition = 0f;
		}
		if (FrontalCollision && FrontalHit.transform.gameObject.name == "2820000d")
		{
			OnHurtEnter(2);
		}
	}

	public bool CanJumpFromSink()
	{
		if (!IsSinking || !CanSinkJump)
		{
			return !IsSinking;
		}
		return true;
	}

	public bool IsGrounded()
	{
		if (Physics.Raycast(base.transform.position, -base.transform.up, out RaycastHit, MaxRayLenght - 0.25f, Collision_Mask))
		{
			Debug.DrawLine(base.transform.position, RaycastHit.point, Color.white);
			PlayerManager.PlayerEvents.GroundTag = RaycastHit.transform.tag;
			return true;
		}
		return false;
	}

	public bool GetPrefab(string Name)
	{
		if (Name == PlayerPrefab.ToString())
		{
			return true;
		}
		return false;
	}

	public void PositionToPoint()
	{
		if (IsGrounded())
		{
			_Rigidbody.position = RaycastHit.point + base.transform.up * 0.25f;
		}
	}

	internal bool ShouldAlignOrFall(bool Align)
	{
		if (Align ? (0.5f > GroundNormal.y) : (0.5f < GroundNormal.y))
		{
			return true;
		}
		return false;
	}

	public void ParentPlayer(Transform Parent)
	{
		base.transform.SetParent(Parent);
	}

	public void UnparentPlayer(Transform Parent)
	{
		if (base.transform.parent == Parent)
		{
			base.transform.SetParent(null);
		}
	}

	public virtual void OnGoal()
	{
		Singleton<GameManager>.Instance.GameState = GameManager.State.Result;
		PlayerGoal = GameObject.FindGameObjectsWithTag("PlayerGoal")[0].GetComponent<PlayerGoal>();
		SetCameraParams(new CameraParameters(3, PlayerGoal.Cam_Pos, PlayerGoal.Cam_Tgt));
		Camera.UncancelableEvent = true;
		StageManager.BGMPlayer.Stop();
		HUD.ShowResultsScreen(this);
		PlayerVoice.PlayGoal();
		StateMachine.ChangeState(StateResult);
	}

	public void OnRailEnter(RailSystem RailSys, int _RailType)
	{
		if ((!(GetState() == "Grinding") || !(RailSystem == RailSys)) && !(GetState() == "Vehicle") && !(GetState() == "Death"))
		{
			RailSystem = RailSys;
			RailType = _RailType;
			StateMachine.ChangeState(StateGrinding);
		}
	}

	public RailSystem GetRailSystem()
	{
		return RailSystem;
	}

	public RailData[] GetRailPathData()
	{
		return RailSystem.GetRailPathData();
	}

	public Ray ClosestRailPoint(Vector3 PlayerPosition, ref int Index)
	{
		Ray result = new Ray(Vector3.zero, Vector3.zero);
		float num = 50f;
		RailData[] railPathData = GetRailPathData();
		for (int i = 0; i < railPathData.Length; i++)
		{
			Vector3 position = railPathData[i].position;
			float num2 = Vector3.Distance(PlayerPosition, position);
			if (num2 < num)
			{
				result.origin = position;
				result.direction = railPathData[i].tangent;
				num = num2;
				Index = i;
			}
		}
		return result;
	}

	public float[] GrindData(Vector3 PlayerPosition, Vector3 PlayerDirection)
	{
		int Index = 0;
		Ray ray = ClosestRailPoint(PlayerPosition, ref Index);
		Vector3 normalized = (ray.origin - PlayerPosition).normalized;
		int num = Index + ((!(Vector3.Dot(ray.direction, normalized) > 0f)) ? 1 : (-1));
		if (num < 0 || num > GetRailPathData().Length - 1)
		{
			if (num >= 0)
			{
				return new float[2] { 1f, -1f };
			}
			return new float[2] { 0f, 1f };
		}
		Vector3 origin = ray.origin;
		Vector3 position = GetRailPathData()[num].position;
		float num2 = ((Vector3.Dot(PlayerDirection, ray.direction) > 0f) ? 1f : (-1f));
		return new float[9] { origin.x, origin.y, origin.z, position.x, position.y, position.z, num2, Index, num };
	}

	private bool CanChangeRail(bool Invert = false)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + base.transform.right * (2.25f * ((!Invert) ? 1f : (-1f))) - base.transform.up * 0.25f, 1.75f, Trigger_Mask.value, QueryTriggerInteraction.Collide);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				RailCollision component = array[i].GetComponent<RailCollision>();
				if ((bool)component && !component.Unswitchable && component.BezierScript != RailSystem)
				{
					SwitchRailSystem = component.BezierScript;
					return true;
				}
			}
		}
		return false;
	}

	private void OnChangeRail(bool Invert = false)
	{
		SwitchPos = RailData.position + base.transform.up * 0.25f;
		RailSystem = SwitchRailSystem;
		RailSys = SwitchRailSystem;
		GrindSplineLength = RailSys.Length();
		float[] array = GrindData(base.transform.position + base.transform.up * 0.25f, base.transform.forward);
		GrindTime = 0f;
		GrindDir = 0f;
		if (array.Length == 2)
		{
			GrindTime = array[0];
			GrindDir = array[1];
		}
		else
		{
			Vector3 vector = new Vector3(array[0], array[1], array[2]);
			Vector3 vector2 = new Vector3(array[3], array[4], array[5]);
			Vector3 vector3 = Math3D.ClosestPointOnLine(vector, vector2, base.transform.position + base.transform.up * 0.25f);
			float t = Vector3.Distance(vector, vector3) / Vector3.Distance(vector, vector2);
			GrindDir = array[6];
			float num = (float)GetRailPathData().Length - 1f;
			GrindTime = Mathf.Lerp(array[7] / num, array[8] / num, t);
			Debug.DrawLine(vector, vector2, Color.white, 10f);
			Debug.DrawLine(base.transform.position - base.transform.up * 0.25f, vector3, Color.yellow, 10f);
		}
		RailData = RailSys.GetRailData(GrindTime);
		SwitchTime = 0f;
		if (!GetPrefab("metal_sonic"))
		{
			PlayerVoice.PlayRandom(1, RandomPlayChance: true);
		}
		Audio.PlayOneShot(GrindWind, Audio.volume);
		LeftSwitching = Invert;
		SwitchRailSystem = null;
	}

	private float GetRailInput()
	{
		float num = Vector3.Dot(base.transform.forward, Camera.transform.forward);
		return Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * ((num > 0f) ? 1f : (-1f));
	}

	public BezierCurve GetSpline(string Spline)
	{
		return GameObject.Find("Stage/Splines/" + Spline).GetComponent<BezierCurve>();
	}

	public void CreateRings(int Rings, bool IsSuperSpeed = false)
	{
		float num = 360f / (float)Rings;
		float num2 = Random.Range(0f, 360f);
		float num3 = Mathf.Lerp(2f, 6f, (float)Rings / 20f);
		for (int i = 0; i < Rings; i++)
		{
			num2 += num;
			Vector3 vector = Quaternion.Euler(new Vector3(0f, num2, 0f)) * Vector3.forward;
			LostRing component = Object.Instantiate(LostRingPrefab, base.transform.position + vector * 0.75f + base.transform.up * Random.Range(0.5f, 0.5125f), Quaternion.identity).GetComponent<LostRing>();
			if (i < 1)
			{
				Singleton<AudioManager>.Instance.PlayClip(component.ClipPool[0]);
			}
			if (!IsSuperSpeed)
			{
				component.SetVelocity(vector * num3 + Vector3.up * 5f);
			}
			else
			{
				component.SetVelocity(vector * num3 + base.transform.forward * (CurSpeed * 1.25f) + Vector3.up * 5f);
			}
		}
	}

	public bool AttackSphere(Vector3 Position, float Radius, Vector3 Force, int Damage, string Type = "")
	{
		HitInfo value = new HitInfo(base.transform, Force, Damage);
		Collider[] array = Physics.OverlapSphere(Position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && (Type != "SuperSpeed" || (Type == "SuperSpeed" && !array[i].GetComponentInParent<ItemBox>())))
			{
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
				if (Type == "SuperSonic")
				{
					array[i].SendMessageUpwards("DestroySearchlight", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		return array.Length != 0;
	}

	public bool HomingAttackSphere(Vector3 Position, float Radius, Vector3 Force, int Damage, string Message = "", int Context = 0)
	{
		HitInfo value = new HitInfo(base.transform, Force, Damage);
		Collider[] array = Physics.OverlapSphere(Position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				if (Message != "")
				{
					array[i].SendMessage(Message, Context, SendMessageOptions.DontRequireReceiver);
				}
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool JumpAttackSphere(Vector3 Position, float Radius, Vector3 Force, int Damage)
	{
		HitInfo value = new HitInfo(base.transform, Force, Damage);
		Collider[] array = Physics.OverlapSphere(Position, Radius, JumpAttack_Mask.value, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				EnemyBase componentInParent = array[i].GetComponentInParent<EnemyBase>();
				if ((bool)componentInParent && (componentInParent.IsPsychokinesis || componentInParent.PsychoThrown))
				{
					return false;
				}
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool AttackSphere_Dir(Vector3 Position, float Radius, float Force, int Damage)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
				if (vector == Vector3.zero)
				{
					vector = base.transform.forward.MakePlanar();
				}
				Vector3 force = (vector + Vector3.up * Random.Range(0.1f, 0.25f)).normalized * Force;
				HitInfo value = new HitInfo(base.transform, force, Damage);
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool DeflectSphere(Vector3 Position, float Radius)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage("OnDeflect", base.transform, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool AttractSphere(float Radius)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + base.transform.up * 0.25f, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage("OnAttract", base.transform, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool StunSphere(Vector3 Position, float Radius, bool AffectObjs = false)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage("OnFlash", SendMessageOptions.DontRequireReceiver);
				if (AffectObjs && (bool)array[i].GetComponentInParent<Common_Switch>())
				{
					array[i].SendMessageUpwards("OnSwitch", SendMessageOptions.DontRequireReceiver);
				}
				if (AffectObjs && (bool)array[i].GetComponentInParent<ItemBox>())
				{
					array[i].SendMessageUpwards("OnHit", new HitInfo(base.transform, Vector3.zero), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		return array.Length != 0;
	}

	public bool OnButtonPressed(string Input)
	{
		ComboInput = Input;
		return Singleton<RInput>.Instance.P.GetButtonDown(Input);
	}

	public bool OnCombo(string Input)
	{
		if (ComboInput == Input)
		{
			if (!Singleton<RInput>.Instance.P.GetButtonDown(Input) || QueuedPress)
			{
				return QueuedPress;
			}
			return true;
		}
		return false;
	}

	public void SetCameraParams(CameraParameters CameraParameters)
	{
		Camera.SetParams(CameraParameters);
	}

	public void DestroyCameraParams(CameraParameters CameraParameters)
	{
		Camera.DestroyParams(CameraParameters);
	}

	private float CamHeight()
	{
		float result = ((Singleton<Settings>.Instance.settings.CameraType != 2) ? 0.35f : 0f);
		if (Camera.CameraState == PlayerCamera.State.FirstPerson)
		{
			result = 0.35f;
		}
		if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
		{
			result = 0.95f;
		}
		return result;
	}

	private void UpdateCameraTarget()
	{
		float num = Vector3.Dot(TargetDirection * (IsGrounded() ? 1.25f : 0.5f), Camera.transform.right);
		CamDirDot = ((Singleton<Settings>.Instance.settings.CameraLeaning == 1 && Camera.CameraState != PlayerCamera.State.FirstPerson && Camera.CameraState != PlayerCamera.State.OverTheShoulder && Camera.CameraState != PlayerCamera.State.OverTheShoulderFadeIn) ? Mathf.Lerp(CamDirDot, (!LockControls) ? num : 0f, Mathf.SmoothStep(0f, 1.5f, 3f * Time.deltaTime)) : 0f);
		CameraTarget.rotation = Quaternion.identity;
		if (Camera.CameraState == PlayerCamera.State.Event && (Camera.parameters.Mode == 4 || Camera.parameters.Mode == 40 || Camera.parameters.Mode == 41 || Camera.parameters.Mode == 42 || Camera.parameters.Mode == 5))
		{
			CamOffset.y = ((!GetPrefab("omega")) ? 0.15f : 0.55f);
		}
		else if (Camera.CameraState == PlayerCamera.State.OverTheShoulder || Camera.CameraState == PlayerCamera.State.OverTheShoulderFadeIn)
		{
			CamOffset.y = Common_Lua.c_camera.y - 0.2f;
		}
		else if (!GetPrefab("snow_board"))
		{
			if (GetState() != "Vehicle")
			{
				CamOffset.x = Mathf.Lerp(CamOffset.x, 0f, Time.deltaTime * 5f);
				CamOffset.y = Mathf.Lerp(CamOffset.y, (!GetPrefab("omega")) ? (Common_Lua.c_camera.y - 0.2f) : (Common_Lua.c_camera.y + 0.2f), Time.deltaTime * 5f);
				CamOffset.z = Mathf.Lerp(CamOffset.z, 0f, Time.deltaTime * 5f);
			}
		}
		else
		{
			CamOffset.y = Mathf.Lerp(CamOffset.y, Common_Lua.c_camera.y - (IsGrounded() ? (-0.125f) : 0.2f), Time.deltaTime * 5f);
		}
		Vector3 vector = Camera.transform.right * CamOffset.x + CameraTarget.up * CamOffset.y + base.transform.forward * CamOffset.z + base.transform.right * Common_Lua.c_camera.x + base.transform.forward * Common_Lua.c_camera.z;
		CameraTarget.position = base.transform.position + base.transform.up * CamHeight() + vector + Camera.transform.right * CamDirDot;
	}

	private void UpdateCameraTarget_Mach()
	{
		CamDirDot = ((Singleton<Settings>.Instance.settings.CameraLeaning == 1) ? Mathf.Lerp(CamDirDot, (!LockControls && !IsOnWall) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, Mathf.SmoothStep(0f, 1.5f, 3f * Time.deltaTime)) : 0f);
		CameraTarget.rotation = Quaternion.identity;
		if (Camera.CameraState == PlayerCamera.State.Event && (Camera.parameters.Mode == 3 || Camera.parameters.Mode == 4 || Camera.parameters.Mode == 40 || Camera.parameters.Mode == 41 || Camera.parameters.Mode == 42 || Camera.parameters.Mode == 5))
		{
			MachCamOffset.y = 0.15f;
		}
		else
		{
			MachCamOffset.y = Mathf.Lerp(MachCamOffset.y, Common_Lua.c_camera.y - 0.2f, Time.deltaTime * 5f);
		}
		if (Camera.MultDistance && Camera.parameters.Mode == 103 && IsGrounded())
		{
			MachCamOffset.x = Mathf.Lerp(MachCamOffset.x, (Camera.parameters.Target.x == 0f) ? (-1.75f) : 1.75f, Time.deltaTime * 2.5f);
		}
		else
		{
			MachCamOffset.x = Mathf.Lerp(MachCamOffset.x, 0f, Time.deltaTime * 2.5f);
		}
		Vector3 vector = CameraTarget.up * MachCamOffset.y + base.transform.right * Common_Lua.c_camera.x + base.transform.forward * Common_Lua.c_camera.z;
		CameraTarget.position = base.transform.position + base.transform.up * ((Singleton<Settings>.Instance.settings.CameraType != 2) ? 0.35f : 0f) + vector + Camera.transform.right * CamDirDot + Camera.transform.right * MachCamOffset.x;
	}

	private void UpdateTrigger()
	{
		Triggers = Physics.OverlapBox(base.transform.position + base.transform.up * ((!GetPrefab("omega")) ? 0.24f : 0.49f), (!GetPrefab("omega")) ? HalfExtents : BigExtents, base.transform.rotation, Trigger_Mask.value, QueryTriggerInteraction.Collide);
		for (int i = 0; i < Triggers.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < LastTriggers.Length; j++)
			{
				if (Triggers[i] == LastTriggers[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Triggers[i].SendMessage("OnTriggerEnter", CapsuleCollider, SendMessageOptions.DontRequireReceiver);
			}
		}
		LastTriggers = Triggers;
	}

	internal GameObject FindHomingTarget(bool OnlyEnemy = false)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		GameObject gameObject = null;
		float num = ExtensionMethods.C_LockOn_Homing.Far.z;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject2.transform.position);
			if (num2 < num && CanSetHomingTarget(gameObject2, OnlyEnemy))
			{
				gameObject = gameObject2;
				num = num2;
			}
		}
		if (!gameObject && !OnlyEnemy)
		{
			array2 = GameObject.FindGameObjectsWithTag("RailHomingTarget");
			foreach (GameObject gameObject3 in array2)
			{
				float num3 = Vector3.Distance(base.transform.position, gameObject3.transform.position);
				if (num3 < num && CanSetHomingTarget(gameObject3, OnlyEnemy))
				{
					gameObject = gameObject3;
					num = num3;
				}
			}
		}
		return gameObject;
	}

	internal GameObject FindSnapTarget()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		GameObject gameObject = null;
		float num = ExtensionMethods.C_LockOn_Snap.Far.z;
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject2.transform.position);
			if (num2 < num && CanSetSnapTarget(gameObject2))
			{
				gameObject = gameObject2;
				num = num2;
			}
		}
		if (!gameObject)
		{
			array2 = GameObject.FindGameObjectsWithTag("RailHomingTarget");
			foreach (GameObject gameObject3 in array2)
			{
				float num3 = Vector3.Distance(base.transform.position, gameObject3.transform.position);
				if (num3 < num && CanSetSnapTarget(gameObject3))
				{
					gameObject = gameObject3;
					num = num3;
				}
			}
		}
		return gameObject;
	}

	internal GameObject FindPsychoTarget()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		GameObject result = null;
		float num = ExtensionMethods.C_LockOn_Psycho.Far.z * 2f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			if (num2 < num && ExtensionMethods.C_LockOn_Psycho.Inside(base.transform, gameObject.transform.position) && gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				result = gameObject;
				num = num2;
			}
		}
		return result;
	}

	internal List<GameObject> FindLaserTarget(int AmountAllowed)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(base.transform.position + base.transform.up * 0.25f, gameObject.transform.position) < ExtensionMethods.C_LockOn_Laser.Far.z && ExtensionMethods.C_LockOn_Laser.Inside(base.transform, gameObject.transform.position) && !Physics.Linecast(base.transform.position, gameObject.transform.position, ExtensionMethods.HomingBlock_Mask) && LaserClosestTargets.Count < AmountAllowed && !LaserClosestTargets.Contains(gameObject) && gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				LaserClosestTargets.Add(gameObject);
			}
		}
		if (LaserClosestTargets.Count != 0)
		{
			for (int j = 0; j < LaserClosestTargets.Count; j++)
			{
				if (LaserClosestTargets[j] == null)
				{
					LaserClosestTargets.RemoveAt(j);
				}
			}
		}
		return LaserClosestTargets;
	}

	internal List<GameObject> FindSpearTarget(int AmountAllowed)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		List<GameObject> list = new List<GameObject>();
		list.Clear();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (Vector3.Distance(base.transform.position, gameObject.transform.position) < ExtensionMethods.C_LockOn_Homing.Far.z && ExtensionMethods.C_LockOn_Homing.Inside(base.transform, gameObject.transform.position) && !Physics.Linecast(base.transform.position, gameObject.transform.position, ExtensionMethods.HomingBlock_Mask) && list.Count < AmountAllowed && gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	private bool CanSetHomingTarget(GameObject Target, bool OnlyEnemy)
	{
		if (ExtensionMethods.C_LockOn_Homing.Inside(base.transform, Target.transform.position) && !Physics.Linecast(base.transform.position, Target.transform.position, ExtensionMethods.HomingBlock_Mask))
		{
			if (OnlyEnemy)
			{
				if (OnlyEnemy)
				{
					return Target.layer == LayerMask.NameToLayer("Enemy");
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private bool CanSetSnapTarget(GameObject Target)
	{
		if (ExtensionMethods.C_LockOn_Snap.Inside(base.transform, Target.transform.position))
		{
			return !Physics.Linecast(base.transform.position, Target.transform.position, ExtensionMethods.HomingBlock_Mask);
		}
		return false;
	}

	public Vector3 LightDashGroundPos(Vector3 SplinePos)
	{
		if (Physics.Raycast(SplinePos, -Vector3.up, out var hitInfo, 1f))
		{
			return hitInfo.point + Vector3.up * 0.25f;
		}
		return SplinePos;
	}

	public bool CanLightDash(float Distance = 5f)
	{
		LightDashRings = GameObject.FindGameObjectsWithTag("LightDashable");
		for (int i = 0; i < LightDashRings.Length; i++)
		{
			Vector3 position = LightDashRings[i].transform.position;
			Vector3 vector = position - base.transform.position;
			if (vector.magnitude < Distance && Vector3.Dot(vector.normalized, base.transform.forward) > 0.5f && GetRingSplineDist(i, position) < 1f)
			{
				return true;
			}
		}
		return false;
	}

	public BezierCurve GetRingSpline(int Index)
	{
		string splineName = LightDashRings[Index].GetComponent<Ring>().SplineName;
		if (splineName == "")
		{
			return null;
		}
		return GameObject.Find("Stage/Splines/" + splineName).GetComponent<BezierCurve>();
	}

	public float GetRingSplineDist(int Index, Vector3 Position)
	{
		Ring component = LightDashRings[Index].GetComponent<Ring>();
		string splineName = component.SplineName;
		if (splineName == "")
		{
			return float.PositiveInfinity;
		}
		float splineTime = component.SplineTime;
		if ((bool)GameObject.Find("Stage/Splines/" + splineName))
		{
			return Vector3.Distance(Position, GameObject.Find("Stage/Splines/" + splineName).GetComponent<BezierCurve>().GetPosition(splineTime));
		}
		return float.PositiveInfinity;
	}

	public int ClosestLightDashRing(float Distance = 5f, int Ignore = -1)
	{
		int result = -1;
		Vector3 vector = ((Ignore == -1) ? base.transform.position : LightDashRings[Ignore].transform.position);
		for (int i = 0; i < LightDashRings.Length; i++)
		{
			GameObject gameObject = LightDashRings[i];
			if ((bool)gameObject && i != Ignore)
			{
				Vector3 vector2 = gameObject.transform.position - vector;
				float magnitude = vector2.magnitude;
				if (magnitude < Distance && Vector3.Dot(vector2.normalized, base.transform.forward) > 0.45f)
				{
					result = i;
					Distance = magnitude;
				}
			}
		}
		return result;
	}

	public bool LastLightDashRing(Vector3 Direction, out int Index)
	{
		float num = 6f;
		Vector3 position = base.transform.position;
		int num2 = 0;
		Index = 0;
		for (int i = 0; i < LightDashRings.Length; i++)
		{
			GameObject gameObject = LightDashRings[i];
			if ((bool)gameObject)
			{
				Vector3 vector = gameObject.transform.position - position;
				if (vector.magnitude < num && Vector3.Dot(vector.normalized, Direction) > 0.45f)
				{
					num2++;
					Index = i;
				}
			}
		}
		return num2 == 1;
	}
}
