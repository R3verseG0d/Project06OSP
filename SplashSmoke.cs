using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SplashSmoke : MonoBehaviour
{
	public Material material;

	private Vector2 offsetA;

	private Vector2 offsetB;

	private Vector2 offsetC;

	private Vector2 dirA;

	private Vector2 dirB;

	private Vector2 dirC;

	private Vector2 dirSlerpA;

	private Vector2 dirSlerpB;

	private Vector2 dirSlerpC;

	private void Start()
	{
		CreateMaterials();
	}

	private void CreateMaterials()
	{
		if (material != null)
		{
			InvokeRepeating("UpdateDirectionA", 0f, 0.5f);
			InvokeRepeating("UpdateDirectionB", 0f, 1.5f);
			InvokeRepeating("UpdateDirectionC", 0f, 1f);
		}
	}

	public void UpdateDirectionA()
	{
		dirA = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		dirA.Normalize();
	}

	public void UpdateDirectionB()
	{
		dirB = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		dirB.Normalize();
	}

	public void UpdateDirectionC()
	{
		dirC = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		dirC.Normalize();
	}

	public void Update()
	{
		dirSlerpA = Vector2.Lerp(dirSlerpA, dirA, Time.deltaTime).normalized;
		dirSlerpB = Vector2.Lerp(dirSlerpB, dirB, Time.deltaTime).normalized;
		dirSlerpC = Vector2.Lerp(dirSlerpC, dirC, Time.deltaTime).normalized;
		offsetA += dirSlerpA * 0.25f * Time.deltaTime;
		offsetB += dirSlerpB * 0.15f * Time.deltaTime;
		offsetC += dirSlerpC * 0.05f * Time.deltaTime;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CreateMaterials();
		material.SetVector("_UVOffsetA", offsetA);
		material.SetVector("_UVOffsetB", offsetB);
		material.SetVector("_UVOffsetC", offsetC);
		Graphics.Blit(source, destination, material);
	}
}
