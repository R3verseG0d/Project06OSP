using UnityEngine;

public class eLiner : EnemyBase
{
	public enum Type
	{
		eLiner = 0,
		eChaser = 1
	}

	public enum Mode
	{
		Alarm = 0,
		Chase = 1,
		Normal = 2,
		Master = 3,
		Slave = 4
	}

	public enum State
	{
		Stand = 0,
		Search = 1,
		Seek = 2,
		Roam = 3,
		Command = 4,
		ShootBombs = 5,
		Move = 6,
		MoveParallel = 7,
		ChargeAttack = 8,
		Found = 9,
		Stuned = 10,
		CrashStuned = 11,
		DamageKnockBack = 12
	}

	[Header("Framework")]
	public Mode RobotMode;

	public string GroupID;

	[Header("Prefab")]
	public Type EnemyType;

	public LayerMask FrontalColLayer;

	public Transform[] BombCannons;

	[Header("Instantiation")]
	public GameObject BombPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip[] eSeries;

	public AudioSource[] MoveSources;

	public AudioSource[] RushSources;

	private State EnemyState;

	private bool TriggerStateSwitch;

	private bool GroupDestroy;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private float StateSound;

	private int StateVar;

	private int ChaserAction;

	private int SlaveAction;

	private int FoundState;

	private bool LaunchedBombs;

	private Vector3 DistFromPlayer;

	private bool MoveSwitch;

	private PlayerBase Player;

	private float Speed;

	private float TimeBeforeCharge;

	private float TimeAfterCharge;

	private bool Charging;

	private bool PlaySound;

