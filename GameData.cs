using System;
using System.Collections.Generic;
using System.IO;
using STHEngine;
using UnityEngine;

public class GameData : Singleton<GameData>
{
	[Serializable]
	public struct StoryData
	{
		public int Lives;
	}

	[Serializable]
	public struct GlobalData
	{
		public List<int> ObtainedGems;

		public List<StageData> StageRecords;

		public int TotalGoldMedals;

		public HashSet<string> Flags;

		public int Banner;

		public float Playtime;

		public string LastSave;

		public void InitGems()
		{
			if (ObtainedGems == null)
			{
				ObtainedGems = new List<int>();
				ObtainedGems.Add(0);
			}
		}

		public bool HasFlag(string Flag)
		{
			return GetFlags().Contains(Flag);
		}

		public void ActivateFlag(string Flag)
		{
			GetFlags().Add(Flag);
		}

		private HashSet<string> GetFlags()
		{
			if (Flags == null)
			{
				Flags = new HashSet<string>();
			}
			return Flags;
		}
	}

	[Serializable]
	public class SaveData
	{
		public GlobalData game;

		public StoryData sonic;

		public StoryData shadow;

		public StoryData silver;

		public StoryData last;

		public string version;

		public SaveData(GlobalData _game, StoryData _sonic, StoryData _shadow, StoryData _silver, StoryData _last, string _version)
		{
			game = _game;
			sonic = _sonic;
			shadow = _shadow;
			silver = _silver;
			last = _last;
			version = _version;
		}
	}

	public GlobalData Game;

	public StoryData Sonic;

	public StoryData Shadow;

	public StoryData Silver;

	public StoryData Last;

	public string Version;

	public int SaveIndex;

	private bool Initialized;

	internal float Playtime;

	private string GameDataPath
	{
		get
		{
			GetSavesPath();
			string saveFile = GetSaveFile(SaveIndex);
			string savesPath = GetSavesPath();
			if (!Directory.Exists(savesPath))
			{
				Directory.CreateDirectory(savesPath);
			}
			return savesPath + saveFile;
		}
		set
		{
			GameDataPath = value;
		}
	}

	protected GameData()
	{
	}

	public static string GetSavesPath()
	{
		return Application.dataPath.Replace("/" + Application.productName + "_Data", "") + "/SaveFiles";
	}

	public static string GetSaveFile(int Index)
	{
		return "/File" + Index + ".bin";
	}

	private void Awake()
	{
		if (!Initialized)
		{
			LoadGameData();
			Playtime = Game.Playtime;
			Initialized = true;
		}
	}

	public void CreateGameData(int Banner)
	{
		Game = default(GlobalData);
		Game.InitGems();
		Game.TotalGoldMedals = 0;
		Game.Flags = null;
		Game.StageRecords = new List<StageData>();
		Game.Banner = Banner;
		Playtime = 0f;
		Game.Playtime = Playtime;
		Game.LastSave = ExtensionMethods.GetSystemDate();
		Version = Application.version;
		Sonic = default(StoryData);
		Sonic.Lives = 5;
		Shadow = default(StoryData);
		Shadow.Lives = 5;
		Silver = default(StoryData);
		Silver.Lives = 5;
		Last = default(StoryData);
		Last.Lives = 5;
		new SaveData(Game, Sonic, Shadow, Silver, Last, Version).SaveClass(GameDataPath);
	}

	public void SaveGameData()
	{
		Game.LastSave = ExtensionMethods.GetSystemDate();
		Game.Playtime = Playtime;
		Version = Application.version;
		new SaveData(Game, Sonic, Shadow, Silver, Last, Version).SaveClass(GameDataPath);
	}

	public void LoadGameData(bool GameOver = false)
	{
		SaveData saveData = Helper.LoadClass<SaveData>(GameDataPath);
		if (saveData == null)
		{
			Game = default(GlobalData);
			Game.InitGems();
			Game.TotalGoldMedals = 0;
			Game.Flags = null;
			Game.StageRecords = new List<StageData>();
			Playtime = 0f;
			Game.Playtime = Playtime;
			Game.LastSave = ExtensionMethods.GetSystemDate();
			Version = Application.version;
			Sonic = default(StoryData);
			Sonic.Lives = 5;
			Shadow = default(StoryData);
			Shadow.Lives = 5;
			Silver = default(StoryData);
			Silver.Lives = 5;
			Last = default(StoryData);
			Last.Lives = 5;
			return;
		}
		Game = saveData.game;
		Playtime = Game.Playtime;
		Sonic = saveData.sonic;
		Shadow = saveData.shadow;
		Silver = saveData.silver;
		Last = saveData.last;
		Version = saveData.version;
		if (GameOver)
		{
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Sonic:
				Sonic.Lives = 5;
				break;
			case GameManager.Story.Shadow:
				Shadow.Lives = 5;
				break;
			case GameManager.Story.Silver:
				Silver.Lives = 5;
				break;
			case GameManager.Story.Last:
				Last.Lives = 5;
				break;
			}
		}
	}

	public void ResetGameData()
	{
		int banner = Game.Banner;
		Game = default(GlobalData);
		Game.InitGems();
		Game.TotalGoldMedals = 0;
		Game.Flags = null;
		Game.StageRecords = new List<StageData>();
		Game.Banner = banner;
		Playtime = 0f;
		Game.Playtime = Playtime;
		Game.LastSave = ExtensionMethods.GetSystemDate();
		Version = Application.version;
		Sonic = default(StoryData);
		Sonic.Lives = 5;
		Shadow = default(StoryData);
		Shadow.Lives = 5;
		Silver = default(StoryData);
		Silver.Lives = 5;
		Last = default(StoryData);
		Last.Lives = 5;
		SaveGameData();
	}

	public void OnGameOver()
	{
		LoadGameData(GameOver: true);
	}
}
