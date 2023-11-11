using UnityEngine;

public class PrincessEffects : EffectsBase
{
	[Header("Framework")]
	public Color ShieldOrangeGlow;

	public Color TrailColor;

	public Color ShieldOrangeTrailColor;

	public TrailRenderer Trail;

	[Header("Princess Particles")]
	public ParticleSystem[] ShieldOrangeParticles;

	public ParticleSystem[] jumpDashParticles;

	public ParticleSystem[] ShieldOrangeJumpDashParticles;

	public ParticleSystem[] SlideParticles;

	public ParticleSystem[] ShieldOrangeSlideParticles;

	public ParticleSystem[] LightDashParticles;

	[Header("Instantiation")]
	public GameObject ShieldOrangeJumpDashStartPrefab;

	public GameObject ShieldGreenJumpDashStartPrefab;

	public GameObject ShieldOrangeStartPrefab;

	public GameObject ShieldGreenStartPrefab;

	private float TrailAlpha;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public override void Update()
	{
		base.Update();
		for (int i = 0; i < ShieldOrangeParticles.Length; i++)
		{
			ParticleSystem.EmissionModule emission = ShieldOrangeParticles[i].emission;
			emission.enabled = PM.princess.UsingShield;
		}
		for (int j = 0; j < PM.princess.PlayerRenderers.Length; j++)
		{
			PM.princess.PlayerRenderers[j].GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_OutlineColor", ShieldOrangeGlow);
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", PM.princess.UsingShield ? 1f : 0f);
			PM.princess.PlayerRenderers[j].SetPropertyBlock(PropBlock);
		}
		if (PM.princess.Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int k = 0; k < PM.princess.Upgrades.Renderers.Count; k++)
			{
				PM.princess.Upgrades.Renderers[k].GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_OutlineColor", ShieldOrangeGlow);
				PropBlock.SetFloat("_OutlinePulseSpd", 0f);
				PropBlock.SetFloat("_OutlineInt", PM.princess.UsingShield ? 1f : 0f);
				PM.princess.Upgrades.Renderers[k].SetPropertyBlock(PropBlock);
			}
		}
		for (int l = 0; l < jumpDashParticles.Length; l++)
		{
			ParticleSystem.EmissionModule emission2 = jumpDashParticles[l].emission;
			emission2.enabled = !PM.princess.UsingShield && (PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing");
		}
		for (int m = 0; m < ShieldOrangeJumpDashParticles.Length; m++)
		{
			ParticleSystem.EmissionModule emission3 = ShieldOrangeJumpDashParticles[m].emission;
			emission3.enabled = PM.princess.UsingShield && (PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing");
		}
		Gradient gradient = new Gradient();
		TrailAlpha = ((PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing") ? 1f : Mathf.Lerp(TrailAlpha, 0f, Time.deltaTime * 10f));
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(PM.princess.UsingShield ? ShieldOrangeTrailColor : TrailColor, 0f),
			new GradientColorKey(PM.princess.UsingShield ? ShieldOrangeTrailColor : TrailColor, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(TrailAlpha, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Trail.colorGradient = gradient;
		for (int n = 0; n < SlideParticles.Length; n++)
		{
			ParticleSystem.EmissionModule emission4 = SlideParticles[n].emission;
			emission4.enabled = !PM.princess.UsingShield && PM.Base.GetState() == "Slide";
		}
		for (int num = 0; num < ShieldOrangeSlideParticles.Length; num++)
		{
			ParticleSystem.EmissionModule emission5 = ShieldOrangeSlideParticles[num].emission;
			emission5.enabled = PM.princess.UsingShield && PM.Base.GetState() == "Slide";
		}
		for (int num2 = 0; num2 < LightDashParticles.Length; num2++)
		{
			ParticleSystem.EmissionModule emission6 = LightDashParticles[num2].emission;
			emission6.enabled = PM.Base.GetState() == "LightDash";
		}
	}

	public void CreateShieldFX()
	{
		Object.Instantiate(ShieldOrangeStartPrefab, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}
}
