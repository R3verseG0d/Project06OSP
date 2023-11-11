using UnityEngine;

public class SnowBoardEffects : EffectsBase
{
	[Header("Framework")]
	public Material[] NormalGemShoeMats;

	[Header("Snow Board Particles")]
	public ParticleSystem PerfectRampFX;

	public override void Start()
	{
		base.Start();
		for (int i = 0; i < NormalGemShoeMats.Length; i++)
		{
			NormalGemShoeMats[i].SetFloat("_GlowInt", 0f);
		}
	}

	public void CreatePerfectRampFX()
	{
		PerfectRampFX.Play();
	}
}
