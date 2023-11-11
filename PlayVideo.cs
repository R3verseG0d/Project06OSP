using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
	public VideoPlayer Player;

	public string GoTo;

	[Header("Optional")]
	public PlayableDirector SceneDirector;

	public GameObject[] LangChannels;

	private bool Stopped;

	private void Start()
	{
		Player.loopPointReached += EndReached;
		if (LangChannels.Length == 0)
		{
			return;
		}
		string audioLanguage = Singleton<Settings>.Instance.settings.AudioLanguage;
		if (!(audioLanguage == "j"))
		{
			if (audioLanguage == "e")
			{
				Object.Destroy(LangChannels[0]);
			}
		}
		else
		{
			Object.Destroy(LangChannels[1]);
		}
	}

	private void Update()
	{
		if (Player.isPlaying && !Stopped && (Singleton<RInput>.Instance.P.GetButtonDown("Start") || Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
		{
			OnStop();
		}
	}

	private void EndReached(VideoPlayer VP)
	{
		Invoke("NextScene", 1.5f);
	}

	private void OnStop()
	{
		Player.Stop();
		if ((bool)SceneDirector)
		{
			SceneDirector.time = 0.0;
			SceneDirector.Stop();
			SceneDirector.Evaluate();
		}
		Invoke("NextScene", 1.5f);
		Stopped = true;
	}

	private void NextScene()
	{
		SceneManager.LoadScene(GoTo);
	}
}
