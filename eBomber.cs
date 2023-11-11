using UnityEngine;

public class eBomber : EnemyBase
{
	public enum Type
	{
		eBomber = 0,
		eSweeper = 1,
		eArmor = 2
	}

	public enum Mode
	{
		Allaround = 0,
		Boss = 1,
		Fix = 2,
		Normal = 3,
		Wall_Fix = 4,
		Wall_Normal = 5
	}

	internal enum State
	{
		TransferFall = 0,
		Stand = 1,
		Search = 2,
		Seek = 3,
		Found = 4,
		Roam = 5,
		Move = 6,
		Charge = 7,
		Dash = 8,
		Reload = 9,
		Launch = 10,
		Stuned = 11,
		DamageKnockBack = 12
	}

	[Header("Framework")]
	public Mode RobotMode;

	[Header("Prefab")]
	public Type EnemyType;

	public LayerMask FrontalColLayer;

	public Transform BombPoint;

	public GameObject BombPrefab;

	public Material[] CloakMats;

	[Header("Instantiation")]
	public GameObject ShieldEffectPrefab;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioClip[] eSeries;

	public AudioClip[] BombSounds;

	public AudioSource[] SpinAttackSources;

	private Vector3 DirectionPosition;

	private Material[] NormalMats;

	private Quaternion TargetRotation;

	private GameObject Bomb;

	private bool Reloaded;

	private bool Launched;

	private bool HasUncloaked;

	private float CloakTimer;

	private float[] StateTime = new float[10];

	private float StateEnd;

	private int[] StateVar = new int[10];

	private int FoundState;

	private float TurnDirection;

	private RaycastHit GroundRay;

	private bool DashGrounded;

