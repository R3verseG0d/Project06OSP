using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SceneParameters : MonoBehaviour
{
	public enum Setting
	{
		Normal = 0,
		FoggyNoise = 1,
		Thick = 2
	}

	[Header("Framework")]
	public TextAsset SceneFile;

	public Flare LensFlare;

	public float BloomThreshold;

	public float BloomMaxThreshold;

	public float BloomSpread;

	public bool UseShadows = true;

	public bool UseVolumetricLight;

	public Setting VolumetricSetting;

	public Cubemap StageCubemap;

	public LightPreset[] LightPresets;

	[Header("Optional")]
	public PostProcessProfile TrackballsSettings;

	public LightAnimationPreset[] LightAnimationPresets;

	private LensFlare MainLensFlare;

	private VolumetricLight MainVLight;

	private Light LightMain;

	private Light LightSub;

	private Color LightMainColor;

	private Color LightSubColor;

	private Color AmbientColor;

	private Vector3 LightMainDir;

	private Vector3 LightSubDir;

	private float AmbientIntensity;

	internal bool LightChange;

	internal bool LightAnimationChange;

	private Light MainLightComp;

	private Light SubLightComp;

	private string MainLight;

	private string SubLight;

	private string Ambient;

	private Color MainChangeColor;

	private float MainChangeInt;

	private Color SubChangeColor;

	private float SubChangeInt;

	private Color AmbChangeColor;

	private float AmbChangeMult;

	private string AnimMainLight;

	private string AnimSubLight;

	private string AnimAmbient;

	private Gradient MainAnimColor;

	private float MainAnimSpeed;

	private Gradient SubAnimColor;

	private float SubAnimSpeed;

	private Gradient AmbAnimColor;

	private float AmbAnimSpeed;

	private Color OrigMainColor;

	private float OrigMainInt;

	private Color OrigSubColor;

	private float OrigSubInt;

	private Color OrigAmbColor;

	private Vector3 MainChangeDir;

	private Vector3 SubChangeDir;

	private Vector3 OrigMainDir;

	private Vector3 OrigSubDir;

	public void UpdateCubemap()
	{
		Awake();
	}

	private void Awake()
	{
		Shader.SetGlobalTexture("_Cube", StageCubemap);
	}

	private void Start()
	{
		if (LightPresets.Length != 0 || LightAnimationPresets.Length != 0)
		{
			MainLightComp = GameObject.Find("Directional light").GetComponent<Light>();
			SubLightComp = GameObject.Find("Secondary light").GetComponent<Light>();
			OrigMainColor = MainLightComp.color;
			OrigMainInt = MainLightComp.intensity;
			OrigMainDir = MainLightComp.transform.forward;
			OrigSubColor = SubLightComp.color;
			OrigSubInt = SubLightComp.intensity;
			OrigSubDir = SubLightComp.transform.forward;
			OrigAmbColor = RenderSettings.ambientLight;
		}
	}

	private void Update()
	{
		if (LightPresets.Length != 0)
		{
			MainLightComp.color = Color.Lerp(MainLightComp.color, (LightChange && !LightAnimationChange) ? MainChangeColor : OrigMainColor, Time.deltaTime * 0.4f);
			MainLightComp.intensity = Mathf.Lerp(MainLightComp.intensity, LightChange ? MainChangeInt : OrigMainInt, Time.deltaTime * 0.4f);
			MainLightComp.transform.forward = Vector3.Lerp(MainLightComp.transform.forward, LightChange ? MainChangeDir : OrigMainDir, Time.deltaTime * 0.4f);
			SubLightComp.color = Color.Lerp(SubLightComp.color, (LightChange && !LightAnimationChange) ? SubChangeColor : OrigSubColor, Time.deltaTime * 0.4f);
			SubLightComp.intensity = Mathf.Lerp(SubLightComp.intensity, LightChange ? SubChangeInt : OrigSubInt, Time.deltaTime * 0.4f);
			SubLightComp.transform.forward = Vector3.Lerp(SubLightComp.transform.forward, LightChange ? SubChangeDir : OrigSubDir, Time.deltaTime * 0.4f);
			RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, (LightChange && !LightAnimationChange) ? AmbChangeColor : OrigAmbColor, Time.deltaTime * 0.4f);
		}
		if (LightAnimationPresets.Length != 0)
		{
			MainLightComp.color = Color.Lerp(MainLightComp.color, (LightAnimationChange && !LightChange) ? MainAnimColor.Evaluate(Mathf.Repeat(Time.time, MainAnimSpeed) / MainAnimSpeed) : OrigMainColor, Time.deltaTime * 2.5f);
			SubLightComp.color = Color.Lerp(SubLightComp.color, (LightAnimationChange && !LightChange) ? SubAnimColor.Evaluate(Mathf.Repeat(Time.time, SubAnimSpeed) / SubAnimSpeed) : OrigSubColor, Time.deltaTime * 2.5f);
			RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, (LightAnimationChange && !LightChange) ? AmbAnimColor.Evaluate(Mathf.Repeat(Time.time, AmbAnimSpeed) / AmbAnimSpeed) : OrigAmbColor, Time.deltaTime * 2.5f);
		}
	}

	private Vector3 GetNewDir(Vector3 OrigDir, int Index)
	{
		if (LightPresets[Index].Direction_3dsmax != Vector3.zero)
		{
			Vector3 vector = new Vector3(LightPresets[Index].Direction_3dsmax.x, LightPresets[Index].Direction_3dsmax.y, LightPresets[Index].Direction_3dsmax.z);
			return Quaternion.Euler(0f, 180f, 0f) * -new Vector3(vector.x, vector.z, vector.y);
		}
		return OrigDir;
	}

	public void SetLightPreset(string _MainLight, string _SubLight, string _Ambient)
	{
		if (!(MainLight != _MainLight) && !(SubLight != _SubLight) && !(Ambient != _Ambient))
		{
			return;
		}
		MainLight = _MainLight;
		SubLight = _SubLight;
		Ambient = _Ambient;
		for (int i = 0; i < LightPresets.Length; i++)
		{
			if (LightPresets[i].PresetName == MainLight)
			{
				MainChangeColor = LightPresets[i].LightColor;
				MainChangeInt = LightPresets[i].Alpha;
				MainChangeDir = GetNewDir(OrigMainDir, i);
			}
			else if (_MainLight == "")
			{
				MainChangeColor = OrigMainColor;
				MainChangeInt = OrigMainInt;
				MainChangeDir = OrigMainDir;
			}
			if (LightPresets[i].PresetName == SubLight)
			{
				SubChangeColor = LightPresets[i].LightColor;
				SubChangeInt = LightPresets[i].Alpha;
				SubChangeDir = GetNewDir(OrigSubDir, i);
			}
			else if (_SubLight == "")
			{
				SubChangeColor = OrigSubColor;
				SubChangeInt = OrigSubInt;
				SubChangeDir = OrigSubDir;
			}
			if (LightPresets[i].PresetName == Ambient)
			{
				AmbChangeColor = LightPresets[i].LightColor * LightPresets[i].Alpha;
			}
			else if (_Ambient == "")
			{
				AmbChangeColor = OrigAmbColor;
			}
		}
	}

	public void SetLightAnimationPreset(string _AnimMainLight, string _AnimSubLight, string _AnimAmbient)
	{
		if (!(AnimMainLight != _AnimMainLight) || !(AnimSubLight != _AnimSubLight) || !(AnimAmbient != _AnimAmbient))
		{
			return;
		}
		AnimMainLight = _AnimMainLight;
		AnimSubLight = _AnimSubLight;
		AnimAmbient = _AnimAmbient;
		for (int i = 0; i < LightAnimationPresets.Length; i++)
		{
			if (LightAnimationPresets[i].PresetName == AnimMainLight)
			{
				MainAnimColor = LightAnimationPresets[i].LightGradient;
				MainAnimSpeed = LightAnimationPresets[i].Speed;
			}
			if (LightAnimationPresets[i].PresetName == AnimSubLight)
			{
				SubAnimColor = LightAnimationPresets[i].LightGradient;
				SubAnimSpeed = LightAnimationPresets[i].Speed;
			}
			if (LightAnimationPresets[i].PresetName == AnimAmbient)
			{
				AmbAnimColor = LightAnimationPresets[i].LightGradient;
				AmbAnimSpeed = LightAnimationPresets[i].Speed;
			}
		}
	}

	private Color StringToColor(string String)
	{
		string[] array = String.Replace(" ", "").Split(',');
		return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	private Vector3 StringToVector3(string String)
	{
		string[] array = String.Replace(" ", "").Split(',');
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}

	public void ReadParameters()
	{
		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
		GameObject gameObject = GameObject.Find("Stage");
		GameObject gameObject2 = GameObject.Find("Stage/Lights");
		if (!gameObject2)
		{
			gameObject2 = new GameObject
			{
				name = "Lights"
			};
			gameObject2.transform.SetParent(gameObject.transform);
		}
		GameObject gameObject3 = GameObject.Find("Directional light");
		if ((bool)gameObject3)
		{
			Light component = gameObject3.GetComponent<Light>();
			LightMain = component;
			if ((bool)LensFlare)
			{
				if ((bool)gameObject3.GetComponent<LensFlare>())
				{
					MainLensFlare = gameObject3.GetComponent<LensFlare>();
				}
				else
				{
					MainLensFlare = gameObject3.AddComponent<LensFlare>();
				}
				MainLensFlare.flare = LensFlare;
				MainLensFlare.color = new Vector4(1f, 1f, 1f, 1f);
				MainLensFlare.fadeSpeed = 10f;
			}
			else if ((bool)gameObject3.GetComponent<LensFlare>())
			{
				Object.DestroyImmediate(gameObject3.GetComponent<LensFlare>());
			}
			if (UseVolumetricLight)
			{
				if ((bool)gameObject3.GetComponent<VolumetricLight>())
				{
					MainVLight = gameObject3.GetComponent<VolumetricLight>();
				}
				else
				{
					MainVLight = gameObject3.AddComponent<VolumetricLight>();
				}
				SetVolumetricLightSettings(MainVLight);
			}
			else if ((bool)gameObject3.GetComponent<VolumetricLight>())
			{
				Object.DestroyImmediate(gameObject3.GetComponent<VolumetricLight>());
			}
		}
		else
		{
			GameObject gameObject4 = new GameObject
			{
				name = "Directional light"
			};
			if ((bool)LensFlare)
			{
				LensFlare lensFlare = gameObject4.AddComponent<LensFlare>();
				lensFlare.flare = LensFlare;
				lensFlare.color = new Vector4(1f, 1f, 1f, 1f);
			}
			Light light = gameObject4.AddComponent<Light>();
			light.type = LightType.Directional;
			LightMain = light;
			gameObject4.transform.SetParent(gameObject2.transform);
		}
		GameObject gameObject5 = GameObject.Find("Secondary light");
		if ((bool)gameObject5)
		{
			Light component2 = gameObject5.GetComponent<Light>();
			LightSub = component2;
		}
		else
		{
			GameObject obj = new GameObject
			{
				name = "Secondary light"
			};
			Light light2 = obj.AddComponent<Light>();
			light2.type = LightType.Directional;
			LightSub = light2;
			obj.transform.SetParent(gameObject2.transform);
		}
		if ((bool)SceneFile)
		{
			string text = SceneFile.text;
			string text2 = "Light = {Ambient = {Color = {";
			string text3 = "}}, Main = {Color = {";
			string text4 = "}, Direction_3dsmax = {Position = {";
			string text5 = "}, Target = {0, 0, 0}}}, Sub = {Color = {";
			string text6 = "}, Direction_3dsmax = {Position = {";
			string text7 = "}, Target = {0, 0, 0}}}}, Bloom = {MinThreshold = ";
			string text8 = ", MaxThreshold = ";
			string text9 = ", Scale = ";
			StringRead stringRead = new StringRead(text);
			stringRead.Skip(text2.Length);
			Color color = StringToColor(stringRead.ReadUntil('}'));
			AmbientColor = new Color(color.r, color.g, color.b, 1f);
			AmbientIntensity = color.a;
			stringRead.Skip(text3.Length);
			LightMainColor = StringToColor(stringRead.ReadUntil('}'));
			stringRead.Skip(text4.Length);
			LightMainDir = StringToVector3(stringRead.ReadUntil('}')).normalized;
			stringRead.Skip(text5.Length);
			LightSubColor = StringToColor(stringRead.ReadUntil('}'));
			stringRead.Skip(text6.Length);
			LightSubDir = StringToVector3(stringRead.ReadUntil('}')).normalized;
			stringRead.Skip(text7.Length);
			BloomThreshold = float.Parse(stringRead.ReadUntil(','));
			stringRead.Skip(text8.Length);
			BloomMaxThreshold = float.Parse(stringRead.ReadUntil(','));
			stringRead.Skip(text9.Length);
			BloomSpread = float.Parse(stringRead.ReadUntil('}'));
			LightMainDir = Quaternion.Euler(0f, 180f, 0f) * -new Vector3(LightMainDir.x, LightMainDir.z, LightMainDir.y);
			LightSubDir = Quaternion.Euler(0f, 180f, 0f) * -new Vector3(LightSubDir.x, LightSubDir.z, LightSubDir.y);
			LightMain.transform.forward = LightMainDir;
			LightMain.color = new Color(LightMainColor.r, LightMainColor.g, LightMainColor.b, 1f);
			LightMain.intensity = LightMainColor.a;
			LightMain.bounceIntensity = 0f;
			LightMain.shadows = (UseShadows ? LightShadows.Soft : LightShadows.None);
			LightMain.shadowStrength = 1f;
			LightMain.shadowBias = 0.075f;
			LightMain.shadowNormalBias = 0f;
			LightMain.shadowNearPlane = 0.2f;
			LightMain.renderMode = LightRenderMode.ForcePixel;
			LightSub.transform.forward = LightSubDir;
			LightSub.color = new Color(LightSubColor.r, LightSubColor.g, LightSubColor.b, 1f);
			LightSub.intensity = LightSubColor.a;
			LightSub.bounceIntensity = 0f;
			LightSub.renderMode = LightRenderMode.ForcePixel;
			RenderSettings.ambientLight = AmbientColor * AmbientIntensity;
			RenderSettings.ambientIntensity = 1f;
		}
		else
		{
			Debug.Log("Assign a text file in the inspector, idiot.");
		}
	}

	private void SetVolumetricLightSettings(VolumetricLight Recipient)
	{
		switch (VolumetricSetting)
		{
		case Setting.Normal:
			Recipient.SampleCount = 16;
			Recipient.ScatteringCoef = 0.135f;
			Recipient.ExtinctionCoef = 0f;
			Recipient.SkyboxExtinctionCoef = 0f;
			Recipient.MieG = 0.55f;
			Recipient.MaxRayLength = 5f;
			Recipient.Noise = false;
			Recipient.NoiseScale = 0f;
			Recipient.NoiseIntensity = 0f;
			Recipient.NoiseIntensityOffset = 0f;
			Recipient.NoiseVelocity = Vector2.zero;
			break;
		case Setting.FoggyNoise:
			Recipient.SampleCount = 16;
			Recipient.ScatteringCoef = 0.2f;
			Recipient.ExtinctionCoef = 0f;
			Recipient.SkyboxExtinctionCoef = 0f;
			Recipient.MieG = 0.35f;
			Recipient.MaxRayLength = 5f;
			Recipient.Noise = true;
			Recipient.NoiseScale = 0.1f;
			Recipient.NoiseIntensity = 100f;
			Recipient.NoiseIntensityOffset = 0.4f;
			Recipient.NoiseVelocity = new Vector2(0.25f, -0.5f);
			break;
		case Setting.Thick:
			Recipient.SampleCount = 16;
			Recipient.ScatteringCoef = 0.285f;
			Recipient.ExtinctionCoef = 0f;
			Recipient.SkyboxExtinctionCoef = 0f;
			Recipient.MieG = 0.5f;
			Recipient.MaxRayLength = 5f;
			Recipient.Noise = false;
			Recipient.NoiseScale = 0f;
			Recipient.NoiseIntensity = 0f;
			Recipient.NoiseIntensityOffset = 0f;
			Recipient.NoiseVelocity = Vector2.zero;
			break;
		default:
			MonoBehaviour.print("Idiot.");
			break;
		}
	}
}
