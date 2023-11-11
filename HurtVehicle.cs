using UnityEngine;

public class HurtVehicle : MonoBehaviour
{
	public bool OnCollision;

	public bool Kill;

	public float Damage = 1f;

	private void OnTriggerEnter(Collider collider)
	{
		if (!OnCollision)
		{
			VehicleBase componentInParent = collider.GetComponentInParent<VehicleBase>();
			if ((bool)componentInParent)
			{
				componentInParent.OnDamage((!Kill) ? Damage : 100f);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (OnCollision)
		{
			VehicleBase componentInParent = collision.gameObject.GetComponentInParent<VehicleBase>();
			if ((bool)componentInParent)
			{
				componentInParent.OnDamage((!Kill) ? Damage : 100f);
			}
		}
	}
}
