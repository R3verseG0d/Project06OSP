using UnityEngine;

public class Bomb : ObjectBase
{
	public enum Type
	{
		Normal = 0,
		Heart = 1
	}

	[Header("Framework")]
	public Type BombType;

	public Collider[] Colliders;

	public GameObject ParticlePrefab;

	public AudioSource HeartBombTimer;

	public Collider BombSolidCollider;

	public bool ExplodeOnCollision;

	[Header("Optional")]
	public TrailRenderer Trail;

	internal Transform Player;

	internal GameObject ClosestTarget;

	internal GameObject AttachedObj;

	private bool Destroyed;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
		Invoke("DestroyBomb", (BombType == Type.Heart) ? 1.5f : 3f);
		if (BombType == Type.Heart)
		{
			HeartBombTimer.Play();
		}
	}

	private void Update()
	{
		for (int i = 0; i < Colliders.Length; i++)
		{
			if (Time.time - StartTime > 0.03f && !Colliders[i].enabled)
			{
				Colliders[i].enabled = true;
			}
		}
		if (BombType != Type.Heart && (bool)ClosestTarget && ClosestTarget.layer == LayerMask.NameToLayer("Enemy"))
		{
			base.transform.forward = ClosestTarget.transform.position - base.transform.position;
			GetComponent<Rigidbody>().velocity = base.transform.forward * 20f;
		}
		if (BombType == Type.Heart && !AttachedObj)
		{
			DestroyBomb();
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!Destroyed && !player && (ExplodeOnCollision || collider.gameObject.layer == LayerMask.NameToLayer("EnemyTrigger") || collider.gameObject.layer == LayerMask.NameToLayer("BreakableObj")) && BombType != Type.Heart && (BombType == Type.Heart || !ExplodeOnCollision || collider.gameObject.layer != LayerMask.NameToLayer("PlayerOnly")))
		{
			if (BombType != Type.Heart)
			{
				collider.gameObject.SendMessage("OnHit", new HitInfo(Player, Vector3.zero), SendMessageOptions.DontRequireReceiver);
			}
			Destroyed = true;
			DestroyBomb();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (BombType != Type.Heart && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), BombSolidCollider);
		}
	}

	private void DestroyBomb()
	{
		Object.Instantiate(ParticlePrefab, base.transform.position, Quaternion.identity);
		if (BombType == Type.Heart)
		{
			AttackSphere_Dir(base.transform.position, 4f, 10f, 1);
		}
		else
		{
			AttackSphere_Dir(base.transform.position, 1.25f, 5f, 1);
		}
		if ((bool)Trail)
		{
			Trail.transform.SetParent(null);
			Trail.emitting = false;
			Trail.autodestruct = true;
		}
		Object.Destroy(base.gameObject);
	}

	private bool AttackSphere_Dir(Vector3 Position, float Radius, float Force, int Damage)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
				if (vector == Vector3.zero)
				{
					vector = base.transform.forward.MakePlanar();
				}
				Vector3 force = (vector + Vector3.up * Random.Range(0.1f, 0.25f)).normalized * Force;
				HitInfo value = new HitInfo(Player, force, Damage);
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}
}
