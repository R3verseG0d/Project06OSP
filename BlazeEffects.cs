using UnityEngine;

public class BlazeEffects : EffectsBase
{
	[Header("Framework")]
	public Transform[] HandBones;

	public Transform FireClawTransform;

	public TrailRenderer[] Trails;

	[Header("Blaze Particles")]
	public ParticleSystem[] AccelJumpFX;

	public ParticleSystem[] AccelJumpFlamesFX;

	public ParticleSystem[] FireClawFX;

	public ParticleSystem[] SpinningClawFX;

	[Header("Instantiation")]
	public GameObject AccelJumpTrailFX;

	public GameObject CrowAttackTrailFX;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public override void Update()
	{
		base.Update();
		bool conditions = PM.Base.GetState() == "Jump" && PM.Base.JumpAnimation == 1;
		UpdateJumpBallFX(conditions);
		for (int i = 0; i < AccelJumpFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = AccelJumpFX[i].emission;
			emission.enabled = PM.Base.GetState() == "AccelJump" && PM.blaze.ReachedApex;
		}
		for (int j = 0; j < AccelJumpFlamesFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = AccelJumpFlamesFX[j].emission;
			emission2.enabled = PM.Base.GetState() == "AccelJump" && PM.blaze.ReachedApex;
		}
		FireClawTransform.rotation = Quaternion.LookRotation((PM.Base.GetState() != "Homing") ? PM.Base._Rigidbody.velocity.normalized : PM.blaze.HomingDirection);
		for (int k = 0; k < FireClawFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission3 = FireClawFX[k].emission;
			emission3.enabled = PM.Base.GetState() == "FireClaw" || PM.Base.GetState() == "Homing";
		}
		SpinningClawFX[0].transform.localEulerAngles = PM.Base.BodyTransform.localEulerAngles;
		for (int l = 0; l < SpinningClawFX.Length; l++)
		{
			ParticleSystem.EmissionModule emission4 = SpinningClawFX[l].emission;
			emission4.enabled = PM.Base.GetState() == "SpinningClaw";
		}
	}

	public void OnClawTrailFX(bool Enable)
	{
		for (int i = 0; i < Trails.Length; i++)
		{
			Trails[i].emitting = Enable;
		}
	}

	public void CreateAccelJumpTrailFX()
	{
		for (int i = 0; i < HandBones.Length; i++)
		{
			Object.Instantiate(AccelJumpTrailFX, HandBones[i].position, HandBones[i].rotation).transform.SetParent(HandBones[i]);
		}
	}

	public void CreateCrowAttackTrailFX()
	{
		for (int i = 0; i < HandBones.Length; i++)
		{
			Object.Instantiate(CrowAttackTrailFX, HandBones[i].position, HandBones[i].rotation).transform.SetParent(HandBones[i]);
		}
	}
}
