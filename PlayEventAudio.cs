using UnityEngine;

public class PlayEventAudio : MonoBehaviour
{
	[Header("Voice")]
	public AudioSource VoiceAudio;

	public AudioClip[] English;

	public AudioClip[] Japanese;

	[Header("Sounds")]
	public AudioSource[] Audios;

	[Header("Optional")]
	public bool PlayOnStart;

	public int StartIndex;

	private void Start()
	{
		if (PlayOnStart)
		{
			PlayEventVoice(StartIndex);
		}
	}

	public void PlayEventVoice(int Index)
	{
		string text = Singleton<Settings>.Instance.AudioLanguage();
		if (!(text == "e"))
		{
			if (text == "j")
			{
				VoiceAudio.PlayOneShot(Japanese[Index], VoiceAudio.volume);
			}
		}
		else
		{
			VoiceAudio.PlayOneShot(English[Index], VoiceAudio.volume);
		}
	}

	public void PlayEventClip(int Index)
	{
		Audios[Index].Play();
	}
}