	private bool PlayRushSound;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition + Vector3.up * DescentOffset;
		BaseStart();
		Transfer();
	}

	public void SetParameters(string _GroupID)
	{
		GroupID = _GroupID;
	}

	private void Transfer()
	{
		if (!(Position == Vector3.zero))
		{
			FoundState = 0;
			_Rigidbody.velocity = Physics.gravity * 0.5f;
			Grounded = false;
			StateStandStart();
			StateMachine.Initialize(StateStand);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1f, StartRotation);
			}
		}
	}

	private void Reset()
	{
		Target = null;
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 1f, StartRotation);
		}
		if ((bool)ParalysisEffect)
		{
			Animator.speed = 1f;
			Stuned = false;
			Object.Destroy(ParalysisEffect);
		}
		CurHealth = MaxHealth;
		base.transform.position = StartPosition;
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eChaser)
		{
			return 1;
		}
		return 0;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(2f);
	}

	private void StateStandStart()
	{
		EnemyState = State.Stand;
		Animation = 1;
		StateEnd = Time.time + 2.5f;
	}

	private void StateStand()
	{
		if (GetTarget(Mesh.forward))
		{
			if (FoundState == 0)
			{
				FoundState = 1;
				StateMachine.ChangeState(StateFound);
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
			}
		}
		else if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateStandEnd()
	{
	}

	private void StateSearchStart()
	{
		EnemyState = State.Search;
		Animation = 1;
		StateEnd = Time.time + 1.9665f;
		StateVar = 0;
	}

	private void StateSearch()
	{
		if (GetTarget(Mesh.forward))
		{
			if (FoundState == 0 && RobotMode != Mode.Chase)
			{
				FoundState = 1;
				StateMachine.ChangeState(StateFound);
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
			}
		}
		if (!(Time.time > StateEnd))
		{
			return;
		}
		if (StateVar == 0)
		{
			Animation = 1;
			TargetPosition = base.transform.position + Vector3.up - ((RobotMode == Mode.Normal && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar = 1;
		}
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 1;
			StateEnd = Time.time + 1.9665f;
			StateVar = 0;
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
		Animation = 1;
		StateEnd = Time.time;
	}

	private void StateSeek()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f);
		if (EnemyType == Type.eChaser)
		{
			if (!(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir))
			{
				return;
			}
			switch (RobotMode)
			{
			case Mode.Normal:
				if (ChaserAction == 0)
				{
					if (Time.time - StateEnd > 2.5f)
					{
						StateMachine.ChangeState(StateMove);
					}
				}
				else if (ChaserAction == 1)
				{
					if (Time.time - StateEnd > 2.5f)
					{
						StateMachine.ChangeState(StateShootBombs);
					}
				}
				else if (Time.time - StateEnd > 2.5f)
				{
					StateMachine.ChangeState(StateChargeAttack);
				}
				break;
			case Mode.Master:
				if (ChaserAction == 0)
				{
					if (Time.time - StateEnd > 2.5f)
					{
						StateMachine.ChangeState(StateCommand);
					}
				}
				else if (Time.time - StateEnd > 2.5f)
				{
					StateMachine.ChangeState(StateShootBombs);
				}
				break;
			}
			return;
		}
		switch (RobotMode)
		{
		case Mode.Chase:
			StateMachine.ChangeState(StateMoveParallel);
			break;
		case Mode.Normal:
			if (SlaveAction == 0)
			{
				if (Time.time - StateEnd > 2.5f)
				{
					StateMachine.ChangeState(StateMove);
				}
			}
			else if (SlaveAction == 1)
			{
				if (Time.time - StateEnd > 2.5f)
				{
					StateMachine.ChangeState(StateShootBombs);
				}
			}
			else if (Time.time - StateEnd > 2.5f)
			{
				StateMachine.ChangeState(StateChargeAttack);
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
		Animation = 1;
		StateEnd = Time.time + 1.5f;
		MoveSources[1].volume = 0f;
		MoveSources[0].Play();
		MoveSources[1].Play();
	}

	private void StateRoam()
	{
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * 2.5f);
		if (!MoveSources[0].isPlaying)
		{
			MoveSources[1].volume = 1f;
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateRoamEnd()
	{
	}

	private void StateCommandStart()
	{
		EnemyState = State.Command;
		Animation = 3;
		StateEnd = Time.time + 0.95f;
		ChaserAction = 1;
	}

	private void StateCommand()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
			if ((bool)Target)
			{
				ManageSlaves("Command");
			}
		}
	}

	private void StateCommandEnd()
	{
	}

	private void StateShootBombsStart()
	{
		EnemyState = State.ShootBombs;
		Animation = 4;
		StateTime[0] = Time.time + 0.25f;
		StateEnd = Time.time + 0.5f;
		LaunchedBombs = false;
		SlaveAction = 2;
		ChaserAction = ((RobotMode == Mode.Normal) ? 2 : 0);
		Audio.PlayOneShot(eSeries[1], Audio.volume * 2f);
	}

	private void StateShootBombs()
	{
		if (Time.time > StateTime[0] && !LaunchedBombs)
		{
			for (int i = 0; i < BombCannons.Length; i++)
			{
				DeployBombs(BombCannons[i]);
			}
			LaunchedBombs = true;
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootBombsEnd()
	{
	}

	private void StateMoveStart()
	{
		EnemyState = State.Move;
		Animation = 1;
		StateEnd = Time.time + 1.25f;
		MoveSwitch = Random.value < 0.75f;
		MoveSources[1].volume = 0f;
		MoveSources[0].Play();
		MoveSources[1].Play();
		SlaveAction = 1;
		ChaserAction = 1;
	}

	private void StateMove()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		DistFromPlayer = (((bool)Target && GetDistance() < 6f) ? ((TargetPosition - base.transform.position).normalized * -6f) : Vector3.zero);
		Vector3 vector = base.transform.InverseTransformDirection((TargetPosition - base.transform.position).normalized);
		vector.y = 0f;
		ToTargetDir = vector.normalized;
		Mesh.localRotation = Quaternion.RotateTowards(Mesh.localRotation, Quaternion.LookRotation(ToTargetDir) * Quaternion.Euler(0f, (!(GetDistance() < 6f)) ? 0f : (MoveSwitch ? 90f : (-90f)), 0f), 3f);
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * 6f + DistFromPlayer);
		if (!MoveSources[0].isPlaying)
		{
			MoveSources[1].volume = 1f;
		}
		if (Time.time > StateEnd || !Target)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveEnd()
	{
		MoveSources[0].Stop();
		MoveSources[1].Stop();
	}

	private void StateMoveParallelStart()
	{
		EnemyState = State.MoveParallel;
		Animation = 1;
		MoveSources[1].volume = 0f;
		MoveSources[0].Play();
		MoveSources[1].Play();
		Player = Target.GetComponent<PlayerBase>();
		StateVar = 0;
		StateTime[0] = Time.time + 5f;
		LaunchedBombs = false;
	}

	private void StateMoveParallel()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Vector3 forward = Target.transform.forward;
		forward.y = 0f;
		ToTargetDir = forward.normalized;
		Mesh.rotation = Quaternion.Slerp(Mesh.rotation, Quaternion.LookRotation(ToTargetDir), 3f * Time.fixedDeltaTime);
		float num = Vector3.Dot(Mesh.forward, (base.transform.position - Target.transform.position).normalized);
		num = ((num > 0f) ? 1f : (-1f));
		Speed = Mathf.Lerp(Speed, Player.CurSpeed * ((num > 0f && !Player.IsGrounded()) ? 0.5f : 1.25f), Time.fixedDeltaTime * ((Player.TargetDirection != Vector3.zero) ? 4f : 1f));
		_Rigidbody.velocity = SetMoveVel(Mesh.forward * Speed);
		if (!MoveSources[0].isPlaying)
		{
			MoveSources[1].volume = 1f;
		}
		if (StateVar == 0)
		{
			if (Time.time > StateTime[0])
			{
				Animation = 4;
				StateTime[0] = Time.time + 0.25f;
				StateEnd = Time.time + 0.5f;
				Audio.PlayOneShot(eSeries[1], Audio.volume * 2f);
				StateVar = 1;
			}
		}
		else
		{
			if (Time.time > StateTime[0] && !LaunchedBombs)
			{
				for (int i = 0; i < BombCannons.Length; i++)
				{
					DeployBombs(BombCannons[i]);
				}
				LaunchedBombs = true;
			}
			if (Time.time > StateEnd)
			{
				Animation = 1;
				StateTime[0] = Time.time + 5f;
				LaunchedBombs = false;
				StateVar = 0;
			}
		}
		if (!Target)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveParallelEnd()
	{
		MoveSources[0].Stop();
		MoveSources[1].Stop();
	}

	private void StateChargeAttackStart()
	{
		EnemyState = State.ChargeAttack;
		Animation = 5;
		Animator.SetTrigger("On Attack");
		StateEnd = Time.time + 4f;
		StateSound = Time.time;
		TimeAfterCharge = 0f;
		TimeBeforeCharge = Time.time + 2f;
		PlaySound = false;
		PlayRushSound = false;
		SlaveAction = 0;
		ChaserAction = 0;
	}

	private void StateChargeAttack()
	{
		if (Time.time > TimeBeforeCharge)
		{
			TimeAfterCharge += Time.fixedDeltaTime;
			Charging = TimeAfterCharge < 1.1f;
			if (TimeAfterCharge < 1.1f)
			{
				_Rigidbody.velocity = SetMoveVel(Mesh.forward * 12f);
				if (Physics.Raycast(base.transform.position + base.transform.up * 1.25f, Mesh.forward, out var hitInfo, 0.75f, FrontalColLayer) && hitInfo.transform.GetComponent<Collider>() != GetComponent<Collider>())
				{
					StateMachine.ChangeState(StateCrashStuned);
				}
			}
			else
			{
				if ((bool)Target)
				{
					TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				}
				Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			}
		}
		else
		{
			if ((bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			}
			Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			Charging = false;
		}
		if (Time.time - StateSound > 0.5f && !PlaySound)
		{
			PlaySound = true;
			Audio.PlayOneShot(eSeries[2], Audio.volume);
		}
		if (Time.time - StateSound > 2f)
		{
			if (!PlayRushSound)
			{
				PlayRushSound = true;
				RushSources[1].volume = 0f;
				RushSources[0].Play();
				RushSources[1].Play();
			}
			else if (!RushSources[0].isPlaying)
			{
				RushSources[1].volume = 1f;
			}
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			TimeBeforeCharge = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateChargeAttackEnd()
	{
		RushSources[0].Stop();
		RushSources[1].Stop();
	}

	private void StateFoundStart()
	{
		EnemyState = State.Found;
		Animation = 6;
		StateEnd = Time.time + 1f;
		_Rigidbody.velocity = Vector3.zero;
		Audio.PlayOneShot(eSeries[0], Audio.volume);
	}

	private void StateFound()
	{
		if (!(Time.time > StateEnd))
		{
			return;
		}
		StateEnd = 0f;
		if (EnemyType != Type.eChaser && TriggerStateSwitch)
		{
			if (RobotMode != Mode.Normal)
			{
				if (SlaveAction == 0)
				{
					StateMachine.ChangeState(StateMove);
				}
				else
				{
					StateMachine.ChangeState(StateChargeAttack);
				}
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
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
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateStunedEnd()
	{
		Animator.speed = 1f;
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateCrashStunedStart()
	{
		EnemyState = State.CrashStuned;
		StateEnd = Time.time + 7.5f;
		Animation = 7;
		if (!ParalysisEffect)
		{
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 1.25f, Quaternion.identity);
		}
		ParalysisEffect.transform.SetParent(base.transform);
	}

	private void StateCrashStuned()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateCrashStunedEnd()
	{
		Object.Destroy(ParalysisEffect);
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 8;
		StateEnd = Time.time + 1.3f;
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

	private void OnOrderIssue()
	{
		if (EnemyType != Type.eChaser && EnemyState != State.Move && EnemyState != State.ChargeAttack && EnemyState != State.Stuned && EnemyState != State.CrashStuned && (bool)Target)
		{
			TriggerStateSwitch = true;
			StateMachine.ChangeState(StateFound);
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 1.25f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
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
		if (GroupDestroy || EnemyType != Type.eChaser)
		{
			return;
		}
		eLiner[] array = Object.FindObjectsOfType<eLiner>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(Method == "Destroy"))
			{
				if (Method == "Command" && array[i] != this && array[i].GroupID == GroupID)
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
