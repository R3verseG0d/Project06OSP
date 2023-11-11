using UnityEngine;

public class eCannon : EnemyBase
{
	public enum Type
	{
		eCannon = 0,
		eWalker = 1
	}

	public enum Mode
	{
		Fix = 0,
		Fix_Laser = 1,
		Fix_Launcher = 2,
		Normal = 3,
		Trans = 4,
		Wall_Fix = 5
	}

	public enum State
	{
		TransferFall = 0,
		Stand = 1,
		Search = 2,
		Seek = 3,
		Roam = 4,
		FollowPlayer = 5,
		ShootVulcan = 6,
		ShootLaserBeam = 7,
		FireOscillators = 8,
		LaunchBarrage = 9,
		Stuned = 10,
		StunedBack = 11,
		DamageKnockBack = 12
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform Muzzle;

	public Transform[] MissileComports;

	public Transform[] Oscillators;

	public Collider[] LegHitboxes;

	public GameObject[] LegTargets;

	public Transform ParalysisPoint;

	[Header("Instantiation")]
	public GameObject VulcanBulletPrefab;

	public GameObject MissilePrefab;

	public GameObject OscillatorBeamPrefab;

	public GameObject LaserBeamPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioSource MissileAudio;

	public AudioClip[] eSeries;

	public AudioClip VulcanChargeSound;

	private BezierCurve PathSpline;

	private State EnemyState;

	private LaserBeam[] OscillatorBeam;

	private LaserBeam[] LaserGunBeam;

	private Vector3 DirectionPosition;

	private LaserBeam LaserBeam;

	private bool Aim;

	private bool IsOscillatorActive;

	private bool LaunchedMissile;

	private float SplineTime;

	private float FootBrokenWait;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private float FootHitPoint;

	private int[] StateVar = new int[10];

	private int LastAnimation;

	private int AttackCount;

	private int MaxAttack;

	private string StunSource;

	private string LastStunSource;

