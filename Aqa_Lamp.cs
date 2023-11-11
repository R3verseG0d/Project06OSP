using UnityEngine;

public class Aqa_Lamp : MonoBehaviour
{
	[Header("Prefab")]
	public Renderer Renderer;

	public ParticleSystem FX;

	public GameObject Lights;

	public Animation Animation;

	private MaterialPropertyBlock PropBlock;

	private bool Inactive;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void OnEventSignal()
	{
		Inactive = !Inactive;
		Renderer.GetPropertyBlock(PropBlock, 1);
		PropBlock.SetFloat("_EmissionGain", (!Inactive) ? 1f : 0f);
		Renderer.SetPropertyBlock(PropBlock, 1);
		ParticleSystem.EmissionModule emission = FX.emission;
		emission.enabled = !Inactive;
		Lights.SetActive(!Inactive);
		Animation.enabled = !Inactive;
	}
}
