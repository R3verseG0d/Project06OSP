using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Light))]
[ExecuteInEditMode]
public class NGSS_Directional : MonoBehaviour
{
	public enum ShadowMapResolution
	{
		UseQualitySettings = 0x100,
		VeryLow = 0x200,
		Low = 0x400,
		Med = 0x800,
		High = 0x1000,
		Ultra = 0x2000,
		Mega = 0x4000
	}

	[Header("MAIN SETTINGS")]
	[Tooltip("If disabled, NGSS Directional shadows replacement will be removed from Graphics settings when OnDisable is called in this component.")]
	public bool NGSS_KEEP_ONDISABLE = true;

	[Tooltip("Check this option if you don't need to update shadows variables at runtime, only once when scene loads.\nUseful to save some CPU cycles.")]
	public bool NGSS_NO_UPDATE_ON_PLAY;

	[Tooltip("Shadows resolution.\nUseQualitySettings = From Quality Settings, SuperLow = 512, Low = 1024, Med = 2048, High = 4096, Ultra = 8192, Mega = 16384.")]
	public ShadowMapResolution NGSS_SHADOWS_RESOLUTION = ShadowMapResolution.UseQualitySettings;

	[Header("BASE SAMPLING")]
	[Tooltip("Used to test blocker search and early bail out algorithms. Keep it as low as possible, might lead to white noise if too low.\nRecommended values: Mobile = 8, Consoles & VR = 16, Desktop = 24")]
	[Range(4f, 32f)]
	public int NGSS_SAMPLING_TEST = 16;

	[Tooltip("Number of samplers per pixel used for PCF and PCSS shadows algorithms.\nRecommended values: Mobile = 16, Consoles & VR = 32, Desktop Med = 48, Desktop High = 64, Desktop Ultra = 128")]
	[Range(8f, 128f)]
	public int NGSS_SAMPLING_FILTER = 48;

	[Tooltip("New optimization that reduces sampling over distance. Interpolates current sampling set (TEST and FILTER) down to 4spp when reaching this distance.")]
	[Range(0f, 500f)]
	public float NGSS_SAMPLING_DISTANCE = 75f;

	[Header("SHADOW SOFTNESS")]
	[Tooltip("Overall shadows softness.")]
	[Range(0f, 3f)]
	public float NGSS_SHADOWS_SOFTNESS = 1f;

	[Header("PCSS")]
	[Tooltip("PCSS Requires inline sampling and SM3.5.\nProvides Area Light soft-shadows.\nDisable it if you are looking for PCF filtering (uniform soft-shadows) which runs with SM3.0.")]
	public bool NGSS_PCSS_ENABLED;

	[Tooltip("How soft shadows are when close to caster.")]
	[Range(0f, 2f)]
	public float NGSS_PCSS_SOFTNESS_NEAR = 0.125f;

	[Tooltip("How soft shadows are when far from caster.")]
	[Range(0f, 2f)]
	public float NGSS_PCSS_SOFTNESS_FAR = 1f;

	[Header("NOISE")]
	[Tooltip("If zero = 100% noise.\nIf one = 100% dithering.\nUseful when fighting banding.")]
	[Range(0f, 1f)]
	public int NGSS_NOISE_TO_DITHERING_SCALE;

	[Tooltip("If you set the noise scale value to something less than 1 you need to input a noise texture.\nRecommended noise textures are blue noise signals.")]
	public Texture2D NGSS_NOISE_TEXTURE;

	[Header("DENOISER")]
	[Tooltip("Separable low pass filter that help fight artifacts and noise in shadows.\nRequires NGSS Shadows Libraries to be installed and Cascaded Shadows to be enabled in the Editor Graphics Settings.")]
	public bool NGSS_DENOISER_ENABLED = true;

	[Tooltip("How many iterations the Denoiser algorithm should do.")]
	[Range(0f, 4f)]
	public int NGSS_DENOISER_ITERATIONS = 2;

