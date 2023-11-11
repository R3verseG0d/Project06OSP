using UnityEngine;

public class Aqa_Balancer : ObjectBase
{
	[Header("Framework")]
	public float Power;

	public float Deceleration;

	[Header("Prefab")]
	public GameObject[] Meshes;

	public ReflectionProbe Probe;

	public AudioSource Audio;

	public AudioClip[] Clips;

	public Transform PlayerPoint;

	public Animator Animator;

	public Collider Collider;

	public Rigidbody Rigidbody;

	public ParticleSystem DamageFX;

	public ParticleSystem ExplodeFX;

	private PlayerManager PM;

	private Vector3 StartPos;

	private bool Exploded;

	private bool CanGetDamaged;

	private float StartTime;

	private float HitCooldownTime;

	private float Speed;

	private float MaxSpeed;

	private int HP;

	public void SetParameters(float _Power, float _Deceleration)
	{
		Power = _Power;
		Deceleration = _Deceleration;
	}

	private void Start()
	{
		Meshes[0].SetActive(Singleton<Settings>.Instance.settings.Reflections == 0);
		Meshes[1].SetActive(Singleton<Settings>.Instance.settings.Reflections != 0);
		switch (Singleton<Settings>.Instance.settings.Reflections)
		{
		case 3:
			Probe.resolution = 256;
			break;
		case 2:
			Probe.resolution = 128;
			break;
		default:
			Probe.resolution = 64;
			break;
		}
		StartPos = base.transform.position;
		HP = 2;
		CanGetDamaged = true;
	}

	private void StateBalancerStart()
	{
		PM.Base.SetState("Balancer");
		PM.transform.SetParent(PlayerPoint);
		base.transform.forward = PM.transform.forward.MakePlanar();
		PM.RBody.isKinematic = true;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		PM.transform.position = PlayerPoint.position + PlayerPoint.up * 0.25f;
		PM.transform.rotation = base.transform.rotation;
		PM.RBody.velocity = Vector3.zero;
		MaxSpeed = PM.Base.TopSpeed;
	}

	private void StateBalancer()
	{
		PM.Base.SetState("Balancer");
		PM.Base.LockControls = true;
		PM.Base.PlayAnimation("Movement (Blend Tree)", "On Ground");
		PM.Base.CurSpeed = Rigidbody.velocity.magnitude;
		PM.transform.position = PlayerPoint.position + PlayerPoint.up * 0.25f;
		PM.transform.rotation = base.transform.rotation;
		PM.RBody.velocity = Vector3.zero;
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
	}

	private void StateBalancerEnd()
	{
	}

