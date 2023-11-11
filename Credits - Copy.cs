using UnityEngine;

public class Credits : UIBase
{
	public bool IsCreditsMenu;

	public Animator Animator;

	public AudioSource Audio;

	private float StartTime;

	private bool Dismissed;

	private void Start()
	{
		if (!IsCreditsMenu)
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			if (!gameData.HasFlag(Game.CreditsPlayed))
			{
				gameData.ActivateFlag(Game.CreditsPlayed);
			}
			Singleton<GameManager>.Instance.SetGameData(gameData);
		}
		else
		{
			StartTime = Time.time;
		}
		Settings.SetLocalSettings();
	}

	private void Update()
	{
		if (IsCreditsMenu && Time.time - StartTime > 2f && !Dismissed && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
		{
			Animator.SetTrigger("On Dismiss");
			Audio.Play();
			Dismissed = true;
		}
	}

	private void GoToLoadingScreen()
	{
		Singleton<GameManager>.Instance.SetLoadingTo("MainMenu", Game.AutoSaveMode);
	}
}
