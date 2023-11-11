using UnityEngine;

public class Cycad : ObjectBase
{
	private void OnCollisionEnter(Collision collision)
	{
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && player.GetPrefab("sonic_fast") && player.CurSpeed >= 30f)
		{
			player.OnHurtEnter(2);
		}
	}
}
