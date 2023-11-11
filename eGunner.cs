using UnityEngine;

public class eGunner : EnemyBase
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
		Normal = 5,
		Trans = 6
	}

	public enum State
	{
		TransferFall = 0,
		Stand = 1,
		Search = 2,
		Seek = 3,
		Roam = 4,
		Move = 5,
		MoveParallel = 6,
		ShootVulcan = 7,
		ShootRocket = 8,
		ShootMissile = 9,
		ShootBarrage = 10,
		Stuned = 11,
		DamageKnockBack = 12
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform LeftMuzzle;

	public Transform RightMuzzle;

	public ParticleSystem[] JetFireFX;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject MissilePrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioSource HoverAudio;

	public AudioClip landSound;

	public AudioClip vulcanChargeSound;

	private BezierCurve PathSpline;

	private Vector3 DirectionPosition;

	private State EnemyState;

	private bool Aim;

	private float SplineTime;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int MaxAttack;

	private int AttackCount;

	private float DistFromPlayer;

	private float RoundPlayer;

	private bool MoveSwitch;

	private bool CanFlyAgain;

	private int SiegeType;

	private PlayerBase Player;

	private float Speed;

	private float Dist;

	private bool Shooted;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		if (AppearPath != "")
		{
			PathSpline = GameObject.Find("Stage/Splines/" + AppearPath).GetComponent<BezierCurve>();
		}
		DescentOffset *= ((EnemyType != Type.eBuster) ? 1500f : 1000f);
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = (IsFixed ? StartPosition : (StartPosition + Vector3.up * DescentOffset));
		BaseStart();
		Transfer();
	}

	private void Transfer()
	{
		if (!(Position == Vector3.zero))
		{
			_Rigidbody.velocity = Physics.gravity * 0.5f;
			Grounded = false;
			base.transform.position = ((AppearPath != "") ? PathSpline.GetPosition(SplineTime) : Position);
			base.transform.rotation = StartRotation;
			DirectionPosition = (StartPosition - base.transform.position).normalized;
			if (!IsFixed)
			{
				StateTransferFallStart();
				StateMachine.Initialize(StateTransferFall);
			}
			else
			{
				StateStandStart();
				StateMachine.Initialize(StateStand);
			}
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
			Target = null;
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
			ManageJetFire(Enable: false);
			CurHealth = MaxHealth;
			SplineTime = 0f;
			base.transform.position = StartPosition + Vector3.up * DescentOffset;
			base.transform.rotation = StartRotation;
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

	private float MoveDist(int _Type)
	{
		if (EnemyType == Type.eGunner)
		{
			switch (_Type)
			{
			default:
				return 15.75f;
			case 1:
				return 10.5f;
			case 0:
				return 7.875f;
			}
		}
		switch (_Type)
		{
		default:
			return 21f;
		case 1:
			return 9.45f;
		case 0:
			return 7.875f;
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
		int num = RandomInt(2, 3);
		Type enemyType = EnemyType;
		if (enemyType == Type.eStinger)
		{
			num = 1;
		}
		return num - 1;
	}

	private int GetSubAmmo()
	{
		int result = RandomInt(3, 5);
		switch (EnemyType)
		{
		case Type.eStinger:
			result = ((RobotMode == Mode.Normal) ? RandomInt(4, 6) : RandomInt(3, 5));
			break;
		case Type.eLancer:
			result = ((RobotMode == Mode.Normal) ? RandomInt(4, 6) : RandomInt(3, 5));
			break;
		}
		return result;
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
		if (AppearPath != "" && (bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.25f, StartRotation);
		}
	}

	private void StateTransferFall()
	{
		if (Grounded)
		{
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateStand);
			return;
		}
		if (AppearPath == "")
		{
			if (Physics.Raycast(base.transform.position, DirectionPosition.normalized, out var hitInfo))
			{
				Debug.DrawLine(base.transform.position, hitInfo.point, Color.green);
				if (base.transform.position.y < hitInfo.point.y + 0.5f)
				{
					_Rigidbody.velocity = Physics.gravity * 0.5f;
				}
			}
			return;
		}
		_Rigidbody.velocity = Vector3.zero;
		SplineTime += AppearVelocity / PathSpline.Length() * Time.fixedDeltaTime;
		base.transform.position = PathSpline.GetPosition(SplineTime);
		base.transform.rotation = Quaternion.LookRotation(PathSpline.GetTangent(SplineTime).MakePlanar());
		if (SplineTime >= 1f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateTransferFallEnd()
	{
		Audio.PlayOneShot(landSound, Audio.volume);
	}

	private void StateStandStart()
	{
		EnemyState = State.Stand;
		Animation = 1;
		StateTime[0] = Time.time + 2.5f;
	}

	private void StateStand()
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

	private void StateStandEnd()
	{
	}

	private void StateSearchStart()
	{
		EnemyState = State.Search;
		Animation = 2;
		StateTime[0] = Time.time + 2f;
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
			TargetPosition = base.transform.position + Vector3.up - (((RobotMode == Mode.Normal || RobotMode == Mode.Chase) && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 2;
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if ((RobotMode == Mode.Normal || RobotMode == Mode.Chase) && Random.value > 0.5f)
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
		Animation = 3;
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
	}

	private void StateSeek()
	{
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Audio.PlayOneShot(vulcanChargeSound, Audio.volume);
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
		Animation = 10;
		StateEnd = Time.time + 3f;
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * 1.5f);
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateRoamEnd()
	{
	}

	private void StateMoveStart()
	{
		EnemyState = State.Move;
		Animation = 9;
		StateEnd = Time.time + 3f;
		MoveSwitch = Random.value < 0.75f;
		StateVar[0] = 0;
		ManageJetFire(Enable: true);
	}

	private void StateMove()
	{
		if (StateVar[0] == 0)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			if (SiegeType != 1)
			{
				DistFromPlayer = (((bool)Target && GetDistance() > MoveDist(SiegeType)) ? 6f : 0f);
				RoundPlayer = (((bool)Target && GetDistance() < MoveDist(SiegeType)) ? 1f : 0f);
			}
			else
			{
				DistFromPlayer = (((bool)Target && GetDistance() < MoveDist(SiegeType)) ? (-6f) : 0f);
				RoundPlayer = (((bool)Target && GetDistance() > MoveDist(SiegeType)) ? 1f : 0f);
			}
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			_Rigidbody.velocity = Mesh.right * ((MoveSwitch ? 5f : (-5f)) * RoundPlayer) + Mesh.forward * DistFromPlayer;
			if (Time.time > StateEnd)
			{
				StateEnd = Time.time + 1f;
				Animation = 1;
				StateVar[0] = 1;
				ManageJetFire(Enable: false);
			}
		}
		else if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			if (SiegeType == 0)
			{
				if (EnemyType != 0)
				{
					SecondaryAttack();
				}
				else
				{
					StateMachine.ChangeState(StateShootVulcan);
				}
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
			}
		}
		if (!Target)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveEnd()
	{
		ManageJetFire(Enable: false);
	}

	private void StateMoveParallelStart()
	{
		EnemyState = State.MoveParallel;
		Animation = 9;
		ManageJetFire(Enable: true);
		Player = Target.GetComponent<PlayerBase>();
		Speed = Player.CurSpeed;
	}

	private void StateMoveParallel()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Speed = Mathf.Lerp(Speed, Player.CurSpeed, Time.fixedDeltaTime * 5f);
		Dist = Mathf.Lerp(Dist, ((bool)Target && GetDistance() > 10.5f) ? 6f : 0f, Time.fixedDeltaTime * 5f);
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = Mesh.forward * Dist + Target.transform.forward * (Speed * ((GetDistance() > 10.5f) ? 1f : 2f));
		if (!Target)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveParallelEnd()
	{
		ManageJetFire(Enable: false);
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
		if (EnemyType != Type.eBuster)
		{
			if (RobotMode == Mode.Chase)
			{
				StateMachine.ChangeState(StateMoveParallel);
			}
			else if (RobotMode == Mode.Fix_Vulcan)
			{
				StateMachine.ChangeState(StateShootVulcan);
			}
			else if (RobotMode == Mode.Fix_Rocket || RobotMode == Mode.Fix_Missile)
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
					if (GetDistance() < 6.3f)
					{
						SiegeType = 1;
						CanFlyAgain = true;
						StateMachine.ChangeState(StateShootVulcan);
					}
					else if (GetDistance() < 12.6f)
					{
						if (CanFlyAgain)
						{
							SiegeType = 0;
							StateMachine.ChangeState(StateMove);
						}
						else if (EnemyType != 0)
						{
							SecondaryAttack();
						}
						else
						{
							StateMachine.ChangeState(StateShootVulcan);
						}
					}
					else
					{
						SecondaryAttack();
					}
				}
				else
				{
					SiegeType = 2;
					CanFlyAgain = false;
					StateMachine.ChangeState(StateMove);
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
		Animation = 4;
		AttackCount = 0;
		MaxAttack = GetSubAmmo();
		StateTime[0] = Time.time + 0.567f;
		StateTime[1] = -1f;
		Aim = true;
	}

	private void StateShootVulcan()
	{
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
				Animation = 1;
				Aim = false;
			}
		}
		else if (Time.time > StateTime[1])
		{
			if (RobotMode == Mode.Normal && SiegeType == 1)
			{
				StateMachine.ChangeState(StateMove);
			}
			else
			{
				StateMachine.ChangeState(StateStand);
			}
		}
	}

	private void StateShootVulcanEnd()
	{
	}

	private void StateShootRocketStart()
	{
		EnemyState = State.ShootRocket;
		Animation = 5;
		AttackCount = 0;
		MaxAttack = GetMainAmmo();
		StateTime[0] = Time.time + 0.6335f;
		StateEnd = Time.time + 10f;
		Aim = true;
	}

	private void StateShootRocket()
	{
		if (AttackCount <= MaxAttack && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		}
		if (Time.time > StateTime[0] + (float)AttackCount * 0.3f && AttackCount <= MaxAttack)
		{
			if ((bool)Target)
			{
				if (AttackCount == MaxAttack)
				{
					Animation = 6;
				}
				else
				{
					Animator.SetTrigger("Attack");
				}
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
			Animation = 1;
			Aim = false;
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootRocketEnd()
	{
	}

	private void StateShootMissileStart()
	{
		EnemyState = State.ShootMissile;
		Animation = 5;
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
			TargetPosition.y = RightMuzzle.position.y;
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		}
		if (Time.time > StateTime[0] && !Shooted)
		{
			Animation = 6;
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
			Animation = 1;
			Aim = false;
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootMissileEnd()
	{
	}

	private void StateShootBarrageStart()
	{
		EnemyState = State.ShootBarrage;
		Animation = 8;
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
				GameObject gameObject = Object.Instantiate(MissilePrefab, LeftMuzzle.position, LeftMuzzle.rotation);
				gameObject.GetComponent<Missile>().Player = Target.transform;
				gameObject.GetComponent<Missile>().Owner = base.transform;
				GameObject gameObject2 = Object.Instantiate(MissilePrefab, RightMuzzle.position, RightMuzzle.rotation);
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
					Animation = 1;
					StateEnd = Time.time + 1.3f;
				}
			}
			else
			{
				AttackCount = MaxAttack + 1;
				if (AttackCount > MaxAttack)
				{
					Aim = false;
					Animation = 1;
					StateEnd = Time.time + 1.3f;
				}
			}
		}
		if (Time.time > StateEnd + 2f)
		{
			StateMachine.ChangeState(StateStand);
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
			StateMachine.ChangeState(StateStand);
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
		Animation = 7;
		StateEnd = Time.time + 1.9665f;
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void ManageJetFire(bool Enable)
	{
		for (int i = 0; i < JetFireFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = JetFireFX[i].emission;
			emission.enabled = Enable;
		}
		if (Enable)
		{
			HoverAudio.Play();
		}
		else
		{
			HoverAudio.Stop();
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

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		TrackGroundedBox(collision);
	}

	private void OnCollisionStay(Collision collision)
	{
		TrackGroundedBox(collision);
	}
}
