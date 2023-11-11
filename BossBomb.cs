using UnityEngine;

public class BossBomb : MonoBehaviour
{
	[Header("Prefab")]
	public Rigidbody RBody;

	public GameObject Mesh;

	public GameObject ExplosionFX;

	private bool Destroyed;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - StartTime > 5f)
		{
			Explode();
		}
	}

	private void Explode()
	{
		Object.Destroy(RBody);
		Mesh.SetActive(value: false);
		ExplosionFX.SetActive(value: true);
		Object.Destroy(base.gameObject, 2f);
		Destroyed = true;
	}

	private void ExplodeObj(Transform _Transform)
	{
		HitInfo value = new HitInfo(base.transform, RBody.velocity.normalized * 25f, 0);
		if (_Transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj") || _Transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || _Transform.gameObject.layer == LayerMask.NameToLayer("EnemyTrigger"))
		{
			_Transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer != LayerMask.NameToLayer("Default") && !Destroyed)
		{
			if (!collider.transform.GetComponent<ItemBox>())
			{
				ExplodeObj(collider.transform);
			}
			Explode();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Default") && !Destroyed)
		{
			if (!collision.collider.transform.GetComponent<ItemBox>())
			{
				ExplodeObj(collision.transform);
			}
			Explode();
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Default") && !Destroyed)
		{
			if (!collision.collider.transform.GetComponent<ItemBox>())
			{
				ExplodeObj(collision.transform);
			}
			Explode();
		}
	}
}
