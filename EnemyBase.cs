using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STHEngine;
using UnityEngine;

public class EnemyBase : PsiObject
{
	[Header("Extra Framework")]
	public bool FindPlayer;

	public bool IsFixed;

	public string AppearPath;

	public float AppearVelocity;

	public string ActionPath;

	public Vector3 HomingTarget;

	[Header("Enemy Base")]
	public int Score;

	public GameObject[] CommandsTo;

	public StateMachine StateMachine;

	public Rigidbody _Rigidbody;

	public Renderer Renderer;

	public Animator Animator;

	public Transform Mesh;

	public Transform[] HomingTargets;

	public GameObject HitEffectPrefab;

	public GameObject TeleportEffectPrefab;

	public GameObject ParalysisEffectPrefab;

	public GameObject RagdollPrefab;

	internal GaugeController GaugeController;

	internal GameObject Target;

	internal GameObject ParalysisEffect;

	internal Vector3 Position;

	internal Vector3 TargetPosition;

	internal Vector3 StartPosition;

	internal Vector3 ToTargetDir;

	internal Quaternion StartRotation;

	internal bool Grounded;

	internal bool Destroyed;

	internal bool Stuned;

	internal bool Blocking;

	internal float DescentOffset = 0.0105f;

	internal float HitTimer = 5f;

	internal float GaugeScale;

	internal int Animation;

	internal int MaxHealth;

	internal int CurHealth;

	private float HitFXTimer;

	private float GroundDot = 0.75f;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	internal bool PsychoThrown;

	internal bool ChainTarget;

	internal int ChainLevel;

	private LayerMask EventBox_Mask => LayerMask.GetMask("Default");

	internal LayerMask Collision_Mask => LayerMask.GetMask("PlayerCollision");

	public void SetParameters(bool _FindPlayer, bool _IsFixed, string _AppearPath, float _AppearVelocity, string _ActionPath, Vector3 _HomingTarget)
	{
		FindPlayer = _FindPlayer;
		IsFixed = _IsFixed;
		AppearPath = _AppearPath;
		AppearVelocity = _AppearVelocity;
		ActionPath = _ActionPath;
		HomingTarget = _HomingTarget;
	}

	public void BaseStart()
	{
		if (MaxHealth > 0 && Singleton<Settings>.Instance.settings.EnemyHealthType == 0)
		{
			GaugeController = CreateGauge(Resources.Load("DefaultPrefabs/UI/EnemyGauge") as GameObject);
		}
		if (FindPlayer)
		{
			Target = GameObject.FindGameObjectWithTag("Player");
		}
	}

	public GaugeController CreateGauge(GameObject Gauge)
	{
		if (Gauge != null)
		{
			return Object.Instantiate(Gauge, base.transform.position, base.transform.rotation, base.transform).GetComponent<GaugeController>();
		}
		return null;
	}

