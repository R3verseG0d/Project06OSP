using UnityEngine;

public class FadeRenderer : MonoBehaviour
{
	public float FadeSpeed = 1f;

	private Renderer Renderer;

	private void Awake()
	{
		Renderer = GetComponent<Renderer>();
	}

	private void Update()
	{
		Material[] materials = Renderer.materials;
		foreach (Material material in materials)
		{
			material.SetFloat("_Intensity", Mathf.Lerp(material.GetFloat("_Intensity"), 0f, Time.deltaTime * FadeSpeed));
		}
	}
}
