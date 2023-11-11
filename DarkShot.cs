using UnityEngine;

public class DarkShot : ObjectBase
{
	[Header("Framework")]
	public Rigidbody RigidBody;

	public float Speed = 0.15f;

	public float TurnSpeed = 2.5f;

	public GameObject Explosion;

	internal Transform Player;

	internal Transform Owner;

	private bool Deflect;

	private bool Exploded;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (!Player)
		{
			Explode();
		}
		if (!Exploded)
		{
			if ((bool)Owner)
			{
				Vector3 vector = Player.position + Player.up * 0.25f;
				base.transform.forward = Vector3.Lerp(base.transform.forward, (vector - base.transform.position).normalized, Time.fixedDeltaTime * TurnSpeed);
			}
			RigidBody.MovePosition(base.transform.position + base.transform.forward * Speed);
			if (Time.time - StartTime > 5f)
			{
				Explode();
			}
		}
	}

	private void Explode()
	{
		Exploded = true;
		Object.Instantiate(Explosion, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	private void AutoDestroy()
	{
		Exploded = true;
		GameObject obj = Object.Instantiate(Explosion, base.transform.position, Quaternion.identity);
		Object.Destroy(obj.GetComponentInChildren<HurtPlayer>());
		Object.Destroy(obj.GetComponentInChildren<Collider>());
		Object.Destroy(base.gameObject);
	}

	private void ExplodeObj(Transform _Transform)
	{
		HitInfo value = new HitInfo(Player, base.transform.forward * 25f, 0);
		if (_Transform.gameObject.tag == "Vehicle")
		{
			_Transform.gameObject.SendMessage("OnVehicleHit", 1.5f, SendMessageOptions.DontRequireReceiver);
		}
		if (_Transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			_Transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!Exploded)
		{
			Explode();
		}
		ExplodeObj(collider.transform);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!Exploded)
		{
			Explode();
		}
		ExplodeObj(collision.transform);
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!Exploded)
		{
			Explode();
		}
		ExplodeObj(collision.transform);
	}

	private void OnDeflect(Transform _PlayerPos)
	{
		if (base.enabled && !Deflect)
		{
			if ((bool)Owner)
			{
				Owner = null;
			}
			PlayerBase component = _PlayerPos.GetComponent<PlayerBase>();
			if ((bool)component && (bool)component.PlayerManager.silver && component.PlayerManager.silver.IsAwakened)
			{
				component.PlayerManager.silver.SilverEffects.CreatePsiDeflectFX(base.transform.position);
			}
			base.transform.forward = (base.transform.position - _PlayerPos.position).normalized;
			Deflect = true;
		}
	}
}
