using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

[RequireComponent(typeof(Camera))]
public class VolumetricLightRenderer : MonoBehaviour
{
	public enum VolumtericResolution
	{
		Full = 0,
		Half = 1,
		Quarter = 2
	}

	private static Mesh _pointLightMesh;

	private static Mesh _spotLightMesh;

	private static Material _lightMaterial;

	private Camera _camera;

	private CommandBuffer _preLightPass;

	private Matrix4x4 _viewProj;

	private Material _blitAddMaterial;

	private Material _bilateralBlurMaterial;

	private RenderTexture _volumeLightTexture;

	private RenderTexture _halfVolumeLightTexture;

	private RenderTexture _quarterVolumeLightTexture;

	private static Texture _defaultSpotCookie;

	private RenderTexture _halfDepthBuffer;

	private RenderTexture _quarterDepthBuffer;

	private VolumtericResolution _currentResolution = VolumtericResolution.Half;

	private Texture2D _ditheringTexture;

	private Texture3D _noiseTexture;

	public VolumtericResolution Resolution = VolumtericResolution.Half;

	public Texture DefaultSpotCookie;

	public CommandBuffer GlobalCommandBuffer => _preLightPass;

	public static event Action<VolumetricLightRenderer, Matrix4x4> PreRenderEvent;

	public static Material GetLightMaterial()
	{
		return _lightMaterial;
	}

	public static Mesh GetPointLightMesh()
	{
		return _pointLightMesh;
	}

	public static Mesh GetSpotLightMesh()
	{
		return _spotLightMesh;
	}

