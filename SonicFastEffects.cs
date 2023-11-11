using UnityEngine;

public class SonicFastEffects : EffectsBase
{
	[Header("Framework")]
	public Renderer[] SuperRenderers;

	public Material[] NormalGemShoeMats;

	public Material[] GemShoeMats;

	public Gradient SuperGradient;

	public Gradient SuperSkinGradient;

	public Material SuperSkin;

	public Light SuperLight;

	public Light LightSpeedLight;

	public Transform SuperFXPivot;

	public Transform SuperFXLandPivot;

	public Color TrailColor;

	public Color SuperTrailColor;

	public TrailRenderer Trail;

	public Renderer SuperJumpballRenderer;

	[Header("Fast Particles")]
	public ParticleSystem[] FastDirtFX;

	public ParticleSystem[] FastSandFX;

	public ParticleSystem[] FastWaterFX;

	[Header("Sonic Particles")]
	public ParticleSystem SpeedBarrierFX;

	public ParticleSystem[] SlideFX;

	public ParticleSystem[] BoundAttackFX;

	public ParticleSystem[] LightDashFX;

	public ParticleSystem[] SuperFX;

	public ParticleSystem[] WaterRunFX;

	public ParticleSystem GemShoesFX;

	[Header("Super Particles")]
	public ParticleSystem[] LightSpeedFX;

	public ParticleSystem[] SuperBoostFX;

	public ParticleSystem[] SuperJumpFX;

	public ParticleSystem[] SuperSlideFX;

	public ParticleSystem[] SuperBoundAttackFX;

	[Header("Instantiation")]
	public GameObject BoundAttackPrefab;

	public GameObject DetransformFX;

	[Header("Super Instantiation")]
	public GameObject SuperBoundAttackPrefab;

	private bool SuperJumpStartFX;

	private float TrailAlpha;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public override void Start()
	{
		base.Start();
		for (int i = 0; i < NormalGemShoeMats.Length; i++)
		{
			NormalGemShoeMats[i].SetFloat("_GlowInt", 0f);
		}
	}

