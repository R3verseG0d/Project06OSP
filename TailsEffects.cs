using UnityEngine;

public class TailsEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] TailBones;

	[Header("Tails Particles")]
	public ParticleSystem[] DRSFX;

	public ParticleSystem[] SwipeLoopFX;

	[Header("Instantiation")]
	public GameObject FlyFX;

	public GameObject SwipeFX;

	public GameObject TailTrailFX;

	public override void Update()
	{
		base.Update();
		bool conditions = PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1 && PM.tails.CanAirThrow;
		UpdateJumpBallFX(conditions);
		for (int i = 0; i < DRSFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = DRSFX[i].emission;
			emission.enabled = PM.Base.GetState() == "DummyRingBomb" && PM.tails.DRSnipe && PM.tails.DRBState == 0;
		}
		SwipeLoopFX[0].transform.localEulerAngles = PM.Base.BodyTransform.localEulerAngles;
		for (int j = 0; j < SwipeLoopFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = SwipeLoopFX[j].emission;
			emission2.enabled = PM.Base.GetState() == "TailSwipe" && PM.tails.UseRythmBadge;
		}
	}

	public void CreateFlyFX()
	{
		Object.Instantiate(FlyFX, base.transform.position + base.transform.up * 0.25f, Quaternion.identity).transform.SetParent(base.transform);
		for (int i = 0; i < TailBones.Length; i++)
		{
			Object.Instantiate(TailTrailFX, TailBones[i].position, Quaternion.identity).transform.SetParent(TailBones[i]);
		}
	}

	public void CreateTailSwipeFX()
	{
		Object.Instantiate(SwipeFX, base.transform.position + base.transform.up * 0.25f, base.transform.rotation).transform.SetParent(base.transform);
		for (int i = 0; i < TailBones.Length; i++)
		{
			Object.Instantiate(TailTrailFX, TailBones[i].position, Quaternion.identity).transform.SetParent(TailBones[i]);
		}
	}
}