	[Tooltip("Overall Denoiser softness.")]
	[Range(0f, 1f)]
	public float NGSS_DENOISER_BLUR = 0.25f;

	[Tooltip("The amount of shadow edges the Denoiser can tolerate during denoising.")]
	[Range(0.05f, 1f)]
	public float NGSS_DENOISER_EDGE_TOLERANCE = 0.5f;

	[Header("BIAS")]
	[Tooltip("This estimates receiver slope using derivatives and tries to tilt the filtering kernel along it.\nHowever, when doing it in screenspace from the depth texture can leads to shadow artifacts.\nThus it is disabled by default.")]
	public bool NGSS_RECEIVER_PLANE_BIAS;

	[Header("CASCADES")]
	[Tooltip("Blends cascades at seams intersection.\nAdditional overhead required for this option.")]
	public bool NGSS_CASCADES_BLENDING = true;

	[Tooltip("Tweak this value to adjust the blending transition between cascades.")]
	[Range(0f, 2f)]
	public float NGSS_CASCADES_BLENDING_VALUE = 1f;

	[Range(0f, 1f)]
	[Tooltip("If one, softness across cascades will be matched using splits distribution, resulting in realistic soft-ness over distance.\nIf zero the softness distribution will be based on cascade index, resulting in blurrier shadows over distance thus less realistic.")]
	public float NGSS_CASCADES_SOFTNESS_NORMALIZATION = 1f;

	private bool isSetup;

	private bool isInitialized;

	private bool isGraphicSet;

	private Light _DirLight;

	private Light DirLight
	{
		get
		{
			if (_DirLight == null)
			{
				_DirLight = GetComponent<Light>();
			}
			return _DirLight;
		}
	}

