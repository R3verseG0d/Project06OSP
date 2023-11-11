using UnityEngine;

public class CharacterTerrain : MonoBehaviour
{
	[Header("Prefab")]
	public TerrainObject[] FootstepFX;

	public ParticleSystem RainStepFX;

	public TerrainObject[] LandFX;

	public ParticleSystem RainLandFX;

	public AudioSource Audio;

	public AudioClip[] OmegaClips;

	[Header("Instantiation")]
	public GameObject[] SandFootprintFX;

	internal CameraEffects CamEff;

	internal bool IsRaining;

	internal bool IsOmega;

	internal int CharacterIndex;

	private bool BlockedRain;

	private float SpawnTimer;

	private void Start()
	{
		CamEff = Object.FindObjectOfType<CameraEffects>();
		IsRaining = CamEff.GetRainBool();
	}

	private void Update()
	{
		SpawnTimer += Time.deltaTime;
		BlockedRain = !CamEff.CameraWaterDrops.FX[0].emission.enabled;
	}

	public void PlayFootstep(int Index, bool Raining = false, bool IsFootprint = false)
	{
		if (!Raining || (Raining && BlockedRain))
		{
			if ((bool)FootstepFX[Index].FX)
			{
				FootstepFX[Index].FX.Play();
			}
		}
		else
		{
			RainStepFX.Play();
		}
		if (IsFootprint && SpawnTimer > 0.01f)
		{
			Object.Instantiate(SandFootprintFX[CharacterIndex], base.transform.position, base.transform.rotation * Quaternion.Euler(90f, 0f, 0f));
			SpawnTimer = 0f;
		}
		if (!IsOmega)
		{
			if (FootstepFX[Index].Clips.Length > 1)
			{
				Audio.PlayOneShot(FootstepFX[Index].Clips[Random.Range(0, FootstepFX[Index].Clips.Length)], FootstepFX[Index].Volume);
			}
			else
			{
				Audio.PlayOneShot(FootstepFX[Index].Clips[0], FootstepFX[Index].Volume);
			}
		}
		else
		{
			Audio.PlayOneShot(OmegaClips[0], 0.5f);
		}
	}

	public void PlayLand(int Index, Vector3 Pos, Quaternion Rot, bool Raining = false)
	{
		if (!Raining || (Raining && BlockedRain))
		{
			if ((bool)LandFX[Index].FX)
			{
				LandFX[Index].FX.transform.position = Pos;
				LandFX[Index].FX.transform.rotation = Rot;
				LandFX[Index].FX.Play();
			}
		}
		else
		{
			RainLandFX.transform.position = Pos;
			RainLandFX.transform.rotation = Rot;
			RainLandFX.Play();
		}
		if (!IsOmega)
		{
			if (LandFX[Index].Clips.Length > 1)
			{
				Audio.PlayOneShot(LandFX[Index].Clips[Random.Range(0, LandFX[Index].Clips.Length)], LandFX[Index].Volume);
			}
			else
			{
				Audio.PlayOneShot(LandFX[Index].Clips[0], LandFX[Index].Volume);
			}
		}
		else
		{
			Audio.PlayOneShot(OmegaClips[1], 1.5f);
		}
	}
}
