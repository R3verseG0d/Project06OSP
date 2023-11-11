using UnityEngine;

public class TropicalWeather : MonoBehaviour
{
	[Header("Framework")]
	public Light DirLight;

	public Light SecLight;

	public Renderer Skybox1;

	public Renderer Skybox2;

	public Renderer Rainbow;

	public LensFlare SunFlare;

	public ParticleSystem RainFX;

	public Renderer OptionalStageRenderer;

	public AudioSource[] RainSources;

	[Header("Settings")]
	public Color DirLightColor;

	public float DirLightInt;

	public float DirLightShadowInt;

	public Color SecLightColor;

	public float SecLightInt;

	[ColorUsage(false, true)]
	public Color AmbientColor;

	public Color FogColor;

	public float FogDensity;

	public float SunFlareInt;

	public float OptionalStageRendererInt;

	public float RainVol;

	public Vector2 SunnySwitchTime;

	public Vector2 RainSwitchTime;

	private MaterialPropertyBlock PropBlock;

	private PlayerBase Player;

	private bool Switched;

	private float StartTime;

	private Color OrigDirLightColor;

	private float OrigDirLightInt;

	private float OrigDirLightShadowInt;

	private Color OrigSecLightColor;

	private float OrigSecLightInt;

	[ColorUsage(false, true)]
	private Color OrigAmbientColor;

	private Color OrigFogColor;

	private float OrigFogDensity;

	private float OrigSunFlareInt;

	private float OrigOptionalStageRendererInt;

	private Color32 RainbowColor;

	private float Sky1Alpha;

	private float Sky2Alpha;

	private float RainbowAlpha;

	private float LerpFactor;

