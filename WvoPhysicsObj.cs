using STHEngine;
using UnityEngine;

public class WvoPhysicsObj : MonoBehaviour
{
	public enum PhysType
	{
		NoLaunchForce = 0,
		LaunchForce = 1
	}

	public PhysType PhysicsType;

	public GameObject brokenPrefab;

	public LayerMask layerMask = 0;

	internal bool Destroyed;

	public void OnHit(HitInfo HitInfo)
	{
		if (!Destroyed && (bool)brokenPrefab)
		{
			Destroyed = true;
			if ((bool)HitInfo.player && HitInfo.player.tag == "Player")
			{
				HitInfo.player.GetComponent<PlayerBase>().AddScore(20);
			}
			GameObject gameObject = Object.Instantiate(brokenPrefab, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			if (PhysicsType == PhysType.LaunchForce)
			{
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Vector3 normalized = Vector3.Slerp(HitInfo.force.normalized, (component.worldCenterOfMass - HitInfo.player.position).normalized, 0.75f).normalized;
				component.AddForce((HitInfo.force.normalized + normalized).normalized * HitInfo.force.magnitude * 1f, ForceMode.VelocityChange);
			}
			gameObject.SendMessage("OnCreate", HitInfo);
			Object.Destroy(base.transform.gameObject);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}
}
