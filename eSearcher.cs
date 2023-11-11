using UnityEngine;

public class eSearcher : EnemyBase
{
	public enum Type
	{
		eSearcher = 0,
		eHunter = 1
	}

	public enum Mode
	{
		Alarm = 0,
		Fix = 1,
		Fix_Bomb = 2,
		Fix_Rocket = 3,
		Hide_Fix = 4,
		Normal = 5
	}

	public enum State
	{
		TransferFall = 0,
		Wait = 1,
		Alarm = 2,
		Search = 3,
		Seek = 4,
		Roam = 5,
		FollowPlayer = 6,
		MoveHeight = 7,
		ShootBombs = 8,
		ShootRockets = 9,
		ShootPodLaser = 10,
		ShootPodVulcan = 11,
		ShootVulcan = 12,
		Stuned = 13,
		DamageKnockBack = 14
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform[] BombCannons;

	public Transform GunMuzzle;

	public Transform PodCenter;

	public Transform PodPivot;

	public Transform[] PodPositions;

	public ParticleSystem AlarmFX;

	public Material[] CloakMats;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject PodPrefab;

	public GameObject BombPrefab;

	public GameObject RocketPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip[] eSeries;

	private Material[] NormalMats;

	private State EnemyState;

	private GameObject[] Pods;

	private bool Aim;

	private bool PodLaserHeight;

	private bool PodLaserFollow;

	private bool PodLaserHoming;

	private bool PodVulcanSiege;

	private bool HasRaised;

	private bool RaiseAttack;

	private bool Spotted;

	private bool HasUncloaked;

	private float CloakTimer;

	private float PodPivotRotation;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int AttackCount;

	private int RocketCount;

	private Vector3 HeightPos;

	private bool LaunchedBombs;

	private bool ShootOnce;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		DescentOffset *= 1000f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition + Vector3.up * (DescentOffset * ((RobotMode != 0) ? 1f : 0f));
		Pods = new GameObject[(EnemyType == Type.eSearcher) ? 3 : 6];
		if (EnemyType == Type.eHunter && RobotMode == Mode.Hide_Fix)
		{
			NormalMats = Renderer.materials;
		}
		BaseStart();
		Transfer();
	}

