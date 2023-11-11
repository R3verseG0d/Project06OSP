using UnityEngine;

public class cCrawler : EnemyBase
{
	public enum Type
	{
		cCrawler = 0,
		cGazer = 1
	}

	public enum Mode
	{
		Alarm = 0,
		Fix = 1,
		Freeze = 2,
		Normal = 3,
		Wall_Fix = 4,
		Wall_Homing = 5
	}

	public enum State
	{
		Appear = 0,
		Freeze = 1,
		Stand = 2,
		Search = 3,
		Seek = 4,
		Guard = 5,
		GuardBreak = 6,
		Attack = 7,
		ShootBomb = 8,
		BreathAttack = 9,
		DivingAttack = 10,
		Found = 11,
		Stuned = 12,
		DamageKnockBack = 13
	}

	[Header("Framework")]
	public Mode CreatureMode;

	[Header("Prefab")]
	public Type EnemyType;

	public GameObject AttackCol;

	public Transform BombPoint;

	public ParticleSystem BreathFX;

	public GameObject BreathArea;

	[Header("Instantiation")]
	public GameObject ImpactPrefab;

	public GameObject BombPrefab;

	public GameObject ShieldEffectPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioSource UndergroundAudio;

	public AudioClip RoarSound;

	public AudioClip AttackSound;

	public AudioClip ProjectileSound;

	public AudioClip DiveSound;

	public AudioClip WinceSound;

	public AudioClip JawCloseSound;

	public AudioClip JawOpenSound;

	public AudioSource[] BreathAttackSources;

	private State EnemyState;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int MaxAttack;

	private int AttackCount;

	private int FoundState;

	private bool StartBreath;

	private bool LargeDive;

	private void Start()
	{
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition;
		BaseStart();
		Transfer();
	}

