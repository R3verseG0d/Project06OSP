using UnityEngine;

public class TownsGoal : ObjectBase
{
	private bool Triggered;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && !Triggered)
		{
			player.OnGoal();
			GoalRing goalRing = Object.FindObjectOfType<GoalRing>();
			if ((bool)goalRing)
			{
				goalRing.ActivateGoal();
			}
			Triggered = true;
		}
	}
}
