using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public struct PlayerData
	{
		public int score;

		public float time;

		public int rings;

		public int maxCollectedRings;

		public CheckpointData checkpoint;
	}

	public enum State
	{
		Menu = 0,
		Loading = 1,
		Playing = 2,
		Hub = 3,
		Paused = 4,
		Result = 5
	}

	public enum Story
	{
		Sonic = 0,
		Shadow = 1,
		Silver = 2,
		Last = 3,
		Other = 4
	}

	public Story GameStory;

	public Scene FirstSection;

	public PlayerData _PlayerData;

	public State GameState;

	public bool GoToActSelect;

	public bool PlayedEventLimit;

	public float SaveTime;

	public float SectionSaveTime;

	public string FirstSectionPath;

	public string LoadingTo;

	public int ActSelectLastIndex;

	public PlayerAttributesData[] StoredPlayerVars;

	public List<string> LifeItemIDs = new List<string>();

	private global::Discord.Discord discord;

	private bool IsDiscordOpen;

	internal bool CountTime;

	protected GameManager()
	{
	}

	[DllImport("user32.dll")]
	public static extern bool SetWindowText(IntPtr hwnd, string lpString);

	[DllImport("user32.dll")]
	public static extern IntPtr FindWindow(string className, string windowName);

	public void StartGameManager()
	{
		SetWindowText(FindWindow(null, Application.productName), Game.TitleBarName);
	}

	private void Start()
	{
		IsDiscordOpen = Process.GetProcessesByName("discord").Length != 0;
		if (!IsDiscordOpen)
		{
			return;
		}
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		long start = Convert.ToInt64((utcNow - dateTime).TotalSeconds);
		discord = new global::Discord.Discord(962193216269082684L, 1uL);
		ActivityManager activityManager = discord.GetActivityManager();
		Activity activity = new Activity
		{
			Timestamps = 
			{
				Start = start
			},
			Assets = 
			{
				LargeImage = "sonicnexticon_new",
				LargeText = "Sonic the Hedgehog (P-06)"
			}
		};
		activityManager.UpdateActivity(activity, delegate(Result res)
		{
			if (res == Result.Ok)
			{
				UnityEngine.Debug.Log("Discord Rich Presence properly initialized.");
			}
		});
	}

	public void SetFirstSectionPath()
	{
		FirstSection = SceneManager.GetActiveScene();
		FirstSectionPath = FirstSection.path.Replace("Assets/", "").Replace(".unity", "");
	}

	public void SetLoadingTo(string BuildScene, string Mode = "")
	{
		LoadingTo = BuildScene;
		GameState = State.Loading;
		if (Mode == Game.AutoSaveMode)
		{
			SceneManager.LoadScene("AutoSaveScene", LoadSceneMode.Single);
		}
		else if (Mode == Game.BlankLoadMode)
		{
			SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
		}
		else if (Singleton<Settings>.Instance.settings.LoadingScreenType != 0)
		{
			SceneManager.LoadScene((Mode == Game.MenuLoadMode) ? "LoadingScreen" : "E3LoadingScreen", LoadSceneMode.Single);
		}
		else
		{
			SceneManager.LoadScene("RetailLoadingScreen", LoadSceneMode.Single);
		}
	}

	public void OnStageStart(bool IsFirstSection, bool KeepTime, bool IsHub)
	{
		if (IsFirstSection)
		{
			_PlayerData.score = 0;
			_PlayerData.rings = 0;
			_PlayerData.maxCollectedRings = 0;
			StoredPlayerVars = null;
			SetFirstSectionPath();
			if (!KeepTime)
			{
				SaveTime = 0f;
				SectionSaveTime = 0f;
			}
		}
		else if (FirstSectionPath == null)
		{
			SetFirstSectionPath();
		}
		_PlayerData.time = SaveTime;
		GC.Collect();
		GameState = ((!IsHub) ? State.Playing : State.Hub);
		CountTime = true;
	}

	public void OnChangeSection()
	{
		SaveTime = _PlayerData.time;
		SectionSaveTime = _PlayerData.time;
		if (_PlayerData.checkpoint != null)
		{
			_PlayerData.checkpoint.Saved = false;
		}
		LifeItemIDs.Clear();
		ResetTimeScaleAndSoundPitch();
	}

	public void OnStageRestart(bool RestartStage)
	{
		if (!RestartStage)
		{
			if (_PlayerData.checkpoint != null)
			{
				_PlayerData.checkpoint.Saved = false;
			}
			SaveTime = SectionSaveTime;
		}
		_PlayerData = default(PlayerData);
		GameData.StoryData storyData = GetStoryData();
		storyData.Lives--;
		SetStoryData(storyData);
		StoredPlayerVars = null;
		LifeItemIDs.Clear();
		SceneManager.LoadScene(RestartStage ? FirstSectionPath : SceneManager.GetActiveScene().name);
	}

	public void OnPlayerDeath()
	{
		GameData.StoryData storyData = GetStoryData();
		storyData.Lives--;
		SetStoryData(storyData);
		StoredPlayerVars = null;
		if (storyData.Lives < 0)
		{
			Exit(GameOver: true);
			return;
		}
		_PlayerData.score = 0;
		_PlayerData.rings = 0;
		_PlayerData.maxCollectedRings = 0;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Exit(bool GameOver = false)
	{
		_PlayerData = default(PlayerData);
		GameState = State.Menu;
		StoredPlayerVars = null;
		PlayedEventLimit = false;
		if (!GameOver)
		{
			GoToActSelect = !SceneManager.GetActiveScene().name.Contains("test") && SceneManager.GetActiveScene().name != "kdv_e_sn" && SceneManager.GetActiveScene().name != "csc_f_sv";
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
		}
		else
		{
			Singleton<GameData>.Instance.OnGameOver();
			SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
		}
		ResetTimeScaleAndSoundPitch();
	}

	public void ResetTimeScaleAndSoundPitch()
	{
		if (GameState != State.Paused && Time.timeScale != 1f)
		{
			Time.timeScale = 1f;
		}
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("MusicPitch", 1f);
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("SoundsPitch", 1f);
		Singleton<AudioManager>.Instance.MainMixer.SetFloat("VoicesPitch", 1f);
	}

	public void KeepPlayerAttributes(PlayerAttributesData[] Variables)
	{
		StoredPlayerVars = Variables;
	}

	private void Update()
	{
		if (IsDiscordOpen)
		{
			discord.RunCallbacks();
		}
	}

	private void FixedUpdate()
	{
		if (CountTime && GameState == State.Playing)
		{
			_PlayerData.time += Time.fixedDeltaTime;
		}
		Singleton<GameData>.Instance.Playtime += Time.fixedUnscaledDeltaTime;
		Singleton<GameData>.Instance.Playtime = Mathf.Clamp(Singleton<GameData>.Instance.Playtime, 0f, 3599999f);
	}

	public string GetGameStory()
	{
		return GameStory.ToString();
	}

	public void SetGameStory(string Story)
	{
		GameStory = (Story)Enum.Parse(typeof(Story), Story);
	}

	public int GetLifeCount()
	{
		return GetStoryData().Lives;
	}

	public GameData.GlobalData GetGameData()
	{
		return Singleton<GameData>.Instance.Game;
	}

	public void SetGameData(GameData.GlobalData GlobalData)
	{
		Singleton<GameData>.Instance.Game = GlobalData;
	}

	public GameData.StoryData GetStoryData()
	{
		switch (GameStory)
		{
		case Story.Sonic:
			return Singleton<GameData>.Instance.Sonic;
		case Story.Shadow:
			return Singleton<GameData>.Instance.Shadow;
		case Story.Silver:
			return Singleton<GameData>.Instance.Silver;
		case Story.Last:
			return Singleton<GameData>.Instance.Last;
		default:
			return Singleton<GameData>.Instance.Sonic;
		}
	}

	public GameData.StoryData GetStringStoryData(string _Story)
	{
		switch (_Story)
		{
		case "Sonic":
			return Singleton<GameData>.Instance.Sonic;
		case "Shadow":
			return Singleton<GameData>.Instance.Shadow;
		case "Silver":
			return Singleton<GameData>.Instance.Silver;
		case "Last":
			return Singleton<GameData>.Instance.Last;
		default:
			return Singleton<GameData>.Instance.Sonic;
		}
	}

	public void SetStoryData(GameData.StoryData StoryData)
	{
		switch (GameStory)
		{
		case Story.Sonic:
			Singleton<GameData>.Instance.Sonic = StoryData;
			break;
		case Story.Shadow:
			Singleton<GameData>.Instance.Shadow = StoryData;
			break;
		case Story.Silver:
			Singleton<GameData>.Instance.Silver = StoryData;
			break;
		case Story.Last:
			Singleton<GameData>.Instance.Last = StoryData;
			break;
		}
	}
}
