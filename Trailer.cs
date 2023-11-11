using UnityEngine;

public class Trailer : MonoBehaviour
{
	[Header("Prefab")]
	public Rigidbody _Rigidbody;

	public Collider Collider;

	public Animator Animator;

	public GameObject FX;

	public Transform ContainerPoint;

	public GameObject Container;

	public GameObject ContainerExplosion;

	public GameObject[] Pipes;

	public GameObject[] FireFX;

	internal PlayerBase Player;

	internal TornadoChase Tornado;

	private float CrashTime;

	private bool HasCrashed;

	private bool ExplodeCargo;

	private void Start()
	{
		Animator.speed = 0f;
	}

	private void Update()
	{
		if (HasCrashed && Player.GetState() == "Result" && !ExplodeCargo)
		{
			Invoke("Explode", 0.75f);
			ExplodeCargo = true;
		}
	}

	private void FixedUpdate()
	{
		if (!HasCrashed)
		{
			Vector3 velocity = _Rigidbody.velocity;
			if (velocity.y > 0f)
			{
				velocity.y = 0f;
				_Rigidbody.velocity = velocity;
			}
		}
	}

	private void Explode()
	{
		ContainerExplosion.SetActive(value: true);
		ContainerExplosion.transform.rotation = Quaternion.identity;
		Container.SetActive(value: false);
		for (int i = 0; i < Pipes.Length; i++)
		{
			Pipes[i].SetActive(value: true);
			Pipes[i].transform.SetParent(null);
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				Physics.IgnoreCollision(componentsInChildren[j], Pipes[i].GetComponent<Collider>());
			}
			Vector3 velocity = (Player.transform.position - Pipes[i].transform.position).normalized * Random.Range(30f, 50f) + Vector3.up * Random.Range(10f, 25f);
			velocity.x += Random.Range(-10f, 10f);
			velocity.y += Random.Range(-10f, 10f);
			velocity.z += Random.Range(-10f, 10f);
			Pipes[i].GetComponent<Rigidbody>().velocity = velocity;
			if (Random.value > 0.5f)
			{
				Pipes[i].transform.rotation = Random.rotation;
			}
		}
		for (int k = 0; k < FireFX.Length; k++)
		{
			FireFX[k].SetActive(value: false);
		}
		Player.Camera.PlayShakeMotion();
		Tornado.OnDissipate();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Collider[] componentsInChildren2 = collision.gameObject.GetComponentsInChildren<Collider>();
				foreach (Collider collider2 in componentsInChildren2)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
		}
		if (collision.gameObject.name == "08000004" && !HasCrashed)
		{
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
			_Rigidbody.velocity = Vector3.zero;
			Animator.speed = 1f;
			FX.SetActive(value: true);
			HasCrashed = true;
		}
	}
}
