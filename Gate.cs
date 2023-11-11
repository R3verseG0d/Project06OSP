using UnityEngine;

public class Gate : PsiObject
{
	[Header("Framework")]
	public Animator Animator;

	public Collider Collider;

	public Renderer Renderer;

	public GameObject SFX;

	public ParticleSystem[] FX;

	private bool Opened;

	private float PsiTime;

	private void Update()
	{
		if (Opened)
		{
			OnPsiFX(Renderer, Time.time - PsiTime < 4f);
			Animator.SetBool("Opened", Opened);
		}
	}

	private void OnPsychokinesis(Transform PlayerPos)
	{
		if (!Opened)
		{
			Opened = true;
			Animator.SetTrigger("On Trigger");
			Collider.enabled = false;
			SFX.SetActive(value: true);
			for (int i = 0; i < FX.Length; i++)
			{
				FX[i].Play(withChildren: true);
			}
			PsiTime = Time.time;
		}
	}
}
