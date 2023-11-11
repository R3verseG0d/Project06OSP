using STHEngine;
using UnityEngine;

public class Container : ObjectBase
{
	[Header("Framework")]
	public GameObject BrokenPrefab;

	internal bool Destroyed;

	private Rigidbody RigidBody;

	private void Start()
	{
		RigidBody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && player.GetPrefab("sonic_fast") && player.CurSpeed >= 30f)
		{
			player.OnHurtEnter(1);
			DestroyContainer(new HitInfo(player.transform, player.transform.forward * player.CurSpeed));
		}
	}

	private void DestroyContainer(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			Destroyed = true;
			GameObject gameObject = Object.Instantiate(BrokenPrefab, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", HitInfo);
			Object.Destroy(base.transform.gameObject);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		DestroyContainer(HitInfo);
	}
}
