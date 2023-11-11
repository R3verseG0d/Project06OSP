using System.Collections.Generic;
using UnityEngine;

public class Aqa_Mercury_Small : PsiObject
{
	[Header("Framework")]
	public int HP;

	public float Friction;

	public float Air_Friction;

	[Header("Prefab")]
	public Rigidbody Rigidbody;

	public Renderer Renderer;

	public Animator Animator;

	public GameObject HomingTarget;

	public Collider Collider;

	public AudioSource Audio;

	public GameObject ExplodeFX;

	internal bool Launch;

	internal bool Appear;

	private float HoverForce;

	private float HoverSpeed;

	private float StartTime;

	private List<GameObject> PsychoObjs;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	internal bool PsychoThrown;

	public void SetParameters(int _HP, float _Friction, float _Air_Friction)
	{
		HP = _HP;
		Friction = _Friction;
		Air_Friction = _Air_Friction;
	}

	private void Start()
	{
		if (!Launch)
		{
			Rigidbody.drag = Air_Friction;
			Rigidbody.angularDrag = Friction;
		}
		HoverForce = Random.Range(6f, 12f);
		HoverSpeed = Random.Range(4f, 6f);
		IsPsychokinesis = false;
		PsychoThrown = false;
	}

	private void Update()
	{
		OnPsiFX(Renderer, IsPsychokinesis);
	}

	private void FixedUpdate()
	{
		if (!Appear)
		{
			if (!IsPsychokinesis && !PsychoThrown && !Launch && Rigidbody.velocity.magnitude < 5f)
			{
				Rigidbody.AddForce(Vector3.up * HoverForce * Mathf.Cos(Time.time * HoverSpeed));
			}
			if (IsPsychokinesis)
			{
				if (!PlayerTransform)
				{
					OnReleasePsycho();
				}
				else
				{
					Rigidbody.velocity = (PlayerPos.position + PlayerPos.forward * -6.5f - base.transform.position) * 12f;
				}
			}
		}
		else
		{
			if (Time.time - StartTime > 1.5f)
			{
				Rigidbody.velocity = Vector3.zero;
				Appear = false;
			}
			Rigidbody.velocity = Vector3.Lerp(Rigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * 1f);
		}
	}

	public void OnAppear(float Vel)
	{
		Appear = true;
		StartTime = Time.time;
		Animator.SetTrigger("On Appear");
		Rigidbody.velocity = Vector3.up * Vel;
	}

	public void OnLaunch(Vector3 Velocity)
	{
		Launch = true;
		Rigidbody.drag = 0f;
		Rigidbody.angularDrag = 0f;
		Rigidbody.velocity = Velocity;
		HomingTarget.tag = "HomingTarget";
	}

	private void OnHit(HitInfo HitInfo)
	{
		HP--;
		if (HP <= 0)
		{
			Object.Instantiate(ExplodeFX, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
			return;
		}
		if (Launch)
		{
			Rigidbody.drag = Air_Friction;
			Rigidbody.angularDrag = Friction;
			HomingTarget.tag = "Untagged";
			Launch = false;
		}
		Rigidbody.velocity = HitInfo.force;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!IsPsychokinesis && !PsychoThrown)
		{
			PlayerBase player = GetPlayer(collision.transform);
			if (Appear || Launch)
			{
				return;
			}
			Audio.Play();
			Audio.pitch = Random.Range(1.25f, 2f);
			ContactPoint[] contacts = collision.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				if (!player && collision.gameObject.layer != LayerMask.NameToLayer("PlayerPushColl"))
				{
					Vector3 velocity = Vector3.Reflect(Rigidbody.velocity, contactPoint.normal);
					Rigidbody.velocity = velocity;
				}
				Animator.SetTrigger("On Bump");
			}
		}
		else if (PsychoThrown)
		{
			HitInfo hitInfo = new HitInfo(PlayerTransform, Rigidbody.velocity, PsiThrowDamage);
			OnHit(hitInfo);
			collision.gameObject.SendMessage("OnHit", hitInfo, SendMessageOptions.DontRequireReceiver);
			ManagePsychoCol();
			PsychoThrown = false;
		}
	}

	private void PsychoThrowSetup(List<GameObject> Objs)
	{
		PsychoObjs = Objs;
	}

	private void OnPsychoThrow(HitInfo HitInfo)
	{
		IsPsychokinesis = false;
		PsychoThrown = true;
		PsiThrowDamage = HitInfo.damage;
		Rigidbody.AddForce(HitInfo.force, ForceMode.VelocityChange);
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		IsPsychokinesis = true;
		PsychoThrown = false;
		PlayerPos = _PlayerPos;
		PlayerTransform = _PlayerPos.root.transform;
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
		PlayerPos = null;
		ManagePsychoCol();
	}

	private void ManagePsychoCol()
	{
		if (PsychoObjs.Count == 0)
		{
			return;
		}
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		List<Collider> list = new List<Collider>();
		for (int i = 0; i < PsychoObjs.Count; i++)
		{
			if ((bool)PsychoObjs[i])
			{
				Collider[] componentsInChildren2 = PsychoObjs[i].GetComponentsInChildren<Collider>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					list.Add(componentsInChildren2[j]);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			Collider[] array = componentsInChildren;
			foreach (Collider collider in array)
			{
				Physics.IgnoreCollision(list[k], collider, ignore: false);
			}
		}
		PsychoObjs.Clear();
	}
}
