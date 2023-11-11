using UnityEngine;

public class CameraWaterDrops : MonoBehaviour
{
	[Header("Framework")]
	public PlayerCamera Camera;

	public ParticleSystem MainFX;

	public ParticleSystem[] FX;

	internal bool HeavyRain;

	internal bool IsBlocked;

	private float ScaledSpeed;

	private void Start()
	{
		MainFX.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = !IsBlocked;
		}
		if (IsBlocked)
		{
			IsBlocked = false;
		}
		if ((bool)Camera && MainFX.gameObject.activeSelf)
		{
			ScaledSpeed = ((Singleton<GameManager>.Instance.GameState != GameManager.State.Result) ? Mathf.Clamp(Camera.DistanceToTarget - Camera.Distance, 0f, 0.75f) : 0f);
			ParticleSystem.MainModule main = MainFX.main;
			main.startSpeed = ScaledSpeed;
			ParticleSystem.EmissionModule emission2 = MainFX.emission;
			ParticleSystem.EmissionModule emission3 = FX[1].emission;
			if (emission2.enabled)
			{
				emission2.rateOverTime = Mathf.Lerp(HeavyRain ? 25f : 10f, HeavyRain ? 75f : 35f, ScaledSpeed);
			}
			if (emission3.enabled)
			{
				emission3.rateOverTime = Mathf.Lerp(HeavyRain ? 8f : 4f, HeavyRain ? 18f : 6f, ScaledSpeed);
			}
		}
	}
}
