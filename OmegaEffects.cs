using Ara;
using UnityEngine;

public class OmegaEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] JetBones;

	public Transform[] ShotBones;

	public AraTrail[] OmegaShotTrails;

	[Header("Omega Particles")]
	public ParticleSystem[] JetFireFX;

	public ParticleSystem[] LaserFX;

	public ParticleSystem[] OmegaShotFlamesFX;

	[Header("Instantiation")]
	public GameObject HurtFX;

	public GameObject JetFX;

	public GameObject OmegaShotFX;

	public GameObject OmegaLauncherFX;

	public GameObject GunSmokeFX;

	private bool StartJetFire;

	public override void Update()
	{
		base.Update();
		for (int i = 0; i < JetFireFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = JetFireFX[i].emission;
			if (((PM.Base.GetState() == "Ground" || PM.Base.GetState() == "DashPanel" || PM.Base.GetState() == "Path") && PM.Base.CurSpeed >= 20f) || PM.Base.GetState() == "Hover" || (PM.Base.GetState() == "OmegaShot" && PM.Base.CurSpeed >= 20f) || PM.Base.GetState() == "OmegaLauncher" || (PM.Base.GetState() == "LockOnShot" && !PM.Base.IsGrounded()) || (PM.Base.GetState() == "DashPanel" && PM.Base.CurSpeed >= 20f))
			{
				emission.enabled = true;
				if (!StartJetFire)
				{
					StartJetFire = true;
					CreateJetFireFX();
				}
			}
			else
			{
				emission.enabled = false;
				StartJetFire = false;
			}
		}
		for (int j = 0; j < LaserFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = LaserFX[j].emission;
			emission2.enabled = (PM.Base.GetState() != "LockOnShot" && PM.omega.LockOnTargets) || PM.Base.GetState() == "LockOnShot";
		}
		for (int k = 0; k < OmegaShotFlamesFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission3 = OmegaShotFlamesFX[k].emission;
			emission3.enabled = PM.Base.GetState() == "OmegaShot" && PM.omega.ComboShotCount == 2 && Time.time - PM.omega.ComboShotTimer > 0.1125f && Time.time - PM.omega.ComboShotTimer < 0.25f;
		}
	}

	public void OnOmegaShotTrailsFX(bool Enable)
	{
		for (int i = 0; i < OmegaShotTrails.Length; i++)
		{
			OmegaShotTrails[i].emit = Enable;
		}
	}

	public void CreateHurtFX()
	{
		Object.Instantiate(HurtFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity).transform.SetParent(base.transform);
	}

	public void CreateOmegaShotFX(int ArmID)
	{
		Object.Instantiate(OmegaShotFX, ShotBones[ArmID].position, ShotBones[ArmID].rotation);
	}

	public void CreateOmegaLauncherFX()
	{
		Object.Instantiate(OmegaLauncherFX, base.transform.position + base.transform.up * 1f + base.transform.forward * 0.75f + base.transform.right * ((PM.omega.LauncherSide == 1f) ? (-0.75f) : 0.75f), base.transform.rotation);
	}

	public void CreateJetFireFX()
	{
		for (int i = 0; i < JetBones.Length; i++)
		{
			Object.Instantiate(JetFX, JetBones[i].position, JetBones[i].rotation).transform.SetParent(JetBones[i]);
		}
	}

	public void CreateGunMuzzleFX(Vector3 Pos, Quaternion Rot)
	{
		Object.Instantiate(GunSmokeFX, Pos, Rot).transform.SetParent(base.transform);
	}
}
