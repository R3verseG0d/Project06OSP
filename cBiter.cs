using UnityEngine;

public class cBiter : EnemyBase
{
	public enum Type
	{
		cBiter = 0,
		cStalker = 1
	}

	public enum Mode
	{
		Fix = 0,
		Freeze = 1,
		Normal = 2,
		Wall_Fix = 3,
		Wall_Normal = 4
	}

	public enum State
	{
		Appear = 0,
		Freeze = 1,
		Stand = 2,
		Search = 3,
		Seek = 4,
		Move = 5,
		FollowPlayer = 6,
		BreathAttack = 7,
		JumpAttack = 8,
		Stuned = 9,
		DamageKnockBack = 10
	}

	[Header("Framework")]
	public Mode CreatureMode;

	[Header("Prefab")]
	public Type EnemyType;

	public ParticleSystem FireFX;

	public GameObject FireArea;

	public LaserBeam Beam;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip AppearSound;

	public AudioClip JumpSound;

	public AudioClip BiteSound;

	public AudioClip SighSound;

	public AudioSource[] FireBreathSources;

	private State EnemyState;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private bool StartBreath;

	private bool Jumped;

	private bool Landed;

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
		_Rigidbody.velocity = Physics.gravity * 0.5f;
		if (CreatureMode == Mode.Wall_Fix || CreatureMode == Mode.Wall_Normal)
		{
			_Rigidbody.isKinematic = true;
		}
		Grounded = false;
		base.transform.position = Position;
		base.transform.rotation = StartRotation;
		if (CreatureMode != Mode.Freeze)
		{
			if (!IsFixed)
			{
				StateAppearStart();
				StateMachine.Initialize(StateAppear);
				if ((bool)TeleportEffectPrefab)
				{
					Object.Instantiate(TeleportEffectPrefab, Mesh.position - Mesh.forward * 1f, StartRotation);
				}
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
	}

	private void Reset()
	{
		Target = null;
		if (CreatureMode != Mode.Freeze && (bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position - Mesh.forward * 1f, StartRotation);
		}
		if ((bool)ParalysisEffect)
		{
			Animator.speed = 1f;
			Stuned = false;
			Object.Destroy(ParalysisEffect);
		}
		CurHealth = MaxHealth;
		if (EnemyType == Type.cStalker)
		{
			DeactivateBeam();
		}
		base.transform.position = StartPosition;
		base.transform.rotation = StartRotation;
		Mesh.localEulerAngles = Vector3.zero;
		base.gameObject.SetActive(value: false);
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.cStalker)
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

	private void StateAppearStart()
	{
		EnemyState = State.Appear;
		Animation = 0;
		StateTime[0] = Time.time + 2.5f;
		Audio.PlayOneShot(AppearSound, Audio.volume);
	}

	private void StateAppear()
	{
		if (Time.time > StateTime[0])
		{
			_Rigidbody.velocity = Vector3.zero;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateAppearEnd()
	{
	}

	private void StateFreezeStart()
	{
		EnemyState = State.Freeze;
		Animation = 10;
	}

	private void StateFreeze()
	{
		if (GetTarget(Mesh.forward, 150f, Radial: true))
		{
			StateMachine.ChangeState(StateAppear);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position - Mesh.forward * 1f, StartRotation);
			}
		}
	}

	private void StateFreezeEnd()
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
			TargetPosition = base.transform.position + Vector3.up - ((CreatureMode == Mode.Normal && Random.value > 0.5f) ? Mesh.right : Mesh.forward);
			StateVar[0] = 1;
		}
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
		{
			Animation = 2;
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if (CreatureMode == Mode.Normal && Random.value > 0.5f)
			{
				StateMachine.ChangeState(StateMove);
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
			ToAttackState();
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateMoveStart()
	{
		EnemyState = State.Move;
		Animation = 3;
		StateEnd = Time.time + 2.5f;
	}

	private void StateMove()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 3f);
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * 3f);
		}
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateMoveEnd()
	{
	}

	private void ToAttackState()
	{
		if (CreatureMode == Mode.Normal)
		{
			if (GetDistance() < 18.9f)
			{
				SubOnRangeAttack();
			}
			else
			{
				StateMachine.ChangeState(StateFollowPlayer);
			}
		}
		else
		{
			StateMachine.ChangeState(StateBreathAttack);
		}
	}

	private void SubOnRangeAttack()
	{
		if (GetDistance() < 9.45f)
		{
			StateMachine.ChangeState(StateBreathAttack);
		}
		else if (GetDistance() < 18.9f)
		{
			StateMachine.ChangeState(StateJumpAttack);
		}
	}

	private void StateFollowPlayerStart()
	{
		EnemyState = State.FollowPlayer;
		Animation = 5;
	}

	private void StateFollowPlayer()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 5f);
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * 5f);
		}
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (GetDistance() < 18.9f)
		{
			SubOnRangeAttack();
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

	private void StateBreathAttackStart()
	{
		EnemyState = State.BreathAttack;
		Animation = 4;
		StateEnd = Time.time + 3f;
		StateTime[0] = Time.time + 0.4f;
		StateTime[1] = -1f;
		StartBreath = false;
	}

	private void StateBreathAttack()
	{
		if (EnemyType == Type.cStalker)
		{
			Vector3 targetPos;
			if (Physics.Raycast(Beam.transform.position, Beam.transform.forward, out var hitInfo, 65f))
			{
				targetPos = hitInfo.point;
				if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
				{
					HitInfo value = new HitInfo(base.transform, Beam.transform.forward * 25f, 0);
					hitInfo.transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				targetPos = Beam.transform.position + Beam.transform.forward * 65f;
			}
			Beam.UpdateBeam(targetPos);
		}
		if (StateTime[1] == -1f)
		{
			if (Time.time > StateEnd)
			{
				StateTime[1] = Time.time + ((EnemyType == Type.cBiter) ? 5f : 3f);
				Animation = 1;
				if (StartBreath)
				{
					if (EnemyType == Type.cBiter)
					{
						ManageFireFX(Play: false);
						FireBreathSources[0].Stop();
						FireBreathSources[1].Stop();
					}
					else
					{
						DeactivateBeam();
					}
					StartBreath = false;
				}
				return;
			}
			if (!StartBreath && Time.time > StateTime[0])
			{
				if (EnemyType == Type.cBiter)
				{
					ManageFireFX(Play: true);
					FireBreathSources[1].volume = 0f;
					FireBreathSources[0].Play();
					FireBreathSources[1].Play();
				}
				else
				{
					ActivateBeam();
				}
				StartBreath = true;
			}
			if (EnemyType == Type.cBiter && StartBreath && !FireBreathSources[0].isPlaying)
			{
				FireBreathSources[1].volume = 1f;
			}
		}
		else if (Time.time > StateTime[1])
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateBreathAttackEnd()
	{
		if (EnemyType == Type.cBiter)
		{
			ManageFireFX(Play: false);
			FireBreathSources[0].Stop();
			FireBreathSources[1].Stop();
		}
		else
		{
			DeactivateBeam();
		}
	}

	private void StateJumpAttackStart()
	{
		EnemyState = State.JumpAttack;
		Animation = 6;
		StateTime[0] = Time.time + 1f;
		Jumped = false;
		Landed = false;
		Audio.PlayOneShot(SighSound, Audio.volume);
	}

	private void StateJumpAttack()
	{
		if (!Jumped && Time.time > StateTime[0])
		{
			StateTime[0] = Time.time + 0.25f;
			_Rigidbody.velocity = Mesh.forward * 8f + Vector3.up * 8f;
			Animation = 8;
			Audio.PlayOneShot(JumpSound, Audio.volume);
			Jumped = true;
		}
		else
		{
			if (!Landed && Time.time > StateTime[0] && Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f, base.Collision_Mask))
			{
				base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
				base.transform.position = hitInfo.point;
				StateTime[0] = Time.time + 1f;
				Animation = 9;
				Audio.PlayOneShot(BiteSound, Audio.volume);
				Landed = true;
			}
			if (!Landed)
			{
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation, Time.deltaTime * 10f);
			}
		}
		if (Landed && Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateJumpAttackEnd()
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
		StateEnd = Time.time + 1f;
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

	private void ManageFireFX(bool Play)
	{
		if (Play)
		{
			FireFX.Play();
		}
		else
		{
			FireFX.Stop();
		}
		FireArea.SetActive(Play);
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
			if (!ParalysisEffect && EnemyState != State.JumpAttack && !IsPsychokinesis && !PsychoThrown)
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

	private void ActivateBeam()
	{
		Beam.State = 0;
	}

	private void DeactivateBeam()
	{
		Beam.State = 2;
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
