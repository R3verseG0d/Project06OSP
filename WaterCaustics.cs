using UnityEngine;

public class WaterCaustics : MonoBehaviour
{
	public float FPS = 30f;

	public Texture2D[] Frames;

	private int FrameIndex;

	private Projector Proj;

	private void Start()
	{
		Proj = GetComponent<Projector>();
		NextFrame();
		InvokeRepeating("NextFrame", 1f / FPS, 1f / FPS);
	}

	private void NextFrame()
	{
		Proj.material.SetTexture("_MainTex", Frames[FrameIndex]);
		FrameIndex = (FrameIndex + 1) % Frames.Length;
	}
}
