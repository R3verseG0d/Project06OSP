using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class NGSS_FrustumShadows : MonoBehaviour
{
	[Header("REFERENCES")]
	public Light mainShadowsLight;

	public Shader frustumShadowsShader;

	[SerializeField]
	private bool m_debug;

	[Header("SHADOWS SETTINGS")]
	[Tooltip("Poisson Noise. Randomize samples to remove repeated patterns.")]
	public bool m_dithering;

	[Tooltip("If enabled a faster separable blur will be used.\nIf disabled a slower depth aware blur will be used.")]
	public bool m_fastBlur = true;

	[Tooltip("If enabled, backfaced lit fragments will be skipped increasing performance. Requires GBuffer normals.")]
	public bool m_deferredBackfaceOptimization;

	[Range(0f, 1f)]
	[Tooltip("Set how backfaced lit fragments are shaded. Requires DeferredBackfaceOptimization to be enabled.")]
	public float m_deferredBackfaceTranslucency;

	[Tooltip("Tweak this value to remove soft-shadows leaking around edges.")]
	[Range(0.01f, 1f)]
	public float m_shadowsEdgeBlur = 0.25f;

	[Tooltip("Overall softness of the shadows.")]
	[Range(0.01f, 1f)]
	public float m_shadowsBlur = 0.5f;

	[Tooltip("Tweak this value if your objects display backface shadows.")]
	[Range(0f, 1f)]
	public float m_shadowsBias = 0.05f;

	[Tooltip("The distance in metters from camera where shadows start to shown.")]
	public float m_shadowsDistanceStart;

	[Header("RAY SETTINGS")]
	[Tooltip("If enabled the ray length will be scaled at screen space instead of world space. Keep it enabled for an infinite view shadows coverage. Disable it for a ContactShadows like effect. Adjust the Ray Scale property accordingly.")]
	public bool m_rayScreenScale = true;

	[Tooltip("Number of samplers between each step. The higher values produces less gaps between shadows but is more costly.")]
	[Range(16f, 128f)]
	public int m_raySamples = 64;

	[Tooltip("The higher the value, the larger the shadows ray will be.")]
	[Range(0.01f, 1f)]
	public float m_rayScale = 0.25f;

	[Tooltip("The higher the value, the ticker the shadows will look.")]
	[Range(0f, 1f)]
	public float m_rayThickness = 0.01f;

	[Header("TEMPORAL SETTINGS (EXPERIMENTAL)")]
	[Tooltip("Temporal filtering. Improves the shadows aliasing by adding an extra temporal pass. Currently experimental, does not work when the Scene View is open, only in Game View.")]
	public bool m_Temporal;

	private bool isTemporal;

	[Range(0f, 1f)]
	[Tooltip("Temporal scale in seconds. The bigger the smoother the shadows but produces trail/blur within shadows.")]
	public float m_Scale = 0.75f;

	[Tooltip("Improves the temporal filter by shaking the screen space shadows at different frames.")]
	[Range(0f, 0.25f)]
	public float m_Jittering;

	private int mainTexID = Shader.PropertyToID("_MainTex");

	private int debugSource = Shader.PropertyToID("Debug RT");

	private int cShadow = Shader.PropertyToID("NGSS_ContactShadowRT");

	private int cShadow2 = Shader.PropertyToID("NGSS_ContactShadowRT2");

	private int dSource = Shader.PropertyToID("NGSS_DepthSourceRT");

	private int m_SampleIndex;

	private RenderingPath currentRenderingPath;

	private CommandBuffer computeShadowsCB;

	private CommandBuffer debugCB;

	private bool isInitialized;

	private RenderTexture mTempRT;

	private Camera _mCamera;

	private Material _mMaterial;

	private Mesh fullScreenTriangle;

	private RenderTexture TempRT
	{
		get
		{
			if (mTempRT == null || mTempRT.width != mCamera.pixelWidth || mTempRT.height != mCamera.pixelHeight)
			{
				if ((bool)mTempRT)
				{
					RenderTexture.ReleaseTemporary(mTempRT);
				}
				mTempRT = RenderTexture.GetTemporary(mCamera.pixelWidth, mCamera.pixelHeight, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
				mTempRT.hideFlags = HideFlags.HideAndDontSave;
			}
			return mTempRT;
		}
		set
		{
			mTempRT = value;
		}
	}

	private Camera mCamera
	{
		get
		{
			if (_mCamera == null)
			{
				_mCamera = GetComponent<Camera>();
				if (_mCamera == null)
				{
					_mCamera = Camera.main;
				}
				if (_mCamera == null)
				{
					Debug.LogError("NGSS Error: No MainCamera found, please provide one.", this);
					base.enabled = false;
				}
			}
			return _mCamera;
		}
	}

	private Material mMaterial
	{
		get
		{
			if (_mMaterial == null)
			{
				if (frustumShadowsShader == null)
				{
					frustumShadowsShader = Shader.Find("Hidden/NGSS_FrustumShadows");
				}
				_mMaterial = new Material(frustumShadowsShader);
				if (_mMaterial == null)
				{
					Debug.LogWarning("NGSS Warning: can't find NGSS_FrustumShadows shader, make sure it's on your project.", this);
					base.enabled = false;
				}
			}
			return _mMaterial;
		}
	}

	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2;
	}

	private void AddCommandBuffers()
	{
		currentRenderingPath = mCamera.renderingPath;
		if (computeShadowsCB == null)
		{
			computeShadowsCB = new CommandBuffer
			{
				name = "NGSS FrustumShadows: Compute"
			};
		}
		else
		{
			computeShadowsCB.Clear();
		}
		bool flag = true;
		if (mCamera.renderingPath == RenderingPath.DeferredShading)
		{
			CommandBuffer[] commandBuffers = mCamera.GetCommandBuffers(m_Temporal ? CameraEvent.AfterGBuffer : CameraEvent.BeforeLighting);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == computeShadowsCB.name)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				mCamera.AddCommandBuffer(m_Temporal ? CameraEvent.AfterGBuffer : CameraEvent.BeforeLighting, computeShadowsCB);
			}
		}
		else
		{
			CommandBuffer[] commandBuffers = mCamera.GetCommandBuffers(CameraEvent.AfterDepthTexture);
			for (int i = 0; i < commandBuffers.Length; i++)
			{
				if (commandBuffers[i].name == computeShadowsCB.name)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				mCamera.AddCommandBuffer((!m_Temporal) ? CameraEvent.AfterDepthTexture : CameraEvent.AfterForwardOpaque, computeShadowsCB);
			}
		}
		if (m_debug)
		{
			if (debugCB == null)
			{
				debugCB = new CommandBuffer();
				debugCB.name = "Draw to Temporary RT";
			}
			debugCB.Clear();
			debugCB.GetTemporaryRT(debugSource, XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
			CustomBlit(debugCB, BuiltinRenderTextureType.CurrentActive, debugSource, mMaterial, 0);
			mCamera.AddCommandBuffer(CameraEvent.AfterImageEffects, debugCB);
		}
	}

	private void RemoveCommandBuffers()
	{
		_mMaterial = null;
		if (mCamera.renderingPath == RenderingPath.DeferredShading)
		{
			if ((bool)mCamera)
			{
				mCamera.RemoveCommandBuffer(isTemporal ? CameraEvent.AfterGBuffer : CameraEvent.BeforeLighting, computeShadowsCB);
			}
			else if ((bool)mCamera)
			{
				mCamera.RemoveCommandBuffer((!m_Temporal) ? CameraEvent.AfterDepthTexture : CameraEvent.AfterForwardOpaque, computeShadowsCB);
			}
		}
		isInitialized = false;
		if (m_debug && debugCB != null && (bool)mCamera)
		{
			mCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, debugCB);
		}
	}

	private void CustomBlit(CommandBuffer cmd, RenderTargetIdentifier src, RenderTargetIdentifier dest, Material mat, int pass)
	{
		cmd.SetRenderTarget(dest, 0, CubemapFace.Unknown, -1);
		cmd.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
		cmd.DrawMesh(fullScreenTriangle, Matrix4x4.identity, mat, pass);
	}

	private void Init()
	{
		if (isInitialized || mainShadowsLight == null)
		{
			return;
		}
		if (mCamera.renderingPath == RenderingPath.VertexLit)
		{
			Debug.LogWarning("Vertex Lit Rendering Path is not supported by NGSS Contact Shadows. Please set the Rendering Path in your game camera or Graphics Settings to something else than Vertex Lit.", this);
			base.enabled = false;
			return;
		}
		CreateFullscreenQuad(mCamera);
		AddCommandBuffers();
		computeShadowsCB.GetTemporaryRT(cShadow, XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
		computeShadowsCB.GetTemporaryRT(cShadow2, XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
		computeShadowsCB.GetTemporaryRT(dSource, XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 0, FilterMode.Bilinear, RenderTextureFormat.RFloat);
		computeShadowsCB.Blit(cShadow, dSource, mMaterial, 0);
		computeShadowsCB.Blit(dSource, cShadow, mMaterial, 1);
		computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, 1f));
		computeShadowsCB.Blit(cShadow, cShadow2, mMaterial, 2);
		computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(1f, 0f));
		computeShadowsCB.Blit(cShadow2, cShadow, mMaterial, 2);
		if (m_Temporal)
		{
			computeShadowsCB.SetGlobalTexture("NGSS_Temporal_Tex", TempRT);
			computeShadowsCB.Blit(cShadow, cShadow2, mMaterial, 3);
			computeShadowsCB.Blit(cShadow2, TempRT);
			computeShadowsCB.SetGlobalTexture("NGSS_FrustumShadowsTexture", TempRT);
		}
		else
		{
			computeShadowsCB.SetGlobalTexture("NGSS_FrustumShadowsTexture", cShadow);
		}
		isInitialized = true;
	}

	private void OnEnable()
	{
		if (IsNotSupported())
		{
			Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
			base.enabled = false;
			return;
		}
		if (m_Temporal)
		{
			mCamera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}
		else
		{
			mCamera.depthTextureMode = DepthTextureMode.Depth;
		}
		Init();
	}

	private void OnDisable()
	{
		Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 0f);
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
		if (TempRT != null)
		{
			RenderTexture.ReleaseTemporary(TempRT);
			TempRT = null;
		}
	}

	private void OnApplicationQuit()
	{
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
	}

	private void OnPreCull()
	{
	}

	private void OnPreRender()
	{
		Init();
		if (isInitialized && !(mainShadowsLight == null))
		{
			if (currentRenderingPath != mCamera.renderingPath)
			{
				RemoveCommandBuffers();
				AddCommandBuffers();
			}
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 1f);
			Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_OPACITY", 1f - mainShadowsLight.shadowStrength);
			if (m_Temporal != isTemporal)
			{
				base.enabled = false;
				isTemporal = m_Temporal;
				base.enabled = true;
			}
			mMaterial.SetFloat("_TemporalScale", m_Temporal ? Mathf.Clamp(m_Scale, 0f, 0.99f) : 0f);
			mMaterial.SetVector("_Jitter_Offset", m_Temporal ? (GenerateRandomOffset() * m_Jittering) : Vector2.zero);
			mMaterial.SetMatrix("WorldToView", mCamera.worldToCameraMatrix);
			mMaterial.SetVector("LightPos", mainShadowsLight.transform.position);
			mMaterial.SetVector("LightDir", -mCamera.transform.InverseTransformDirection(mainShadowsLight.transform.forward));
			mMaterial.SetVector("LightDirWorld", -mainShadowsLight.transform.forward);
			mMaterial.SetFloat("ShadowsEdgeTolerance", m_shadowsEdgeBlur * 0.075f);
			mMaterial.SetFloat("ShadowsSoftness", m_shadowsBlur);
			mMaterial.SetFloat("RayScale", m_rayScale);
			mMaterial.SetFloat("ShadowsBias", m_shadowsBias * 0.02f);
			mMaterial.SetFloat("ShadowsDistanceStart", m_shadowsDistanceStart - 10f);
			mMaterial.SetFloat("RayThickness", m_rayThickness);
			mMaterial.SetFloat("RaySamples", m_raySamples);
			if (m_deferredBackfaceOptimization && mCamera.actualRenderingPath == RenderingPath.DeferredShading)
			{
				mMaterial.EnableKeyword("NGSS_DEFERRED_OPTIMIZATION");
				mMaterial.SetFloat("BackfaceOpacity", m_deferredBackfaceTranslucency);
			}
			else
			{
				mMaterial.DisableKeyword("NGSS_DEFERRED_OPTIMIZATION");
			}
			if (m_dithering)
			{
				mMaterial.EnableKeyword("NGSS_USE_DITHERING");
			}
			else
			{
				mMaterial.DisableKeyword("NGSS_USE_DITHERING");
			}
			if (m_fastBlur)
			{
				mMaterial.EnableKeyword("NGSS_FAST_BLUR");
			}
			else
			{
				mMaterial.DisableKeyword("NGSS_FAST_BLUR");
			}
			if (mainShadowsLight.type != LightType.Directional)
			{
				mMaterial.EnableKeyword("NGSS_USE_LOCAL_SHADOWS");
			}
			else
			{
				mMaterial.DisableKeyword("NGSS_USE_LOCAL_SHADOWS");
			}
			mMaterial.SetFloat("RayScreenScale", m_rayScreenScale ? 1f : 0f);
		}
	}

	private void OnPostRender()
	{
		Shader.SetGlobalFloat("NGSS_FRUSTUM_SHADOWS_ENABLED", 0f);
	}

	private float GetHaltonValue(int index, int radix)
	{
		float num = 0f;
		float num2 = 1f / (float)radix;
		while (index > 0)
		{
			num += (float)(index % radix) * num2;
			index /= radix;
			num2 /= (float)radix;
		}
		return num;
	}

	private Vector2 GenerateRandomOffset()
	{
		Vector2 result = new Vector2(GetHaltonValue(m_SampleIndex & 0x3FF, 2), GetHaltonValue(m_SampleIndex & 0x3FF, 3));
		if (++m_SampleIndex >= 16)
		{
			m_SampleIndex = 0;
		}
		float num = Mathf.Tan((float)Math.PI / 360f * mCamera.fieldOfView);
		float num2 = num * mCamera.aspect;
		result.x *= num2 / (0.5f * (float)mCamera.pixelWidth);
		result.y *= num / (0.5f * (float)mCamera.pixelHeight);
		return result;
	}

	private void InitializeTriangle()
	{
		if (!fullScreenTriangle)
		{
			fullScreenTriangle = new Mesh
			{
				name = "My Post-Processing Stack Full-Screen Triangle",
				vertices = new Vector3[3]
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				},
				triangles = new int[3] { 0, 1, 2 }
			};
			fullScreenTriangle.UploadMeshData(markNoLongerReadable: true);
		}
	}

	private void CreateFullscreenQuad(Camera cam)
	{
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[4]
		{
			cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane + 0.01f)),
			cam.ViewportToWorldPoint(new Vector3(1f, 0f, cam.nearClipPlane + 0.01f)),
			cam.ViewportToWorldPoint(new Vector3(0f, 1f, cam.nearClipPlane + 0.01f)),
			cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane + 0.01f))
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		int[] triangles = new int[6] { 0, 3, 1, 0, 2, 3 };
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		fullScreenTriangle = mesh;
	}
}
