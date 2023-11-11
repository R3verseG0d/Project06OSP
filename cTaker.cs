using UnityEngine;

public class cTaker : EnemyBase
{
	public enum Type
	{
		cTaker = 0,
		cTricker = 1
	}

	public enum Mode
	{
		Chase_Bomb = 0,
		Fix = 1,
		Fix_Bomb = 2,
		Homing = 3,
		Master = 4,
		Normal = 5,
		Normal_Bomb = 6,
		Slave = 7
	}

	public enum State
	{
		TransferFall = 0,
		Wait = 1,
		Search = 2,
		Seek = 3,
		Roam = 4,
		Move = 5,
		Command = 6,
		Found = 7,
		ShootRocket = 8,
		Reload = 9,
		DropBomb = 10,
		GlidingAttack = 11,
		Stuned = 12,
		CrashStuned = 13,
		DamageKnockBack = 14
	}

	[Header("Framework")]
	public Mode CreatureMode;

	public string GroupID;

	[Header("Prefab")]
	public Type EnemyType;

	public LayerMask FrontalColLayer;

	public AnimationCurve YSwingMotion;

	public Renderer[] Models;

	public ParticleSystem[] FireFX;

	public Transform BombPoint;

	public Transform DropBombPoint;

	[Header("Instantiation")]
	public GameObject BombPrefab;

	public GameObject DropBombPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip FlameOn;

	public AudioClip FlameOff;

	public AudioClip AttackCry;

	internal State EnemyState;

	private BezierCurve PathSpline;

	private GameObject DropBomb;

	private bool TriggerStateSwitch;

	private bool GroupDestroy;

	private bool Reloaded;

	private bool FoundTarget;

	private bool CatchedPlayer;

	private bool DropPossession;

	private float ActionSplineTime;

	private float FlySpeed;

	private float StartTime;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int AttackCount;

	private int SlaveAction;

	private float Timer;

	private float DistFromPlayer;

	private float RoundPlayer;

	private bool MoveSwitch;

	private int SiegeType;

	private float EvTime;

	private Vector3 OrigY;

	private Vector3 AdjustY;

