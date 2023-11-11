using UnityEngine;

public class DummyRing : ObjectBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public GameObject Mesh;

	public GameObject ParticleFX;

	internal Transform Player;

	private bool Destroyed;

	private float Timer;

	public void SetVelocity(Vector3 Velocity)
	{
		_Rigidbody.velocity = Velocity;
	}

	private void Start()
	{
		Timer = Time.time;
		Invoke("DestroyRing", 10f);
	}

	private void FixedUpdate()
	{
		if (!Destroyed)
		{
			Vector3 velocity = _Rigidbody.velocity;
			velocity.y = _Rigidbody.velocity.y + -10f * Time.fixedDeltaTime;
			velocity.x = Mathf.Lerp(velocity.x, 0f, Time.fixedDeltaTime);
			velocity.z = Mathf.Lerp(velocity.z, 0f, Time.fixedDeltaTime);
			_Rigidbody.velocity = velocity;
		}
	}

	private void DestroyRing()
	{
		if (!Destroyed)
		{
			_Rigidbody.useGravity = false;
			_Rigidbody.isKinematic = true;
			Mesh.SetActive(value: false);
			ParticleFX.SetActive(value: true);
			ParticleFX.transform.rotation = Quaternion.identity;
			Object.Destroy(base.gameObject, 1.2f);
			Destroyed = true;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if ((bool)collider.GetComponent<EnemyBase>() && !(Time.time - Timer < 0.5f) && !Destroyed)
		{
			collider.SendMessage("OnHit", new HitInfo(Player, Vector3.zero), SendMessageOptions.DontRequireReceiver);
			DestroyRing();
		}
	}
}
