using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
	[Header("Framework")]
	public Vector2 ExplosionDelay;

	public float AngularMultplier = 5f;

	public float MaxAngularSpeed = 15f;

	public Transform Center;

	public float ExplosionForce = 5f;

	public float ExplosionForceRadius = 5f;

	public Vector2 Duration;

	public GameObject SmashChain;

	public int BonusCount = 1;

	public GameObject BonusObject;

	public GameObject ExplosionFX;

	public bool Detach = true;

	public bool ExplodeOnCollision = true;

	[Header("Break Object")]
	public float Radius = 2f;

	public LayerMask LayerMask;

	internal Transform Player;

	private float RagdollTime;

	private float ExplodeTime;

	private bool Exploded;

	internal bool IsChainTarget;

	internal int ChainLevel;

	private HomingSmashChain Chain;

	private Rigidbody[] RigidBodies;

	private void Start()
	{
		ExplodeTime = Time.time + Random.Range(Duration.x, Duration.y);
		RagdollTime = 0f;
		RigidBodies = GetComponentsInChildren<Rigidbody>();
		Rigidbody[] rigidBodies = RigidBodies;
		for (int i = 0; i < rigidBodies.Length; i++)
		{
			rigidBodies[i].maxAngularVelocity = MaxAngularSpeed;
		}
		if (IsChainTarget)
		{
			Chain = Object.Instantiate(SmashChain, Center.position, Center.rotation).GetComponent<HomingSmashChain>();
			Chain.Player = Player;
			Chain.IsNuke = ChainLevel > 1;
			Chain.transform.SetParent(Center);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollision(collision.collider);
	}

	private void OnCollisionStay(Collision collision)
	{
		OnCollision(collision.collider);
	}

	private void OnCollision(Collider collider)
	{
		if (!Exploded && ExplodeOnCollision && !(collider.tag == "Ragdoll") && collider.gameObject.layer != LayerMask.NameToLayer("PlayerPushColl") && !Exploded && RagdollTime > Random.Range(ExplosionDelay.x, ExplosionDelay.y))
		{
			Explode();
		}
	}

	private void FixedUpdate()
	{
		if (ExplodeOnCollision && !Exploded)
		{
			RagdollTime += Time.fixedDeltaTime;
			if (AngularMultplier != 1f)
			{
				Rigidbody[] rigidBodies = RigidBodies;
				foreach (Rigidbody rigidbody in rigidBodies)
				{
					if (rigidbody.angularVelocity.magnitude < MaxAngularSpeed)
					{
						rigidbody.angularVelocity *= AngularMultplier;
					}
				}
			}
		}
		if (Time.time >= ExplodeTime && !Exploded)
		{
			Explode();
		}
	}

	private void Explode()
	{
		if ((ExplodeOnCollision || IsChainTarget) && !Exploded)
		{
			OnBreak(Center.position, new HitInfo(Player, GetComponent<Rigidbody>().velocity, 0));
			for (int i = 0; i < BonusCount; i++)
			{
				Object.Instantiate(BonusObject, Center.position, Random.rotation);
			}
			Object.Instantiate(ExplosionFX, Center.position, base.transform.rotation);
			if ((bool)Chain)
			{
				Chain.Detonate();
			}
			if (Detach)
			{
				DoDetach();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
			Exploded = true;
			base.enabled = false;
		}
	}

	private void DoDetach()
	{
		Joint[] componentsInChildren = GetComponentsInChildren<Joint>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
		Rigidbody[] rigidBodies = RigidBodies;
		foreach (Rigidbody rigidbody in rigidBodies)
		{
			rigidbody.useGravity = true;
			rigidbody.drag = 0.1f;
			Object.Destroy(rigidbody.transform.gameObject, 10f);
			if (!(rigidbody.transform == base.transform))
			{
				rigidbody.transform.parent = base.transform.parent;
				rigidbody.AddExplosionForce(ExplosionForce, Center.position, ExplosionForceRadius, 0f, ForceMode.VelocityChange);
			}
		}
	}

	public void OnBreak(Vector3 position, HitInfo HitInfo)
	{
		Collider[] array = Physics.OverlapSphere(position, Radius, LayerMask);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SendMessage("OnHit", HitInfo, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Center.position, Radius);
	}
}