	private void FixedUpdate()
	{
		if (Exploded)
		{
			return;
		}
		if ((bool)PM)
		{
			bool flag = Vector3.Dot(PM.Base.TargetDirection, Rigidbody.velocity.normalized) < -0.75f;
			if (PM.Base.GetState() == "Balancer" && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused)
			{
				float axis = Singleton<RInput>.Instance.P.GetAxis("Left Stick X");
				float axis2 = Singleton<RInput>.Instance.P.GetAxis("Left Stick Y");
				Quaternion quaternion = Quaternion.Euler(Vector3.up * ((PM.Base.Camera != null) ? PM.Base.Camera.transform.localEulerAngles.y : 1f));
				Vector3 vector = quaternion * Vector3.forward;
				vector.y = 0f;
				vector.Normalize();
				Vector3 vector2 = quaternion * Vector3.right;
				vector2.y = 0f;
				vector2.Normalize();
				PM.Base.TargetDirection = (axis * vector2 + axis2 * vector).normalized;
				if (PM.Base.TargetDirection != Vector3.zero && !flag)
				{
					Quaternion b = Quaternion.LookRotation(PM.Base.TargetDirection, Vector3.up);
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, PM.Base.RotOverSpdCurve.Evaluate(PM.Base.CurSpeed) * Time.fixedDeltaTime);
				}
			}
			if (PM.Base.TargetDirection != Vector3.zero && !flag)
			{
				Speed += Power * Time.fixedDeltaTime;
			}
			else
			{
				Speed -= Deceleration * ((!flag) ? 1f : 2f) * Time.fixedDeltaTime;
			}
			if (Vector3.Dot(PM.Base.TargetDirection, Rigidbody.velocity.normalized) < -0.15f && Speed > 0f)
			{
				Speed -= Deceleration * Time.fixedDeltaTime * 2f;
			}
		}
		else
		{
			Speed -= 2f * Time.fixedDeltaTime;
		}
		Speed = Mathf.Clamp(Speed, 0f, MaxSpeed);
		Rigidbody.velocity = base.transform.forward * Speed;
	}

	private void Update()
	{
		Animator.SetInteger("Health", HP);
		if ((bool)PM && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && PM.Base.GetState() == "Balancer" && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			PM.Base.SetMachineState("StateJump");
			OnBalancerExit(PM.Base);
		}
		if (Exploded && Time.time - StartTime > 1f)
		{
			Reappear();
			Exploded = false;
		}
		if (!CanGetDamaged && Time.time - HitCooldownTime > 0.5f)
		{
			CanGetDamaged = true;
		}
	}

	private void Explode()
	{
		ExplodeFX.Play();
		Speed = 0f;
		Rigidbody.velocity = Vector3.zero;
		if ((bool)PM && PM.Base.GetState() == "Balancer")
		{
			PM.Base.SetMachineState("StateAir");
			OnBalancerExit(PM.Base);
		}
		Animator.SetTrigger("On Explode");
		Exploded = true;
		StartTime = Time.time;
	}

	private void Reappear()
	{
		HP = 2;
		base.transform.position = StartPos;
		Animator.SetTrigger("On Reappear");
	}

	private void OnBalancerExit(PlayerBase Player)
	{
		if (!(Player != PM.Base))
		{
			PM.Base.transform.SetParent(null);
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			if (PM.Base.GetPrefab("silver"))
			{
				PM.silver.BalancerFactor = 0f;
			}
			PM = null;
		}
	}

	private void OnBounce(ContactPoint[] Contacts, bool Damage = false)
	{
		for (int i = 0; i < Contacts.Length; i++)
		{
			ContactPoint contactPoint = Contacts[i];
			base.transform.forward = (Quaternion.AngleAxis(180f, contactPoint.normal) * base.transform.forward * -1f).MakePlanar();
			if (Damage)
			{
				DamageFX.transform.position = contactPoint.point;
				DamageFX.transform.rotation = Quaternion.LookRotation(contactPoint.point - base.transform.position);
				DamageFX.Play();
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !(player.GetState() == "Balancer") && !player.GetPrefab("snow_board") && !Exploded)
		{
			PM = collider.GetComponent<PlayerManager>();
			Audio.PlayOneShot(Clips[0], Audio.volume);
			player.StateMachine.ChangeState(base.gameObject, StateBalancer);
			if (PM.Base.GetPrefab("silver"))
			{
				PM.silver.BalancerFactor = 1f;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((bool)GetPlayer(collision.transform))
		{
			return;
		}
		if ((bool)collision.transform.GetComponent<HurtPlayer>())
		{
			if (CanGetDamaged)
			{
				HP--;
				if (HP == 0)
				{
					Explode();
				}
				else
				{
					CanGetDamaged = false;
					HitCooldownTime = Time.time;
					OnBounce(collision.contacts, Damage: true);
					Animator.SetTrigger("On Damage");
				}
				Audio.PlayOneShot(Clips[2], Audio.volume);
			}
		}
		else if (!collision.transform.GetComponent<Bullet>() && !collision.transform.GetComponent<Missile>() && !collision.transform.GetComponent<TimedBomb>() && !collision.transform.GetComponent<LaserBeam>())
		{
			OnBounce(collision.contacts);
			Animator.SetTrigger("On Bump");
			Audio.PlayOneShot(Clips[1], Audio.volume);
		}
	}
}
