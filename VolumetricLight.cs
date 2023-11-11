using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Light))]
public class VolumetricLight : MonoBehaviour
{
	private Light _light;

	private Material _material;

	private CommandBuffer _commandBuffer;

	private CommandBuffer _cascadeShadowCommandBuffer;

	[Range(1f, 64f)]
	public int SampleCount = 8;

	[Range(0f, 1f)]
	public float ScatteringCoef = 0.5f;

	[Range(0f, 0.1f)]
	public float ExtinctionCoef = 0.01f;

	[Range(0f, 1f)]
	public float SkyboxExtinctionCoef = 0.9f;

	[Range(0f, 0.999f)]
	public float MieG = 0.1f;

	public bool HeightFog;

	[Range(0f, 0.5f)]
	public float HeightScale = 0.1f;

	public float GroundLevel;

	public bool Noise;

	public float NoiseScale = 0.015f;

	public float NoiseIntensity = 1f;

	public float NoiseIntensityOffset = 0.3f;

	public Vector2 NoiseVelocity = new Vector2(3f, 3f);

	[Tooltip("")]
	public float MaxRayLength = 400f;

	private Vector4[] _frustumCorners = new Vector4[4];

	private bool _reversedZ;

	public Light Light => _light;

	public Material VolumetricMaterial => _material;

	public event Action<VolumetricLightRenderer, VolumetricLight, CommandBuffer, Matrix4x4> CustomRenderEvent;