	public virtual void FixedUpdate()
	{
		StateMachine.UpdateStateMachine();
		if (!PsychoThrown)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.5f);
		for (int i = 0; i < array.Length; i++)
		{
			EnemyBase component = array[i].GetComponent<EnemyBase>();
			if (array[i].gameObject.layer == LayerMask.NameToLayer("Enemy") && array[i].gameObject != base.gameObject && (!component || ((bool)component && !component.IsPsychokinesis && !component.PsychoThrown)))
			{
				array[i].SendMessage("OnHit", new HitInfo(PlayerTransform, _Rigidbody.velocity, 10), SendMessageOptions.DontRequireReceiver);
				AutoDestroy();
			}
		}
	}

	public virtual void Update()
	{
		HitTimer += Time.deltaTime;
		Animator.SetInteger("Animation", Animation);
		OnPsiFX(Renderer, IsPsychokinesis);
		HitFXTimer -= Time.deltaTime;
	}

	public float GetDistance()
	{
		return Vector3.Distance(base.transform.position, TargetPosition);
	}

	public int GetCurHealth()
	{
		return CurHealth;
	}

	public int GetMaxHealth()
	{
		return MaxHealth;
	}

	public float GetHitTimer(HitInfo HitInfo)
	{
		float result = 0.15f;
		if (HitInfo.player.tag == "Player" && (HitInfo.player.GetComponent<PlayerBase>().GetState() == "JumpDash" || HitInfo.player.GetComponent<PlayerBase>().GetState() == "Homing" || HitInfo.player.GetComponent<PlayerBase>().GetState() == "ChaosAttack"))
		{
			result = 0.01f;
		}
		return result;
	}

	public int RandomInt(int First, int Second)
	{
		return Random.Range(First, Second + 1);
	}

	public void SetChainProperties(int Context)
	{
		ChainTarget = Context != 5;
		ChainLevel = Context;
	}

	public void OnImpact(HitInfo HitInfo, bool Explosion = false)
	{
		EnemyRagdoll component = Object.Instantiate(RagdollPrefab, base.transform.position, Mesh.rotation).GetComponent<EnemyRagdoll>();
		if (HitInfo.player.tag == "Player")
		{
			if (!HitInfo.player.GetComponent<PlayerBase>().GetPrefab("silver"))
			{
				DestroyHomingTargets();
			}
			HitInfo.player.GetComponent<PlayerBase>().AddScore(Score);
			component.Player = HitInfo.player;
		}
		else
		{
			component.Player = component.transform;
		}
		if (ChainTarget)
		{
			component.IsChainTarget = ChainTarget;
			component.ChainLevel = ChainLevel;
			if (ChainLevel > 0 && HomingTarget == Vector3.zero)
			{
				GameObject gameObject = FindClosestChainTarget(HitInfo.player);
				if (gameObject != null)
				{
					HomingTarget = gameObject.transform.position;
				}
				component.ExplodeOnCollision = false;
			}
		}
		DestroySlaves(HitInfo);
		SynchRagdoll(component.transform);
		if (Explosion)
		{
			Rigidbody[] componentsInChildren = component.transform.GetComponentsInChildren<Rigidbody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].AddExplosionForce(50f, HitInfo.force, 10f, 2f, ForceMode.VelocityChange);
			}
		}
		else
		{
			RagdollDirection(component.transform, HitInfo);
		}
	}

	public Vector3 SetMoveVel(Vector3 Movement)
	{
		return new Vector3(Movement.x, _Rigidbody.velocity.y, Movement.z);
	}

	public void ApplyVelToRigidbodies(Transform ThisRagdoll, Vector3 Velocity, Vector3 Position)
	{
		Rigidbody[] componentsInChildren = ThisRagdoll.GetComponentsInChildren<Rigidbody>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].AddForceAtPosition(Velocity, Position, ForceMode.VelocityChange);
		}
	}

	public void SynchRagdoll(Transform ThisRagdoll)
	{
		Transform[] componentsInChildren = ThisRagdoll.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			string inName = transform.name;
			Transform transform2 = base.transform.FindInChildren(inName, Recursive: true);
			if ((bool)transform2)
			{
				transform.position = transform2.position;
				transform.rotation = transform2.rotation;
			}
		}
	}

	public Vector3 RagdollLaunchDirection(Vector3 Force, float Min = 1f, float Max = 2.5f)
	{
		_ = Vector3.zero;
		return ((Vector3.Dot(Vector3.up, Force.normalized) >= 0f) ? Force : (Vector3.Reflect(Force.normalized, Vector3.up).normalized * Force.magnitude)) * Random.Range(Min, Max);
	}

	public void RagdollDirection(Transform RagdollTransform, HitInfo HitInfo)
	{
		if (HomingTarget != Vector3.zero)
		{
			Vector3 vector = HomingTarget - RagdollTransform.position;
			ApplyVelToRigidbodies(RagdollTransform, vector.normalized * vector.magnitude * 5f, HitInfo.player.position);
		}
		else
		{
			ApplyVelToRigidbodies(RagdollTransform, RagdollLaunchDirection(HitInfo.force), HitInfo.player.position);
		}
	}

	internal bool GetTarget(Vector3 Forward, float Range = 360f, bool Radial = false)
	{
		if (!Target)
		{
			Target = FindClosestTarget(Forward, Range, Radial);
		}
		return Target != null;
	}

	internal bool GetTargetBox(Vector3 Position, Vector3 Scale)
	{
		if (!Target)
		{
			Target = FindTargetBox(Position, Scale);
		}
		return Target != null;
	}

	private GameObject FindClosestTarget(Vector3 Direction, float ThisDistance, bool IsRadial = false)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject result = null;
		float num = ThisDistance;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Vector3 from = gameObject.transform.position - position;
			float sqrMagnitude = from.sqrMagnitude;
			if (sqrMagnitude < num && gameObject.GetComponent<PlayerBase>().IsVisible)
			{
				float num2 = Vector3.Angle(from, Direction);
				if ((!IsRadial && num2 < 90f) || IsRadial)
				{
					result = gameObject;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	private GameObject FindTargetBox(Vector3 Position, Vector3 Scale)
	{
		Collider[] array = Physics.OverlapBox(Position, Scale / 2f, base.transform.rotation);
		GameObject result = null;
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.tag == "Player")
				{
					result = array[i].gameObject;
				}
			}
		}
		return result;
	}

	private GameObject FindClosestChainTarget(Transform Point)
	{
		List<GameObject> list = GameObject.FindGameObjectsWithTag("HomingTarget").ToList();
		foreach (Transform item in HomingTargets.ToList())
		{
			if (list.Contains(item.gameObject) || item.gameObject.layer != LayerMask.NameToLayer("Enemy"))
			{
				list.Remove(item.gameObject);
			}
		}
		GameObject result = null;
		float num = ExtensionMethods.C_LockOn_Homing.Far.z * 2f;
		foreach (GameObject item2 in list)
		{
			float num2 = Vector3.Distance(Point.position, item2.transform.position);
			if (num2 < num && ExtensionMethods.C_LockOn_Homing.Inside(Point, item2.transform.position) && !Physics.Linecast(base.transform.position, item2.transform.position, ExtensionMethods.HomingBlock_Mask))
			{
				result = item2;
				num = num2;
			}
		}
		return result;
	}

	internal float SignedDirection(Vector3 A, Vector3 B)
	{
		return Mathf.Acos(Vector3.Dot(A.normalized, B.normalized)) * Mathf.Sign(Vector3.Cross(A, B).y);
	}

	internal void UpdateGauge(float Offset, float Scale = 1.25f)
	{
		if ((bool)GaugeController && Singleton<Settings>.Instance.settings.EnemyHealthType == 0)
		{
			GaugeScale = Mathf.Lerp(GaugeScale, (HitTimer > 3f) ? 0f : Scale, Time.deltaTime * ((HitTimer > 3f) ? 5f : 10f));
			GaugeController.transform.localScale = GaugeScale * Vector3.one;
			if (MaxHealth > 0)
			{
				GaugeController.transform.position = base.transform.position + base.transform.up * Offset;
				GaugeController.FillAmount = ((float)CurHealth + 1f) * 1f / ((float)MaxHealth + 1f);
			}
		}
	}

	internal Quaternion FaceTarget(Vector3 TargetPos, Quaternion InRotation, bool Inverse = false, float Speed = 0.75f)
	{
		Vector3 vector = (TargetPos - base.transform.position).normalized;
		if (Inverse)
		{
			vector = -vector;
		}
		Vector3 vector2 = base.transform.InverseTransformDirection(vector);
		vector2.y = 0f;
		ToTargetDir = vector2.normalized;
		return Quaternion.RotateTowards(InRotation, Quaternion.LookRotation(ToTargetDir), Speed);
	}

	internal Quaternion SmoothFaceTarget(Vector3 TargetPos, Quaternion InRotation, bool Inverse = false, float Speed = 0.75f, bool Smooth = false)
	{
		Vector3 vector = (TargetPos - base.transform.position).normalized;
		if (Inverse)
		{
			vector = -vector;
		}
		Vector3 vector2 = base.transform.InverseTransformDirection(vector);
		vector2.y = 0f;
		ToTargetDir = vector2.normalized;
		if (Smooth)
		{
			return Quaternion.Slerp(InRotation, Quaternion.LookRotation(ToTargetDir), Speed * Time.fixedDeltaTime);
		}
		return Quaternion.RotateTowards(InRotation, Quaternion.LookRotation(ToTargetDir), Speed);
	}

	public void CreateHitFX(HitInfo HitInfo, bool IgnoreTimer = false)
	{
		if (!(HitFXTimer > 0f) || IgnoreTimer)
		{
			Vector3 position = _Rigidbody.ClosestPointOnBounds((HitInfo.player != null) ? HitInfo.player.position : base.transform.position);
			Object.Instantiate(HitEffectPrefab, position, Quaternion.identity);
			HitFXTimer = 0.125f;
		}
	}

	public void CreateShieldFX(HitInfo HitInfo, GameObject Effect)
	{
		if ((bool)Effect)
		{
			Vector3 position = _Rigidbody.ClosestPointOnBounds(HitInfo.player.position);
			Object.Instantiate(Effect, position, Quaternion.LookRotation(HitInfo.player.position - base.transform.position));
		}
	}

	public void OnDestroy()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		Collider[] array;
		if (component != null)
		{
			array = Physics.OverlapBox(base.transform.position + component.center, component.size * 0.5f, base.transform.rotation, EventBox_Mask.value, QueryTriggerInteraction.Collide);
		}
		else
		{
			SphereCollider component2 = GetComponent<SphereCollider>();
			if (component2 != null)
			{
				array = Physics.OverlapSphere(base.transform.position + component2.center, component2.radius, EventBox_Mask.value, QueryTriggerInteraction.Collide);
			}
			else
			{
				CapsuleCollider component3 = GetComponent<CapsuleCollider>();
				array = Physics.OverlapBox(base.transform.position + component3.center, new Vector3(component3.radius, component3.height * 0.5f, component3.radius), base.transform.rotation, EventBox_Mask.value, QueryTriggerInteraction.Collide);
			}
		}
		Collider[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].transform.SendMessage("OnEnemyDestroy", base.transform, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void DestroySlaves(HitInfo HitInfo)
	{
		if (CommandsTo == null)
		{
			return;
		}
		for (int i = 0; i < CommandsTo.Length; i++)
		{
			if ((bool)CommandsTo[i])
			{
				CommandsTo[i].SendMessage("OnCommandDestroy", HitInfo, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void OnCommandDestroy(HitInfo HitInfo)
	{
		DestroyHomingTargets();
		StartCoroutine(DestroyTimed(HitInfo, Random.Range(0.05f, 0.5f)));
	}

	private IEnumerator DestroyTimed(HitInfo HitInfo, float Timer)
	{
		yield return new WaitForSeconds(Timer);
		base.gameObject.SendMessage("OnExplosion", HitInfo, SendMessageOptions.DontRequireReceiver);
	}

	public void TrackGroundedBox(Collision collision)
	{
		if (collision == null)
		{
			return;
		}
		ContactPoint[] contacts = collision.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (Vector3.Dot(contactPoint.normal, Vector3.up) > GroundDot)
			{
				Grounded = true;
				break;
			}
		}
	}

	private void DestroyHomingTargets()
	{
		for (int i = 0; i < HomingTargets.Length; i++)
		{
			if ((bool)HomingTargets[i])
			{
				Object.Destroy(HomingTargets[i].gameObject);
			}
		}
	}

	private void StateCaughtStart()
	{
		Animation = 20;
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = false;
		Object.Destroy(ParalysisEffect);
	}

	private void StateCaught()
	{
		if (IsPsychokinesis)
		{
			if (!PlayerTransform)
			{
				Target = GameObject.FindGameObjectWithTag("Player");
				OnReleasePsycho();
			}
			else
			{
				_Rigidbody.velocity = (PlayerPos.position + PlayerPos.forward * -6.5f - base.transform.position) * 12f;
				base.transform.GetChild(0).rotation = Quaternion.Lerp(base.transform.GetChild(0).rotation, PlayerPos.rotation, Time.fixedDeltaTime * 5f);
			}
		}
	}

	private void StateCaughtEnd()
	{
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (PsychoThrown)
		{
			AutoDestroy();
		}
	}

	private void AutoDestroy()
	{
		base.gameObject.SendMessage("OnHit", new HitInfo(Target.transform, _Rigidbody.velocity, 10), SendMessageOptions.DontRequireReceiver);
	}

	private void OnPsychoThrow(HitInfo HitInfo)
	{
		IsPsychokinesis = false;
		PsychoThrown = true;
		Invoke("AutoDestroy", 3f);
		_Rigidbody.useGravity = true;
		_Rigidbody.AddForce(HitInfo.force, ForceMode.VelocityChange);
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		if (Stuned)
		{
			if (!IsPsychokinesis)
			{
				IsPsychokinesis = true;
				base.transform.SetParent(null);
				StateMachine.ChangeState(StateCaught);
			}
			PlayerPos = _PlayerPos;
			Target = _PlayerPos.root.gameObject;
			PlayerTransform = _PlayerPos.root.transform;
			DestroyHomingTargets();
		}
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
		base.gameObject.SendMessage("OnHit", new HitInfo(Target.transform, Vector3.zero, 10), SendMessageOptions.DontRequireReceiver);
		PlayerPos = null;
	}

	private void OnPsychoShock(Transform _PlayerPos)
	{
		Target = _PlayerPos.gameObject;
	}

	private void OnDrawGizmosSelected()
	{
		if (HomingTarget != Vector3.zero)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(base.transform.position, HomingTarget);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(HomingTarget, 1f);
		}
	}
}
