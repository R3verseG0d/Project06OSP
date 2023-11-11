using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GlowPrePass : MonoBehaviour
{
	public bool OverrideBlurSize;

	public float OverrideValue;

	private static RenderTexture PrePass;

	private static RenderTexture Blurred;

	private Material BlurMat;

	private void OnEnable()
	{
		PrePass = new RenderTexture(Screen.width, Screen.height, 24);
		PrePass.antiAliasing = QualitySettings.antiAliasing;
		Blurred = new RenderTexture(Screen.width >> 1, Screen.height >> 1, 0);
		Camera component = GetComponent<Camera>();
		Shader shader = Shader.Find("Hidden/GlowReplace");
		component.targetTexture = PrePass;
		component.SetReplacementShader(shader, "RenderType");
		Shader.SetGlobalTexture("_GlowPrePassTex", PrePass);
		Shader.SetGlobalTexture("_GlowBlurredTex", Blurred);
		BlurMat = new Material(Shader.Find("Hidden/Blur"));
		float num = 0.75f;
		if (OverrideBlurSize)
		{
			num = OverrideValue;
		}
		BlurMat.SetVector("_BlurSize", new Vector2(Blurred.texelSize.x * num, Blurred.texelSize.y * num));
	}

	private void OnRenderImage(RenderTexture Src, RenderTexture Dst)
	{
		Graphics.Blit(Src, Dst);
		Graphics.SetRenderTarget(Blurred);
		GL.Clear(clearDepth: false, clearColor: true, Color.clear);
		Graphics.Blit(Src, Blurred);
		for (int i = 0; i < 4; i++)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
			Graphics.Blit(Blurred, temporary, BlurMat, 0);
			Graphics.Blit(temporary, Blurred, BlurMat, 1);
			RenderTexture.ReleaseTemporary(temporary);
		}
	}
}
