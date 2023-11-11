using UnityEngine;

public class KnucklesEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] HandBones;

	public Transform[] KnucklesBones;

	public Transform ScrewdriverTransform;

	public Color TrailColor;

	public TrailRenderer Trail;

	[Header("Knuckles Particles")]
	public ParticleSystem[] ChargeFX;

	public ParticleSystem[] ScrewdriverFX;

	[Header("Instantiation")]
	public GameObject GlideFX;

	public GameObject[] PunchFX;

	public GameObject[] PunchAuraFX;

	public GameObject GroundPoundFX;

	public GameObject LungeChargeFX;

	public GameObject UppercutFX;

	public GameObject QuakeFX;

	public GameObject KnucklesTrailFX;

	private bool ChargeStartFX;

	private float TrailAlpha;

	public override void Update()
	{
		base.Update();
		bool conditions = (PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1) || PM.Base.GetState() == "BallFall" || (PM.Base.GetState() == "ClimbUp" && PM.Base.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll And Fall"));
		UpdateJumpBallFX(conditions);
		Gradient gradient = new Gradient();
		TrailAlpha = (((PM.Base.GetState() == "Quake" && PM.knuckles.QuakeState == 0) || (PM.Base.GetState() == "Screwdriver" && PM.knuckles.ScrewState == 1) || PM.Base.GetState() == "Homing") ? 1f : Mathf.Lerp(TrailAlpha, 0f, Time.deltaTime * 10f));
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
		if (PM.Base.GetState() == "Screwdriver")
		{
			ScrewdriverTransform.rotation = Quaternion.LookRotation(PM.Base._Rigidbody.velocity.normalized);
		}
		if (PM.Base.GetState() == "Quake")
		{
			ScrewdriverTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
		}
		if (PM.Base.GetState() == "Homing")
		{
			ScrewdriverTransform.rotation = Quaternion.LookRotation(PM.knuckles.HomingDirection);
		}
		for (int i = 0; i < ChargeFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = ChargeFX[i].emission;
			emission.enabled = PM.Base.GetState() == "Screwdriver" && PM.knuckles.ScrewState == 0;
			if (PM.Base.GetState() == "Screwdriver")
			{
				if (!ChargeStartFX)
				{
					ChargeFX[i].Play();
					ChargeStartFX = true;
				}
			}
			else
			{
				ChargeStartFX = false;
			}
		}
		for (int j = 0; j < ScrewdriverFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = ScrewdriverFX[j].emission;
			emission2.enabled = (PM.Base.GetState() == "Quake" && PM.knuckles.QuakeState == 0) || (PM.Base.GetState() == "Screwdriver" && PM.knuckles.ScrewState == 1) || PM.Base.GetState() == "Homing";
		}
	}

	public void CreateGlideFX()
	{
		Object.Instantiate(GlideFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreatePunchFX(int HandIndex, int PunchLevel)
	{
		if (PunchLevel < 3)
		{
			Object.Instantiate(PunchAuraFX[PunchLevel], base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
		}
		Object.Instantiate(PunchFX[PunchLevel], HandBones[HandIndex].position, Quaternion.identity).transform.SetParent(HandBones[HandIndex]);
		Object.Instantiate(KnucklesTrailFX, KnucklesBones[(HandIndex != 0) ? 1 : 0].position, KnucklesBones[(HandIndex != 0) ? 1 : 0].rotation).transform.SetParent(KnucklesBones[(HandIndex != 0) ? 1 : 0]);
	}

	public void CreateFinisherFX(int Type)
	{
		switch (Type)
		{
		case 0:
			Object.Instantiate(GroundPoundFX, PM.Base.RaycastHit.point + base.transform.forward * 0.15f, base.transform.rotation);
			break;
		case 1:
			Object.Instantiate(LungeChargeFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
			break;
		default:
			Object.Instantiate(UppercutFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
			break;
		}
	}

	public void CreateQuakeFX()
	{
		Object.Instantiate(QuakeFX, PM.Base.RaycastHit.point, base.transform.rotation);
	}
}