	private void Start()
	{
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan || SystemInfo.graphicsDeviceType == GraphicsDeviceType.XboxOne)
		{
			_reversedZ = true;
		}
		_commandBuffer = new CommandBuffer();
		_commandBuffer.name = "Light Command Buffer";
		_cascadeShadowCommandBuffer = new CommandBuffer();
		_cascadeShadowCommandBuffer.name = "Dir Light Command Buffer";
		_cascadeShadowCommandBuffer.SetGlobalTexture("_CascadeShadowMapTexture", new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive));
		_light = GetComponent<Light>();
		if (_light.type == LightType.Directional)
		{
			_light.AddCommandBuffer(LightEvent.BeforeScreenspaceMask, _commandBuffer);
			_light.AddCommandBuffer(LightEvent.AfterShadowMap, _cascadeShadowCommandBuffer);
		}
		else
		{
			_light.AddCommandBuffer(LightEvent.AfterShadowMap, _commandBuffer);
		}
		Shader shader = Shader.Find("Sandbox/VolumetricLight");
		if (shader == null)
		{
			throw new Exception("Critical Error: \"Sandbox/VolumetricLight\" shader is missing. Make sure it is included in \"Always Included Shaders\" in ProjectSettings/Graphics.");
		}
		_material = new Material(shader);
	}

	private void OnEnable()
	{
		VolumetricLightRenderer.PreRenderEvent += VolumetricLightRenderer_PreRenderEvent;
	}

	private void OnDisable()
	{
		VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
	}

	public void OnDestroy()
	{
		UnityEngine.Object.Destroy(_material);
	}

	private void VolumetricLightRenderer_PreRenderEvent(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		if (_light == null || _light.gameObject == null)
		{
			VolumetricLightRenderer.PreRenderEvent -= VolumetricLightRenderer_PreRenderEvent;
		}
		if (_light.gameObject.activeInHierarchy && _light.enabled)
		{
			_material.SetVector("_CameraForward", Camera.current.transform.forward);
			_material.SetInt("_SampleCount", SampleCount);
			_material.SetVector("_NoiseVelocity", new Vector4(NoiseVelocity.x, NoiseVelocity.y) * NoiseScale);
			_material.SetVector("_NoiseData", new Vector4(NoiseScale, NoiseIntensity, NoiseIntensityOffset));
			_material.SetVector("_MieG", new Vector4(1f - MieG * MieG, 1f + MieG * MieG, 2f * MieG, 1f / (4f * (float)Math.PI)));
			_material.SetVector("_VolumetricLight", new Vector4(ScatteringCoef, ExtinctionCoef, _light.range, 1f - SkyboxExtinctionCoef));
			_material.SetTexture("_CameraDepthTexture", renderer.GetVolumeLightDepthBuffer());
			_material.SetFloat("_ZTest", 8f);
			if (HeightFog)
			{
				_material.EnableKeyword("HEIGHT_FOG");
				_material.SetVector("_HeightFog", new Vector4(GroundLevel, HeightScale));
			}
			else
			{
				_material.DisableKeyword("HEIGHT_FOG");
			}
			if (_light.type == LightType.Point)
			{
				SetupPointLight(renderer, viewProj);
			}
			else if (_light.type == LightType.Spot)
			{
				SetupSpotLight(renderer, viewProj);
			}
			else if (_light.type == LightType.Directional)
			{
				SetupDirectionalLight(renderer, viewProj);
			}
		}
	}

	private void SetupPointLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		_commandBuffer.Clear();
		int num = 0;
		if (!IsCameraInPointLightBounds())
		{
			num = 2;
		}
		_material.SetPass(num);
		Mesh pointLightMesh = VolumetricLightRenderer.GetPointLightMesh();
		float num2 = _light.range * 2f;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, _light.transform.rotation, new Vector3(num2, num2, num2));
		_material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		_material.SetMatrix("_WorldView", Camera.current.worldToCameraMatrix * matrix4x);
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		_material.SetVector("_LightPos", new Vector4(_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1f / (_light.range * _light.range)));
		_material.SetColor("_LightColor", _light.color * _light.intensity);
		if (_light.cookie == null)
		{
			_material.EnableKeyword("POINT");
			_material.DisableKeyword("POINT_COOKIE");
		}
		else
		{
			Matrix4x4 inverse = Matrix4x4.TRS(_light.transform.position, _light.transform.rotation, Vector3.one).inverse;
			_material.SetMatrix("_MyLightMatrix0", inverse);
			_material.EnableKeyword("POINT_COOKIE");
			_material.DisableKeyword("POINT");
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		bool flag = false;
		if ((_light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (_light.shadows != 0 && !flag)
		{
			_material.EnableKeyword("SHADOWS_CUBE");
			_commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			_commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
			_commandBuffer.DrawMesh(pointLightMesh, matrix4x, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_CUBE");
			renderer.GlobalCommandBuffer.DrawMesh(pointLightMesh, matrix4x, _material, 0, num);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupSpotLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		_commandBuffer.Clear();
		int shaderPass = 1;
		if (!IsCameraInSpotLightBounds())
		{
			shaderPass = 3;
		}
		Mesh spotLightMesh = VolumetricLightRenderer.GetSpotLightMesh();
		float range = _light.range;
		float num = Mathf.Tan((_light.spotAngle + 1f) * 0.5f * ((float)Math.PI / 180f)) * _light.range;
		Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, new Vector3(num, num, range));
		Matrix4x4 inverse = Matrix4x4.TRS(_light.transform.position, _light.transform.rotation, Vector3.one).inverse;
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(-0.5f, -0.5f, 1f));
		Matrix4x4 matrix4x3 = Matrix4x4.Perspective(_light.spotAngle, 1f, 0f, 1f);
		_material.SetMatrix("_MyLightMatrix0", matrix4x2 * matrix4x3 * inverse);
		_material.SetMatrix("_WorldViewProj", viewProj * matrix4x);
		_material.SetVector("_LightPos", new Vector4(_light.transform.position.x, _light.transform.position.y, _light.transform.position.z, 1f / (_light.range * _light.range)));
		_material.SetVector("_LightColor", _light.color * _light.intensity);
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		float value = 0f - Vector3.Dot(position + forward * _light.range, forward);
		_material.SetFloat("_PlaneD", value);
		_material.SetFloat("_CosAngle", Mathf.Cos((_light.spotAngle + 1f) * 0.5f * ((float)Math.PI / 180f)));
		_material.SetVector("_ConeApex", new Vector4(position.x, position.y, position.z));
		_material.SetVector("_ConeAxis", new Vector4(forward.x, forward.y, forward.z));
		_material.EnableKeyword("SPOT");
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		if (_light.cookie == null)
		{
			_material.SetTexture("_LightTexture0", VolumetricLightRenderer.GetDefaultSpotCookie());
		}
		else
		{
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		bool flag = false;
		if ((_light.transform.position - Camera.current.transform.position).magnitude >= QualitySettings.shadowDistance)
		{
			flag = true;
		}
		if (_light.shadows != 0 && !flag)
		{
			matrix4x2 = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
			matrix4x3 = ((!_reversedZ) ? Matrix4x4.Perspective(_light.spotAngle, 1f, _light.shadowNearPlane, _light.range) : Matrix4x4.Perspective(_light.spotAngle, 1f, _light.range, _light.shadowNearPlane));
			Matrix4x4 matrix4x4 = matrix4x2 * matrix4x3;
			matrix4x4[0, 2] *= -1f;
			matrix4x4[1, 2] *= -1f;
			matrix4x4[2, 2] *= -1f;
			matrix4x4[3, 2] *= -1f;
			_material.SetMatrix("_MyWorld2Shadow", matrix4x4 * inverse);
			_material.SetMatrix("_WorldView", matrix4x4 * inverse);
			_material.EnableKeyword("SHADOWS_DEPTH");
			_commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
			_commandBuffer.SetRenderTarget(renderer.GetVolumeLightBuffer());
			_commandBuffer.DrawMesh(spotLightMesh, matrix4x, _material, 0, shaderPass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.DrawMesh(spotLightMesh, matrix4x, _material, 0, shaderPass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private void SetupDirectionalLight(VolumetricLightRenderer renderer, Matrix4x4 viewProj)
	{
		_commandBuffer.Clear();
		int pass = 4;
		_material.SetPass(pass);
		if (Noise)
		{
			_material.EnableKeyword("NOISE");
		}
		else
		{
			_material.DisableKeyword("NOISE");
		}
		_material.SetVector("_LightDir", new Vector4(_light.transform.forward.x, _light.transform.forward.y, _light.transform.forward.z, 1f / (_light.range * _light.range)));
		_material.SetVector("_LightColor", _light.color * _light.intensity);
		_material.SetFloat("_MaxRayLength", MaxRayLength);
		if (_light.cookie == null)
		{
			_material.EnableKeyword("DIRECTIONAL");
			_material.DisableKeyword("DIRECTIONAL_COOKIE");
		}
		else
		{
			_material.EnableKeyword("DIRECTIONAL_COOKIE");
			_material.DisableKeyword("DIRECTIONAL");
			_material.SetTexture("_LightTexture0", _light.cookie);
		}
		_frustumCorners[0] = Camera.current.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.current.farClipPlane));
		_frustumCorners[2] = Camera.current.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.current.farClipPlane));
		_frustumCorners[3] = Camera.current.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.current.farClipPlane));
		_frustumCorners[1] = Camera.current.ViewportToWorldPoint(new Vector3(1f, 0f, Camera.current.farClipPlane));
		_material.SetVectorArray("_FrustumCorners", _frustumCorners);
		Texture source = null;
		if (_light.shadows != 0)
		{
			_material.EnableKeyword("SHADOWS_DEPTH");
			_commandBuffer.Blit(source, renderer.GetVolumeLightBuffer(), _material, pass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, _commandBuffer, viewProj);
			}
		}
		else
		{
			_material.DisableKeyword("SHADOWS_DEPTH");
			renderer.GlobalCommandBuffer.Blit(source, renderer.GetVolumeLightBuffer(), _material, pass);
			if (this.CustomRenderEvent != null)
			{
				this.CustomRenderEvent(renderer, this, renderer.GlobalCommandBuffer, viewProj);
			}
		}
	}

	private bool IsCameraInPointLightBounds()
	{
		float sqrMagnitude = (_light.transform.position - Camera.current.transform.position).sqrMagnitude;
		float num = _light.range + 1f;
		if (sqrMagnitude < num * num)
		{
			return true;
		}
		return false;
	}

	private bool IsCameraInSpotLightBounds()
	{
		float num = Vector3.Dot(_light.transform.forward, Camera.current.transform.position - _light.transform.position);
		float num2 = _light.range + 1f;
		if (num > num2)
		{
			return false;
		}
		if (Mathf.Acos(Vector3.Dot(base.transform.forward, (Camera.current.transform.position - _light.transform.position).normalized)) * 57.29578f > (_light.spotAngle + 3f) * 0.5f)
		{
			return false;
		}
		return true;
	}
}
