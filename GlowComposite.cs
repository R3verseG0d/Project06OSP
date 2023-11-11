using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowComposite : MonoBehaviour
{
	private Material CompositeMat;

	private void OnEnable()
	{
		CompositeMat = new Material(Shader.Find("Hidden/GlowComposite"));
	}

	private void OnRenderImage(RenderTexture Src, RenderTexture Dst)
	{
		Graphics.Blit(Src, Dst, CompositeMat, 0);
	}
}