	private void Start()
	{
		if ((RobotMode == Mode.Allaround || RobotMode == Mode.Normal || RobotMode == Mode.Fix || RobotMode == Mode.Wall_Fix || RobotMode == Mode.Wall_Normal) && Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 2f))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
		}
		MaxHealth = EnemyMaxHealth();
		CurHealth = MaxHealth;
		StartPosition = base.transform.position;
		StartRotation = base.transform.rotation;
		Position = StartPosition + Vector3.up * DescentOffset;
		if (EnemyType == Type.eSweeper)
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
			if (RobotMode != Mode.Wall_Fix && RobotMode != Mode.Wall_Normal)
			{
				_Rigidbody.velocity = Physics.gravity * 0.5f;
				_Rigidbody.useGravity = true;
				_Rigidbody.isKinematic = false;
				Grounded = false;
				base.transform.position = Position;
				base.transform.rotation = StartRotation;
				DirectionPosition = (StartPosition - base.transform.position).normalized;
				StateTransferFallStart();
				StateMachine.Initialize(StateTransferFall);
			}
			else
			{
				_Rigidbody.velocity = Vector3.zero;
				_Rigidbody.useGravity = false;
				_Rigidbody.isKinematic = true;
				StateStandStart();
				StateMachine.Initialize(StateStand);
			}
			ToggleCloak(Toggle: true);
			if ((bool)TeleportEffectPrefab)
			{
				Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 0.75f, StartRotation);
			}
			Blocking = false;
		}
	}

	private void Reset()
	{
		Target = null;
		if ((bool)TeleportEffectPrefab)
		{
			Object.Instantiate(TeleportEffectPrefab, Mesh.position + Mesh.up * 0.75f, StartRotation);
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
		if (RobotMode != Mode.Wall_Fix && RobotMode != Mode.Wall_Normal)
		{
			_Rigidbody.velocity = Vector3.zero;
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
		}
		else
		{
			_Rigidbody.useGravity = true;
			_Rigidbody.isKinematic = false;
		}
		if (EnemyType == Type.eArmor)
		{
			_Rigidbody.constraints = RigidbodyConstraints.None;
		}
		if (EnemyType != Type.eArmor && Reloaded)
		{
			DestroyBomb();
		}
		base.gameObject.SetActive(value: false);
		Blocking = false;
	}

	private int EnemyMaxHealth()
	{
		switch (EnemyType)
		{
		case Type.eSweeper:
			return 1;
		case Type.eArmor:
			return 4;
		default:
			return 0;
		}
	}

	public override void Update()
	{
		base.Update();
		UpdateGauge(1.8f);
		if (EnemyType == Type.eSweeper && HasUncloaked && Time.time - CloakTimer > 3f)
		{
			HasUncloaked = false;
			ToggleCloak(Toggle: true);
		}
	}

	private void StateTransferFallStart()
	{
		Animation = 9;
	}

	private void StateTransferFall()
	{
		RaycastHit hitInfo;
		if (Grounded)
		{
			_Rigidbody.velocity = Vector3.zero;
			if (RobotMode != Mode.Wall_Fix && RobotMode != Mode.Wall_Normal)
			{
				_Rigidbody.useGravity = false;
				_Rigidbody.isKinematic = true;
			}
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

	private void StateStandStart()
	{
		Animation = 1;
		StateTime[0] = Time.time + 4f;
	}

	private void StateStand()
	{
		if (!(Time.time > StateTime[0]))
		{
			return;
		}
		if (GetTarget(-Mesh.forward, 200f))
		{
			if (FoundState == 0)
			{
				FoundState = 1;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
				StateMachine.ChangeState(StateFound);
			}
			else
			{
				StateMachine.ChangeState(StateSeek);
			}
		}
		else
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
		StateTime[0] = Time.time + 1.9835f;
		StateVar[0] = 0;
		TurnDirection = 0f;
		TargetRotation = Mesh.localRotation * Quaternion.LookRotation(Vector3.back);
	}

	private void StateSearch()
	{
		if (GetTarget(-Mesh.forward))
		{
			if (FoundState == 0)
			{
				FoundState = 1;
				Audio.PlayOneShot(eSeries[1], Audio.volume);
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
			TargetRotation *= Quaternion.LookRotation(Vector3.back);
			StateVar[0] = 1;
			if (TurnDirection == 0f)
			{
				TurnDirection = SignedDirection(base.transform.InverseTransformDirection(Mesh.forward), ToTargetDir);
			}
			if (TurnDirection < 0f)
			{
				Animation = 3;
			}
			else if (TurnDirection > 0f)
			{
				Animation = 4;
			}
		}
		Mesh.localRotation = Quaternion.RotateTowards(Mesh.localRotation, TargetRotation, 1f);
		if (Quaternion.Dot(Mesh.localRotation, TargetRotation) == 1f)
		{
			Animation = 2;
			StateTime[0] = Time.time + 1.9665f;
			StateVar[0] = 0;
			if ((RobotMode == Mode.Allaround || RobotMode == Mode.Normal || RobotMode == Mode.Wall_Normal) && Random.value > 0.5f)
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
		else
		{
			StateMachine.ChangeState(StateStand);
		}
		TurnDirection = 0f;
	}

	private void StateSeek()
	{
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: true, 1f);
		if (TurnDirection == 0f)
		{
			TurnDirection = SignedDirection(base.transform.InverseTransformDirection(Mesh.forward), ToTargetDir);
		}
		if (TurnDirection < 0f)
		{
			Animation = 3;
		}
		else if (TurnDirection > 0f)
		{
			Animation = 4;
		}
		if (!(base.transform.InverseTransformDirection(Mesh.forward) == ToTargetDir))
		{
			return;
		}
		if (EnemyType != Type.eArmor)
		{
			if ((RobotMode == Mode.Allaround || RobotMode == Mode.Normal || RobotMode == Mode.Wall_Normal) && GetDistance() > 10f)
			{
				StateMachine.ChangeState(StateMove);
			}
			else
			{
				StateMachine.ChangeState(StateReload);
			}
		}
		else
		{
			StateMachine.ChangeState(StateCharge);
		}
	}

	private void StateSeekEnd()
	{
	}

	private void StateFoundStart()
	{
		Animation = 11;
		StateEnd = Time.time + 1f;
		StateTime[0] = Time.time + 1f;
	}

	private void StateFound()
	{
		if (Time.time > StateTime[0])
		{
			StateMachine.ChangeState(StateSeek);
		}
	}

	private void StateFoundEnd()
	{
	}

	private void StateRoamStart()
	{
		Animation = 10;
		StateEnd = Time.time + 2.5f;
	}

	private void StateRoam()
	{
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = false;
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
		}
		_Rigidbody.velocity = -Mesh.forward * 1.5f;
		if (Time.time > StateEnd)
		{
			StateEnd = 0f;
			_Rigidbody.velocity = Vector3.zero;
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateSearch);
		}
	}

	private void StateRoamEnd()
	{
		_Rigidbody.velocity = Vector3.zero;
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = true;
	}

	private void StateMoveStart()
	{
		Animation = 10;
	}

	private void StateMove()
	{
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = false;
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -Vector3.up, out var hitInfo, 1f))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
		}
		else
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		}
		TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
		Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: true, 2f);
		_Rigidbody.velocity = -Mesh.forward * 2.5f;
		if (GetDistance() < 10f)
		{
			_Rigidbody.velocity = Vector3.zero;
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateReload);
		}
		if (GetDistance() > 40f || !Target)
		{
			Target = null;
			_Rigidbody.velocity = Vector3.zero;
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateMoveEnd()
	{
		_Rigidbody.velocity = Vector3.zero;
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = true;
	}

	private void StateChargeStart()
	{
		StateEnd = Time.time + 1f;
		Animation = 0;
		Animator.SetTrigger("Stun");
		Audio.PlayOneShot(eSeries[0], Audio.volume);
	}

	private void StateCharge()
	{
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateDash);
		}
	}

	private void StateChargeEnd()
	{
	}

	private void StateDashStart()
	{
		StateEnd = Time.time + 1.5f;
		Animation = 8;
		_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		SpinAttackSources[1].volume = 0f;
		SpinAttackSources[0].Play();
		SpinAttackSources[1].Play();
		Blocking = true;
	}

	private void StateDash()
	{
		_Rigidbody.useGravity = true;
		_Rigidbody.isKinematic = false;
		DashGrounded = Physics.Raycast(base.transform.position + base.transform.up * 0.75f, -base.transform.up, out GroundRay, 1f, FrontalColLayer);
		base.transform.rotation = Quaternion.FromToRotation(base.transform.up, DashGrounded ? GroundRay.normal : Vector3.up) * base.transform.rotation;
		if (!SpinAttackSources[0].isPlaying)
		{
			SpinAttackSources[1].volume = 1f;
		}
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.75f, -Mesh.forward, out GroundRay, 1f, FrontalColLayer))
		{
			if (DashGrounded)
			{
				OnFlash();
			}
		}
		else if (DashGrounded)
		{
			_Rigidbody.AddForce(-Mesh.forward * 25f, ForceMode.Acceleration);
			if ((bool)Target)
			{
				TargetPosition = Target.transform.position + Target.transform.up * 0.25f;
				Mesh.localRotation = FaceTarget(TargetPosition, Mesh.localRotation, Inverse: true, 2f);
			}
		}
		if (Time.time > StateEnd && DashGrounded)
		{
			Blocking = false;
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDashEnd()
	{
		_Rigidbody.velocity = Vector3.zero;
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = true;
		_Rigidbody.constraints = RigidbodyConstraints.None;
		SpinAttackSources[0].Stop();
		SpinAttackSources[1].Stop();
		Blocking = false;
	}

	private void StateReloadStart()
	{
		StateTime[0] = Time.time + 1f;
		StateEnd = Time.time + 2.633f;
		Animation = 5;
	}

	private void StateReload()
	{
		if (Time.time > StateTime[0] && !Reloaded)
		{
			Bomb = Object.Instantiate(BombPrefab, BombPoint.position, BombPoint.rotation);
			Audio.PlayOneShot(BombSounds[1], Audio.volume);
			Reloaded = true;
		}
		HoldBomb();
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateLaunch);
		}
	}

	private void StateReloadEnd()
	{
	}

	private void StateLaunchStart()
	{
		Launched = false;
		StateTime[0] = Time.time + 1.4f;
		StateEnd = Time.time + 2.967f;
		Animation = 6;
	}

	private void StateLaunch()
	{
		if (Time.time > StateTime[0] && !Launched)
		{
			Audio.PlayOneShot(BombSounds[0], Audio.volume);
			Vector3 vector = BallisticVel(BombPoint.position, TargetPosition);
			Bomb.transform.GetComponent<Rigidbody>().AddForce(vector, ForceMode.Impulse);
			Bomb.transform.GetComponent<Rigidbody>().AddTorque(vector, ForceMode.Impulse);
			Bomb.GetComponent<TimedBomb>().enabled = true;
			Bomb.GetComponent<TimedBomb>().Launched = true;
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(Bomb.GetComponentInChildren<Collider>(), collider);
			}
			Launched = true;
			Reloaded = false;
			Bomb = null;
		}
		else
		{
			HoldBomb();
		}
		if (Time.time > StateEnd)
		{
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateLaunchEnd()
	{
	}

	private void StateStunedStart()
	{
		StateEnd = Time.time + 7.5f;
		Animation = 9;
		Audio.PlayOneShot(eSeries[0], Audio.volume);
		ToggleCloak(Toggle: false);
	}

	private void StateStuned()
	{
		_Rigidbody.useGravity = true;
		_Rigidbody.isKinematic = false;
		if (Time.time > StateEnd)
		{
			if (EnemyType == Type.eSweeper)
			{
				HasUncloaked = true;
				CloakTimer = Time.time;
			}
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateStunedEnd()
	{
		Stuned = false;
		Object.Destroy(ParalysisEffect);
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.75f, -Vector3.up, out var hitInfo, 1f, FrontalColLayer))
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, hitInfo.normal) * base.transform.rotation;
			base.transform.position = hitInfo.point;
		}
		else
		{
			base.transform.rotation = Quaternion.FromToRotation(base.transform.up, Vector3.up) * base.transform.rotation;
		}
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = true;
	}

	private void StateDamageKnockBackStart()
	{
		StateEnd = Time.time + 1.5f;
		Animation = 7;
		ToggleCloak(Toggle: false);
	}

	private void StateDamageKnockBack()
	{
		if (Time.time > StateEnd)
		{
			if (EnemyType == Type.eSweeper)
			{
				HasUncloaked = true;
				CloakTimer = Time.time;
			}
			StateMachine.ChangeState(StateStand);
		}
	}

	private void StateDamageKnockBackEnd()
	{
	}

	private void ToggleCloak(bool Toggle)
	{
		if (EnemyType == Type.eSweeper)
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
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position + Vector3.up * 1f, Quaternion.identity);
			ParalysisEffect.transform.SetParent(base.transform);
			if (EnemyType != Type.eArmor && Reloaded)
			{
				DestroyBomb();
			}
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			if (EnemyType != Type.eArmor && Reloaded)
			{
				DestroyBomb();
			}
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
		if (Blocking)
		{
			CreateShieldFX(HitInfo, ShieldEffectPrefab);
			HitTimer = 0f;
			if (((bool)HitInfo.player.GetComponent<SonicNew>() && (HitInfo.player.GetComponent<SonicNew>().UsingWhiteGem || HitInfo.player.GetComponent<SonicNew>().IsSuper)) || ((bool)HitInfo.player.GetComponent<Shadow>() && HitInfo.player.GetComponent<PlayerBase>().GetState() != "ChaosBlast" && HitInfo.player.GetComponent<Shadow>().IsFullPower))
			{
				OnFlash();
			}
			return;
		}
		if (EnemyType != Type.eArmor && Reloaded)
		{
			DestroyBomb();
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

	private void HoldBomb()
	{
		if ((bool)Bomb)
		{
			Bomb.GetComponent<Rigidbody>().position = BombPoint.position;
			Bomb.GetComponent<Rigidbody>().rotation = BombPoint.rotation;
			Bomb.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	private void DestroyBomb()
	{
		if ((bool)Bomb)
		{
			Bomb.GetComponent<TimedBomb>().AutoDestroy();
			Bomb = null;
			Reloaded = false;
		}
	}

	private Vector3 BallisticVel(Vector3 Source, Vector3 Target)
	{
		Vector3 vector = Target - Source;
		float y = vector.y;
		float num = (vector.y = vector.magnitude) + y;
		return Mathf.Sqrt(num * Physics.gravity.magnitude) * vector.normalized;
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
