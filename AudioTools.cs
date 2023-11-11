using UnityEngine;

public class AudioTools : ObjectBase
{
	public enum Kit
	{
		IntroLoop = 0,
		Pitch = 1,
		RandomTime = 2,
		Delete = 3,
		PlayOnTrigger = 4
	}

	public Kit ToolKit;

	public AudioSource Audio;

	public AudioClip Intro;

	public AudioClip Loop;

	public AudioClip End;

	private bool PlayedLoop;

	private bool PlayedEnd;

	private bool PlayEnd;

	public Vector2 PitchRanges;

	private void Start()
	{
		if (ToolKit == Kit.IntroLoop)
		{
			Audio.clip = Intro;
			Audio.Play();
			PlayEnd = End;
		}
		else if (ToolKit == Kit.Pitch)
		{
			Audio.pitch = Random.Range(PitchRanges.x, PitchRanges.y);
			Audio.Play();
		}
		else if (ToolKit == Kit.RandomTime)
		{
			Audio.time = Random.Range(0.1f, Audio.clip.length);
			Audio.Play();
		}
	}

	private void Update()
	{
		if (ToolKit == Kit.IntroLoop)
		{
			if (!PlayedLoop && !Audio.isPlaying)
			{
				Audio.loop = ((!PlayEnd) ? true : false);
				Audio.clip = Loop;
				Audio.Play();
				PlayedLoop = true;
			}
			if (PlayEnd && !PlayedEnd && PlayedLoop && !Audio.isPlaying)
			{
				Audio.clip = End;
				Audio.Play();
				PlayedEnd = true;
			}
		}
		else if (ToolKit == Kit.Delete && (bool)Audio && !Audio.isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (ToolKit == Kit.PlayOnTrigger && (bool)GetPlayer(collider))
		{
			Audio.Play();
		}
	}
}
