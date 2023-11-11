using UnityEngine;

public class WallAlign : ObjectBase
{
	private void OnCollisionStay(Collision collision)
	{
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && !player.IsDead && !(player.GetState() == "Ground") && !(player.GetState() == "WallSlam") && !(player.GetState() == "AirSlam") && !(player.GetState() == "ChainJump") && !player.IsGrounded() && (Physics.Raycast(player.transform.position + player.transform.up * 0.5f, player.transform.right, out var hitInfo, 1.75f) || Physics.Raycast(player.transform.position + player.transform.up * 0.5f, -player.transform.right, out hitInfo, 1.75f)))
		{
			player.transform.rotation = Quaternion.FromToRotation(player.transform.up, hitInfo.normal) * player.transform.rotation;
			player.SetMachineState("StateGround");
		}
	}
}