	private void Start()
	{
		Player = Object.FindObjectOfType<PlayerBase>();
		PropBlock = new MaterialPropertyBlock();
		StartTime = Time.time;
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1)
		{
			Player.Camera.CameraEffects.CameraWaterDrops.enabled = true;
			Player.Camera.CameraEffects.CameraWaterDrops.HeavyRain = true;
		}
		GrabSettings();
		if (Random.value > 0.5f)
		{
			Switched = true;
			LerpFactor = 1f;
			AggressiveSwitch();
		}
		else
		{
			RainFX.Clear();
		}
	}

	private void Update()
	{
		if (!Player)
		{
			Player = Object.FindObjectOfType<PlayerBase>();
		}
		if (!Switched && Time.time - StartTime > Random.Range(SunnySwitchTime.x, SunnySwitchTime.y))
		{
			StartTime = Time.time;
			Switched = true;
		}
		else if (Switched && Time.time - StartTime > Random.Range(RainSwitchTime.x, RainSwitchTime.y))
		{
			PlayRain(Enable: false);
			StartTime = Time.time;
			Switched = false;
		}
		if (Switched && !RainFX.emission.enabled && Time.time - StartTime > 1.5f)
		{
			PlayRain(Enable: true);
		}
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1 && (!Switched || (Switched && Time.time - StartTime < 1.5f)))
		{
			Player.Camera.CameraEffects.CameraWaterDrops.IsBlocked = true;
		}
		LerpFactor = Mathf.MoveTowards(LerpFactor, Switched ? 1f : 0f, Time.deltaTime * 0.25f);
		DirLight.color = Color.Lerp(OrigDirLightColor, DirLightColor, LerpFactor);
		DirLight.intensity = Mathf.Lerp(OrigDirLightInt, DirLightInt, LerpFactor);
		DirLight.shadowStrength = Mathf.Lerp(OrigDirLightShadowInt, DirLightShadowInt, LerpFactor);
		SecLight.color = Color.Lerp(OrigSecLightColor, SecLightColor, LerpFactor);
		SecLight.intensity = Mathf.Lerp(OrigSecLightInt, SecLightInt, LerpFactor);
		RenderSettings.ambientLight = Color.Lerp(OrigAmbientColor, AmbientColor, LerpFactor);
		RenderSettings.fogColor = Color.Lerp(OrigFogColor, FogColor, LerpFactor);
		RenderSettings.fogDensity = Mathf.Lerp(OrigFogDensity, FogDensity, LerpFactor);
		SunFlare.brightness = Mathf.Lerp(OrigSunFlareInt, SunFlareInt, LerpFactor);
		Sky1Alpha = Mathf.Lerp(1f, 0f, LerpFactor);
		Sky2Alpha = Mathf.Lerp(0f, 1f, LerpFactor);
		RainbowAlpha = Mathf.Lerp(96f, 0f, LerpFactor);
		SetRainVolume(Mathf.Lerp(0f, RainVol, LerpFactor));
		Skybox1.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Alpha", Sky1Alpha);
		Skybox1.SetPropertyBlock(PropBlock);
		Skybox2.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Alpha", Sky2Alpha);
		Skybox2.SetPropertyBlock(PropBlock);
		Rainbow.GetPropertyBlock(PropBlock);
		RainbowColor.a = (byte)RainbowAlpha;
		PropBlock.SetColor("_TintColor", RainbowColor);
		Rainbow.SetPropertyBlock(PropBlock);
		if ((bool)OptionalStageRenderer)
		{
			OptionalStageRenderer.GetPropertyBlock(PropBlock);
			PropBlock.SetFloat("_Intensity", Mathf.Lerp(OrigOptionalStageRendererInt, OptionalStageRendererInt, LerpFactor));
			OptionalStageRenderer.SetPropertyBlock(PropBlock);
		}
	}

	private void GrabSettings()
	{
		OrigDirLightColor = DirLight.color;
		OrigDirLightInt = DirLight.intensity;
		OrigDirLightShadowInt = DirLight.shadowStrength;
		OrigSecLightColor = SecLight.color;
		OrigSecLightInt = SecLight.intensity;
		OrigAmbientColor = RenderSettings.ambientLight;
		OrigFogColor = RenderSettings.fogColor;
		OrigFogDensity = RenderSettings.fogDensity;
		OrigSunFlareInt = SunFlare.brightness;
		if ((bool)OptionalStageRenderer)
		{
			OrigOptionalStageRendererInt = 1f;
		}
		Sky1Alpha = 1f;
		Sky2Alpha = 0f;
		RainbowColor = new Color32(128, 128, 128, 48);
		PlayRain(Enable: false);
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1)
		{
			Player.Camera.CameraEffects.CameraWaterDrops.IsBlocked = true;
		}
		SetRainVolume(0f);
	}

	private void SetRainVolume(float Vol)
	{
		for (int i = 0; i < RainSources.Length; i++)
		{
			RainSources[i].volume = Vol;
		}
	}

	private void PlayRain(bool Enable)
	{
		ParticleSystem.EmissionModule emission = RainFX.emission;
		emission.enabled = Enable;
	}

	private void AggressiveSwitch()
	{
		DirLight.color = DirLightColor;
		DirLight.intensity = DirLightInt;
		DirLight.shadowStrength = DirLightShadowInt;
		SecLight.color = SecLightColor;
		SecLight.intensity = SecLightInt;
		RenderSettings.ambientLight = AmbientColor;
		RenderSettings.fogColor = FogColor;
		RenderSettings.fogDensity = FogDensity;
		SunFlare.brightness = SunFlareInt;
		if ((bool)OptionalStageRenderer)
		{
			OptionalStageRenderer.GetPropertyBlock(PropBlock);
			PropBlock.SetFloat("_Intensity", OptionalStageRendererInt);
			OptionalStageRenderer.SetPropertyBlock(PropBlock);
		}
		Sky1Alpha = 0f;
		Sky2Alpha = 1f;
		RainbowColor = new Color32(128, 128, 128, 0);
		PlayRain(Enable: true);
		SetRainVolume(1f);
	}
}
