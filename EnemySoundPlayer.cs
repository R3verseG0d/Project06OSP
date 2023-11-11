using UnityEngine;

public class EnemySoundPlayer : MonoBehaviour
{
	public AudioClip[] Steps;

	public AudioClip Land;

	public AudioClip Misc;

	public AudioSource Audio;

	public void PlayStepSound()
	{
		Audio.PlayOneShot(Steps[Random.Range(0, Steps.Length)], Audio.volume);
	}

	public void PlayLandSound()
	{
		Audio.PlayOneShot(Land, Audio.volume);
	}

	public void PlayMiscSound()
	{
		Audio.PlayOneShot(Misc, Audio.volume);
	}
}
