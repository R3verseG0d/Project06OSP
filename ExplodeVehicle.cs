using UnityEngine;

public class ExplodeVehicle : MonoBehaviour
{
	public float MaxAngularSpeed = 15f;

	public float ExplosionForce = 5f;

	public float ExplosionForceRadius = 5f;

	public GameObject DamageCol;

	public GameObject ExplosionFX;

	public Vector3 Offset;

	private Rigidbody[] RigidBodies;

	private void Start()
	{
		RigidBodies = GetComponentsInChildren<Rigidbody>();
		Object.Instantiate(ExplosionFX, base.transform.position + base.transform.right * Offset.x + base.transform.up * Offset.y + base.transform.forward * Offset.z, base.transform.rotation);
		Rigidbody[] rigidBodies = RigidBodies;
		foreach (Rigidbody rigidbody in rigidBodies)
		{
			rigidbody.maxAngularVelocity = MaxAngularSpeed;
			rigidbody.useGravity = true;
			rigidbody.drag = 0.1f;
			if (!(rigidbody.transform == base.transform))
			{
				rigidbody.transform.parent = base.transform.parent;
				rigidbody.AddExplosionForce(ExplosionForce, base.transform.position, ExplosionForceRadius, 0f, ForceMode.VelocityChange);
			}
		}
		Object.Destroy(DamageCol, 0.5f);
		Object.Destroy(base.gameObject, 10f);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position + base.transform.right * Offset.x + base.transform.up * Offset.y + base.transform.forward * Offset.z, 0.5f);
	}
}