	private void Transfer()
	{
		if (Position == Vector3.zero)
		{
			return;
		}
		FoundState = 0;
		Grounded = false;
		base.transform.position = Position;
		base.transform.rotation = StartRotation;
		if (CreatureMode != Mode.Freeze)
		{
			StateAppearStart();
			StateMachine.Initialize(StateAppear);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position, StartRotation);
			}
		}
		else
		{
			StateFreezeStart();
			StateMachine.Initialize(StateFreeze);
		}
		Blocking = false;
	}

	private void Reset()
	{
		Target = null;
		if (CreatureMode != Mode.Freeze && (bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position, StartRotation);
		}
		if ((bool)ParalysisEffect)
		{
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
		if (enemyType == Type.cGazer)
		{
			return 5;
		}
		return 2;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(6.5f);
	}

	private void StateAppearStart()
	{
		EnemyState = State.Appear;
		Animation = 0;
		StateTime[0] = Time.time;
		StateTime[1] = Time.time;
		StateVar[0] = 0;
	}

	private void StateAppear()
	{
		if (StateVar[0] != 1 && Time.time - StateTime[1] > 1.5f)
		{
			Object.Instantiate(ImpactPrefab, base.transform.position, base.transform.rotation);
			Audio.PlayOneShot(RoarSound, Audio.volume);
			StateVar[0] = 1;
		}
		if (Time.time - StateTime[0] > 2.75f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateAppearEnd()
	{
	}

	private void StateFreezeStart()
	{
		EnemyState = State.Freeze;
		Animation = 13;
	}

	private void StateFreeze()
	{
		if (GetTarget(Mesh.forward, 150f, Radial: true))
		{
			StateAppearStart();
			StateMachine.ChangeState(StateAppear);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position, StartRotation);
			}
		}
	}

	private void StateFreezeEnd()
	{
	}

	private void StateStandStart()
	{
		EnemyState = State.Stand;
		Animation = (Target ? 1 : 12);
		StateTime[0] = Time.time + 2.5f;
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
		Animation = (Target ? 1 : 12);
		StateTime[0] = Time.time + 1.9665f;
		StateVar[0] = 0;
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
		if (Time.time > StateTime[0])
		{
			if (StateVar[0] == 0)
			{
				TargetPosition = base.transform.position + Vector3.up - Mesh.forward;
				StateVar[0] = 1;
			}
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
			if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
			{
				StateTime[0] = Time.time + 1.9665f;
				StateVar[0] = 0;
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
		if ((bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
	}

	private void StateSeek()
	{
		if (CreatureMode == Mode.Wall_Homing && (bool)Target)
		{
			TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir && CreatureMode != Mode.Wall_Homing)
		{
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void ToAttackState()
	{
		if (CreatureMode == Mode.Alarm)
		{
			if (GetDistance() < 5.25f)
			{
				StateMachine.ChangeState(StateAttack);
			}
			else
			{
				StateMachine.ChangeState(StateShootBomb);
			}
		}
		else if (CreatureMode == Mode.Normal)
		{
			if (GetDistance() < 5.25f)
			{
				StateMachine.ChangeState(StateAttack);
			}
			else if (GetDistance() < 10.5f)
			{
				LargeDive = false;
				DiveOrAttack();
			}
			else
			{
				LargeDive = true;
				DiveOrAttack();
			}
		}
		else if (CreatureMode == Mode.Fix || CreatureMode == Mode.Freeze)
		{
			if (EnemyType == Type.cGazer)
			{
				if (GetDistance() < 5.25f)
				{
					StateMachine.ChangeState(StateAttack);
				}
				else if (GetDistance() < 10.5f)
				{
					StateMachine.ChangeState(StateBreathAttack);
				}
				else
				{
					StateMachine.ChangeState(StateShootBomb);
				}
			}
			else if (GetDistance() < 5.25f)
			{
				StateMachine.ChangeState(StateAttack);
			}
			else
			{
				StateMachine.ChangeState(StateShootBomb);
			}
		}
		else if (CreatureMode == Mode.Wall_Fix)
		{
			if (GetDistance() < 5.25f)
			{
				StateMachine.ChangeState(StateBreathAttack);
			}
			else
			{
				StateMachine.ChangeState(StateShootBomb);
			}
		}
	}

	private void DiveOrAttack()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.25f + Mesh.forward * ((!LargeDive) ? 6.5f : 13f), -Mesh.up, out var _, 0.3f, base.Collision_Mask))
		{
			StateMachine.ChangeState(StateDivingAttack);
		}
		else
		{
			StateMachine.ChangeState(StateAttack);
		}
	}

	private void StateGuardStart()
	{
		EnemyState = State.Guard;
		Animation = 8;
		Animator.SetTrigger("On Guard");
		StateTime[0] = Time.time;
		StateVar[0] = 0;
		Audio.PlayOneShot(JawCloseSound, Audio.volume);
		Blocking = true;
	}

	private void StateGuard()
	{
		if (StateVar[0] == 0)
		{
			if (Time.time - StateTime[0] > 3f)
			{
				Animation = 9;
				Audio.PlayOneShot(JawOpenSound, Audio.volume);
				StateTime[0] = Time.time;
				Blocking = false;
				StateVar[0] = 1;
			}
			if (Time.time - StateTime[0] > 1f && (bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1.5f);
			}
		}
		else if (Time.time - StateTime[0] > 1.5f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateGuardEnd()
	{
		Blocking = false;
	}

	private void StateGuardBreakStunedStart()
	{
		EnemyState = State.GuardBreak;
		StateEnd = Time.time + 1.3f;
		Animation = 10;
	}

	private void StateGuardBreak()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateGuardBreakEnd()
	{
	}

	private void StateAttackStart()
	{
		EnemyState = State.Attack;
		Animation = 3;
		StateEnd = Time.time + 2.25f;
		StateVar[0] = 0;
		StateVar[1] = 0;
		AttackCol.SetActive(value: true);
	}

	private void StateAttack()
	{
		if (StateVar[1] != 1 && Time.time > StateEnd - 1.5f)
		{
			Audio.PlayOneShot(AttackSound, Audio.volume);
			StateVar[1] = 1;
		}
		if (StateVar[0] == 0)
		{
			if (Time.time > StateEnd)
			{
				AttackCol.SetActive(value: false);
				if (EnemyType == Type.cGazer)
				{
					StateMachine.ChangeState(StateGuard);
					return;
				}
				Animation = 1;
				StateEnd = Time.time + 2f;
				StateVar[0] = 1;
			}
		}
		else if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateAttackEnd()
	{
		AttackCol.SetActive(value: false);
	}

	private void StateShootBombStart()
	{
		EnemyState = State.ShootBomb;
		Animation = 4;
		StateTime[0] = Time.time;
		StateTime[1] = Time.time;
		StateVar[0] = 0;
		StateVar[1] = 0;
		AttackCount = 0;
		MaxAttack = 2;
	}

	private void StateShootBomb()
	{
		if (StateVar[0] == 0)
		{
			if ((bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 3f);
			}
			else
			{
				StateMachine.ChangeState(StateStand);
			}
			if (Time.time - StateTime[1] > 2.25f)
			{
				if (AttackCount == MaxAttack)
				{
					Animation = 1;
					StateEnd = Time.time + ((EnemyType == Type.cCrawler) ? 2.5f : 2f);
					StateVar[0] = 1;
				}
				Animator.SetTrigger("Attack");
				StateTime[0] = Time.time;
				StateTime[1] = Time.time;
				StateVar[1] = 0;
			}
			if (Time.time - StateTime[0] > 1f && AttackCount <= MaxAttack && StateVar[1] == 0)
			{
				Audio.PlayOneShot(ProjectileSound, Audio.volume);
				GameObject gameObject = Object.Instantiate(BombPrefab, BombPoint.position, BombPoint.rotation);
				gameObject.GetComponent<Rigidbody>().AddForce((TargetPosition - BombPoint.position).normalized * ((EnemyType == Type.cCrawler) ? 25f : 30f) + Mesh.up * -2.5f, ForceMode.Impulse);
				gameObject.GetComponent<TimedBomb>().Launched = true;
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(gameObject.GetComponentInChildren<Collider>(), collider);
				}
				AttackCount++;
				StateVar[1] = 1;
			}
		}
		else if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateShootBombEnd()
	{
	}

	private void StateBreathAttackStart()
	{
		EnemyState = State.BreathAttack;
		Animation = 4;
		StateEnd = Time.time + 2f;
		StateTime[0] = Time.time + 0.8f;
		StateTime[1] = -1f;
		StartBreath = false;
	}

	private void StateBreathAttack()
	{
		if (StateTime[1] == -1f)
		{
			if (Time.time > StateEnd)
			{
				StateTime[1] = Time.time + 3f;
				Animation = 1;
				if (StartBreath)
				{
					ManageBreathFX(Play: false);
					BreathAttackSources[0].Stop();
					BreathAttackSources[1].Stop();
					StartBreath = false;
				}
				return;
			}
			if (!StartBreath && Time.time > StateTime[0])
			{
				ManageBreathFX(Play: true);
				BreathAttackSources[1].volume = 0f;
				BreathAttackSources[0].Play();
				BreathAttackSources[1].Play();
				StartBreath = true;
			}
			if (StartBreath && !BreathAttackSources[0].isPlaying)
			{
				BreathAttackSources[1].volume = 1f;
			}
		}
		else if (Time.time > StateTime[1])
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateBreathAttackEnd()
	{
		ManageBreathFX(Play: false);
		BreathAttackSources[0].Stop();
		BreathAttackSources[1].Stop();
	}

	private void StateDivingAttackStart()
	{
		EnemyState = State.DivingAttack;
		Animation = ((!LargeDive) ? 5 : 6);
		StateTime[0] = Time.time + ((EnemyType != Type.cGazer) ? 7f : 8f);
		StateTime[1] = Time.time + 2.75f;
		StateVar[0] = 0;
		StateVar[1] = 0;
		Audio.PlayOneShot(DiveSound, Audio.volume);
		for (int i = 0; i < HomingTargets.Length; i++)
		{
			if ((bool)HomingTargets[i])
			{
				HomingTargets[i].gameObject.tag = "Untagged";
			}
		}
	}

	private void StateDivingAttack()
	{
		if (StateVar[0] == 0)
		{
			if (StateVar[1] < 1 && Time.time > StateTime[1])
			{
				Object.Instantiate(ImpactPrefab, base.transform.position + Mesh.forward * ((!LargeDive) ? 6.5f : 13f), base.transform.rotation);
				Object.Instantiate(TeleportEffectPrefab, base.transform.position + Mesh.forward * ((!LargeDive) ? 6.5f : 13f), base.transform.rotation);
				UndergroundAudio.Play();
				StateTime[1] = Time.time + 2.75f;
				StateVar[1] = 1;
			}
			if (StateVar[1] == 1 && StateVar[1] != 2 && Time.time > StateTime[0] - 1.5f)
			{
				base.transform.position = base.transform.position + Mesh.forward * ((!LargeDive) ? 6.25f : 12.5f);
				if ((bool)Target)
				{
					TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				}
				Vector3 vector = base.transform.InverseTransformDirection((TargetPosition - base.transform.position).normalized);
				vector.y = 0f;
				Mesh.localRotation = Quaternion.LookRotation(vector.normalized);
				if (GetDistance() < ((EnemyType != Type.cGazer) ? 7.5f : 12.5f))
				{
					base.transform.position = new Vector3(TargetPosition.x, base.transform.position.y, TargetPosition.z);
				}
				Object.Instantiate(TeleportEffectPrefab, base.transform.position, base.transform.rotation);
				Animation = 0;
				StateVar[1] = 2;
			}
			if (Time.time > StateTime[0])
			{
				UndergroundAudio.Stop();
				Object.Instantiate(ImpactPrefab, base.transform.position, base.transform.rotation);
				StateTime[0] = Time.time + 1.5f;
				StateVar[0] = 1;
			}
		}
		else if (StateVar[0] == 1)
		{
			if (Time.time > StateTime[0])
			{
				Animation = 1;
				StateTime[0] = Time.time + ((EnemyType != Type.cGazer) ? 3f : 2f);
				StateVar[0] = 2;
			}
		}
		else
		{
			if (!(Time.time > StateTime[0]))
			{
				return;
			}
			for (int i = 0; i < HomingTargets.Length; i++)
			{
				if ((bool)HomingTargets[i])
				{
					HomingTargets[i].gameObject.tag = "HomingTarget";
				}
			}
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDivingAttackEnd()
	{
		for (int i = 0; i < HomingTargets.Length; i++)
		{
			if ((bool)HomingTargets[i])
			{
				HomingTargets[i].gameObject.tag = "HomingTarget";
			}
		}
	}

	private void StateFoundStart()
	{
		EnemyState = State.Found;
		Animation = 2;
		StateEnd = Time.time + 2f;
	}

	private void StateFound()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateFoundEnd()
	{
	}

	private void StateStunedStart()
	{
		EnemyState = State.Stuned;
		Animation = 11;
		StateEnd = Time.time + 7.5f;
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
		Stuned = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateDamageKnockBackStart()
	{
		EnemyState = State.DamageKnockBack;
		Animation = 7;
		StateEnd = Time.time + 1.5f;
		Audio.PlayOneShot(WinceSound, Audio.volume);
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

	private void ManageBreathFX(bool Play)
	{
		if (Play)
		{
			BreathFX.Play();
		}
		else
		{
			BreathFX.Stop();
		}
		BreathArea.SetActive(Play);
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
		if (Blocking)
		{
			if (HitTimer >= GetHitTimer(HitInfo))
			{
				CreateShieldFX(HitInfo, ShieldEffectPrefab);
				HitTimer = 0f;
				if (((bool)HitInfo.player.GetComponent<SonicNew>() && (HitInfo.player.GetComponent<SonicNew>().UsingWhiteGem || HitInfo.player.GetComponent<SonicNew>().IsSuper)) || ((bool)HitInfo.player.GetComponent<Shadow>() && HitInfo.player.GetComponent<PlayerBase>().GetState() != "ChaosBlast" && HitInfo.player.GetComponent<Shadow>().IsFullPower))
				{
					StateMachine.ChangeState(StateGuardBreak);
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
				if (!ParalysisEffect && EnemyState != State.DivingAttack && !IsPsychokinesis && !PsychoThrown)
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
