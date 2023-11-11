using UnityEngine;

public class RingDrain : ObjectBase
{
	public enum Type
	{
		FromToPlayer = 0,
		Random = 1
	}

	public Type DrainType;

	private void OnTriggerStay(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player)
		{
			player.OnBulletHit((DrainType == Type.FromToPlayer) ? (player.transform.position - base.transform.position) : new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
		}
	}
}
