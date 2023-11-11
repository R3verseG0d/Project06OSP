using UnityEngine;

public class ObjectHuntingManager : ObjectBase
{
	[Header("Framework")]
	public Common_Key Obj1;

	public Common_Key Obj2;

	public Common_Key Obj3;

	public Vector3[] Obj1Pos;

	public Vector3[] Obj2Pos;

	public Vector3[] Obj3Pos;

	[Header("Prefab")]
	public Collider Collider;

	private float CollectTime;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !player.HUD.UseRadar && (player.GetPrefab("knuckles") || player.GetPrefab("rouge")))
		{
			Obj1.transform.position = Obj1Pos[Random.Range(0, Obj1Pos.Length)];
			Obj1.Manager = this;
			Obj2.transform.position = Obj2Pos[Random.Range(0, Obj2Pos.Length)];
			Obj2.Manager = this;
			Obj3.transform.position = Obj3Pos[Random.Range(0, Obj3Pos.Length)];
			Obj3.Manager = this;
			player.HUD.OpenRadar(player.transform, Obj1, Obj2, Obj3);
			CollectTime = Time.time;
			Collider.enabled = false;
		}
	}

	public void AddScore(PlayerBase Player)
	{
		float num = Time.time - CollectTime;
		if (num < 15f)
		{
			Player.AddScore(300, InstantChain: true, 10);
		}
		else if (num < 20f)
		{
			Player.AddScore(250, InstantChain: true, 9);
		}
		else if (num < 27.5f)
		{
			Player.AddScore(190, InstantChain: true, 8);
		}
		else if (num < 37.5f)
		{
			Player.AddScore(150, InstantChain: true, 5);
		}
		else
		{
			Player.AddScore(75, InstantChain: true, 2);
		}
		Player.ComboTime = 1.1f;
		CollectTime = Time.time;
	}

	private void OnDrawGizmosSelected()
	{
		if (Obj1Pos != null)
		{
			Gizmos.color = Color.red;
			for (int i = 0; i < Obj1Pos.Length; i++)
			{
				Gizmos.DrawWireSphere(Obj1Pos[i], 0.75f);
			}
		}
		if (Obj2Pos != null)
		{
			Gizmos.color = Color.green;
			for (int j = 0; j < Obj2Pos.Length; j++)
			{
				Gizmos.DrawWireSphere(Obj2Pos[j], 0.75f);
			}
		}
		if (Obj3Pos != null)
		{
			Gizmos.color = Color.blue;
			for (int k = 0; k < Obj3Pos.Length; k++)
			{
				Gizmos.DrawWireSphere(Obj3Pos[k], 0.75f);
			}
		}
	}
}
