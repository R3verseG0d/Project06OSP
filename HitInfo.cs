using UnityEngine;

public class HitInfo
{
	public Transform player;

	public Vector3 force;

	public int damage;

	public HitInfo(Transform Player, Vector3 Force, int Damage = 1)
	{
		player = Player;
		force = Force;
		damage = Damage;
	}
}
