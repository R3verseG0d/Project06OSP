using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveScene : MonoBehaviour
{
	[Header("Prefab")]
	public Animator SaveIconAnimator;

	private float StartTime;

	private bool FadeAway;

	private bool Done;

	private bool ChangeScene;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void Update()
	{
		if (!Done && Time.time - StartTime > 1.9f)
		{
			SaveIconAnimator.SetTrigger("On End");
			Singleton<GameData>.Instance.SaveGameData();
			Done = true;
		}
		if (!FadeAway && SaveIconAnimator.GetCurrentAnimatorStateInfo(0).IsName("SaveIcon_End"))
		{
			StartTime = Time.time;
			FadeAway = true;
		}
		if (FadeAway && !ChangeScene && Time.time - StartTime > 0.5f)
		{
			SceneManager.LoadScene(Singleton<GameManager>.Instance.LoadingTo, LoadSceneMode.Single);
			ChangeScene = true;
		}
	}
}
