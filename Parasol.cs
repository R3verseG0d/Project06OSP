using System.Collections.Generic;
using UnityEngine;

public class Parasol : PsiObject
{
	[Header("Prefab")]
	public Rigidbody RigidBody;

	public Renderer Renderer;

	private List<GameObject> PsychoObjs;

	private Transform PlayerPos;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	internal bool PsychoThrown;

	private void Start()
	{
		IsPsychokinesis = false;
		PsychoThrown = false;
		RigidBody.useGravity = true;
		RigidBody.isKinematic = true;
	}

	private void Update()
	{
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
	}

	private void OnCollisionEnter(Collision collision)
	{
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && player.GetPrefab("sonic_fast") && player.CurSpeed >= 30f)
		{
			player.OnHurtEnter();
			OnHit(new HitInfo(player.transform, player.transform.forward * player.CurSpeed));
		}
		if (PsychoThrown)
		{
			collision.gameObject.SendMessage("OnHit", new HitInfo(PlayerTransform, RigidBody.velocity, PsiThrowDamage), SendMessageOptions.DontRequireReceiver);
			ManagePsychoCol();
		}
	}

	public void OnHit(HitInfo HitInfo)
	{
		RigidBody.isKinematic = false;
		Vector3 normalized = Vector3.Slerp(HitInfo.force.normalized, (RigidBody.worldCenterOfMass - HitInfo.player.position).normalized, 0.75f).normalized;
		RigidBody.AddForce((HitInfo.force.normalized + normalized).normalized * HitInfo.force.magnitude * 1f, ForceMode.VelocityChange);
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
		RigidBody.isKinematic = false;
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