	private Vector3 CrashVel;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		DescentOffset *= 750f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = base.transform.position + Vector3.up * DescentOffset;
		BaseStart();
		if (CreatureMode == Mode.Chase_Bomb && !FindPlayer)
		{
			Target = GameObject.FindGameObjectWithTag("Player");
		}
		if (CreatureMode == Mode.Chase_Bomb)
		{
			StateWaitStart();
			StateMachine.Initialize(StateWait);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.25f, StartRotation);
			}
			if ((bool)GameObject.Find("Stage/Splines/" + ActionPath))
			{
				PathSpline = GameObject.Find("Stage/Splines/" + ActionPath).GetComponent<BezierCurve>();
			}
			StartTime = Time.time;
		}
		else
		{
			Transfer();
		}
	}

	private void Transfer()
	{
		if (CreatureMode != 0 && !(Position == Vector3.zero))
		{
			StateTime[0] = Time.time + DescentOffset / 9.81f;
			base.transform.position = StartPosition + Vector3.up * DescentOffset;
			base.transform.rotation = StartRotation;
			StateTransferFallStart();
			StateMachine.Initialize(StateTransferFall);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.5f, StartRotation);
			}
			_Rigidbody.isKinematic = true;
		}
	}

	private void Reset()
	{
		if (CreatureMode != 0)
		{
			Target = null;
		}
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.5f, StartRotation);
		}
		if ((bool)ParalysisEffect)
		{
			Animator.speed = 1f;
			Stuned = false;
			Object.Destroy(ParalysisEffect);
		}
		CurHealth = MaxHealth;
		ActionSplineTime = 0f;
		base.transform.position = StartPosition + Vector3.up * DescentOffset;
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		if ((CreatureMode != Mode.Fix_Bomb || CreatureMode != 0) && Reloaded)
		{
			DestroyBomb();
		}
		base.gameObject.SetActive(value: false);
		_Rigidbody.isKinematic = true;
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.cTricker)
		{
			return 1;
		}
		return 0;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (CreatureMode == Mode.Chase_Bomb && (bool)PathSpline)
		{
			float num = Vector3.Dot(base.transform.forward, (base.transform.position - Target.transform.position).normalized);
			num = ((num > 0f) ? 1f : (-1f));
			float num2 = Vector3.Distance(base.transform.position, Target.transform.position) * num;
			float b = Mathf.Lerp(Random.Range(80f, 100f), 0f, num2 / Random.Range(25f, 35f));
			FlySpeed = Mathf.Lerp(FlySpeed, b, Time.fixedDeltaTime * 75f);
			float num3 = Mathf.Clamp((Time.time - StartTime) * 0.7f, 0f, 1f);
			ActionSplineTime += FlySpeed / PathSpline.Length() * Time.fixedDeltaTime * num3;
			base.transform.position = PathSpline.GetPosition(ActionSplineTime);
			base.transform.rotation = Quaternion.LookRotation(PathSpline.GetTangent(ActionSplineTime).MakePlanar());
			CatchedPlayer = num == 1f;
			if (ActionSplineTime >= 1f)
			{
				OnTransfer();
			}
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(1.6f);
		Models[0].enabled = !Target;
		Models[1].enabled = Target;
		for (int i = 0; i < FireFX.Length; i++)
		{
			if (!FoundTarget && (bool)Target)
			{
				FireFX[i].Play();
				Audio.PlayOneShot(FlameOn, Audio.volume * 1.5f);
				FoundTarget = true;
			}
			else if (FoundTarget && !Target)
			{
				FireFX[i].Stop();
				Audio.PlayOneShot(FlameOff, Audio.volume * 0.75f);
				FoundTarget = false;
			}
		}
	}

	private void StateTransferFallStart()
	{
		EnemyState = State.TransferFall;
		Animation = 0;
	}

	private void StateTransferFall()
	{
		if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateWait);
			return;
		}
		float t = Mathf.SmoothStep(0f, 1f, 1f - (StateTime[0] - Time.time));
		_Rigidbody.position = Vector3.Lerp(Position, StartPosition, t);
	}

	private void StateTransferFallEnd()
	{
	}

	private void StateWaitStart()
	{
		EnemyState = State.Wait;
		Animation = 0;
		StateTime[0] = Time.time + 2.5f;
		if (CreatureMode != 0 && GetDistance() > 25f)
		{
			Target = null;
		}
	}

	private void StateWait()
	{
		if (CreatureMode != 0)
		{
			if (GetTarget(Mesh.forward))
			{
				StateMachine.ChangeState(StateSeek);
			}
			else if (Time.time > StateTime[0])
			{
				StateMachine.ChangeState(StateSearch);
			}
		}
		else if ((((bool)PathSpline && CatchedPlayer) || !PathSpline) && GetTarget(Mesh.forward))
		{
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateWaitEnd()
	{
	}

	private void StateSearchState()
	{
		EnemyState = State.Search;
		Animation = 0;
		StateTime[0] = Time.time + 1.9665f;
		StateVar[0] = 0;
	}

	private void StateSearch()
	{
		if (GetTarget(Mesh.forward))
		{
			StateMachine.ChangeState(StateSeek);
		}
		if (!(Time.time > StateTime[0]))
		{
			return;
		}
		if (StateVar[0] == 0)
		{
			TargetPosition = base.transform.position + Vector3.up - (((CreatureMode == Mode.Normal || CreatureMode == Mode.Normal_Bomb) && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if ((CreatureMode == Mode.Normal || CreatureMode == Mode.Normal_Bomb) && Random.value > 0.5f)
			{
				StateMachine.ChangeState(StateRoam);
			}
		}
	}

	private void StateSearchEnd()
	{
	}

	private void StateSeekStart()
	{
		EnemyState = State.Seek;
		Animation = 0;
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Timer = 0f;
	}

	private void StateSeek()
	{
		if (CreatureMode == Mode.Homing && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (CreatureMode != 0 && GetDistance() > 25f)
		{
			Target = null;
		}
		if (!Target)
		{
			StateMachine.ChangeState(StateWait);
		}
		if (CreatureMode == Mode.Homing || !(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir) || (EnemyType == Type.cTricker && (EnemyType != Type.cTricker || (CreatureMode != Mode.Normal && CreatureMode != Mode.Fix && CreatureMode != Mode.Master && CreatureMode != Mode.Slave))))
		{
			return;
		}
		if (EnemyType == Type.cTricker)
		{
			if (CreatureMode == Mode.Master)
			{
				Timer += Time.fixedDeltaTime;
				if (Timer > 2f)
				{
					StateMachine.ChangeState(StateCommand);
				}
			}
			else if (CreatureMode != Mode.Slave)
			{
				ToAttackState();
			}
		}
		else
		{
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void ToAttackState()
	{
		switch (CreatureMode)
		{
		case Mode.Chase_Bomb:
			StateMachine.ChangeState(StateReload);
			break;
		case Mode.Fix:
			StateMachine.ChangeState(StateShootRocket);
			break;
		case Mode.Fix_Bomb:
			StateMachine.ChangeState(StateReload);
			break;
		case Mode.Normal:
			if (EnemyType != Type.cTricker)
			{
				if (GetDistance() < 7.875f)
				{
					StateMachine.ChangeState(StateShootRocket);
				}
				else
				{
					StateMachine.ChangeState(StateMove);
				}
			}
			else if (GetDistance() < 7.875f)
			{
				StateMachine.ChangeState(StateGlidingAttack);
			}
			else
			{
				StateMachine.ChangeState(StateMove);
			}
			break;
		case Mode.Normal_Bomb:
		{
			string possessionName = GetPossessionName();
			DropPossession = possessionName == "R_Toe";
			StateMachine.ChangeState(StateMove);
			break;
		}
		case Mode.Homing:
		case Mode.Master:
			break;
		}
	}

	private void StateRoamStart()
	{
		EnemyState = State.Seek;
		Animation = 0;
		StateEnd = Time.time + 2f;
		_Rigidbody.isKinematic = false;
	}

	private void StateRoam()
	{
		EnemyState = State.Seek;
		_Rigidbody.velocity = Mesh.forward * 2f;
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateRoamEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateMoveStart()
	{
		EnemyState = State.Move;
		Animation = 0;
		StateEnd = Time.time + 2f;
		MoveSwitch = Random.value < 0.75f;
		StateVar[0] = 0;
		_Rigidbody.isKinematic = false;
	}

	private void StateMove()
	{
		HoldBomb();
		if (StateVar[0] == 0)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			if (CreatureMode == Mode.Normal_Bomb)
			{
				DistFromPlayer = ((!DropPossession) ? (-4f) : 4f);
				RoundPlayer = 0f;
				if (!DropPossession)
				{
					Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
				}
			}
			else
			{
				DistFromPlayer = (((bool)Target && GetDistance() > 7.875f) ? 4f : 0f);
				RoundPlayer = (((bool)Target && GetDistance() < 7.875f) ? 1f : 0f);
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			}
			_Rigidbody.velocity = Mesh.right * ((MoveSwitch ? 5f : (-5f)) * RoundPlayer) + Mesh.forward * DistFromPlayer;
			if (Time.time > StateEnd)
			{
				StateEnd = Time.time + 1f;
				StateVar[0] = 1;
			}
		}
		else if (EnemyType != Type.cTricker || (EnemyType == Type.cTricker && CreatureMode != Mode.Slave))
		{
			if (CreatureMode == Mode.Normal_Bomb)
			{
				if (Time.time > StateEnd || (!DropPossession && GetDistance() > 10.5f) || (DropPossession && GetDistance() < 5.25f))
				{
					StateEnd = 0f;
					if (DropPossession)
					{
						StateMachine.ChangeState(StateDropBomb);
					}
					else
					{
						StateMachine.ChangeState(StateReload);
					}
				}
			}
			else if (Time.time > StateEnd)
			{
				StateEnd = 0f;
				if (EnemyType == Type.cTricker)
				{
					StateMachine.ChangeState(StateGlidingAttack);
				}
				else
				{
					StateMachine.ChangeState(StateShootRocket);
				}
			}
		}
		else
		{
			StateMachine.ChangeState(StateWait);
		}
		if (!Target)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateMoveEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
		SlaveAction = 1;
	}

	private void StateCommandStart()
	{
		EnemyState = State.Command;
		Animation = 6;
		StateEnd = Time.time + 1.5f;
	}

	private void StateCommand()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateWait);
			if ((bool)Target)
			{
				ManageSlaves("Command");
			}
		}
	}

	private void StateCommandEnd()
	{
	}

	private void StateFoundStart()
	{
		EnemyState = State.Found;
		Animation = 6;
		StateEnd = Time.time + 1.5f;
	}

	private void StateFound()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f);
		if (!(Time.time > StateEnd))
		{
			return;
		}
		StateEnd = 0f;
		if (TriggerStateSwitch)
		{
			if (SlaveAction == 0)
			{
				StateMachine.ChangeState(StateMove);
			}
			else
			{
				StateMachine.ChangeState(StateGlidingAttack);
			}
		}
		else
		{
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateFoundEnd()
	{
	}

	private void StateShootRocketStart()
	{
		EnemyState = State.ShootRocket;
		Animation = 0;
		AttackCount = 0;
		StateTime[0] = Time.time + 0.6335f;
		StateTime[1] = 0f;
	}

	private void StateShootRocket()
	{
		if ((bool)Target)
		{
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		}
		else
		{
			StateMachine.ChangeState(StateWait);
		}
		if (Time.time > StateTime[0] && (float)AttackCount < 2f)
		{
			Vector3 normalized = (TargetPosition - BombPoint.position).normalized;
			GameObject gameObject = Object.Instantiate(BombPrefab, BombPoint.position, Quaternion.LookRotation(normalized));
			if (EnemyType != Type.cTricker)
			{
				gameObject.GetComponent<Rigidbody>().AddForce(normalized * 21f, ForceMode.Impulse);
				gameObject.GetComponent<TimedBomb>().Launched = true;
			}
			else
			{
				gameObject.GetComponent<DarkShot>().Player = Target.transform;
				gameObject.GetComponent<DarkShot>().Owner = base.transform;
			}
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
			}
			StateTime[0] = Time.time + 1f;
			AttackCount++;
		}
		if ((float)AttackCount >= 2f)
		{
			if (StateTime[1] == 0f)
			{
				StateTime[1] += Time.time + 2f;
			}
			else if (Time.time > StateTime[1])
			{
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateShootRocketEnd()
	{
	}

	private void StateReloadStart()
	{
		EnemyState = State.Reload;
		StateEnd = Time.time + ((CreatureMode == Mode.Normal_Bomb) ? 1.5f : 2f);
		Animation = 0;
		if (!DropPossession && !Reloaded)
		{
			DropBomb = Object.Instantiate(DropBombPrefab, DropBombPoint.position, DropBombPoint.rotation);
			Reloaded = true;
		}
	}

	private void StateReload()
	{
		HoldBomb();
		if (Time.time > StateEnd)
		{
			if (CreatureMode == Mode.Normal_Bomb && !DropPossession && GetDistance() < 25f)
			{
				StateMachine.ChangeState(StateSeek);
			}
			else
			{
				StateMachine.ChangeState(StateDropBomb);
			}
		}
	}

	private void StateReloadEnd()
	{
	}

	private void StateDropBombStart()
	{
		EnemyState = State.DropBomb;
		Animation = 3;
		StateEnd = Time.time + 3f;
		StateTime[0] = Time.time + 1f;
		if ((bool)DropBomb)
		{
			DropBomb.GetComponent<TimedBomb>().enabled = true;
			DropBomb.GetComponent<TimedBomb>().Launched = true;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(DropBomb.GetComponentInChildren<Collider>(), collider);
			}
			DropBomb = null;
		}
		Reloaded = false;
	}

	private void StateDropBomb()
	{
		if (Time.time > StateTime[0])
		{
			Animation = 0;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateDropBombEnd()
	{
	}

	private void StateGlidingAttackStart()
	{
		EnemyState = State.GlidingAttack;
		Animation = 4;
		Animator.SetTrigger("On Glide Attack");
		StateEnd = Time.time + 2f;
		_Rigidbody.isKinematic = false;
		StateVar[0] = 0;
		EvTime = 0f;
		OrigY = base.transform.position;
	}

	private void StateGlidingAttack()
	{
		if (StateVar[0] == 0)
		{
			if ((bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
			}
			_Rigidbody.velocity = Vector3.zero;
			AdjustY = new Vector3(base.transform.position.x, TargetPosition.y + 2f, base.transform.position.z);
			OrigY = Vector3.Lerp(OrigY, AdjustY, Time.fixedDeltaTime);
			_Rigidbody.MovePosition(OrigY);
			if (Time.time > StateEnd)
			{
				StateEnd = Time.time + 0.5f;
				Audio.PlayOneShot(AttackCry, Audio.volume);
				StateVar[0] = 1;
			}
		}
		else if (StateVar[0] == 1)
		{
			if (Physics.Raycast(base.transform.position + base.transform.up * 0.4f, Mesh.forward, out var hitInfo, 0.75f, FrontalColLayer) && hitInfo.transform.GetComponent<Collider>() != GetComponent<Collider>())
			{
				StateMachine.ChangeState(StateCrashStuned);
			}
			EvTime += Time.fixedDeltaTime;
			Vector3 velocity = Mesh.forward * 26.25f + Vector3.up * YSwingMotion.Evaluate(EvTime);
			_Rigidbody.velocity = velocity;
			if (Time.time > StateEnd)
			{
				StateEnd = Time.time + 2.5f;
				Animation = 0;
				StateVar[0] = 2;
			}
		}
		else
		{
			_Rigidbody.velocity = Vector3.zero;
			if (Time.time > StateEnd)
			{
				StateEnd = 0f;
				_Rigidbody.isKinematic = true;
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateGlidingAttackEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
		SlaveAction = 0;
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		Animation = 0;
		StateEnd = Time.time + 7.5f;
		Animator.speed = 0f;
	}

	private void StateStuned()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateStunedEnd()
	{
		Stuned = false;
		Object.Destroy(ParalysisEffect);
		Animator.speed = 1f;
	}

	private void StateCrashStunedStart()
	{
		EnemyState = State.CrashStuned;
		StateEnd = Time.time + 7.5f;
		Animation = 5;
		Animator.SetTrigger("On Crash Stuned");
		if (!ParalysisEffect)
		{
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 1.25f, Quaternion.identity);
		}
		ParalysisEffect.transform.SetParent(base.transform);
		_Rigidbody.isKinematic = false;
		CrashVel = Mesh.forward * -13.125f + Vector3.up * 6.5625f;
	}

	private void StateCrashStuned()
	{
		CrashVel = Vector3.Lerp(CrashVel, Vector3.zero, Time.fixedDeltaTime * 2f);
		_Rigidbody.velocity = CrashVel;
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.isKinematic = true;
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateCrashStunedEnd()
	{
		Object.Destroy(ParalysisEffect);
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 2;
		StateTime[0] = Time.time + 0.65f;
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void OnTransfer()
	{
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, base.transform.position, StartRotation);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnOrderIssue()
	{
		if (CreatureMode != Mode.Master && EnemyState != State.Move && EnemyState != State.GlidingAttack && EnemyState != State.Stuned && EnemyState != State.CrashStuned && (bool)Target)
		{
			TriggerStateSwitch = true;
			StateMachine.ChangeState(StateFound);
		}
	}

	private void OnFlash()
	{
		if (!Stuned && !IsPsychokinesis && !PsychoThrown)
		{
			StateMachine.ChangeState(StateStuned);
			Stuned = true;
			if ((bool)ParalysisEffect)
			{
				Object.Destroy(ParalysisEffect);
			}
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 0.75f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
			if ((CreatureMode != Mode.Fix_Bomb || CreatureMode != 0) && Reloaded)
			{
				DestroyBomb();
			}
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			if ((CreatureMode != Mode.Fix_Bomb || CreatureMode != 0) && Reloaded)
			{
				DestroyBomb();
			}
			CreateHitFX(HitInfo, IgnoreTimer: true);
			OnImpact(HitInfo, Explosion: true);
			ManageSlaves("Destroy", HitInfo);
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (HitTimer <= GetHitTimer(HitInfo))
		{
			return;
		}
		if ((CreatureMode != Mode.Fix_Bomb || CreatureMode != 0) && Reloaded)
		{
			DestroyBomb();
		}
		if (CurHealth > 0)
		{
			CurHealth -= HitInfo.damage;
			if (!ParalysisEffect && !IsPsychokinesis && !PsychoThrown)
			{
				StateMachine.ChangeState(StateDamageKnockBack);
			}
			HitTimer = 0f;
			CreateHitFX(HitInfo);
			if (CurHealth >= 0)
			{
				return;
			}
		}
		if (!Destroyed)
		{
			CreateHitFX(HitInfo, IgnoreTimer: true);
			OnDestroy();
			OnImpact(HitInfo);
			ManageSlaves("Destroy", HitInfo);
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void ManageSlaves(string Method, HitInfo HitInfo = null)
	{
		if (GroupDestroy || EnemyType != Type.cTricker || CreatureMode != Mode.Master)
		{
			return;
		}
		cTaker[] array = Object.FindObjectsOfType<cTaker>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(Method == "Destroy"))
			{
				if (Method == "Command" && array[i] != this && array[i].EnemyType == Type.cTricker && array[i].GroupID == GroupID && array[i].CreatureMode == Mode.Slave)
				{
					array[i].SendMessage("OnOrderIssue", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				array[i].SendMessage("OnCommandDestroy", HitInfo, SendMessageOptions.DontRequireReceiver);
				GroupDestroy = true;
			}
		}
	}

	private void HoldBomb()
	{
		if ((bool)DropBomb)
		{
			DropBomb.GetComponent<Rigidbody>().position = DropBombPoint.position;
			DropBomb.GetComponent<Rigidbody>().rotation = DropBombPoint.rotation;
			DropBomb.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	private void DestroyBomb()
	{
		if ((bool)DropBomb)
		{
			DropBomb.GetComponent<TimedBomb>().AutoDestroy();
			DropBomb = null;
			Reloaded = false;
		}
	}

	private string GetPossessionName()
	{
		if ((bool)DropBomb)
		{
			return "R_Toe";
		}
		return "null";
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (CreatureMode == Mode.Chase_Bomb)
		{
			OnHit(new HitInfo(base.transform, base.transform.forward * FlySpeed, 10));
		}
	}
}
