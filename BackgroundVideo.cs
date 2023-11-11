using UnityEngine;
using UnityEngine.Video;

public class BackgroundVideo : MonoBehaviour
{
	[Header("Framework")]
	public VideoPlayer VideoPlayer;

	public VideoClip[] VideoClips;

	[Header("Main Menu")]
	public GameObject BGVideo;

	public GameObject[] BGs;

	private void Start()
	{
		UpdateVideo();
	}

	public void UpdateVideo()
	{
		if ((bool)BGVideo)
		{
			BGVideo.SetActive(Singleton<Settings>.Instance.settings.BGVideo != 11);
		}
		if (BGs.Length != 0)
		{
			BGs[0].SetActive(Singleton<Settings>.Instance.settings.BGVideo != 11);
			BGs[1].SetActive(Singleton<Settings>.Instance.settings.BGVideo == 11);
		}
		if (VideoPlayer.isPlaying)
		{
			VideoPlayer.Stop();
		}
		VideoPlayer.clip = VideoClips[Singleton<Settings>.Instance.settings.BGVideo];
		VideoPlayer.Play();
	}
}
