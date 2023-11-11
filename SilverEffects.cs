using UnityEngine;

public class SilverEffects : EffectsBase
{
	[Header("Framework")]
	public Color[] PsiGlows;

	public Transform[] PsiPoints;

	public Material FlashMat;

	[Header("Silver Particles")]
	public ParticleSystem[] PsiAuraFX;

	public ParticleSystem[] PsiLeftHandFX;

	public ParticleSystem[] PsiRightHandFX;

	public ParticleSystem SmashFullChargeFX;

	public ParticleSystem[] PsychicChargeFX;

	public ParticleSystem[] PsychicChargeFullFX;

	public ParticleSystem UpgradeGetFX;

	public ParticleSystem[] ESPTriggerFX;

	public ParticleSystem[] ESPFX;

	[Header("Instantiation")]
	public GameObject PsiTrailFX;

	public GameObject UpheaveFX;

	public GameObject PsychoSmashFX;

	public GameObject GrabAllActivateFX;

	public GameObject GrabAllFX;

	public GameObject TeleDashFX;

	public GameObject PsychoShockFX;

	public GameObject PsiDeflectFX;

	public GameObject ESPActivateFX;

	private float FlashInt;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public override void Start()
	{
		base.Start();
		FlashMat.SetFloat("_Intensity", 0f);
	}

