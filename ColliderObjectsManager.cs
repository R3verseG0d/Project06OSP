using UnityEngine;

public class ColliderObjectsManager : MonoBehaviour
{
	[Header("Adds options for physics objects and broken objects, this is meant to be used on stage collision (exceptions for some objects sometimes).")]
	public bool IgnoreBrokenObjOnCollision;

	public bool IgnorePhysicsObjOnCollision;

	public bool IgnoreEnemiesOnCollision;

	public bool DestroyPhysicsObjOnCollision;

	private void OnCollisionEnter(Collision collision)
	{
		if ((IgnoreBrokenObjOnCollision && collision.gameObject.layer == LayerMask.NameToLayer("BrokenObj")) || (IgnorePhysicsObjOnCollision && (bool)collision.transform.GetComponent<PhysicsObj>()) || (IgnoreEnemiesOnCollision && collision.gameObject.layer == LayerMask.NameToLayer("Enemy")))
		{
			Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
		}
		if (!DestroyPhysicsObjOnCollision)
		{
			return;
		}
		PhysicsObj component = collision.transform.GetComponent<PhysicsObj>();
		if ((bool)component && (bool)component.brokenPrefab)
		{
			if (component.SpecialDestroy)
			{
				component.SpecialDestroy = false;
			}
			component.OnHit(new HitInfo(base.transform, Vector3.zero, 10));
		}
	}
}