	public override void Update()
	{
		base.Update();
		bool conditions = (PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1) || (PM.Base.GetState() == "BoundAttack" && (PM.sonic_fast.BoundState == 0 || (PM.sonic_fast.BoundState == 1 && PM.sonic_fast.AirMotionVelocity.y > -0.25f)));
		if (PM.sonic_fast.IsSuper)
		{
			UpdateJumpBallFX(conditions, SuperJumpballRenderer);
		}
		else
		{
			UpdateJumpBallFX(conditions);
		}
		for (int i = 0; i < FastDirtFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FastDirtFX[i].emission;
			emission.enabled = PM.Base.PlayerManager.PlayerEvents.GroundTag == "Dirt" && PM.Base.IsGrounded() && PM.Base.CurSpeed > PM.Base.WalkSpeed / 2f && !PM.Base.IsDead;
		}
		for (int j = 0; j < FastSandFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = FastSandFX[j].emission;
			emission2.enabled = PM.Base.PlayerManager.PlayerEvents.GroundTag == "Sand" && PM.Base.IsGrounded() && PM.Base.CurSpeed > PM.Base.WalkSpeed / 2f && !PM.Base.IsDead;
		}
		for (int k = 0; k < FastWaterFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission3 = FastWaterFX[k].emission;
			emission3.enabled = (PM.Base.PlayerManager.PlayerEvents.GroundTag == "Water" || PM.Base.PlayerManager.PlayerEvents.GetTag() == "Water") && PM.Base.IsGrounded() && PM.Base.CurSpeed > PM.Base.WalkSpeed / 2f && !PM.Base.IsDead;
		}
		UseJumpFX = !PM.sonic_fast.IsSuper;
		for (int l = 0; l < SuperJumpFX.Length; l++)
		{
			ParticleSystem.EmissionModule emission4 = SuperJumpFX[l].emission;
			emission4.enabled = PM.sonic_fast.IsSuper && PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1;
			if (PM.sonic_fast.IsSuper && PM.Base.GetState() == "Jump")
			{
				if (PM.Base.JumpAnimation == 1)
				{
					if (!SuperJumpStartFX)
					{
						SuperJumpStartFX = true;
						SuperJumpFX[l].Play();
					}
				}
				else if (PM.Base.JumpAnimation == 2)
				{
					SuperJumpStartFX = false;
					SuperJumpFX[l].Stop();
				}
			}
			else
			{
				SuperJumpStartFX = false;
				SuperJumpFX[l].Stop();
			}
		}
		Gradient gradient = new Gradient();
		TrailAlpha = ((PM.Base.GetState() == "BoundAttack" && PM.sonic_fast.BoundState == 0) ? 1f : Mathf.Lerp(TrailAlpha, 0f, Time.deltaTime * 10f));
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey((!PM.sonic_fast.IsSuper) ? TrailColor : SuperTrailColor, 0f),
			new GradientColorKey((!PM.sonic_fast.IsSuper) ? TrailColor : SuperTrailColor, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(TrailAlpha, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Trail.colorGradient = gradient;
		for (int m = 0; m < SlideFX.Length; m++)
		{
			ParticleSystem.EmissionModule emission5 = SlideFX[m].emission;
			emission5.enabled = !PM.sonic_fast.IsSuper && PM.Base.GetState() == "Slide";
		}
		for (int n = 0; n < SuperSlideFX.Length; n++)
		{
			ParticleSystem.EmissionModule emission6 = SuperSlideFX[n].emission;
			emission6.enabled = PM.sonic_fast.IsSuper && PM.Base.GetState() == "Slide";
		}
		for (int num = 0; num < LightDashFX.Length; num++)
		{
			ParticleSystem.EmissionModule emission7 = LightDashFX[num].emission;
			emission7.enabled = PM.Base.GetState() == "LightDash";
		}
		for (int num2 = 0; num2 < BoundAttackFX.Length; num2++)
		{
			ParticleSystem.EmissionModule emission8 = BoundAttackFX[num2].emission;
			emission8.enabled = !PM.sonic_fast.IsSuper && PM.Base.GetState() == "BoundAttack" && (PM.sonic_fast.BoundState == 0 || (PM.sonic_fast.BoundState == 1 && PM.sonic_fast.AirMotionVelocity.y > -0.25f));
		}
		for (int num3 = 0; num3 < SuperBoundAttackFX.Length; num3++)
		{
			ParticleSystem.EmissionModule emission9 = SuperBoundAttackFX[num3].emission;
			emission9.enabled = PM.sonic_fast.IsSuper && PM.Base.GetState() == "BoundAttack" && (PM.sonic_fast.BoundState == 0 || (PM.sonic_fast.BoundState == 1 && PM.sonic_fast.AirMotionVelocity.y > -0.25f));
		}
		ParticleSystem.EmissionModule emission10 = SpeedBarrierFX.emission;
		emission10.enabled = PM.sonic_fast.UseSpeedBarrier;
		for (int num4 = 0; num4 < WaterRunFX.Length; num4++)
		{
			ParticleSystem.EmissionModule emission11 = WaterRunFX[num4].emission;
			emission11.enabled = (PM.Base.PlayerManager.PlayerEvents.GroundTag == "Water" || PM.Base.PlayerManager.PlayerEvents.GroundTag == "ShoreWater" || PM.Base.PlayerManager.PlayerEvents.GetTag() == "Water" || PM.Base.PlayerManager.PlayerEvents.GetTag() == "ShoreWater") && PM.Base.IsGrounded() && ((PM.Base.GetState() != "Slide" && PM.Base.GetState() != "Land" && PM.Base.CurSpeed > 0f) || (PM.Base.GetState() == "Slide" && PM.Base.CurSpeed > 17.5f));
		}
		for (int num5 = 0; num5 < SuperRenderers.Length; num5++)
		{
			SuperRenderers[num5].GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_OutlineColor", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", PM.sonic_fast.IsSuper ? 1f : 0f);
			SuperRenderers[num5].SetPropertyBlock(PropBlock);
		}
		if (PM.sonic_fast.Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int num6 = 0; num6 < PM.sonic_fast.Upgrades.Renderers.Count; num6++)
			{
				PM.sonic_fast.Upgrades.Renderers[num6].GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_OutlineColor", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
				PropBlock.SetFloat("_OutlinePulseSpd", 0f);
				PropBlock.SetFloat("_OutlineInt", PM.sonic_fast.IsSuper ? 1f : 0f);
				PM.sonic_fast.Upgrades.Renderers[num6].SetPropertyBlock(PropBlock);
			}
		}
		if (PM.sonic_fast.IsSuper)
		{
			for (int num7 = 0; num7 < GemShoeMats.Length; num7++)
			{
				GemShoeMats[num7].SetColor("_Color", Color.Lerp(GemShoeMats[num7].GetColor("_Color"), SuperGradient.Evaluate(Mathf.Repeat(Time.time, 2f) / 2f), Time.deltaTime * 10f));
			}
			SuperSkin.SetColor("_Color", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
		}
		if (Singleton<Settings>.Instance.settings.GemShoesType == 1)
		{
			for (int num8 = 0; num8 < NormalGemShoeMats.Length; num8++)
			{
				if (PM.sonic_fast.IsSuper)
				{
					NormalGemShoeMats[num8].SetColor("_GlowColor", Color.Lerp(NormalGemShoeMats[num8].GetColor("_GlowColor"), SuperGradient.Evaluate(Mathf.Repeat(Time.time, 2f) / 2f), Time.deltaTime * 10f));
				}
				NormalGemShoeMats[num8].SetFloat("_GlowInt", Mathf.Lerp(NormalGemShoeMats[num8].GetFloat("_GlowInt"), PM.sonic_fast.IsSuper ? 4f : 0f, Time.deltaTime * 10f));
			}
		}
		SuperLight.intensity = ((PM.sonic_fast.IsSuper && PM.Base.CurSpeed < 74f) ? Mathf.Lerp(0.75f, 1.25f, Mathf.Abs(Mathf.Cos(Time.time * 10f))) : Mathf.Lerp(SuperLight.intensity, 0f, Time.deltaTime * 5f));
		LightSpeedLight.intensity = ((PM.sonic_fast.IsSuper && PM.Base.CurSpeed >= 74f) ? Mathf.Lerp(1f, (!PM.sonic_fast.UseSpeedBarrier) ? 1.5f : 2f, Mathf.Abs(Mathf.Cos(Time.time * 10f))) : Mathf.Lerp(LightSpeedLight.intensity, 0f, Time.deltaTime * 5f));
		if (PM.Base.IsGrounded() || PM.Base.GetState() == "Land" || PM.Base.GetState() == "LightDash" || PM.Base.GetState() == "Path" || PM.Base.GetState() == "ChainJump")
		{
			if (PM.Base.GetState() == "LightDash" || PM.Base.GetState() == "Path")
			{
				SuperFXPivot.rotation = PM.Base.Mesh.rotation;
			}
			else if (PM.Base.GetState() == "Land" || PM.Base.GetState() == "ChainJump")
			{
				SuperFXPivot.rotation = PM.Base.Mesh.rotation * Quaternion.Euler(90f, 0f, 0f);
			}
			else
			{
				SuperFXPivot.localEulerAngles = new Vector3(Mathf.Lerp(90f, 0f, PM.Base.CurSpeed / 20f), 0f, 0f);
			}
		}
		else
		{
			SuperFXPivot.rotation = Quaternion.Lerp(SuperFXPivot.rotation, Quaternion.LookRotation(PM.RBody.velocity.normalized), Time.deltaTime * 8f);
		}
		if (PM.Base.GetState() == "Result")
		{
			SuperFXPivot.localPosition = new Vector3(HipBone.localPosition.x, SuperFXPivot.localPosition.y, HipBone.localPosition.z);
		}
		else if (PM.Base.GetState() == "Land" && PM.Base.StageManager._Stage == StageManager.Stage.kdv && PM.Base.StageManager.StageSection == StageManager.Section.C)
		{
			SuperFXPivot.localPosition = new Vector3(SuperFXLandPivot.localPosition.x, SuperFXPivot.localPosition.y, SuperFXLandPivot.localPosition.z);
			if ((bool)PM.Base.ShieldObject)
			{
				PM.Base.ShieldObject.transform.localPosition = new Vector3(SuperFXLandPivot.localPosition.x, PM.Base.ShieldObject.transform.localPosition.y, SuperFXLandPivot.localPosition.z);
			}
		}
		else
		{
			if (SuperFXPivot.localPosition != Vector3.zero)
			{
				SuperFXPivot.localPosition = Vector3.zero;
			}
			if (PM.Base.GetState() != "Result" && (bool)PM.Base.ShieldObject && PM.Base.ShieldObject.transform.localPosition != new Vector3(0f, PM.Base.ShieldObject.transform.localPosition.y, 0f))
			{
				PM.Base.ShieldObject.transform.localPosition = new Vector3(0f, PM.Base.ShieldObject.transform.localPosition.y, 0f);
			}
		}
		for (int num9 = 0; num9 < SuperFX.Length; num9++)
		{
			ParticleSystem.EmissionModule emission12 = SuperFX[num9].emission;
			emission12.enabled = PM.sonic_fast.IsSuper && PM.Base.CurSpeed < 74f;
		}
		for (int num10 = 0; num10 < LightSpeedFX.Length; num10++)
		{
			ParticleSystem.EmissionModule emission13 = LightSpeedFX[num10].emission;
			emission13.enabled = PM.sonic_fast.IsSuper && PM.Base.CurSpeed >= 74f && !PM.sonic_fast.UseSpeedBarrier;
		}
		for (int num11 = 0; num11 < SuperBoostFX.Length; num11++)
		{
			ParticleSystem.EmissionModule emission14 = SuperBoostFX[num11].emission;
			emission14.enabled = PM.sonic_fast.IsSuper && PM.sonic_fast.UseSpeedBarrier;
		}
	}

	public void CreateDetransformFX()
	{
		Object.Instantiate(DetransformFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity).transform.SetParent(base.transform);
	}

	public void CreateBoundAttackFX()
	{
		Object.Instantiate((!PM.sonic_fast.IsSuper) ? BoundAttackPrefab : SuperBoundAttackPrefab, PM.Base.RaycastHit.point, base.transform.rotation);
	}
}
