using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public enum State
	{
		MainMenu = 0,
		ActTrial = 1,
		GoldMedalResults = 2,
		ExtrasAudio = 3,
		ExtrasGame = 4,
		Options = 5,
		VideoTab = 6,
		GameTab = 7,
		AudioTab = 8,
		DataTab = 9,
		FileSelect = 10,
		UIConfig = 11,
		Load = 12,
		Exit = 13
	}

	[Header("------ Common ------")]
	public StateMachine StateMachine;

	public BackgroundVideo BGVideo;

	public Image[] BGGradients;

	public Image[] BGGradientsAdd;

	public Image[] BGGradientsOther;

	public Animator MainMenuAnimator;

	public RectTransform SelectorTransform;

	public RectTransform SelectorParent;

	public RectTransform ShortSelectorTransform;

	public Animator SelectorAnimator;

	public AudioSource Music;

	public AudioClip[] AudioClip;

	public RectTransform[] Headers;

	public Text CommonText;

	public Animator SaveIcon;

	public Animator FadeScreen;

	private State MenuState;

	private float WaitToLoad;

	private float XAxis;

	private float YAxis;

	private float AxisXTime;

	private float AxisYTime;

	private bool UsingXAxis;

	private bool UsingYAxis;

	private bool StartXScrolling;

	private bool StartYScrolling;

	private bool FastXScroll;

	private bool FastYScroll;

	[Header("------ Main Menu ------")]
	public RectTransform[] MenuTransforms;

	public RectTransform[] SPDropdown;

	public RectTransform[] TSDropdown;

	public RectTransform[] MPTransforms;

	public RectTransform[] TagTransforms;

	public RectTransform[] ExtrasDropdown;

	private int MainMenuCount;

	private int MainMenuSelector;

	private bool IsSinglePlayer;

	private bool IsMultiPlayer;

	private bool IsTag;

	private bool IsTrialSelect;

	private bool IsExtras;

	private bool GoToTitleScreen;

	[Header("------ Act Trial ------")]
	public Animator[] PlayerModels;

	public Renderer[] SonicRenderers;

	public Renderer[] ShadowRenderers;

	public Renderer[] SilverRenderers;

	public Renderer[] LastStoryRenderers;

	public Animation PlayerRendererAnim;

	public GameObject[] PlayerArrowIndicator;

	public Image PlayerNameBG;

	public Text[] PlayerText;

	public RectTransform ActListSonicPanel;

	public RectTransform ActListShadowPanel;

	public RectTransform ActListSilverPanel;

	public RectTransform[] ActListSonic;

	public RectTransform[] ActListShadow;

	public RectTransform[] ActListSilver;

	private bool IsActSelect;

	private bool IsDifficultySelect;

	private bool CharSelected;

	private float CharSelectTimer;

	private int PlayerSelectCount;

	private int PlayerSelector;

	private int ActSelectCount;

	private int ActSelector;

	private int ActMissionSelector;

	private int ActMissionSelectCount;

	[Header("Act Mission")]
	public RectTransform ActMissionDiffs;

	public RectTransform ActMissionPanel;

	public RectTransform[] ActMissionTransforms;

	public Text[] ActMissionDataElements;

	public Image RankIcon;

	public Sprite[] RankSprites;

	[Header("------ Gold Medal Results ------")]
	public Animator GoldMedalPanelScroll;

	public Animator[] GoldMedalStoryAnimators;

	public GameObject[] GoldMedalStoryIndicator;

	public Image GoldMedalAmountIcon;

	public Text GoldMedalAmount;

	public Animator GoldMedalPanelAnimator;

	public RectTransform[] GoldMedalPanelContent;

	private float PanelHeight;

	private int LastIndex;

	private int VerIndex;

	private int MaxVerIndex;

	[Header("------ Audio Room ------")]
	public AudioSource AudioMusic;

	public AudioSource MusicPlayer;

	public AudioClip[] ThemeMusic;

	public AudioClip[] ActMusic;

	public AudioClip[] OtherMusic;

	public AudioClip[] Project06Music;

	public RectTransform AudioRoomOptions;

	public Animator AudioSelectorAnimator;

	public Animator AudioArrowsAnimator;

	public RectTransform[] AudioRoomTransforms;

	public RectTransform AudioRoomPanel;

	public RectTransform AudioListSelectorTransform;

	public Animator AudioListSelectorAnimator;

	public RectTransform[] AudioRoomLists;

	public RectTransform[] ThemeAudioList;

	public RectTransform[] ActAudioList;

	public RectTransform[] BossAudioList;

	public RectTransform[] TownAudioList;

	public RectTransform[] OtherAudioList;

	public RectTransform[] Project06AudioList;

	private int AudioRoomCount;

	private int AudioRoomSelector;

	private int AudioListSelector;

	[Header("------ Game Room ------")]
	public RectTransform GameRoomPanel;

	[Header("Option Tabs")]
	public RectTransform[] GameRoomSwitchTransforms;

	[Header("------------------------------------------------------------------------------------------------------------------------------------------------")]
	[Header("Options")]
	public RectTransform[] OptionTabsTransforms;

	public Image[] OptionTabsHighlights;

	public RectTransform OptionsPanel;

	public RectTransform OptionsArrowTransform;

	public GameObject OptionsArrowSelector;

	public Animator OptionsArrowAnimator;

	public Animator NotificationWarning;

	public Text NotificationText;

	private bool OptionHasChanged;

	private int OptionsCount;

	private int OptionsSelector;

	[Header("Option Tabs")]
	public RectTransform[] OptionTabs;

	public RectTransform[] GraphicsSwitchTransforms;

	public RectTransform[] GameSwitchTransforms;

	public RectTransform[] AudioSliderTransforms;

	public RectTransform[] DataSwitchTransforms;

	[Header("Video Tab")]
	public RectTransform[] VideoSettingsTransforms;

	[Header("Audio Tab")]
	public Slider[] AudioSliders;

	[Header("Data Tab")]
	public Text[] DataInfoTransforms;

	public SaveSlotUI FileSlot;

	public Animator ConfirmPanel;

	public RectTransform[] ConfirmPanelTransforms;

	public RectTransform ConfirmPanelSelector;

	[Header("------ UI Config ------")]
	public RectTransform UIDoubleArrowTransform;

	public RectTransform UIDoubleArrowSeparator;

	public GameObject UIDoubleArrowSelector;

	public Animator[] UIDoubleArrowAnimators;

	public RectTransform[] UIPanels;

	public RectTransform[] UISwitchTransforms;

	public RectTransform[] UIDescriptionTransforms;

	public Image UIPresetBG;

	public Text[] UIText;

	public Image UIBG;

	public Image[] UIDisplayElements;

	public Image[] UITextBoxElements;

	public Image[] UIItemBoxElements;

	public Image[] UIPauseMenuElements;

	public Image[] UIEnemyHealthElements;

	public Image[] UILoadingBGs;

	private int Index;

	private int MaxIndex;

	private Dictionary<string, string> OptionsText;

	private Dictionary<string, string> OptionsText_Alt = new Dictionary<string, string>();

	private string[] ResolutionOptions;

	private string[][] GameRoomOptions_Switch = new string[12][]
	{
		new string[2] { "msg_off", "msg_on" },
		new string[3] { "msg_retail", "msg_e3", "msg_tgs" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[3] { "msg_normal", "msg_adventure", "msg_adventure2" },
		new string[3] { "msg_straight", "msg_curved", "msg_legacy" },
		new string[2] { "msg_original", "msg_custom" },
		new string[2] { "msg_on", "msg_off" },
		new string[2] { "msg_off", "msg_on" },
		new string[1] { "msg_none" },
		new string[1] { "msg_none" },
		new string[1] { "msg_none" }
	};

	private int[] GameRoomSettings = new int[12];

	private string[][] Resolution_Switch;

	private string[][] GraphicsOptions_Switch = new string[19][]
	{
		new string[3] { "msg_windowed", "msg_fullscreen_borderless", "msg_fullscreen" },
		new string[1] { "msg_none" },
		new string[2] { "msg_off", "msg_on" },
		new string[4] { "msg_off", "msg_fxaa", "msg_smaa", "msg_taa" },
		new string[3] { "msg_off", "msg_pertexture", "msg_forcedon" },
		new string[3] { "msg_off", "msg_on", "msg_halfvsync" },
		new string[1] { "msg_none" },
		new string[5] { "msg_verylow", "msg_low", "msg_medium", "msg_high", "msg_veryhigh" },
		new string[4] { "msg_low", "msg_medium", "msg_high", "msg_veryhigh" },
		new string[2] { "msg_off", "msg_on" },
		new string[3] { "msg_off", "msg_radial", "msg_motion" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[3] { "msg_off", "msg_medium", "msg_high" },
		new string[4] { "msg_refoff", "msg_refonlow", "msg_refonmed", "msg_refonhigh" },
		new string[5] { "msg_off", "msg_low", "msg_medium", "msg_high", "msg_veryhigh" },
		new string[2] { "msg_basic", "msg_advanced" },
		new string[5] { "msg_verylow", "msg_low", "msg_medium", "msg_high", "msg_veryhigh" },
		new string[3] { "msg_nocascades", "msg_2cascades", "msg_4cascades" }
	};

	private int[] GraphicsSettings = new int[19];

	private string[][] GameOptions_Switch = new string[13][]
	{
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_hide", "msg_show" },
		new string[2] { "msg_japanese", "msg_english" },
		new string[2] { "msg_xenon", "msg_ps3" },
		new string[12]
		{
			"msg_wvo", "msg_dtd", "msg_wap", "msg_csc", "msg_flc", "msg_rct", "msg_tpj", "msg_kdv", "msg_aqa", "msg_box",
			"msg_e3", "msg_retail"
		},
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" },
		new string[2] { "msg_off", "msg_on" }
	};

	private int[] GameSettings = new int[13];

	private float[] AudioSettings = new float[3];

	private string[][] UIOptions_Switch = new string[7][]
	{
		new string[2] { "msg_retail", "msg_e3" },
		new string[3] { "msg_retail", "msg_e3", "msg_off" },
		new string[2] { "msg_retail", "msg_e3" },
		new string[2] { "msg_retail", "msg_e3" },
		new string[2] { "msg_retail", "msg_e3" },
		new string[2] { "msg_retail", "msg_e3" },
		new string[3] { "msg_retail", "msg_e3", "msg_custom" }
	};

	private int[] UISettings = new int[7];

	private bool SwitchButtonWin;

	private bool IsAudioList;

	private bool IsPlayingAudio;

	private int InitialIndex;

	private bool AdvancedSettings;

	private bool ConfirmReset;

	private float ConfirmPanelTime;

	private int ConfirmIndex;

	private int ConfirmState;

	private bool LoadCredits;

	private bool ExitSaveData;

	private void Start()
	{
		GrabSettings();
		MainMenuCount = 4;
		PlayerSelectCount = 3;
		OptionsCount = 3;
		BGGradients[0].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear);
		BGGradients[1].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white);
		BGGradientsAdd[0].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear);
		BGGradientsAdd[1].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white);
		BGGradientsOther[0].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear);
		BGGradientsOther[1].color = ((Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white);
		if (Singleton<GameManager>.Instance.GoToActSelect)
		{
			StateActTrialStart();
			StateMachine.Initialize(StateActTrial);
		}
		else
		{
			StateMainMenuStart();
			StateMachine.Initialize(StateMainMenu);
		}
		Singleton<GameManager>.Instance.LifeItemIDs.Clear();
	}

	private void Update()
	{
		StateMachine.UpdateStateMachine();
		UpdateMeshes();
		UpdateUI();
		UpdateMusic();
		XAxis = Singleton<RInput>.Instance.P.GetAxis("Left Stick X") + Singleton<RInput>.Instance.P.GetAxis("D-Pad X");
		YAxis = 0f - Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") + (0f - Singleton<RInput>.Instance.P.GetAxis("D-Pad Y"));
		if (XAxis == 0f)
		{
			UsingXAxis = false;
			StartXScrolling = false;
			FastXScroll = false;
			AxisXTime = Time.time;
		}
		else if (Time.time - AxisXTime > ((!FastXScroll) ? 0.5f : 0.125f) && !StartXScrolling)
		{
			FastXScroll = true;
			StartXScrolling = true;
			AxisXTime = Time.time;
			UsingXAxis = false;
			StartXScrolling = false;
		}
		if (YAxis == 0f)
		{
			UsingYAxis = false;
			StartYScrolling = false;
			FastYScroll = false;
			AxisYTime = Time.time;
		}
		else if (Time.time - AxisYTime > ((!FastYScroll) ? 0.5f : 0.125f) && !StartYScrolling)
		{
			FastYScroll = true;
			StartYScrolling = true;
			AxisYTime = Time.time;
			UsingYAxis = false;
			StartYScrolling = false;
		}
	}

	private void StateMainMenuStart()
	{
		MenuState = State.MainMenu;
		IsSinglePlayer = false;
		IsTrialSelect = false;
		IsExtras = false;
		MenuAnimTrigger("On Title Bar");
	}

	private void StateMainMenu()
	{
		DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref MainMenuSelector, ref MainMenuCount);
		if (!IsSinglePlayer && !IsMultiPlayer && !IsExtras)
		{
			MainMenuCount = 4;
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				if (MainMenuSelector == 0)
				{
					DoSelector(1);
					IsSinglePlayer = true;
					MainMenuSelector = 0;
				}
				else if (MainMenuSelector == 1)
				{
					PlayAudio(2);
				}
				else if (MainMenuSelector == 2)
				{
					DoSelector(1);
					IsExtras = true;
					MainMenuSelector = 0;
				}
				else if (MainMenuSelector == 3)
				{
					PlayAudio(1);
					MenuAnimTrigger("On Title Bar");
					StateMachine.ChangeState(StateOptions);
				}
				else if (MainMenuSelector == 4)
				{
					PlayAudio(1);
					StateMachine.ChangeState(StateExit);
				}
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3);
				GoToTitleScreen = true;
				StateMachine.ChangeState(StateExit);
			}
			if (MainMenuSelector == 0)
			{
				CommonText.text = "Single Player: For one player";
			}
			else if (MainMenuSelector == 1 || MainMenuSelector == 2)
			{
				CommonText.text = ((MainMenuSelector == 1) ? "Multiplayer: Two player mode" : "Extras: Listen to music and view event movies");
			}
			else if (MainMenuSelector == 3)
			{
				CommonText.text = "Options: Adjust various game settings";
			}
			else if (MainMenuSelector == 4)
			{
				CommonText.text = "Exit: Quit the game";
			}
		}
		else if (IsSinglePlayer)
		{
			if (!IsTrialSelect)
			{
				MainMenuCount = 2;
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					if (MainMenuSelector == 0)
					{
						PlayAudio(2);
					}
					else if (MainMenuSelector == 1)
					{
						DoSelector(1);
						IsTrialSelect = true;
						MainMenuSelector = 0;
					}
					else if (MainMenuSelector == 2)
					{
						DoSelector(1);
						MainMenuSelector = 0;
						StateMachine.ChangeState(StateGoldMedalResults);
					}
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					DoSelector(3);
					IsSinglePlayer = false;
					MainMenuSelector = 0;
				}
				CommonText.text = ((MainMenuSelector == 0) ? "Episode Select: Play through the storyline" : ((MainMenuSelector == 1) ? "Trial Select: Play stages you have cleared" : "Gold Medal Results: Displays list of Gold Medals collected"));
				return;
			}
			MainMenuCount = 1;
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				if (MainMenuSelector == 0)
				{
					DoSelector(1);
					MainMenuSelector = 0;
					PlayerSelector = 0;
					StateMachine.ChangeState(StateActTrial);
				}
				else if (MainMenuSelector == 1)
				{
					PlayAudio(2);
				}
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3);
				IsTrialSelect = false;
				MainMenuSelector = 1;
			}
			CommonText.text = ((MainMenuSelector == 0) ? "ACT Trial: Choose a Stage and play!" : "Town Trial: Choose a Town Mission and play!");
		}
		else if (IsMultiPlayer)
		{
			MainMenuCount = 1;
			if (!IsTag)
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					if (MainMenuSelector == 0)
					{
						DoSelector(1);
						IsTag = true;
					}
					else
					{
						PlayAudio(2);
					}
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					DoSelector(3);
					IsMultiPlayer = false;
					MainMenuSelector = 1;
				}
				if (MainMenuSelector == 0)
				{
					CommonText.text = "Tag: Work together and head to the goal!";
				}
				else if (MainMenuSelector == 1)
				{
					CommonText.text = "Battle: Reach the goal first!";
				}
			}
			else
			{
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					PlayAudio(2);
				}
				else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					DoSelector(3);
					IsTag = false;
					MainMenuSelector = 0;
				}
				if (MainMenuSelector == 0)
				{
					CommonText.text = "Tag Story: A special two-player story";
				}
				else if (MainMenuSelector == 1)
				{
					CommonText.text = "Tag Trial: Challenge previously-cleared missions with two players!";
				}
			}
		}
		else
		{
			if (!IsExtras)
			{
				return;
			}
			MainMenuCount = 2;
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				if (MainMenuSelector == 0)
				{
					PlayAudio(1);
					StateMachine.ChangeState(StateExtrasAudio);
				}
				else if (MainMenuSelector == 1)
				{
					PlayAudio(2);
				}
				else if (MainMenuSelector == 2)
				{
					PlayAudio(1);
					StateMachine.ChangeState(StateExtrasGame);
				}
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3);
				IsExtras = false;
				MainMenuSelector = 2;
			}
			if (MainMenuSelector == 0)
			{
				CommonText.text = "Audio Room: You can listen to different sounds";
			}
			else if (MainMenuSelector == 1)
			{
				CommonText.text = "Theater Room: View story events";
			}
			else if (MainMenuSelector == 2)
			{
				CommonText.text = "Game Room: Access various extra gameplay features";
			}
		}
	}

	private void StateMainMenuEnd()
	{
	}

	private void StateActTrialStart()
	{
		MenuState = State.ActTrial;
		if (Singleton<GameManager>.Instance.GoToActSelect)
		{
			PlayerSelector = (int)Singleton<GameManager>.Instance.GameStory;
			switch (PlayerSelector)
			{
			case 0:
				ActSelectCount = ActListSonic.Length - 1;
				break;
			case 1:
				ActSelectCount = ActListShadow.Length - 1;
				break;
			case 2:
				ActSelectCount = ActListSilver.Length - 1;
				break;
			}
			IsActSelect = true;
			IsDifficultySelect = false;
			CharSelected = false;
			ActSelector = Singleton<GameManager>.Instance.ActSelectLastIndex;
			Singleton<GameManager>.Instance.GoToActSelect = false;
		}
		else
		{
			IsActSelect = false;
			IsDifficultySelect = false;
			CharSelected = false;
		}
		ActMissionSelectCount = 0;
		MenuAnimTrigger("On Title Bar");
	}

	private void StateActTrial()
	{
		if (!IsActSelect)
		{
			if (PlayerSelector == 0 || PlayerSelector == 1 || PlayerSelector == 2)
			{
				if (PlayerSelector == 0)
				{
					CommonText.text = "Play Sonic the Hedgehog Missions";
				}
				else if (PlayerSelector == 1)
				{
					CommonText.text = "Play Shadow the Hedgehog Missions";
				}
				else if (PlayerSelector == 2)
				{
					CommonText.text = "Play Silver the Hedgehog Missions";
				}
				if (!CharSelected && (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X")))
				{
					switch (PlayerSelector)
					{
					case 0:
						ActSelectCount = ActListSonic.Length - 1;
						PlayVoiceAudio("sn");
						Singleton<GameManager>.Instance.SetGameStory("Sonic");
						break;
					case 1:
						ActSelectCount = ActListShadow.Length - 1;
						PlayVoiceAudio("sd");
						Singleton<GameManager>.Instance.SetGameStory("Shadow");
						break;
					case 2:
						ActSelectCount = ActListSilver.Length - 1;
						PlayVoiceAudio("sv");
						Singleton<GameManager>.Instance.SetGameStory("Silver");
						break;
					}
					PlayAudio(1);
					PlayerRendererAnim.Play();
					CharSelected = true;
					CharSelectTimer = Time.time;
				}
			}
			else
			{
				CommonText.text = "Play the Last Episode";
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
				{
					PlayAudio(2);
				}
			}
			if (CharSelected)
			{
				if (Time.time - CharSelectTimer > 1.8f)
				{
					ActSelector = 0;
					IsActSelect = true;
				}
				return;
			}
			DoOptions(ref XAxis, ref UsingXAxis, UseSelector: false, ref PlayerSelector, ref PlayerSelectCount);
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				PlayAudio(3);
				StateMachine.ChangeState(StateMainMenu);
				IsSinglePlayer = true;
				IsTrialSelect = true;
				MainMenuSelector = 0;
			}
			return;
		}
		CommonText.text = ((!IsDifficultySelect) ? "Please select a Stage" : "Please select a Mission");
		if (!IsDifficultySelect)
		{
			DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref ActSelector, ref ActSelectCount);
			bool flag = (PlayerSelector == 0 && ActSelector < 10) || (PlayerSelector == 1 && ActSelector < 9) || (PlayerSelector == 2 && ActSelector < 9);
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				if (flag)
				{
					IsDifficultySelect = true;
					DoSelector(1, UseAudio: true, "Audio Selector");
					string[] array = new string[0];
					switch (PlayerSelector)
					{
					case 0:
						array = ActListSonic[ActSelector].gameObject.name.Split('/');
						break;
					case 1:
						array = ActListShadow[ActSelector].gameObject.name.Split('/');
						break;
					case 2:
						array = ActListSilver[ActSelector].gameObject.name.Split('/');
						break;
					}
					ActMissionDataElements[0].text = array[1];
					GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
					if (gameData.StageRecords.Count != 0)
					{
						bool flag2 = true;
						for (int i = 0; i < gameData.StageRecords.Count; i++)
						{
							if (gameData.StageRecords[i].StageName == array[0])
							{
								flag2 = false;
								SetActMissionInfo(Reset: false, gameData.StageRecords[i].BestScore, gameData.StageRecords[i].BestTime, gameData.StageRecords[i].BestRings, gameData.StageRecords[i].BestTotalScore, gameData.StageRecords[i].BestRank);
							}
						}
						if (flag2)
						{
							SetActMissionInfo(Reset: true);
						}
					}
					else
					{
						SetActMissionInfo(Reset: true);
					}
				}
				else
				{
					PlayAudio(2);
				}
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				PlayAudio(3);
				CharSelected = false;
				IsActSelect = false;
			}
		}
		else if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
		{
			PlayAudio(1);
			Singleton<GameManager>.Instance.ActSelectLastIndex = ActSelector;
			StateMachine.ChangeState(StateLoad);
		}
		else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3);
			IsDifficultySelect = false;
		}
	}

	private void StateActTrialEnd()
	{
	}

	private void StateGoldMedalResultsStart()
	{
		MenuState = State.GoldMedalResults;
		Index = 0;
		VerIndex = 0;
		LastIndex = Index;
		MaxIndex = GoldMedalStoryAnimators.Length - 1;
		MaxVerIndex = 12;
		MenuAnimTrigger("On Button B");
		GoldMedalStoryAnimators[Index].SetTrigger("On Change Left");
		GoldMedalPanelAnimator.SetTrigger("On Appear");
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		GoldMedalAmount.text = gameData.TotalGoldMedals.ToString();
		MenuAnimTrigger("On Title Bar");
	}

	private void StateGoldMedalResults()
	{
		if (LastIndex != Index)
		{
			GoldMedalStoryAnimators[LastIndex].SetTrigger((XAxis > 0f) ? "On Hide Away Left" : "On Hide Away Right");
			VerIndex = 0;
			LastIndex = Index;
		}
		switch (Index)
		{
		case 0:
			MaxVerIndex = 13;
			break;
		case 1:
			MaxVerIndex = 12;
			break;
		case 2:
			MaxVerIndex = 12;
			break;
		case 3:
			MaxVerIndex = 0;
			break;
		}
		DoOptions(ref XAxis, ref UsingXAxis, UseSelector: true, ref Index, ref MaxIndex, "Gold Medal Names", 4);
		DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref VerIndex, ref MaxVerIndex, "Gold Medal Scroll");
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3);
			MenuAnimTrigger("On Button Main");
			GoldMedalStoryAnimators[Index].SetTrigger("On Hide Away Left");
			GoldMedalPanelAnimator.SetTrigger("On Hide Away");
			StateMachine.ChangeState(StateMainMenu);
			IsSinglePlayer = true;
			MainMenuSelector = 2;
		}
		if (Index == 0)
		{
			CommonText.text = "Sonic the Hedgehog's Gold Medal list";
		}
		else if (Index == 1)
		{
			CommonText.text = "Shadow the Hedgehog's Gold Medal list";
		}
		else if (Index == 2)
		{
			CommonText.text = "Silver the Hedgehog's Gold Medal list";
		}
		else if (Index == 3)
		{
			CommonText.text = "Gold Medal list for the Last Episode";
		}
	}

	private void StateGoldMedalResultsEnd()
	{
	}

	private void StateExtrasAudioStart()
	{
		MenuState = State.ExtrasAudio;
		AudioRoomSelector = 0;
		MaxIndex = AudioRoomTransforms.Length - 1;
		IsAudioList = false;
		IsPlayingAudio = false;
		MainMenuAnimator.SetTrigger("On Audio Room Enter");
		MenuAnimTrigger("On Title Bar");
	}

	private void StateExtrasAudio()
	{
		if (!IsAudioList)
		{
			DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref AudioRoomSelector, ref MaxIndex, "Audio Selector");
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				DoSelector(1, UseAudio: true, "Audio Selector", "EnterOptions");
				DoSelector(0, UseAudio: false, "Audio List Selector");
				switch (AudioRoomSelector)
				{
				case 0:
					MaxIndex = ThemeAudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[0], worldPositionStays: true);
					break;
				case 1:
					MaxIndex = ActAudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[1], worldPositionStays: true);
					break;
				case 2:
					MaxIndex = BossAudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[2], worldPositionStays: true);
					break;
				case 3:
					MaxIndex = TownAudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[3], worldPositionStays: true);
					break;
				case 4:
					MaxIndex = OtherAudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[4], worldPositionStays: true);
					break;
				case 5:
					MaxIndex = Project06AudioList.Length - 1;
					AudioListSelectorTransform.SetParent(AudioRoomLists[5], worldPositionStays: true);
					break;
				}
				AudioListSelectorTransform.SetSiblingIndex(0);
				IsAudioList = true;
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3);
				IsAudioList = false;
				StateMachine.ChangeState(StateMainMenu);
				IsExtras = true;
				MainMenuSelector = 0;
			}
		}
		else if (!IsPlayingAudio)
		{
			DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref AudioListSelector, ref MaxIndex, "Audio List Selector");
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				if (AudioRoomSelector == 2 || AudioRoomSelector == 3 || (AudioRoomSelector == 4 && AudioListSelector == 1))
				{
					PlayAudio(2);
				}
				else
				{
					DoSelector(0, UseAudio: false, "Audio List Selector", "EnterPlaying");
					switch (AudioRoomSelector)
					{
					case 0:
						MusicPlayer.clip = ThemeMusic[AudioListSelector];
						break;
					case 1:
						MusicPlayer.clip = ActMusic[AudioListSelector];
						break;
					case 4:
						MusicPlayer.clip = OtherMusic[AudioListSelector];
						break;
					case 5:
						MusicPlayer.clip = Project06Music[AudioListSelector];
						break;
					}
					MusicPlayer.Play();
					IsPlayingAudio = true;
				}
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3, UseAudio: true, "Audio Selector", "ExitOptions");
				DoSelector(0, UseAudio: false, "Audio List Selector", "ExitOptions");
				AudioListSelectorTransform.SetParent(AudioRoomPanel, worldPositionStays: true);
				AudioListSelectorTransform.SetSiblingIndex(1);
				MaxIndex = AudioRoomTransforms.Length - 1;
				AudioListSelector = 0;
				IsAudioList = false;
			}
		}
		else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(0, UseAudio: false, "Audio List Selector", "ExitPlaying");
			MusicPlayer.Stop();
			IsPlayingAudio = false;
		}
		CommonText.text = ((IsAudioList && IsPlayingAudio) ? "NOW PLAYING" : "Select a song to play");
	}

	private void StateExtrasAudioEnd()
	{
		MainMenuAnimator.SetTrigger("On Audio Room Exit");
	}

	private void StateExtrasGameStart()
	{
		MenuState = State.ExtrasGame;
		Index = 0;
		MaxIndex = GameRoomOptions_Switch.Length - 1;
		OptionHasChanged = false;
		UpdateSwitcher(GameRoomSwitchTransforms, GameRoomOptions_Switch, GameRoomSettings);
		MenuAnimTrigger("On Button B");
		MenuAnimTrigger("On Title Bar");
	}

	private void StateExtrasGame()
	{
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(0, UseAudio: true, "Arrow");
			if (YAxis < 0f)
			{
				if (Index > 0)
				{
					Index--;
				}
				else
				{
					Index = MaxIndex;
				}
			}
			if (YAxis > 0f)
			{
				if (Index < MaxIndex)
				{
					Index++;
				}
				else
				{
					Index = 0;
				}
			}
		}
		if (!UsingXAxis && XAxis != 0f && Index < GameRoomSettings.Length && Index < 9)
		{
			UsingXAxis = true;
			OptionHasChanged = true;
			PlayAudio(4);
			if (XAxis < 0f)
			{
				if (GameRoomSettings[Index] > 0)
				{
					GameRoomSettings[Index]--;
				}
				else
				{
					GameRoomSettings[Index] = GameRoomOptions_Switch[Index].Length - 1;
				}
			}
			if (XAxis > 0f)
			{
				if (GameRoomSettings[Index] < GameRoomOptions_Switch[Index].Length - 1)
				{
					GameRoomSettings[Index]++;
				}
				else
				{
					GameRoomSettings[Index] = 0;
				}
			}
		}
		UpdateSwitcher(GameRoomSwitchTransforms, GameRoomOptions_Switch, GameRoomSettings);
		if (Index > 8)
		{
			if (!SwitchButtonWin)
			{
				MenuAnimTrigger("On Button Main");
				SwitchButtonWin = true;
			}
		}
		else if (SwitchButtonWin)
		{
			MenuAnimTrigger("On Button B");
			SwitchButtonWin = false;
		}
		if (Index == 0)
		{
			CommonText.text = "Enables animations seen in the Tokyo Game Show demo";
		}
		else if (Index == 1)
		{
			if (GameRoomSettings[Index] == 0)
			{
				CommonText.text = "Retail type: normal distance and speed";
			}
			else if (GameRoomSettings[Index] == 1)
			{
				CommonText.text = "E3 type: mildly shorter distance and slower speed";
			}
			else
			{
				CommonText.text = "TGS type: short distance, lower height, and slower speed";
			}
		}
		else if (Index == 2)
		{
			CommonText.text = "Enable a Lock-On reticle for the homing attack";
		}
		else if (Index == 3)
		{
			CommonText.text = "Enable Lock-On reticles for a few character attacks";
		}
		else if (Index == 4)
		{
			CommonText.text = "Set a type of effect for (most of) the spin animations";
		}
		else if (Index == 5)
		{
			CommonText.text = "Set the type of jumpdash used in gameplay";
		}
		else if (Index == 6)
		{
			if (GameRoomSettings[Index] == 0)
			{
				CommonText.text = "Original type: gem shoes used in the original game";
			}
			else
			{
				CommonText.text = "Custom type: alternate variation that uses Sonic's normal shoes";
			}
		}
		else if (Index == 7)
		{
			CommonText.text = "Toggle upgrade models that characters wear";
		}
		else if (Index == 8)
		{
			CommonText.text = "Use E3/XBLA jingles and music in Kingdom Valley and Crisis City";
		}
		else if (Index == 9)
		{
			CommonText.text = "Customize the gameplay UI with different options";
		}
		else if (Index == 10)
		{
			CommonText.text = "Play the test stage";
		}
		else if (Index == 11)
		{
			CommonText.text = "Roll the credits";
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
		{
			if ((Index == 9 || Index == 10) && OptionHasChanged)
			{
				SetGameRoomSettings();
				Singleton<Settings>.Instance.SaveSettings();
				NotificationText.text = "Game Room settings have been saved";
				NotificationWarning.SetTrigger("OnSettingsApply");
				PlayAudio(5);
			}
			if (Index == 9)
			{
				DoSelector(1, UseAudio: true, "Arrow");
				StateMachine.ChangeState(StateUIConfig);
			}
			if (Index == 10)
			{
				Singleton<GameManager>.Instance.SetGameStory("Sonic");
				ActSelector = 1000;
				PlayAudio(1);
				StateMachine.ChangeState(StateLoad);
			}
			if (Index == 11)
			{
				PlayAudio(1);
				LoadCredits = true;
				StateMachine.ChangeState(StateLoad);
			}
		}
		else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3);
			StateMachine.ChangeState(StateMainMenu);
			IsExtras = true;
			MainMenuSelector = 2;
			if (OptionHasChanged)
			{
				SetGameRoomSettings();
				Singleton<Settings>.Instance.SaveSettings();
				NotificationText.text = "Game Room settings have been saved";
				NotificationWarning.SetTrigger("OnSettingsApply");
				PlayAudio(5);
			}
		}
	}

	private void StateExtrasGameEnd()
	{
		if (OptionHasChanged)
		{
			SetGameRoomSettings();
			Singleton<Settings>.Instance.SaveSettings();
		}
		if (Index < 9)
		{
			MenuAnimTrigger("On Button Main");
		}
		SwitchButtonWin = false;
	}

	private void StateOptionsStart()
	{
		MenuState = State.Options;
		OptionsSelector = 0;
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		FileSlot.SetBanner(gameData.Banner);
	}

	private void StateOptions()
	{
		DoOptions(ref YAxis, ref UsingYAxis, UseSelector: true, ref OptionsSelector, ref OptionsCount, "Arrow");
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
		{
			DoSelector(1, UseAudio: true, "Arrow");
			OptionHasChanged = false;
			if (OptionsSelector == 0)
			{
				StateMachine.ChangeState(StateVideoTab);
			}
			else if (OptionsSelector == 1)
			{
				StateMachine.ChangeState(StateGameTab);
			}
			else if (OptionsSelector == 2)
			{
				StateMachine.ChangeState(StateAudioTab);
			}
			else if (OptionsSelector == 3)
			{
				StateMachine.ChangeState(StateDataTab);
			}
		}
		else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3);
			StateMachine.ChangeState(StateMainMenu);
		}
		if (OptionsSelector == 0)
		{
			CommonText.text = "Adjust video settings";
			UpdateVideoSwitchers(GraphicsSwitchTransforms, GraphicsOptions_Switch, GraphicsSettings);
		}
		else if (OptionsSelector == 1)
		{
			CommonText.text = "Adjust game settings";
			UpdateSwitcher(GameSwitchTransforms, GameOptions_Switch, GameSettings);
		}
		else if (OptionsSelector == 2)
		{
			CommonText.text = "Adjust audio settings";
			UpdateAudioSliders(AudioSliderTransforms, AudioSettings);
		}
		else if (OptionsSelector == 3)
		{
			CommonText.text = "Adjust data settings";
			UpdateDataInfo();
		}
	}

	private void StateOptionsEnd()
	{
	}

	private void StateVideoTabStart()
	{
		MenuState = State.VideoTab;
		AdvancedSettings = false;
		InitialIndex = 0;
		Index = 0;
		MaxIndex = 6;
		UpdateVideoSwitchers(GraphicsSwitchTransforms, GraphicsOptions_Switch, GraphicsSettings);
		MenuAnimTrigger("On Button B");
	}

	private void StateVideoTab()
	{
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			if (!AdvancedSettings)
			{
				DoSelector(3, UseAudio: true, "Arrow");
				StateMachine.ChangeState(StateOptions);
				OptionsSelector = 0;
				AdvancedSettings = false;
				if (OptionHasChanged)
				{
					SetVideoSettings();
					Singleton<Settings>.Instance.SaveSettings();
					Singleton<Settings>.Instance.SetGlobalSettings();
					NotificationText.text = "Video settings have been saved";
					NotificationWarning.SetTrigger("OnSettingsApply");
					PlayAudio(5);
				}
			}
			else
			{
				Index = 0;
				DoSelector(3, UseAudio: true, "Arrow");
				AdvancedSettings = false;
			}
		}
		if (!AdvancedSettings && Index == 6 && (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X")))
		{
			Index = 7;
			DoSelector(1, UseAudio: true, "Arrow");
			AdvancedSettings = true;
		}
		MaxIndex = ((!AdvancedSettings) ? 6 : (GraphicsOptions_Switch.Length - 1));
		InitialIndex = (AdvancedSettings ? 7 : 0);
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(0, UseAudio: true, "Arrow");
			if (YAxis < 0f)
			{
				if (Index > InitialIndex)
				{
					Index--;
				}
				else
				{
					Index = MaxIndex;
				}
			}
			if (YAxis > 0f)
			{
				if (Index < MaxIndex)
				{
					Index++;
				}
				else
				{
					Index = InitialIndex;
				}
			}
		}
		if (!UsingXAxis && XAxis != 0f && Index < GraphicsSettings.Length && Index != 6)
		{
			UsingXAxis = true;
			OptionHasChanged = true;
			PlayAudio(4);
			if (XAxis < 0f)
			{
				if (GraphicsSettings[Index] > 0)
				{
					GraphicsSettings[Index]--;
				}
				else if (Index != 1)
				{
					GraphicsSettings[Index] = GraphicsOptions_Switch[Index].Length - 1;
				}
				else
				{
					GraphicsSettings[Index] = Resolution_Switch[0].Length - 1;
				}
			}
			if (XAxis > 0f)
			{
				if (Index != 1)
				{
					if (GraphicsSettings[Index] < GraphicsOptions_Switch[Index].Length - 1)
					{
						GraphicsSettings[Index]++;
					}
					else
					{
						GraphicsSettings[Index] = 0;
					}
				}
				else if (GraphicsSettings[Index] < Resolution_Switch[0].Length - 1)
				{
					GraphicsSettings[Index]++;
				}
				else
				{
					GraphicsSettings[Index] = 0;
				}
			}
		}
		UpdateVideoSwitchers(GraphicsSwitchTransforms, GraphicsOptions_Switch, GraphicsSettings);
		if (Index == 6)
		{
			if (!SwitchButtonWin)
			{
				MenuAnimTrigger("On Button Main");
				SwitchButtonWin = true;
			}
		}
		else if (SwitchButtonWin)
		{
			MenuAnimTrigger("On Button B");
			SwitchButtonWin = false;
		}
		if (Index == 0)
		{
			CommonText.text = "Set the game's display mode";
		}
		else if (Index == 1)
		{
			CommonText.text = "Set the game's resolution";
		}
		else if (Index == 2)
		{
			CommonText.text = "Allows the game to run in the background while out of focus";
		}
		else if (Index == 3)
		{
			CommonText.text = "Enables FXAA, SMAA or TAA, used to smooth jagged edges";
		}
		else if (Index == 4)
		{
			CommonText.text = "Enhances quality of textures at oblique viewing angles";
		}
		else if (Index == 5)
		{
			CommonText.text = "Locks framerate to monitor's full/half refresh rate";
		}
		else if (Index == 6)
		{
			CommonText.text = "Advanced video settings";
		}
		else if (Index == 7)
		{
			CommonText.text = "Distance in which the camera renders the world";
		}
		else if (Index == 8)
		{
			CommonText.text = "Quality of textures in-game";
		}
		else if (Index == 9)
		{
			CommonText.text = "Displays water, mist, heat and fire effects on the camera lens";
		}
		else if (Index == 10)
		{
			if (GraphicsSettings[Index] == 0)
			{
				CommonText.text = "Choose between a type of Blur Effect: Radial or Motion";
			}
			else if (GraphicsSettings[Index] == 1)
			{
				CommonText.text = "Radial: Blurs corners of the screen as the player goes faster";
			}
			else
			{
				CommonText.text = "Motion: Motion blur caused by object/camera movement";
			}
		}
		else if (Index == 11)
		{
			CommonText.text = "Makes bright lights appear to leak into surrounding scenery";
		}
		else if (Index == 12)
		{
			CommonText.text = "Renders soft outlines on edges of glowing objects and characters";
		}
		else if (Index == 13)
		{
			CommonText.text = "Renders beams of light shining through the environment";
		}
		else if (Index == 14)
		{
			CommonText.text = "Displays reflections on various surfaces";
		}
		else if (Index == 15)
		{
			CommonText.text = "Quality/Resolution of shadows";
		}
		else if (Index == 16)
		{
			if (GraphicsSettings[Index] == 0)
			{
				CommonText.text = "Uses more basic sharp-edged shadows";
			}
			else if (GraphicsSettings[Index] == 1)
			{
				CommonText.text = "Uses Next-Gen Soft-Shadows to display more detailed shadows";
			}
		}
		else if (Index == 17)
		{
			CommonText.text = "Rendering distance of shadows";
		}
		else if (Index == 18)
		{
			CommonText.text = "Renders additional textures for shadows";
		}
	}

	private void StateVideoTabEnd()
	{
		if (OptionHasChanged)
		{
			SetVideoSettings();
			Singleton<Settings>.Instance.SaveSettings();
			Singleton<Settings>.Instance.SetGlobalSettings();
		}
		if (Index != 6)
		{
			MenuAnimTrigger("On Button Main");
		}
		SwitchButtonWin = false;
	}

	private void StateGameTabStart()
	{
		MenuState = State.GameTab;
		Index = 0;
		MaxIndex = GameOptions_Switch.Length - 1;
		UpdateSwitcher(GameSwitchTransforms, GameOptions_Switch, GameSettings);
		MenuAnimTrigger("On Button B");
	}

	private void StateGameTab()
	{
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3, UseAudio: true, "Arrow");
			StateMachine.ChangeState(StateOptions);
			OptionsSelector = 1;
			if (OptionHasChanged)
			{
				SetGameSettings();
				Singleton<Settings>.Instance.SaveSettings();
				NotificationText.text = "Game settings have been saved";
				NotificationWarning.SetTrigger("OnSettingsApply");
				PlayAudio(5);
			}
		}
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(0, UseAudio: true, "Arrow");
			if (YAxis < 0f)
			{
				if (Index > 0)
				{
					Index--;
				}
				else
				{
					Index = MaxIndex;
				}
			}
			if (YAxis > 0f)
			{
				if (Index < MaxIndex)
				{
					Index++;
				}
				else
				{
					Index = 0;
				}
			}
		}
		if (!UsingXAxis && XAxis != 0f && Index < GameSettings.Length)
		{
			UsingXAxis = true;
			OptionHasChanged = true;
			PlayAudio(4);
			if (XAxis < 0f)
			{
				if (GameSettings[Index] > 0)
				{
					GameSettings[Index]--;
				}
				else
				{
					GameSettings[Index] = GameOptions_Switch[Index].Length - 1;
				}
			}
			if (XAxis > 0f)
			{
				if (GameSettings[Index] < GameOptions_Switch[Index].Length - 1)
				{
					GameSettings[Index]++;
				}
				else
				{
					GameSettings[Index] = 0;
				}
			}
		}
		UpdateSwitcher(GameSwitchTransforms, GameOptions_Switch, GameSettings);
		if (Index == 0)
		{
			CommonText.text = "Invert the X rotation axis of the camera";
		}
		else if (Index == 1)
		{
			CommonText.text = "Invert the Y rotation axis of the camera";
		}
		else if (Index == 2)
		{
			CommonText.text = "Invert the Y steering axis of Shadow's glider";
		}
		else if (Index == 3)
		{
			CommonText.text = "Play dialogue from characters in stages";
		}
		else if (Index == 4)
		{
			CommonText.text = "Play non-mandatory in-game cutscenes in stages";
		}
		else if (Index == 5)
		{
			CommonText.text = "Disable non-important camera event volumes in stages";
		}
		else if (Index == 6)
		{
			CommonText.text = "Hint rings that help players on stages";
		}
		else if (Index == 7)
		{
			CommonText.text = "Language of the voice-overs";
		}
		else if (Index == 8)
		{
			CommonText.text = "Icons that display controller buttons";
		}
		else if (Index == 9)
		{
			CommonText.text = "Background video displayed on the title screen and main menu";
		}
		else if (Index == 10)
		{
			CommonText.text = "Enable tilting motion on characters when turning";
		}
		else if (Index == 11)
		{
			CommonText.text = "Enable camera leaning when steering, similar to modern games";
		}
		else if (Index == 12)
		{
			CommonText.text = "Enable jiggle bones that react to physics on characters";
		}
	}

	private void StateGameTabEnd()
	{
		if (OptionHasChanged)
		{
			SetGameSettings();
			Singleton<Settings>.Instance.SaveSettings();
		}
		MenuAnimTrigger("On Button Main");
		SwitchButtonWin = false;
	}

	private void StateAudioTabStart()
	{
		MenuState = State.AudioTab;
		Index = 0;
		MaxIndex = AudioSliderTransforms.Length - 1;
		UpdateAudioSliders(AudioSliderTransforms, AudioSettings);
		MenuAnimTrigger("On Button B");
	}

	private void StateAudioTab()
	{
		for (int i = 0; i < AudioSettings.Length; i++)
		{
			AudioSettings[i] = Mathf.Clamp(AudioSettings[i], 0.001f, 1f);
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3, UseAudio: true, "Arrow");
			StateMachine.ChangeState(StateOptions);
			OptionsSelector = 2;
			if (OptionHasChanged)
			{
				SetAudioSettings();
				Singleton<Settings>.Instance.SaveSettings();
				NotificationText.text = "Audio settings have been saved";
				NotificationWarning.SetTrigger("OnSettingsApply");
				PlayAudio(5);
			}
		}
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(0, UseAudio: true, "Arrow");
			if (YAxis < 0f)
			{
				if (Index > 0)
				{
					Index--;
				}
				else
				{
					Index = MaxIndex;
				}
			}
			if (YAxis > 0f)
			{
				if (Index < MaxIndex)
				{
					Index++;
				}
				else
				{
					Index = 0;
				}
			}
		}
		if (!UsingXAxis && XAxis != 0f && Index < AudioSettings.Length)
		{
			UsingXAxis = true;
			OptionHasChanged = true;
			PlayAudio(0);
			_ = Index;
			_ = 2;
			if (AudioSettings[Index] > 0.001f && XAxis < 0f)
			{
				AudioSettings[Index] -= 0.1f;
			}
			if (AudioSettings[Index] < 1f && XAxis > 0f)
			{
				AudioSettings[Index] += 0.1f;
			}
			UpdateAudioSliders(AudioSliderTransforms, AudioSettings);
		}
		CommonText.text = "";
	}

	private void StateAudioTabEnd()
	{
		if (OptionHasChanged)
		{
			SetAudioSettings();
			Singleton<Settings>.Instance.SaveSettings();
		}
		MenuAnimTrigger("On Button Main");
	}

	private void StateDataTabStart()
	{
		MenuState = State.DataTab;
		ConfirmReset = false;
		ConfirmState = 0;
		Index = 1;
		MaxIndex = 1;
	}

	private void StateDataTab()
	{
		if (!ConfirmReset)
		{
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				DoSelector(1, UseAudio: true, "Arrow");
				ConfirmPanel.SetTrigger("On Open");
				ConfirmPanelTime = Time.time;
				ConfirmState = 0;
				ConfirmReset = true;
			}
			else if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				DoSelector(3, UseAudio: true, "Arrow");
				StateMachine.ChangeState(StateOptions);
				OptionsSelector = 3;
			}
		}
		else if (ConfirmState == 0)
		{
			if (Time.time - ConfirmPanelTime > 0.3f)
			{
				ConfirmState = 1;
			}
		}
		else if (ConfirmState == 1)
		{
			if (!UsingXAxis && XAxis != 0f)
			{
				UsingXAxis = true;
				PlayAudio(0);
				if (XAxis < 0f)
				{
					if (ConfirmIndex > 0)
					{
						ConfirmIndex--;
					}
					else
					{
						ConfirmIndex = 1;
					}
				}
				if (XAxis > 0f)
				{
					if (ConfirmIndex < 1)
					{
						ConfirmIndex++;
					}
					else
					{
						ConfirmIndex = 0;
					}
				}
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				ConfirmPanel.SetTrigger("On Close");
				ConfirmPanelTime = Time.time;
				if (ConfirmIndex == 0)
				{
					NotificationText.text = "Data settings have been saved";
					NotificationWarning.SetTrigger("OnSettingsApply");
					PlayAudio(5);
					Singleton<GameData>.Instance.ResetGameData();
				}
				else
				{
					PlayAudio(3);
				}
				ConfirmState = 2;
			}
		}
		else if (Time.time - ConfirmPanelTime > 0.3f)
		{
			ConfirmIndex = 0;
			ConfirmReset = false;
		}
		UpdateDataInfo();
		if (Index == 0)
		{
			CommonText.text = "Select and load a different save file";
		}
		else
		{
			CommonText.text = "Reset the current save file's data";
		}
	}

	private void StateDataTabEnd()
	{
	}

	private void StateFileSelectStart()
	{
		MenuState = State.FileSelect;
		MainMenuAnimator.SetTrigger("On File Select Enter");
	}

	private void StateFileSelect()
	{
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			StateMachine.ChangeState(StateDataTab);
		}
		CommonText.text = "Select a save file";
	}

	private void StateFileSelectEnd()
	{
		MainMenuAnimator.SetTrigger("On File Select Exit");
	}

	private void StateUIConfigStart()
	{
		MenuState = State.UIConfig;
		Index = 0;
		OptionHasChanged = false;
		MaxIndex = UIOptions_Switch.Length - 1;
		UpdateSwitcher(UISwitchTransforms, UIOptions_Switch, UISettings);
		MenuAnimTrigger("On Button B");
		MenuAnimTrigger("On Title Bar");
	}

	private void StateUIConfig()
	{
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			DoSelector(3, UseAudio: true, "Arrow");
			if (OptionHasChanged)
			{
				SetUISettings();
				Singleton<Settings>.Instance.SaveSettings();
				NotificationText.text = "UI settings have been saved";
				NotificationWarning.SetTrigger("OnSettingsApply");
				PlayAudio(5);
			}
			StateMachine.ChangeState(StateExtrasGame);
		}
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(0, UseAudio: true, "Double Arrow");
			if (YAxis < 0f)
			{
				if (Index > 0)
				{
					Index--;
				}
				else
				{
					Index = MaxIndex;
				}
			}
			if (YAxis > 0f)
			{
				if (Index < MaxIndex)
				{
					Index++;
				}
				else
				{
					Index = 0;
				}
			}
		}
		if (!UsingXAxis && XAxis != 0f && Index < UISettings.Length)
		{
			UsingXAxis = true;
			OptionHasChanged = true;
			PlayAudio(4);
			if (XAxis < 0f)
			{
				if (UISettings[Index] > 0)
				{
					UISettings[Index]--;
				}
				else
				{
					UISettings[Index] = UIOptions_Switch[Index].Length - 1;
				}
			}
			if (XAxis > 0f)
			{
				if (UISettings[Index] < UIOptions_Switch[Index].Length - 1)
				{
					UISettings[Index]++;
				}
				else
				{
					UISettings[Index] = 0;
				}
			}
		}
		UpdateSwitcher(UISwitchTransforms, UIOptions_Switch, UISettings);
		if (Index == 0)
		{
			CommonText.text = "Select the display type";
		}
		else if (Index == 1)
		{
			CommonText.text = "Select the text box type";
		}
		else if (Index == 2)
		{
			CommonText.text = "Select the item box type";
		}
		else if (Index == 3)
		{
			CommonText.text = "Select the pause menu type";
		}
		else if (Index == 4)
		{
			CommonText.text = "Select the enemy healthbar type";
		}
		else if (Index == 5)
		{
			CommonText.text = "Select the loading screen type";
		}
		else if (Index == 6)
		{
			CommonText.text = "Select a UI preset";
		}
	}

	private void StateUIConfigEnd()
	{
		if (OptionHasChanged)
		{
			SetUISettings();
			Singleton<Settings>.Instance.SaveSettings();
		}
	}

	private void StateLoadStart()
	{
		MenuState = State.Load;
		WaitToLoad = Time.time;
		MenuAnimTrigger("On End Menu");
		FadeScreen.SetTrigger("FadeIn");
	}

	private void StateLoad()
	{
		if (!(Time.time - WaitToLoad > 2.5f))
		{
			return;
		}
		if (!LoadCredits)
		{
			switch (PlayerSelector)
			{
			case 0:
				if (ActSelector == 0)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wvo_a_sn");
				}
				else if (ActSelector == 1)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("dtd_a_sn");
				}
				else if (ActSelector == 2)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wap_a_sn");
				}
				else if (ActSelector == 3)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("csc_a_sn");
				}
				else if (ActSelector == 4)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("flc_a_sn");
				}
				else if (ActSelector == 5)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("rct_a_sn");
				}
				else if (ActSelector == 6)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("tpj_a_sn");
				}
				else if (ActSelector == 7)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("kdv_a_sn");
				}
				else if (ActSelector == 8)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("aqa_a_sn");
				}
				else if (ActSelector == 9)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wvo_a_tl");
				}
				else if (ActSelector == 1000)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("test_a_sn");
				}
				break;
			case 1:
				if (ActSelector == 0)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wap_a_sd");
				}
				else if (ActSelector == 1)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("kdv_a_sd");
				}
				else if (ActSelector == 2)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("csc_a_sd");
				}
				else if (ActSelector == 3)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("flc_a_sd");
				}
				else if (ActSelector == 4)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("rct_a_sd");
				}
				else if (ActSelector == 5)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("aqa_a_sd");
				}
				else if (ActSelector == 6)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wvo_b_sd");
				}
				else if (ActSelector == 7)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("dtd_a_sd");
				}
				else if (ActSelector == 8)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("tpj_c_rg");
				}
				else if (ActSelector == 1000)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("test_a_sn");
				}
				break;
			case 2:
				if (ActSelector == 0)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("csc_f1_sv");
				}
				else if (ActSelector == 1)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("tpj_c_sv");
				}
				else if (ActSelector == 2)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("dtd_b_sv");
				}
				else if (ActSelector == 3)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wap_a_sv");
				}
				else if (ActSelector == 4)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("rct_a_sv");
				}
				else if (ActSelector == 5)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("aqa_a_sv");
				}
				else if (ActSelector == 6)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("kdv_d_sv");
				}
				else if (ActSelector == 7)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("flc_a_sv");
				}
				else if (ActSelector == 8)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("wvo_a_bz");
				}
				else if (ActSelector == 1000)
				{
					Singleton<GameManager>.Instance.SetLoadingTo("test_a_sn");
				}
				break;
			}
		}
		else
		{
			SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Single);
		}
	}

	private void StateLoadEnd()
	{
	}

	private void StateExitStart()
	{
		MenuState = State.Exit;
		WaitToLoad = Time.time;
		MenuAnimTrigger("On End Menu");
		FadeScreen.SetTrigger("FadeIn");
		if (!GoToTitleScreen)
		{
			SaveIcon.gameObject.SetActive(value: true);
		}
	}

	private void StateExit()
	{
		if (!GoToTitleScreen && !ExitSaveData && Time.time - WaitToLoad > 1.9f)
		{
			SaveIcon.SetTrigger("On End");
			Singleton<GameData>.Instance.SaveGameData();
			Singleton<Settings>.Instance.SaveSettings();
			ExitSaveData = true;
		}
		if (Time.time - WaitToLoad > ((!GoToTitleScreen) ? 3.5f : 2.5f))
		{
			if (!GoToTitleScreen)
			{
				Application.Quit();
			}
			else
			{
				SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
			}
		}
	}

	private void StateExitEnd()
	{
	}

	private void MenuAnimTrigger(string TriggerName)
	{
		AnimatorControllerParameter[] parameters = MainMenuAnimator.parameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
		{
			if (animatorControllerParameter.type == AnimatorControllerParameterType.Trigger)
			{
				MainMenuAnimator.ResetTrigger(animatorControllerParameter.name);
			}
		}
		MainMenuAnimator.SetTrigger(TriggerName);
	}

	private void GrabSettings()
	{
		GraphicsSettings[0] = Singleton<Settings>.Instance.settings.DisplayMode;
		if (Singleton<Settings>.Instance.settings.Resolution == -1)
		{
			Singleton<Settings>.Instance.settings.Resolution = 0;
		}
		GraphicsSettings[1] = Singleton<Settings>.Instance.settings.Resolution;
		GraphicsSettings[2] = Singleton<Settings>.Instance.settings.RunInBG;
		GraphicsSettings[3] = Singleton<Settings>.Instance.settings.AntiAliasing;
		GraphicsSettings[4] = Singleton<Settings>.Instance.settings.AnisotropicFiltering;
		GraphicsSettings[5] = Singleton<Settings>.Instance.settings.VSync;
		GraphicsSettings[7] = Singleton<Settings>.Instance.settings.DrawDistance;
		GraphicsSettings[8] = Singleton<Settings>.Instance.settings.TextureQuality;
		GraphicsSettings[9] = Singleton<Settings>.Instance.settings.CameraLensFX;
		GraphicsSettings[10] = Singleton<Settings>.Instance.settings.BlurFX;
		GraphicsSettings[11] = Singleton<Settings>.Instance.settings.Bloom;
		GraphicsSettings[12] = Singleton<Settings>.Instance.settings.Outlines;
		GraphicsSettings[13] = Singleton<Settings>.Instance.settings.VolumetricLights;
		GraphicsSettings[14] = Singleton<Settings>.Instance.settings.Reflections;
		GraphicsSettings[15] = Singleton<Settings>.Instance.settings.ShadowQuality;
		GraphicsSettings[16] = Singleton<Settings>.Instance.settings.ShadowType;
		GraphicsSettings[17] = Singleton<Settings>.Instance.settings.ShadowDistance;
		GraphicsSettings[18] = Singleton<Settings>.Instance.settings.ShadowCascades;
		GameSettings[0] = Singleton<Settings>.Instance.settings.InvertCamX;
		GameSettings[1] = Singleton<Settings>.Instance.settings.InvertCamY;
		GameSettings[2] = Singleton<Settings>.Instance.settings.InvertGliderY;
		GameSettings[3] = Singleton<Settings>.Instance.settings.Dialogue;
		GameSettings[4] = Singleton<Settings>.Instance.settings.Cutscenes;
		GameSettings[5] = Singleton<Settings>.Instance.settings.NoCameraVolumes;
		GameSettings[6] = Singleton<Settings>.Instance.settings.Hints;
		for (int i = 0; i < Singleton<Settings>.Instance.LanguageArray.Length; i++)
		{
			if (Singleton<Settings>.Instance.LanguageArray[i] == Singleton<Settings>.Instance.settings.AudioLanguage)
			{
				GameSettings[7] = i;
			}
		}
		GameSettings[8] = Singleton<Settings>.Instance.settings.ButtonIcons;
		GameSettings[9] = Singleton<Settings>.Instance.settings.BGVideo;
		GameSettings[10] = Singleton<Settings>.Instance.settings.CharacterSway;
		GameSettings[11] = Singleton<Settings>.Instance.settings.CameraLeaning;
		GameSettings[12] = Singleton<Settings>.Instance.settings.JiggleBones;
		UISettings[0] = Singleton<Settings>.Instance.settings.DisplayType;
		UISettings[1] = Singleton<Settings>.Instance.settings.TextBoxType;
		UISettings[2] = Singleton<Settings>.Instance.settings.ItemBoxType;
		UISettings[3] = Singleton<Settings>.Instance.settings.PauseMenuType;
		UISettings[4] = Singleton<Settings>.Instance.settings.EnemyHealthType;
		UISettings[5] = Singleton<Settings>.Instance.settings.LoadingScreenType;
		UISettings[6] = Singleton<Settings>.Instance.settings.UIPreset;
		AudioSettings[0] = Singleton<Settings>.Instance.settings.MusicVolume;
		AudioSettings[1] = Singleton<Settings>.Instance.settings.SEVolume;
		AudioSettings[2] = Singleton<Settings>.Instance.settings.VoiceVolume;
		GameRoomSettings[0] = Singleton<Settings>.Instance.settings.TGSSonic;
		GameRoomSettings[1] = Singleton<Settings>.Instance.settings.CameraType;
		GameRoomSettings[2] = Singleton<Settings>.Instance.settings.HomingReticle;
		GameRoomSettings[3] = Singleton<Settings>.Instance.settings.AttackReticles;
		GameRoomSettings[4] = Singleton<Settings>.Instance.settings.SpinEffect;
		GameRoomSettings[5] = Singleton<Settings>.Instance.settings.JumpdashType;
		GameRoomSettings[6] = Singleton<Settings>.Instance.settings.GemShoesType;
		GameRoomSettings[7] = Singleton<Settings>.Instance.settings.UpgradeModels;
		GameRoomSettings[8] = Singleton<Settings>.Instance.settings.E3XBLAMusic;
	}

	private void SetGameRoomSettings()
	{
		Singleton<Settings>.Instance.settings.TGSSonic = GameRoomSettings[0];
		Singleton<Settings>.Instance.settings.CameraType = GameRoomSettings[1];
		Singleton<Settings>.Instance.settings.HomingReticle = GameRoomSettings[2];
		Singleton<Settings>.Instance.settings.AttackReticles = GameRoomSettings[3];
		Singleton<Settings>.Instance.settings.SpinEffect = GameRoomSettings[4];
		Singleton<Settings>.Instance.settings.JumpdashType = GameRoomSettings[5];
		Singleton<Settings>.Instance.settings.GemShoesType = GameRoomSettings[6];
		Singleton<Settings>.Instance.settings.UpgradeModels = GameRoomSettings[7];
		Singleton<Settings>.Instance.settings.E3XBLAMusic = GameRoomSettings[8];
	}

	private void SetVideoSettings()
	{
		Singleton<Settings>.Instance.settings.DisplayMode = GraphicsSettings[0];
		if (Singleton<Settings>.Instance.settings.Resolution >= Singleton<Settings>.Instance.AvailableResolutions.Length)
		{
			Singleton<Settings>.Instance.settings.Resolution = Singleton<Settings>.Instance.AvailableResolutions.Length - 1;
		}
		Singleton<Settings>.Instance.settings.Resolution = GraphicsSettings[1];
		Singleton<Settings>.Instance.settings.RunInBG = GraphicsSettings[2];
		Singleton<Settings>.Instance.settings.AntiAliasing = GraphicsSettings[3];
		Singleton<Settings>.Instance.settings.AnisotropicFiltering = GraphicsSettings[4];
		Singleton<Settings>.Instance.settings.VSync = GraphicsSettings[5];
		Singleton<Settings>.Instance.settings.DrawDistance = GraphicsSettings[7];
		Singleton<Settings>.Instance.settings.TextureQuality = GraphicsSettings[8];
		Singleton<Settings>.Instance.settings.CameraLensFX = GraphicsSettings[9];
		Singleton<Settings>.Instance.settings.BlurFX = GraphicsSettings[10];
		Singleton<Settings>.Instance.settings.Bloom = GraphicsSettings[11];
		Singleton<Settings>.Instance.settings.Outlines = GraphicsSettings[12];
		Singleton<Settings>.Instance.settings.VolumetricLights = GraphicsSettings[13];
		Singleton<Settings>.Instance.settings.Reflections = GraphicsSettings[14];
		Singleton<Settings>.Instance.settings.ShadowQuality = GraphicsSettings[15];
		Singleton<Settings>.Instance.settings.ShadowType = GraphicsSettings[16];
		Singleton<Settings>.Instance.settings.ShadowDistance = GraphicsSettings[17];
		Singleton<Settings>.Instance.settings.ShadowCascades = GraphicsSettings[18];
	}

	private void SetGameSettings()
	{
		Singleton<Settings>.Instance.settings.InvertCamX = GameSettings[0];
		Singleton<Settings>.Instance.settings.InvertCamY = GameSettings[1];
		Singleton<Settings>.Instance.settings.InvertGliderY = GameSettings[2];
		Singleton<Settings>.Instance.settings.Dialogue = GameSettings[3];
		Singleton<Settings>.Instance.settings.Cutscenes = GameSettings[4];
		Singleton<Settings>.Instance.settings.NoCameraVolumes = GameSettings[5];
		Singleton<Settings>.Instance.settings.Hints = GameSettings[6];
		Singleton<Settings>.Instance.settings.AudioLanguage = Singleton<Settings>.Instance.LanguageArray[GameSettings[7]];
		Singleton<Settings>.Instance.settings.ButtonIcons = GameSettings[8];
		Singleton<Settings>.Instance.settings.BGVideo = GameSettings[9];
		Singleton<Settings>.Instance.settings.CharacterSway = GameSettings[10];
		Singleton<Settings>.Instance.settings.CameraLeaning = GameSettings[11];
		Singleton<Settings>.Instance.settings.JiggleBones = GameSettings[12];
		BGVideo.UpdateVideo();
	}

	private void SetAudioSettings()
	{
		Singleton<Settings>.Instance.settings.MusicVolume = AudioSettings[0];
		Singleton<Settings>.Instance.settings.SEVolume = AudioSettings[1];
		Singleton<Settings>.Instance.settings.VoiceVolume = AudioSettings[2];
	}

	private void SetUISettings()
	{
		Singleton<Settings>.Instance.settings.DisplayType = UISettings[0];
		Singleton<Settings>.Instance.settings.TextBoxType = UISettings[1];
		Singleton<Settings>.Instance.settings.ItemBoxType = UISettings[2];
		Singleton<Settings>.Instance.settings.PauseMenuType = UISettings[3];
		Singleton<Settings>.Instance.settings.EnemyHealthType = UISettings[4];
		Singleton<Settings>.Instance.settings.LoadingScreenType = UISettings[5];
		Singleton<Settings>.Instance.settings.UIPreset = UISettings[6];
	}

	private void UpdateUI()
	{
		BGGradients[0].color = Color.Lerp(BGGradients[0].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear, Time.deltaTime * 2f);
		BGGradients[1].color = Color.Lerp(BGGradients[1].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white, Time.deltaTime * 2f);
		BGGradientsAdd[0].color = Color.Lerp(BGGradientsAdd[0].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear, Time.deltaTime * 2f);
		BGGradientsAdd[1].color = Color.Lerp(BGGradientsAdd[1].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white, Time.deltaTime * 2f);
		BGGradientsOther[0].color = Color.Lerp(BGGradientsOther[0].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.white : Color.clear, Time.deltaTime * 2f);
		BGGradientsOther[1].color = Color.Lerp(BGGradientsOther[1].color, (Singleton<Settings>.Instance.settings.ButtonIcons == 0) ? Color.clear : Color.white, Time.deltaTime * 2f);
		Headers[0].anchoredPosition = Vector3.Lerp(Headers[0].anchoredPosition, new Vector3((MenuState == State.MainMenu) ? (-288.75f) : 870f, Headers[0].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[1].anchoredPosition = Vector3.Lerp(Headers[1].anchoredPosition, new Vector3((MenuState == State.ActTrial) ? (-287.5f) : 870f, Headers[1].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[2].anchoredPosition = Vector3.Lerp(Headers[2].anchoredPosition, new Vector3((MenuState == State.GoldMedalResults) ? (-237.5f) : 920f, Headers[2].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[3].anchoredPosition = Vector3.Lerp(Headers[3].anchoredPosition, new Vector3((MenuState == State.ExtrasAudio) ? (-237f) : 920f, Headers[3].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[4].anchoredPosition = Vector3.Lerp(Headers[4].anchoredPosition, new Vector3((MenuState == State.ExtrasGame) ? (-288.2f) : 870f, Headers[4].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[5].anchoredPosition = Vector3.Lerp(Headers[5].anchoredPosition, new Vector3((MenuState == State.Options || MenuState == State.VideoTab || MenuState == State.GameTab || MenuState == State.AudioTab || MenuState == State.DataTab) ? (-288.2f) : 870f, Headers[5].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[6].anchoredPosition = Vector3.Lerp(Headers[6].anchoredPosition, new Vector3((MenuState == State.FileSelect) ? (-288.2f) : 870f, Headers[6].anchoredPosition.y), Time.deltaTime * 10f);
		Headers[7].anchoredPosition = Vector3.Lerp(Headers[7].anchoredPosition, new Vector3((MenuState == State.UIConfig) ? (-288.2f) : 870f, Headers[7].anchoredPosition.y), Time.deltaTime * 10f);
		if (MenuState == State.MainMenu)
		{
			SelectorTransform.anchoredPosition = new Vector3(IsSinglePlayer ? ((!IsTrialSelect) ? 134f : 234f) : ((!IsMultiPlayer && !IsExtras) ? 38f : ((!IsTag) ? 175f : 270f)), IsSinglePlayer ? ((!IsTrialSelect) ? SPDropdown[MainMenuSelector].anchoredPosition.y : TSDropdown[MainMenuSelector].anchoredPosition.y) : (IsExtras ? ExtrasDropdown[MainMenuSelector].anchoredPosition.y : ((!IsMultiPlayer) ? MenuTransforms[MainMenuSelector].anchoredPosition.y : ((!IsTag) ? MPTransforms[MainMenuSelector].anchoredPosition.y : TagTransforms[MainMenuSelector].anchoredPosition.y))));
		}
		else if (MenuState == State.ActTrial)
		{
			SelectorTransform.anchoredPosition = new Vector3((MenuState == State.ActTrial && IsActSelect && !IsDifficultySelect) ? 38f : (-1500f), (MenuState != State.ActTrial || !IsActSelect) ? SelectorTransform.anchoredPosition.y : ((PlayerSelector == 0) ? ActListSonic[ActSelector].anchoredPosition.y : ((PlayerSelector == 1) ? ActListShadow[ActSelector].anchoredPosition.y : ActListSilver[ActSelector].anchoredPosition.y)));
		}
		else
		{
			SelectorTransform.anchoredPosition = new Vector3(-1500f, SelectorTransform.anchoredPosition.y);
		}
		SelectorTransform.SetParent((MenuState != State.ActTrial || !IsActSelect) ? SelectorParent : ((PlayerSelector == 0) ? ActListSonicPanel : ((PlayerSelector == 1) ? ActListShadowPanel : ActListSilverPanel)), worldPositionStays: true);
		SelectorTransform.SetSiblingIndex(0);
		ShortSelectorTransform.anchoredPosition = new Vector3((MenuState != State.ExtrasAudio && (MenuState != State.ActTrial || !IsDifficultySelect)) ? (-1500f) : ((MenuState == State.ActTrial) ? 77f : 28f), (MenuState == State.ActTrial) ? ActMissionTransforms[ActMissionSelector].anchoredPosition.y : AudioRoomTransforms[AudioRoomSelector].anchoredPosition.y);
		MenuTransforms[0].anchoredPosition = Vector3.Lerp(MenuTransforms[0].anchoredPosition, new Vector3((MenuState == State.MainMenu || MenuState == State.ExtrasGame) ? (-240f) : (-900f), MenuTransforms[0].anchoredPosition.y), Time.deltaTime * 25f);
		MenuTransforms[1].anchoredPosition = Vector3.Lerp(MenuTransforms[1].anchoredPosition, new Vector3(((MenuState == State.MainMenu && !IsSinglePlayer) || MenuState == State.ExtrasGame) ? (-240f) : (-900f), MenuTransforms[1].anchoredPosition.y), Time.deltaTime * 25f);
		MenuTransforms[2].anchoredPosition = Vector3.Lerp(MenuTransforms[2].anchoredPosition, new Vector3(((MenuState == State.MainMenu && !IsSinglePlayer && !IsMultiPlayer) || MenuState == State.ExtrasGame) ? (-240f) : (-900f), MenuTransforms[2].anchoredPosition.y), Time.deltaTime * 25f);
		MenuTransforms[3].anchoredPosition = Vector3.Lerp(MenuTransforms[3].anchoredPosition, new Vector3((MenuState == State.MainMenu && !IsSinglePlayer && !IsMultiPlayer && !IsExtras) ? (-240f) : (-900f), MenuTransforms[3].anchoredPosition.y), Time.deltaTime * 25f);
		MenuTransforms[4].anchoredPosition = Vector3.Lerp(MenuTransforms[4].anchoredPosition, new Vector3((MenuState == State.MainMenu && !IsTrialSelect && !IsTag && !IsExtras) ? (-240f) : (-900f), MenuTransforms[4].anchoredPosition.y), Time.deltaTime * 25f);
		SPDropdown[0].anchoredPosition = Vector3.Lerp(SPDropdown[0].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsSinglePlayer) ? (-117f) : (-900f), SPDropdown[0].anchoredPosition.y), Time.deltaTime * 25f);
		SPDropdown[1].anchoredPosition = Vector3.Lerp(SPDropdown[1].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsSinglePlayer) ? (-117f) : (-900f), SPDropdown[1].anchoredPosition.y), Time.deltaTime * 25f);
		SPDropdown[2].anchoredPosition = Vector3.Lerp(SPDropdown[2].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsSinglePlayer) ? (-117f) : (-900f), IsTrialSelect ? (-116.5f) : 3.5f), Time.deltaTime * 25f);
		for (int i = 0; i < TSDropdown.Length; i++)
		{
			TSDropdown[i].anchoredPosition = Vector3.Lerp(TSDropdown[i].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsTrialSelect) ? (-40.2f) : (-900f), TSDropdown[i].anchoredPosition.y), Time.deltaTime * 25f);
		}
		MPTransforms[0].anchoredPosition = Vector3.Lerp(MPTransforms[0].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsMultiPlayer) ? (-140f) : (-900f), MPTransforms[0].anchoredPosition.y), Time.deltaTime * 25f);
		MPTransforms[1].anchoredPosition = Vector3.Lerp(MPTransforms[1].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsMultiPlayer) ? (-140f) : (-900f), IsTag ? (-116.5f) : 3.5f), Time.deltaTime * 25f);
		TagTransforms[0].anchoredPosition = Vector3.Lerp(TagTransforms[0].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsTag) ? (-39.75f) : (-900f), TagTransforms[0].anchoredPosition.y), Time.deltaTime * 25f);
		TagTransforms[1].anchoredPosition = Vector3.Lerp(TagTransforms[1].anchoredPosition, new Vector3((MenuState == State.MainMenu && IsTag) ? (-39.75f) : (-900f), TagTransforms[1].anchoredPosition.y), Time.deltaTime * 25f);
		ExtrasDropdown[0].anchoredPosition = Vector3.Lerp(ExtrasDropdown[0].anchoredPosition, new Vector3(((MenuState == State.MainMenu && IsExtras) || MenuState == State.ExtrasGame) ? (-117f) : (-900f), ExtrasDropdown[0].anchoredPosition.y), Time.deltaTime * 25f);
		ExtrasDropdown[1].anchoredPosition = Vector3.Lerp(ExtrasDropdown[1].anchoredPosition, new Vector3(((MenuState == State.MainMenu && IsExtras) || MenuState == State.ExtrasGame) ? (-117f) : (-900f), ExtrasDropdown[1].anchoredPosition.y), Time.deltaTime * 25f);
		ExtrasDropdown[2].anchoredPosition = Vector3.Lerp(ExtrasDropdown[2].anchoredPosition, new Vector3(((MenuState == State.MainMenu && IsExtras) || MenuState == State.ExtrasGame) ? (-117f) : (-900f), ExtrasDropdown[2].anchoredPosition.y), Time.deltaTime * 25f);
		AudioRoomOptions.anchoredPosition = Vector3.Lerp(AudioRoomOptions.anchoredPosition, new Vector3((MenuState == State.ExtrasAudio) ? 0f : (-900f), AudioRoomOptions.anchoredPosition.y), Time.deltaTime * 25f);
		AudioRoomPanel.anchoredPosition = Vector3.Lerp(AudioRoomPanel.anchoredPosition, new Vector3((MenuState == State.ExtrasAudio) ? 31.9f : 900f, OptionsPanel.anchoredPosition.y), Time.deltaTime * 30f);
		for (int j = 0; j < AudioRoomLists.Length; j++)
		{
			AudioRoomLists[j].anchoredPosition = Vector3.Lerp(AudioRoomLists[j].anchoredPosition, new Vector3((MenuState == State.ExtrasAudio && j == AudioRoomSelector) ? 0f : 900f, AudioRoomLists[j].anchoredPosition.y), Time.deltaTime * 30f);
		}
		if (AudioRoomSelector == 0)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, ThemeAudioList[AudioListSelector].anchoredPosition.y);
			ThemeAudioList[0].gameObject.SetActive(AudioListSelector <= 4);
			ThemeAudioList[1].gameObject.SetActive(AudioListSelector <= 5);
			ThemeAudioList[2].gameObject.SetActive(AudioListSelector <= 6);
			ThemeAudioList[3].gameObject.SetActive(AudioListSelector <= 7);
			ThemeAudioList[4].gameObject.SetActive(AudioListSelector <= 8);
			ThemeAudioList[5].gameObject.SetActive(AudioListSelector <= 9);
			ThemeAudioList[8].gameObject.SetActive(AudioListSelector > 4);
			ThemeAudioList[9].gameObject.SetActive(AudioListSelector > 5);
			ThemeAudioList[10].gameObject.SetActive(AudioListSelector > 6);
			ThemeAudioList[11].gameObject.SetActive(AudioListSelector > 7);
			ThemeAudioList[12].gameObject.SetActive(AudioListSelector > 8);
			ThemeAudioList[13].gameObject.SetActive(AudioListSelector > 9);
			if (AudioListSelector <= 4)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 0f);
			}
			else if (AudioListSelector <= 5)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 43f);
			}
			else if (AudioListSelector <= 6)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 86f);
			}
			else if (AudioListSelector <= 7)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 129f);
			}
			else if (AudioListSelector <= 8)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 172f);
			}
			else if (AudioListSelector <= 9)
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 215f);
			}
			else
			{
				AudioRoomLists[0].anchoredPosition = new Vector2(AudioRoomLists[0].anchoredPosition.x, 258f);
			}
		}
		else if (AudioRoomSelector == 1)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, ActAudioList[AudioListSelector].anchoredPosition.y);
			ActAudioList[0].gameObject.SetActive(AudioListSelector <= 4);
			ActAudioList[1].gameObject.SetActive(AudioListSelector <= 5);
			ActAudioList[2].gameObject.SetActive(AudioListSelector <= 6);
			ActAudioList[3].gameObject.SetActive(AudioListSelector <= 7);
			ActAudioList[4].gameObject.SetActive(AudioListSelector <= 8);
			ActAudioList[5].gameObject.SetActive(AudioListSelector <= 9);
			ActAudioList[6].gameObject.SetActive(AudioListSelector <= 10);
			ActAudioList[7].gameObject.SetActive(AudioListSelector <= 11);
			ActAudioList[8].gameObject.SetActive(AudioListSelector > 4 && AudioListSelector < 13);
			ActAudioList[9].gameObject.SetActive(AudioListSelector > 5 && AudioListSelector < 14);
			ActAudioList[10].gameObject.SetActive(AudioListSelector > 6 && AudioListSelector < 15);
			ActAudioList[11].gameObject.SetActive(AudioListSelector > 7 && AudioListSelector < 16);
			ActAudioList[12].gameObject.SetActive(AudioListSelector > 8 && AudioListSelector < 17);
			ActAudioList[13].gameObject.SetActive(AudioListSelector > 9 && AudioListSelector < 18);
			ActAudioList[14].gameObject.SetActive(AudioListSelector > 10 && AudioListSelector < 19);
			ActAudioList[15].gameObject.SetActive(AudioListSelector > 11 && AudioListSelector < 20);
			ActAudioList[16].gameObject.SetActive(AudioListSelector > 12 && AudioListSelector < 21);
			ActAudioList[17].gameObject.SetActive(AudioListSelector > 13 && AudioListSelector < 22);
			ActAudioList[18].gameObject.SetActive(AudioListSelector > 14 && AudioListSelector < 23);
			ActAudioList[19].gameObject.SetActive(AudioListSelector > 15 && AudioListSelector < 24);
			ActAudioList[20].gameObject.SetActive(AudioListSelector > 16 && AudioListSelector < 25);
			ActAudioList[21].gameObject.SetActive(AudioListSelector > 17 && AudioListSelector < 26);
			ActAudioList[22].gameObject.SetActive(AudioListSelector > 18 && AudioListSelector < 27);
			ActAudioList[23].gameObject.SetActive(AudioListSelector > 19 && AudioListSelector < 28);
			ActAudioList[24].gameObject.SetActive(AudioListSelector > 20 && AudioListSelector < 29);
			ActAudioList[25].gameObject.SetActive(AudioListSelector > 21 && AudioListSelector < 30);
			ActAudioList[26].gameObject.SetActive(AudioListSelector > 22 && AudioListSelector < 31);
			ActAudioList[27].gameObject.SetActive(AudioListSelector > 23 && AudioListSelector < 32);
			ActAudioList[28].gameObject.SetActive(AudioListSelector > 24 && AudioListSelector < 33);
			ActAudioList[29].gameObject.SetActive(AudioListSelector > 25);
			ActAudioList[30].gameObject.SetActive(AudioListSelector > 26);
			ActAudioList[31].gameObject.SetActive(AudioListSelector > 27);
			ActAudioList[32].gameObject.SetActive(AudioListSelector > 28);
			ActAudioList[33].gameObject.SetActive(AudioListSelector > 29);
			ActAudioList[34].gameObject.SetActive(AudioListSelector > 30);
			ActAudioList[35].gameObject.SetActive(AudioListSelector > 31);
			ActAudioList[36].gameObject.SetActive(AudioListSelector > 32);
			if (AudioListSelector <= 4)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 0f);
			}
			else if (AudioListSelector <= 5)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 43f);
			}
			else if (AudioListSelector <= 6)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 86f);
			}
			else if (AudioListSelector <= 7)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 129f);
			}
			else if (AudioListSelector <= 8)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 172f);
			}
			else if (AudioListSelector <= 9)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 215f);
			}
			else if (AudioListSelector <= 10)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 258f);
			}
			else if (AudioListSelector <= 11)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 301f);
			}
			else if (AudioListSelector <= 12)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 344f);
			}
			else if (AudioListSelector <= 13)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 387f);
			}
			else if (AudioListSelector <= 14)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 430f);
			}
			else if (AudioListSelector <= 15)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 473f);
			}
			else if (AudioListSelector <= 16)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 516f);
			}
			else if (AudioListSelector <= 17)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 559f);
			}
			else if (AudioListSelector <= 18)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 602f);
			}
			else if (AudioListSelector <= 19)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 645f);
			}
			else if (AudioListSelector <= 20)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 688f);
			}
			else if (AudioListSelector <= 21)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 731f);
			}
			else if (AudioListSelector <= 22)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 774f);
			}
			else if (AudioListSelector <= 23)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 817f);
			}
			else if (AudioListSelector <= 24)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 860f);
			}
			else if (AudioListSelector <= 25)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 903f);
			}
			else if (AudioListSelector <= 26)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 946f);
			}
			else if (AudioListSelector <= 27)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 989f);
			}
			else if (AudioListSelector <= 28)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1032f);
			}
			else if (AudioListSelector <= 29)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1075f);
			}
			else if (AudioListSelector <= 30)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1118f);
			}
			else if (AudioListSelector <= 31)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1161f);
			}
			else if (AudioListSelector <= 32)
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1204f);
			}
			else
			{
				AudioRoomLists[1].anchoredPosition = new Vector2(AudioRoomLists[1].anchoredPosition.x, 1247f);
			}
		}
		else if (AudioRoomSelector == 2)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, BossAudioList[AudioListSelector].anchoredPosition.y);
			BossAudioList[0].gameObject.SetActive(AudioListSelector <= 4);
			BossAudioList[8].gameObject.SetActive(AudioListSelector > 4);
			if (AudioListSelector <= 4)
			{
				AudioRoomLists[2].anchoredPosition = new Vector2(AudioRoomLists[2].anchoredPosition.x, 0f);
			}
			else
			{
				AudioRoomLists[2].anchoredPosition = new Vector2(AudioRoomLists[2].anchoredPosition.x, 43f);
			}
		}
		else if (AudioRoomSelector == 3)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, TownAudioList[AudioListSelector].anchoredPosition.y);
		}
		else if (AudioRoomSelector == 4)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, OtherAudioList[AudioListSelector].anchoredPosition.y);
		}
		else if (AudioRoomSelector == 5)
		{
			AudioListSelectorTransform.anchoredPosition = new Vector3(-105.25f, Project06AudioList[AudioListSelector].anchoredPosition.y);
		}
		GameRoomPanel.anchoredPosition = Vector3.Lerp(GameRoomPanel.anchoredPosition, new Vector3((MenuState == State.ExtrasGame) ? 305f : 1030f, GameRoomPanel.anchoredPosition.y), Time.deltaTime * 30f);
		for (int k = 0; k < ActListSonic.Length; k++)
		{
			ActListSonic[k].anchoredPosition = Vector3.Lerp(ActListSonic[k].anchoredPosition, new Vector3((MenuState == State.ActTrial && IsActSelect && !IsDifficultySelect && PlayerSelector == 0) ? (-81f) : (-1000f), ActListSonic[k].anchoredPosition.y), Time.deltaTime * 25f);
		}
		for (int l = 0; l < ActListShadow.Length; l++)
		{
			ActListShadow[l].anchoredPosition = Vector3.Lerp(ActListShadow[l].anchoredPosition, new Vector3((MenuState == State.ActTrial && IsActSelect && !IsDifficultySelect && PlayerSelector == 1) ? (-81f) : (-1000f), ActListShadow[l].anchoredPosition.y), Time.deltaTime * 25f);
		}
		for (int m = 0; m < ActListSilver.Length; m++)
		{
			ActListSilver[m].anchoredPosition = Vector3.Lerp(ActListSilver[m].anchoredPosition, new Vector3((MenuState == State.ActTrial && IsActSelect && !IsDifficultySelect && PlayerSelector == 2) ? (-81f) : (-1000f), ActListSilver[m].anchoredPosition.y), Time.deltaTime * 25f);
		}
		if (PlayerSelector == 0)
		{
			ActListSonic[0].gameObject.SetActive(ActSelector <= 3);
			ActListSonic[1].gameObject.SetActive(ActSelector <= 4);
			ActListSonic[2].gameObject.SetActive(ActSelector <= 5);
			ActListSonic[3].gameObject.SetActive(ActSelector <= 6);
			ActListSonic[4].gameObject.SetActive(ActSelector <= 7);
			ActListSonic[5].gameObject.SetActive(ActSelector <= 8);
			ActListSonic[6].gameObject.SetActive(ActSelector <= 9);
			ActListSonic[7].gameObject.SetActive(ActSelector <= 10 && ActSelector >= 4);
			ActListSonic[8].gameObject.SetActive(ActSelector > 4);
			ActListSonic[9].gameObject.SetActive(ActSelector > 5);
			ActListSonic[10].gameObject.SetActive(ActSelector > 6);
			ActListSonic[11].gameObject.SetActive(ActSelector > 7);
			ActListSonic[12].gameObject.SetActive(ActSelector > 8);
			ActListSonic[13].gameObject.SetActive(ActSelector > 9);
			ActListSonic[14].gameObject.SetActive(ActSelector > 10);
		}
		if (PlayerSelector == 1)
		{
			ActListShadow[0].gameObject.SetActive(ActSelector <= 3);
			ActListShadow[1].gameObject.SetActive(ActSelector <= 4);
			ActListShadow[2].gameObject.SetActive(ActSelector <= 5);
			ActListShadow[3].gameObject.SetActive(ActSelector <= 6);
			ActListShadow[4].gameObject.SetActive(ActSelector <= 7);
			ActListShadow[5].gameObject.SetActive(ActSelector <= 8);
			ActListShadow[6].gameObject.SetActive(ActSelector <= 9);
			ActListShadow[7].gameObject.SetActive(ActSelector >= 4);
			ActListShadow[8].gameObject.SetActive(ActSelector > 4);
			ActListShadow[9].gameObject.SetActive(ActSelector > 5);
			ActListShadow[10].gameObject.SetActive(ActSelector > 6);
			ActListShadow[11].gameObject.SetActive(ActSelector > 7);
			ActListShadow[12].gameObject.SetActive(ActSelector > 8);
			ActListShadow[13].gameObject.SetActive(ActSelector > 9);
		}
		if (PlayerSelector == 2)
		{
			ActListSilver[0].gameObject.SetActive(ActSelector <= 3);
			ActListSilver[1].gameObject.SetActive(ActSelector <= 4);
			ActListSilver[2].gameObject.SetActive(ActSelector <= 5);
			ActListSilver[3].gameObject.SetActive(ActSelector <= 6);
			ActListSilver[4].gameObject.SetActive(ActSelector <= 7);
			ActListSilver[5].gameObject.SetActive(ActSelector <= 8);
			ActListSilver[6].gameObject.SetActive(ActSelector <= 9);
			ActListSilver[7].gameObject.SetActive(ActSelector >= 4);
			ActListSilver[8].gameObject.SetActive(ActSelector > 4);
			ActListSilver[9].gameObject.SetActive(ActSelector > 5);
			ActListSilver[10].gameObject.SetActive(ActSelector > 6);
			ActListSilver[11].gameObject.SetActive(ActSelector > 7);
			ActListSilver[12].gameObject.SetActive(ActSelector > 8);
			ActListSilver[13].gameObject.SetActive(ActSelector > 9);
		}
		if ((MenuState == State.ActTrial && IsActSelect) || MenuState == State.Load)
		{
			switch (PlayerSelector)
			{
			case 0:
				if (ActSelector <= 3)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 0f);
				}
				else if (ActSelector == 4)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 50f);
				}
				else if (ActSelector == 5)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 100f);
				}
				else if (ActSelector == 6)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 150f);
				}
				else if (ActSelector == 7)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 200f);
				}
				else if (ActSelector == 8)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 250f);
				}
				else if (ActSelector == 9)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 300f);
				}
				else if (ActSelector == 10)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 350f);
				}
				else if (ActSelector >= 11)
				{
					ActListSonicPanel.anchoredPosition = new Vector2(ActListSonicPanel.anchoredPosition.x, 400f);
				}
				break;
			case 1:
				if (ActSelector <= 3)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 0f);
				}
				else if (ActSelector == 4)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 50f);
				}
				else if (ActSelector == 5)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 100f);
				}
				else if (ActSelector == 6)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 150f);
				}
				else if (ActSelector == 7)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 200f);
				}
				else if (ActSelector == 8)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 250f);
				}
				else if (ActSelector == 9)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 300f);
				}
				else if (ActSelector >= 10)
				{
					ActListShadowPanel.anchoredPosition = new Vector2(ActListShadowPanel.anchoredPosition.x, 350f);
				}
				break;
			case 2:
				if (ActSelector <= 3)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 0f);
				}
				else if (ActSelector == 4)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 50f);
				}
				else if (ActSelector == 5)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 100f);
				}
				else if (ActSelector == 6)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 150f);
				}
				else if (ActSelector == 7)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 200f);
				}
				else if (ActSelector == 8)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 250f);
				}
				else if (ActSelector == 9)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 300f);
				}
				else if (ActSelector >= 10)
				{
					ActListSilverPanel.anchoredPosition = new Vector2(ActListSilverPanel.anchoredPosition.x, 350f);
				}
				break;
			}
		}
		ActMissionPanel.anchoredPosition = Vector3.Lerp(ActMissionPanel.anchoredPosition, new Vector3((MenuState == State.ActTrial && IsActSelect && IsDifficultySelect) ? 285.5f : 1050f, ActMissionPanel.anchoredPosition.y), Time.deltaTime * 25f);
		ActMissionDiffs.anchoredPosition = Vector3.Lerp(ActMissionDiffs.anchoredPosition, new Vector3((MenuState == State.ActTrial && IsActSelect && IsDifficultySelect) ? 0f : (-500f), ActMissionDiffs.anchoredPosition.y), Time.deltaTime * 25f);
		for (int n = 0; n < PlayerArrowIndicator.Length; n++)
		{
			PlayerArrowIndicator[n].SetActive(MenuState == State.ActTrial && !IsActSelect);
		}
		Color color = PlayerNameBG.color;
		color.a = Mathf.Lerp(color.a, (MenuState == State.ActTrial && !IsActSelect) ? 255f : 0f, Time.deltaTime * 20f);
		PlayerNameBG.color = color;
		PlayerText[0].color = Color.Lerp(PlayerText[0].color, (MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 0) ? Color.white : Color.clear, Time.deltaTime * 20f);
		PlayerText[1].color = Color.Lerp(PlayerText[1].color, (MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 1) ? Color.white : Color.clear, Time.deltaTime * 20f);
		PlayerText[2].color = Color.Lerp(PlayerText[2].color, (MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 2) ? Color.white : Color.clear, Time.deltaTime * 20f);
		PlayerText[3].color = Color.Lerp(PlayerText[3].color, (MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 3) ? Color.gray : Color.clear, Time.deltaTime * 20f);
		for (int num = 0; num < GoldMedalStoryIndicator.Length; num++)
		{
			GoldMedalStoryIndicator[num].SetActive(MenuState == State.GoldMedalResults);
		}
		Color color2 = GoldMedalAmountIcon.color;
		color2.a = Mathf.Lerp(color2.a, (MenuState == State.GoldMedalResults) ? 255f : 0f, Time.deltaTime * 30f);
		GoldMedalAmountIcon.color = color2;
		GoldMedalAmount.color = color2;
		if (VerIndex == 0)
		{
			PanelHeight = -12.6f;
		}
		else if (VerIndex == 1)
		{
			PanelHeight = -73.6f;
		}
		else if (VerIndex == 2)
		{
			PanelHeight = -134.6f;
		}
		else if (VerIndex == 3)
		{
			PanelHeight = -195.6f;
		}
		else if (VerIndex == 4)
		{
			PanelHeight = -256.6f;
		}
		else if (VerIndex == 5)
		{
			PanelHeight = -317.6f;
		}
		else if (VerIndex == 6)
		{
			PanelHeight = -378.6f;
		}
		else if (VerIndex == 7)
		{
			PanelHeight = -439.6f;
		}
		else if (VerIndex == 8)
		{
			PanelHeight = -500.6f;
		}
		else if (VerIndex == 9)
		{
			PanelHeight = -561.6f;
		}
		else if (VerIndex == 10)
		{
			PanelHeight = -622.6f;
		}
		else if (VerIndex == 11)
		{
			PanelHeight = -683.6f;
		}
		else if (VerIndex == 12)
		{
			PanelHeight = -744.6f;
		}
		else if (VerIndex == 13)
		{
			PanelHeight = -805.6f;
		}
		GoldMedalPanelContent[0].anchoredPosition = new Vector3(PanelHeight, (Index == 0 && MenuState == State.GoldMedalResults) ? 0f : 1200f, 0f);
		GoldMedalPanelContent[1].anchoredPosition = new Vector3(PanelHeight, (Index == 1 && MenuState == State.GoldMedalResults) ? 0f : 1200f, 0f);
		GoldMedalPanelContent[2].anchoredPosition = new Vector3(PanelHeight, (Index == 2 && MenuState == State.GoldMedalResults) ? 0f : 1200f, 0f);
		GoldMedalPanelContent[3].anchoredPosition = new Vector3(PanelHeight, (Index == 3 && MenuState == State.GoldMedalResults) ? 0f : 1200f, 0f);
		OptionsArrowSelector.SetActive(MenuState == State.ExtrasGame || MenuState == State.Options || MenuState == State.VideoTab || MenuState == State.GameTab || MenuState == State.AudioTab || MenuState == State.DataTab);
		if (MenuState == State.ExtrasGame)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3(60f, GameRoomSwitchTransforms[Index].anchoredPosition.y - 2f);
		}
		else if (MenuState == State.Options)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3(-486.8f, OptionTabsTransforms[OptionsSelector].anchoredPosition.y + 0.5f);
		}
		else if (MenuState == State.VideoTab)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3((Index != 17 && Index != 18 && Index != 19) ? 61.5f : 96.5f, GraphicsSwitchTransforms[Index].anchoredPosition.y);
		}
		else if (MenuState == State.GameTab)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3(61.5f, GameSwitchTransforms[Index].anchoredPosition.y);
		}
		else if (MenuState == State.AudioTab)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3(61.5f, AudioSliderTransforms[Index].anchoredPosition.y);
		}
		else if (MenuState == State.DataTab)
		{
			OptionsArrowTransform.anchoredPosition = new Vector3(61.5f, DataSwitchTransforms[Index].anchoredPosition.y);
		}
		UIDoubleArrowSelector.SetActive(MenuState == State.UIConfig);
		if (MenuState == State.UIConfig)
		{
			UIDoubleArrowTransform.anchoredPosition = new Vector3((Index != 6) ? (-284f) : (-368f), UIDescriptionTransforms[Index].anchoredPosition.y - 3f);
			UIDoubleArrowSeparator.sizeDelta = new Vector2((Index != 6) ? 100f : 155f, UIDoubleArrowSeparator.sizeDelta.y);
			if (Index == 6 && (UISettings[Index] == 0 || UISettings[Index] == 1))
			{
				UISettings[0] = ((UISettings[Index] != 0) ? 1 : 0);
				UISettings[1] = ((UISettings[Index] != 0) ? 1 : 0);
				UISettings[2] = ((UISettings[Index] != 0) ? 1 : 0);
				UISettings[3] = ((UISettings[Index] != 0) ? 1 : 0);
				UISettings[4] = ((UISettings[Index] != 0) ? 1 : 0);
				UISettings[5] = ((UISettings[Index] != 0) ? 1 : 0);
			}
			if (Index != 6 && ((UISettings[6] == 0 && (UISettings[0] != 0 || UISettings[1] != 0 || UISettings[2] != 0 || UISettings[3] != 0 || UISettings[4] != 0 || UISettings[5] != 0)) || (UISettings[6] == 1 && (UISettings[0] != 1 || UISettings[1] != 1 || UISettings[2] != 1 || UISettings[3] != 1 || UISettings[4] != 1 || UISettings[5] != 1))))
			{
				UISettings[6] = 2;
			}
			UIBG.enabled = Index != 4;
			UIDisplayElements[0].enabled = UISettings[0] == 0 && Index != 4 && Index != 5;
			UIDisplayElements[1].enabled = UISettings[0] == 1 && Index != 4 && Index != 5;
			UITextBoxElements[0].enabled = UISettings[1] == 0 && Index != 4 && Index != 5;
			UITextBoxElements[1].enabled = UISettings[1] == 1 && Index != 4 && Index != 5;
			UIItemBoxElements[0].enabled = UISettings[2] == 0 && UISettings[0] == 0 && Index != 4 && Index != 5;
			UIItemBoxElements[1].enabled = UISettings[2] == 0 && UISettings[0] == 1 && Index != 4 && Index != 5;
			UIPauseMenuElements[0].enabled = UISettings[3] == 0 && Index != 4 && Index != 5;
			UIPauseMenuElements[1].enabled = UISettings[3] == 1 && Index != 4 && Index != 5;
			UIEnemyHealthElements[0].enabled = UISettings[4] == 0 && Index == 4;
			UIEnemyHealthElements[1].enabled = UISettings[4] == 1 && Index == 4;
			UILoadingBGs[0].enabled = UISettings[5] == 0 && Index == 5;
			UILoadingBGs[1].enabled = UISettings[5] == 1 && Index == 5;
		}
		UIPanels[0].anchoredPosition = Vector3.Lerp(UIPanels[0].anchoredPosition, new Vector3((MenuState == State.UIConfig) ? (-368f) : (-900f), UIPanels[0].anchoredPosition.y), Time.deltaTime * 25f);
		UIPanels[1].anchoredPosition = Vector3.Lerp(UIPanels[1].anchoredPosition, new Vector3((MenuState == State.UIConfig) ? 250f : 1000f, UIPanels[1].anchoredPosition.y), Time.deltaTime * 25f);
		for (int num2 = 0; num2 < UIText.Length; num2++)
		{
			UIText[num2].color = Color.Lerp(UIText[num2].color, (MenuState == State.UIConfig) ? Color.white : Color.clear, Time.deltaTime * 20f);
		}
		Color color3 = UIPresetBG.color;
		color3.a = Mathf.Lerp(color3.a, (MenuState == State.UIConfig) ? 255f : 0f, Time.deltaTime * 20f);
		UIPresetBG.color = color3;
		for (int num3 = 0; num3 < OptionTabsTransforms.Length; num3++)
		{
			OptionTabsTransforms[num3].anchoredPosition = Vector3.Lerp(OptionTabsTransforms[num3].anchoredPosition, new Vector3((MenuState == State.Options || MenuState == State.VideoTab || MenuState == State.GameTab || MenuState == State.AudioTab || MenuState == State.DataTab) ? (-310.25f) : (-925f), OptionTabsTransforms[num3].anchoredPosition.y), Time.deltaTime * 30f);
		}
		OptionsPanel.anchoredPosition = Vector3.Lerp(OptionsPanel.anchoredPosition, new Vector3((MenuState == State.Options || MenuState == State.VideoTab || MenuState == State.GameTab || MenuState == State.AudioTab || MenuState == State.DataTab) ? 200f : 900f, OptionsPanel.anchoredPosition.y), Time.deltaTime * 30f);
		for (int num4 = 0; num4 < OptionTabsHighlights.Length; num4++)
		{
			OptionTabsHighlights[num4].color = Color.Lerp(OptionTabsHighlights[num4].color, (num4 == OptionsSelector) ? Color.white : Color.clear, Time.deltaTime * 20f);
		}
		for (int num5 = 0; num5 < OptionTabs.Length; num5++)
		{
			OptionTabs[num5].anchoredPosition = Vector3.Lerp(OptionTabs[num5].anchoredPosition, new Vector3((num5 == OptionsSelector) ? 0f : 650f, OptionTabs[0].anchoredPosition.y), Time.deltaTime * 20f);
		}
		VideoSettingsTransforms[0].anchoredPosition = Vector3.Lerp(VideoSettingsTransforms[0].anchoredPosition, new Vector3((!AdvancedSettings) ? 0f : 650f, VideoSettingsTransforms[0].anchoredPosition.y), Time.deltaTime * 20f);
		VideoSettingsTransforms[1].anchoredPosition = Vector3.Lerp(VideoSettingsTransforms[1].anchoredPosition, new Vector3((MenuState == State.VideoTab && AdvancedSettings) ? 0f : 650f, VideoSettingsTransforms[1].anchoredPosition.y), Time.deltaTime * 20f);
		ConfirmPanelSelector.anchoredPosition = Vector3.Lerp(ConfirmPanelSelector.anchoredPosition, new Vector3(ConfirmPanelTransforms[ConfirmIndex].anchoredPosition.x, ConfirmPanelSelector.anchoredPosition.y), Time.deltaTime * 10f);
	}

	private void UpdateMusic()
	{
		if (MenuState != State.Load && MenuState != State.Exit)
		{
			Music.volume = Mathf.Lerp(Music.volume, (MenuState == State.ExtrasAudio) ? 0f : 1f, Time.deltaTime * 5f);
			AudioMusic.volume = Mathf.Lerp(AudioMusic.volume, (MenuState != State.ExtrasAudio || IsAudioList) ? 0f : 1f, Time.deltaTime * 5f);
		}
		else
		{
			Music.volume = Mathf.Lerp(Music.volume, 0f, Time.deltaTime * 2.75f);
			AudioMusic.volume = Mathf.Lerp(AudioMusic.volume, 0f, Time.deltaTime * 2.75f);
		}
	}

	private void UpdateMeshes()
	{
		for (int i = 0; i < SonicRenderers.Length; i++)
		{
			SonicRenderers[i].enabled = MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 0;
		}
		for (int j = 0; j < ShadowRenderers.Length; j++)
		{
			ShadowRenderers[j].enabled = MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 1;
		}
		for (int k = 0; k < SilverRenderers.Length; k++)
		{
			SilverRenderers[k].enabled = MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 2;
		}
		for (int l = 0; l < LastStoryRenderers.Length; l++)
		{
			LastStoryRenderers[l].enabled = MenuState == State.ActTrial && !IsActSelect && PlayerSelector == 3;
		}
		for (int m = 0; m < PlayerModels.Length; m++)
		{
			PlayFaceAnimation(PlayerModels[m], "Face Index", 1);
		}
		if (PlayerModels[PlayerSelector] != null)
		{
			if (PlayerSelector == 3)
			{
				PlayAnimation(PlayerModels[3], ((!IsActSelect && !CharSelected) || IsActSelect) ? "Select" : "Select End", ((!IsActSelect && !CharSelected) || IsActSelect) ? "On Select" : "On Select End");
				PlayAnimation(PlayerModels[4], ((!IsActSelect && !CharSelected) || IsActSelect) ? "Select" : "Select End", ((!IsActSelect && !CharSelected) || IsActSelect) ? "On Select" : "On Select End");
				PlayAnimation(PlayerModels[5], ((!IsActSelect && !CharSelected) || IsActSelect) ? "Select" : "Select End", ((!IsActSelect && !CharSelected) || IsActSelect) ? "On Select" : "On Select End");
			}
			else
			{
				PlayAnimation(PlayerModels[PlayerSelector], ((!IsActSelect && !CharSelected) || IsActSelect) ? "Select" : "Select End", ((!IsActSelect && !CharSelected) || IsActSelect) ? "On Select" : "On Select End");
			}
		}
	}

	private void UpdateSwitcher(RectTransform[] SwitchTransforms, string[][] Switch, int[] Option)
	{
		for (int i = 0; i < SwitchTransforms.Length; i++)
		{
			SwitchTransforms[i].GetComponent<Text>().text = ((i < Switch.Length) ? Singleton<MSTManager>.Instance.HintOptionsDemo[Switch[i][Option[i]]].Text : "");
		}
	}

	private void UpdateVideoSwitchers(RectTransform[] SwitchTransforms, string[][] Switch, int[] Option)
	{
		ResolutionOptions = new string[Singleton<Settings>.Instance.AvailableResolutions.Length];
		for (int i = 0; i < ResolutionOptions.Length; i++)
		{
			ResolutionOptions[i] = Singleton<Settings>.Instance.AvailableResolutions[i].ToString();
		}
		Resolution_Switch = new string[1][] { ResolutionOptions };
		SwitchTransforms[1].GetComponent<Text>().text = Resolution_Switch[0][Option[1]];
		for (int j = 0; j < SwitchTransforms.Length; j++)
		{
			if (j != 1)
			{
				SwitchTransforms[j].GetComponent<Text>().text = ((j < Switch.Length) ? Singleton<MSTManager>.Instance.HintOptionsDemo[Switch[j][Option[j]]].Text : "");
			}
		}
	}

	private void UpdateAudioSliders(RectTransform[] Switch, float[] Value)
	{
		for (int i = 0; i < AudioSliders.Length; i++)
		{
			AudioSliders[i].value = ((i < Switch.Length) ? Value[i] : 0f);
		}
		Singleton<Settings>.Instance.settings.MusicVolume = AudioSettings[0];
		Singleton<Settings>.Instance.settings.SEVolume = AudioSettings[1];
		Singleton<Settings>.Instance.settings.VoiceVolume = AudioSettings[2];
	}

	private void UpdateDataInfo()
	{
		int num = Mathf.FloorToInt(Singleton<GameData>.Instance.Playtime / 3600f);
		int num2 = Mathf.FloorToInt(Singleton<GameData>.Instance.Playtime / 60f);
		int num3 = Mathf.FloorToInt(Singleton<GameData>.Instance.Playtime - (float)(num2 * 60));
		string text = "File " + Singleton<GameData>.Instance.SaveIndex;
		string text2 = $"{num:0}:{num2 % 60:00}:{num3:00}";
		string lastSave = Singleton<GameData>.Instance.Game.LastSave;
		DataInfoTransforms[0].text = "1." + Application.version.Split('.')[1];
		FileSlot.TotalPlaytimeText.text = text2;
		FileSlot.FileText.text = text;
		FileSlot.LastSaveText.text = lastSave;
	}

	private void SetActMissionInfo(bool Reset, int Score = 0, float Time = 0f, int Rings = 0, int TotalScore = 0, int Rank = 0)
	{
		if (!Reset)
		{
			ActMissionDataElements[1].text = Score.ToString();
			ActMissionDataElements[2].text = Game.FormatTime(Time);
			ActMissionDataElements[3].text = Rings.ToString();
			ActMissionDataElements[4].text = TotalScore.ToString();
			RankIcon.enabled = true;
			RankIcon.sprite = RankSprites[Rank];
		}
		else
		{
			ActMissionDataElements[1].text = "0";
			ActMissionDataElements[2].text = "0'00\"00";
			ActMissionDataElements[3].text = "0";
			ActMissionDataElements[4].text = "0";
			RankIcon.enabled = false;
		}
	}

	public void PlayFaceAnimation(Animator Anim, string VarName, int Index)
	{
		Anim.SetInteger(VarName, Index);
	}

	public void PlayAnimation(Animator Anim, string AnimState, string TriggerName)
	{
		if (!Anim.GetCurrentAnimatorStateInfo(0).IsName(AnimState))
		{
			Anim.SetTrigger(TriggerName);
		}
	}

	private void PlayAudio(int ClipInt)
	{
		Singleton<AudioManager>.Instance.PlayClip(AudioClip[ClipInt], 0.7f);
	}

	private void PlayVoiceAudio(string PlayerName)
	{
		string text = "sys01_e00_" + PlayerName;
		Singleton<AudioManager>.Instance.PlayVoiceClip(Resources.Load<AudioClip>("Win32-Xenon/sound/voice/" + Singleton<Settings>.Instance.AudioLanguage() + "/" + text));
	}

	private void DoSelector(int AudioIndex, bool UseAudio = true, string Type = "Selector", string TriggerName = "ChangedOptions")
	{
		if (UseAudio)
		{
			PlayAudio(AudioIndex);
		}
		switch (Type)
		{
		case "Selector":
			SelectorAnimator.SetTrigger(TriggerName);
			break;
		case "Audio Selector":
			AudioSelectorAnimator.SetTrigger(TriggerName);
			break;
		case "Audio List Selector":
			AudioListSelectorAnimator.SetTrigger(TriggerName);
			if (YAxis > 0.1f)
			{
				AudioArrowsAnimator.SetTrigger("On Down");
			}
			else if (YAxis < -0.1f)
			{
				AudioArrowsAnimator.SetTrigger("On Up");
			}
			break;
		case "Arrow":
			OptionsArrowAnimator.SetTrigger(TriggerName);
			break;
		case "Gold Medal Names":
		{
			for (int j = 0; j < GoldMedalStoryAnimators.Length; j++)
			{
				if (j == Index)
				{
					GoldMedalStoryAnimators[j].SetTrigger((XAxis > 0f) ? "On Change Left" : "On Change Right");
				}
			}
			break;
		}
		case "Gold Medal Scroll":
			GoldMedalPanelScroll.SetTrigger((YAxis > 0f) ? "On Down" : "On Up");
			break;
		case "Double Arrow":
		{
			for (int i = 0; i < UIDoubleArrowAnimators.Length; i++)
			{
				UIDoubleArrowAnimators[i].SetTrigger(TriggerName);
			}
			break;
		}
		}
	}

	private void DoOptions(ref float Axis, ref bool UsedAxis, bool UseSelector, ref int Selector, ref int SelectorCount, string SelectorType = "Selector", int AudioIndex = 0)
	{
		if (UsedAxis || Axis == 0f)
		{
			return;
		}
		UsedAxis = true;
		if (Axis > 0f)
		{
			if (Selector < SelectorCount)
			{
				Selector++;
			}
			else
			{
				Selector = 0;
			}
		}
		else if (Axis < 0f)
		{
			if (Selector > 0)
			{
				Selector--;
			}
			else
			{
				Selector = SelectorCount;
			}
		}
		if (UseSelector)
		{
			DoSelector(AudioIndex, UseAudio: false, SelectorType);
		}
		PlayAudio(AudioIndex);
	}
}
