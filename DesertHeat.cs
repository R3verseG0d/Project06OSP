using UnityEngine;

public class DesertHeat : MonoBehaviour
{
	public Material material;

	[Range(0.1f, 100f)]
	public float FocusDistance = 10f;

	[Range(0.1f, 10f)]
	public float FocusRange = 3f;

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		material.SetFloat("_FocusDistance", FocusDistance);
		material.SetFloat("_FocusRange", FocusRange);
		Graphics.Blit(source, destination, material);
	}
}
