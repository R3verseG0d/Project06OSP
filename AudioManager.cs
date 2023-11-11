using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
	internal AudioMixer MainMixer;

	internal AudioSource Audio;

	internal AudioSource VoiceAudio;

	protected AudioManager()
	{
	}

	public void StartAudioManager()
	{
	}

	private void Awake()
	{
		MainMixer = Resources.Load("Master Mixer") as AudioMixer;
		if (!Audio)
		{
			Audio = base.gameObject.AddComponent<AudioSource>();
			Audio.outputAudioMixerGroup = MainMixer.FindMatchingGroups("Sounds")[0];
			Audio.bypassReverbZones = true;
		}
		if (!VoiceAudio)
		{
			GameObject gameObject = new GameObject("VoiceAudio");
			gameObject.transform.SetParent(base.transform);
			VoiceAudio = gameObject.AddComponent<AudioSource>();
			VoiceAudio.outputAudioMixerGroup = MainMixer.FindMatchingGroups("Voices")[0];
			VoiceAudio.bypassReverbZones = true;
		}
	}

	public void PlayClip(AudioClip Clip, float multiplier = 1f)
	{
		Audio.PlayOneShot(Clip, Audio.volume * multiplier);
	}

	public void PlayClip(AudioSource Source, AudioClip Clip, float multiplier = 1f)
	{
		Source.PlayOneShot(Clip, Source.volume * multiplier);
	}

	public void PlayVoiceClip(AudioClip Clip, float multiplier = 1f, bool Uncut = false)
	{
		if (!Uncut)
		{
			VoiceAudio.Stop();
		}
		VoiceAudio.PlayOneShot(Clip, VoiceAudio.volume * multiplier);
	}

	public void PlayVoiceClip(AudioSource Source, AudioClip Clip, float multiplier = 1f, bool Uncut = false)
	{
		if (!Uncut)
		{
			Source.Stop();
		}
		Source.PlayOneShot(Clip, Source.volume * multiplier);
	}

	private void Update()
	{
		MainMixer.SetFloat("Music", Mathf.Log(Singleton<Settings>.Instance.settings.MusicVolume) * 20f);
		MainMixer.SetFloat("Sounds", Mathf.Log(Singleton<Settings>.Instance.settings.SEVolume) * 20f);
		MainMixer.SetFloat("Voices", Mathf.Log(Singleton<Settings>.Instance.settings.VoiceVolume) * 20f);
	}
}
