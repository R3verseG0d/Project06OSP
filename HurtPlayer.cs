using UnityEngine;

public class HurtPlayer : ObjectBase
{
	public enum Type
	{
		Enemy = 0,
		Projectile = 1,
		Object = 2
	}

	public Type hurtType;

	public bool OnCollision;

	public bool OnlyMachSpeed;

	private void OnTriggerStay(Collider collider)
	{
		if (!OnCollision)
		{
			PlayerBase player = GetPlayer(collider);
			if ((bool)player && !(player.GetState() == "Vehicle") && (!OnlyMachSpeed || (OnlyMachSpeed && player.GetPrefab("sonic_fast"))))
			{
				player.OnHurtEnter((int)hurtType);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (OnCollision)
		{
			PlayerBase player = GetPlayer(collision.transform);
			if ((bool)player && !(player.GetState() == "Vehicle") && (!OnlyMachSpeed || (OnlyMachSpeed && player.GetPrefab("sonic_fast"))))
			{
				player.OnHurtEnter((int)hurtType);
			}
		}
	}
}
