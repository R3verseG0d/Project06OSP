using System.Collections.Generic;
using UnityEngine;

public class AmigoParams : MonoBehaviour
{
	public enum Amigo
	{
		player_null = 0,
		sonic_new = 1,
		sonic_fast = 2,
		princess = 3,
		snow_board = 4,
		shadow = 5,
		silver = 6,
		tails = 7,
		amy = 8,
		knuckles = 9,
		blaze = 10,
		rouge = 11,
		omega = 12,
		metal_sonic = 13
	}

	[Header("Common")]
	public Amigo AmigoPrefab;

	public string AmigoName;

	public string AmigoShortName;

	public float WalkSpeed;

	public float RunAcc;

	public Animator Animator;

	public int IdleIDs;

	[Header("Audio")]
	public AudioClip JumpSound;

	public string VoicePattern;

	public int VoiceGroupCount;

	[Header("Effects")]
	public Transform[] BreathPoints;

	public GameObject BreathFX;

	public ParticleSystem[] JumpFX;

	public ParticleSystem[] JumpSA2FX;

	[Header("Events")]
	public AmigoAIBase AB;

	public RuntimeAnimatorController AnimatorOverride;

	[Header("Optional")]
	public Vector2 LandFXGoalOffset;

	public Transform FootstepOverride;

	public Renderer JumpballRenderer;

	internal StageManager StageManager;

	internal AudioClip VictoryVoice;

	internal Vector3[] JumpSA2Pos;

	internal string GroundTag;

	internal bool UseJumpFX;

	private RaycastHit TagHit;

	private List<AudioClip[]> VoiceClip = new List<AudioClip[]>();

	private CharacterTerrain Terrain;

	private float JumpballBlink;

	private bool JumpStartFX;

	internal LayerMask Tag_Mask => LayerMask.GetMask("DetectionParticleCollider");

	private void Awake()
	{
		StageManager = Object.FindObjectOfType<StageManager>();
		Terrain = Object.Instantiate(AB.TerrainPrefab, base.transform.position, base.transform.rotation).GetComponent<CharacterTerrain>();
		Terrain.IsOmega = AmigoName == "omega";
		Terrain.CharacterIndex = (int)AmigoPrefab;
		Terrain.transform.SetParent(base.transform);
		if (AmigoName == "sonic" && (bool)AnimatorOverride && Singleton<Settings>.Instance.settings.TGSSonic == 1)
		{
			Animator.runtimeAnimatorController = AnimatorOverride;
		}
	}

	private void Start()
	{
		LoadVoiceClips();
		UseJumpFX = JumpFX != null;
		if (UseJumpFX && Singleton<Settings>.Instance.settings.SpinEffect == 2)
		{
			JumpSA2Pos = new Vector3[3];
		}
		if (BreathPoints != null && (bool)BreathFX && StageManager._Stage == StageManager.Stage.wap)
		{
			CreateBreathFX();
		}
	}

