using UnityEngine;

public class RailCollision : ObjectBase
{
	public enum Type
	{
		Metal = 0,
		Wind = 1,
		Nature = 2
	}

	public RailSystem BezierScript;

	public Type RailType;

	public bool Unswitchable;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead)
		{
			player.OnRailEnter(BezierScript, (int)RailType);
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead)
		{
			player.OnRailEnter(BezierScript, (int)RailType);
		}
	}
}
