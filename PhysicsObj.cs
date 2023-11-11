using System.Collections.Generic;
using STHEngine;
using UnityEngine;

public class PhysicsObj : PsiObject
{
	public enum Type
	{
		Physics = 0,
		Explosive = 1,
		Flash = 2
	}

	[Header("Framework")]
	public int Score;

	public Renderer Renderer;

	public Renderer[] AdditionalRenderers;

	public GameObject brokenPrefab;

	public LayerMask layerMask = 0;

	public Type objectType;

	[Header("Optional")]
	public float PhysForceMult = 1f;

	public bool UseHealth;

	public int Health;

	public bool DestroyOnCollision;

	public float SpeedToDestroy;

	public bool UnfreezeOnHit;

	public bool SpecialDestroy;

	internal bool Destroyed;

	private int HP;

	private List<GameObject> PsychoObjs;

	private Rigidbody RigidBody;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	private bool IsUpheave;

	internal bool PsychoThrown;

	private bool RBGrabSettings;

	private bool StartRBGrav;

	private bool StartRBKine;

	private void Start()
	{
		RigidBody = GetComponent<Rigidbody>();
		IsPsychokinesis = false;
		PsychoThrown = false;
		if ((bool)RigidBody)
		{
			if (!RBGrabSettings)
			{
				StartRBGrav = RigidBody.useGravity;
				StartRBKine = RigidBody.isKinematic;
				RBGrabSettings = true;
			}
			RigidBody.useGravity = StartRBGrav;
			RigidBody.isKinematic = StartRBKine;
		}
	}

	private void Update()
	{
		if (!Renderer)
		{
			return;
		}
		OnPsiFX(Renderer, IsPsychokinesis || IsUpheave);
		if (AdditionalRenderers != null)
		{
			for (int i = 0; i < AdditionalRenderers.Length; i++)
			{
				OnPsiFX(AdditionalRenderers[i], IsPsychokinesis || IsUpheave);
			}
		}
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
		if (IsUpheave && !PlayerTransform)
		{
			OnUpheaveRelease();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (DestroyOnCollision && !IsPsychokinesis && !PsychoThrown && collision.relativeVelocity.magnitude > SpeedToDestroy && collision.gameObject.layer != LayerMask.NameToLayer("Player") && collision.gameObject.layer != LayerMask.NameToLayer("PlayerPushCol"))
		{
			OnHit(new HitInfo(base.gameObject.transform, Vector3.zero));
		}
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && player.GetPrefab("sonic_fast") && player.CurSpeed >= 30f)
		{
			player.OnHurtEnter();
			OnHit(new HitInfo(player.transform, player.transform.forward * player.CurSpeed));
		}
		if ((bool)brokenPrefab)
		{
			if (PsychoThrown)
			{
				HitInfo hitInfo = new HitInfo(PlayerTransform, RigidBody.velocity, PsiThrowDamage);
				collision.gameObject.SendMessage("OnHit", hitInfo, SendMessageOptions.DontRequireReceiver);
				OnHit(hitInfo);
				ManagePsychoCol();
			}
			PsychoThrown = false;
		}
		else
		{
			if (PsychoThrown)
			{
				collision.gameObject.SendMessage("OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsiThrowDamage), SendMessageOptions.DontRequireReceiver);
				ManagePsychoCol();
			}
			PsychoThrown = false;
		}
	}

	public void OnHit(HitInfo HitInfo)
	{
		if (Destroyed || IsUpheave)
		{
			return;
		}
		if (UnfreezeOnHit && !IsUpheave && !IsPsychokinesis && RigidBody.isKinematic)
		{
			RigidBody.useGravity = true;
			RigidBody.isKinematic = false;
		}
		bool flag = (bool)HitInfo.player && HitInfo.player.tag == "Player" && ((HitInfo.player.GetComponent<PlayerBase>().GetPrefab("shadow") && HitInfo.player.GetComponent<Shadow>().IsChaosBoost) || (HitInfo.player.GetComponent<PlayerBase>().GetPrefab("sonic_new") && HitInfo.player.GetComponent<SonicNew>().IsSuper) || (HitInfo.player.GetComponent<PlayerBase>().GetPrefab("sonic_fast") && HitInfo.player.GetComponent<SonicFast>().IsSuper));
		if ((bool)brokenPrefab && (!SpecialDestroy || (SpecialDestroy && flag)))
		{
			if (UseHealth)
			{
				HP += ((!flag) ? HitInfo.damage : 10);
			}
			if (!UseHealth || (UseHealth && HP > Health))
			{
				Destroyed = true;
				GameObject gameObject = Object.Instantiate(brokenPrefab, base.transform.position, base.transform.rotation);
				ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
				if (objectType != 0)
				{
					HitInfo.force = base.transform.position;
				}
				gameObject.SendMessage("OnCreate", HitInfo, SendMessageOptions.DontRequireReceiver);
				if (objectType != 0)
				{
					OnExplode(HitInfo);
				}
				if (Score != 0 && (bool)HitInfo.player && HitInfo.player.tag == "Player")
				{
					HitInfo.player.GetComponent<PlayerBase>().AddScore(Score);
				}
				Object.Destroy(base.transform.gameObject);
			}
			else
			{
				Launch(HitInfo);
			}
		}
		else
		{
			Launch(HitInfo);
		}
	}

	private void Launch(HitInfo HitInfo)
	{
		Vector3 normalized = Vector3.Slerp(HitInfo.force.normalized, (RigidBody.worldCenterOfMass - HitInfo.player.position).normalized, 0.75f).normalized;
		RigidBody.AddForce((HitInfo.force.normalized + normalized).normalized * HitInfo.force.magnitude * PhysForceMult, ForceMode.VelocityChange);
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}

	private void OnExplode(HitInfo HitInfo)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 5f, layerMask);
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].isTrigger && array[i].transform != base.transform)
			{
				HitInfo.force = base.transform.position;
				if (objectType == Type.Explosive)
				{
					array[i].SendMessage("OnExplosion", HitInfo, SendMessageOptions.DontRequireReceiver);
				}
				else if (objectType == Type.Flash)
				{
					array[i].SendMessage("OnFlash", null, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void OnEventSignal()
	{
		if ((bool)brokenPrefab)
		{
			OnHit(new HitInfo(base.transform, Vector3.zero, 10));
		}
	}

	private void OnUpheave(HitInfo HitInfo)
	{
		IsUpheave = true;
		PlayerTransform = HitInfo.player;
		RigidBody.isKinematic = false;
		RigidBody.useGravity = false;
		RigidBody.velocity = HitInfo.force;
		RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private void OnUpheaveRelease()
	{
		IsUpheave = false;
		RigidBody.useGravity = true;
		RigidBody.constraints = RigidbodyConstraints.None;
		RigidBody.velocity = Vector3.zero;
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
		RigidBody.isKinematic = false;
		PlayerPos = _PlayerPos;
		PlayerTransform = _PlayerPos.root.transform;
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
