using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class Settings : Singleton<Settings>
{
	[Serializable]
	public class GameSettings
	{
		public int DisplayMode = 1;

		public int Resolution = -1;

		public int RunInBG = 1;

		public int AntiAliasing = 2;

		public int AnisotropicFiltering = 2;

		public int VSync = 1;

		public int DrawDistance = 4;

		public int TextureQuality = 3;

		public int CameraLensFX = 1;

		public int BlurFX = 2;

		public int Bloom = 1;

		public int Outlines = 1;

		public int VolumetricLights = 2;

		public int Reflections = 3;

		public int ShadowQuality = 4;

		public int ShadowType = 1;

		public int ShadowDistance = 4;

		public int ShadowCascades = 2;

		public int InvertCamX = 1;

		public int InvertCamY = 1;

		public int InvertGliderY = 1;

		public int Dialogue = 1;

		public int Cutscenes = 1;

		public int NoCameraVolumes;

		public int Hints = 1;

		public string AudioLanguage = "e";

		public int ButtonIcons;

		public int BGVideo = 7;

		public int CharacterSway = 1;

		public int CameraLeaning = 1;

		public int JiggleBones = 1;

		public int DisplayType;

		public int TextBoxType;

		public int ItemBoxType;

		public int PauseMenuType;

		public int EnemyHealthType;

		public int LoadingScreenType;

		public int UIPreset;

		public float MusicVolume = 0.6f;

		public float SEVolume = 0.6f;

		public float VoiceVolume = 0.6f;

		public int TGSSonic;

		public int CameraType;

		public int HomingReticle;

		public int AttackReticles = 1;

		public int SpinEffect;

		public int JumpdashType;

		public int GemShoesType;

		public int UpgradeModels;

		public int E3XBLAMusic;
	}

	public Resolution[] AvailableResolutions;

	public GameSettings settings = new GameSettings();

	internal string[] LanguageArray = new string[7] { "j", "e", "f", "g", "s", "i", "r" };

	private static string settingsPath
	{
		get
		{
			return Application.dataPath + "/Settings.bin";
		}
		set
		{
			settingsPath = value;
		}
	}

	protected Settings()
	{
	}

	private void Awake()
	{
		LoadSettings();
		AvailableResolutions = Screen.resolutions;
		if (settings.Resolution == -1)
		{
			settings.Resolution = AvailableResolutions.Length - 1;
		}
		SetGlobalSettings();
	}

	public void LoadSettings()
	{
		if (File.Exists(settingsPath))
		{
			FileStream fileStream = new FileStream(settingsPath, FileMode.Open);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			settings = binaryFormatter.Deserialize(fileStream) as GameSettings;
			fileStream.Close();
		}
	}

	public void SaveSettings()
	{
		FileStream fileStream = File.Create(settingsPath);
		new BinaryFormatter().Serialize(fileStream, settings);
		fileStream.Close();
	}

	public string AudioLanguage()
	{
		return settings.AudioLanguage;
	}

	public void SetGlobalSettings()
	{
		Resolution resolution = AvailableResolutions[settings.Resolution];
		Screen.SetResolution(resolution.width, resolution.height, (settings.DisplayMode <= 0) ? FullScreenMode.Windowed : ((settings.DisplayMode <= 1) ? FullScreenMode.FullScreenWindow : FullScreenMode.ExclusiveFullScreen), resolution.refreshRate);
		Application.runInBackground = settings.RunInBG == 1;
		QualitySettings.shadows = ((settings.ShadowQuality != 0) ? ShadowQuality.HardOnly : ShadowQuality.Disable);
		QualitySettings.shadowResolution = (ShadowResolution)(settings.ShadowQuality - 1);
		QualitySettings.shadowCascades = settings.ShadowCascades * 2;
		switch (settings.ShadowDistance)
		{
		case 0:
			QualitySettings.shadowDistance = 50f;
			break;
		case 1:
			QualitySettings.shadowDistance = 100f;
			break;
		case 2:
			QualitySettings.shadowDistance = 175f;
			break;
		case 3:
			QualitySettings.shadowDistance = 275f;
			break;
		case 4:
			QualitySettings.shadowDistance = 400f;
			break;
		}
		switch (settings.TextureQuality)
		{
		case 0:
			QualitySettings.masterTextureLimit = 3;
			break;
		case 1:
			QualitySettings.masterTextureLimit = 2;
			break;
		case 2:
			QualitySettings.masterTextureLimit = 1;
			break;
		case 3:
			QualitySettings.masterTextureLimit = 0;
			break;
		}
		QualitySettings.anisotropicFiltering = (AnisotropicFiltering)settings.AnisotropicFiltering;
		QualitySettings.vSyncCount = settings.VSync;
	}

	public static void SetLocalSettings()
	{
		GameSettings gameSettings = Singleton<Settings>.Instance.settings;
		if (gameSettings.ShadowType == 0)
		{
			NGSS_Directional nGSS_Directional = UnityEngine.Object.FindObjectOfType<NGSS_Directional>();
			if (nGSS_Directional != null)
			{
				nGSS_Directional.NGSS_SHADOWS_SOFTNESS = 0f;
				nGSS_Directional.NGSS_PCSS_ENABLED = false;
				nGSS_Directional.NGSS_DENOISER_ENABLED = false;
			}
		}
		SurfaceReflection[] array = UnityEngine.Object.FindObjectsOfType<SurfaceReflection>();
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = gameSettings.Reflections > 0;
				if (gameSettings.Reflections > 0)
				{
					array[i].m_TextureSize = gameSettings.Reflections;
				}
			}
			if (gameSettings.Reflections > 0)
			{
				Shader.EnableKeyword("PLANARREF");
			}
			else
			{
				Shader.DisableKeyword("PLANARREF");
			}
		}
		SurfaceReflectionArea surfaceReflectionArea = UnityEngine.Object.FindObjectOfType<SurfaceReflectionArea>();
		if (surfaceReflectionArea != null && gameSettings.Reflections > 0)
		{
			surfaceReflectionArea.m_TextureSize = gameSettings.Reflections;
		}
		Camera main = Camera.main;
		switch (gameSettings.DrawDistance)
		{
		case 0:
			main.farClipPlane = 250f;
			break;
		case 1:
			main.farClipPlane = 500f;
			break;
		case 2:
			main.farClipPlane = 1000f;
			break;
		case 3:
			main.farClipPlane = 1500f;
			break;
		case 4:
			main.farClipPlane = 4500f;
			break;
		}
		PostProcessLayer postProcessLayer = UnityEngine.Object.FindObjectOfType<PostProcessLayer>();
		if ((bool)postProcessLayer)
		{
			switch (gameSettings.AntiAliasing)
			{
			case 0:
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
				break;
			case 1:
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				break;
			case 2:
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
				break;
			case 3:
				postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				break;
			}
		}
		PostProcessVolume postProcessVolume = UnityEngine.Object.FindObjectOfType<PostProcessVolume>();
		if ((bool)postProcessVolume)
		{
			MotionBlur outSetting = null;
			postProcessVolume.profile.TryGetSettings<MotionBlur>(out outSetting);
			if (outSetting != null)
			{
				outSetting.enabled.value = gameSettings.BlurFX == 2;
			}
			ChromaticAberration outSetting2 = null;
			postProcessVolume.profile.TryGetSettings<ChromaticAberration>(out outSetting2);
			if (outSetting2 != null)
			{
				outSetting2.enabled.value = gameSettings.CameraLensFX == 1;
			}
			Vignette outSetting3 = null;
			postProcessVolume.profile.TryGetSettings<Vignette>(out outSetting3);
			if (outSetting3 != null)
			{
				outSetting3.enabled.value = gameSettings.CameraLensFX == 1;
			}
			Bloom outSetting4 = null;
			postProcessVolume.profile.TryGetSettings<Bloom>(out outSetting4);
			if (outSetting4 != null)
			{
				outSetting4.enabled.value = gameSettings.Bloom == 1;
			}
		}
		RadialBlur radialBlur = UnityEngine.Object.FindObjectOfType<RadialBlur>();
		if ((bool)radialBlur)
		{
			radialBlur.enabled = gameSettings.BlurFX == 1;
		}
		LegacyBloom legacyBloom = UnityEngine.Object.FindObjectOfType<LegacyBloom>();
		if ((bool)legacyBloom)
		{
			legacyBloom.enabled = gameSettings.Bloom == 1;
		}
		GlowComposite glowComposite = UnityEngine.Object.FindObjectOfType<GlowComposite>();
		if ((bool)glowComposite)
		{
			glowComposite.enabled = gameSettings.Outlines == 1;
			GameObject gameObject = UnityEngine.Object.FindObjectOfType<GlowPrePass>().gameObject;
			if ((bool)gameObject && gameSettings.Outlines == 0)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		VolumetricLightRenderer volumetricLightRenderer = UnityEngine.Object.FindObjectOfType<VolumetricLightRenderer>();
		if ((bool)volumetricLightRenderer)
		{
			switch (gameSettings.VolumetricLights)
			{
			case 1:
				volumetricLightRenderer.Resolution = VolumetricLightRenderer.VolumtericResolution.Quarter;
				break;
			case 2:
				volumetricLightRenderer.Resolution = VolumetricLightRenderer.VolumtericResolution.Half;
				break;
			default:
				volumetricLightRenderer.Resolution = VolumetricLightRenderer.VolumtericResolution.Quarter;
				break;
			}
			volumetricLightRenderer.enabled = gameSettings.VolumetricLights > 0;
		}
	}
}
