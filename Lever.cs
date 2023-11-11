using UnityEngine;

public class Lever : EventStation
{
	[Header("Framework")]
	public string Event;

	[Header("Prefab")]
	public Renderer Renderer;

	public Gradient HelpColor;

	public ParticleSystem HelpFX;

	public ParticleSystem[] FX;

	public Animator Animator;

	public AudioSource Audio;

	public GameObject Target;

	internal bool IsCaged;

	private MaterialPropertyBlock PropBlock;

	private bool IsActivated;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Update()
	{
		Renderer.GetPropertyBlock(PropBlock);
		PropBlock.SetColor("_ExtFresColor", HelpColor.Evaluate(Mathf.Repeat(Time.time, 0.75f) / 0.75f));
		PropBlock.SetFloat("_ExtPulseSpd", 0f);
		PropBlock.SetFloat("_ExtFresPower", 1.5f);
		PropBlock.SetFloat("_ExtFresThre", (!IsActivated) ? 1.5f : 0f);
		PropBlock.SetColor("_OutlineColor", HelpColor.Evaluate(Mathf.Repeat(Time.time, 0.75f) / 0.75f));
		PropBlock.SetFloat("_OutlinePulseSpd", 0f);
		PropBlock.SetFloat("_OutlineInt", (!IsActivated) ? 1f : 0f);
		Renderer.SetPropertyBlock(PropBlock);
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!IsActivated && !IsCaged)
		{
			OnActivate(HitInfo.player);
		}
	}

	private void OnActivate(Transform Player)
	{
		if ((bool)GetPlayer(Player))
		{
			HelpFX.Stop();
			for (int i = 0; i < FX.Length; i++)
			{
				FX[i].Play();
			}
			Animator.SetTrigger("On Trigger");
			Audio.Play();
			Target.SetActive(value: false);
			CallEvent(Event);
			IsActivated = true;
		}
	}
}
