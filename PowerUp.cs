using UnityEngine;

public class PowerUp : MonoBehaviour
{
	public bool IsJingle;

	[Header("Prefab")]
	public AudioSource Audio;

	public AudioClip[] Jingles;

	private void Start()
	{
		if (IsJingle)
		{
			Audio.PlayOneShot(Jingles[Singleton<Settings>.Instance.settings.E3XBLAMusic], Audio.volume);
		}
	}
}
