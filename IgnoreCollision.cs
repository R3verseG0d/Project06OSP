using UnityEngine;

public class IgnoreCollision : ObjectBase
{
	public MeshCollider MeshCollider;

	public string OnState;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (!(OnState != "") || !(player.GetState() != OnState)))
		{
			MeshCollider.enabled = false;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (!(OnState != "") || !(player.GetState() != OnState)))
		{
			MeshCollider.enabled = true;
		}
	}
}
