using UnityEngine;

public class RougeEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] FootBones;

	public Transform KickDiveTransform;

	[Header("Rouge Particles")]
	public ParticleSystem[] KickDiveFX;

	public ParticleSystem[] BombChargeFX;

	[Header("Instantiation")]
	public GameObject TrailFX;

	public GameObject FinisherTrailFX;

	public GameObject FlyingKickFX;

	public GameObject GlideFX;

	public GameObject KickDiveLandFX;

	public override void Update()
	{
		base.Update();
		bool conditions = (PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1) || PM.Base.GetState() == "BallFall" || (PM.Base.GetState() == "ClimbUp" && PM.Base.Animator.GetCurrentAnimatorStateInfo(0).IsName("Roll And Fall"));
		UpdateJumpBallFX(conditions);
		if (PM.Base.GetState() == "KickDive")
		{
			KickDiveTransform.localEulerAngles = new Vector3(90f, 0f, 0f);
		}
		for (int i = 0; i < KickDiveFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = KickDiveFX[i].emission;
			emission.enabled = PM.Base.GetState() == "KickDive" && PM.rouge.KickDiveState == 0;
		}
		for (int j = 0; j < BombChargeFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = BombChargeFX[j].emission;
			emission2.enabled = PM.Base.GetState() == "BombScatter" && PM.rouge.ScatterState == 0;
		}
	}

	public void CreateGlideFX()
	{
		Object.Instantiate(GlideFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
	}

	public void CreateKickFX(int KickLevel)
	{
		if (KickLevel < 3)
		{
			Object.Instantiate(TrailFX, FootBones[KickLevel - 1].position, FootBones[KickLevel - 1].rotation).transform.SetParent(FootBones[KickLevel - 1]);
		}
	}

	public void CreateFlyingKickFX()
	{
		Object.Instantiate(FlyingKickFX, base.transform.position, base.transform.rotation).transform.SetParent(base.transform);
		for (int i = 0; i < FootBones.Length; i++)
		{
			Object.Instantiate(FinisherTrailFX, FootBones[i].position, FootBones[i].rotation).transform.SetParent(FootBones[i]);
		}
	}

	public void CreateKickDiveLandFX()
	{
		Object.Instantiate(KickDiveLandFX, PM.Base.RaycastHit.point + base.transform.forward * 0.35f, base.transform.rotation);
	}
}
