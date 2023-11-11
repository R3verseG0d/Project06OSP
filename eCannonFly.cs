using UnityEngine;

public class eCannonFly : EnemyBase
{
	public enum Mode
	{
		Carrier = 0,
		Fix = 1,
		Normal = 2,
		Trans = 3
	}

	public enum State
	{
		TransferFall = 0,
		Wait = 1,
		Search = 2,
		Seek = 3,
		Roam = 4,
		FollowPlayer = 5,
		ShootVulcan = 6,
		LaunchBarrage = 7,
		Stuned = 8,
		DamageKnockBack = 9
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Transform Muzzle;

	public Transform[] MissileComports;

	public Transform ParalysisPoint;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject MissilePrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioSource MissileAudio;

	public AudioClip[] eSeries;

	public AudioClip VulcanChargeSound;

	private State EnemyState;

	private bool Aim;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int AttackCount;

	private int MaxAttack;

	private void Start()
	{
		MaxHealth = 2;
		CurHealth = MaxHealth;
		DescentOffset *= 1000f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition + Vector3.up * (DescentOffset * ((RobotMode != 0) ? 1f : 0f));
		BaseStart();
		Transfer();
	}

	private void Transfer()
	{
		if (!(Position == Vector3.zero))
		{
			base.transform.position = Position;
			base.transform.rotation = StartRotation;
			_Rigidbody.velocity = Vector3.zero;
			if (RobotMode != 0)
			{
				StateTransferFallStart();
				StateMachine.Initialize(StateTransferFall);
			}
			else
			{
				StateWaitStart();
				StateMachine.Initialize(StateWait);
			}
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 2.25f, StartRotation);
			}
			_Rigidbody.isKinematic = true;
		}
	}

	private void Reset()
	{
		Target = null;
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 2.25f, StartRotation);
		}
		if ((bool)ParalysisEffect)
		{
			Animator.speed = 1f;
			Stuned = false;
			Object.Destroy(ParalysisEffect);
		}
		CurHealth = MaxHealth;
		base.transform.position = StartPosition + Vector3.up * (DescentOffset * ((RobotMode != 0) ? 1f : 0f));
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
		_Rigidbody.isKinematic = true;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(5f);
	}

	private void LateUpdate()
	{
		if ((bool)Target && Aim && EnemyState == State.ShootVulcan)
		{
			Transform parent = Muzzle.parent.parent;
			Vector3 normalized = (TargetPosition - parent.position).normalized;
			parent.eulerAngles = Quaternion.LookRotation(normalized, parent.up).eulerAngles;
		}
	}

	private void StateTransferFallStart()
	{
		EnemyState = State.TransferFall;
		Animation = 0;
		StateTime[0] = Time.time + DescentOffset / 9.81f;
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
		StateTime[0] = Time.time + 3f;
	}

	private void StateWait()
	{
		if (Time.time > StateTime[0])
		{
			if (GetTarget(Mesh.forward))
			{
				StateMachine.ChangeState(StateSeek);
			}
			else
			{
				StateMachine.ChangeState(StateSearch);
			}
		}
	}

	private void StateWaitEnd()
	{
	}

	private void StateSearchStart()
	{
		EnemyState = State.Search;
		Animation = 0;
		StateTime[0] = Time.time + 3f;
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
			TargetPosition = base.transform.position + Vector3.up - ((RobotMode == Mode.Normal && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			StateTime[0] = Time.time + 2.65f;
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
		EnemyState = State.Seek;
		Animation = 0;
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		else
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateSeek()
	{
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir && RobotMode != 0)
		{
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void ToAttackState()
	{
		if (GetDistance() < 7.875f)
		{
			Audio.PlayOneShot(VulcanChargeSound, Audio.volume);
			StateMachine.ChangeState(StateShootVulcan);
		}
		else
		{
			StateMachine.ChangeState(StateLaunchBarrage);
		}
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
		Animation = 0;
		StateEnd = Time.time + 2f;
		_Rigidbody.isKinematic = false;
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = Mesh.forward * 3f;
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateRoamEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		Animation = 0;
		StateEnd = Time.time + 2f;
		_Rigidbody.isKinematic = true;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = Mesh.forward * 3f;
		if (GetDistance() < 7.875f || Time.time > StateEnd)
		{
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateSeek);
		}
		if (GetDistance() > 40f || !Target)
		{
			Target = null;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateFollowPlayerEnd()
	{
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateShootVulcanStart()
	{
		EnemyState = State.ShootVulcan;
		Animation = 1;
		AttackCount = 0;
		MaxAttack = RandomInt(6, 8);
		StateTime[0] = Time.time + 0.55f;
		StateTime[1] = -1f;
		Aim = true;
	}

	private void StateShootVulcan()
	{
		if (Time.time > StateTime[0] + (float)AttackCount * 0.125f && AttackCount < MaxAttack)
		{
			Vector3 normalized = (TargetPosition - Muzzle.position).normalized;
			normalized.x += Random.Range(-0.025f, 0.025f);
			normalized.y += Random.Range(-0.025f, 0.025f);
			normalized.z += Random.Range(-0.025f, 0.025f);
			normalized.Normalize();
			Object.Instantiate(VulcanBulletPrefab, Muzzle.position, Quaternion.LookRotation(normalized));
			AttackCount++;
		}
		if (StateTime[1] == -1f)
		{
			if (AttackCount >= MaxAttack)
			{
				StateTime[1] = Time.time + 1f;
				Aim = false;
			}
		}
		else if (Time.time > StateTime[1])
		{
			if (RobotMode == Mode.Normal)
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			else
			{
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateShootVulcanEnd()
	{
	}

	private void StateLaunchBarrageStart()
	{
		EnemyState = State.LaunchBarrage;
		Animation = 0;
		AttackCount = 0;
		MaxAttack = RandomInt(1, 2);
		StateTime[0] = Time.time + 0.25f;
		StateEnd = Time.time + 1.25f;
		Audio.PlayOneShot(eSeries[0], Audio.volume);
	}

	private void StateLaunchBarrage()
	{
		if (Time.time > StateTime[0] + (float)AttackCount * 2f && AttackCount < MaxAttack)
		{
			for (int i = 0; i < MissileComports.Length; i++)
			{
				FireMissiles(MissileComports[i]);
			}
			MissileAudio.Play();
			AttackCount++;
		}
		if (Time.time > StateEnd + (float)AttackCount * 2f)
		{
			if (RobotMode == Mode.Normal)
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			else
			{
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateLaunchBarrageEnd()
	{
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		StateEnd = Time.time + 7.5f;
		Animator.speed = 0f;
		_Rigidbody.velocity = Vector3.zero;
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
		Animator.speed = 1f;
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 0;
		StateEnd = Time.time + 0.8f;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void FireMissiles(Transform Launcher)
	{
		GameObject gameObject = Object.Instantiate(MissilePrefab, Launcher.position, Quaternion.identity);
		gameObject.transform.forward = Launcher.forward;
		gameObject.GetComponent<Missile>().Player = Target.transform;
		gameObject.GetComponent<Missile>().Owner = base.transform;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, ParalysisPoint.position, Quaternion.identity);
			ParalysisEffect.transform.SetParent(ParalysisPoint);
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
