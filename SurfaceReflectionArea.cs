using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class SurfaceReflectionArea : MonoBehaviour
{
	public int m_TextureSize = 2;

	public float m_clipPlaneOffset = 0.07f;

	public LayerMask m_ReflectLayers = -1;

	private Hashtable m_ReflectionCameras = new Hashtable();

	private RenderTexture m_ReflectionTexture;

	private int m_OldReflectionTextureSize;

	private void Start()
	{
		if (!base.enabled || !Application.isPlaying || Singleton<Settings>.Instance.settings.Reflections != 0)
		{
			return;
		}
		foreach (ReflectionArea reflectionArea in ReflectionSystem.instance.ReflectionAreas)
		{
			Material[] sharedMaterials = reflectionArea.Renderer.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				if (material.HasProperty("_ReflectionTex"))
				{
					if (Singleton<Settings>.Instance.settings.Reflections > 0)
					{
						material.EnableKeyword("PLANARREF");
					}
					else
					{
						material.DisableKeyword("PLANARREF");
					}
				}
			}
		}
	}

	private void Update()
	{
		if (!base.enabled || (Application.isPlaying && Singleton<Settings>.Instance.settings.Reflections == 0))
		{
			return;
		}
		Camera main = Camera.main;
		if (!main)
		{
			return;
		}
		CreateSurfaceObjects(main, out var reflectionCamera);
		ReflectionSystem instance = ReflectionSystem.instance;
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(main);
		ReflectionArea reflectionArea = null;
		Vector3 position = main.transform.position;
		float num = float.MaxValue;
		Vector3 vector = Vector3.zero;
		Vector3 up = Vector3.up;
		foreach (ReflectionArea reflectionArea2 in instance.ReflectionAreas)
		{
			if (GeometryUtility.TestPlanesAABB(planes, reflectionArea2.bounds))
			{
				float num2 = Vector3.Distance(reflectionArea2.transform.position, position);
				if (num2 < num)
				{
					reflectionArea = reflectionArea2;
					vector = reflectionArea2.transform.position;
					up = reflectionArea2.transform.up;
					num = num2;
				}
			}
		}
		if (reflectionArea == null || reflectionArea.Renderer == null || !reflectionArea.Renderer.enabled)
		{
			return;
		}
		Material[] sharedMaterials = reflectionArea.Renderer.sharedMaterials;
		foreach (Material material in sharedMaterials)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				if (Singleton<Settings>.Instance.settings.Reflections > 0)
				{
					material.EnableKeyword("PLANARREF");
					material.SetTexture("_ReflectionTex", m_ReflectionTexture);
				}
				else
				{
					material.DisableKeyword("PLANARREF");
				}
			}
		}
		UpdateCameraModes(main, reflectionCamera);
		float w = 0f - Vector3.Dot(up, vector) - m_clipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 reflectionMat = Matrix4x4.zero;
		CalculateReflectionMatrix(ref reflectionMat, plane);
		Vector3 position2 = main.transform.position;
		Vector3 position3 = reflectionMat.MultiplyPoint(position2);
		reflectionCamera.worldToCameraMatrix = main.worldToCameraMatrix * reflectionMat;
		Vector4 clipPlane = CameraSpacePlane(reflectionCamera, vector, up, 1f);
		Matrix4x4 projection = main.projectionMatrix;
		CalculateObliqueMatrix(ref projection, clipPlane);
		reflectionCamera.projectionMatrix = projection;
		reflectionCamera.cullingMask = -17 & m_ReflectLayers.value;
		GL.invertCulling = true;
		reflectionCamera.transform.position = position3;
		Vector3 eulerAngles = main.transform.eulerAngles;
		reflectionCamera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		reflectionCamera.Render();
		reflectionCamera.transform.position = position2;
		GL.invertCulling = false;
	}

	private void OnDisable()
	{
		if ((bool)m_ReflectionTexture)
		{
			Object.DestroyImmediate(m_ReflectionTexture);
			m_ReflectionTexture = null;
		}
		foreach (DictionaryEntry reflectionCamera in m_ReflectionCameras)
		{
			Object.DestroyImmediate(((Camera)reflectionCamera.Value).gameObject);
		}
		m_ReflectionCameras.Clear();
	}

	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.renderingPath = src.renderingPath;
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	private void CreateSurfaceObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
		{
			if ((bool)m_ReflectionTexture)
			{
				Object.DestroyImmediate(m_ReflectionTexture);
			}
			float num = (float)m_TextureSize / 10f;
			m_ReflectionTexture = new RenderTexture(Mathf.RoundToInt((float)currentCamera.pixelWidth * num), Mathf.RoundToInt((float)currentCamera.pixelHeight * num), 16);
			m_ReflectionTexture.name = "_SurfaceReflection" + GetInstanceID();
			m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			m_OldReflectionTextureSize = m_TextureSize;
		}
		reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Surface Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = base.transform.position;
			reflectionCamera.transform.rotation = base.transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			m_ReflectionCameras[currentCamera] = reflectionCamera;
			reflectionCamera.targetTexture = m_ReflectionTexture;
			reflectionCamera.useOcclusionCulling = false;
		}
	}

	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * m_clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(rhs.x, rhs.y, rhs.z, 0f - Vector3.Dot(lhs, rhs));
	}

	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}
}
