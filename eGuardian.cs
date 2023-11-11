using UnityEngine;

public class eGuardian : EnemyBase
{
	public enum Type
	{
		eGuardian = 0,
		eKeeper = 1
	}

	public enum Mode
	{
		Ball_Fix = 0,
		Ball_Normal = 1,
		Fix = 2,
		Freeze = 3,
		Normal = 4
	}

	internal enum State
	{
		TransferFall = 0,
		Freeze = 1,
		Stand = 2,
		Search = 3,
		Seek = 4,
		Roam = 5,
		FollowPlayer = 6,
		LaunchArm = 7,
		Launch2Arms = 8,
		ScatterAttack = 9,
		Stuned = 10
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public Transform[] Muzzle;

	public Transform[] ForearmPoint;

	public Transform ParalysisPoint;

	public BoxCollider MainCollider;

	public GameObject Spine;

	public GameObject L_upperArm;

	public GameObject R_upperArm;

	public GameObject Beam;

	[Header("Instantiation")]
	public GameObject ShieldEffectPrefab;

	public GameObject ScatterPrefab;

	public GameObject L_ArmPrefab;

	public GameObject R_ArmPrefab;

	public GameObject BallPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip[] eSeries;

	private State EnemyState;

	private LaserBeam[] Beams;

	private GameObject[] Arm;

	private Vector3 DirectionPosition;

	private Vector3 ShootDirection;

	private bool IsStuned;

	private bool Launched;

	private float SpineRotation;

	private float L_upperArmRotation;

	private float R_upperArmRotation;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int ScatterRotation;

	private int AttackCount;

	private int ArmToLaunch;

	private bool ShootLaser;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		DescentOffset *= 1000f;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = (IsFixed ? StartPosition : ((EnemyType != Type.eKeeper || (EnemyType == Type.eKeeper && RobotMode != Mode.Fix)) ? (StartPosition + Vector3.up * (DescentOffset * ((RobotMode != Mode.Freeze) ? 1f : 0f))) : StartPosition));
		if (EnemyType == Type.eKeeper)
		{
			Beams = new LaserBeam[2];
			for (int i = 0; i < Muzzle.Length; i++)
			{
				Beams[i] = Object.Instantiate(Beam, Muzzle[i].position, Muzzle[i].rotation).GetComponent<LaserBeam>();
			}
		}
		BaseStart();
		Transfer();
		HoldArm();
	}

