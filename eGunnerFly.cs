using UnityEngine;

public class eGunnerFly : EnemyBase
{
	public enum Type
	{
		eGunner = 0,
		eStinger = 1,
		eLancer = 2,
		eBuster = 3
	}

	public enum Mode
	{
		Chase = 0,
		Fix = 1,
		Fix_Vulcan = 2,
		Fix_Rocket = 3,
		Fix_Missile = 4,
		Fix_Laser = 5,
		Homing = 6,
		Normal = 7,
		Trans = 8
	}

	public enum State
	{
		TransferFall = 0,
		Fly = 1,
		Search = 2,
		Seek = 3,
		Roam = 4,
		FollowPlayer = 5,
		ShootVulcan = 6,
		ShootRocket = 7,
		ShootMissile = 8,
		ShootBarrage = 9,
		Stuned = 10,
		DamageKnockBack = 11
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform LeftMuzzle;

	public Transform RightMuzzle;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject MissilePrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip vulcanChargeSound;

	private BezierCurve PathSpline;

	private State EnemyState;

	private bool Aim;

	private bool CatchedPlayer;

	private float SplineTime;

	private float ActionSplineTime;

	private float FlySpeed;

	private float StartTime;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int MaxAttack;

	private int AttackCount;

	private bool Shooted;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		if (AppearPath != "")
		{
			PathSpline = GameObject.Find("Stage/Splines/" + AppearPath).GetComponent<BezierCurve>();
		}
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = base.transform.position + Vector3.up * DescentOffset;
		BaseStart();
		if (RobotMode == Mode.Chase && !FindPlayer)
		{
			Target = GameObject.FindGameObjectWithTag("Player");
		}
		if (RobotMode == Mode.Chase)
		{
			StateFlyStart();
			StateMachine.Initialize(StateFly);
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
		if (RobotMode != 0 && !(Position == Vector3.zero))
		{
			if (AppearPath != "")
			{
				base.transform.position = PathSpline.GetPosition(SplineTime);
			}
			_Rigidbody.velocity = Vector3.zero;
			StateTransferFallStart();
			StateMachine.Initialize(StateTransferFall);
			if ((bool)TeleportEffectPrefab && (AppearPath == "" || (EnemyState == State.TransferFall && AppearPath != "")))
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.25f, StartRotation);
			}
		}
	}

	private void Reset()
	{
		if (EnemyState != 0 || !(AppearPath != ""))
		{
			if (RobotMode != 0)
			{
				Target = null;
			}
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.25f, StartRotation);
			}
			if ((bool)ParalysisEffect)
			{
				Animator.speed = 1f;
				Stuned = false;
				Object.Destroy(ParalysisEffect);
			}
			CurHealth = MaxHealth;
			SplineTime = 0f;
			ActionSplineTime = 0f;
			if (AppearPath == "")
			{
				base.transform.position = StartPosition + Vector3.up * DescentOffset;
				base.transform.rotation = StartRotation;
			}
			Mesh.localEulerAngles = Vector3.zero;
			base.gameObject.SetActive(value: false);
		}
	}

	private int EnemyMaxHealth()
	{
		switch (EnemyType)
		{
		case Type.eLancer:
			return 1;
		case Type.eBuster:
			return 2;
		default:
			return 0;
		}
	}

	private float SiegeDist()
	{
		if (EnemyType == Type.eGunner)
		{
			return 18.9f;
		}
		return 22.05f;
	}

	private int GetMainAmmo()
	{
		int num = ((RobotMode == Mode.Trans) ? RandomInt(1, 2) : RandomInt(2, 3));
		switch (EnemyType)
		{
		case Type.eStinger:
			num = 1;
			break;
		case Type.eLancer:
			num = RandomInt(2, 3);
			break;
		case Type.eBuster:
			num = RandomInt(2, 3);
			break;
		}
		return num - 1;
	}

	private int GetSubAmmo()
	{
		int result = ((RobotMode == Mode.Chase) ? 7 : RandomInt(3, 5));
		Type enemyType = EnemyType;
		if (enemyType == Type.eStinger)
		{
			result = RandomInt(3, 5);
		}
		return result;
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (RobotMode == Mode.Chase)
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
		UpdateGauge(3f);
	}

	private void LateUpdate()
	{
		if ((bool)Target && Aim)
		{
			if (EnemyState == State.ShootVulcan)
			{
				Transform parent = LeftMuzzle.parent.parent;
				Vector3 normalized = (TargetPosition - parent.position).normalized;
				parent.eulerAngles = Quaternion.LookRotation(normalized, parent.up).eulerAngles;
				parent.localEulerAngles += new Vector3(35f, 20f, 0f);
			}
			else if (EnemyState == State.ShootRocket || EnemyState == State.ShootMissile)
			{
				Transform parent2 = RightMuzzle.parent.parent;
				Vector3 normalized2 = (TargetPosition - parent2.position).normalized;
				parent2.eulerAngles = Quaternion.LookRotation(normalized2, parent2.up).eulerAngles;
				parent2.localEulerAngles += new Vector3(0f, 20f, 0f);
			}
		}
	}

	private void StateTransferFallStart()
	{
		EnemyState = State.TransferFall;
		Animation = 0;
		if (AppearPath == "")
		{
			StateTime[0] = Time.time + DescentOffset / 9.81f;
		}
		else if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.25f, StartRotation);
		}
	}

	private void StateTransferFall()
	{
		if (AppearPath == "")
		{
			if (Time.time > StateTime[0])
			{
				StateMachine.ChangeState(StateFly);
				return;
			}
			float t = Mathf.SmoothStep(0f, 1f, 1f - (StateTime[0] - Time.time));
			_Rigidbody.position = Vector3.Lerp(Position, StartPosition, t);
			return;
		}
		_Rigidbody.velocity = Vector3.zero;
		SplineTime += AppearVelocity / PathSpline.Length() * Time.fixedDeltaTime;
		base.transform.position = PathSpline.GetPosition(SplineTime);
		base.transform.rotation = Quaternion.LookRotation(PathSpline.GetTangent(SplineTime).MakePlanar());
		if (SplineTime >= 1f)
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateTransferFallEnd()
	{
	}

	private void StateFlyStart()
	{
		EnemyState = State.Fly;
		Animation = 0;
		StateTime[0] = Time.time + 2.5f;
	}

	private void StateFly()
	{
		if (RobotMode == Mode.Chase)
		{
			if ((((bool)PathSpline && CatchedPlayer) || !PathSpline) && GetTarget(Mesh.forward))
			{
				StateMachine.ChangeState(StateSeek);
			}
		}
		else if (GetTarget(Mesh.forward))
		{
			StateMachine.ChangeState(StateSeek);
		}
		else if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateFlyEnd()
	{
	}

	private void StateSearchStart()
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
			Animation = 3;
			TargetPosition = base.transform.position + Vector3.up - ((RobotMode == Mode.Normal && EnemyType != Type.eBuster && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 2;
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if (RobotMode == Mode.Normal && EnemyType != Type.eBuster && Random.value > 0.5f)
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
	}

	private void StateSeek()
	{
		if (RobotMode == Mode.Homing && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, (RobotMode == Mode.Chase) ? 4f : 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
		StateEnd = Time.time + 1.5f;
		Animation = 0;
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = Mesh.forward * 3f;
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateRoamEnd()
	{
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		StateEnd = Time.time + 1.5f;
		Animation = 0;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = Mesh.forward * 3f;
		if (GetDistance() < 10.5f || Time.time > StateEnd)
		{
			ToAttackState();
		}
		if (GetDistance() > 40f || !Target)
		{
			StateEnd = 0f;
			Target = null;
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateFollowPlayerEnd()
	{
		_Rigidbody.velocity = Vector3.zero;
	}

	private void SecondaryAttack()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eStinger)
		{
			StateMachine.ChangeState(StateShootMissile);
		}
		else
		{
			StateMachine.ChangeState(StateShootRocket);
		}
	}

	private void ToAttackState()
	{
		if (RobotMode == Mode.Homing)
		{
			return;
		}
		if (EnemyType != Type.eBuster)
		{
			if (RobotMode == Mode.Chase || RobotMode == Mode.Fix_Vulcan)
			{
				StateMachine.ChangeState(StateShootVulcan);
			}
			else if (RobotMode == Mode.Fix_Rocket || RobotMode == Mode.Fix_Missile || RobotMode == Mode.Fix_Laser)
			{
				SecondaryAttack();
			}
			else if (RobotMode == Mode.Fix || RobotMode == Mode.Trans)
			{
				if (GetDistance() < 10.5f)
				{
					StateMachine.ChangeState(StateShootVulcan);
				}
				else
				{
					SecondaryAttack();
				}
			}
			else
			{
				if (RobotMode != Mode.Normal)
				{
					return;
				}
				if (GetDistance() < SiegeDist())
				{
					if (GetDistance() < 10.5f)
					{
						StateMachine.ChangeState(StateShootVulcan);
					}
					else
					{
						SecondaryAttack();
					}
				}
				else
				{
					StateMachine.ChangeState(StateFollowPlayer);
				}
			}
		}
		else
		{
			StateMachine.ChangeState(StateShootBarrage);
		}
	}

	private void StateShootVulcanStart()
	{
		EnemyState = State.ShootVulcan;
		Animation = 1;
		AttackCount = 0;
		MaxAttack = GetSubAmmo();
		StateTime[0] = Time.time + 0.567f;
		StateTime[1] = -1f;
		Aim = true;
		Audio.PlayOneShot(vulcanChargeSound, Audio.volume);
	}

	private void StateShootVulcan()
	{
		if (RobotMode == Mode.Chase && AttackCount <= MaxAttack && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 2f);
		}
		if (Time.time > StateTime[0] + (float)AttackCount * 0.125f && AttackCount < MaxAttack)
		{
			Animator.SetTrigger("Attack");
			Vector3 normalized = (TargetPosition - LeftMuzzle.position).normalized;
			normalized.x += Random.Range(-0.025f, 0.025f);
			normalized.y += Random.Range(-0.025f, 0.025f);
			normalized.z += Random.Range(-0.025f, 0.025f);
			normalized.Normalize();
			Object.Instantiate(VulcanBulletPrefab, LeftMuzzle.position, Quaternion.LookRotation(normalized));
			AttackCount++;
		}
		if (StateTime[1] == -1f)
		{
			if (AttackCount >= MaxAttack)
			{
				StateTime[1] = Time.time + 2f;
				Animation = 0;
				Aim = false;
			}
			return;
		}
		if (RobotMode == Mode.Chase)
		{
			Mesh.localRotation = Quaternion.RotateTowards(Mesh.localRotation, Quaternion.identity, 4f);
		}
		if (Time.time > StateTime[1])
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateShootVulcanEnd()
	{
	}

	private void StateShootRocketStart()
	{
		EnemyState = State.ShootRocket;
		Animation = 2;
		AttackCount = 0;
		MaxAttack = GetMainAmmo();
		StateTime[0] = Time.time + 0.6335f;
		StateEnd = Time.time + 10f;
		Aim = true;
		Audio.PlayOneShot(vulcanChargeSound, Audio.volume);
	}

	private void StateShootRocket()
	{
		if (AttackCount <= MaxAttack && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 2f);
		}
		if (Time.time > StateTime[0] + (float)AttackCount * 0.3f && AttackCount <= MaxAttack)
		{
			if ((bool)Target)
			{
				Animator.SetTrigger("Attack");
				Vector3 normalized = (TargetPosition - RightMuzzle.position).normalized;
				GameObject gameObject = Object.Instantiate(MissilePrefab, RightMuzzle.position, Quaternion.LookRotation(normalized));
				gameObject.GetComponent<Missile>().Player = Target.transform;
				gameObject.GetComponent<Missile>().Owner = base.transform;
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
				}
				AttackCount++;
				if (AttackCount > MaxAttack)
				{
					StateEnd = Time.time + 1.3f;
				}
			}
			else
			{
				AttackCount = MaxAttack + 1;
				if (AttackCount > MaxAttack)
				{
					StateEnd = Time.time + 1.3f;
				}
			}
		}
		if (Time.time > StateEnd && Aim)
		{
			Animation = 0;
			Aim = false;
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateShootRocketEnd()
	{
	}

	private void StateShootMissileStart()
	{
		EnemyState = State.ShootMissile;
		Animation = 2;
		StateTime[0] = Time.time + 0.6335f;
		StateEnd = Time.time + 10f;
		Aim = true;
		Shooted = false;
	}

	private void StateShootMissile()
	{
		if (!Shooted && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		}
		if (Time.time > StateTime[0] && !Shooted)
		{
			if ((bool)Target)
			{
				GameObject gameObject = Object.Instantiate(MissilePrefab, RightMuzzle.position, Quaternion.LookRotation(TargetPosition - RightMuzzle.position));
				gameObject.GetComponent<Missile>().Player = Target.transform;
				gameObject.GetComponent<Missile>().Owner = base.transform;
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
				}
			}
			Shooted = true;
			StateEnd = Time.time + 1.3f;
		}
		if (Time.time > StateEnd && Aim)
		{
			Animation = 0;
			Aim = false;
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateShootMissileEnd()
	{
	}

	private void StateShootBarrageStart()
	{
		EnemyState = State.ShootBarrage;
		Animation = 3;
		AttackCount = 0;
		MaxAttack = GetMainAmmo();
		StateTime[0] = Time.time + 0.6335f;
		StateEnd = Time.time + 10f;
		Aim = true;
	}

	private void StateShootBarrage()
	{
		if (Time.time > StateTime[0] + (float)AttackCount * 1f && AttackCount <= MaxAttack)
		{
			if ((bool)Target)
			{
				Animator.SetTrigger("Attack");
				GameObject gameObject = Object.Instantiate(MissilePrefab, LeftMuzzle.position, Quaternion.LookRotation((TargetPosition - LeftMuzzle.position).normalized));
				gameObject.GetComponent<Missile>().Player = Target.transform;
				gameObject.GetComponent<Missile>().Owner = base.transform;
				GameObject gameObject2 = Object.Instantiate(MissilePrefab, RightMuzzle.position, Quaternion.LookRotation((TargetPosition - RightMuzzle.position).normalized));
				gameObject2.GetComponent<Missile>().Player = Target.transform;
				gameObject2.GetComponent<Missile>().Owner = base.transform;
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
					Physics.IgnoreCollision(gameObject2.GetComponentInChildren<Collider>(), collider);
				}
				AttackCount++;
				if (AttackCount > MaxAttack)
				{
					Aim = false;
					Animation = 0;
					StateEnd = Time.time + 1.3f;
				}
			}
			else
			{
				AttackCount = MaxAttack + 1;
				if (AttackCount > MaxAttack)
				{
					Aim = false;
					Animation = 0;
					StateEnd = Time.time + 1.3f;
				}
			}
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateShootBarrageEnd()
	{
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		StateEnd = Time.time + 7.5f;
		Animator.speed = 0f;
	}

	private void StateStuned()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateFly);
		}
	}

	private void StateStunedEnd()
	{
		Animator.speed = 1f;
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 0;
		StateEnd = Time.time + 1.9665f;
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateFly);
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 1f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			CreateHitFX(HitInfo, IgnoreTimer: true);
			OnImpact(HitInfo, Explosion: true);
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
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void OnHitDestroy(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (RobotMode == Mode.Chase)
		{
			OnHit(new HitInfo(base.transform, base.transform.forward * FlySpeed, 10));
		}
	}
}