	private void Transfer()
	{
		if (Position == Vector3.zero)
		{
			return;
		}
		ToggleCloak(Toggle: true);
		base.transform.position = Position;
		base.transform.rotation = StartRotation;
		if (RobotMode != 0)
		{
			StateTransferFallStart();
			StateMachine.Initialize(StateTransferFall);
			if (RobotMode != Mode.Hide_Fix)
			{
				ManagePods("Deploy");
			}
		}
		else
		{
			StateWaitStart();
			StateMachine.Initialize(StateWait);
		}
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1.5f, StartRotation);
		}
		_Rigidbody.isKinematic = true;
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
		ManagePods("Reset");
		CurHealth = MaxHealth;
		base.transform.position = StartPosition + Vector3.up * (DescentOffset * ((RobotMode != 0) ? 1f : 0f));
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
		_Rigidbody.isKinematic = true;
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eHunter)
		{
			return 2;
		}
		return 0;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(2.75f);
		if (EnemyType == Type.eHunter && RobotMode == Mode.Hide_Fix && HasUncloaked && Time.time - CloakTimer > 3f)
		{
			HasUncloaked = false;
			ToggleCloak(Toggle: true);
		}
		if (EnemyState == State.ShootPodLaser && PodLaserHoming)
		{
			PodPivot.localEulerAngles = Vector3.Lerp(PodPivot.localEulerAngles, Vector3.zero, Time.deltaTime * 10f);
			PodCenter.rotation = Quaternion.Lerp(PodCenter.rotation, Quaternion.LookRotation((PodCenter.position - TargetPosition).normalized) * Quaternion.Euler(90f, 0f, 0f), Time.deltaTime * 10f);
		}
		else
		{
			Vector3 localEulerAngles = PodPivot.localEulerAngles;
			localEulerAngles.x = Mathf.Lerp(localEulerAngles.x, (EnemyState == State.ShootPodLaser && PodLaserHeight) ? 12.5f : 0f, Time.deltaTime * 5f);
			PodPivot.localEulerAngles = localEulerAngles;
			Vector3 localEulerAngles2 = PodCenter.localEulerAngles;
			localEulerAngles2.x = Mathf.Lerp(localEulerAngles2.x, 0f, Time.deltaTime * 5f);
			localEulerAngles2.z = Mathf.Lerp(localEulerAngles2.z, 0f, Time.deltaTime * 5f);
			PodCenter.localEulerAngles = localEulerAngles2;
			if (EnemyState != State.ShootPodVulcan && EnemyState != State.Stuned)
			{
				PodCenter.Rotate(0f, 56.25f * ((EnemyState == State.ShootPodLaser) ? 2f : 1f) * Time.deltaTime, 0f);
			}
		}
		Vector3 vector = base.transform.position + base.transform.up * 2.25f;
		Vector3 vector2 = new Vector3(TargetPosition.x, vector.y, TargetPosition.z);
		PodCenter.position = Vector3.MoveTowards(PodCenter.position, (EnemyState == State.ShootPodVulcan && PodVulcanSiege) ? vector2 : vector, Time.deltaTime * 10f);
	}

	private void LateUpdate()
	{
		if ((bool)Target && Aim)
		{
			Transform parent = GunMuzzle.parent;
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
			if (RobotMode != Mode.Hide_Fix)
			{
				ManagePods("Deploy");
			}
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, base.transform.position + Vector3.up * 1f, base.transform.rotation);
			}
			StateMachine.ChangeState(StateWait);
		}
		else
		{
			float t = Mathf.SmoothStep(0f, 1f, 1f - (StateTime[0] - Time.time));
			_Rigidbody.position = Vector3.Lerp(Position, StartPosition, t);
		}
	}

	private void StateTransferFallEnd()
	{
	}

	private void StateWaitStart()
	{
		EnemyState = State.Wait;
		Animation = 1;
		StateTime[0] = Time.time + ((EnemyType == Type.eHunter) ? 1f : 2.5f);
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

	private void StateAlarmStart()
	{
		EnemyState = State.Alarm;
		Animation = 1;
		StateTime[0] = Time.time + 2f;
		AlarmFX.Play();
		Audio.PlayOneShot(eSeries[3], Audio.volume);
	}

	private void StateAlarm()
	{
		if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateMoveHeight);
		}
	}

	private void StateAlarmEnd()
	{
	}

	private void StateSearchStart()
	{
		EnemyState = State.Search;
		Animation = 1;
		StateTime[0] = Time.time + 1.9665f;
		StateVar[0] = 0;
	}

	private void StateSearch()
	{
		if (GetTarget(Mesh.forward))
		{
			Audio.PlayOneShot(eSeries[0], Audio.volume * 1.5f);
			if (RobotMode == Mode.Alarm)
			{
				HasRaised = false;
				Spotted = true;
				StateMachine.ChangeState(StateMoveHeight);
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
			}
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
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if (RobotMode == Mode.Normal && Random.value > 0.5f)
			{
				StateMachine.ChangeState(StateRoam);
			}
			if (RobotMode == Mode.Alarm)
			{
				StateMachine.ChangeState(StateAlarm);
			}
		}
	}

	private void StateSearchEnd()
	{
	}

	private void StateSeekStart()
	{
		EnemyState = State.Seek;
		if (RobotMode == Mode.Alarm || RobotMode == Mode.Hide_Fix)
		{
			ManagePods("Deploy");
		}
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
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void ToAttackState()
	{
		switch (RobotMode)
		{
		case Mode.Alarm:
			if (GetDistance() < 18.9f)
			{
				HasRaised = false;
				RaiseAttack = true;
				StateMachine.ChangeState(StateMoveHeight);
			}
			else
			{
				HasRaised = true;
				StateMachine.ChangeState(StateMoveHeight);
			}
			break;
		case Mode.Fix:
			if (GetDistance() < 18.9f)
			{
				SubOnRangeAttack();
				break;
			}
			if (EnemyType == Type.eSearcher)
			{
				StateMachine.ChangeState(StateShootVulcan);
				break;
			}
			PodVulcanSiege = false;
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			StateMachine.ChangeState(StateShootPodVulcan);
			break;
		case Mode.Fix_Bomb:
			StateMachine.ChangeState(StateShootBombs);
			break;
		case Mode.Fix_Rocket:
			StateMachine.ChangeState(StateShootRockets);
			break;
		case Mode.Hide_Fix:
			if (GetDistance() < 18.9f)
			{
				SubOnRangeAttack();
				break;
			}
			if (EnemyType == Type.eSearcher)
			{
				StateMachine.ChangeState(StateShootVulcan);
				break;
			}
			PodVulcanSiege = false;
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			StateMachine.ChangeState(StateShootPodVulcan);
			break;
		case Mode.Normal:
			if (GetDistance() < 18.9f)
			{
				SubOnRangeAttack();
				break;
			}
			if (EnemyType == Type.eSearcher)
			{
				StateMachine.ChangeState(StateFollowPlayer);
				break;
			}
			PodVulcanSiege = true;
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			StateMachine.ChangeState(StateShootPodVulcan);
			break;
		}
	}

	private void SubOnRangeAttack()
	{
		switch (EnemyType)
		{
		case Type.eSearcher:
			if (GetDistance() < 6.3f)
			{
				StateMachine.ChangeState(StateShootBombs);
			}
			else if (GetDistance() < 12.6f)
			{
				PodLaserHeight = false;
				PodLaserFollow = false;
				PodLaserHoming = false;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateShootPodLaser);
			}
			else
			{
				PodVulcanSiege = RobotMode == Mode.Normal;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateShootPodVulcan);
			}
			break;
		case Type.eHunter:
			if (GetDistance() < 6.3f)
			{
				PodLaserHeight = true;
				PodLaserFollow = false;
				PodLaserHoming = false;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateShootPodLaser);
			}
			else if (GetDistance() < 12.6f)
			{
				PodLaserHeight = false;
				PodLaserFollow = false;
				PodLaserHoming = true;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateShootPodLaser);
			}
			else
			{
				PodLaserHeight = false;
				PodLaserFollow = RobotMode == Mode.Normal;
				PodLaserHoming = false;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateShootPodLaser);
			}
			break;
		}
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
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

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		StateEnd = Time.time + 1.5f;
		_Rigidbody.isKinematic = false;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = Mesh.forward * 3f;
		if (GetDistance() < 17.85f || Time.time > StateEnd)
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

	private void StateMoveHeightStart()
	{
		EnemyState = State.MoveHeight;
		Animation = 1;
		StateTime[0] = Time.time;
		HasRaised = !HasRaised;
		HeightPos = StartPosition;
		HeightPos.y += (HasRaised ? 4.725f : 0f);
	}

	private void StateMoveHeight()
	{
		if (Time.time - StateTime[0] > 2f)
		{
			if (RaiseAttack)
			{
				SubOnRangeAttack();
			}
			else if (Spotted)
			{
				StateMachine.ChangeState(StateSeek);
			}
			else
			{
				StateMachine.ChangeState(StateWait);
			}
		}
		else
		{
			float t = Mathf.SmoothStep(0f, 1f, (Time.time - StateTime[0]) / 12f);
			_Rigidbody.position = Vector3.Lerp(_Rigidbody.position, HeightPos, t);
		}
	}

	private void StateMoveHeightEnd()
	{
		RaiseAttack = false;
		Spotted = false;
		ManagePods("Deploy");
	}

	private void StateShootPodLaserStart()
	{
		EnemyState = State.ShootPodLaser;
		Animation = 1;
		if (PodLaserHeight)
		{
			StateEnd = Time.time + 7.5f;
			ManagePods("ShootLaserHeight");
		}
		else if (PodLaserHoming)
		{
			StateEnd = Time.time + 2.5f;
			ManagePods("ShootHomingLaser");
		}
		else
		{
			StateEnd = Time.time + 7.5f;
			ManagePods("ShootLaser");
		}
		_Rigidbody.isKinematic = false;
	}

	private void StateShootPodLaser()
	{
		if (PodLaserHoming)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		if (PodLaserFollow && Time.time > StateEnd - 6.5f)
		{
			if (GetDistance() > 4.2f)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
				_Rigidbody.velocity = Mesh.forward * 6f;
			}
			else
			{
				_Rigidbody.velocity = Vector3.zero;
			}
		}
		if (Time.time > StateEnd || (PodVulcanSiege && !Target))
		{
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateShootPodLaserEnd()
	{
		ManagePods("StopLaser");
		ManagePods("Deploy");
		_Rigidbody.isKinematic = true;
		_Rigidbody.velocity = Vector3.zero;
	}

	private void StateShootPodVulcanStart()
	{
		EnemyState = State.ShootPodVulcan;
		Animation = 1;
		StateEnd = Time.time + 3.5f;
		ManagePods("ShootVulcan");
	}

	private void StateShootPodVulcan()
	{
		if (Time.time > StateEnd)
		{
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			if (EnemyType == Type.eHunter && PodVulcanSiege)
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			else
			{
				StateMachine.ChangeState(StateWait);
			}
		}
	}

	private void StateShootPodVulcanEnd()
	{
		ManagePods("Deploy");
	}

	private void StateShootBombsStart()
	{
		EnemyState = State.ShootBombs;
		Animation = 2;
		StateTime[0] = Time.time + 1f;
		StateEnd = Time.time + 2.5f;
		LaunchedBombs = false;
	}

	private void StateShootBombs()
	{
		if (Time.time > StateTime[0] && !LaunchedBombs)
		{
			for (int i = 0; i < BombCannons.Length; i++)
			{
				DeployBombs(BombCannons[i]);
			}
			Audio.PlayOneShot(eSeries[2], Audio.volume * 2.5f);
			LaunchedBombs = true;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateShootBombsEnd()
	{
		ManagePods("Deploy");
	}

	private void StateShootRocketsStart()
	{
		EnemyState = State.ShootRockets;
		Animation = 2;
		RocketCount = 0;
		StateTime[0] = Time.time + 1f;
		StateEnd = Time.time + 2.5f;
		ShootOnce = !ShootOnce;
	}

	private void StateShootRockets()
	{
		if (Time.time > StateTime[0] + (float)RocketCount * 0.75f && RocketCount < (ShootOnce ? 1 : 2))
		{
			for (int i = 0; i < BombCannons.Length; i++)
			{
				DeployRockets(BombCannons[i]);
			}
			Audio.PlayOneShot(eSeries[2], Audio.volume * 2.5f);
			RocketCount++;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateShootRocketsEnd()
	{
		ManagePods("Deploy");
	}

	private void StateShootVulcanStart()
	{
		EnemyState = State.ShootVulcan;
		Animation = 1;
		AttackCount = 0;
		StateTime[0] = Time.time + 0.567f;
		StateTime[1] = 0f;
		Aim = true;
	}

	private void StateShootVulcan()
	{
		if (Time.time > StateTime[0] + (float)AttackCount * 0.125f && (float)AttackCount < 6f)
		{
			Object.Instantiate(VulcanBulletPrefab, GunMuzzle.position, Quaternion.LookRotation((TargetPosition - GunMuzzle.position).normalized));
			AttackCount++;
		}
		if ((float)AttackCount >= 6f)
		{
			if (StateTime[1] == 0f)
			{
				StateTime[1] += Time.time + 2f;
			}
			else if (Time.time > StateTime[1])
			{
				StateMachine.ChangeState(StateWait);
			}
			Aim = true;
		}
	}

	private void StateShootVulcanEnd()
	{
		ManagePods("Deploy");
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		StateEnd = Time.time + 7.5f;
		Animator.speed = 0f;
		ToggleCloak(Toggle: false);
	}

	private void StateStuned()
	{
		if (Time.time > StateEnd)
		{
			if (EnemyType == Type.eHunter && RobotMode == Mode.Hide_Fix)
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
		EnemyState = State.DamageKnockBack;
		Animation = 3;
		StateEnd = Time.time + 1.5f;
		ToggleCloak(Toggle: false);
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			if (EnemyType == Type.eHunter && RobotMode == Mode.Hide_Fix)
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
		if (EnemyType == Type.eHunter && RobotMode == Mode.Hide_Fix)
		{
			Renderer.materials = (Toggle ? CloakMats : NormalMats);
		}
	}

	private void DeployBombs(Transform Launcher)
	{
		GameObject gameObject = Object.Instantiate(BombPrefab, Launcher.position, Quaternion.identity);
		gameObject.GetComponent<Rigidbody>().AddForce(Launcher.forward * Random.Range(3f, 12f), ForceMode.Impulse);
		gameObject.GetComponent<TimedBomb>().Launched = true;
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
		}
	}

	private void DeployRockets(Transform Launcher)
	{
		GameObject gameObject = Object.Instantiate(RocketPrefab, Launcher.position, Quaternion.identity);
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 0.75f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
			ManagePods("Stun");
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			CreateHitFX(HitInfo, IgnoreTimer: true);
			OnImpact(HitInfo, Explosion: true);
			ManagePods("Destroy");
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
			ManagePods("Destroy");
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void ManagePods(string Action)
	{
		for (int i = 0; i < Pods.Length; i++)
		{
			if (!Pods[i])
			{
				if (Action == "Deploy")
				{
					Pods[i] = Object.Instantiate(PodPrefab, base.transform.position + Vector3.up * 1f, base.transform.rotation);
					Pods[i].GetComponent<Pod>().Pivot = PodPositions[i];
					Pods[i].transform.SetParent(base.transform);
				}
			}
			else
			{
				if (!Pods[i])
				{
					continue;
				}
				switch (Action)
				{
				case "Reset":
					if ((bool)TeleportEffectPrefab)
					{
						Object.Instantiate(TeleportEffectPrefab, Pods[i].transform.position, Pods[i].transform.rotation);
					}
					Pods[i].SendMessage("Destroy", SendMessageOptions.DontRequireReceiver);
					break;
				case "ShootLaser":
					Pods[i].SendMessage(Action, false, SendMessageOptions.DontRequireReceiver);
					break;
				case "ShootLaserHeight":
					Pods[i].SendMessage("ShootLaser", true, SendMessageOptions.DontRequireReceiver);
					break;
				case "ShootHomingLaser":
					Pods[i].SendMessage(Action, Target.transform, SendMessageOptions.DontRequireReceiver);
					break;
				case "StopLaser":
					Pods[i].SendMessage(Action, SendMessageOptions.DontRequireReceiver);
					break;
				case "ShootVulcan":
					Pods[i].SendMessage(Action, Target.transform, SendMessageOptions.DontRequireReceiver);
					break;
				case "Stun":
					Pods[i].SendMessage("OnFlash", SendMessageOptions.DontRequireReceiver);
					break;
				case "Destroy":
					Pods[i].transform.SetParent(null);
					Pods[i].SendMessage("OnDestroyPod", SendMessageOptions.DontRequireReceiver);
					break;
				}
			}
		}
	}
}
