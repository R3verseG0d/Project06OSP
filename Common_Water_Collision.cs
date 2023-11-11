using UnityEngine;

public class Common_Water_Collision : MonoBehaviour
{
	public Collider Coll;

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("BreakableObj") && (bool)collider.GetComponent<Rigidbody>() && !collider.GetComponent<Rigidbody>().isKinematic)
		{
			float num = collider.bounds.center.y - 0.5f;
			if (num < Coll.bounds.max.y)
			{
				float num2 = ((collider.GetComponent<Rigidbody>().velocity.y < 0f) ? 1.25f : 1f);
				float num3 = 15f * collider.GetComponent<Rigidbody>().mass * num2;
				float num4 = num3 + num3 * Mathf.Clamp01(Coll.bounds.max.y - num);
				collider.GetComponent<Rigidbody>().AddForce(Vector3.up * num4);
			}
		}
	}
}