	public RenderTexture GetVolumeLightBuffer()
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			return _quarterVolumeLightTexture;
		}
		if (Resolution == VolumtericResolution.Half)
		{
			return _halfVolumeLightTexture;
		}
		return _volumeLightTexture;
	}

	public RenderTexture GetVolumeLightDepthBuffer()
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			return _quarterDepthBuffer;
		}
		if (Resolution == VolumtericResolution.Half)
		{
			return _halfDepthBuffer;
		}
		return null;
	}

	public static Texture GetDefaultSpotCookie()
	{
		return _defaultSpotCookie;
	}

	private void Awake()
	{
		_camera = GetComponent<Camera>();
		if (_camera.actualRenderingPath == RenderingPath.Forward)
		{
			_camera.depthTextureMode = DepthTextureMode.Depth;
		}
		_currentResolution = Resolution;
		Shader shader = Shader.Find("Hidden/BlitAdd");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Hidden/BlitAdd\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_blitAddMaterial = new Material(shader);
		shader = Shader.Find("Hidden/BilateralBlur");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Hidden/BilateralBlur\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_bilateralBlurMaterial = new Material(shader);
		_preLightPass = new CommandBuffer();
		_preLightPass.name = "PreLight";
		ChangeResolution();
		if (_pointLightMesh == null)
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_pointLightMesh = obj.GetComponent<MeshFilter>().sharedMesh;
			UnityEngine.Object.Destroy(obj);
		}
		if (_spotLightMesh == null)
		{
			_spotLightMesh = CreateSpotLightMesh();
		}
		if (_lightMaterial == null)
		{
			shader = Shader.Find("Sandbox/VolumetricLight");
			if (shader == null)
			{
				throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
			}
			_lightMaterial = new Material(shader);
		}
		if (_defaultSpotCookie == null)
		{
			_defaultSpotCookie = DefaultSpotCookie;
		}
		LoadNoise3dTexture();
		GenerateDitherTexture();
	}

	private void OnEnable()
	{
		if (_camera.actualRenderingPath == RenderingPath.Forward)
		{
			_camera.AddCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
		}
		else
		{
			_camera.AddCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
		}
	}

	private void OnDisable()
	{
		if (_camera.actualRenderingPath == RenderingPath.Forward)
		{
			_camera.RemoveCommandBuffer(CameraEvent.AfterDepthTexture, _preLightPass);
		}
		else
		{
			_camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, _preLightPass);
		}
	}

	private void ChangeResolution()
	{
		int pixelWidth = _camera.pixelWidth;
		int pixelHeight = _camera.pixelHeight;
		if (_volumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(_volumeLightTexture);
		}
		_volumeLightTexture = new RenderTexture(pixelWidth, pixelHeight, 0, RenderTextureFormat.ARGBHalf);
		_volumeLightTexture.name = "VolumeLightBuffer";
		_volumeLightTexture.filterMode = FilterMode.Bilinear;
		if (_halfDepthBuffer != null)
		{
			UnityEngine.Object.Destroy(_halfDepthBuffer);
		}
		if (_halfVolumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(_halfVolumeLightTexture);
		}
		if (Resolution == VolumtericResolution.Half || Resolution == VolumtericResolution.Quarter)
		{
			_halfVolumeLightTexture = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.ARGBHalf);
			_halfVolumeLightTexture.name = "VolumeLightBufferHalf";
			_halfVolumeLightTexture.filterMode = FilterMode.Bilinear;
			_halfDepthBuffer = new RenderTexture(pixelWidth / 2, pixelHeight / 2, 0, RenderTextureFormat.RFloat);
			_halfDepthBuffer.name = "VolumeLightHalfDepth";
			_halfDepthBuffer.Create();
			_halfDepthBuffer.filterMode = FilterMode.Point;
		}
		if (_quarterVolumeLightTexture != null)
		{
			UnityEngine.Object.Destroy(_quarterVolumeLightTexture);
		}
		if (_quarterDepthBuffer != null)
		{
			UnityEngine.Object.Destroy(_quarterDepthBuffer);
		}
		if (Resolution == VolumtericResolution.Quarter)
		{
			_quarterVolumeLightTexture = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.ARGBHalf);
			_quarterVolumeLightTexture.name = "VolumeLightBufferQuarter";
			_quarterVolumeLightTexture.filterMode = FilterMode.Bilinear;
			_quarterDepthBuffer = new RenderTexture(pixelWidth / 4, pixelHeight / 4, 0, RenderTextureFormat.RFloat);
			_quarterDepthBuffer.name = "VolumeLightQuarterDepth";
			_quarterDepthBuffer.Create();
			_quarterDepthBuffer.filterMode = FilterMode.Point;
		}
	}

	public void OnPreRender()
	{
		Matrix4x4 proj = Matrix4x4.Perspective(_camera.fieldOfView, _camera.aspect, 0.01f, _camera.farClipPlane);
		if (XRSettings.enabled)
		{
			proj = Camera.current.projectionMatrix;
		}
		proj = GL.GetGPUProjectionMatrix(proj, renderIntoTexture: true);
		_viewProj = proj * _camera.worldToCameraMatrix;
		_preLightPass.Clear();
		bool flag = SystemInfo.graphicsShaderLevel > 40;
		if (Resolution == VolumtericResolution.Quarter)
		{
			Texture source = null;
			_preLightPass.Blit(source, _halfDepthBuffer, _bilateralBlurMaterial, flag ? 4 : 10);
			_preLightPass.Blit(source, _quarterDepthBuffer, _bilateralBlurMaterial, flag ? 6 : 11);
			_preLightPass.SetRenderTarget(_quarterVolumeLightTexture);
		}
		else if (Resolution == VolumtericResolution.Half)
		{
			Texture source2 = null;
			_preLightPass.Blit(source2, _halfDepthBuffer, _bilateralBlurMaterial, flag ? 4 : 10);
			_preLightPass.SetRenderTarget(_halfVolumeLightTexture);
		}
		else
		{
			_preLightPass.SetRenderTarget(_volumeLightTexture);
		}
		_preLightPass.ClearRenderTarget(clearDepth: false, clearColor: true, new Color(0f, 0f, 0f, 1f));
		UpdateMaterialParameters();
		if (VolumetricLightRenderer.PreRenderEvent != null)
		{
			VolumetricLightRenderer.PreRenderEvent(this, _viewProj);
		}
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Resolution == VolumtericResolution.Quarter)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(_quarterDepthBuffer.width, _quarterDepthBuffer.height, 0, RenderTextureFormat.ARGBHalf);
			temporary.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_quarterVolumeLightTexture, temporary, _bilateralBlurMaterial, 8);
			Graphics.Blit(temporary, _quarterVolumeLightTexture, _bilateralBlurMaterial, 9);
			Graphics.Blit(_quarterVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 7);
			RenderTexture.ReleaseTemporary(temporary);
		}
		else if (Resolution == VolumtericResolution.Half)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(_halfVolumeLightTexture.width, _halfVolumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_halfVolumeLightTexture, temporary2, _bilateralBlurMaterial, 2);
			Graphics.Blit(temporary2, _halfVolumeLightTexture, _bilateralBlurMaterial, 3);
			Graphics.Blit(_halfVolumeLightTexture, _volumeLightTexture, _bilateralBlurMaterial, 5);
			RenderTexture.ReleaseTemporary(temporary2);
		}
		else
		{
			RenderTexture temporary3 = RenderTexture.GetTemporary(_volumeLightTexture.width, _volumeLightTexture.height, 0, RenderTextureFormat.ARGBHalf);
			temporary3.filterMode = FilterMode.Bilinear;
			Graphics.Blit(_volumeLightTexture, temporary3, _bilateralBlurMaterial, 0);
			Graphics.Blit(temporary3, _volumeLightTexture, _bilateralBlurMaterial, 1);
			RenderTexture.ReleaseTemporary(temporary3);
		}
		_blitAddMaterial.SetTexture("_Source", source);
		Graphics.Blit(_volumeLightTexture, destination, _blitAddMaterial, 0);
	}

	private void UpdateMaterialParameters()
	{
		_bilateralBlurMaterial.SetTexture("_HalfResDepthBuffer", _halfDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_HalfResColor", _halfVolumeLightTexture);
		_bilateralBlurMaterial.SetTexture("_QuarterResDepthBuffer", _quarterDepthBuffer);
		_bilateralBlurMaterial.SetTexture("_QuarterResColor", _quarterVolumeLightTexture);
		Shader.SetGlobalTexture("_DitherTexture", _ditheringTexture);
		Shader.SetGlobalTexture("_NoiseTexture", _noiseTexture);
	}

	private void Update()
	{
		if (_currentResolution != Resolution)
		{
			_currentResolution = Resolution;
			ChangeResolution();
		}
		if (_volumeLightTexture.width != _camera.pixelWidth || _volumeLightTexture.height != _camera.pixelHeight)
		{
			ChangeResolution();
		}
	}

	private void LoadNoise3dTexture()
	{
		TextAsset textAsset = Resources.Load("NoiseVolume") as TextAsset;
		byte[] bytes = textAsset.bytes;
		uint num = BitConverter.ToUInt32(textAsset.bytes, 12);
		uint num2 = BitConverter.ToUInt32(textAsset.bytes, 16);
		uint num3 = BitConverter.ToUInt32(textAsset.bytes, 20);
		uint num4 = BitConverter.ToUInt32(textAsset.bytes, 24);
		uint num5 = BitConverter.ToUInt32(textAsset.bytes, 80);
		uint num6 = BitConverter.ToUInt32(textAsset.bytes, 88);
		if (num6 == 0)
		{
			num6 = num3 / num2 * 8;
		}
		_noiseTexture = new Texture3D((int)num2, (int)num, (int)num4, TextureFormat.RGBA32, mipChain: false);
		_noiseTexture.name = "3D Noise";
		Color[] array = new Color[num2 * num * num4];
		uint num7 = 128u;
		if (textAsset.bytes[84] == 68 && textAsset.bytes[85] == 88 && textAsset.bytes[86] == 49 && textAsset.bytes[87] == 48 && (num5 & 4u) != 0)
		{
			uint num8 = BitConverter.ToUInt32(textAsset.bytes, (int)num7);
			if (num8 >= 60 && num8 <= 65)
			{
				num6 = 8u;
			}
			else if (num8 >= 48 && num8 <= 52)
			{
				num6 = 16u;
			}
			else if (num8 >= 27 && num8 <= 32)
			{
				num6 = 32u;
			}
			num7 += 20;
		}
		uint num9 = num6 / 8;
		num3 = (num2 * num6 + 7) / 8;
		for (int i = 0; i < num4; i++)
		{
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					float num10 = (float)(int)bytes[num7 + k * num9] / 255f;
					array[k + j * num2 + i * num2 * num] = new Color(num10, num10, num10, num10);
				}
				num7 += num3;
			}
		}
		_noiseTexture.SetPixels(array);
		_noiseTexture.Apply();
	}

	private void GenerateDitherTexture()
	{
		if (!(_ditheringTexture != null))
		{
			int num = 8;
			_ditheringTexture = new Texture2D(num, num, TextureFormat.Alpha8, mipChain: false, linear: true);
			_ditheringTexture.filterMode = FilterMode.Point;
			Color32[] array = new Color32[num * num];
			int num2 = 0;
			byte b = 3;
			array[num2++] = new Color32(b, b, b, b);
			b = 192;
			array[num2++] = new Color32(b, b, b, b);
			b = 51;
			array[num2++] = new Color32(b, b, b, b);
			b = 239;
			array[num2++] = new Color32(b, b, b, b);
			b = 15;
			array[num2++] = new Color32(b, b, b, b);
			b = 204;
			array[num2++] = new Color32(b, b, b, b);
			b = 62;
			array[num2++] = new Color32(b, b, b, b);
			b = 251;
			array[num2++] = new Color32(b, b, b, b);
			b = 129;
			array[num2++] = new Color32(b, b, b, b);
			b = 66;
			array[num2++] = new Color32(b, b, b, b);
			b = 176;
			array[num2++] = new Color32(b, b, b, b);
			b = 113;
			array[num2++] = new Color32(b, b, b, b);
			b = 141;
			array[num2++] = new Color32(b, b, b, b);
			b = 78;
			array[num2++] = new Color32(b, b, b, b);
			b = 188;
			array[num2++] = new Color32(b, b, b, b);
			b = 125;
			array[num2++] = new Color32(b, b, b, b);
			b = 35;
			array[num2++] = new Color32(b, b, b, b);
			b = 223;
			array[num2++] = new Color32(b, b, b, b);
			b = 19;
			array[num2++] = new Color32(b, b, b, b);
			b = 207;
			array[num2++] = new Color32(b, b, b, b);
			b = 47;
			array[num2++] = new Color32(b, b, b, b);
			b = 235;
			array[num2++] = new Color32(b, b, b, b);
			b = 31;
			array[num2++] = new Color32(b, b, b, b);
			b = 219;
			array[num2++] = new Color32(b, b, b, b);
			b = 160;
			array[num2++] = new Color32(b, b, b, b);
			b = 98;
			array[num2++] = new Color32(b, b, b, b);
			b = 145;
			array[num2++] = new Color32(b, b, b, b);
			b = 82;
			array[num2++] = new Color32(b, b, b, b);
			b = 172;
			array[num2++] = new Color32(b, b, b, b);
			b = 109;
			array[num2++] = new Color32(b, b, b, b);
			b = 156;
			array[num2++] = new Color32(b, b, b, b);
			b = 94;
			array[num2++] = new Color32(b, b, b, b);
			b = 11;
			array[num2++] = new Color32(b, b, b, b);
			b = 200;
			array[num2++] = new Color32(b, b, b, b);
			b = 58;
			array[num2++] = new Color32(b, b, b, b);
			b = 247;
			array[num2++] = new Color32(b, b, b, b);
			b = 7;
			array[num2++] = new Color32(b, b, b, b);
			b = 196;
			array[num2++] = new Color32(b, b, b, b);
			b = 54;
			array[num2++] = new Color32(b, b, b, b);
			b = 243;
			array[num2++] = new Color32(b, b, b, b);
			b = 137;
			array[num2++] = new Color32(b, b, b, b);
			b = 74;
			array[num2++] = new Color32(b, b, b, b);
			b = 184;
			array[num2++] = new Color32(b, b, b, b);
			b = 121;
			array[num2++] = new Color32(b, b, b, b);
			b = 133;
			array[num2++] = new Color32(b, b, b, b);
			b = 70;
			array[num2++] = new Color32(b, b, b, b);
			b = 180;
			array[num2++] = new Color32(b, b, b, b);
			b = 117;
			array[num2++] = new Color32(b, b, b, b);
			b = 43;
			array[num2++] = new Color32(b, b, b, b);
			b = 231;
			array[num2++] = new Color32(b, b, b, b);
			b = 27;
			array[num2++] = new Color32(b, b, b, b);
			b = 215;
			array[num2++] = new Color32(b, b, b, b);
			b = 39;
			array[num2++] = new Color32(b, b, b, b);
			b = 227;
			array[num2++] = new Color32(b, b, b, b);
			b = 23;
			array[num2++] = new Color32(b, b, b, b);
			b = 211;
			array[num2++] = new Color32(b, b, b, b);
			b = 168;
			array[num2++] = new Color32(b, b, b, b);
			b = 105;
			array[num2++] = new Color32(b, b, b, b);
			b = 153;
			array[num2++] = new Color32(b, b, b, b);
			b = 90;
			array[num2++] = new Color32(b, b, b, b);
			b = 164;
			array[num2++] = new Color32(b, b, b, b);
			b = 102;
			array[num2++] = new Color32(b, b, b, b);
			b = 149;
			array[num2++] = new Color32(b, b, b, b);
			b = 86;
			array[num2++] = new Color32(b, b, b, b);
			_ditheringTexture.SetPixels32(array);
			_ditheringTexture.Apply();
		}
	}

	private Mesh CreateSpotLightMesh()
	{
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[50];
		Color32[] array2 = new Color32[50];
		array[0] = new Vector3(0f, 0f, 0f);
		array[1] = new Vector3(0f, 0f, 1f);
		float num = 0f;
		float num2 = (float)Math.PI / 8f;
		float num3 = 0.9f;
		for (int i = 0; i < 16; i++)
		{
			array[i + 2] = new Vector3((0f - Mathf.Cos(num)) * num3, Mathf.Sin(num) * num3, num3);
			array2[i + 2] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			array[i + 2 + 16] = new Vector3(0f - Mathf.Cos(num), Mathf.Sin(num), 1f);
			array2[i + 2 + 16] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			array[i + 2 + 32] = new Vector3((0f - Mathf.Cos(num)) * num3, Mathf.Sin(num) * num3, 1f);
			array2[i + 2 + 32] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			num += num2;
		}
		mesh.vertices = array;
		mesh.colors32 = array2;
		int[] array3 = new int[288];
		int num4 = 0;
		for (int j = 2; j < 17; j++)
		{
			array3[num4++] = 0;
			array3[num4++] = j;
			array3[num4++] = j + 1;
		}
		array3[num4++] = 0;
		array3[num4++] = 17;
		array3[num4++] = 2;
		for (int k = 2; k < 17; k++)
		{
			array3[num4++] = k;
			array3[num4++] = k + 16;
			array3[num4++] = k + 1;
			array3[num4++] = k + 1;
			array3[num4++] = k + 16;
			array3[num4++] = k + 16 + 1;
		}
		array3[num4++] = 2;
		array3[num4++] = 17;
		array3[num4++] = 18;
		array3[num4++] = 18;
		array3[num4++] = 17;
		array3[num4++] = 33;
		for (int l = 18; l < 33; l++)
		{
			array3[num4++] = l;
			array3[num4++] = l + 16;
			array3[num4++] = l + 1;
			array3[num4++] = l + 1;
			array3[num4++] = l + 16;
			array3[num4++] = l + 16 + 1;
		}
		array3[num4++] = 18;
		array3[num4++] = 33;
		array3[num4++] = 34;
		array3[num4++] = 34;
		array3[num4++] = 33;
		array3[num4++] = 49;
		for (int m = 34; m < 49; m++)
		{
			array3[num4++] = 1;
			array3[num4++] = m + 1;
			array3[num4++] = m;
		}
		array3[num4++] = 1;
		array3[num4++] = 34;
		array3[num4++] = 49;
		mesh.triangles = array3;
		mesh.RecalculateBounds();
		return mesh;
	}
}
