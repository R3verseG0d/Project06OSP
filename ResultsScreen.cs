using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreen : UIBase
{
	[Header("Framework")]
	public StateMachine StateMachine;

	public Animator Animator;

	public AudioClip RankSkipSound;

	public AudioSource ResultsSource;

	public AudioClip[] Jingles;

	public AudioSource CountSoundLoop;

	public Image[] ResultTexts;

	public Image RankDisplay;

	public Image RankFXDisplay;

	public Sprite[] RankSprite;

	public Transform[] StageNames;

	public Transform StageMissions;

	public TextMeshProUGUI StageMission;

	public Image[] MissionImages;

	public Image[] SlicedImages;

	public Sprite[] MissionSprites;

	public Sprite[] RetailSlicedSprites;

	public Sprite[] E3SlicedSprites;

	public RectTransform[] NumberHolders;

	public RectTransform BigNumberHolder;

	public Sprite[] Numbers;

	public Sprite[] NumbersBig;

	public Image[] ScoreHud;

	public Image[] RingHud;

	public Image[] MinutesHud;

	public Image[] SecondsHud;

	public Image[] FractionHud;

	public Image[] TimeBonusHud;

	public Image[] RingBonusHud;

	public Image[] TotalScoreHud;

	[Header("Record Stars")]
	public Animator[] StarAnimators;

	private StageData StageRecords;

	[Header("Campaign Themes")]
	public Image StagePlate;

	public Image[] LeftPlates;

	public Image[] RightPlates;

	public Image[] Stats;

	public Image BigStat;

	public Image[] Text;

	public Sprite[] RetailResultThemes;

	public Sprite[] E3ResultThemes;

	public Sprite[] StagePlateThemes;

	public Sprite[] LeftPlateThemes;

	public Sprite[] RightPlateThemes;

	public Sprite[] StatThemes;

	public Sprite[] BigStatThemes;

	public Sprite[] ShadowTextThemes;

	public Sprite[] SilverTextThemes;

	private int MinutesRound;

	private int SecondsRound;

	private int FractionRound;

	internal PlayerBase Player;

	private bool FirstStageSave;

	private string CurrentStage;

	private string Stage;

	private string Char;

	private float StartTime;

	private float TimeRec;

	private float NewTimeRec;

	private int StageRecordsIndex;

	private int Score;

	private int NewScore;

	private int Rings;

	private int NewRings;

	private int TimeBonus;

	private int RingBonus;

	private int TotalScore;

	private int NewTotalScore;

	private int TimeBonusBase;

	private int TimeBonusRate;

	private int RingBonusRate;

	private int Rank_S;

	private int Rank_A;

	private int Rank_B;

	private int Rank_C;

	private int Rank_D;

	private bool DefineBasicRecords;

	private float TimeBonusCount;

	private float RingBonusCount;

	private bool Started;

	private float TotalScoreCount;

	private bool TotalScoreAnim;

	private bool SetScore;

	private bool DefineTotalScoreRecord;

	private bool EndAnim;

	private bool Fading;

	private bool Exited;

	private int FinalRank;

	private int NewFinalRank;

	private string GoTo;

	private void Start()
	{
		ResultTexts[Singleton<Settings>.Instance.settings.DisplayType].enabled = true;
		for (int i = 0; i < MissionImages.Length; i++)
		{
			MissionImages[i].sprite = MissionSprites[Singleton<Settings>.Instance.settings.DisplayType];
		}
		if (Singleton<Settings>.Instance.settings.DisplayType == 0)
		{
			for (int j = 0; j < SlicedImages.Length; j++)
			{
				SlicedImages[j].sprite = RetailSlicedSprites[j];
			}
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Shadow:
				ResultTexts[Singleton<Settings>.Instance.settings.DisplayType].sprite = RetailResultThemes[0];
				break;
			case GameManager.Story.Silver:
				ResultTexts[Singleton<Settings>.Instance.settings.DisplayType].sprite = RetailResultThemes[1];
				break;
			}
		}
		else
		{
			for (int k = 0; k < SlicedImages.Length; k++)
			{
				SlicedImages[k].sprite = E3SlicedSprites[k];
			}
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Shadow:
				ResultTexts[Singleton<Settings>.Instance.settings.DisplayType].sprite = E3ResultThemes[0];
				break;
			case GameManager.Story.Silver:
				ResultTexts[Singleton<Settings>.Instance.settings.DisplayType].sprite = E3ResultThemes[1];
				break;
			}
		}
		switch (Singleton<GameManager>.Instance.GameStory)
		{
		case GameManager.Story.Shadow:
		{
			StagePlate.sprite = StagePlateThemes[0];
			for (int num2 = 0; num2 < LeftPlates.Length; num2++)
			{
				LeftPlates[num2].sprite = LeftPlateThemes[0];
			}
			for (int num3 = 0; num3 < RightPlates.Length; num3++)
			{
				RightPlates[num3].sprite = RightPlateThemes[0];
			}
			for (int num4 = 0; num4 < Stats.Length; num4++)
			{
				Stats[num4].sprite = StatThemes[0];
			}
			BigStat.sprite = BigStatThemes[0];
			for (int num5 = 0; num5 < Text.Length; num5++)
			{
				Text[num5].sprite = ShadowTextThemes[num5];
			}
			break;
		}
		case GameManager.Story.Silver:
		{
			StagePlate.sprite = StagePlateThemes[1];
			for (int l = 0; l < LeftPlates.Length; l++)
			{
				LeftPlates[l].sprite = LeftPlateThemes[1];
			}
			for (int m = 0; m < RightPlates.Length; m++)
			{
				RightPlates[m].sprite = RightPlateThemes[1];
			}
			for (int n = 0; n < Stats.Length; n++)
			{
				Stats[n].sprite = StatThemes[1];
			}
			BigStat.sprite = BigStatThemes[1];
			for (int num = 0; num < Text.Length; num++)
			{
				Text[num].sprite = SilverTextThemes[num];
			}
			break;
		}
		}
		for (int num6 = 0; num6 < NumberHolders.Length; num6++)
		{
			NumberHolders[num6].anchoredPosition = new Vector2((Singleton<Settings>.Instance.settings.DisplayType == 0) ? 0f : 23.5f, NumberHolders[num6].anchoredPosition.y);
		}
		BigNumberHolder.anchoredPosition = new Vector3((Singleton<Settings>.Instance.settings.DisplayType == 0) ? 0f : 43f, BigNumberHolder.anchoredPosition.y);
		CurrentStage = SceneManager.GetActiveScene().name;
		Stage = CurrentStage.Split('_')[0];
		Char = CurrentStage.Split('_')[2];
		StageNames[Singleton<Settings>.Instance.settings.DisplayType].Find(Stage).gameObject.SetActive(value: true);
		string text = "";
		if (CurrentStage == "kdv_e_sn")
		{
			text = "stg_kdv_xbla";
		}
		else if (CurrentStage == "csc_f_sv")
		{
			text = "stg_csc_e3";
		}
		else
		{
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Sonic:
				text = "sonic";
				break;
			case GameManager.Story.Shadow:
				text = "shadow";
				break;
			case GameManager.Story.Silver:
				text = "silver";
				break;
			}
			text = text + "_stg_" + Stage;
			switch (Char)
			{
			case "tl":
				text += "_tails";
				break;
			case "rg":
				text += "_rouge";
				break;
			case "bz":
				text += "_blaze";
				break;
			}
		}
		StageMission.text = Singleton<MSTManager>.Instance.GetSystem(text, ToUpper: true);
		Invoke("PlayResultJingle", 7f);
		GameObject gameObject = GameObject.Find("Enemies");
		if ((bool)gameObject)
		{
			gameObject.SetActive(value: false);
		}
		Object.FindObjectOfType<StageManager>();
		XmlDocument xmlDocument = new XmlDocument();
		string text2 = Char;
		if (CurrentStage == "kdv_e_sn")
		{
			text2 = "xbla";
		}
		else if (CurrentStage == "csc_f_sv")
		{
			text2 = "e3";
		}
		xmlDocument.LoadXml(Resources.Load<TextAsset>("Win32-Xenon/stage/" + Stage + "/" + text2).text);
		foreach (XmlNode childNode in xmlDocument.DocumentElement.ChildNodes)
		{
			if (childNode.Name == "timebonus")
			{
				TimeBonusBase = childNode.Attributes["base"].Value.ToInt();
				TimeBonusRate = childNode.Attributes["rate"].Value.ToInt();
			}
			else if (childNode.Name == "ringbonus")
			{
				RingBonusRate = childNode.Attributes["rate"].Value.ToInt();
			}
			else if (childNode.Name == "rank")
			{
				Rank_S = childNode.Attributes["s"].Value.ToInt();
				Rank_A = childNode.Attributes["a"].Value.ToInt();
				Rank_B = childNode.Attributes["b"].Value.ToInt();
				Rank_C = childNode.Attributes["c"].Value.ToInt();
				Rank_D = childNode.Attributes["d"].Value.ToInt();
			}
		}
		GameManager.PlayerData playerData = Singleton<GameManager>.Instance._PlayerData;
		MinutesRound = Mathf.FloorToInt(playerData.time / 60f);
		SecondsRound = Mathf.FloorToInt(playerData.time - (float)MinutesRound * 60f);
		FractionRound = Mathf.FloorToInt(playerData.time * 1000f % 1000f);
		Counter(playerData.rings.ToString("d3"), RingHud, Numbers);
		Counter(playerData.score.ToString("d8"), ScoreHud, Numbers);
		Counter(MinutesRound.ToString("d2"), MinutesHud, Numbers);
		Counter(SecondsRound.ToString("d2"), SecondsHud, Numbers);
		Counter(FractionRound.ToString("d3"), FractionHud, Numbers);
		MinutesHud[0].enabled = Singleton<Settings>.Instance.settings.DisplayType != 1 || playerData.time > 600f;
		Score = playerData.score;
		TimeRec = playerData.time;
		Rings = playerData.rings;
		TimeBonus = CalcTimeBonus(playerData.time, TimeBonusBase - 10000, TimeBonusRate);
		RingBonus = playerData.rings * RingBonusRate;
		TotalScore = Score + TimeBonus + RingBonus;
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv")
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			string text3 = Stage + "_" + Char;
			if (gameData.StageRecords.Count != 0)
			{
				bool flag = true;
				for (int num7 = 0; num7 < gameData.StageRecords.Count; num7++)
				{
					if (gameData.StageRecords[num7].StageName == text3)
					{
						flag = false;
						StageRecords = gameData.StageRecords[num7];
						StageRecordsIndex = num7;
					}
				}
				if (flag)
				{
					FirstStageSave = true;
					StageRecords = new StageData(text3, 0, 0f, 0, 0, 0);
				}
			}
			else
			{
				FirstStageSave = true;
				StageRecords = new StageData(text3, 0, 0f, 0, 0, 0);
			}
		}
		StartTime = Time.time;
		StateMachine.Initialize(StateEnter);
	}

	private int CountSpeed(float Count)
	{
		int num = 5;
		if (Count >= 1000f)
		{
			num = 55;
		}
		if (Count >= 10000f)
		{
			num = 555;
		}
		if (Count >= 100000f)
		{
			num = 5555;
		}
		if (Count >= 1000000f)
		{
			num = 55555;
		}
		if (Count >= 10000000f)
		{
			num = 555555;
		}
		if (Count >= 100000000f)
		{
			num = 5555555;
		}
		return num * 60;
	}

	private int Rank(int FinalScore)
	{
		if (FinalScore >= Rank_S)
		{
			return 0;
		}
		if (FinalScore >= Rank_A)
		{
			return 1;
		}
		if (FinalScore >= Rank_B)
		{
			return 2;
		}
		if (FinalScore >= Rank_C)
		{
			return 3;
		}
		_ = Rank_D;
		return 4;
	}

	private int CalcTimeBonus(float Time, int Base, int Rate)
	{
		if (Base - Mathf.FloorToInt(Time) * Rate < 0)
		{
			return 0;
		}
		return Base - Mathf.FloorToInt(Time) * Rate;
	}

	private void PlayResultJingle()
	{
		ResultsSource.clip = Jingles[Singleton<Settings>.Instance.settings.E3XBLAMusic];
		ResultsSource.Play();
	}

	private void StateEnterStart()
	{
	}

	private void StateEnter()
	{
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv" && !FirstStageSave && Time.time - StartTime > 7.15f && !DefineBasicRecords)
		{
			bool flag = false;
			if (Score > StageRecords.BestScore)
			{
				NewScore = Score;
				StarAnimators[0].SetTrigger("On Play");
				flag = true;
			}
			if (TimeRec < StageRecords.BestTime)
			{
				NewTimeRec = TimeRec;
				StarAnimators[1].SetTrigger("On Play");
				flag = true;
			}
			if (Rings > StageRecords.BestRings)
			{
				NewRings = Rings;
				StarAnimators[2].SetTrigger("On Play");
				flag = true;
			}
			if (flag)
			{
				Singleton<AudioManager>.Instance.PlayClip(RankSkipSound);
			}
			DefineBasicRecords = true;
		}
		if (Time.time - StartTime > 8f)
		{
			StateMachine.ChangeState(StateA);
		}
	}

	private void StateEnterEnd()
	{
	}

	private void StateAStart()
	{
		CountSoundLoop.Play();
	}

	private void StateA()
	{
		if (TimeBonusCount != (float)TimeBonus && (Singleton<RInput>.Instance.P.GetButtonDown("Start") || Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
		{
			TimeBonusCount = TimeBonus;
		}
		TimeBonusCount = Mathf.Min(TimeBonusCount + (float)CountSpeed(TimeBonusCount) * Time.deltaTime, TimeBonus);
		Counter(((int)Mathf.Round(TimeBonusCount)).ToString("d5"), TimeBonusHud, Numbers);
		if (TimeBonusCount == (float)TimeBonus)
		{
			CountSoundLoop.Stop();
			StateMachine.ChangeState(StateB);
		}
	}

	private void StateAEnd()
	{
	}

	private void StateBStart()
	{
		StartTime = Time.time;
	}

	private void StateB()
	{
		if (!Started && Time.time - StartTime > 0.5f)
		{
			CountSoundLoop.Play();
			Started = true;
		}
		if (Started)
		{
			if (RingBonusCount != (float)RingBonus && (Singleton<RInput>.Instance.P.GetButtonDown("Start") || Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
			{
				RingBonusCount = RingBonus;
			}
			RingBonusCount = Mathf.Min(RingBonusCount + (float)CountSpeed(RingBonusCount) * Time.deltaTime, RingBonus);
			Counter(((int)Mathf.Round(RingBonusCount)).ToString("d5"), RingBonusHud, Numbers);
			if (RingBonusCount == (float)RingBonus)
			{
				CountSoundLoop.Stop();
				StateMachine.ChangeState(StateC);
			}
		}
	}

	private void StateBEnd()
	{
	}

	private void StateCStart()
	{
		StartTime = Time.time;
	}

	private void StateC()
	{
		if (Time.time - StartTime > 0.5f && !TotalScoreAnim)
		{
			Animator.SetTrigger("On Total Score");
			TotalScoreAnim = true;
		}
		if (Time.time - StartTime > 1.5f && !SetScore)
		{
			CountSoundLoop.Play();
			SetScore = true;
		}
		if (!SetScore)
		{
			return;
		}
		if (TotalScoreCount != (float)TotalScore && (Singleton<RInput>.Instance.P.GetButtonDown("Start") || Singleton<RInput>.Instance.P.GetButtonDown("Button A")))
		{
			TotalScoreCount = TotalScore;
		}
		TotalScoreCount = Mathf.Min(TotalScoreCount + (float)CountSpeed(TotalScoreCount) * Time.deltaTime, TotalScore);
		Counter(((int)Mathf.Round(TotalScoreCount)).ToString("d6"), TotalScoreHud, NumbersBig);
		if (TotalScoreCount != (float)TotalScore)
		{
			return;
		}
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv" && !FirstStageSave && !DefineTotalScoreRecord)
		{
			if (TotalScore > StageRecords.BestTotalScore)
			{
				Singleton<AudioManager>.Instance.PlayClip(RankSkipSound);
				NewTotalScore = TotalScore;
				StarAnimators[3].SetTrigger("On Play");
			}
			DefineTotalScoreRecord = true;
		}
		CountSoundLoop.Stop();
		StateMachine.ChangeState(StateQuit);
	}

	private void StateCEnd()
	{
	}

	private void StateQuitStart()
	{
		StartTime = Time.time;
		FinalRank = Rank(TotalScore);
		RankDisplay.sprite = RankSprite[FinalRank];
		RankFXDisplay.sprite = RankSprite[FinalRank];
		NewFinalRank = 5;
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv" && !FirstStageSave && FinalRank < StageRecords.BestRank)
		{
			NewFinalRank = FinalRank;
			StarAnimators[3].SetTrigger("On Play");
		}
	}

	private void StateQuit()
	{
		if (Time.time - StartTime > 0.25f && !EndAnim)
		{
			Animator.SetInteger("Rank", FinalRank);
			Animator.SetTrigger("On End");
			EndAnim = true;
		}
		if (Time.time - StartTime > 2.5f && !Fading)
		{
			PlayRankVoice(FinalRank, Player.PlayerNameShort);
			Fading = true;
		}
		if (!(Time.time - StartTime > 6.25f) || Exited)
		{
			return;
		}
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv")
		{
			string flag = Stage + "_" + Char + "_clear";
			if (!gameData.HasFlag(flag))
			{
				gameData.ActivateFlag(flag);
			}
		}
		if (!gameData.HasFlag(Game.StgSnAllClear) && gameData.HasFlag("wvo_sn_clear") && gameData.HasFlag("dtd_sn_clear") && gameData.HasFlag("wap_sn_clear") && gameData.HasFlag("csc_sn_clear") && gameData.HasFlag("flc_sn_clear") && gameData.HasFlag("rct_sn_clear") && gameData.HasFlag("tpj_sn_clear") && gameData.HasFlag("kdv_sn_clear") && gameData.HasFlag("aqa_sn_clear") && gameData.HasFlag("wvo_tl_clear"))
		{
			gameData.ActivateFlag(Game.StgSnAllClear);
		}
		if (!gameData.HasFlag(Game.StgSdAllClear) && gameData.HasFlag("wap_sd_clear") && gameData.HasFlag("kdv_sd_clear") && gameData.HasFlag("csc_sd_clear") && gameData.HasFlag("flc_sd_clear") && gameData.HasFlag("rct_sd_clear") && gameData.HasFlag("aqa_sd_clear") && gameData.HasFlag("wvo_sd_clear") && gameData.HasFlag("dtd_sd_clear") && gameData.HasFlag("tpj_rg_clear"))
		{
			gameData.ActivateFlag(Game.StgSdAllClear);
		}
		if (!gameData.HasFlag(Game.StgSvAllClear) && gameData.HasFlag("csc_sv_clear") && gameData.HasFlag("tpj_sv_clear") && gameData.HasFlag("dtd_sv_clear") && gameData.HasFlag("wap_sv_clear") && gameData.HasFlag("rct_sv_clear") && gameData.HasFlag("aqa_sv_clear") && gameData.HasFlag("kdv_sv_clear") && gameData.HasFlag("flc_sv_clear") && gameData.HasFlag("wvo_bz_clear"))
		{
			gameData.ActivateFlag(Game.StgSvAllClear);
		}
		string goTo = ((gameData.HasFlag(Game.StgSnAllClear) && gameData.HasFlag(Game.StgSdAllClear) && gameData.HasFlag(Game.StgSvAllClear) && !gameData.HasFlag(Game.CreditsPlayed)) ? Game.CreditsScene : "MainMenu");
		if (FinalRank == 0)
		{
			string flag2 = Stage + "_" + Char;
			if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv")
			{
				if (!gameData.HasFlag(flag2))
				{
					GoTo = Game.GoldMedalScene;
					gameData.ActivateFlag(flag2);
					gameData.TotalGoldMedals++;
				}
				else
				{
					GoTo = goTo;
				}
			}
			else
			{
				GoTo = "MainMenu";
			}
		}
		else
		{
			GoTo = goTo;
		}
		if (!CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv")
		{
			if (!FirstStageSave)
			{
				if (NewScore != 0)
				{
					StageRecords.BestScore = NewScore;
				}
				if (NewTimeRec != 0f)
				{
					StageRecords.BestTime = NewTimeRec;
				}
				if (NewRings != 0)
				{
					StageRecords.BestRings = NewRings;
				}
				if (NewTotalScore != 0)
				{
					StageRecords.BestTotalScore = NewTotalScore;
				}
				if (NewFinalRank != 5)
				{
					StageRecords.BestRank = NewFinalRank;
				}
				gameData.StageRecords[StageRecordsIndex] = StageRecords;
			}
			else
			{
				StageRecords.BestScore = Score;
				StageRecords.BestTime = TimeRec;
				StageRecords.BestRings = Rings;
				StageRecords.BestTotalScore = TotalScore;
				StageRecords.BestRank = FinalRank;
				gameData.StageRecords.Add(StageRecords);
			}
		}
		Singleton<GameManager>.Instance.SetGameData(gameData);
		Singleton<GameManager>.Instance._PlayerData = default(GameManager.PlayerData);
		Singleton<GameManager>.Instance.GameState = GameManager.State.Menu;
		Singleton<GameManager>.Instance.StoredPlayerVars = null;
		Singleton<GameManager>.Instance.LifeItemIDs.Clear();
		Singleton<GameManager>.Instance.PlayedEventLimit = false;
		Singleton<GameManager>.Instance.GoToActSelect = !CurrentStage.Contains("test") && CurrentStage != "kdv_e_sn" && CurrentStage != "csc_f_sv";
		if (GoTo == Game.GoldMedalScene || GoTo == Game.CreditsScene)
		{
			SceneManager.LoadScene(GoTo, LoadSceneMode.Single);
		}
		else
		{
			Singleton<GameManager>.Instance.SetLoadingTo(GoTo, Game.AutoSaveMode);
		}
		Exited = true;
	}

	private void StateQuitEnd()
	{
	}

	private void Update()
	{
		StateMachine.UpdateStateMachine();
		ScoreHud[1].enabled = Score > 9;
		ScoreHud[2].enabled = Score > 99;
		ScoreHud[3].enabled = Score > 999;
		ScoreHud[4].enabled = Score > 9999;
		ScoreHud[5].enabled = Score > 99999;
		ScoreHud[6].enabled = Score > 999999;
		ScoreHud[7].enabled = Score > 9999999;
		RingHud[1].enabled = Singleton<GameManager>.Instance._PlayerData.rings > 9;
		RingHud[2].enabled = Singleton<GameManager>.Instance._PlayerData.rings > 99;
		TimeBonusHud[1].enabled = TimeBonusCount > 9f;
		TimeBonusHud[2].enabled = TimeBonusCount > 99f;
		TimeBonusHud[3].enabled = TimeBonusCount > 999f;
		TimeBonusHud[4].enabled = TimeBonusCount > 9999f;
		RingBonusHud[1].enabled = RingBonusCount > 9f;
		RingBonusHud[2].enabled = RingBonusCount > 99f;
		RingBonusHud[3].enabled = RingBonusCount > 999f;
		RingBonusHud[4].enabled = RingBonusCount > 9999f;
		TotalScoreHud[1].enabled = TotalScoreCount > 9f;
		TotalScoreHud[2].enabled = TotalScoreCount > 99f;
		TotalScoreHud[3].enabled = TotalScoreCount > 999f;
		TotalScoreHud[4].enabled = TotalScoreCount > 9999f;
		TotalScoreHud[5].enabled = TotalScoreCount > 99999f;
	}

	public void PlayRankVoice(int Rank, string PlayerName)
	{
		int num = 12 + Rank;
		string text = "all01_v" + num + "_" + PlayerName;
		Singleton<AudioManager>.Instance.PlayVoiceClip(Resources.Load<AudioClip>("Win32-Xenon/sound/voice/" + Singleton<Settings>.Instance.AudioLanguage() + "/" + text));
	}
}
