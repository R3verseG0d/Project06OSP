using UnityEngine;

public class HurtEnemy : MonoBehaviour
{
	public bool OnCollision;

	[Header("Optional")]
	public bool SpeedToDamage;

	public Rigidbody RigidBody;

	public float SpeedMag;

	private void OnTriggerEnter(Collider collider)
	{
		if (!OnCollision && (bool)collider.GetComponent<EnemyBase>())
		{
			if (!SpeedToDamage)
			{
				collider.SendMessage("OnHit", new HitInfo(base.transform, Vector3.zero, 10), SendMessageOptions.DontRequireReceiver);
			}
			else if (SpeedToDamage && RigidBody.velocity.magnitude > SpeedMag)
			{
				collider.SendMessage("OnHit", new HitInfo(base.transform, RigidBody.velocity.normalized * RigidBody.velocity.magnitude, 10), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (OnCollision && (bool)collision.transform.GetComponent<EnemyBase>())
		{
			if (!SpeedToDamage)
			{
				collision.transform.SendMessage("OnHit", new HitInfo(base.transform, Vector3.zero, 10), SendMessageOptions.DontRequireReceiver);
			}
			else if (SpeedToDamage && RigidBody.velocity.magnitude > SpeedMag)
			{
				collision.transform.SendMessage("OnHit", new HitInfo(base.transform, RigidBody.velocity.normalized * RigidBody.velocity.magnitude, 10), SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
