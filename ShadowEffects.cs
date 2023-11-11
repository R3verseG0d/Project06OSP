using System.Collections;
using STHLua;
using UnityEngine;

public class ShadowEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] FootBones;

	public Transform[] HandBones;

	public Material AttackFlashMat;

	public Material BlastAuraMat;

	public Color TrailColor;

	public Color JetTrailColor;

	public TrailRenderer Trail;

	public TrailRenderer[] JetTrails;

	public TrailRenderer TornadoTrail;

	public Renderer SpinDashBallRenderer;

	public GameObject SpinDashBallFX;

	[Header("Shadow Particles")]
	public ParticleSystem[] JetFireFX;

	public ParticleSystem[] jumpDashParticles;

	public ParticleSystem[] SpearChargeFX;

	public ParticleSystem[] LanceChargeFX;

	public ParticleSystem[] FullPowerLanceFX;

	public ParticleSystem[] spinDashParticles;

	public ParticleSystem[] spindashShootParticles;

	public ParticleSystem[] LightDashParticles;

	public ParticleSystem[] ChaosBoostFX;

	public ParticleSystem[] BoostLvl2FX;

	public ParticleSystem[] BoostLvl3FX;

	public ParticleSystem BlastChargeFX;

	public ParticleSystem[] WristLimiterFX;

	public ParticleSystem[] FullPowerFX;

	public ParticleSystem FullPowerWaveFX;

	public ParticleSystem RestrainFX;

	public ParticleSystem[] LimiterOffFX;

	public ParticleSystem[] LimiterOnFX;

	public ParticleSystem[] ShardTriggerFX;

	public Color[] BoostGlows;

	public Color[] FullPowerGlows;

	[Header("Instantiation")]
	public GameObject TrailFX;

	public GameObject ShoeJetCombustFX;

	public GameObject[] TornadoFX;

	public GameObject ChaosAttackFX;

	public GameObject ChaosAttackLastFX;

	public GameObject ActivateBoostFX;

	public GameObject SnapDashFX;

	public GameObject ChaosSnapFX;

	public GameObject UninhibitFX;

	[Header("Audio")]
	public AudioSource BlastChargeAudio;

	private bool Startparticles;

	private bool SpearStartFX;

	private bool LanceStartFX;

	private float TrailAlpha;

	private float JetTrailAlpha;

	private float FlashInt;

	private float AuraInt;

	private float SpindashBlink;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public override void Start()
	{
		base.Start();
		AttackFlashMat.SetFloat("_Intensity", 0f);
		BlastAuraMat.SetFloat("_Intensity", 0f);
	}

	public override void Update()
	{
		base.Update();
		bool conditions = (PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1) || PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "Homing" || (PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 1) || (PM.Base.GetState() == "ChaosBoost" && PM.shadow.BoostState == 0) || (PM.Base.GetState() == "DashPanel" && DashPadRoll);
		UpdateJumpBallFX(conditions);
		if (Singleton<Settings>.Instance.settings.SpinEffect != 0)
		{
			if (SpinDashBallFX.activeSelf)
			{
				SpinDashBallFX.SetActive(value: false);
			}
			if (PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 0)
			{
				SpindashBlink += Time.deltaTime * 19f;
				if (SpindashBlink >= 1f)
				{
					SpindashBlink = 0f;
				}
				SpinDashBallRenderer.enabled = SpindashBlink <= 0.5f;
			}
			else if (SpinDashBallRenderer.enabled)
			{
				SpinDashBallRenderer.enabled = false;
			}
		}
		for (int i = 0; i < JetFireFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = JetFireFX[i].emission;
			emission.enabled = ((PM.Base.GetState() == "Ground" || PM.Base.GetState() == "DashPanel" || PM.Base.GetState() == "Path") && !PM.Base.WalkSwitch && PM.Base.CurSpeed > 0f) || PM.Base.GetState() == "Brake" || PM.Base.GetState() == "ChaosAttack" || PM.Base.GetState() == "ChaosSpear" || PM.Base.GetState() == "ChaosBlast" || (PM.Base.GetState() == "Result" && Time.time - PM.Base.ResultTime > 0.45f && Time.time - PM.Base.ResultTime < 0.9f);
		}
		for (int j = 0; j < jumpDashParticles.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = jumpDashParticles[j].emission;
			emission2.enabled = PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || (!PM.shadow.UseChaosSnap && PM.Base.GetState() == "Homing");
		}
		Gradient gradient = new Gradient();
		TrailAlpha = ((PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || (!PM.shadow.UseChaosSnap && PM.Base.GetState() == "Homing") || (PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 1) || (PM.Base.GetState() == "DashPanel" && DashPadRoll)) ? 1f : Mathf.Lerp(TrailAlpha, 0f, Time.deltaTime * 10f));
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(TrailColor, 0f),
			new GradientColorKey(TrailColor, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(TrailAlpha, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Trail.colorGradient = gradient;
		Gradient gradient2 = new Gradient();
		JetTrailAlpha = Mathf.Lerp(JetTrailAlpha, (PM.Base.IsGrounded() && PM.Base.GetState() != "SpinDash" && PM.Base.GetState() != "Grinding" && PM.Base.CurSpeed > 34f) ? 1f : 0f, Time.deltaTime * 10f);
		gradient2.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(JetTrailColor, 0f),
			new GradientColorKey(JetTrailColor, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(JetTrailAlpha, 0f),
			new GradientAlphaKey(0f, 0.5f),
			new GradientAlphaKey(0f, 1f)
		});
		for (int k = 0; k < JetTrails.Length; k++)
		{
			JetTrails[k].colorGradient = gradient2;
		}
		for (int l = 0; l < SpearChargeFX.Length; l++)
		{
			ParticleSystem.EmissionModule emission3 = SpearChargeFX[l].emission;
			emission3.enabled = PM.Base.GetState() == "ChaosSpear" && PM.shadow.ChaosBoostLevel < 2 && PM.shadow.SpearState == 0;
			if (PM.Base.GetState() == "ChaosSpear" && PM.shadow.ChaosBoostLevel < 2 && PM.shadow.SpearState == 0)
			{
				if (!SpearStartFX)
				{
					SpearStartFX = true;
					SpearChargeFX[l].Play();
				}
			}
			else
			{
				SpearChargeFX[l].Stop();
				SpearStartFX = false;
			}
		}
		for (int m = 0; m < LanceChargeFX.Length; m++)
		{
			ParticleSystem.EmissionModule emission4 = LanceChargeFX[m].emission;
			emission4.enabled = PM.Base.GetState() == "ChaosSpear" && PM.shadow.IsChaosBoost && PM.shadow.ChaosBoostLevel > 1 && PM.shadow.SpearState == 0 && !PM.shadow.IsFullPower;
			if (PM.Base.GetState() == "ChaosSpear" && PM.shadow.IsChaosBoost && PM.shadow.ChaosBoostLevel > 1 && PM.shadow.SpearState == 0 && !PM.shadow.IsFullPower)
			{
				if (!LanceStartFX)
				{
					LanceStartFX = true;
					LanceChargeFX[m].Play();
				}
			}
			else
			{
				LanceChargeFX[m].Stop();
				LanceStartFX = false;
			}
		}
		for (int n = 0; n < FullPowerLanceFX.Length; n++)
		{
			ParticleSystem.EmissionModule emission5 = FullPowerLanceFX[n].emission;
			emission5.enabled = PM.Base.GetState() == "ChaosSpear" && PM.shadow.SpearState == 0 && PM.shadow.IsFullPower;
		}
		FlashInt = Mathf.MoveTowards(FlashInt, 0f, Time.deltaTime * 7.5f);
		AttackFlashMat.SetFloat("_Intensity", FlashInt);
		AuraInt = Mathf.MoveTowards(AuraInt, (PM.Base.GetState() == "ChaosBlast") ? 0.125f : 0f, Time.deltaTime * ((PM.Base.GetState() == "ChaosBlast") ? 8f : 2f));
		BlastAuraMat.SetFloat("_Intensity", AuraInt);
		for (int num = 0; num < spinDashParticles.Length; num++)
		{
			ParticleSystem.EmissionModule emission6 = spinDashParticles[num].emission;
			emission6.enabled = PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 0;
		}
		for (int num2 = 0; num2 < spindashShootParticles.Length; num2++)
		{
			ParticleSystem.EmissionModule emission7 = spindashShootParticles[num2].emission;
			emission7.enabled = (PM.Base.GetState() == "SpinDash" && PM.shadow.SpinDashState == 1) || (PM.Base.GetState() == "DashPanel" && DashPadRoll);
		}
		for (int num3 = 0; num3 < LightDashParticles.Length; num3++)
		{
			ParticleSystem.EmissionModule emission8 = LightDashParticles[num3].emission;
			emission8.enabled = PM.Base.GetState() == "LightDash";
		}
		for (int num4 = 0; num4 < ChaosBoostFX.Length; num4++)
		{
			ParticleSystem.EmissionModule emission9 = ChaosBoostFX[num4].emission;
			emission9.enabled = !PM.shadow.IsFullPower && PM.shadow.IsChaosBoost && PM.Base.GetState() != "Homing";
		}
		for (int num5 = 0; num5 < BoostLvl2FX.Length; num5++)
		{
			ParticleSystem.EmissionModule emission10 = BoostLvl2FX[num5].emission;
			emission10.enabled = !PM.shadow.IsFullPower && PM.shadow.IsChaosBoost && PM.shadow.ChaosBoostLevel > 1 && PM.Base.GetState() != "Homing";
		}
		for (int num6 = 0; num6 < BoostLvl3FX.Length; num6++)
		{
			ParticleSystem.EmissionModule emission11 = BoostLvl3FX[num6].emission;
			emission11.enabled = !PM.shadow.IsFullPower && PM.shadow.IsChaosBoost && PM.shadow.ChaosBoostLevel > 2 && PM.Base.GetState() != "Homing";
		}
		for (int num7 = 0; num7 < FullPowerFX.Length; num7++)
		{
			ParticleSystem.EmissionModule emission12 = FullPowerFX[num7].emission;
			emission12.enabled = PM.shadow.IsFullPower && PM.Base.GetState() != "Homing";
		}
		for (int num8 = 0; num8 < PM.shadow.PlayerRenderers.Length; num8++)
		{
			PM.shadow.PlayerRenderers[num8].GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_ExtFresColor", (!PM.shadow.IsFullPower) ? BoostGlows[0] : FullPowerGlows[0]);
			PropBlock.SetColor("_ExtGlowColor", (!PM.shadow.IsFullPower) ? BoostGlows[1] : FullPowerGlows[1]);
			PropBlock.SetFloat("_ExtPulseSpd", 0.5f);
			PropBlock.SetFloat("_ExtFresPower", 1f);
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), (PM.shadow.IsChaosBoost || PM.shadow.IsFullPower) ? 1f : 0f, Time.deltaTime * 10f));
			PropBlock.SetColor("_OutlineColor", (!PM.shadow.IsFullPower) ? BoostGlows[0] : FullPowerGlows[0]);
			PropBlock.SetColor("_OutlinePulseColor", (!PM.shadow.IsFullPower) ? BoostGlows[1] : FullPowerGlows[1]);
			PropBlock.SetFloat("_OutlinePulseSpd", 0.5f);
			PropBlock.SetFloat("_OutlineInt", (PM.shadow.IsChaosBoost || PM.shadow.IsFullPower) ? 1f : 0f);
			PM.shadow.PlayerRenderers[num8].SetPropertyBlock(PropBlock);
		}
		for (int num9 = 0; num9 < ShardTriggerFX.Length; num9++)
		{
			ParticleSystem.EmissionModule emission13 = ShardTriggerFX[num9].emission;
			emission13.enabled = !PM.shadow.IsFullPower && PM.shadow.IsChaosBoost && PM.shadow.ChaosBoostLevel == Shadow_Lua.c_level_max && PM.Base.HUD.ChaosMaturityDisplay >= 100f && PM.shadow.HasLightMemoryShard && (PM.Base.GetState() == "Ground" || PM.Base.GetState() == "Air" || PM.Base.GetState() == "SlowFall" || PM.Base.GetState() == "Jump" || PM.Base.GetState() == "AfterHoming" || PM.Base.GetState() == "Tornado" || PM.Base.GetState() == "SpinDash" || (PM.Base.GetState() == "Spring" && !PM.Base.LockControls) || (PM.Base.GetState() == "WideSpring" && !PM.Base.LockControls) || (PM.Base.GetState() == "JumpPanel" && !PM.Base.LockControls) || (PM.Base.GetState() == "RainbowRing" && !PM.Base.LockControls) || (PM.Base.GetState() == "Pole" && !PM.Base.LockControls) || (PM.Base.GetState() == "Rope" && !PM.Base.LockControls)) && PM.Base.GetState() != "Uninhibit" && Singleton<RInput>.Instance.P.GetAxis("D-Pad Y") > 0f;
		}
		ParticleSystem.EmissionModule emission14 = FullPowerWaveFX.emission;
		if (PM.shadow.IsFullPower && Physics.Raycast(base.transform.position + base.transform.up * 0.25f, -Vector3.up, out var hitInfo, 3f, PM.Base.Collision_Mask))
		{
			FullPowerWaveFX.transform.position = hitInfo.point;
			FullPowerWaveFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
			emission14.enabled = true;
		}
		else
		{
			emission14.enabled = false;
		}
	}

	public void PlayFlash(float Int = 0.75f)
	{
		FlashInt = Int;
	}

	public void ManageTornadoTrail(bool Enable)
	{
		TornadoTrail.emitting = Enable;
	}

	public void CreateShoeJetCombustFX()
	{
		for (int i = 0; i < FootBones.Length; i++)
		{
			Object.Instantiate(ShoeJetCombustFX, FootBones[i].position, FootBones[i].rotation).transform.SetParent(FootBones[i]);
		}
	}

	public void CreateTornadoFX(int Index)
	{
		Object.Instantiate(TornadoFX[Index], base.transform.position - base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreateChaosAttackFX(int Index)
	{
		Object.Instantiate((Index != 5) ? ChaosAttackFX : ChaosAttackLastFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
		if (Index == 1)
		{
			for (int i = 0; i < HandBones.Length; i++)
			{
				Object.Instantiate(TrailFX, HandBones[i].position, HandBones[i].rotation).transform.SetParent(HandBones[i]);
			}
		}
		PlayFlash(0.25f);
		if (Index == 2)
		{
			Object.Instantiate(TrailFX, FootBones[1].position, FootBones[1].rotation).transform.SetParent(FootBones[1]);
		}
		if (Index == 3)
		{
			Object.Instantiate(TrailFX, FootBones[0].position, FootBones[0].rotation).transform.SetParent(FootBones[0]);
		}
		if (Index == 4)
		{
			Object.Instantiate(TrailFX, HandBones[1].position, HandBones[1].rotation).transform.SetParent(HandBones[1]);
		}
	}

	public void CreateActivateBoostFX()
	{
		Object.Instantiate(ActivateBoostFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateSnapDashFX()
	{
		Object.Instantiate(SnapDashFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateChaosSnapFX()
	{
		Object.Instantiate(ChaosSnapFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateBlastChargeFX()
	{
		BlastChargeFX.Play();
		BlastChargeAudio.Play();
	}

	public void CreateUninhibitFX()
	{
		Object.Instantiate(UninhibitFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void PlayLimiterFX(bool Result)
	{
		WristLimiterFX[0].Play();
		WristLimiterFX[1].Play();
		if (Result)
		{
			LimiterOffFX[0].Play();
			LimiterOffFX[1].Play();
		}
		else
		{
			StartCoroutine(PlayLimitersOff());
		}
	}

	public void PlayRestrainFX(float FlashSet)
	{
		RestrainFX.Play();
		PlayFlash(FlashSet);
	}

	private IEnumerator PlayLimitersOff()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		LimiterOnFX[0].transform.SetParent(null);
		LimiterOnFX[1].transform.SetParent(null);
		LimiterOnFX[0].Play();
		LimiterOnFX[1].Play();
		while (Timer <= 2f)
		{
			Timer = Time.time - StartTime;
			yield return new WaitForFixedUpdate();
		}
		LimiterOnFX[0].transform.SetParent(PM.shadow.LimiterAnimators[0].transform);
		LimiterOnFX[1].transform.SetParent(PM.shadow.LimiterAnimators[0].transform);
		LimiterOnFX[0].transform.localPosition = Vector3.zero;
		LimiterOnFX[1].transform.localEulerAngles = Vector3.zero;
	}
}
