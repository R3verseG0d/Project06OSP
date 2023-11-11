using UnityEngine;

public class GlassBuild : MonoBehaviour
{
	[Header("Framework")]
	public float Time;

	[Header("Prefab")]
	public GameObject ModelObj;

	public Collider HurtCollider;

	public Light Light;

	public ParticleSystem[] FX;

	private bool IsTriggered;

	private bool Explode;

	private bool Fire;

	private float StartTime;

	public void SetParameters(float _Time)
	{
		Time = _Time;
	}

	private void Update()
	{
		if (!IsTriggered)
		{
			return;
		}
		if (!Explode && UnityEngine.Time.time - StartTime > Time)
		{
			ModelObj.SetActive(value: true);
			Explode = true;
		}
		if (!Fire && UnityEngine.Time.time - StartTime > Time * 2f)
		{
			HurtCollider.enabled = true;
			for (int i = 0; i < FX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = FX[i].emission;
				emission.enabled = true;
			}
			Fire = true;
		}
		if (Fire)
		{
			Light.intensity = Mathf.Lerp(2f, 4f, Mathf.Abs(Mathf.Cos(UnityEngine.Time.time * 15f)));
		}
	}

	private void OnEventSignal()
	{
		if (!IsTriggered)
		{
			StartTime = UnityEngine.Time.time;
			IsTriggered = true;
		}
	}
}
