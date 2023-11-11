using UnityEngine;

public class VehicleRespawn : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		VehicleBase componentInParent = collider.GetComponentInParent<VehicleBase>();
		if ((bool)componentInParent)
		{
			if (componentInParent.IsMounted)
			{
				componentInParent.Demount();
			}
			componentInParent.CurHP = 0f;
		}
	}
}
