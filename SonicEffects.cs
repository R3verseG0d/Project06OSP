using UnityEngine;

public class SonicEffects : EffectsBase
{
	[Header("Framework")]
	public Renderer[] NormalRenderers;

	public Renderer[] SuperRenderers;

	public Transform rightFoot;

	public Material[] NormalGemShoeMats;

	public Material[] GemShoeMats;

	public Color[] GemShoeColors;

	public Gradient SuperGradient;

	public Gradient SuperSkinGradient;

	public Material SuperSkin;

	public Light SuperLight;

	public Light LightSpeedLight;

	public Transform SuperFXPivot;

	public Color MachSpeedGlow;

	public Color SlowdownGlow;

	public Color TornadoGlow;

	public Color PurpleGlow;

	public Color SuperGlow;

	public Color TrailColor;

	public Color SuperTrailColor;

	public TrailRenderer Trail;

	public Renderer SpinDashBallRenderer;

	public GameObject SpinDashBallFX;

	public Renderer SuperJumpballRenderer;

	public Renderer SuperSpinDashBallRenderer;

	public GameObject SuperSpinDashBallFX;

	[Header("Sonic Particles")]
	public ParticleSystem[] jumpDashParticles;

	public ParticleSystem[] homingSmashParticles;

	public ParticleSystem[] homingSmashStartParticles;

	public ParticleSystem[] kickAttackParticles;

	public ParticleSystem[] SlideParticles;

	public ParticleSystem[] spinDashParticles;

	public ParticleSystem[] spindashShootParticles;

	public ParticleSystem[] boundAttackParticles;

	public ParticleSystem[] LightDashParticles;

	public ParticleSystem[] RedGemParticles;

	public ParticleSystem[] GreenGemParticles;

	public ParticleSystem[] PurpleGemParticles;

	public ParticleSystem[] SkyGemParticles;

	public ParticleSystem[] GunDriveParticles;

	public ParticleSystem[] RaibowGemParticles;

	public ParticleSystem[] waterRun;

	public ParticleSystem NormalShoesFX;

	public ParticleSystem GemShoesFX;

	[Header("Super Particles")]
	public ParticleSystem[] LightSpeedFX;

	public ParticleSystem[] SuperJumpFX;

	public ParticleSystem[] SuperJumpDashFX;

	public ParticleSystem[] SuperKickAttackFX;

	public ParticleSystem[] SuperSlideFX;

	public ParticleSystem[] SuperSpinDashFX;

	public ParticleSystem[] SuperSpinDashShootFX;

	public ParticleSystem[] SuperBoundAttackFX;

	[Header("Instantiation")]
	public GameObject kickAttackGlowPrefab;

	public GameObject boundAttackPrefab;

	public GameObject[] TornadoFX;

	public GameObject[] SlowdownFX;

	public GameObject[] MachSpeedFX;

	public GameObject SkyGemFX;

	public GameObject GunDriveMoveFX;

	public GameObject ScaleFX;

	public GameObject HomingSmashFX;

	public GameObject TransformFX;

	public GameObject DetransformFX;

	public GameObject LightAttackFX;

	[Header("Super Instantiation")]
	public GameObject SuperKickAttackGlowPrefab;

	public GameObject SuperBoundAttackPrefab;

	private bool Startparticles;

	private bool SuperJumpStartFX;

	private bool PlayShoeSwitchFX;

	private float TrailAlpha;