	private void OnDisable()
	{
		isInitialized = false;
		if (!NGSS_KEEP_ONDISABLE && isGraphicSet)
		{
			isGraphicSet = false;
			GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/Internal-ScreenSpaceShadows"));
			GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseBuiltin);
		}
	}

	private void OnEnable()
	{
		if (IsNotSupported())
		{
			Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
			base.enabled = false;
		}
		else
		{
			Init();
		}
	}

	private void Init()
	{
		if (!isInitialized)
		{
			if (!isGraphicSet)
			{
				GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseCustom);
				GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, Shader.Find("Hidden/NGSS_Directional"));
				DirLight.shadows = ((DirLight.shadows != 0) ? LightShadows.Soft : LightShadows.None);
				isGraphicSet = true;
			}
			if (NGSS_NOISE_TEXTURE == null)
			{
				NGSS_NOISE_TEXTURE = Resources.Load<Texture2D>("BlueNoise_R8_8");
			}
			Shader.SetGlobalTexture("_BlueNoiseTextureDir", NGSS_NOISE_TEXTURE);
			isInitialized = true;
		}
	}

	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
	}

	private void Update()
	{
		if ((!Application.isPlaying || !NGSS_NO_UPDATE_ON_PLAY || !isSetup) && DirLight.shadows != 0 && DirLight.type == LightType.Directional)
		{
			DirLight.shadows = LightShadows.Soft;
			Shader.SetGlobalFloat("NGSS_DIR_SAMPLING_DISTANCE", NGSS_SAMPLING_DISTANCE);
			NGSS_SAMPLING_TEST = Mathf.Clamp(NGSS_SAMPLING_TEST, 4, NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat("NGSS_TEST_SAMPLERS_DIR", NGSS_SAMPLING_TEST);
			Shader.SetGlobalFloat("NGSS_FILTER_SAMPLERS_DIR", NGSS_SAMPLING_FILTER);
			Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS", (QualitySettings.shadowProjection == ShadowProjection.CloseFit) ? NGSS_SHADOWS_SOFTNESS : (NGSS_SHADOWS_SOFTNESS * 2f / (QualitySettings.shadowDistance * 0.66f) * ((QualitySettings.shadowCascades == 2) ? 1.5f : ((QualitySettings.shadowCascades == 4) ? 1f : 0.25f))));
			Shader.SetGlobalFloat("NGSS_GLOBAL_SOFTNESS_OPTIMIZED", NGSS_SHADOWS_SOFTNESS / QualitySettings.shadowDistance);
			int num = (int)Mathf.Sqrt(NGSS_SAMPLING_FILTER);
			Shader.SetGlobalInt("NGSS_OPTIMIZED_ITERATIONS", (num % 2 == 0) ? (num + 1) : num);
			Shader.SetGlobalInt("NGSS_OPTIMIZED_SAMPLERS", NGSS_SAMPLING_FILTER);
			Shader.SetGlobalInt("NGSS_DENOISER_ITERATIONS", NGSS_DENOISER_ENABLED ? NGSS_DENOISER_ITERATIONS : 0);
			Shader.SetGlobalFloat("NGSS_DENOISER_BLUR", 1f - Mathf.Clamp(NGSS_DENOISER_BLUR, 0f, 0.95f));
			Shader.SetGlobalFloat("NGSS_DENOISER_EDGE_TOLERANCE", NGSS_DENOISER_EDGE_TOLERANCE);
			if (NGSS_RECEIVER_PLANE_BIAS)
			{
				Shader.EnableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
			}
			else
			{
				Shader.DisableKeyword("NGSS_USE_RECEIVER_PLANE_BIAS");
			}
			Shader.SetGlobalFloat("NGSS_NOISE_TO_DITHERING_SCALE_DIR", NGSS_NOISE_TO_DITHERING_SCALE);
			if (NGSS_PCSS_ENABLED)
			{
				float num2 = NGSS_PCSS_SOFTNESS_NEAR * 0.25f;
				float num3 = NGSS_PCSS_SOFTNESS_FAR * 0.25f;
				Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MIN", (num2 > num3) ? num3 : num2);
				Shader.SetGlobalFloat("NGSS_PCSS_FILTER_DIR_MAX", (num3 < num2) ? num2 : num3);
				Shader.EnableKeyword("NGSS_PCSS_FILTER_DIR");
			}
			else
			{
				Shader.DisableKeyword("NGSS_PCSS_FILTER_DIR");
			}
			if (NGSS_SHADOWS_RESOLUTION == ShadowMapResolution.UseQualitySettings)
			{
				DirLight.shadowResolution = LightShadowResolution.FromQualitySettings;
			}
			else
			{
				DirLight.shadowCustomResolution = (int)NGSS_SHADOWS_RESOLUTION;
			}
			if (QualitySettings.shadowCascades > 1)
			{
				Shader.SetGlobalFloat("NGSS_CASCADES_SOFTNESS_NORMALIZATION", NGSS_CASCADES_SOFTNESS_NORMALIZATION);
				Shader.SetGlobalFloat("NGSS_CASCADES_COUNT", QualitySettings.shadowCascades);
				Shader.SetGlobalVector("NGSS_CASCADES_SPLITS", (QualitySettings.shadowCascades == 2) ? new Vector4(QualitySettings.shadowCascade2Split, 1f, 1f, 1f) : new Vector4(QualitySettings.shadowCascade4Split.x, QualitySettings.shadowCascade4Split.y, QualitySettings.shadowCascade4Split.z, 1f));
			}
			if (NGSS_CASCADES_BLENDING && QualitySettings.shadowCascades > 1)
			{
				Shader.EnableKeyword("NGSS_USE_CASCADE_BLENDING");
				Shader.SetGlobalFloat("NGSS_CASCADE_BLEND_DISTANCE", NGSS_CASCADES_BLENDING_VALUE * 0.125f);
			}
			else
			{
				Shader.DisableKeyword("NGSS_USE_CASCADE_BLENDING");
			}
			isSetup = true;
		}
	}
}
