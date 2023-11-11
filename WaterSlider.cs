using UnityEngine;

public class WaterSlider : ObjectBase
{
	[Header("Framework")]
	public string Path;

	public float Radius;

	public void SetParameters(string _Path, float _Radius)
	{
		Path = _Path;
		Radius = _Radius;
	}

	private void Start()
	{
		GetComponent<SphereCollider>().radius = Radius;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!(player == null) && !player.IsDead && (player.GetPrefab("sonic_new") || player.GetPrefab("metal_sonic")))
		{
			player.OnWaterSlideEnter(Path);
		}
	}
}