	private float SpindashBlink;

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
		bool conditions = (PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1) || PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "Homing" || (PM.Base.GetState() == "BoundAttack" && (PM.sonic.BoundState == 0 || (PM.sonic.BoundState == 1 && PM.sonic.AirMotionVelocity.y > -0.25f))) || (PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 1) || PM.Base.GetState() == "HomingSmash" || (PM.Base.GetState() == "DashPanel" && DashPadRoll);
		if (PM.sonic.IsSuper)
		{
			UpdateJumpBallFX(conditions, SuperJumpballRenderer);
		}
		else
		{
			UpdateJumpBallFX(conditions);
		}
		if (Singleton<Settings>.Instance.settings.SpinEffect != 0)
		{
			if (SpinDashBallFX.activeSelf)
			{
				SpinDashBallFX.SetActive(value: false);
			}
			if (SuperSpinDashBallFX.activeSelf)
			{
				SuperSpinDashBallFX.SetActive(value: false);
			}
			if (PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 0)
			{
				SpindashBlink += Time.deltaTime * 19f;
				if (SpindashBlink >= 1f)
				{
					SpindashBlink = 0f;
				}
				SpinDashBallRenderer.enabled = !PM.sonic.IsSuper && SpindashBlink <= 0.5f;
				SuperSpinDashBallRenderer.enabled = PM.sonic.IsSuper && SpindashBlink <= 0.5f;
			}
			else
			{
				if (SpinDashBallRenderer.enabled)
				{
					SpinDashBallRenderer.enabled = false;
				}
				if (SuperSpinDashBallRenderer.enabled)
				{
					SuperSpinDashBallRenderer.enabled = false;
				}
			}
		}
		MaterialPropertyBlock propBlock2;
		float value;
		for (int i = 0; i < NormalRenderers.Length; propBlock2.SetFloat("_OutlineInt", value), NormalRenderers[i].SetPropertyBlock(PropBlock), i++)
		{
			NormalRenderers[i].GetPropertyBlock(PropBlock);
			if (PM.sonic.ActiveGem == SonicNew.Gem.Blue)
			{
				PropBlock.SetColor("_ExtFresColor", MachSpeedGlow);
				PropBlock.SetColor("_ExtGlowColor", MachSpeedGlow);
				PropBlock.SetColor("_OutlineColor", MachSpeedGlow);
			}
			else if (PM.sonic.ActiveGem == SonicNew.Gem.Red)
			{
				PropBlock.SetColor("_ExtFresColor", SlowdownGlow);
				PropBlock.SetColor("_ExtGlowColor", SlowdownGlow);
				PropBlock.SetColor("_OutlineColor", SlowdownGlow);
			}
			else if (PM.sonic.ActiveGem == SonicNew.Gem.Green)
			{
				PropBlock.SetColor("_ExtFresColor", TornadoGlow);
				PropBlock.SetColor("_ExtGlowColor", TornadoGlow);
				PropBlock.SetColor("_OutlineColor", TornadoGlow);
			}
			else if (PM.sonic.ActiveGem == SonicNew.Gem.Purple)
			{
				PropBlock.SetColor("_ExtFresColor", PurpleGlow);
				PropBlock.SetColor("_ExtGlowColor", PurpleGlow);
				PropBlock.SetColor("_OutlineColor", PurpleGlow);
			}
			PropBlock.SetFloat("_ExtPulseSpd", 1f);
			PropBlock.SetFloat("_ExtFresPower", 1f);
			MaterialPropertyBlock propBlock = PropBlock;
			float @float = PropBlock.GetFloat("_ExtFresThre");
			if (!PM.sonic.UsingBlueGem && !PM.sonic.UsingRedGem)
			{
				bool num;
				if (!PM.sonic.GroundTornado)
				{
					num = PM.sonic.UsingGreenGem;
				}
				else
				{
					if (!PM.sonic.UsingGreenGem)
					{
						goto IL_04df;
					}
					num = PM.sonic.SpawnedAirTornado;
				}
				if (!num)
				{
					goto IL_04df;
				}
			}
			goto IL_04f1;
			IL_04df:
			if (PM.sonic.UsingPurpleGem)
			{
				goto IL_04f1;
			}
			goto IL_04f5;
			IL_04f1:
			if (i >= 3)
			{
				goto IL_04f5;
			}
			float b = 0.75f;
			goto IL_0501;
			IL_04f5:
			b = 0f;
			goto IL_0501;
			IL_0501:
			propBlock.SetFloat("_ExtFresThre", Mathf.Lerp(@float, b, Time.deltaTime * (PM.sonic.UsingBlueGem ? 20f : 10f)));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			propBlock2 = PropBlock;
			if (!PM.sonic.UsingBlueGem && !PM.sonic.UsingRedGem)
			{
				bool num2;
				if (!PM.sonic.GroundTornado)
				{
					num2 = PM.sonic.UsingGreenGem;
				}
				else
				{
					if (!PM.sonic.UsingGreenGem)
					{
						goto IL_05be;
					}
					num2 = PM.sonic.SpawnedAirTornado;
				}
				if (!num2)
				{
					goto IL_05be;
				}
			}
			goto IL_05d7;
			IL_05d7:
			value = 1f;
			continue;
			IL_05be:
			if (!PM.sonic.UsingPurpleGem)
			{
				value = 0f;
				continue;
			}
			goto IL_05d7;
		}
		for (int j = 0; j < SuperRenderers.Length; j++)
		{
			SuperRenderers[j].GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_OutlineColor", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", PM.sonic.IsSuper ? 1f : 0f);
			SuperRenderers[j].SetPropertyBlock(PropBlock);
		}
		if (PM.sonic.Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			float value2;
			MaterialPropertyBlock propBlock4;
			for (int k = 0; k < PM.sonic.Upgrades.Renderers.Count; propBlock4.SetFloat("_OutlineInt", value2), PM.sonic.Upgrades.Renderers[k].SetPropertyBlock(PropBlock), k++)
			{
				PM.sonic.Upgrades.Renderers[k].GetPropertyBlock(PropBlock);
				if (PM.sonic.ActiveGem == SonicNew.Gem.Blue)
				{
					PropBlock.SetColor("_ExtFresColor", MachSpeedGlow);
					PropBlock.SetColor("_ExtGlowColor", MachSpeedGlow);
					PropBlock.SetColor("_OutlineColor", MachSpeedGlow);
				}
				else if (PM.sonic.ActiveGem == SonicNew.Gem.Red)
				{
					PropBlock.SetColor("_ExtFresColor", SlowdownGlow);
					PropBlock.SetColor("_ExtGlowColor", SlowdownGlow);
					PropBlock.SetColor("_OutlineColor", SlowdownGlow);
				}
				else if (PM.sonic.ActiveGem == SonicNew.Gem.Green)
				{
					PropBlock.SetColor("_ExtFresColor", TornadoGlow);
					PropBlock.SetColor("_ExtGlowColor", TornadoGlow);
					PropBlock.SetColor("_OutlineColor", TornadoGlow);
				}
				else if (PM.sonic.ActiveGem == SonicNew.Gem.Purple)
				{
					PropBlock.SetColor("_ExtFresColor", PurpleGlow);
					PropBlock.SetColor("_ExtGlowColor", PurpleGlow);
					PropBlock.SetColor("_OutlineColor", PurpleGlow);
				}
				else if (PM.sonic.ActiveGem == SonicNew.Gem.Rainbow)
				{
					PropBlock.SetColor("_ExtFresColor", SuperGlow);
					PropBlock.SetColor("_ExtGlowColor", SuperGlow);
					PropBlock.SetColor("_OutlineColor", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
				}
				PropBlock.SetFloat("_ExtPulseSpd", 1f);
				PropBlock.SetFloat("_ExtFresPower", 1f);
				MaterialPropertyBlock propBlock3 = PropBlock;
				float float2 = PropBlock.GetFloat("_ExtFresThre");
				if (!PM.sonic.UsingBlueGem && !PM.sonic.UsingRedGem)
				{
					bool num3;
					if (!PM.sonic.GroundTornado)
					{
						num3 = PM.sonic.UsingGreenGem;
					}
					else
					{
						if (!PM.sonic.UsingGreenGem)
						{
							goto IL_09a0;
						}
						num3 = PM.sonic.SpawnedAirTornado;
					}
					if (!num3)
					{
						goto IL_09a0;
					}
				}
				goto IL_09cb;
				IL_09cb:
				float b2 = 0.75f;
				goto IL_09d0;
				IL_0abb:
				value2 = 1f;
				continue;
				IL_09d0:
				propBlock3.SetFloat("_ExtFresThre", Mathf.Lerp(float2, b2, Time.deltaTime * (PM.sonic.UsingBlueGem ? 20f : 10f)));
				PropBlock.SetFloat("_OutlinePulseSpd", 0f);
				propBlock4 = PropBlock;
				if (!PM.sonic.UsingBlueGem && !PM.sonic.UsingRedGem)
				{
					bool num4;
					if (!PM.sonic.GroundTornado)
					{
						num4 = PM.sonic.UsingGreenGem;
					}
					else
					{
						if (!PM.sonic.UsingGreenGem)
						{
							goto IL_0a90;
						}
						num4 = PM.sonic.SpawnedAirTornado;
					}
					if (!num4)
					{
						goto IL_0a90;
					}
				}
				goto IL_0abb;
				IL_09a0:
				if (PM.sonic.UsingPurpleGem || PM.sonic.IsSuper)
				{
					goto IL_09cb;
				}
				b2 = 0f;
				goto IL_09d0;
				IL_0a90:
				if (!PM.sonic.UsingPurpleGem && !PM.sonic.IsSuper)
				{
					value2 = 0f;
					continue;
				}
				goto IL_0abb;
			}
		}
		if (Singleton<Settings>.Instance.settings.GemShoesType == 0)
		{
			for (int l = 0; l < GemShoeMats.Length; l++)
			{
				GemShoeMats[l].SetColor("_Color", Color.Lerp(GemShoeMats[l].GetColor("_Color"), (PM.sonic.GemSelector != 8) ? GemShoeColors[PM.sonic.GemSelector] : SuperGradient.Evaluate(Mathf.Repeat(Time.time, 2f) / 2f), Time.deltaTime * 10f));
			}
			if (PM.sonic.GemSelector > 0)
			{
				if (!PlayShoeSwitchFX)
				{
					PlayShoeSwitchFX = true;
					GemShoesFX.Play();
				}
			}
			else if (PlayShoeSwitchFX)
			{
				PlayShoeSwitchFX = false;
				NormalShoesFX.Play();
			}
		}
		else
		{
			for (int m = 0; m < NormalGemShoeMats.Length; m++)
			{
				NormalGemShoeMats[m].SetColor("_GlowColor", Color.Lerp(NormalGemShoeMats[m].GetColor("_GlowColor"), (PM.sonic.GemSelector != 8) ? GemShoeColors[PM.sonic.GemSelector] : SuperGradient.Evaluate(Mathf.Repeat(Time.time, 2f) / 2f), Time.deltaTime * 10f));
				NormalGemShoeMats[m].SetFloat("_GlowInt", Mathf.Lerp(NormalGemShoeMats[m].GetFloat("_GlowInt"), (PM.sonic.GemSelector != 0) ? 4f : 0f, Time.deltaTime * 10f));
			}
		}
		UseJumpFX = !PM.sonic.IsSuper;
		for (int n = 0; n < SuperJumpFX.Length; n++)
		{
			ParticleSystem.EmissionModule emission = SuperJumpFX[n].emission;
			emission.enabled = PM.sonic.IsSuper && PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1;
			if (PM.sonic.IsSuper && PM.Base.GetState() == "Jump")
			{
				if (PM.Base.JumpAnimation == 1)
				{
					if (!SuperJumpStartFX)
					{
						SuperJumpStartFX = true;
						SuperJumpFX[n].Play();
					}
				}
				else if (PM.Base.JumpAnimation == 2)
				{
					SuperJumpStartFX = false;
					SuperJumpFX[n].Stop();
				}
			}
			else
			{
				SuperJumpStartFX = false;
				SuperJumpFX[n].Stop();
			}
		}
		for (int num5 = 0; num5 < jumpDashParticles.Length; num5++)
		{
			ParticleSystem.EmissionModule emission2 = jumpDashParticles[num5].emission;
			emission2.enabled = !PM.sonic.IsSuper && (PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing");
		}
		for (int num6 = 0; num6 < SuperJumpDashFX.Length; num6++)
		{
			ParticleSystem.EmissionModule emission3 = SuperJumpDashFX[num6].emission;
			emission3.enabled = PM.sonic.IsSuper && (PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing");
		}
		Gradient gradient = new Gradient();
		TrailAlpha = (((PM.Base.GetState() == "BoundAttack" && PM.sonic.BoundState == 0) || PM.Base.GetState() == "JumpDash" || PM.Base.GetState() == "JumpDashSTH" || PM.Base.GetState() == "Homing" || (PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 1) || (PM.Base.GetState() == "DashPanel" && DashPadRoll)) ? 1f : Mathf.Lerp(TrailAlpha, 0f, Time.deltaTime * 10f));
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey((!PM.sonic.IsSuper) ? TrailColor : SuperTrailColor, 0f),
			new GradientColorKey((!PM.sonic.IsSuper) ? TrailColor : SuperTrailColor, 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(TrailAlpha, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		Trail.colorGradient = gradient;
		for (int num7 = 0; num7 < homingSmashParticles.Length; num7++)
		{
			ParticleSystem.EmissionModule emission4 = homingSmashParticles[num7].emission;
			emission4.enabled = PM.Base.GetState() == "HomingSmash";
		}
		for (int num8 = 0; num8 < homingSmashStartParticles.Length; num8++)
		{
			ParticleSystem.EmissionModule emission5 = homingSmashStartParticles[num8].emission;
			if (PM.Base.GetState() == "HomingSmash")
			{
				if (!Startparticles)
				{
					Startparticles = true;
					emission5.enabled = true;
					homingSmashStartParticles[num8].Clear();
					homingSmashStartParticles[num8].Simulate(0f, withChildren: true, restart: true);
					homingSmashStartParticles[num8].Play();
				}
			}
			else
			{
				Startparticles = false;
				emission5.enabled = false;
			}
		}
		for (int num9 = 0; num9 < kickAttackParticles.Length; num9++)
		{
			ParticleSystem.EmissionModule emission6 = kickAttackParticles[num9].emission;
			emission6.enabled = !PM.sonic.IsSuper && (PM.Base.GetState() == "Kick" || PM.Base.GetState() == "Slide");
		}
		for (int num10 = 0; num10 < SlideParticles.Length; num10++)
		{
			ParticleSystem.EmissionModule emission7 = SlideParticles[num10].emission;
			emission7.enabled = !PM.sonic.IsSuper && PM.Base.GetState() == "Slide";
		}
		for (int num11 = 0; num11 < SuperKickAttackFX.Length; num11++)
		{
			ParticleSystem.EmissionModule emission8 = SuperKickAttackFX[num11].emission;
			emission8.enabled = PM.sonic.IsSuper && (PM.Base.GetState() == "Kick" || PM.Base.GetState() == "Slide");
		}
		for (int num12 = 0; num12 < SuperSlideFX.Length; num12++)
		{
			ParticleSystem.EmissionModule emission9 = SuperSlideFX[num12].emission;
			emission9.enabled = PM.sonic.IsSuper && PM.Base.GetState() == "Slide";
		}
		for (int num13 = 0; num13 < spinDashParticles.Length; num13++)
		{
			ParticleSystem.EmissionModule emission10 = spinDashParticles[num13].emission;
			emission10.enabled = !PM.sonic.IsSuper && PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 0;
		}
		for (int num14 = 0; num14 < spindashShootParticles.Length; num14++)
		{
			ParticleSystem.EmissionModule emission11 = spindashShootParticles[num14].emission;
			emission11.enabled = !PM.sonic.IsSuper && ((PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 1) || (PM.Base.GetState() == "DashPanel" && DashPadRoll));
		}
		for (int num15 = 0; num15 < SuperSpinDashFX.Length; num15++)
		{
			ParticleSystem.EmissionModule emission12 = SuperSpinDashFX[num15].emission;
			emission12.enabled = PM.sonic.IsSuper && PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 0;
		}
		for (int num16 = 0; num16 < SuperSpinDashShootFX.Length; num16++)
		{
			ParticleSystem.EmissionModule emission13 = SuperSpinDashShootFX[num16].emission;
			emission13.enabled = PM.sonic.IsSuper && ((PM.Base.GetState() == "SpinDash" && PM.sonic.SpinDashState == 1) || (PM.Base.GetState() == "DashPanel" && DashPadRoll));
		}
		for (int num17 = 0; num17 < boundAttackParticles.Length; num17++)
		{
			ParticleSystem.EmissionModule emission14 = boundAttackParticles[num17].emission;
			emission14.enabled = !PM.sonic.IsSuper && PM.Base.GetState() == "BoundAttack" && (PM.sonic.BoundState == 0 || (PM.sonic.BoundState == 1 && PM.sonic.AirMotionVelocity.y > -0.25f));
		}
		for (int num18 = 0; num18 < SuperBoundAttackFX.Length; num18++)
		{
			ParticleSystem.EmissionModule emission15 = SuperBoundAttackFX[num18].emission;
			emission15.enabled = PM.sonic.IsSuper && PM.Base.GetState() == "BoundAttack" && (PM.sonic.BoundState == 0 || (PM.sonic.BoundState == 1 && PM.sonic.AirMotionVelocity.y > -0.25f));
		}
		for (int num19 = 0; num19 < waterRun.Length; num19++)
		{
			ParticleSystem.EmissionModule emission16 = waterRun[num19].emission;
			emission16.enabled = PM.Base.GetState() == "WaterSlide";
		}
		for (int num20 = 0; num20 < GreenGemParticles.Length; num20++)
		{
			ParticleSystem.EmissionModule emission17 = GreenGemParticles[num20].emission;
			emission17.enabled = (PM.sonic.GroundTornado ? (PM.sonic.UsingGreenGem && PM.sonic.SpawnedAirTornado) : PM.sonic.UsingGreenGem);
		}
		for (int num21 = 0; num21 < RedGemParticles.Length; num21++)
		{
			ParticleSystem.EmissionModule emission18 = RedGemParticles[num21].emission;
			emission18.enabled = PM.sonic.UsingRedGem;
		}
		for (int num22 = 0; num22 < PurpleGemParticles.Length; num22++)
		{
			ParticleSystem.EmissionModule emission19 = PurpleGemParticles[num22].emission;
			emission19.enabled = PM.sonic.UsingPurpleGem && PM.Base.HUD.ActiveGemLevel[PM.sonic.GemSelector] > 1;
		}
		for (int num23 = 0; num23 < SkyGemParticles.Length; num23++)
		{
			ParticleSystem.EmissionModule emission20 = SkyGemParticles[num23].emission;
			emission20.enabled = PM.sonic.GunDriveState == 1 && (bool)PM.sonic.SkyGemObject;
		}
		for (int num24 = 0; num24 < GunDriveParticles.Length; num24++)
		{
			ParticleSystem.EmissionModule emission21 = GunDriveParticles[num24].emission;
			emission21.enabled = PM.sonic.GunDriveAttack;
		}
		if (PM.Base._Rigidbody.velocity != Vector3.zero)
		{
			GunDriveParticles[0].transform.rotation = Quaternion.LookRotation(PM.Base._Rigidbody.velocity.normalized);
		}
		if (PM.Base.GetState() == "Result" && (bool)PM.sonic.ThunderShieldObject)
		{
			PM.sonic.ThunderShieldObject.transform.localPosition = new Vector3(HipBone.localPosition.x * 0.75f, HipBone.localPosition.y - 0.25f, HipBone.localPosition.z);
		}
		if (PM.sonic.IsSuper)
		{
			SuperSkin.SetColor("_Color", SuperSkinGradient.Evaluate(Mathf.Repeat(Time.time, 0.5f) / 0.5f));
		}
		SuperLight.intensity = ((PM.sonic.IsSuper && PM.Base.CurSpeed < 40f) ? Mathf.Lerp(0.75f, 1.25f, Mathf.Abs(Mathf.Cos(Time.time * 10f))) : Mathf.Lerp(SuperLight.intensity, 0f, Time.deltaTime * 5f));
		LightSpeedLight.intensity = ((PM.sonic.IsSuper && PM.Base.CurSpeed >= 40f) ? Mathf.Lerp(1f, 1.5f, Mathf.Abs(Mathf.Cos(Time.time * 10f))) : Mathf.Lerp(LightSpeedLight.intensity, 0f, Time.deltaTime * 5f));
		if (PM.Base.IsGrounded() || PM.Base.GetState() == "LightDash" || PM.Base.GetState() == "Grinding" || PM.Base.GetState() == "Pole" || PM.Base.GetState() == "Path" || PM.Base.GetState() == "WaterSlide" || PM.Base.GetState() == "Hold" || PM.Base.GetState() == "ChainJump" || PM.Base.GetState() == "Orca" || PM.Base.GetState() == "Transform")
		{
			if (PM.Base.GetState() == "LightDash" || PM.Base.GetState() == "Grinding" || PM.Base.GetState() == "Path" || PM.Base.GetState() == "WaterSlide" || PM.Base.GetState() == "Orca")
			{
				SuperFXPivot.rotation = PM.Base.Mesh.rotation;
			}
			else if (PM.Base.GetState() == "Pole")
			{
				SuperFXPivot.rotation = PM.Base.Mesh.rotation * Quaternion.Euler(-90f, 0f, 0f);
			}
			else if (PM.Base.GetState() == "Hold" || PM.Base.GetState() == "ChainJump")
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
		for (int num25 = 0; num25 < RaibowGemParticles.Length; num25++)
		{
			ParticleSystem.EmissionModule emission22 = RaibowGemParticles[num25].emission;
			emission22.enabled = PM.sonic.IsSuper && PM.Base.GetState() != "LightAttack" && PM.Base.CurSpeed < 40f;
		}
		for (int num26 = 0; num26 < LightSpeedFX.Length; num26++)
		{
			ParticleSystem.EmissionModule emission23 = LightSpeedFX[num26].emission;
			emission23.enabled = PM.sonic.IsSuper && PM.Base.GetState() != "LightAttack" && PM.Base.CurSpeed >= 40f;
		}
		for (int num27 = 0; num27 < LightDashParticles.Length; num27++)
		{
			ParticleSystem.EmissionModule emission24 = LightDashParticles[num27].emission;
			emission24.enabled = PM.Base.GetState() == "LightDash";
		}
	}

	public void CreateMachSpeedFX(int GemLevel)
	{
		Object.Instantiate(MachSpeedFX[GemLevel], base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreateSlowdownFX(int GemLevel)
	{
		Object.Instantiate(SlowdownFX[GemLevel], base.transform.position + base.transform.up * 0.25f, Quaternion.identity).transform.SetParent(base.transform);
	}

	public void CreateTornadoFX(int GemLevel)
	{
		Object.Instantiate(TornadoFX[GemLevel], base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
	}

	public void CreateScaleFX()
	{
		Object.Instantiate(ScaleFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreateSkyGemFX()
	{
		Object.Instantiate(SkyGemFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateGunDriveMoveFX()
	{
		Object.Instantiate(GunDriveMoveFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateHomingSmashFX()
	{
		Object.Instantiate(HomingSmashFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation);
	}

	public void CreateTransformFX(bool Super = true)
	{
		if (Super)
		{
			Object.Instantiate(TransformFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity);
		}
		else
		{
			Object.Instantiate(DetransformFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity).transform.SetParent(base.transform);
		}
	}

	public void CreateLightAttackFX()
	{
		Object.Instantiate(LightAttackFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreateKickAttacFX()
	{
		Object.Instantiate((!PM.sonic.IsSuper) ? kickAttackGlowPrefab : SuperKickAttackGlowPrefab, rightFoot.position + new Vector3(-0.075f, 0f, 0f), base.transform.rotation).transform.SetParent(rightFoot);
	}

	public void CreateBoundAttackFX()
	{
		Object.Instantiate((!PM.sonic.IsSuper) ? boundAttackPrefab : SuperBoundAttackPrefab, PM.Base.RaycastHit.point, base.transform.rotation);
	}
}
