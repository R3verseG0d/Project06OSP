using UnityEngine;

public class TimedBomb : PsiObject
{
	[Header("Framework")]
	public Rigidbody RigidBody;

	public Renderer Renderer;

	public float ExplodeTime;

	public bool ExplodeOnCollision;

	public bool ExplodeOnContact;

	public bool DontSafeDestroy;

	public SphereCollider Collider;

	public GameObject Explosion;

	public bool PsychoStun;

	public int PsychoDamage;

	public float PyschoDistFactor = 1f;

	internal bool Launched;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	private bool Deflected;

	private bool Exploded;

	private bool IsPsychokinesis;

	internal bool PsychoThrown;

	private bool PsychoReleased;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void Update()
	{
		OnPsiFX(Renderer, IsPsychokinesis);
	}

	private void FixedUpdate()
	{
		if (!base.enabled || Exploded)
		{
			return;
		}
		if (!IsPsychokinesis)
		{
			if (Time.time - StartTime > 0.1f)
			{
				Collider.enabled = true;
			}
			if (Time.time - StartTime > ((!PsychoReleased) ? ExplodeTime : 1.5f))
			{
				Explode();
			}
		}
		else if (!PlayerTransform)
		{
			OnReleasePsycho();
		}
		else
		{
			RigidBody.velocity = (PlayerPos.position + PlayerPos.forward * (-1.625f * PyschoDistFactor) + PlayerPos.up * -1f - base.transform.position) * 24f;
		}
		if (!PsychoThrown)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.25f);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				array[i].SendMessage(PsychoStun ? "OnFlash" : "OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsychoDamage), SendMessageOptions.DontRequireReceiver);
				ExplodeObj(PlayerTransform);
				AutoDestroy();
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (ExplodeOnCollision && !IsPsychokinesis && !PsychoReleased)
		{
			if (base.enabled && collision.gameObject.tag == "Vehicle")
			{
				collision.gameObject.transform.SendMessage("OnVehicleHit", 1.5f, SendMessageOptions.DontRequireReceiver);
				Explode();
			}
			if (base.enabled && ((!ExplodeOnContact && (bool)collision.rigidbody) || ExplodeOnContact))
			{
				ExplodeObj(collision.transform);
				Explode();
			}
		}
	}

	private void ExplodeObj(Transform _Transform)
	{
		HitInfo value = new HitInfo(base.transform, RigidBody.velocity.normalized * 25f, 0);
		if (_Transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			_Transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
		if (PsychoThrown && (_Transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || _Transform.gameObject.layer == LayerMask.NameToLayer("EnemyTrigger")))
		{
			_Transform.SendMessage(PsychoStun ? "OnFlash" : "OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsychoDamage), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Explode()
	{
		Exploded = true;
		Object.Instantiate(Explosion, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	public void AutoDestroy()
	{
		Exploded = true;
		if (!DontSafeDestroy)
		{
			GameObject obj = Object.Instantiate(Explosion, base.transform.position, Quaternion.identity);
			Object.Destroy(obj.GetComponent<HurtPlayer>());
			Object.Destroy(obj.GetComponent<Collider>());
		}
		else
		{
			Object.Instantiate(Explosion, base.transform.position, Quaternion.identity);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnPsychoThrow(HitInfo HitInfo)
	{
		IsPsychokinesis = false;
		PsychoThrown = true;
		RigidBody.useGravity = true;
		RigidBody.AddForce(HitInfo.force, ForceMode.VelocityChange);
		StartTime = Time.time;
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		IsPsychokinesis = true;
		PsychoReleased = false;
		PlayerPos = _PlayerPos;
		PlayerTransform = _PlayerPos.root.transform;
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
		PsychoReleased = true;
		StartTime = Time.time;
		PlayerPos = null;
	}

	private void OnDeflect(Transform _PlayerPos)
	{
		if (base.enabled && !Deflected && !IsPsychokinesis && !PsychoReleased && !PsychoThrown)
		{
			PlayerBase component = _PlayerPos.GetComponent<PlayerBase>();
			if ((bool)component && (bool)component.PlayerManager.silver && component.PlayerManager.silver.IsAwakened)
			{
				component.PlayerManager.silver.SilverEffects.CreatePsiDeflectFX(base.transform.position);
			}
			RigidBody.AddForce((base.transform.position - _PlayerPos.position).normalized * 20f, ForceMode.VelocityChange);
			Deflected = true;
		}
	}
}
