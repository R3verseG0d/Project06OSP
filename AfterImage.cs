using UnityEngine;
using UnityEngine.Rendering;

public class AfterImage : MonoBehaviour
{
	[Header("Framework")]
	public bool Emit;

	public SkinnedMeshRenderer SkinnedMesh;

	[Header("Positioning variables")]
	public Vector3 RotationOffset;

	[Header("Material settings")]
	public Material CustomMaterial;

	public float FadeSpeed = -0.15f;

	[Header("Time variables")]
	public float Delay;

	public float LifeSpan;

	private Material[] Mats;

	private GameObject PreviousFrameObj;

	private float DelayTimer;

	private float FadeIntensity;

	private float Count;

	private void Awake()
	{
		Mats = new Material[SkinnedMesh.materials.Length];
		if ((bool)CustomMaterial)
		{
			for (int i = 0; i < Mats.Length; i++)
			{
				Mats[i] = CustomMaterial;
			}
		}
		else
		{
			Mats = SkinnedMesh.materials;
		}
	}

	private void Update()
	{
		if (!Emit)
		{
			return;
		}
		if (Delay > 0f)
		{
			DelayTimer += Time.deltaTime;
		}
		else
		{
			DelayTimer = 1f;
		}
		if (DelayTimer > Delay)
		{
			Count += 1f;
			_ = $"AfterImage{Count}";
			Mesh mesh = new Mesh();
			SkinnedMesh.BakeMesh(mesh);
			GameObject gameObject = new GameObject();
			gameObject.transform.position = base.transform.position;
			gameObject.transform.rotation = base.transform.rotation * Quaternion.Euler(RotationOffset);
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.materials = Mats;
			gameObject.AddComponent<FadeRenderer>().FadeSpeed = FadeSpeed;
			Object.Destroy(gameObject, LifeSpan);
			PreviousFrameObj = gameObject;
			if (Delay > 0f)
			{
				DelayTimer = 0f;
			}
		}
	}

	private void OnDisable()
	{
		if (Emit)
		{
			PreviousFrameObj = null;
			Count = 0f;
		}
	}
}