	private bool FullStunMode;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		if (AppearPath != "")
		{
			PathSpline = GameObject.Find("Stage/Splines/" + AppearPath).GetComponent<BezierCurve>();
		}
		DescentOffset *= 1000f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition + Vector3.up * DescentOffset;
		BaseStart();
		Transfer();
		OscillatorBeam = new LaserBeam[2];
		for (int i = 0; i < Oscillators.Length; i++)
		{
			OscillatorBeam[i] = Object.Instantiate(OscillatorBeamPrefab, Oscillators[i].position, Oscillators[i].rotation).GetComponent<LaserBeam>();
		}
		if (EnemyType == Type.eWalker)
		{
			LaserGunBeam = new LaserBeam[6];
			for (int j = 0; j < MissileComports.Length; j++)
			{
				LaserGunBeam[j] = Object.Instantiate(LaserBeamPrefab, MissileComports[j].position, MissileComports[j].rotation).GetComponent<LaserBeam>();
			}
			LaserBeam = Object.Instantiate(LaserBeamPrefab, Muzzle.position, Muzzle.rotation, Muzzle).GetComponent<LaserBeam>();
		}
		FootHitPoint = FootMaxHealth();
		FootBrokenWait = ((EnemyType == Type.eWalker) ? 5f : ((RobotMode == Mode.Normal || RobotMode == Mode.Trans) ? 10f : 7.5f));
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
			StateTransferFallStart();
			StateMachine.Initialize(StateTransferFall);
			if ((bool)TeleportEffectPrefab && (AppearPath == "" || (EnemyState == State.TransferFall && AppearPath != "")))
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 2.25f, StartRotation);
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
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 2.25f, StartRotation);
			}
			if ((bool)ParalysisEffect)
			{
				Stuned = false;
				Object.Destroy(ParalysisEffect);
			}
			CurHealth = MaxHealth;
			DeactivateOscillators();
			if (EnemyType == Type.eWalker)
			{
				DeactivateLaserGuns();
				LaserBeam.State = 2;
			}
			SplineTime = 0f;
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
		Type enemyType = EnemyType;
		if (enemyType == Type.eWalker)
		{
			return 5;
		}
		return 2;
	}

	private int FootMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eWalker)
		{
			return 2;
		}
		return 1;
	}

	private int GetSpecialAmmo()
	{
		int result = ((RobotMode == Mode.Fix_Launcher) ? RandomInt(2, 3) : RandomInt(1, 2));
		Type enemyType = EnemyType;
		if (enemyType == Type.eWalker)
		{
			result = 1;
		}
		return result;
	}

	public override void FixedUpdate()
	{
		if (Animator.GetInteger("Animation") != Animation)
		{
			LastAnimation = Animator.GetInteger("Animation");
		}
		base.FixedUpdate();
		Animator.SetInteger("Last Animation", LastAnimation);
		if (EnemyState == State.FireOscillators)
		{
			for (int i = 0; i < OscillatorBeam.Length; i++)
			{
				Vector3 targetPos;
				if (Physics.Raycast(Oscillators[i].position, Oscillators[i].forward, out var hitInfo, 65f))
				{
					targetPos = hitInfo.point;
					if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
					{
						HitInfo value = new HitInfo(base.transform, Oscillators[i].forward * 25f, 0);
						hitInfo.transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
					}
				}
				else
				{
					targetPos = Oscillators[i].position + Oscillators[i].forward * 65f;
				}
				OscillatorBeam[i].transform.position = Oscillators[i].position - Oscillators[i].forward * 0.8f;
				OscillatorBeam[i].UpdateBeam(targetPos);
			}
		}
		if (EnemyType != Type.eWalker || EnemyState != State.LaunchBarrage)
		{
			return;
		}
		for (int j = 0; j < LaserGunBeam.Length; j++)
		{
			Vector3 targetPos2;
			if (Physics.Raycast(MissileComports[j].position, MissileComports[j].forward, out var hitInfo2, 65f))
			{
				targetPos2 = hitInfo2.point;
				if (hitInfo2.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
				{
					HitInfo value2 = new HitInfo(base.transform, MissileComports[j].forward * 25f, 0);
					hitInfo2.transform.SendMessage("OnHit", value2, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				targetPos2 = MissileComports[j].position + MissileComports[j].forward * 65f;
			}
			LaserGunBeam[j].transform.position = MissileComports[j].position - MissileComports[j].forward * 0.8f;
			LaserGunBeam[j].UpdateBeam(targetPos2);
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(4.25f);
	}

	private void LateUpdate()
	{
		if (!Target || !Aim)
		{
			return;
		}
		if (EnemyState == State.ShootVulcan || EnemyState == State.ShootLaserBeam)
		{
			Transform parent = Muzzle.parent.parent;
			Vector3 normalized = (TargetPosition - parent.position).normalized;
			parent.eulerAngles = Quaternion.LookRotation(normalized, parent.up).eulerAngles;
		}
		if (EnemyType == Type.eWalker && EnemyState == State.LaunchBarrage)
		{
			for (int i = 0; i < MissileComports.Length; i++)
			{
				Vector3 normalized2 = (TargetPosition - MissileComports[i].position).normalized;
				MissileComports[i].forward = normalized2;
			}
		}
	}

	private void StateTransferFallStart()
	{
		EnemyState = State.TransferFall;
		Animation = 0;
		if (AppearPath != "" && (bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 2.25f, StartRotation);
		}
	}

	private void StateTransferFall()
	{
		if (Grounded)
		{
			_Rigidbody.velocity = Vector3.zero;
			Audio.PlayOneShot(eSeries[0], Audio.volume);
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
	}

	private void StateStandStart()
	{
		EnemyState = State.Stand;
		Animation = 1;
		StateTime[0] = Time.time + 2.5f;
	}

	private void StateStand()
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

	private void StateStandEnd()
	{
	}

	private void StateSearchStart()
	{
		EnemyState = State.Search;
		Animation = 10;
		StateTime[0] = Time.time + 2.65f;
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
			Animation = 2;
			TargetPosition = base.transform.position + Vector3.up - ((RobotMode == Mode.Normal && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 10;
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
		Animation = 2;
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		else
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateSeek()
	{
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (!(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir))
		{
			return;
		}
		switch (RobotMode)
		{
		case Mode.Fix:
			ToAttackState();
			break;
		case Mode.Fix_Laser:
			StateMachine.ChangeState(StateFireOscillators);
			break;
		case Mode.Fix_Launcher:
			StateMachine.ChangeState(StateLaunchBarrage);
			break;
		case Mode.Normal:
		{
			if (EnemyType == Type.eCannon)
			{
				if (GetDistance() < 18.9f)
				{
					ToAttackState();
				}
				else
				{
					StateMachine.ChangeState(StateFollowPlayer);
				}
				break;
			}
			float distance = GetDistance();
			if (distance < 6.3f)
			{
				StateMachine.ChangeState(StateLaunchBarrage);
			}
			else if (distance < 12.6f)
			{
				StateMachine.ChangeState(StateShootLaserBeam);
			}
			else if (distance < 18.9f)
			{
				StateMachine.ChangeState(StateFireOscillators);
			}
			else
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			break;
		}
		}
	}

	private void StateSeekEnd()
	{
	}

	private void ToAttackState()
	{
		if (GetDistance() < 6.3f)
		{
			StateMachine.ChangeState(StateLaunchBarrage);
		}
		else if (GetDistance() < 12.6f)
		{
			if (EnemyType == Type.eCannon)
			{
				Audio.PlayOneShot(VulcanChargeSound, Audio.volume);
				StateMachine.ChangeState(StateShootVulcan);
			}
			else
			{
				StateMachine.ChangeState(StateShootLaserBeam);
			}
		}
		else
		{
			StateMachine.ChangeState(StateFireOscillators);
		}
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
		Animation = 2;
		StateEnd = Time.time + 2f;
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * 3f);
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateRoamEnd()
	{
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		Animation = 2;
		StateEnd = Time.time + 2f;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * 3f);
		if (GetDistance() < 18.9f || Time.time > StateEnd)
		{
			if (EnemyType == Type.eCannon)
			{
				ToAttackState();
			}
			else
			{
				StateMachine.ChangeState(StateFireOscillators);
			}
		}
		if (GetDistance() > 40f || !Target)
		{
			StateEnd = 0f;
			Target = null;
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateFollowPlayerEnd()
	{
	}

	private void StateShootVulcanStart()
	{
		EnemyState = State.ShootVulcan;
		Animation = 11;
		Animator.SetTrigger("Change State");
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
				Animation = 12;
			}
		}
		else if (Time.time > StateTime[1])
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootVulcanEnd()
	{
	}

	private void StateShootLaserBeamStart()
	{
		EnemyState = State.ShootLaserBeam;
		Animation = 11;
		Animator.SetTrigger("Change State");
		StateTime[0] = Time.time + 0.55f;
		StateTime[1] = Time.time + 1.05f;
		StateVar[0] = 0;
		Aim = true;
	}

	private void StateShootLaserBeam()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.5f;
		}
		if (Time.time > StateTime[0] && StateVar[0] == 0)
		{
			LaserBeam.State = 0;
			StateVar[0] = 1;
		}
		if (StateVar[0] == 1)
		{
			RaycastHit hitInfo;
			Vector3 targetPos = ((!Physics.Raycast(Muzzle.position, Muzzle.forward, out hitInfo, 65f)) ? (Muzzle.position + Muzzle.forward * 65f) : hitInfo.point);
			LaserBeam.transform.position = Muzzle.position;
			LaserBeam.UpdateBeam(targetPos);
		}
		if (Time.time > StateTime[1] && StateVar[0] == 1)
		{
			StateTime[1] = Time.time + 1f;
			Aim = false;
			Animation = 12;
			LaserBeam.State = 2;
			StateVar[0] = 2;
		}
		if (Time.time > StateTime[1] && StateVar[0] == 2)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootLaserBeamEnd()
	{
		LaserBeam.State = 2;
	}

	private void StateFireOscillatorsStart()
	{
		EnemyState = State.FireOscillators;
		Animation = 3;
		StateTime[0] = Time.time + 0.55f;
		StateTime[1] = Time.time + 3f;
		StateEnd = Time.time + 3.9665f * (float)((EnemyType != 0 || RobotMode != Mode.Fix_Laser) ? 1 : RandomInt(1, 2));
		IsOscillatorActive = false;
	}

	private void StateFireOscillators()
	{
		if (Time.time > StateTime[0] && !IsOscillatorActive)
		{
			ActivateOscillators();
			IsOscillatorActive = true;
		}
		if (Time.time > StateTime[1] && IsOscillatorActive)
		{
			DeactivateOscillators();
			IsOscillatorActive = false;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateFireOscillatorsEnd()
	{
		DeactivateOscillators();
	}

	private void StateLaunchBarrageStart()
	{
		EnemyState = State.LaunchBarrage;
		Animation = 4;
		AttackCount = 0;
		MaxAttack = GetSpecialAmmo();
		StateTime[0] = Time.time + ((EnemyType == Type.eCannon) ? 0.5f : 0.25f);
		StateEnd = Time.time + ((EnemyType == Type.eCannon) ? 0.5f : 0.25f);
		if (EnemyType == Type.eCannon)
		{
			Audio.PlayOneShot(eSeries[1], Audio.volume);
		}
		else
		{
			Aim = true;
		}
	}

	private void StateLaunchBarrage()
	{
		if (EnemyType == Type.eWalker && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		if (Time.time > StateTime[0] + (float)AttackCount * 0.65f && AttackCount < MaxAttack)
		{
			if (EnemyType == Type.eCannon)
			{
				for (int i = 0; i < MissileComports.Length; i++)
				{
					FireMissiles(MissileComports[i]);
				}
				MissileAudio.Play();
			}
			else
			{
				ActivateLaserGuns();
			}
			AttackCount++;
		}
		if (Time.time > StateEnd + (float)AttackCount * 0.65f)
		{
			if (EnemyType == Type.eWalker)
			{
				DeactivateLaserGuns();
				Aim = false;
			}
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateLaunchBarrageEnd()
	{
		if (EnemyType == Type.eWalker)
		{
			DeactivateLaserGuns();
			Aim = false;
		}
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		if (StunSource == "L_Calf")
		{
			Animation = 6;
		}
		else
		{
			Animation = 7;
		}
		Animator.SetTrigger("Change State");
		StateEnd = Time.time + FootBrokenWait;
		Audio.PlayOneShot(eSeries[2], Audio.volume);
	}

	private void StateStuned()
	{
		if (Time.time > StateEnd)
		{
			Animator.SetTrigger("Change State");
			Audio.PlayOneShot(eSeries[3], Audio.volume);
			FootHitPoint = FootMaxHealth();
			LegHitboxes[0].enabled = true;
			LegHitboxes[1].enabled = true;
			LegTargets[0].SetActive(value: true);
			LegTargets[1].SetActive(value: true);
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateStunedEnd()
	{
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateStunedBackStart()
	{
		EnemyState = State.StunedBack;
		if (LastStunSource == "L_Calf")
		{
			Animation = 8;
		}
		else
		{
			Animation = 9;
		}
		Animator.SetTrigger("Change State");
		StateEnd = Time.time + FootBrokenWait;
		Audio.PlayOneShot(eSeries[2], Audio.volume);
		SetParalysisEffect();
	}

	private void StateStunedBack()
	{
		if (Time.time > StateEnd)
		{
			Animator.SetTrigger("Change State");
			FootHitPoint = FootMaxHealth();
			LegHitboxes[0].enabled = true;
			LegHitboxes[1].enabled = true;
			LegTargets[0].SetActive(value: true);
			LegTargets[1].SetActive(value: true);
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateStunedBackEnd()
	{
		Audio.PlayOneShot(eSeries[3], Audio.volume);
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 5;
		StateEnd = Time.time + 0.8f;
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

	private void StunEnemy(string Name)
	{
		if (HitTimer < 0.15f || (FullStunMode && Stuned) || IsPsychokinesis || PsychoThrown)
		{
			return;
		}
		LastStunSource = StunSource;
		StunSource = Name;
		if (StunSource == "L_Calf")
		{
			LegHitboxes[0].enabled = false;
			LegTargets[0].SetActive(value: false);
		}
		else
		{
			LegHitboxes[1].enabled = false;
			LegTargets[1].SetActive(value: false);
		}
		if (!FullStunMode)
		{
			FootHitPoint -= 1f;
		}
		else
		{
			FootHitPoint = 0f;
		}
		if (FootHitPoint > 0f)
		{
			return;
		}
		SetParalysisEffect();
		if (!FullStunMode)
		{
			if (EnemyState != State.Stuned && EnemyState != State.StunedBack)
			{
				StateMachine.ChangeState(StateStuned);
			}
			else
			{
				if (EnemyState == State.StunedBack)
				{
					return;
				}
				StateMachine.ChangeState(StateStunedBack);
			}
		}
		else
		{
			StateMachine.ChangeState(StateStunedBack);
		}
		HitTimer = 0f;
		Stuned = true;
		FullStunMode = false;
	}

	private void FullStun()
	{
		FullStunMode = true;
		StunEnemy("");
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
		StunEnemy("");
	}

	private void SetParalysisEffect()
	{
		if ((bool)ParalysisEffect)
		{
			Object.Destroy(ParalysisEffect);
		}
		ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, ParalysisPoint.position, Quaternion.identity);
		ParalysisEffect.transform.SetParent(ParalysisPoint);
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (Destroyed)
		{
			return;
		}
		CreateHitFX(HitInfo, IgnoreTimer: true);
		OnImpact(HitInfo, Explosion: true);
		if (OscillatorBeam != null)
		{
			for (int i = 0; i < Oscillators.Length; i++)
			{
				Object.Destroy(OscillatorBeam[i].gameObject);
			}
		}
		if (LaserGunBeam != null)
		{
			for (int j = 0; j < MissileComports.Length; j++)
			{
				Object.Destroy(LaserGunBeam[j].gameObject);
			}
		}
		Object.Destroy(base.gameObject);
		Destroyed = true;
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (HitTimer <= GetHitTimer(HitInfo))
		{
			return;
		}
		if (CurHealth > 0)
		{
			CurHealth -= HitInfo.damage * ((EnemyState != State.Stuned && EnemyState != State.StunedBack) ? 1 : 2);
			if (EnemyState != State.Stuned && EnemyState != State.FireOscillators && EnemyState != State.StunedBack && !IsPsychokinesis && !PsychoThrown)
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
		if (Destroyed)
		{
			return;
		}
		CreateHitFX(HitInfo, IgnoreTimer: true);
		OnDestroy();
		OnImpact(HitInfo);
		if (OscillatorBeam != null)
		{
			for (int i = 0; i < Oscillators.Length; i++)
			{
				Object.Destroy(OscillatorBeam[i].gameObject);
			}
		}
		if (LaserGunBeam != null)
		{
			for (int j = 0; j < MissileComports.Length; j++)
			{
				Object.Destroy(LaserGunBeam[j].gameObject);
			}
		}
		Object.Destroy(base.gameObject);
		Destroyed = true;
	}

	private void ActivateOscillators()
	{
		for (int i = 0; i < OscillatorBeam.Length; i++)
		{
			OscillatorBeam[i].State = 0;
		}
	}

	private void DeactivateOscillators()
	{
		for (int i = 0; i < OscillatorBeam.Length; i++)
		{
			OscillatorBeam[i].State = 2;
		}
	}

	private void ActivateLaserGuns()
	{
		for (int i = 0; i < LaserGunBeam.Length; i++)
		{
			LaserGunBeam[i].State = 0;
		}
	}

	private void DeactivateLaserGuns()
	{
		for (int i = 0; i < LaserGunBeam.Length; i++)
		{
			LaserGunBeam[i].State = 2;
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
