using UnityEngine;

public class PlayRandomSounds : MonoBehaviour
{
	public AudioSource Audio;

	public AudioClip[] Sounds;

	private void Start()
	{
		Audio.PlayOneShot(Sounds[Random.Range(0, Sounds.Length)], Audio.volume);
	}
}
