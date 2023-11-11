using UnityEngine;

public class eRounder : EnemyBase
{
	public enum Type
	{
		eRounder = 0,
		eCommander = 1
	}

	public enum Mode
	{
		Alarm = 0,
		Fix = 1,
		Master = 2,
		Normal = 3,
		Slave = 4,
		Twn_Chase = 5,
		Twn_Escape = 6
	}

	public enum State
	{
		Stand = 0,
		Search = 1,
		Seek = 2,
		Roam = 3,
		Command = 4,
		Guard = 5,
		Move = 6,
		FollowPlayer = 7,
		ChargeAttack = 8,
		GroundPound = 9,
		Found = 10,
		Stuned = 11,
		CrashStuned = 12,
		DamageKnockBack = 13
	}

	[Header("Framework")]
	public Mode RobotMode;

	public string GroupID;

	[Header("Prefab")]
	public Type EnemyType;

	public LayerMask FrontalColLayer;

	[Header("Instantiation")]
	public GameObject PoundFX;

	public GameObject ShieldEffectPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip[] eSeries;

	private State EnemyState;

	private bool TriggerStateSwitch;

	private bool GroupDestroy;

	private bool PlayedSounds;

	private float StateEnd;

	private float StateSound;

	private int StateVar;

	private int CommanderAction;

	private int RounderAction;

	private int SlaveAction;

	private int FoundState;

	private float Timer;

	private Vector3 DistFromPlayer;

	private bool MoveSwitch;

	private float TimeBeforeCharge;

	private float TimeAfterCharge;

	private bool Charging;

