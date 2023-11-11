using UnityEngine;

public class PlayerMovingFloor : MonoBehaviour
{
	public Rigidbody RigidBody;

	private PlayerBase PlayerBase;

	private void FixedUpdate()
	{
		if ((bool)PlayerBase && PlayerBase.IsGrounded())
		{
			PlayerBase._Rigidbody.velocity += RigidBody.velocity;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Player" && !PlayerBase)
		{
			PlayerBase = collision.gameObject.GetComponent<PlayerBase>();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			PlayerBase = null;
		}
	}
}
