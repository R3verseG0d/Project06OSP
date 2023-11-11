using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
	[Header("Framework")]
	public PlayerManager PM;

	[Header("Particles")]
	public GameObject TerrainPrefab;

	[Header("Sounds")]
	public AudioClip[] GrindTrickSounds;

	[Header("Shadow")]
	public AudioSource Skate1;

	public AudioSource Skate2;

	[Header("Tails")]
	public AudioSource RythmBadge;

	[Header("Amy")]
	public AudioSource HammerWind;

	[Header("Rouge")]
	public AudioSource WingFlap;

	[Header("Optional")]
	public Vector2 LandFXGoalOffset;

	public Transform FootstepOverride;

	internal CharacterTerrain Terrain;

	internal string GroundTag;

	private RaycastHit TagHit;

	internal LayerMask Tag_Mask => LayerMask.GetMask("DetectionParticleCollider");

	private void Awake()
	{
		Terrain = Object.Instantiate(TerrainPrefab, base.transform.position, base.transform.rotation).GetComponent<CharacterTerrain>();
		Terrain.IsOmega = PM.Base.GetPrefab("omega");
		Terrain.CharacterIndex = (int)PM.Base.PlayerPrefab;
		Terrain.transform.SetParent(PM.transform);
	}

	public string GetTag()
	{
		if (Physics.Raycast(PM.transform.position, -PM.transform.up, out TagHit, PM.Base.MaxRayLenght - 0.25f, Tag_Mask))
		{
			return TagHit.transform.tag;
		}
		return "";
	}

	public void PlayFootsteps(int UseOverride)
	{
		if (UseOverride == 1 && (bool)FootstepOverride)
		{
			Terrain.transform.position = FootstepOverride.position;
		}
		else if (Terrain.transform.localPosition != new Vector3(0f, -0.25f, 0f))
		{
			Terrain.transform.localPosition = new Vector3(0f, -0.25f, 0f);
		}
		if (PM.Base.IsGrounded())
		{
			if (GroundTag == "Normal")
			{
				Terrain.PlayFootstep(0, Terrain.IsRaining);
			}
			if (GroundTag == "Dirt")
			{
				Terrain.PlayFootstep(1, Terrain.IsRaining);
			}
			if (GroundTag == "Sand")
			{
				Terrain.PlayFootstep(2, Terrain.IsRaining, IsFootprint: true);
			}
			if (GroundTag == "Metal")
			{
				Terrain.PlayFootstep(3, Terrain.IsRaining);
			}
			if (GroundTag == "Grass")
			{
				Terrain.PlayFootstep(4, Terrain.IsRaining);
			}
			if (GroundTag == "Wood")
			{
				Terrain.PlayFootstep(5);
			}
			if (GroundTag == "Snow")
			{
				Terrain.PlayFootstep(6);
			}
			if (GroundTag == "Water" || GroundTag == "ShoreWater" || GetTag() == "Water" || GetTag() == "ShoreWater")
			{
				Terrain.PlayFootstep(PM.Base.GetPrefab("sonic_fast") ? 8 : 7, Raining: false, (GroundTag == "ShoreWater" || GetTag() == "ShoreWater") ? true : false);
			}
			if (GroundTag == "Mercury")
			{
				Terrain.PlayFootstep(9);
			}
		}
	}

	private Vector3 LandFXPos()
	{
		if (!(PM.Base.GetState() != "Result"))
		{
			return PM.Base.RaycastHit.point + PM.transform.forward * LandFXGoalOffset.x + PM.transform.right * LandFXGoalOffset.y;
		}
		return PM.Base.RaycastHit.point;
	}

	public void CreateLandFXAndSound()
	{
		if (GroundTag == "Normal")
		{
			Terrain.PlayLand(0, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Dirt")
		{
			Terrain.PlayLand(1, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Sand")
		{
			Terrain.PlayLand(2, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Metal")
		{
			Terrain.PlayLand(3, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Grass")
		{
			Terrain.PlayLand(4, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Wood")
		{
			Terrain.PlayLand(5, LandFXPos(), PM.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Snow")
		{
			Terrain.PlayLand(6, LandFXPos(), PM.transform.rotation);
		}
		if (GroundTag == "Water" || GetTag() == "Water")
		{
			Terrain.PlayLand(7, LandFXPos(), PM.transform.rotation);
		}
		if (GroundTag == "Mercury")
		{
			Terrain.PlayLand(8, LandFXPos(), PM.transform.rotation);
		}
	}

	public void CreateLandFXAndSoundCustomRot(Vector3 Position, Quaternion Rotation, string Tag = "")
	{
		if (!(Tag == ""))
		{
			if (Tag == "Normal")
			{
				Terrain.PlayLand(0, Position, Rotation);
			}
			if (Tag == "Dirt")
			{
				Terrain.PlayLand(1, Position, Rotation);
			}
			if (Tag == "Sand")
			{
				Terrain.PlayLand(2, Position, Rotation);
			}
			if (Tag == "Metal")
			{
				Terrain.PlayLand(3, Position, Rotation);
			}
			if (Tag == "Grass")
			{
				Terrain.PlayLand(4, Position, Rotation);
			}
			if (Tag == "Wood")
			{
				Terrain.PlayLand(5, Position, Rotation);
			}
			if (Tag == "Snow")
			{
				Terrain.PlayLand(6, Position, Rotation);
			}
			if (Tag == "Water")
			{
				Terrain.PlayLand(7, Position, Rotation);
			}
			if (Tag == "Mercury")
			{
				Terrain.PlayLand(8, Position, Rotation);
			}
		}
	}

	public void PlayMiscSound(AudioClip Sound)
	{
		PM.Base.Audio.PlayOneShot(Sound, PM.Base.Audio.volume);
	}

	public void PlayVoiceSingleRandom(int Group)
	{
		PM.Base.PlayerVoice.PlayRandom(Group, RandomPlayChance: true);
	}

	public void PlayTarotFX()
	{
		PM.amy.AmyEffects.PlayTarotFX(Enable: true);
	}

	public void PlaySkateSound(int Index)
	{
		if (Index == 0)
		{
			Skate1.Play();
		}
		else
		{
			Skate2.Play();
		}
	}

	public void PlayRythmBadgeSound()
	{
		RythmBadge.Play();
	}

	public void PlayHammerWindSound()
	{
		HammerWind.pitch = Random.Range(0.9f, 1.1f);
		HammerWind.Play();
	}

	public void PlayWingFlapSound()
	{
		WingFlap.pitch = Random.Range(0.9f, 1.1f);
		WingFlap.Play();
	}

	public void PlayPsiFX(int Result)
	{
		if (PM.Base.GetPrefab("silver"))
		{
			PM.silver.PsiFX = Result != 0;
		}
	}

	public void PlayGrindTrickSounds()
	{
		PM.Base.Audio.PlayOneShot(GrindTrickSounds[PM.Base.RailType], PM.Base.Audio.volume);
	}
}
