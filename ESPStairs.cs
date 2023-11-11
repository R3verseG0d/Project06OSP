using UnityEngine;

public class ESPStairs : PsiObject
{
	[Header("Prefab")]
	public Animator Animator;

	public Renderer Renderer;

	public ParticleSystem[] DebrisFX;

	public AudioSource[] DebrisAudio;

	private bool Triggered;

	private void Update()
	{
		OnPsiFX(Renderer, Triggered);
	}

	private void PlayDebrisID(int Index)
	{
		DebrisFX[Index].Play();
		DebrisAudio[Index].Play();
	}

	private void OnEventSignal()
	{
		if (!Triggered)
		{
			Animator.SetTrigger("On Trigger");
			Triggered = true;
		}
	}
}
