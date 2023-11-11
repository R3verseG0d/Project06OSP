using UnityEngine;

public class cGolem : EnemyBase
{
	public enum Type
	{
		cGolem = 0,
		cTitan = 1
	}

	public enum Mode
	{
		Alarm = 0,
		Fix = 1,
		Freeze = 2,
		Normal = 3
	}

	[Header("Framework")]
	public Mode CreatureMode;

	[Header("Prefab")]
	public Type EnemyType;

	public GameObject ImpactPrefab;

	public GameObject PossesionFX;

	public GameObject PossesionPrefab;

	public Transform PossesionPoint;

	public ParticleSystem FX;

	public Light HeadLight;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip RoarSound;

	public AudioClip WinceSound;

	public AudioClip ArmDownSound;

	public AudioClip MoveSound;

	private GameObject PossesionObj;

	private bool IsOpenHead;

	private bool HazardMoveFront;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int FoundState;

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
		if (!(Position == Vector3.zero))
		{
			FoundState = 0;
			_Rigidbody.velocity = Physics.gravity * 0.5f;
			Grounded = false;
			base.transform.position = Position;
			base.transform.rotation = StartRotation;
			if (CreatureMode != Mode.Freeze)
			{
				StateAppearStart();
				StateMachine.Initialize(StateAppear);
			}
			else
			{
				StateStandStart();
				StateMachine.Initialize(StateStand);
			}
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 3f, StartRotation);
			}
		}
	}

	private void Reset()
	{
		Target = null;
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 3f, StartRotation);
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
		DestroyPossesion();
		base.gameObject.SetActive(value: false);
	}

	private int EnemyMaxHealth()
	{
		Type enemyType = EnemyType;
		if (enemyType == Type.cTitan)
		{
			return 5;
		}
		return 3;
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(5.75f);
		HeadLight.intensity = Mathf.Lerp(HeadLight.intensity, IsOpenHead ? 10f : 0f, Time.deltaTime * 10f);
		ParticleSystem.EmissionModule emission = FX.emission;
		emission.enabled = IsOpenHead;
	}

	private void StateAppearStart()
	{
		Animation = 0;
		StateTime[0] = Time.time + 2f;
		Audio.PlayOneShot(RoarSound, Audio.volume);
		if (EnemyType == Type.cTitan)
		{
			RevealHead(Reveal: true);
		}
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
		if (EnemyType == Type.cTitan)
		{
			RevealHead(Reveal: false);
		}
	}

	private void StateStandStart()
	{
		Animation = 1;
		StateTime[0] = Time.time + 2.5f;
	}

	private void StateStand()
	{
		if (GetTarget(Mesh.forward, (CreatureMode != Mode.Freeze) ? 360f : 50f))
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
		else if (Time.time > StateTime[0] && CreatureMode != Mode.Freeze)
		{
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateStandEnd()
	{
	}

	private void StateSearchStart()
	{
		Animation = 2;
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
				StateMachine.ChangeState(StateRoam);
			}
		}
	}

	private void StateSearchEnd()
	{
	}

	private void StateSeekStart()
	{
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

	private void StateRoamStart()
	{
		Animation = 3;
		StateEnd = Time.time + 3f;
	}

	private void StateRoam()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 2.5f);
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * 2.5f);
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
		Animation = 3;
		StateTime[0] = Time.time;
	}

	private void StateFollowPlayer()
	{
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			_Rigidbody.velocity = SetMoveVel(Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 2.5f);
		}
		else
		{
			_Rigidbody.velocity = SetMoveVel(Mesh.forward * 2.5f);
		}
		if (GetDistance() < 5.25f || Time.time - StateTime[0] > 2f)
		{
			StateMachine.ChangeState(StateGroundSmash);
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

	private void ToAttackState()
	{
		if (CreatureMode == Mode.Fix || CreatureMode == Mode.Alarm)
		{
			if (GetDistance() < 7.875f)
			{
				RevealHead(Reveal: true);
				StateMachine.ChangeState(StateGroundSmash);
			}
			else
			{
				StateMachine.ChangeState(StateThrowHazard);
			}
		}
		else if (GetDistance() < 5.25f)
		{
			RevealHead(Reveal: true);
			StateMachine.ChangeState(StateGroundSmash);
		}
		else if (GetDistance() < 10.5f)
		{
			StateMachine.ChangeState(StateFollowPlayer);
		}
		else if (GetDistance() < 15.75f)
		{
			HazardMoveFront = false;
			StateMachine.ChangeState(StateThrowHazard);
		}
		else
		{
			HazardMoveFront = true;
			StateMachine.ChangeState(StateThrowHazard);
		}
	}

	private void StateGroundSmashStart()
	{
		Animation = 5;
		Animator.SetTrigger("On Smash");
		StateTime[0] = Time.time + 5.5f;
		StateTime[1] = Time.time + 7f;
		StateTime[2] = Time.time + 2f;
		StateTime[3] = Time.time + 1.5f;
		StateVar[0] = 0;
		StateVar[1] = 0;
		StateVar[2] = 0;
		Audio.PlayOneShot(RoarSound, Audio.volume * 1.5f);
	}

	private void StateGroundSmash()
	{
		if (StateVar[0] == 0)
		{
			if (StateVar[2] != 1 && Time.time > StateTime[3])
			{
				Audio.PlayOneShot(ArmDownSound, Audio.volume);
				StateVar[2] = 1;
			}
			if (StateVar[1] != 1 && Time.time > StateTime[2])
			{
				Object.Instantiate(ImpactPrefab, base.transform.position + Mesh.forward * 4.5f, Quaternion.identity);
				StateVar[1] = 1;
			}
			if (Time.time > StateTime[0])
			{
				Animation = 1;
				RevealHead(Reveal: false);
				StateVar[0] = 1;
			}
		}
		else if (Time.time > StateTime[1])
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateGroundSmashEnd()
	{
	}

	private void StateThrowHazardStart()
	{
		Animation = 6;
		StateTime[0] = Time.time;
		StateTime[1] = Time.time;
		StateVar[0] = 0;
		StateVar[1] = 0;
		StateVar[2] = 0;
	}

	private void StateThrowHazard()
	{
		if (!Target)
		{
			DestroyPossesion();
			StateMachine.ChangeState(StateStand);
		}
		if (StateVar[0] == 0)
		{
			if (StateVar[1] != 1 && Time.time - StateTime[1] > 0.75f)
			{
				Object.Instantiate(PossesionFX, PossesionPoint.position, Quaternion.identity);
				PossesionObj = Object.Instantiate(PossesionPrefab, PossesionPoint.position, PossesionPoint.rotation);
				StateVar[1] = 1;
			}
			HoldPossesion();
			if (Time.time - StateTime[0] > 1.75f)
			{
				Animation = 8;
				if ((bool)Target)
				{
					TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				}
				StateVar[0] = 1;
			}
		}
		else if (StateVar[0] == 1)
		{
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
			HoldPossesion();
			if (base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir)
			{
				if (!HazardMoveFront || (HazardMoveFront && GetDistance() < 10.5f))
				{
					Animation = 9;
					StateTime[0] = Time.time;
					StateTime[1] = Time.time;
					StateTime[2] = Time.time;
					StateVar[0] = 2;
				}
				else
				{
					StateTime[0] = Time.time;
					StateVar[0] = 3;
				}
			}
		}
		else if (StateVar[0] == 2)
		{
			if (StateVar[2] != 1 && Time.time - StateTime[1] > 1.15f)
			{
				PossesionObj.transform.GetComponent<Rigidbody>().AddForce((TargetPosition - PossesionPoint.position).normalized * ((EnemyType == Type.cGolem) ? 30f : 40f) + Mesh.up * -15f, ForceMode.Impulse);
				Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
				foreach (Collider collider in componentsInChildren)
				{
					Physics.IgnoreCollision(PossesionObj.GetComponentInChildren<Collider>(), collider);
				}
				PossesionObj.GetComponent<TimedBomb>().enabled = true;
				PossesionObj.GetComponent<TimedBomb>().Launched = true;
				PossesionObj = null;
				StateVar[2] = 1;
			}
			if (StateVar[1] != 2 && Time.time - StateTime[2] > 0.5f)
			{
				Audio.PlayOneShot(MoveSound, Audio.volume * 1.5f);
				StateVar[1] = 2;
			}
			else
			{
				HoldPossesion();
			}
			if (Time.time - StateTime[0] > 3f)
			{
				Animation = 1;
				StateTime[0] = Time.time;
				StateVar[0] = 4;
			}
		}
		else if (StateVar[0] == 3)
		{
			if ((bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
			}
			Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: false, 1f);
			HoldPossesion();
			if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
			{
				_Rigidbody.velocity = Vector3.ProjectOnPlane(Mesh.forward, hitInfo.normal) * 2.5f;
			}
			else
			{
				_Rigidbody.velocity = Mesh.forward * 2.5f;
			}
			if (GetDistance() < 10.5f || Time.time - StateTime[0] > 3f)
			{
				Animation = 9;
				StateTime[0] = Time.time;
				StateTime[1] = Time.time;
				StateTime[2] = Time.time;
				StateVar[0] = 2;
			}
			if (GetDistance() > 40f)
			{
				Target = null;
				DestroyPossesion();
				StateMachine.ChangeState(StateStand);
			}
		}
		else if (Time.time - StateTime[0] > 1.5f)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateThrowHazardEnd()
	{
	}

	private void StateFoundStart()
	{
		Animation = 4;
		StateEnd = Time.time + 1.6f;
		if (CreatureMode == Mode.Freeze)
		{
			RevealHead(Reveal: true);
		}
	}

	private void StateFound()
	{
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			if (CreatureMode == Mode.Freeze)
			{
				RevealHead(Reveal: false);
			}
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateFoundEnd()
	{
	}

	private void StateStunedStart()
	{
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
		Animation = 7;
		StateEnd = Time.time + 2.65f;
		Audio.PlayOneShot(WinceSound, Audio.volume);
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			RevealHead(Reveal: false);
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void RevealHead(bool Reveal)
	{
		IsOpenHead = Reveal;
		Animator.SetTrigger(IsOpenHead ? "Open" : "Close");
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 3f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
			DestroyPossesion();
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			DestroyPossesion();
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
			CurHealth -= HitInfo.damage * ((!IsOpenHead) ? 1 : 2);
			if (!ParalysisEffect && IsOpenHead && !IsPsychokinesis && !PsychoThrown)
			{
				StateMachine.ChangeState(StateDamageKnockBack);
				DestroyPossesion();
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
			DestroyPossesion();
			OnDestroy();
			OnImpact(HitInfo);
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void HoldPossesion()
	{
		if ((bool)PossesionObj)
		{
			PossesionObj.GetComponent<Rigidbody>().position = PossesionPoint.position;
			PossesionObj.GetComponent<Rigidbody>().rotation = PossesionPoint.rotation;
			PossesionObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	private void DestroyPossesion()
	{
		if ((bool)PossesionObj)
		{
			PossesionObj.GetComponent<TimedBomb>().AutoDestroy();
			PossesionObj = null;
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