	private void Transfer()
	{
		if (Position == Vector3.zero)
		{
			return;
		}
		_Rigidbody.velocity = Physics.gravity * 0.5f;
		Grounded = false;
		base.transform.position = Position;
		base.transform.rotation = StartRotation;
		DirectionPosition = (StartPosition - base.transform.position).normalized;
		CreateArms();
		if (RobotMode != Mode.Freeze)
		{
			if (!IsFixed && (EnemyType != Type.eKeeper || (EnemyType == Type.eKeeper && RobotMode != Mode.Fix)))
			{
				StateTransferFallStart();
				StateMachine.Initialize(StateTransferFall);
			}
			else
			{
				StateStandStart();
				StateMachine.Initialize(StateStand);
			}
		}
		else
		{
			StateFreezeStart();
			StateMachine.Initialize(StateFreeze);
		}
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, ParalysisPoint.position, base.transform.rotation);
		}
	}

	private void Reset()
	{
		Target = null;
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, ParalysisPoint.position, base.transform.rotation);
		}
		if ((bool)ParalysisEffect)
		{
			Stuned = false;
			Object.Destroy(ParalysisEffect);
		}
		CurHealth = MaxHealth;
		if (EnemyType == Type.eKeeper)
		{
			DeactivateBeam();
		}
		base.transform.position = StartPosition + Vector3.up * (DescentOffset * ((RobotMode != Mode.Freeze) ? 1f : 0f));
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
	}

	private float LaunchForce()
	{
		if (RobotMode == Mode.Ball_Fix || RobotMode == Mode.Ball_Normal)
		{
			return 1800f;
		}
		return 2500f;
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eKeeper)
		{
			return 5;
		}
		return 3;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(5.5f);
		for (int i = 0; i < ForearmPoint.Length; i++)
		{
			if ((bool)Arm[i])
			{
				OnPsiFX(Arm[i].GetComponentInChildren<Renderer>(), IsPsychokinesis);
			}
		}
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Animator.GetInteger("Animation") != Animation)
		{
			Animator.SetTrigger("StateChange");
		}
		Grounded = false;
		if (EnemyType != Type.eKeeper || EnemyState != State.ScatterAttack)
		{
			return;
		}
		for (int i = 0; i < Beams.Length; i++)
		{
			Vector3 targetPos;
			if (Physics.Raycast(Muzzle[i].transform.position, Muzzle[i].transform.forward, out var hitInfo, 65f))
			{
				targetPos = hitInfo.point;
				if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
				{
					HitInfo value = new HitInfo(base.transform, Muzzle[i].transform.forward * 25f, 0);
					hitInfo.transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				targetPos = Muzzle[i].transform.position + Muzzle[i].transform.forward * 65f;
			}
			Beams[i].transform.position = Muzzle[i].position - Muzzle[i].forward * 0.8f;
			Beams[i].UpdateBeam(targetPos);
		}
	}

	private void LateUpdate()
	{
		HoldArm();
		Spine.transform.Rotate(0f, SpineRotation, 0f);
		L_upperArm.transform.Rotate(L_upperArmRotation, 0f, 0f);
		R_upperArm.transform.Rotate(R_upperArmRotation, 0f, 0f);
	}

	private void StateTransferFallStart()
	{
		EnemyState = State.TransferFall;
		Animation = 0;
	}

	private void StateTransferFall()
	{
		RaycastHit hitInfo;
		if (Grounded)
		{
			_Rigidbody.velocity = Vector3.zero;
			Audio.PlayOneShot(eSeries[0], Audio.volume);
			StateMachine.ChangeState(StateStand);
		}
		else if (Physics.Raycast(base.transform.position, DirectionPosition.normalized, out hitInfo))
		{
			Debug.DrawLine(base.transform.position, hitInfo.point, Color.green);
			if (base.transform.position.y < hitInfo.point.y + 0.5f)
			{
				_Rigidbody.velocity = Physics.gravity * 0.5f;
			}
		}
	}

	private void StateTransferFallEnd()
	{
	}

	private void StateFreezeStart()
	{
		EnemyState = State.Freeze;
		Animation = 17;
		StateVar[0] = 0;
		MainCollider.center = new Vector3(0f, 1.35f, 0f);
		MainCollider.size = new Vector3(3f, 2.7f, 3f);
	}

	private void StateFreeze()
	{
		if (StateVar[0] == 0)
		{
			if (GetTargetBox(base.transform.position + base.transform.up * 1f + base.transform.forward * 2.5f, new Vector3(2.5f, 2f, 7f)))
			{
				Audio.PlayOneShot(eSeries[4], Audio.volume);
				Animation = 18;
				StateTime[0] = Time.time + 2f;
				StateVar[0] = 1;
			}
		}
		else if (Time.time > StateTime[0])
		{
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			StateMachine.ChangeState(StateStand);
			MainCollider.center = new Vector3(0f, 2.1f, 0f);
			MainCollider.size = new Vector3(3f, 4.2f, 3f);
		}
	}

	private void StateFreezeEnd()
	{
		MainCollider.center = new Vector3(0f, 2.1f, 0f);
		MainCollider.size = new Vector3(3f, 4.2f, 3f);
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
				Audio.PlayOneShot(eSeries[1], Audio.volume);
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
		Animation = 3;
		StateTime[0] = Time.time + 1.9665f;
		StateVar[0] = 0;
	}

	private void StateSearch()
	{
		if (GetTarget(Mesh.forward))
		{
			Audio.PlayOneShot(eSeries[1], Audio.volume);
			StateMachine.ChangeState(StateSeek);
		}
		if (!(Time.time > StateTime[0]))
		{
			return;
		}
		if (StateVar[0] == 0)
		{
			Animation = 2;
			TargetPosition = base.transform.position + Vector3.up - (((RobotMode == Mode.Normal || RobotMode == Mode.Ball_Normal) && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 3;
			StateTime[0] = Time.time + 5f;
			StateVar[0] = 0;
			if ((RobotMode == Mode.Normal || RobotMode == Mode.Ball_Normal) && Random.value > 0.5f)
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
		else if (RobotMode != Mode.Freeze)
		{
			StateMachine.ChangeState(StateStand);
		}
		else
		{
			StateMachine.ChangeState(StateFreeze);
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
		case Mode.Ball_Fix:
			OnAction();
			break;
		case Mode.Ball_Normal:
			if (GetDistance() < 12.6f)
			{
				OnAction();
			}
			else
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			break;
		case Mode.Fix:
			OnAction();
			break;
		case Mode.Freeze:
			OnAction();
			break;
		case Mode.Normal:
			if (GetDistance() < 12.6f)
			{
				OnAction();
			}
			else
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
			break;
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateRoamStart()
	{
		EnemyState = State.Roam;
		Animation = 2;
		StateEnd = Time.time + 4.5f;
	}

	private void StateRoam()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 1f);
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * 1f);
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateRoamEnd()
	{
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		Animation = ((EnemyType == Type.eGuardian) ? 2 : 19);
		StateTime[0] = Time.time;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * ((EnemyType == Type.eGuardian) ? 1.5f : 3f));
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * ((EnemyType == Type.eGuardian) ? 1.5f : 3f));
		}
		if (GetDistance() < 12.6f || Time.time - StateTime[0] > 4.5f)
		{
			OnAction();
		}
		if (GetDistance() > 40f || !Target)
		{
			Target = null;
			if (RobotMode != Mode.Freeze)
			{
				StateMachine.ChangeState(StateStand);
			}
			else
			{
				StateMachine.ChangeState(StateFreeze);
			}
		}
	}

	private void StateFollowPlayerEnd()
	{
	}

	private void StateLaunchArmStart()
	{
		EnemyState = State.LaunchArm;
		StateTime[0] = Time.time + 1.25f;
		StateTime[1] = StateTime[0] + 1f;
		StateEnd = StateTime[1] + 1.5f;
		Animation = 11;
		AttackCount = 0;
		Launched = false;
	}

	private void StateLaunchArm()
	{
		if ((bool)Target)
		{
			if (Time.time <= StateTime[1])
			{
				Mesh.localRotation = FaceTarget(Target.transform.position + Target.transform.up * 0.25f, Mesh.localRotation, Inverse: false, 1f);
			}
			if (Time.time > StateTime[0] && AttackCount == 0)
			{
				Animation = 13;
				if ((bool)Arm[ArmToLaunch])
				{
					Vector3 normalized = (Target.transform.position + Target.transform.up * 0.25f - Arm[ArmToLaunch].transform.position).normalized;
					Audio.PlayOneShot(eSeries[2], Audio.volume * 2f);
					ActivateArm(Arm[ArmToLaunch]);
					Arm[ArmToLaunch].transform.SetParent(null);
					Arm[ArmToLaunch].GetComponent<Rigidbody>().isKinematic = false;
					Arm[ArmToLaunch].GetComponent<Forearm>().Player = Target.transform;
					Arm[ArmToLaunch].GetComponent<Forearm>().Owner = base.transform;
					if (EnemyType != Type.eKeeper || (EnemyType == Type.eKeeper && (RobotMode == Mode.Ball_Fix || RobotMode == Mode.Ball_Normal)))
					{
						Arm[ArmToLaunch].transform.GetComponent<Rigidbody>().AddForce(normalized * LaunchForce(), ForceMode.Impulse);
					}
					Arm[ArmToLaunch] = null;
					AttackCount = 1;
				}
			}
		}
		if ((Time.time > StateTime[1] || !Target) && !Launched)
		{
			Launched = true;
			Animation = 14;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateLaunchArmEnd()
	{
	}

	private void StateLaunch2ArmsStart()
	{
		EnemyState = State.Launch2Arms;
		StateTime[0] = Time.time + 1.25f;
		StateTime[1] = StateTime[0] + 1f;
		StateEnd = StateTime[1] + 1.5f;
		Animation = 11;
		ShootDirection = (Target.transform.position + Target.transform.up * 0.25f - Spine.transform.position).normalized;
		AttackCount = 0;
		Launched = false;
	}

	private void StateLaunch2Arms()
	{
		if ((bool)Target)
		{
			if (Time.time <= StateTime[1])
			{
				Mesh.localRotation = FaceTarget(Target.transform.position + Target.transform.up * 0.25f, Mesh.localRotation, Inverse: false, 1f);
			}
			if (Time.time > StateTime[0] && AttackCount == 0)
			{
				Animation = 12;
				for (int i = 0; i < ForearmPoint.Length; i++)
				{
					if ((bool)Arm[i])
					{
						Audio.PlayOneShot(eSeries[2], Audio.volume * 2f);
						ActivateArm(Arm[i]);
						Arm[i].transform.SetParent(null);
						Arm[i].GetComponent<Rigidbody>().isKinematic = false;
						Arm[i].GetComponent<Forearm>().Player = Target.transform;
						Arm[i].GetComponent<Forearm>().Owner = base.transform;
						if (EnemyType != Type.eKeeper || (EnemyType == Type.eKeeper && (RobotMode == Mode.Ball_Fix || RobotMode == Mode.Ball_Normal)))
						{
							Arm[i].transform.GetComponent<Rigidbody>().AddForce(ShootDirection * LaunchForce(), ForceMode.Impulse);
						}
						Arm[i] = null;
					}
				}
				AttackCount = 1;
			}
		}
		if ((Time.time > StateTime[1] || !Target) && !Launched)
		{
			Launched = true;
			Animation = 14;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateLaunch2ArmsEnd()
	{
	}

	private void StateScatterAttackStart()
	{
		EnemyState = State.ScatterAttack;
		StateTime[0] = Time.time + 1f;
		StateTime[1] = Time.time + 3f;
		StateEnd = Time.time + 9f;
		Animation = 6;
		AttackCount = 0;
		ShootLaser = false;
		ScatterRotation = 1;
	}

	private void StateScatterAttack()
	{
		if (Time.time >= StateTime[0] && Time.time < StateTime[1])
		{
			Animation = 7;
			SpineRotation += (float)ScatterRotation * (360f / (StateTime[1] - StateTime[0])) * Time.deltaTime;
			Vector3 vector = (Target ? (Target.transform.position + Target.transform.up * 0.25f) : (L_upperArm.transform.position + L_upperArm.transform.forward * 1f)) - L_upperArm.transform.position;
			Vector3 vector2 = (Target ? (Target.transform.position + Target.transform.up * 0.25f) : (L_upperArm.transform.position + L_upperArm.transform.forward * 1f)) - R_upperArm.transform.position;
			float num = Mathf.Asin(vector.y / vector.magnitude) * 57.29578f;
			float num2 = Mathf.Asin(vector2.y / vector2.magnitude) * 57.29578f;
			L_upperArmRotation = 0f - num;
			R_upperArmRotation = 0f - num2;
			if (EnemyType == Type.eKeeper)
			{
				if (!ShootLaser)
				{
					ActivateBeam();
					ShootLaser = true;
				}
			}
			else
			{
				ShootVulcan(0.0625f);
			}
		}
		if (Time.time >= StateTime[1])
		{
			Animation = 8;
			if (EnemyType == Type.eKeeper && ShootLaser)
			{
				DeactivateBeam();
				ShootLaser = false;
			}
		}
		if (Time.time > StateEnd)
		{
			SpineRotation = 0f;
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateScatterAttackEnd()
	{
		if (EnemyType == Type.eKeeper)
		{
			DeactivateBeam();
		}
		CreateArms();
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		Animation = 4;
		StateTime[0] = Time.time;
		StateEnd = Time.time + 10f;
		StateVar[0] = 0;
		Audio.PlayOneShot(eSeries[3], Audio.volume);
		SetParalysisEffect();
		IsStuned = false;
	}

	private void StateStuned()
	{
		if (Time.time - StateTime[0] > 1.5f && !IsStuned)
		{
			IsStuned = true;
		}
		SpineRotation = Mathf.LerpAngle(SpineRotation, 0f, 0.05f);
		L_upperArmRotation = Mathf.LerpAngle(L_upperArmRotation, 0f, 0.05f);
		R_upperArmRotation = Mathf.LerpAngle(R_upperArmRotation, 0f, 0.05f);
		if (Time.time > StateEnd - 2f && StateVar[0] == 0)
		{
			CreateArms();
			if ((bool)ParalysisEffect)
			{
				Object.Destroy(ParalysisEffect);
			}
			Audio.PlayOneShot(eSeries[4], Audio.volume * 2f);
			StateVar[0] = 1;
		}
		if (Time.time > StateEnd)
		{
			IsStuned = false;
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateStunedEnd()
	{
		IsStuned = false;
		Stuned = false;
	}

	private void OnAction()
	{
		if (RobotMode != Mode.Freeze)
		{
			string possessionName = GetPossessionName();
			if (possessionName == "R_Forearm" || possessionName == "L_Forearm")
			{
				if (Random.value <= ((RobotMode == Mode.Fix || RobotMode == Mode.Ball_Fix || (EnemyType == Type.eKeeper && RobotMode == Mode.Normal)) ? 5f : 8f) / 10f)
				{
					ArmToLaunch = ((possessionName == "R_Forearm") ? 1 : 0);
					StateMachine.ChangeState(StateLaunchArm);
				}
				else
				{
					StateMachine.ChangeState(StateLaunch2Arms);
				}
			}
			else
			{
				StateMachine.ChangeState(StateScatterAttack);
			}
		}
		else
		{
			string possessionName2 = GetPossessionName();
			if (possessionName2 == "L_Forearm" || possessionName2 == "R_Forearm")
			{
				ArmToLaunch = ((possessionName2 == "R_Forearm") ? 1 : 0);
				StateMachine.ChangeState(StateLaunchArm);
			}
			else
			{
				StateMachine.ChangeState(StateScatterAttack);
			}
		}
	}

	private void ShootVulcan(float Delay)
	{
		Vector3[] array = new Vector3[6];
		if (Time.time >= StateTime[0] + (float)AttackCount * Delay)
		{
			for (int i = 0; i < Muzzle.Length; i++)
			{
				array[0] = Muzzle[i].position + Vector3.up * 0.5f;
				array[1] = Muzzle[i].position + Vector3.up * 0.25f + Vector3.right * 0.433f;
				array[2] = Muzzle[i].position - Vector3.up * 0.25f + Vector3.right * 0.433f;
				array[3] = Muzzle[i].position - Vector3.up * 0.5f;
				array[4] = Muzzle[i].position - Vector3.up * 0.25f - Vector3.right * 0.433f;
				array[5] = Muzzle[i].position + Vector3.up * 0.25f - Vector3.right * 0.433f;
				Object.Instantiate(ScatterPrefab, array[(AttackCount + 6) % 6], Muzzle[i].rotation, base.transform);
				Object.Instantiate(ScatterPrefab, array[(AttackCount + 9) % 6], Muzzle[i].rotation, base.transform);
			}
			AttackCount++;
		}
	}

	private void OnFlash()
	{
		if (!Stuned && EnemyState != State.Stuned && !IsPsychokinesis && !PsychoThrown)
		{
			StateMachine.ChangeState(StateStuned);
			Stuned = true;
		}
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
		DestroyArm();
		if (EnemyType == Type.eKeeper)
		{
			for (int i = 0; i < Muzzle.Length; i++)
			{
				Object.Destroy(Beams[i].gameObject);
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
			CurHealth -= HitInfo.damage * ((!IsStuned) ? 1 : 2);
			if (EnemyState != State.Stuned && !IsPsychokinesis && !PsychoThrown && (EnemyState == State.ScatterAttack || (EnemyState == State.LaunchArm && Launched) || (EnemyState == State.Launch2Arms && Launched)))
			{
				StateMachine.ChangeState(StateStuned);
				Stuned = true;
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
		DestroyArm();
		if (EnemyType == Type.eKeeper)
		{
			for (int i = 0; i < Muzzle.Length; i++)
			{
				Object.Destroy(Beams[i].gameObject);
			}
		}
		Object.Destroy(base.gameObject);
		Destroyed = true;
	}

	private void CreateArms()
	{
		if (Arm == null)
		{
			Arm = new GameObject[2];
		}
		bool flag = RobotMode == Mode.Ball_Fix || RobotMode == Mode.Ball_Normal;
		if (!Arm[0])
		{
			Object.Instantiate(TeleportEffectPrefab, ForearmPoint[0].position, ForearmPoint[0].rotation);
			Arm[0] = Object.Instantiate((!flag) ? L_ArmPrefab : BallPrefab, ForearmPoint[0].position, ForearmPoint[0].rotation, base.transform);
		}
		if (!Arm[1])
		{
			Object.Instantiate(TeleportEffectPrefab, ForearmPoint[1].position, ForearmPoint[0].rotation);
			Arm[1] = Object.Instantiate((!flag) ? R_ArmPrefab : BallPrefab, ForearmPoint[1].position, ForearmPoint[0].rotation, base.transform);
		}
	}

	private void HoldArm()
	{
		for (int i = 0; i < ForearmPoint.Length; i++)
		{
			if ((bool)Arm[i])
			{
				Arm[i].GetComponent<Rigidbody>().position = ForearmPoint[i].position;
				Arm[i].GetComponent<Rigidbody>().rotation = ForearmPoint[i].rotation;
				Arm[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}

	private void DropArm()
	{
		for (int i = 0; i < ForearmPoint.Length; i++)
		{
			if ((bool)Arm[i])
			{
				Arm[i].transform.SetParent(null);
				Arm[i].GetComponent<Rigidbody>().isKinematic = false;
				ActivateArm(Arm[i]);
				Arm[i] = null;
			}
		}
	}

	private void ActivateArm(GameObject Arm)
	{
		Arm.GetComponent<Forearm>().enabled = true;
		Arm.GetComponent<Forearm>().Launched = true;
	}

	private void DestroyArm()
	{
		for (int i = 0; i < ForearmPoint.Length; i++)
		{
			if ((bool)Arm[i])
			{
				Arm[i].transform.SetParent(null);
				Arm[i].GetComponent<Rigidbody>().isKinematic = false;
				Arm[i].GetComponent<Forearm>().AutoDestroy();
				Arm[i] = null;
			}
		}
	}

	private void ActivateBeam()
	{
		for (int i = 0; i < Beams.Length; i++)
		{
			Beams[i].State = 0;
		}
	}

	private void DeactivateBeam()
	{
		for (int i = 0; i < Beams.Length; i++)
		{
			Beams[i].State = 2;
		}
	}

	private string GetPossessionName()
	{
		if ((bool)Arm[0])
		{
			return "L_Forearm";
		}
		if ((bool)Arm[1])
		{
			return "R_Forearm";
		}
		return "null";
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
