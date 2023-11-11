using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class CameraEffects : MonoBehaviour
{
	[Header("Framework")]
	public PlayerCamera Camera;

	public PostProcessVolume PostProcessVolume;

	public RadialBlur RadialBlur;

	public LegacyBloom Bloom;

	public Grayscale SlowdownEffect;

	public LegacyMotionBlur FrameBlendEffect;

	public Material UnderwaterEffect;

	public Material HeatEffect;

	public DesertHeat DesertHeat;

	public CameraWaterDrops CameraWaterDrops;

	public CameraTornadoFire CameraTornadoFire;

	public Color[] VignetteColors;

	internal bool IsOnSlowdown;

	internal bool IsOnChaosBoost;

	internal bool IsOnFullPower;

	private ChromaticAberration ChromAberLayer;

	private Vignette VignetteLayer;

	private SceneParameters SceneParameters;

	private bool IsUnderwater;

	private bool IsHeat;

	private float AudioPitch;

	private void Start()
	{
		SceneParameters = Object.FindObjectOfType<SceneParameters>();
		Bloom.bloomThreshold = SceneParameters.BloomThreshold;
		Bloom.sepBlurSpread = SceneParameters.BloomSpread;
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1)
		{
			PostProcessVolume.profile.TryGetSettings<ChromaticAberration>(out ChromAberLayer);
			PostProcessVolume.profile.TryGetSettings<Vignette>(out VignetteLayer);
		}
		UnderwaterEffect.SetFloat("_Intensity", 0f);
		HeatEffect.SetFloat("_Intensity", 0f);
		if (Camera.StageManager._Stage == StageManager.Stage.dtd && Camera.StageManager.StageSection == StageManager.Section.A && Singleton<Settings>.Instance.settings.VolumetricLights != 0)
		{
			DesertHeat.enabled = true;
		}
		AudioPitch = 1f;
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1 && Camera.StageManager._Stage == StageManager.Stage.kdv && GetRainBool())
		{
			CameraWaterDrops.enabled = true;
		}
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1 && Camera.StageManager._Stage == StageManager.Stage.csc && Camera.StageManager.StageSection == StageManager.Section.E)
		{
			CameraTornadoFire.enabled = true;
			CameraTornadoFire.Player = Camera.PlayerBase.transform;
			CameraTornadoFire.Tornado = Object.FindObjectOfType<TornadoChase>().transform;
		}
		string text = SceneManager.GetActiveScene().name;
		string text2 = text.Split("_"[0])[0];
		string text3 = text.Split("_"[0])[1];
		ColorGrading outSetting = null;
		PostProcessVolume.profile.TryGetSettings<ColorGrading>(out outSetting);
		if (!(outSetting != null))
		{
			return;
		}
		switch (text2)
		{
		case "wvo":
			outSetting.temperature.value = ((Singleton<GameManager>.Instance.GameStory == GameManager.Story.Shadow || Singleton<GameManager>.Instance.GameStory == GameManager.Story.Silver) ? 5f : 10f);
			outSetting.tint.value = ((Singleton<GameManager>.Instance.GameStory == GameManager.Story.Shadow || Singleton<GameManager>.Instance.GameStory == GameManager.Story.Silver) ? 5f : 0f);
			outSetting.postExposure.value = 0.5f;
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = 0f;
			break;
		case "dtd":
			outSetting.temperature.value = ((text3 == "b") ? (-20f) : 0f);
			outSetting.tint.value = 0f;
			outSetting.postExposure.value = 0.5f;
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = 0f;
			break;
		case "csc":
			outSetting.temperature.value = 0f;
			outSetting.tint.value = 10f;
			outSetting.postExposure.value = 0.5f;
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = 0f;
			break;
		case "flc":
			outSetting.temperature.value = 0f;
			outSetting.tint.value = 0f;
			outSetting.postExposure.value = ((text3 == "a") ? 1.25f : 1.5f);
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = ((text3 == "a") ? (-5f) : (-10f));
			break;
		case "aqa":
			if (Singleton<GameManager>.Instance.GameStory == GameManager.Story.Shadow)
			{
				outSetting.temperature.value = 0f;
				outSetting.tint.value = 25f;
				outSetting.postExposure.value = 0.5f;
				outSetting.saturation.value = -50f;
				outSetting.contrast.value = 10f;
			}
			else if (Singleton<GameManager>.Instance.GameStory == GameManager.Story.Silver)
			{
				outSetting.temperature.value = 25f;
				outSetting.tint.value = 20f;
				outSetting.postExposure.value = 0.5f;
				outSetting.saturation.value = -50f;
				outSetting.contrast.value = 10f;
			}
			else
			{
				outSetting.temperature.value = 0f;
				outSetting.tint.value = 0f;
				outSetting.postExposure.value = 0.5f;
				outSetting.saturation.value = 10f;
				outSetting.contrast.value = 0f;
			}
			break;
		case "test":
			outSetting.temperature.value = 0f;
			outSetting.tint.value = 0f;
			outSetting.postExposure.value = ((text3 == "b") ? 1.25f : 0.5f);
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = ((text3 == "b") ? (-5f) : 0f);
			break;
		default:
			outSetting.temperature.value = 0f;
			outSetting.tint.value = 0f;
			outSetting.postExposure.value = 0.5f;
			outSetting.saturation.value = 10f;
			outSetting.contrast.value = 0f;
			break;
		}
		if ((bool)SceneParameters.TrackballsSettings)
		{
			ColorGrading outSetting2 = null;
			SceneParameters.TrackballsSettings.TryGetSettings<ColorGrading>(out outSetting2);
			if (outSetting2 != null)
			{
				outSetting.lift.value = outSetting2.lift.value;
				outSetting.gamma.value = outSetting2.gamma.value;
				outSetting.gain.value = outSetting2.gain.value;
			}
		}
	}

	public bool GetRainBool()
	{
		bool result = false;
		if ((Singleton<GameManager>.Instance.GameStory == GameManager.Story.Sonic && (Camera.StageManager.StageSection == StageManager.Section.C || Camera.StageManager.StageSection == StageManager.Section.B)) || (Singleton<GameManager>.Instance.GameStory == GameManager.Story.Shadow && (Camera.StageManager.StageSection == StageManager.Section.A || Camera.StageManager.StageSection == StageManager.Section.D || Camera.StageManager.StageSection == StageManager.Section.B)))
		{
			result = true;
		}
		return result;
	}

	private float RadialInt()
	{
		if (Singleton<Settings>.Instance.settings.CameraType != 0)
		{
			return 0.05f;
		}
		return 0.25f;
	}

	private void Update()
	{
		if (RadialBlur.enabled)
		{
			RadialBlur.EffectAmount = (((Camera.CameraState == PlayerCamera.State.Event && Camera.parameters.Mode != 3 && Camera.parameters.Mode != 30 && Camera.parameters.Mode != 3 && Camera.parameters.Mode != 4 && Camera.parameters.Mode != 40 && Camera.parameters.Mode != 41 && Camera.parameters.Mode != 42 && Camera.parameters.Mode != 104) || Camera.CameraState == PlayerCamera.State.Normal) ? Mathf.Clamp((Camera.DistanceToTarget - Camera.Distance * ((Camera.CameraState == PlayerCamera.State.Event && (Camera.parameters.Mode == 5 || Camera.parameters.Mode == 50) && Camera.PlayerBase.GetPrefab("sonic_fast")) ? 4f : 1f)) * RadialInt(), 0f, 1f) : 0f);
		}
		if (Singleton<Settings>.Instance.settings.CameraLensFX == 1)
		{
			Collider[] array = Physics.OverlapSphere(Camera.transform.position, 0.01f);
			for (int i = 0; i < array.Length; i++)
			{
				if (array != null && array[i].gameObject.layer == LayerMask.NameToLayer("CameraEffects"))
				{
					if (array[i].gameObject.tag == "CameraWater")
					{
						IsUnderwater = true;
					}
					if (array[i].gameObject.tag == "CameraHeat")
					{
						IsHeat = true;
					}
				}
			}
			if (!IsUnderwater)
			{
				UnderwaterEffect.SetFloat("_Intensity", Mathf.MoveTowards(UnderwaterEffect.GetFloat("_Intensity"), 0f, Time.deltaTime * 6f));
			}
			else
			{
				UnderwaterEffect.SetFloat("_Intensity", Mathf.MoveTowards(UnderwaterEffect.GetFloat("_Intensity"), 1f, Time.deltaTime * 6f));
				IsUnderwater = false;
			}
			if (!IsHeat)
			{
				HeatEffect.SetFloat("_Intensity", Mathf.MoveTowards(HeatEffect.GetFloat("_Intensity"), 0f, Time.deltaTime * 0.5f));
			}
			else
			{
				HeatEffect.SetFloat("_Intensity", Mathf.MoveTowards(HeatEffect.GetFloat("_Intensity"), 0.5f, Time.deltaTime * 0.5f));
				IsHeat = false;
			}
			if (ChromAberLayer != null)
			{
				ChromAberLayer.intensity.value = Mathf.Lerp(ChromAberLayer.intensity.value, (!IsOnSlowdown && !IsOnChaosBoost) ? 0f : (Camera.PlayerBase.GetPrefab("sonic_new") ? 1f : Random.Range(0.5f, 0.2f)), Time.deltaTime * (Camera.PlayerBase.GetPrefab("sonic_new") ? 3f : 4f));
			}
			if (VignetteLayer != null)
			{
				VignetteLayer.intensity.value = Mathf.Lerp(VignetteLayer.intensity.value, IsOnChaosBoost ? Random.Range(0.4f, 0.3f) : 0f, Time.deltaTime * 4f);
				VignetteLayer.color.value = Color.Lerp(VignetteLayer.color.value, VignetteColors[IsOnFullPower ? 1 : 0], Time.deltaTime * 4f);
			}
		}
		SlowdownEffect.Intensity = Mathf.MoveTowards(SlowdownEffect.Intensity, IsOnSlowdown ? 0.5f : 0f, Time.unscaledDeltaTime * 3f);
		FrameBlendEffect.blurAmount = Mathf.MoveTowards(FrameBlendEffect.blurAmount, IsOnSlowdown ? 0.5f : 0f, Time.unscaledDeltaTime * 3f);
		if (Singleton<GameManager>.Instance.GameState != GameManager.State.Paused)
		{
			Time.timeScale = Mathf.MoveTowards(Time.timeScale, IsOnSlowdown ? 0.5f : 1f, Time.unscaledDeltaTime * 3f);
			AudioPitch = Mathf.MoveTowards(AudioPitch, IsOnSlowdown ? 0.75f : 1f, Time.unscaledDeltaTime * 3f);
		}
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("MusicPitch", AudioPitch);
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("SoundsPitch", AudioPitch);
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("VoicesPitch", AudioPitch);
		if (!Camera.PlayerBase.GetPrefab("sonic_new") || Singleton<GameManager>.Instance.GameState == GameManager.State.Result)
		{
			IsOnSlowdown = false;
		}
		if (!Camera.PlayerBase.GetPrefab("shadow") || Singleton<GameManager>.Instance.GameState == GameManager.State.Result)
		{
			IsOnChaosBoost = false;
		}
		if (CameraWaterDrops.enabled)
		{
			CameraWaterDrops.HeavyRain = (Camera.StageManager._Stage == StageManager.Stage.kdv && Camera.StageManager.StageSection == StageManager.Section.C) || (Camera.StageManager._Stage == StageManager.Stage.other && Camera.StageManager.StageSection == StageManager.Section.A);
		}
	}
}
