using UnityEngine;

public class Missile : PsiObject
{
	[Header("Framework")]
	public Rigidbody RigidBody;

	public Renderer Renderer;

	public float speed = 0.15f;

	public float turnSpeed = 2.5f;

	public bool upDirection;

	public bool homing = true;

	public GameObject missileExplosion;

	public ParticleSystem[] ExhaustFX;

	public bool PsychoStun;

	public int PsychoDamage;

	internal Transform Player;

	internal Transform Owner;

	private Vector3 Direction;

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
		if ((bool)Renderer)
		{
			OnPsiFX(Renderer, IsPsychokinesis);
		}
		if (!Owner && !IsPsychokinesis && !PsychoThrown && !PsychoReleased && !Exploded)
		{
			Invoke("AutoDestroy", Random.Range(0f, 0.25f));
		}
	}

	private void FixedUpdate()
	{
		if (!Player)
		{
			AutoDestroy();
		}
		if (!Exploded && !IsPsychokinesis)
		{
			if (!PsychoReleased)
			{
				if (!PsychoThrown)
				{
					if (homing)
					{
						Vector3 vector = Player.position + Player.up * 0.25f;
						base.transform.forward = Vector3.Lerp(base.transform.forward, (vector - base.transform.position).normalized, Time.fixedDeltaTime * turnSpeed);
					}
					RigidBody.MovePosition(base.transform.position + base.transform.forward * speed);
				}
				Direction = base.transform.forward;
			}
			if (Time.time - StartTime > ((!PsychoReleased) ? 5f : 1.5f))
			{
				Explode();
			}
		}
		if (IsPsychokinesis)
		{
			if (!PlayerTransform)
			{
				OnReleasePsycho();
			}
			else
			{
				RigidBody.velocity = (PlayerPos.position + PlayerPos.forward * -1.625f + PlayerPos.up * -1f - base.transform.position) * 24f;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, PlayerPos.rotation, Time.fixedDeltaTime * 5f);
			}
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
				array[i].SendMessage(PsychoStun ? "OnFlash" : "OnHit", new HitInfo(PlayerTransform, Direction * 25f, PsychoDamage), SendMessageOptions.DontRequireReceiver);
				ExplodeObj(PlayerTransform);
				AutoDestroy();
			}
		}
	}

	private void Explode()
	{
		Exploded = true;
		Object.Instantiate(missileExplosion, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	private void AutoDestroy()
	{
		Exploded = true;
		GameObject obj = Object.Instantiate(missileExplosion, base.transform.position, Quaternion.identity);
		Object.Destroy(obj.GetComponent<HurtPlayer>());
		Object.Destroy(obj.GetComponent<Collider>());
		Object.Destroy(base.gameObject);
	}

	private void ExplodeObj(Transform _Transform)
	{
		HitInfo value = new HitInfo(Player, Direction * 25f, 0);
		if (_Transform.gameObject.tag == "Vehicle")
		{
			_Transform.gameObject.SendMessage("OnVehicleHit", 1.5f, SendMessageOptions.DontRequireReceiver);
		}
		if (_Transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			_Transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
		if (PsychoThrown && (_Transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || _Transform.gameObject.layer == LayerMask.NameToLayer("EnemyTrigger")))
		{
			_Transform.SendMessage(PsychoStun ? "OnFlash" : "OnHit", new HitInfo(PlayerTransform, Direction * 25f, PsychoDamage), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!IsPsychokinesis && !PsychoReleased)
		{
			if (!Exploded)
			{
				Explode();
			}
			ExplodeObj(collider.transform);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!IsPsychokinesis && !PsychoReleased)
		{
			if (!Exploded)
			{
				Explode();
			}
			ExplodeObj(collision.transform);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!IsPsychokinesis && !PsychoReleased)
		{
			if (!Exploded)
			{
				Explode();
			}
			ExplodeObj(collision.transform);
		}
	}

	private void OnPsychoThrow(HitInfo HitInfo)
	{
		IsPsychokinesis = false;
		PsychoThrown = true;
		if (ExhaustFX != null)
		{
			for (int i = 0; i < ExhaustFX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = ExhaustFX[i].emission;
				emission.enabled = true;
			}
		}
		base.transform.forward = HitInfo.force;
		RigidBody.velocity = HitInfo.force;
		StartTime = Time.time;
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		IsPsychokinesis = true;
		PsychoReleased = false;
		if (ExhaustFX != null)
		{
			for (int i = 0; i < ExhaustFX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = ExhaustFX[i].emission;
				emission.enabled = false;
			}
		}
		PlayerPos = _PlayerPos;
		PlayerTransform = _PlayerPos.root.transform;
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
		PsychoReleased = true;
		RigidBody.useGravity = true;
		PlayerPos = null;
		StartTime = Time.time;
	}

	private void OnDeflect(Transform _PlayerPos)
	{
		if (base.enabled && !Deflected && !IsPsychokinesis && !PsychoReleased && !PsychoThrown)
		{
			if (homing)
			{
				homing = false;
			}
			PlayerBase component = _PlayerPos.GetComponent<PlayerBase>();
			if ((bool)component && (bool)component.PlayerManager.silver && component.PlayerManager.silver.IsAwakened)
			{
				component.PlayerManager.silver.SilverEffects.CreatePsiDeflectFX(base.transform.position);
			}
			base.transform.forward = (base.transform.position - _PlayerPos.position).normalized;
			Deflected = true;
		}
	}
}
