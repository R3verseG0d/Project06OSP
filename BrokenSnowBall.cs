using System.Collections.Generic;
using STHEngine;
using UnityEngine;

public class BrokenSnowBall : PsiObject
{
	[Header("Framework")]
	public float Rate;

	[Header("Prefab")]
	public Renderer Renderer;

	public Rigidbody RigidBody;

	public GameObject BrokenPrefab;

	public AudioSource Audio;

	private bool Destroyed;

	private List<GameObject> PsychoObjs;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	internal bool PsychoThrown;

	public void SetParameters(float _Rate)
	{
		Rate = _Rate;
	}

	private void Start()
	{
		IsPsychokinesis = false;
		PsychoThrown = false;
		RigidBody.useGravity = true;
	}

	private void Update()
	{
		Audio.volume = Mathf.Lerp(0f, 0.5f, RigidBody.velocity.magnitude / 10f);
		OnPsiFX(Renderer, IsPsychokinesis);
	}

	private void FixedUpdate()
	{
		if (IsPsychokinesis)
		{
			if (!PlayerTransform)
			{
				OnReleasePsycho();
			}
			else
			{
				RigidBody.velocity = (PlayerPos.position + PlayerPos.forward * -6.5f - base.transform.position) * 12f;
			}
		}
		else
		{
			RigidBody.AddForce(RigidBody.velocity.normalized * 10f, ForceMode.Force);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (PsychoThrown)
		{
			HitInfo hitInfo = new HitInfo(PlayerTransform, RigidBody.velocity, PsiThrowDamage);
			collision.gameObject.SendMessage("OnHit", hitInfo, SendMessageOptions.DontRequireReceiver);
			OnHit(hitInfo);
			ManagePsychoCol();
		}
		if ((bool)collision.transform.GetComponent<BrokenSnowBall>())
		{
			HitInfo hitInfo2 = new HitInfo(base.transform, RigidBody.velocity);
			OnHit(hitInfo2);
			collision.gameObject.SendMessage("OnHit", hitInfo2, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Snow" && RigidBody.velocity != Vector3.zero && Rate > 0f && base.transform.localScale.x < 3.25f)
		{
			float num = 0.0025f;
			base.transform.localScale += new Vector3(num, num, num) * (RigidBody.velocity.magnitude * Rate);
		}
	}

	public void OnHit(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			Destroyed = true;
			GameObject gameObject = Object.Instantiate(BrokenPrefab, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			float num = base.transform.localScale.x * 0.565f;
			gameObject.transform.localScale = new Vector3(num, num, num);
			gameObject.SendMessage("OnCreate", HitInfo, SendMessageOptions.DontRequireReceiver);
			Object.Destroy(base.gameObject);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
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
		RigidBody.useGravity = true;
		RigidBody.AddForce(HitInfo.force, ForceMode.VelocityChange);
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		IsPsychokinesis = true;
		PsychoThrown = false;
		RigidBody.useGravity = false;
		PlayerPos = _PlayerPos;
		PlayerTransform = _PlayerPos.parent.transform;
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
		PlayerPos = null;
		RigidBody.useGravity = true;
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
			Collider[] componentsInChildren2 = PsychoObjs[i].GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				list.Add(componentsInChildren2[j]);
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
