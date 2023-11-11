using UnityEngine;

public class eFlyer : EnemyBase
{
	public enum Type
	{
		eFlyer = 0,
		eBluster = 1
	}

	public enum Mode
	{
		Boss_Homing = 0,
		Boss_Vulcan = 1,
		Fix = 2,
		Fix_Rocket = 3,
		Fix_Vulcan = 4,
		Homing = 5,
		Normal = 6
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform GunMuzzle;

	public Material[] CloakMats;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject MissilePrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip diskSound;

	public AudioClip vulcanChargeSound;

	private Material[] NormalMats;

	private bool AttackModeToggle;

	private bool MoveToggle;

	private bool HasUncloaked;

	private float CloakTimer;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int MaxAttack;

	private int AttackCount;

	private float DistFromPlayer;

	private float RoundPlayer;

	private bool MoveSwitch;

	private int SiegeType;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		DescentOffset *= 1250f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = base.transform.position + Vector3.up * DescentOffset;
		if (EnemyType == Type.eBluster)
		{
			NormalMats = Renderer.materials;
		}
		BaseStart();
		Transfer();
	}

	private void Transfer()
	{
		if (!(Position == Vector3.zero))
		{
			ToggleCloak(Toggle: true);
			StateTime[0] = Time.time + DescentOffset / 9.81f;
			base.transform.position = StartPosition + Vector3.up * DescentOffset;
			base.transform.rotation = StartRotation;
			StateTransferFallStart();
			StateMachine.Initialize(StateTransferFall);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.5f, StartRotation);
			}
			AttackModeToggle = false;
			_Rigidbody.isKinematic = true;
		}
	}

	private void Reset()
	{
		Target = null;
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
		base.transform.position = StartPosition + Vector3.up * DescentOffset;
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
		_Rigidbody.isKinematic = true;
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eBluster)
		{
			return 1;
		}
		return 0;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(1.6f);
		if (EnemyType == Type.eBluster && HasUncloaked && Time.time - CloakTimer > 3f)
		{
			HasUncloaked = false;
			ToggleCloak(Toggle: true);
		}
	}

	private void StateTransferFallStart()
	{
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
		Animation = 1;
		StateTime[0] = Time.time + 2.5f;
	}

	private void StateWait()
	{
		if (GetTarget(Mesh.forward))
		{
			Audio.PlayOneShot(diskSound, Audio.volume);
			StateMachine.ChangeState(StateSeek);
		}
		else if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateWaitEnd()
	{
	}

	private void StateSearchStart()
	{
		Animation = 1;
		StateTime[0] = Time.time + 1.9665f;
		StateVar[0] = 0;
	}

	private void StateSearch()
	{
		if (GetTarget(Mesh.forward))
		{
			Audio.PlayOneShot(diskSound, Audio.volume);
			StateMachine.ChangeState(StateSeek);
		}
		if (!(Time.time > StateTime[0]))
		{
			return;
		}
		if (StateVar[0] == 0)
		{
			TargetPosition = base.transform.position + Vector3.up - ((EnemyType == Type.eFlyer && RobotMode == Mode.Normal && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if (RobotMode == Mode.Normal && Random.value > 0.5f)
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
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (RobotMode == Mode.Homing || RobotMode == Mode.Boss_Homing || !(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir))
		{
			return;
		}
		switch (RobotMode)
		{
		case Mode.Boss_Vulcan:
			StateMachine.ChangeState(StateShootVulcan);
			break;
		case Mode.Fix:
			if ((EnemyType != Type.eBluster && !AttackModeToggle) || EnemyType == Type.eBluster)
			{
				StateMachine.ChangeState(StateShootRocket);
			}
			else
			{
				StateMachine.ChangeState(StateShootVulcan);
			}
			break;
		case Mode.Fix_Vulcan:
			StateMachine.ChangeState(StateShootVulcan);
			break;
		case Mode.Fix_Rocket:
			StateMachine.ChangeState(StateShootRocket);
			break;
		case Mode.Normal:
			if (MoveToggle)
			{
				if ((EnemyType != Type.eBluster && !AttackModeToggle) || EnemyType == Type.eBluster)
				{
					StateMachine.ChangeState(StateShootRocket);
				}
				else
				{
					StateMachine.ChangeState(StateShootVulcan);
				}
			}
			else
			{
				StateMachine.ChangeState(StateMove);
			}
			break;
		case Mode.Homing:
			break;
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateRoamStart()
	{
		Animation = 1;
		StateEnd = Time.time + 1.5f;
		_Rigidbody.isKinematic = false;
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = Mesh.forward * 3f;
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
		Animation = 9;
		StateEnd = Time.time + 2f;
		MoveSwitch = Random.value < 0.75f;
		StateVar[0] = 0;
		MoveToggle = true;
		_Rigidbody.isKinematic = false;
	}

	private void StateMove()
	{
		if (StateVar[0] == 0)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			DistFromPlayer = (((bool)Target && GetDistance() > 7.875f) ? 6f : 0f);
			RoundPlayer = (((bool)Target && GetDistance() < 7.875f) ? 1f : 0f);
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			_Rigidbody.velocity = Mesh.right * ((MoveSwitch ? 5f : (-5f)) * RoundPlayer) + Mesh.forward * DistFromPlayer;
			if (Time.time > StateEnd)
			{
				StateEnd = Time.time + 1f;
				Animation = 1;
				_Rigidbody.isKinematic = true;
				StateVar[0] = 1;
			}
		}
		else if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateSeek);
		}
		if (!Target)
		{
			StateEnd = 0f;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateMoveEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateShootVulcanStart()
	{
		AttackCount = 0;
		MaxAttack = RandomInt(6, 8);
		StateTime[0] = Time.time + 0.567f;
		StateTime[1] = 0f;
		Audio.PlayOneShot(vulcanChargeSound, Audio.volume);
	}

	private void StateShootVulcan()
	{
		if (Time.time > StateTime[0] + (float)AttackCount * 0.125f && AttackCount < MaxAttack)
		{
			Object.Instantiate(VulcanBulletPrefab, GunMuzzle.position, Quaternion.LookRotation((TargetPosition - GunMuzzle.position).normalized));
			AttackCount++;
		}
		if (AttackCount >= MaxAttack)
		{
			if (StateTime[1] == 0f)
			{
				StateTime[1] += Time.time + 2f;
			}
			else if (Time.time > StateTime[1])
			{
				AttackModeToggle = false;
				MoveToggle = false;
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateShootVulcanEnd()
	{
		AttackModeToggle = false;
		MoveToggle = false;
	}

	private void StateShootRocketStart()
	{
		AttackCount = 0;
		MaxAttack = RandomInt(2, 3);
		StateTime[0] = Time.time + 0.6335f;
		StateTime[1] = 0f;
		Audio.PlayOneShot(vulcanChargeSound, Audio.volume);
	}

	private void StateShootRocket()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			Vector3 normalized = (Target.transform.position + Target.transform.up * 0.25f - base.transform.position).normalized;
			normalized.y = 0f;
			Mesh.rotation = Quaternion.Slerp(Mesh.rotation, Quaternion.LookRotation(normalized), Time.fixedDeltaTime * 4f);
		}
		else
		{
			StateMachine.ChangeState(StateWait);
		}
		if (Time.time > StateTime[0] && AttackCount < MaxAttack)
		{
			GameObject gameObject = Object.Instantiate(MissilePrefab, GunMuzzle.position, Quaternion.LookRotation((TargetPosition - GunMuzzle.position).normalized));
			gameObject.GetComponent<Missile>().Player = Target.transform;
			gameObject.GetComponent<Missile>().Owner = base.transform;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
			}
			StateTime[0] = Time.time + 1f;
			AttackCount++;
		}
		if (AttackCount >= MaxAttack)
		{
			if (StateTime[1] == 0f)
			{
				StateTime[1] += Time.time + ((EnemyType == Type.eFlyer) ? 2f : 3f);
			}
			else if (Time.time > StateTime[1])
			{
				AttackModeToggle = true;
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateShootRocketEnd()
	{
	}

	private void StateStunedStart()
	{
		StateEnd = Time.time + 7.5f;
		Animator.speed = 0f;
		ToggleCloak(Toggle: false);
	}

	private void StateStuned()
	{
		if (Time.time > StateEnd)
		{
			if (EnemyType == Type.eBluster)
			{
				HasUncloaked = true;
				CloakTimer = Time.time;
			}
			StateMachine.ChangeState(StateWait);
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
		Animation = 2;
		StateTime[0] = Time.time + 1.65f;
		ToggleCloak(Toggle: false);
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateTime[0])
		{
			if (EnemyType == Type.eBluster)
			{
				HasUncloaked = true;
				CloakTimer = Time.time;
			}
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void ToggleCloak(bool Toggle)
	{
		if (EnemyType == Type.eBluster)
		{
			Renderer.materials = (Toggle ? CloakMats : NormalMats);
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
}
