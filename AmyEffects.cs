using Ara;
using UnityEngine;

public class AmyEffects : EffectsBase
{
	[Header("Framework")]
	public Transform HammerPeak;

	public AraTrail HammerTrail;

	[Header("Amy Particles")]
	public ParticleSystem[] TarotFX;

	public ParticleSystem[] HammerInvokeFX;

	public ParticleSystem[] HammerSpinFX;

	public ParticleSystem[] StarsFX;

	[Header("Instantiation")]
	public GameObject DoubleJumpFX;

	public GameObject HammerJumpTrailFX;

	public GameObject HammerAttackFX;

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
		HammerSpinFX[0].transform.localEulerAngles = PM.Base.BodyTransform.localEulerAngles;
		for (int i = 0; i < HammerSpinFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = HammerSpinFX[i].emission;
			emission.enabled = PM.Base.GetState() == "HammerSpin" && PM.amy.HammerSpinState == 1;
		}
		for (int j = 0; j < StarsFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = StarsFX[j].emission;
			emission2.enabled = PM.Base.GetState() == "HammerSpin" && PM.amy.HammerSpinState == 2;
		}
	}

	public void OnHammerTrailFX(bool Enable)
	{
		HammerTrail.emit = Enable;
	}

	public void PlayTarotFX(bool Enable)
	{
		for (int i = 0; i < TarotFX.Length; i++)
		{
			if (Enable)
			{
				TarotFX[i].gameObject.SetActive(value: true);
				TarotFX[i].Play();
				PM.amy.TarotAudioSource.Play();
			}
			else
			{
				TarotFX[i].gameObject.SetActive(value: false);
				PM.amy.TarotAudioSource.Stop();
			}
		}
	}

	public void CreateDoubleJumpFX()
	{
		Object.Instantiate(DoubleJumpFX, base.transform.position, Quaternion.identity);
	}

	public void PlayHammerInvokeFX()
	{
		for (int i = 0; i < HammerInvokeFX.Length; i++)
		{
			HammerInvokeFX[i].Play();
		}
	}

	public void CreateHammerJumpTrailFX()
	{
		Object.Instantiate(HammerJumpTrailFX, HammerPeak.position, HammerPeak.rotation).transform.SetParent(HammerPeak);
	}

	public void CreateHammerAttackFX(float OffsetFwd)
	{
		Object.Instantiate(HammerAttackFX, base.transform.position + base.transform.up * -0.25f + base.transform.forward * OffsetFwd, base.transform.rotation);
	}
}
