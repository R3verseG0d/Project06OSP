using UnityEngine;

public class Csc_SuperSpeedPassage : MonoBehaviour
{
	private PlayerBase Player;

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			if (!Player)
			{
				Player = collider.gameObject.GetComponent<PlayerBase>();
			}
			if (Player.GetState() != "LightDash" && Player.GetState() != "Path" && Player.GetState() != "WallSlam" && Player.GetState() != "AirSlam")
			{
				Player.LockControls = true;
				Player.transform.forward = base.transform.forward;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			Player = null;
		}
	}
}
