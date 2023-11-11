using UnityEngine;

public class Forearm : PsiObject
{
	[Header("Framework")]
	public Rigidbody RigidBody;

	public Renderer Renderer;

	public float Timer = 3f;

	public Collider Collider;

	public GameObject Trigger;

	public GameObject Explosion;

	public bool IsHoming;

	public int PsychoDamage;

	internal Transform Player;

	internal Transform Owner;

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
			if (IsHoming)
			{
				if ((bool)Owner)
				{
					Vector3 vector = Player.position + Player.up * 0.25f;
					base.transform.up = Vector3.Lerp(base.transform.up, -(vector - base.transform.position).normalized, Time.fixedDeltaTime * 1.5f);
				}
				RigidBody.MovePosition(base.transform.position - base.transform.up * 0.15f);
				if (Time.time - StartTime > 2f || !Owner)
				{
					RigidBody.useGravity = true;
					IsHoming = false;
				}
			}
			if (Time.time - StartTime > 0.1f)
			{
				Collider.enabled = true;
				Trigger.SetActive(value: false);
			}
			if (Time.time - StartTime > ((!PsychoReleased) ? Timer : 1.5f))
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
			RigidBody.velocity = (PlayerPos.position + PlayerPos.forward * -3.75f - base.transform.position) * 24f;
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, PlayerPos.rotation * Quaternion.Euler(-90f, 0f, 0f), Time.fixedDeltaTime * 5f);
		}
		if (PsychoThrown)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 0.25f);
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
					{
						array[i].SendMessage("OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsychoDamage), SendMessageOptions.DontRequireReceiver);
						ExplodeObj(PlayerTransform);
						AutoDestroy();
					}
				}
			}
		}
		if (!Player)
		{
			AutoDestroy();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (IsPsychokinesis)
		{
			return;
		}
		if (!PsychoReleased)
		{
			if (base.enabled && collision.gameObject.tag == "Vehicle")
			{
				collision.gameObject.transform.SendMessage("OnVehicleHit", 1.5f, SendMessageOptions.DontRequireReceiver);
				Explode();
			}
			if (base.enabled && (bool)collision.rigidbody)
			{
				ExplodeObj(collision.transform);
				Explode();
			}
		}
		if (base.enabled && !IsPsychokinesis && !PsychoThrown && IsHoming)
		{
			RigidBody.useGravity = true;
			IsHoming = false;
		}
	}

	private void ExplodeObj(Transform _Transform)
	{
		HitInfo value = new HitInfo(Player, RigidBody.velocity.normalized * 25f, 0);
		if (_Transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			_Transform.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
		if (PsychoThrown && (_Transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || _Transform.gameObject.layer == LayerMask.NameToLayer("EnemyTrigger")))
		{
			_Transform.SendMessage("OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsychoDamage), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Explode()
	{
		Exploded = true;
		Object.Instantiate(Explosion, base.transform.position - base.transform.up * 1.25f, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	public void AutoDestroy()
	{
		Exploded = true;
		GameObject obj = Object.Instantiate(Explosion, base.transform.position - base.transform.up * 1.25f, Quaternion.identity);
		Object.Destroy(obj.GetComponentInChildren<HurtPlayer>());
		Object.Destroy(obj.GetComponentInChildren<Collider>());
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
		base.transform.SetParent(null);
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
			RigidBody.AddForce((base.transform.position - _PlayerPos.position).normalized * RigidBody.velocity.magnitude * 1.5f, ForceMode.VelocityChange);
			Deflected = true;
		}
	}
}
