using UnityEngine;

public class VehicleSensor : MonoBehaviour
{
	public VehicleBase MainScript;

	private void OnTriggerEnter(Collider collider)
	{
		if ((bool)collider.GetComponent<EnemyBase>())
		{
			if (MainScript.Hit() != 0)
			{
				collider.SendMessage("OnHit", new HitInfo(MainScript.PM ? MainScript.PM.transform : base.transform, ImpactForce(), MainScript.Hit()), SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				MainScript.Audio.PlayOneShot(MainScript.Crash, MainScript.Audio.volume * 0.5f);
				MainScript.OnVehicleHit(MainScript.Damage());
			}
		}
		if (collider.gameObject.name != "wap_obj_spot" && (bool)collider.GetComponentInParent<SearchLight>())
		{
			collider.GetComponentInParent<SearchLight>().DestroySearchlight();
		}
		if (MainScript.GetVehicleName() != "Glider" && collider.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
		{
			collider.SendMessage("OnHit", new HitInfo(MainScript.PM ? MainScript.PM.transform : base.transform, ImpactForce(), MainScript.Hit()), SendMessageOptions.DontRequireReceiver);
		}
		if ((bool)collider.GetComponent<Ring>() && MainScript.IsMounted)
		{
			MainScript.PM.Base.AddRing();
			MainScript.PM.Base.AddScore(10);
			collider.SendMessage("OnCollect", SendMessageOptions.DontRequireReceiver);
		}
	}

	private Vector3 ImpactForce()
	{
		return MainScript._Rigidbody.velocity.normalized * MainScript._Rigidbody.velocity.magnitude;
	}

	private void OnVehicleHit(float Damage)
	{
		MainScript.OnVehicleHit(Damage);
	}
}
