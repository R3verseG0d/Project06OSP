using UnityEngine;

public class LightningFlash : MonoBehaviour
{
	public Light DirectionalLight;

	public Renderer Renderer;

	public Renderer[] ExtraRenderers;

	public Color NormalColor;

	public Color FlashColor;

	public ParticleSystem LightningFX;

	public AudioClip[] LightningSounds;

	public float LightningIntensity;

	public float LoopTime;

	public float[] ExecTimes;

	public AnimationCurve[] SkyFlashPatterns;

	public AnimationCurve[] LightFlashPatterns;

	private float StartLightInt;

	private float LastTime;

	private float PatternTime;

	private int RandomFlashIndex;

	private void Start()
	{
		if (!Renderer)
		{
			return;
		}
		Renderer.material.SetColor("_TintColor", NormalColor);
		if (ExtraRenderers != null)
		{
			for (int i = 0; i < ExtraRenderers.Length; i++)
			{
				ExtraRenderers[i].material.SetFloat("_TransInt", 0f);
			}
		}
		StartLightInt = DirectionalLight.intensity;
		PatternTime = 1f;
	}

	private void Update()
	{
		PatternTime += Time.deltaTime;
		if (Time.time - LastTime > LoopTime)
		{
			LastTime = Time.time;
			RandomFlashIndex = Random.Range(0, 3);
			PatternTime = 0f;
			LoopTime = ExecTimes[Random.Range(0, ExecTimes.Length)];
			Singleton<AudioManager>.Instance.PlayClip(LightningSounds[Random.Range(0, LightningSounds.Length)], 0.75f);
			if ((bool)LightningFX)
			{
				LightningFX.Play();
			}
		}
		if (!Renderer)
		{
			return;
		}
		Renderer.material.SetColor("_TintColor", Color.Lerp(NormalColor, FlashColor, SkyFlashPatterns[RandomFlashIndex].Evaluate(PatternTime)));
		DirectionalLight.intensity = Mathf.Lerp(StartLightInt, LightningIntensity, LightFlashPatterns[RandomFlashIndex].Evaluate(PatternTime));
		if (ExtraRenderers != null)
		{
			for (int i = 0; i < ExtraRenderers.Length; i++)
			{
				ExtraRenderers[i].material.SetFloat("_TransInt", LightFlashPatterns[RandomFlashIndex].Evaluate(PatternTime));
			}
		}
	}
}
