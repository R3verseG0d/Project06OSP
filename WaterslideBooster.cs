using UnityEngine;

public class WaterslideBooster : ObjectBase
{
	[Header("Framework")]
	public float Speed;

	[Header("Prefab")]
	public ParticleSystem[] Particles;

	public GameObject FX;

	private bool IsTriggered;

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!IsTriggered && (bool)player && !player.IsDead && (player.GetPrefab("sonic_new") || player.GetPrefab("metal_sonic")))
		{
			Swoosh();
			player.OnWaterSlideEnter("", TriggerState: false, Speed);
		}
	}

	private void Swoosh()
	{
		FX.SetActive(value: true);
		for (int i = 0; i < Particles.Length; i++)
		{
			Particles[i].Stop();
		}
		Object.Destroy(base.gameObject, 1f);
		IsTriggered = true;
	}
}