	public string GetTag()
	{
		if (Physics.Raycast(base.transform.position, -base.transform.up, out TagHit, AB.MaxRayLenght - 0.25f, Tag_Mask))
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
		if (AB.IsGrounded())
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
				Terrain.PlayFootstep(7, Raining: false, (GroundTag == "ShoreWater" || GetTag() == "ShoreWater") ? true : false);
			}
			if (GroundTag == "Mercury")
			{
				Terrain.PlayFootstep(9);
			}
		}
	}

	private Vector3 LandFXPos()
	{
		if (!(AB.GetAmigoState() != "Result"))
		{
			return AB.RaycastHit.point + base.transform.forward * LandFXGoalOffset.x + base.transform.right * LandFXGoalOffset.y;
		}
		return AB.RaycastHit.point;
	}

	public void CreateLandFXAndSound()
	{
		if (GroundTag == "Normal")
		{
			Terrain.PlayLand(0, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Dirt")
		{
			Terrain.PlayLand(1, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Sand")
		{
			Terrain.PlayLand(2, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Metal")
		{
			Terrain.PlayLand(3, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Grass")
		{
			Terrain.PlayLand(4, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Wood")
		{
			Terrain.PlayLand(5, LandFXPos(), base.transform.rotation, Terrain.IsRaining);
		}
		if (GroundTag == "Snow")
		{
			Terrain.PlayLand(6, LandFXPos(), base.transform.rotation);
		}
		if (GroundTag == "Water" || GetTag() == "Water")
		{
			Terrain.PlayLand(7, LandFXPos(), base.transform.rotation);
		}
		if (GroundTag == "Mercury")
		{
			Terrain.PlayLand(8, LandFXPos(), base.transform.rotation);
		}
	}

	public void PlayMiscSound(AudioClip Sound)
	{
		AB.Audio.PlayOneShot(Sound, AB.Audio.volume);
	}

	public void PlaySkateSound(int Index)
	{
		if (Index == 0)
		{
			AB.Skate1.Play();
		}
		else
		{
			AB.Skate2.Play();
		}
	}

	public void LoadVoiceClips()
	{
		string path = "Win32-Xenon/sound/player/player_" + AmigoName + "/" + Singleton<Settings>.Instance.AudioLanguage() + "/all01_v11_" + AmigoShortName + "_aif";
		VictoryVoice = Resources.Load<AudioClip>(path);
		string text = "Win32-Xenon/sound/player/player_" + AmigoName + "/" + Singleton<Settings>.Instance.AudioLanguage() + "/";
		for (int i = 0; i < VoiceGroupCount; i++)
		{
			string voicePattern = VoicePattern;
			voicePattern = voicePattern.Replace("[g]", i.ToString("D2"));
			List<AudioClip> list = new List<AudioClip>();
			int num = 0;
			while (true)
			{
				string text2 = ((num != 0) ? voicePattern.Replace("[gc]", (num + 1).ToString()) : voicePattern.Replace("[gc]", ""));
				AudioClip audioClip = Resources.Load<AudioClip>(text + text2);
				if (!(audioClip != null))
				{
					break;
				}
				list.Add(audioClip);
				num++;
			}
			VoiceClip.Add(list.ToArray());
		}
	}

	public void PlayRandomVoice(int Group, bool RandomPlayChance = false, bool RandomMulticast = false)
	{
		if ((bool)AB.FollowTarget.HUD.MsgBoxObject)
		{
			return;
		}
		int num = Random.Range(0, VoiceClip[Group].Length + 1);
		if (VoiceClip[Group].Length != 0)
		{
			int num2 = Random.Range(0, VoiceClip[Group].Length + 1);
			if (((RandomPlayChance && num2 > 0) || !RandomPlayChance) && ((RandomMulticast && num != 0) || !RandomMulticast))
			{
				Singleton<AudioManager>.Instance.PlayVoiceClip(AB.VoicesAudio, VoiceClip[Group][Random.Range(0, VoiceClip[Group].Length)]);
			}
		}
	}

	public void CreateBreathFX()
	{
		for (int i = 0; i < BreathPoints.Length; i++)
		{
			ParticleSystem component = Object.Instantiate(BreathFX, BreathPoints[i].position, BreathPoints[i].rotation).GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			main.duration = Random.Range(2.5f, 3.5f);
			component.transform.SetParent(BreathPoints[i]);
			component.Play();
		}
	}

	public void UpdateJumpFX()
	{
		if (Singleton<Settings>.Instance.settings.SpinEffect != 2)
		{
			for (int i = 0; i < JumpFX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = JumpFX[i].emission;
				emission.enabled = UseJumpFX && AB.GetAmigoState() == "Jump" && AB.JumpAnimation == 1 && Singleton<Settings>.Instance.settings.SpinEffect != 1;
				if (UseJumpFX && AB.GetAmigoState() == "Jump" && Singleton<Settings>.Instance.settings.SpinEffect != 1)
				{
					if (AB.JumpAnimation == 1)
					{
						if (!JumpStartFX)
						{
							JumpStartFX = true;
							JumpFX[i].Play();
						}
					}
					else if (AB.JumpAnimation == 2)
					{
						JumpStartFX = false;
						JumpFX[i].Stop();
					}
				}
				else
				{
					JumpStartFX = false;
					JumpFX[i].Stop();
				}
			}
		}
		else
		{
			for (int j = 0; j < JumpSA2FX.Length; j++)
			{
				ParticleSystem.EmissionModule emission2 = JumpSA2FX[j].emission;
				emission2.enabled = UseJumpFX && AB.GetAmigoState() == "Jump";
			}
		}
	}

	public void UpdateJumpBallFX(bool Conditions)
	{
		if (!JumpballRenderer || Singleton<Settings>.Instance.settings.SpinEffect != 1)
		{
			return;
		}
		if (Conditions)
		{
			JumpballBlink += Time.deltaTime * 19f;
			if (JumpballBlink >= 1f)
			{
				JumpballBlink = 0f;
			}
			JumpballRenderer.enabled = JumpballBlink <= 0.5f;
		}
		else if (JumpballRenderer.enabled)
		{
			JumpballRenderer.enabled = false;
		}
	}
}