	public override void Update()
	{
		base.Update();
		bool flag = PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1;
		UpdateJumpBallFX(flag && PM.silver.HasLotusOfResilience);
		for (int i = 0; i < PsiAuraFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = PsiAuraFX[i].emission;
			emission.enabled = PM.silver.UsePsiElements;
		}
		for (int j = 0; j < PsiRightHandFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = PsiRightHandFX[j].emission;
			emission2.enabled = PM.silver.UsePsiElements && ((PM.Base.GetState() == "Ground" && PM.Base.CurSpeed == 0f) || (PM.Base.GetState() == "PsychoSmash" && PM.silver.PSmashState == 0) || PM.Base.GetState() == "GrabAll" || (PM.Base.GetState() == "PsychicShot" && !PM.silver.FirePsychicShot));
		}
		for (int k = 0; k < PsiLeftHandFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission3 = PsiLeftHandFX[k].emission;
			emission3.enabled = PM.silver.UsePsiElements && ((PM.Base.GetState() == "Ground" && PM.silver.PickedObjects.Count != 0 && PM.Base.CurSpeed == 0f) || PM.Base.GetState() == "GrabAll" || (PM.Base.GetState() == "PsychicShot" && !PM.silver.FirePsychicShot));
		}
		for (int l = 0; l < PM.silver.PlayerRenderers.Length; l++)
		{
			PM.silver.PlayerRenderers[l].GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_ExtFresColor", PsiGlows[0]);
			PropBlock.SetColor("_ExtGlowColor", PsiGlows[1]);
			PropBlock.SetFloat("_ExtPulseSpd", 1f);
			PropBlock.SetFloat("_ExtFresPower", 1f);
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), PM.silver.UsePsiElements ? 0.75f : 0f, Time.deltaTime * 10f));
			PropBlock.SetFloat("_GlowInt", Mathf.Lerp(PropBlock.GetFloat("_GlowInt"), PM.silver.IsAwakened ? 1f : 0f, Time.deltaTime * 10f));
			PropBlock.SetColor("_OutlineColor", PsiGlows[0]);
			PropBlock.SetColor("_OutlinePulseColor", PsiGlows[1]);
			PropBlock.SetFloat("_OutlinePulseSpd", 1f);
			PropBlock.SetFloat("_OutlineInt", PM.silver.UsePsiElements ? 1f : ((!PM.silver.IsAwakened) ? 0f : 0.5f));
			PM.silver.PlayerRenderers[l].SetPropertyBlock(PropBlock);
		}
		if (PM.silver.Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int m = 0; m < PM.silver.Upgrades.Renderers.Count; m++)
			{
				PM.silver.Upgrades.Renderers[m].GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_ExtFresColor", PsiGlows[0]);
				PropBlock.SetColor("_ExtGlowColor", PsiGlows[1]);
				PropBlock.SetFloat("_ExtPulseSpd", 1f);
				PropBlock.SetFloat("_ExtFresPower", 1f);
				PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), PM.silver.UsePsiElements ? 0.75f : 0f, Time.deltaTime * 10f));
				PropBlock.SetFloat("_GlowInt", Mathf.Lerp(PropBlock.GetFloat("_GlowInt"), PM.silver.IsAwakened ? 1f : 0f, Time.deltaTime * 10f));
				PropBlock.SetColor("_OutlineColor", PsiGlows[0]);
				PropBlock.SetColor("_OutlinePulseColor", PsiGlows[1]);
				PropBlock.SetFloat("_OutlinePulseSpd", 1f);
				PropBlock.SetFloat("_OutlineInt", PM.silver.UsePsiElements ? 1f : ((!PM.silver.IsAwakened) ? 0f : 0.75f));
				PM.silver.Upgrades.Renderers[m].SetPropertyBlock(PropBlock);
			}
		}
		for (int n = 0; n < PsychicChargeFX.Length; n++)
		{
			ParticleSystem.EmissionModule emission4 = PsychicChargeFX[n].emission;
			emission4.enabled = PM.silver.ChargingPsychicKnife;
		}
		for (int num = 0; num < PsychicChargeFullFX.Length; num++)
		{
			ParticleSystem.EmissionModule emission5 = PsychicChargeFullFX[num].emission;
			emission5.enabled = PM.silver.FullyChargedPsychicKnife;
		}
		for (int num2 = 0; num2 < ESPTriggerFX.Length; num2++)
		{
			ParticleSystem.EmissionModule emission6 = ESPTriggerFX[num2].emission;
			emission6.enabled = !PM.silver.IsAwakened && PM.Base.HUD.ESPMaturityDisplay >= 30f && PM.silver.HasSigilOfAwakening && PM.Base.GetState() == "Ground" && PM.Base.GetState() != "ESPAwaken" && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f;
		}
		for (int num3 = 0; num3 < ESPFX.Length; num3++)
		{
			ParticleSystem.EmissionModule emission7 = ESPFX[num3].emission;
			emission7.enabled = PM.silver.IsAwakened;
		}
		FlashInt = Mathf.MoveTowards(FlashInt, 0f, Time.deltaTime * 2f);
		FlashMat.SetFloat("_Intensity", FlashInt);
	}

	public void PlayFlash(float Int = 0.75f)
	{
		FlashInt = Int;
	}

	public void PlaySmashFullChargeFX()
	{
		SmashFullChargeFX.Play();
	}

	public void CreatePsiTrailFX()
	{
		for (int i = 0; i < PsiPoints.Length; i++)
		{
			Object.Instantiate(PsiTrailFX, PsiPoints[i].position, Quaternion.identity).transform.SetParent(PsiPoints[i]);
		}
	}

	public void CreateUpheaveFX()
	{
		Object.Instantiate(UpheaveFX, base.transform.position + base.transform.forward * 0.25f - base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreatePsychoSmashFX()
	{
		Object.Instantiate(PsychoSmashFX, base.transform.position + base.transform.up * 0.4f, base.transform.rotation);
	}

	public void CreateGrabAllActivateFX()
	{
		Object.Instantiate(GrabAllActivateFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateGrabAllFX()
	{
		Object.Instantiate(GrabAllFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateTeleDashFX()
	{
		Object.Instantiate(TeleDashFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreatePsychoShockFX()
	{
		Object.Instantiate(PsychoShockFX, base.transform.position - base.transform.up * 0.1f, base.transform.rotation);
	}

	public void CreatePsiDeflectFX(Vector3 Target)
	{
		Object.Instantiate(PsiDeflectFX, Target, Quaternion.LookRotation(Target - base.transform.position));
	}

	public void CreateESPActivateFX()
	{
		Object.Instantiate(ESPActivateFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity);
	}

	public void PlayUpgradeGetFX()
	{
		UpgradeGetFX.Play();
	}
}
