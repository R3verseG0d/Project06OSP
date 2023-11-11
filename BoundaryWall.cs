using UnityEngine;

public class BoundaryWall : MonoBehaviour
{
	public Material Material;

	public float ScaleFactor = 5f;

	private Transform Player;

	private void Update()
	{
		if (!Player)
		{
			Player = Object.FindObjectOfType<PlayerBase>().transform;
		}
		Material.SetVector("_PlayerPos", Player.position);
		Material.SetTextureScale("_MainTex", new Vector2(base.transform.localScale.x / ScaleFactor, base.transform.localScale.z / ScaleFactor));
		Material.SetTextureScale("_Noise", new Vector2(base.transform.localScale.x / ScaleFactor, base.transform.localScale.z / ScaleFactor));
	}
}