	private bool SpawnedPound;

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
			Blocking = false;
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
		Blocking = false;
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.eCommander)
		{
			return 1;
		}
		return 0;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(2.25f);
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
		Animation = 2;
		StateEnd = Time.time + 1.9665f;
		StateVar = 0;
	}

	private void StateSearch()
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
			Animation = 2;
			StateEnd = Time.time + 1.9665f;
			StateVar = 0;
			if (EnemyType == Type.eRounder && RobotMode == Mode.Normal && Random.value > 0.5f)
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
		Timer = 0f;
	}

	private void StateSeek()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f);
		if (EnemyType == Type.eCommander)
		{
			if (!(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir))
			{
				return;
			}
			Timer += Time.fixedDeltaTime;
			switch (CommanderAction)
			{
			case 0:
				if (Timer > 2f)
				{
					StateMachine.ChangeState(StateCommand);
				}
				break;
			case 1:
				if (Timer > 1f)
				{
					StateMachine.ChangeState(StateGuard);
				}
				break;
			case 2:
				if (Timer > 1f)
				{
					StateMachine.ChangeState(StateChargeAttack);
				}
				break;
			}
			return;
		}
		Timer += Time.fixedDeltaTime;
		switch (RobotMode)
		{
		case Mode.Normal:
			if (Timer > 4f)
			{
				if (RounderAction == 0)
				{
					StateMachine.ChangeState(StateFollowPlayer);
				}
				else
				{
					StateMachine.ChangeState(StateChargeAttack);
				}
			}
			break;
		case Mode.Fix:
			if (Timer > 3f)
			{
				StateMachine.ChangeState(StateGroundPound);
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
		StateEnd = Time.time + 2.5f;
	}

	private void StateRoam()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 2.5f;
		}
		else
		{
			_Rigidbody.velocity = Mesh.forward * 2.5f;
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
		Animation = 4;
		StateEnd = Time.time + 1.58f;
		StateSound = Time.time + 1f;
		CommanderAction = 1;
	}

	private void StateCommand()
	{
		if (Time.time > StateSound && !PlayedSounds)
		{
			PlayedSounds = true;
			Audio.PlayOneShot(eSeries[5], Audio.volume);
		}
		if (Time.time > StateEnd)
		{
			StateSound = 0f;
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
			PlayedSounds = false;
			if ((bool)Target)
			{
				ManageSlaves("Command");
			}
		}
	}

	private void StateCommandEnd()
	{
	}

	private void StateGuardStart()
	{
		EnemyState = State.Guard;
		Animation = 5;
		StateEnd = Time.time + 5f;
		StateSound = Time.time + 0.4f;
		CommanderAction = ((RobotMode != Mode.Master) ? 2 : 0);
		Blocking = true;
	}

	private void StateGuard()
	{
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f, Smooth: true);
		if (Time.time > StateSound && !PlayedSounds)
		{
			PlayedSounds = true;
			Audio.PlayOneShot(eSeries[3], Audio.volume);
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateSound = 0f;
			PlayedSounds = false;
			StateMachine.ChangeState(StateStand);
			Blocking = false;
		}
	}

	private void StateGuardEnd()
	{
		Blocking = false;
	}

	private void StateMoveStart()
	{
		EnemyState = State.Move;
		Animation = 1;
		StateEnd = Time.time + 1.25f;
		MoveSwitch = Random.value < 0.75f;
	}

	private void StateMove()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		DistFromPlayer = (((bool)Target && GetDistance() < 6f) ? ((TargetPosition - base.transform.position).normalized * -6f) : Vector3.zero);
		Mesh.localRotation = SmoothFaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		_Rigidbody.velocity = Mesh.right * (MoveSwitch ? 6f : (-6f)) + DistFromPlayer;
		if (Time.time > StateEnd || !Target)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveEnd()
	{
		SlaveAction = 1;
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		Animation = 1;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 3f;
		}
		else
		{
			_Rigidbody.velocity = Mesh.forward * 3f;
		}
		if (GetDistance() < 5f)
		{
			StateMachine.ChangeState(StateGroundPound);
		}
		if (GetDistance() > 40f || !Target)
		{
			Target = null;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateFollowPlayerEnd()
	{
	}

	private void StateChargeAttackStart()
	{
		EnemyState = State.ChargeAttack;
		Animation = 8;
		StateEnd = Time.time + 3f;
		TimeAfterCharge = 0f;
		TimeBeforeCharge = Time.time + 1f;
		StateSound = Time.time + 0.31f;
		RounderAction = 0;
		CommanderAction = 0;
	}

	private void StateChargeAttack()
	{
		if (Time.time > TimeBeforeCharge)
		{
			TimeAfterCharge += Time.fixedDeltaTime;
			Charging = TimeAfterCharge < 1.1f;
			if (TimeAfterCharge < 1.1f)
			{
				if (PlayedSounds)
				{
					PlayedSounds = false;
					Audio.PlayOneShot(eSeries[4], Audio.volume);
				}
				if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
				{
					_Rigidbody.velocity = Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 12f;
				}
				else
				{
					_Rigidbody.velocity = Mesh.forward * 12f;
				}
				if (Physics.Raycast(base.transform.position + base.transform.up * 1.25f, Mesh.forward, out var hitInfo2, 0.75f, FrontalColLayer) && hitInfo2.transform.GetComponent<Collider>() != GetComponent<Collider>())
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
		if (Time.time > StateSound && !PlayedSounds)
		{
			PlayedSounds = true;
			Audio.PlayOneShot(eSeries[3], Audio.volume);
		}
		if (Time.time > StateEnd)
		{
			StateSound = 0f;
			StateEnd = 0f;
			TimeBeforeCharge = 0f;
			PlayedSounds = false;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateChargeAttackEnd()
	{
		SlaveAction = 0;
	}

	private void StateGroundPoundStart()
	{
		EnemyState = State.GroundPound;
		Animation = 9;
		StateEnd = Time.time + 1.5f;
		RounderAction = 1;
		SpawnedPound = false;
	}

	private void StateGroundPound()
	{
		if (!SpawnedPound && Time.time > StateEnd - 0.5f)
		{
			Object.Instantiate(PoundFX, base.transform.position, Quaternion.identity);
			SpawnedPound = true;
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateGroundPoundEnd()
	{
	}

	private void StateFoundStart()
	{
		EnemyState = State.Found;
		Animation = 7;
		StateEnd = Time.time + 1f;
		Audio.PlayOneShot(eSeries[(EnemyType != Type.eCommander) ? 1u : 0u], Audio.volume);
		Audio.PlayOneShot(eSeries[2], Audio.volume);
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
		if (EnemyType == Type.eRounder && RobotMode == Mode.Normal)
		{
			StateMachine.ChangeState(StateFollowPlayer);
		}
		if (EnemyType != Type.eCommander)
		{
			if (RobotMode != Mode.Normal)
			{
				if (TriggerStateSwitch)
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
				StateMachine.ChangeState(StateFollowPlayer);
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
		Animation = 3;
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
		Animation = 6;
		StateEnd = Time.time + 1.9665f;
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void OnOrderIssue()
	{
		if (EnemyType != Type.eCommander && EnemyState != State.Move && EnemyState != State.ChargeAttack && EnemyState != State.Stuned && EnemyState != State.CrashStuned && (bool)Target)
		{
			TriggerStateSwitch = true;
			Audio.PlayOneShot(eSeries[6], Audio.volume);
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
		if (Blocking)
		{
			if (HitTimer >= GetHitTimer(HitInfo))
			{
				CreateShieldFX(HitInfo, ShieldEffectPrefab);
				HitTimer = 0f;
				if (((bool)HitInfo.player.GetComponent<SonicNew>() && (HitInfo.player.GetComponent<SonicNew>().UsingWhiteGem || HitInfo.player.GetComponent<SonicNew>().IsSuper)) || ((bool)HitInfo.player.GetComponent<Shadow>() && HitInfo.player.GetComponent<PlayerBase>().GetState() != "ChaosBlast" && HitInfo.player.GetComponent<Shadow>().IsFullPower))
				{
					StateMachine.ChangeState(StateCrashStuned);
				}
			}
		}
		else
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
	}

	private void ManageSlaves(string Method, HitInfo HitInfo = null)
	{
		if (GroupDestroy || EnemyType != Type.eCommander)
		{
			return;
		}
		eRounder[] array = Object.FindObjectsOfType<eRounder>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(Method == "Destroy"))
			{
				if (Method == "Command" && array[i] != this && array[i].GroupID == GroupID && array[i].RobotMode == Mode.Slave)
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
