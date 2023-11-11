using UnityEngine;

public class RigidbodyRotation : MonoBehaviour
{
	public Rigidbody Rigidbody;

	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized);
	}
}
