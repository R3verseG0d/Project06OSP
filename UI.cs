using System.Collections.Generic;
using STHEngine;
using STHLua;
using UnityEngine;
using UnityEngine.UI;

public class UI : UIBase
{
	[Header("Framework")]
	public GameObject[] PauseMenuPrefabs;

	public GameObject[] MessageBoxPrefabs;

	public GameObject ResultsScreenPrefab;

	internal GameObject MsgBoxObject;

	[Header("HUD")]
	public GameObject[] StageElements;

	public GameObject[] HubElements;

	public Animator HudAnimator;

	public Animator LifeAnimator;

	public Animator CheckpointAnimator;

	public GameObject Crosshair;

	public Sprite[] CharImages;

	public Sprite[] SuperCharImages;

	public Sprite[] Numbers;

	public Sprite[] SmallNumbers;

	public Image[] LifeHolders;

	public Image[] LivesHud;

	private Animator CrosshairAnimator;

	[Header("Stage")]
	public Animator RingAnimator;

	public Combo ComboPanel;

	public Image[] ScoreHud;

	public Image[] MinutesHud;

	public Image[] SecondsHud;

	public Image[] FractionHud;

	public Image[] RingHud;

	public Image RingAlert;

	[Header("Hub")]
	public RectTransform RetailLivesElement;

	public float RetailLivesHeight;

	public Animator E3LifeAnimator;

	public Image[] E3LivesHud;

	public Image[] BankHud;

	public GameObject RadarMap;

	public RectTransform RadarCompass;

	internal bool IsPaused;

	private bool IsHub;

	[Header("Common")]
	public UICornersGradient[] BaseUI;

	public UICornersGradient[] OutlineUI;

	public UICornersGradient[] RadarUI;

	public UICornersGradient BaseRadarMapUI;

	public Animation fadeOutAnim;

	internal int Score;

	internal int Lives;

	internal int Rings;

	internal int Bank;

	[Header("Item Box Icons")]
	public ItemDisplayManager ItemDisplay;

	[Header("Collectibles")]
	public Animator CollectibleAnimator;

	public Image[] CollectibleHud;

	public Image[] CollectibleMaxHud;

	internal string CollectibleModule;

	internal int CollectibleDisplay;

	internal int MaxCollectibleDisplay;

	private bool UseCollectibles;

	private bool CollectibleOnScreen;

	private float CollectibleTime;

	[Header("Radar")]
	public Animator RadarAnimator;

	public Animator Obj1Animator;

	public Animator Obj2Animator;

	public Animator Obj3Animator;

	public AudioClip RadarBeep;

	public CanvasGroup NearbyObjIndicator;

	internal Common_Key RadarTarget1;

	internal Common_Key RadarTarget2;

	internal Common_Key RadarTarget3;

	internal Transform RadarHolder;

	internal bool UseRadar;

	private float Beep1Time;

	private float Beep2Time;

	private float Beep3Time;

	private float Beep1Wait;

	private float Beep2Wait;

	private float Beep3Wait;

	private float DistToHolder1;

	private float DistToHolder2;

	private float DistToHolder3;

	private bool Obj1Obtained;

	private bool Obj2Obtained;

	private bool Obj3Obtained;

	private int Obj1Index;

	private int Obj2Index;

	private int Obj3Index;

	[Header("Lock On Indicators")]
	public GameObject HomingReticleObject;

	public GameObject GemReticleObject;

	public GameObject SpearReticleObject;

	public GameObject LaserReticleObject;

	internal List<GameObject> MultiReticles;

	[Header("Gems & Maturity")]
	public Animation levelUpAnimation;

	public Image GemHolder;

	public Animator GemPanelAnimator;

	public Animator GemPanelEffect;

	public RectTransform[] GemSlots;

	public Sprite[] GemImages;

	public Sprite LockedGemImage;

	public Image maturityGauge;

	public GameObject SilverUpgradeLimit;

	internal float MaturityDisplay;

	internal float[] GemDisplay = new float[9];

	internal float ChaosMaturityDisplay;

	internal float ESPMaturityDisplay;

	private bool GemPanelShowed;

	private float GemPanelShowTime;

	[Header("Gems 3-Light Level")]
	public Image[] LevelIndicator;

	public AudioClip LevelUpSound;

	public Animator LevelAnimator;

	internal int[] ActiveGemLevel = new int[9];

	private int GemLevel;

	[Header("Action Gauge")]
	public RectTransform ActionGaugeObj;

	public AudioClip actionGaugeMax;

	public Image GaugeBase;

	public Image GaugeOutline;

	public Sprite[] GaugeBaseType;

	public Sprite[] GaugeOutlineType;

	public Scrollbar ActionGauge;

	public Animator FullGaugeAnimator;

	public Image GaugeBar;

	public Image GaugeBarGlow;

	[Header("Gadget UI")]
	public Image GadgetIcon;

	public Sprite[] AmmoIcons;

	public Image[] AmmoHud;

	public Image[] AmmoMaxHud;

	public RectTransform[] AmmoPanels;

	public RectTransform SplitPanel;

	public RectTransform VehicleUI;

	public RectTransform WeaponsUI;

	public Image HealthGauge;

	internal PlayerManager PM;

	internal float ActionDisplay;

	internal StageManager StageManager;

	internal bool UseGauge;

	internal string GaugeModule;

	internal float MaxActionGauge;

	internal float GaugeHeal;

	internal float GaugeHealDelay;

	internal float HealthDisplay;

	internal int AmmoDisplay;

	internal int MaxAmmoDisplay;

	private PauseMenu Pause;

	private bool PlayActionGaugeMaxSound;

	private bool UseVehicleUI;

	private bool UseWeaponsUI;

	private bool IsResultsScreen;

	private float GaugeWaitToRefill;

	private float MaxVehicleHealth;

	private float PauseTimer;

	private int MinutesRound;

	private int SecondsRound;

	private int FractionRound;

	internal bool ActiveCrosshair;

	private void Start()
	{
		IsHub = StageManager.MissionIsHub;
		if (IsHub)
		{
			for (int i = 0; i < StageElements.Length; i++)
			{
				StageElements[i].SetActive(value: false);
			}
			for (int j = 0; j < HubElements.Length; j++)
			{
				HubElements[j].SetActive(value: true);
			}
			if (Singleton<Settings>.Instance.settings.DisplayType == 0)
			{
				RetailLivesElement.anchoredPosition = new Vector2(RetailLivesElement.anchoredPosition.x, RetailLivesHeight);
			}
			else
			{
				LifeAnimator = E3LifeAnimator;
				LivesHud = E3LivesHud;
			}
		}
		for (int k = 0; k < BaseUI.Length; k++)
		{
			BaseUI[k].m_topLeftColor = BaseColor(0);
			BaseUI[k].m_bottomLeftColor = BaseColor(1);
			BaseUI[k].m_topRightColor = BaseColor(2);
			BaseUI[k].m_bottomRightColor = BaseColor(3);
		}
		for (int l = 0; l < OutlineUI.Length; l++)
		{
			OutlineUI[l].m_topLeftColor = OutlineColor(0);
			OutlineUI[l].m_bottomLeftColor = OutlineColor(1);
			OutlineUI[l].m_topRightColor = OutlineColor(2);
			OutlineUI[l].m_bottomRightColor = OutlineColor(3);
		}
		GaugeBar.color = ActionGaugeColor();
		GaugeBarGlow.color = ActionGaugeGlowColor();
		RadarUI[0].m_topLeftColor = TextBoxBGColor(1);
		RadarUI[0].m_topRightColor = TextBoxBGColor(0);
		RadarUI[0].m_bottomRightColor = TextBoxBGColor(3);
		RadarUI[0].m_bottomLeftColor = TextBoxBGColor(2);
		RadarUI[1].m_topLeftColor = TextBoxOutlineColor(0);
		RadarUI[1].m_topRightColor = TextBoxOutlineColor(1);
		RadarUI[1].m_bottomRightColor = TextBoxOutlineColor(2);
		RadarUI[1].m_bottomLeftColor = TextBoxOutlineColor(3);
		RadarUI[2].m_topLeftColor = TextBoxBaseColor(1);
		RadarUI[2].m_topRightColor = TextBoxBaseColor(0);
		RadarUI[2].m_bottomRightColor = TextBoxBaseColor(3);
		RadarUI[2].m_bottomLeftColor = TextBoxBaseColor(2);
		BaseRadarMapUI.m_topLeftColor = BaseRadarmapColor(0);
		BaseRadarMapUI.m_bottomLeftColor = BaseRadarmapColor(1);
		BaseRadarMapUI.m_topRightColor = BaseRadarmapColor(2);
		BaseRadarMapUI.m_bottomRightColor = BaseRadarmapColor(3);
		switch (Singleton<GameManager>.Instance.GameStory)
		{
		case GameManager.Story.Sonic:
			CrosshairAnimator = Crosshair.transform.GetChild(0).GetComponent<Animator>();
			break;
		case GameManager.Story.Shadow:
			CrosshairAnimator = Crosshair.transform.GetChild(1).GetComponent<Animator>();
			break;
		case GameManager.Story.Silver:
			CrosshairAnimator = Crosshair.transform.GetChild(2).GetComponent<Animator>();
			break;
		}
		CrosshairAnimator.gameObject.SetActive(value: true);
		if (Singleton<Settings>.Instance.settings.HomingReticle == 1)
		{
			HomingReticle component = Object.Instantiate(HomingReticleObject, Vector3.zero, Quaternion.identity).GetComponent<HomingReticle>();
			component.HUD = this;
			component.transform.SetParent(base.transform, worldPositionStays: false);
		}
		GemReticle component2 = Object.Instantiate(GemReticleObject, Vector3.zero, Quaternion.identity).GetComponent<GemReticle>();
		component2.HUD = this;
		component2.transform.SetParent(base.transform, worldPositionStays: false);
		MultiReticles = new List<GameObject>();
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		if (gameData.ObtainedGems != null && gameData.ObtainedGems.Count > 1)
		{
			for (int m = 1; m < gameData.ObtainedGems.Count; m++)
			{
				GemSlots[gameData.ObtainedGems[m] - 1].GetComponent<Image>().sprite = GemImages[gameData.ObtainedGems[m]];
			}
		}
		PauseTimer = Time.time;
	}

	public void OpenGauge(string ModuleType, float MaxGauge, float HealAmount, float HealDelay)
	{
		UseGauge = true;
		GaugeModule = ModuleType;
		MaxActionGauge = MaxGauge;
		GaugeHeal = HealAmount;
		GaugeHealDelay = HealDelay;
		UpdateActionGaugePanel();
		ChaosMaturityDisplay = 0f;
		ESPMaturityDisplay = 0f;
	}

	public void CloseGauge()
	{
		UseGauge = false;
		GaugeModule = "";
		MaxActionGauge = 0f;
		GaugeHeal = 0f;
		GaugeHealDelay = 0f;
		ChaosMaturityDisplay = 0f;
		ESPMaturityDisplay = 0f;
	}

	public void OpenVehicle(float MaxHealth)
	{
		UseVehicleUI = true;
		MaxVehicleHealth = MaxHealth;
	}

	public void CloseVehicle()
	{
		UseVehicleUI = false;
	}

	public void OpenWeapons(int Ammo, string ShotName)
	{
		UseWeaponsUI = true;
		MaxAmmoDisplay = Ammo;
		MaxAmmoDisplay = Mathf.Clamp(MaxAmmoDisplay, MaxAmmoDisplay, 100);
		if (!(ShotName == "GadgetMissile"))
		{
			if (ShotName == "GadgetVulcan")
			{
				GadgetIcon.sprite = AmmoIcons[1];
			}
		}
		else
		{
			GadgetIcon.sprite = AmmoIcons[0];
		}
	}

	public void CloseWeapons()
	{
		UseWeaponsUI = false;
	}

	public void OpenCollectibles(string Module, int MaxAmount)
	{
		UseCollectibles = true;
		CollectibleModule = Module;
		CollectibleDisplay = GetFlagCount();
		MaxCollectibleDisplay = MaxAmount;
	}

	public void UpdateCollectibles()
	{
		if (UseCollectibles)
		{
			CollectibleDisplay = GetFlagCount();
			if (!CollectibleOnScreen)
			{
				CollectibleAnimator.SetTrigger("On Appear");
				CollectibleTime = Time.time;
				CollectibleOnScreen = true;
			}
		}
	}

	public void DisplayCollectibles(bool Show)
	{
		if (!UseCollectibles)
		{
			return;
		}
		if (Show)
		{
			if (!CollectibleOnScreen)
			{
				CollectibleAnimator.SetTrigger("On Appear");
				CollectibleOnScreen = true;
			}
		}
		else if (CollectibleOnScreen)
		{
			CollectibleAnimator.SetTrigger("On Disappear");
			CollectibleOnScreen = false;
		}
	}

	public void OpenRadarMap(int MapIndex)
	{
		(Object.Instantiate(Resources.Load("DefaultPrefabs/RadarMap/RadarMap"), Vector3.zero, Quaternion.identity) as GameObject).GetComponent<RadarMap>().OpenMap(MapIndex);
	}

	public void OpenRadar(Transform Holder, Common_Key Target1, Common_Key Target2, Common_Key Target3)
	{
		UseRadar = true;
		RadarHolder = Holder;
		RadarTarget1 = Target1;
		RadarTarget2 = Target2;
		RadarTarget3 = Target3;
		RadarAnimator.SetTrigger("On Appear");
	}

	public void CloseRadar()
	{
		UseRadar = false;
		RadarHolder = null;
		RadarTarget1 = null;
		RadarTarget2 = null;
		RadarTarget3 = null;
		RadarAnimator.SetTrigger("On Disappear");
		NearbyObjIndicator.alpha = 0f;
	}

	private void LateUpdate()
	{
		if (UseRadar)
		{
			Vector3 position = PM.Base.BodyTransform.transform.position + PM.Base.BodyTransform.transform.up * 1.5f;
			position = PM.Base.Camera.Camera.WorldToViewportPoint(position);
			if (position.z < 0f)
			{
				position.x = 1f - position.x;
				position.y = 1f - position.y;
				position.z = 0f;
				position = ExtensionMethods.Vector3Maxamize(position);
			}
			position = PM.Base.Camera.Camera.ViewportToScreenPoint(position);
			position.x = Mathf.Clamp(position.x, 25f, (float)Screen.width - 25f);
			position.y = Mathf.Clamp(position.y, 25f, (float)Screen.height - 25f);
			NearbyObjIndicator.transform.position = position;
			NearbyObjIndicator.transform.eulerAngles = Vector3.zero;
		}
	}

	private void Update()
	{
		GameManager.PlayerData playerData = Singleton<GameManager>.Instance._PlayerData;
		if (!IsHub)
		{
			MinutesRound = Mathf.FloorToInt(playerData.time / 60f);
			SecondsRound = Mathf.FloorToInt(playerData.time - (float)MinutesRound * 60f);
			FractionRound = Mathf.FloorToInt(playerData.time * 1000f % 1000f);
			Counter(Score.ToString("d8"), ScoreHud, Numbers);
			Counter(MinutesRound.ToString("d2"), MinutesHud, Numbers);
			Counter(SecondsRound.ToString("d2"), SecondsHud, Numbers);
			Counter(FractionRound.ToString("d3"), FractionHud, Numbers);
			Counter(Rings.ToString("d3"), RingHud, Numbers);
			Score = Mathf.Clamp(playerData.score, 0, 99999999);
			Rings = Mathf.Clamp(playerData.rings, 0, 999);
			ScoreHud[1].enabled = Score > 9;
			ScoreHud[2].enabled = Score > 99;
			ScoreHud[3].enabled = Score > 999;
			ScoreHud[4].enabled = Score > 9999;
			ScoreHud[5].enabled = Score > 99999;
			ScoreHud[6].enabled = Score > 999999;
			ScoreHud[7].enabled = Score > 9999999;
			MinutesHud[0].enabled = Singleton<Settings>.Instance.settings.DisplayType != 1 || playerData.time > 600f;
			for (int i = 0; i < RingHud.Length; i++)
			{
				RingHud[i].color = Color.Lerp(RingHud[i].color, (Rings <= 0) ? Color.red : Color.white, Time.deltaTime * 10f);
			}
			RingAlert.color = ((Rings <= 0) ? Color.Lerp(Color.red, Color.clear, Mathf.PingPong(Time.time, 0.1f) / 0.1f) : Color.Lerp(RingAlert.color, Color.clear, Time.deltaTime * 10f));
		}
		else
		{
			Counter(Bank.ToString("d8"), BankHud, Numbers);
			Bank = Mathf.Clamp(Bank, 0, 99999999);
			BankHud[1].enabled = Score > 9;
			BankHud[2].enabled = Score > 99;
			BankHud[3].enabled = Score > 999;
			BankHud[4].enabled = Score > 9999;
			BankHud[5].enabled = Score > 99999;
			BankHud[6].enabled = Score > 999999;
			BankHud[7].enabled = Score > 9999999;
		}
		Counter(Lives.ToString("d3"), LivesHud, Numbers);
		Lives = Mathf.Clamp(Singleton<GameManager>.Instance.GetLifeCount(), 0, 999);
		for (int j = 0; j < LifeHolders.Length; j++)
		{
			if ((StageManager.Player == StageManager.PlayerName.Sonic_New && (bool)PM.sonic && PM.sonic.IsSuper) || (StageManager.Player == StageManager.PlayerName.Sonic_Fast && (bool)PM.sonic_fast && PM.sonic_fast.IsSuper))
			{
				LifeHolders[j].sprite = SuperCharImages[0];
			}
			else
			{
				LifeHolders[j].sprite = CharImages[(Singleton<Settings>.Instance.settings.DisplayType == 0) ? ((int)StageManager.Player) : ((int)Singleton<GameManager>.Instance.GameStory)];
			}
		}
		LivesHud[1].enabled = Lives > 9;
		LivesHud[2].enabled = Lives > 99;
		if (Time.time - PauseTimer > 0.5f)
		{
			if (!IsResultsScreen && !IsPaused && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
			{
				Pause = Object.Instantiate(PauseMenuPrefabs[Singleton<Settings>.Instance.settings.PauseMenuType], Vector3.zero, Quaternion.identity).GetComponent<PauseMenu>();
				Pause.HUD = this;
				Pause.transform.SetParent(base.transform, worldPositionStays: false);
				IsPaused = true;
			}
			else if (IsResultsScreen && IsPaused && (bool)Pause)
			{
				Object.Destroy(Pause.gameObject);
				IsPaused = false;
				Singleton<GameManager>.Instance.GameState = GameManager.State.Result;
				AudioListener.pause = false;
				Pause = null;
			}
		}
		if (Singleton<Settings>.Instance.settings.DisplayType == 0)
		{
			ActionGaugeObj.anchoredPosition = Vector3.Lerp(ActionGaugeObj.anchoredPosition, new Vector3(ActionGaugeObj.anchoredPosition.x, (!UseGauge) ? 0f : ((!(GaugeModule == "gauge_module_sonic")) ? (((GaugeModule != "" && GaugeModule != "gauge_module_amy") || (GaugeModule == "gauge_module_amy" && !PM.Base.IsVisible)) ? 360f : 200f) : ((PM.sonic.GemSelector == 0) ? 200f : 360f))), Time.deltaTime * 20f);
		}
		else
		{
			ActionGaugeObj.anchoredPosition = Vector3.Lerp(ActionGaugeObj.anchoredPosition, new Vector3(ActionGaugeObj.anchoredPosition.x, (!UseGauge) ? 0f : ((!(GaugeModule == "gauge_module_sonic")) ? (((GaugeModule != "" && GaugeModule != "gauge_module_amy") || (GaugeModule == "gauge_module_amy" && !PM.Base.IsVisible)) ? 346.5f : 200f) : ((PM.sonic.GemSelector == 0) ? 200f : 346.5f))), Time.deltaTime * 20f);
		}
		if (GaugeModule != "gauge_module_shadow" && LevelAnimator.GetBool("Is Pulsate"))
		{
			LevelAnimator.SetBool("Is Pulsate", value: false);
		}
		if (GaugeModule != "gauge_module_silver" && LevelAnimator.GetBool("Is Upgrade Limit"))
		{
			LevelAnimator.SetBool("Is Upgrade Limit", value: false);
		}
		bool flag = PM.Base.GetState() != "Result" && PM.Base.GetState() != "Cutscene" && !PM.Base.IsSinking;
		if (UseGauge)
		{
			ActionDisplay = Mathf.Clamp(ActionDisplay, 0f, MaxActionGauge);
			ActionGauge.size = ActionDisplay / MaxActionGauge;
			if (ActionDisplay >= MaxActionGauge)
			{
				if (!PlayActionGaugeMaxSound)
				{
					if (GaugeModule != "gauge_module_amy")
					{
						PM.Base.Audio.PlayOneShot(actionGaugeMax, PM.Base.Audio.volume);
					}
					FullGaugeAnimator.SetTrigger("On Full");
					PlayActionGaugeMaxSound = true;
				}
			}
			else if (PlayActionGaugeMaxSound)
			{
				FullGaugeAnimator.SetTrigger("On Not Full");
				PlayActionGaugeMaxSound = false;
			}
			MaturityDisplay = Mathf.Clamp(MaturityDisplay, 0f, MaxActionGauge);
			maturityGauge.enabled = (GaugeModule == "gauge_module_sonic" && PM.sonic.GemSelector != 0) || GaugeModule == "gauge_module_shadow" || (GaugeModule == "gauge_module_silver" && PM.silver.HasSigilOfAwakening);
			maturityGauge.fillAmount = MaturityDisplay / MaxActionGauge;
			GemHolder.enabled = GaugeModule == "gauge_module_sonic";
			LevelIndicator[0].enabled = (GaugeModule == "gauge_module_sonic" && PM.sonic.GemSelector != 0) || (GaugeModule == "gauge_module_shadow" && PM.shadow.ChaosBoostLevel > 0);
			LevelIndicator[1].enabled = (GaugeModule == "gauge_module_sonic" && PM.sonic.GemSelector != 0 && GemLevel > 0) || (GaugeModule == "gauge_module_shadow" && PM.shadow.ChaosBoostLevel > 1);
			LevelIndicator[2].enabled = (GaugeModule == "gauge_module_sonic" && PM.sonic.GemSelector != 0 && GemLevel > 1) || (GaugeModule == "gauge_module_shadow" && PM.shadow.ChaosBoostLevel > 2);
			if ((Time.time - GemPanelShowTime > 2f || GaugeModule != "gauge_module_sonic") && GemPanelShowed)
			{
				GemPanelAnimator.SetTrigger("On Hide");
				GemPanelShowed = false;
			}
			if (GaugeModule == "gauge_module_sonic")
			{
				if (!PM.Base.IsGrounded() && PM.Base.GetState() != "Grinding" && PM.Base.GetState() != "WaterSlide")
				{
					GaugeWaitToRefill = Time.time;
				}
				else
				{
					ReplenishActionGauge(flag && !PM.sonic.UsingGreenGem && !PM.sonic.UsingBlueGem && !PM.sonic.UsingWhiteGem && !PM.sonic.UsingYellowGem && !PM.sonic.UsingYellowGem && PM.sonic.GemSelector != 0);
				}
				if (PM.sonic.UsingRedGem && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused)
				{
					float amount = Sonic_New_Lua.c_red * ((ActiveGemLevel[PM.sonic.GemSelector] == 0) ? 1.25f : ((ActiveGemLevel[PM.sonic.GemSelector] == 1) ? 1f : 0.75f));
					DrainActionGauge(amount, UseTime: true, TimeIsUnscaled: true);
				}
				if (PM.sonic.UsingYellowGem)
				{
					float amount2 = Sonic_New_Lua.c_yellow * ((ActiveGemLevel[PM.sonic.GemSelector] == 0) ? 0.5f : ((ActiveGemLevel[PM.sonic.GemSelector] == 1) ? 0.25f : 0.15f));
					DrainActionGauge(amount2, UseTime: true);
				}
				if (PM.sonic.UsingPurpleGem)
				{
					float amount3 = Sonic_New_Lua.c_purple * ((ActiveGemLevel[PM.sonic.GemSelector] == 0) ? 1f : ((ActiveGemLevel[PM.sonic.GemSelector] == 1) ? 0.75f : 0.5f));
					DrainActionGauge(amount3, UseTime: true);
				}
				GemHolder.sprite = GemImages[PM.sonic.GemSelector];
				MaturityDisplay = GemDisplay[PM.sonic.GemSelector];
				GemLevel = ActiveGemLevel[PM.sonic.GemSelector];
				if (PM.sonic.ObtainedGemIndex != 0)
				{
					for (int k = 0; k < GemSlots.Length; k++)
					{
						GemSlots[k].localScale = Vector3.Lerp(GemSlots[k].localScale, Vector3.one * ((PM.sonic.GemSelector - 1 == k) ? 1f : 0.65f), Time.deltaTime * 15f);
					}
				}
				for (int l = 0; l < GemDisplay.Length; l++)
				{
					ActiveGemLevel[l] = Mathf.Clamp(ActiveGemLevel[l], 0, 2);
					GemDisplay[l] = Mathf.Clamp(GemDisplay[l], 0f, MaxActionGauge);
				}
				if (GemDisplay[PM.sonic.GemSelector] >= MaxActionGauge && ActiveGemLevel[PM.sonic.GemSelector] < 2)
				{
					if (ActiveGemLevel[PM.sonic.GemSelector] < 1)
					{
						GemDisplay[PM.sonic.GemSelector] = 0f;
					}
					ActiveGemLevel[PM.sonic.GemSelector]++;
					PM.Base.Audio.PlayOneShot(LevelUpSound, PM.Base.Audio.volume / 3f);
					levelUpAnimation.Play();
				}
			}
			if (GaugeModule == "gauge_module_princess")
			{
				if (PM.princess.UsingShield)
				{
					DrainActionGauge(Princess_Lua.c_gauge_barrier, UseTime: true);
				}
				else
				{
					ReplenishActionGauge(flag && (PM.Base.IsGrounded() || PM.Base.GetState() == "Grinding"));
				}
			}
			if (GaugeModule == "gauge_module_shadow")
			{
				ChaosMaturityDisplay = Mathf.Clamp(ChaosMaturityDisplay, 0f, MaxActionGauge);
				MaturityDisplay = ChaosMaturityDisplay;
				if (PM.Base.GetState() != "ChaosBoost" && PM.Base.GetState() != "ChaosBlast" && PM.Base.GetState() != "Uninhibit")
				{
					DrainActionGauge((!PM.shadow.IsChaosBoost) ? Shadow_Lua.c_gauge_consumption_0 : (PM.shadow.IsFullPower ? Shadow_Lua.c_gauge_consumption_4 : (((float)PM.shadow.ChaosBoostLevel == 1f) ? Shadow_Lua.c_gauge_consumption_1 : (((float)PM.shadow.ChaosBoostLevel == 2f) ? Shadow_Lua.c_gauge_consumption_2 : Shadow_Lua.c_gauge_consumption_3))), UseTime: true);
				}
			}
			if (GaugeModule == "gauge_module_silver")
			{
				ESPMaturityDisplay = Mathf.Clamp(ESPMaturityDisplay, 0f, MaxActionGauge);
				MaturityDisplay = ESPMaturityDisplay;
				if (PM.Base.GetState() == "Levitate" || PM.Base.GetState() == "PsychicShot" || PM.Base.GetState() == "Upheave" || PM.Base.GetState() == "Dash" || (!PM.Base.IsGrounded() && PM.Base.GetState() != "Grinding") || PM.silver.UsingPsychokinesis || PM.silver.PickedObjects.Count != 0)
				{
					GaugeWaitToRefill = Time.time;
					if (PM.Base.GetState() == "Levitate")
					{
						DrainActionGauge(Silver_Lua.c_psi_gauge_float * PM.silver.ESPMult(), UseTime: true);
					}
					if (PM.Base.GetState() == "Upheave" && PM.silver.UpheaveState == 0 && Time.time - PM.silver.UpheaveTime > 0.45f)
					{
						DrainActionGauge(Silver_Lua.c_psi_gauge_catch_ride * PM.silver.ESPMult(), UseTime: true);
					}
					if (PM.Base.GetState() == "Dash")
					{
						DrainActionGauge(Silver_Lua.c_psi_gauge_teleport_dash_burn * PM.silver.ESPMult(), UseTime: true);
					}
				}
				else
				{
					ReplenishActionGauge(flag);
				}
				if (PM.silver.IsAwakened && PM.Base.GetState() != "ESPAwaken")
				{
					ESPMaturityDisplay -= Silver_Lua.c_psi_awaken_consumption * Time.deltaTime;
				}
			}
			if (GaugeModule == "gauge_module_tails")
			{
				if (PM.Base.GetState() == "Fly")
				{
					GaugeWaitToRefill = Time.time;
					DrainActionGauge(Tails_Lua.c_gauge_max / (Tails_Lua.c_flight_timer * (Tails_Lua.c_flight_timer_b * ((!PM.tails.FlyReleasedKey) ? 0.75f : 1.25f))), UseTime: true);
				}
				else
				{
					ReplenishActionGauge(flag && PM.Base.GetState() != "AerialTailSwipe" && ((!PM.Base.IsGrounded() && PM.tails.CanRegenFly) || PM.Base.IsGrounded() || PM.Base.GetState() == "Grinding"));
				}
			}
			if (GaugeModule == "gauge_module_amy" && !PM.Base.IsVisible)
			{
				GaugeWaitToRefill = Time.time;
				DrainActionGauge(Amy_Lua.c_stealth_limit * 0.7f, UseTime: true);
			}
			if (GaugeModule == "gauge_module_omega")
			{
				if (PM.Base.GetState() == "Hover" || (!PM.Base.IsGrounded() && PM.Base.GetState() != "Grinding"))
				{
					GaugeWaitToRefill = Time.time;
					if (PM.Base.GetState() == "Hover" || PM.Base.GetState() == "OmegaLauncher")
					{
						DrainActionGauge(Omega_Lua.c_gauge_max / Omega_Lua.c_hover, UseTime: true);
					}
				}
				else
				{
					ReplenishActionGauge(flag);
				}
			}
			if (GaugeModule == "gauge_module_metal")
			{
				if (PM.Base.GetState() == "Boost" || (!PM.Base.IsGrounded() && PM.Base.GetState() != "Grinding" && PM.Base.GetState() != "WaterSlide"))
				{
					GaugeWaitToRefill = Time.time;
					if (PM.Base.GetState() == "Boost")
					{
						DrainActionGauge(Metal_Sonic_Lua.c_gauge_max / Metal_Sonic_Lua.c_boost_deplete, UseTime: true);
					}
				}
				else
				{
					ReplenishActionGauge(flag);
				}
			}
		}
		else if (GemPanelShowed)
		{
			GemPanelAnimator.SetTrigger("On Hide");
			GemPanelShowed = false;
		}
		if (UseCollectibles)
		{
			Counter(CollectibleDisplay.ToString("d2"), CollectibleHud, Numbers);
			Counter(MaxCollectibleDisplay.ToString("d2"), CollectibleMaxHud, Numbers);
			CollectibleDisplay = Mathf.Clamp(CollectibleDisplay, 0, MaxCollectibleDisplay);
			MaxCollectibleDisplay = Mathf.Clamp(MaxCollectibleDisplay, 0, 99);
			if (CollectibleOnScreen && !IsPaused && Time.time - CollectibleTime > 2.5f)
			{
				CollectibleAnimator.SetTrigger("On Disappear");
				CollectibleOnScreen = false;
			}
		}
		if (UseRadar)
		{
			if ((!RadarTarget1 && !RadarTarget2 && !RadarTarget3) || (RadarTarget1.Obtained && RadarTarget2.Obtained && RadarTarget3.Obtained) || !RadarHolder)
			{
				CloseRadar();
			}
			NearbyObjIndicator.alpha = (((DistToHolder1 < 12f && (bool)RadarTarget1 && !RadarTarget1.Obtained) || (DistToHolder2 < 12f && (bool)RadarTarget2 && !RadarTarget2.Obtained) || (DistToHolder3 < 12f && (bool)RadarTarget3 && !RadarTarget3.Obtained)) ? 1f : 0f);
			if ((bool)RadarTarget1 && !RadarTarget1.Obtained)
			{
				DistToHolder1 = Vector3.Distance(RadarTarget1.transform.position, RadarHolder.position);
				if (DistToHolder1 < 15f)
				{
					Obj1Index = 3;
					Beep1Wait = 0.25f;
				}
				else if (DistToHolder1 < 40f)
				{
					Obj1Index = 2;
					Beep1Wait = 0.5f;
				}
				else if (DistToHolder1 < 80f)
				{
					Obj1Index = 1;
					Beep1Wait = 1f;
				}
				else
				{
					Beep1Time = Time.time;
					Obj1Index = 0;
				}
				if (Obj1Index != 0 && Time.time - Beep1Time > Beep1Wait)
				{
					Singleton<AudioManager>.Instance.PlayClip(RadarBeep, 0.5f);
					Obj1Animator.SetTrigger("On Beep");
					Beep1Time = Time.time;
				}
				Obj1Animator.SetInteger("Index", Obj1Index);
			}
			else if (!Obj1Obtained)
			{
				Obj1Index = 4;
				Obj1Animator.SetInteger("Index", Obj1Index);
				Obj1Animator.SetTrigger("On Obtain");
				Obj1Obtained = true;
			}
			if ((bool)RadarTarget2 && !RadarTarget2.Obtained)
			{
				DistToHolder2 = Vector3.Distance(RadarTarget2.transform.position, RadarHolder.position);
				if (DistToHolder2 < 15f)
				{
					Obj2Index = 3;
					Beep2Wait = 0.25f;
				}
				else if (DistToHolder2 < 40f)
				{
					Obj2Index = 2;
					Beep2Wait = 0.5f;
				}
				else if (DistToHolder2 < 80f)
				{
					Obj2Index = 1;
					Beep2Wait = 1f;
				}
				else
				{
					Beep2Time = Time.time;
					Obj2Index = 0;
				}
				if (Obj2Index != 0 && Time.time - Beep2Time > Beep2Wait)
				{
					Singleton<AudioManager>.Instance.PlayClip(RadarBeep, 0.5f);
					Obj2Animator.SetTrigger("On Beep");
					Beep2Time = Time.time;
				}
				Obj2Animator.SetInteger("Index", Obj2Index);
			}
			else if ((!RadarTarget2 || RadarTarget2.Obtained) && !Obj2Obtained)
			{
				Obj2Index = 4;
				Obj2Animator.SetInteger("Index", Obj2Index);
				Obj2Animator.SetTrigger("On Obtain");
				Obj2Obtained = true;
			}
			if ((bool)RadarTarget3 && !RadarTarget3.Obtained)
			{
				DistToHolder3 = Vector3.Distance(RadarTarget3.transform.position, RadarHolder.position);
				if (DistToHolder3 < 15f)
				{
					Obj3Index = 3;
					Beep3Wait = 0.25f;
				}
				else if (DistToHolder3 < 40f)
				{
					Obj3Index = 2;
					Beep3Wait = 0.5f;
				}
				else if (DistToHolder3 < 80f)
				{
					Obj3Index = 1;
					Beep3Wait = 1f;
				}
				else
				{
					Beep3Time = Time.time;
					Obj3Index = 0;
				}
				if (Obj3Index != 0 && Time.time - Beep3Time > Beep3Wait)
				{
					Singleton<AudioManager>.Instance.PlayClip(RadarBeep, 0.5f);
					Obj3Animator.SetTrigger("On Beep");
					Beep3Time = Time.time;
				}
				Obj3Animator.SetInteger("Index", Obj3Index);
			}
			else if ((!RadarTarget3 || RadarTarget3.Obtained) && !Obj3Obtained)
			{
				Obj3Index = 4;
				Obj3Animator.SetInteger("Index", Obj3Index);
				Obj3Animator.SetTrigger("On Obtain");
				Obj3Obtained = true;
			}
		}
		if (StageManager.Player == StageManager.PlayerName.Shadow || StageManager.Player == StageManager.PlayerName.Omega)
		{
			VehicleUI.anchoredPosition = Vector3.Lerp(VehicleUI.anchoredPosition, new Vector3(UseVehicleUI ? (-640f) : (-140f), VehicleUI.anchoredPosition.y), Time.deltaTime * 20f);
			WeaponsUI.anchoredPosition = Vector3.Lerp(WeaponsUI.anchoredPosition, new Vector3(UseWeaponsUI ? (-640f) : (-140f), WeaponsUI.anchoredPosition.y), Time.deltaTime * 20f);
			if (UseVehicleUI)
			{
				HealthDisplay = Mathf.Clamp(HealthDisplay, 0f, MaxVehicleHealth);
				HealthGauge.fillAmount = HealthDisplay / MaxVehicleHealth;
			}
			if (UseWeaponsUI)
			{
				Counter(AmmoDisplay.ToString("d3"), AmmoHud, SmallNumbers);
				Counter(MaxAmmoDisplay.ToString("d3"), AmmoMaxHud, SmallNumbers);
				AmmoDisplay = Mathf.Clamp(AmmoDisplay, 0, MaxAmmoDisplay);
				MaxAmmoDisplay = Mathf.Clamp(MaxAmmoDisplay, 0, 100);
				AmmoHud[1].enabled = AmmoDisplay > 9;
				AmmoHud[2].enabled = AmmoDisplay > 99;
				AmmoMaxHud[1].enabled = MaxAmmoDisplay > 9;
				AmmoMaxHud[2].enabled = MaxAmmoDisplay > 99;
				if (AmmoDisplay < 10)
				{
					AmmoPanels[0].anchoredPosition = new Vector3(-49.75f, AmmoPanels[0].anchoredPosition.y);
					SplitPanel.anchoredPosition = new Vector3(-15.25f, SplitPanel.anchoredPosition.y);
				}
				else if (AmmoDisplay < 100)
				{
					AmmoPanels[0].anchoredPosition = new Vector3(-32.5f, AmmoPanels[0].anchoredPosition.y);
					SplitPanel.anchoredPosition = new Vector3(2f, SplitPanel.anchoredPosition.y);
				}
				else
				{
					AmmoPanels[0].anchoredPosition = new Vector3(-15.25f, AmmoPanels[0].anchoredPosition.y);
					SplitPanel.anchoredPosition = new Vector3(19.25f, SplitPanel.anchoredPosition.y);
				}
				if (MaxAmmoDisplay < 10)
				{
					AmmoPanels[1].anchoredPosition = new Vector3(-3f, AmmoPanels[1].anchoredPosition.y);
				}
				else if (MaxAmmoDisplay < 100)
				{
					AmmoPanels[1].anchoredPosition = new Vector3(14.25f, AmmoPanels[1].anchoredPosition.y);
				}
				else
				{
					AmmoPanels[1].anchoredPosition = new Vector3(31.5f, AmmoPanels[1].anchoredPosition.y);
				}
			}
		}
		if (Singleton<Settings>.Instance.settings.AttackReticles != 1)
		{
			return;
		}
		if (PM.Base.GetPrefab("shadow"))
		{
			if (PM.shadow.SpearTargets != null && PM.shadow.GetState() == "ChaosSpear" && PM.shadow.SpearState == 0)
			{
				for (int m = 0; m < PM.shadow.SpearTargets.Count; m++)
				{
					if (PM.shadow.SpearTargets[m] != null)
					{
						CreateMultiReticle(PM.shadow.SpearTargets[m], 0);
					}
				}
			}
			else if (MultiReticles.Count != 0)
			{
				MultiReticles.Clear();
			}
		}
		if (!PM.Base.GetPrefab("omega"))
		{
			return;
		}
		if (PM.omega.LaserTargets != null && PM.omega.LockOnTargets)
		{
			for (int n = 0; n < PM.omega.LaserTargets.Count; n++)
			{
				if (PM.omega.LaserTargets[n] != null)
				{
					CreateMultiReticle(PM.omega.LaserTargets[n], 1);
				}
			}
		}
		else if (MultiReticles.Count != 0)
		{
			MultiReticles.Clear();
		}
	}

	private void CreateMultiReticle(GameObject Target, int ReticleType)
	{
		if (!MultiReticles.Contains(Target))
		{
			Vector3 position = PM.Base.Camera.Camera.ViewportToScreenPoint(Target.transform.position);
			MultiReticle component = Object.Instantiate((ReticleType == 0) ? SpearReticleObject : LaserReticleObject, position, Quaternion.identity).GetComponent<MultiReticle>();
			component.HUD = this;
			component.Target = Target;
			component.transform.SetParent(base.transform, worldPositionStays: false);
			MultiReticles.Add(Target);
		}
	}

	public void UseCrosshair(bool EndCrosshair = false, bool Reset = false)
	{
		if (!Reset || (Reset && ActiveCrosshair))
		{
			if (!EndCrosshair)
			{
				ActiveCrosshair = true;
				CrosshairAnimator.SetTrigger("On Start");
				CrosshairAnimator.ResetTrigger("On End");
			}
			else
			{
				ActiveCrosshair = false;
				CrosshairAnimator.SetTrigger("On End");
				CrosshairAnimator.ResetTrigger("On Start");
			}
		}
	}

	public void AddChaosDriveEnergy(bool Replenish = false)
	{
		if (!(GaugeModule == "gauge_module_amy"))
		{
			ActionDisplay += (Replenish ? Common_Lua.c_gauge_up : 7.5f);
			if (GaugeModule == "gauge_module_sonic" && PM.sonic.GemSelector != 0)
			{
				GemDisplay[PM.sonic.GemSelector] += ((!Replenish) ? 33.35f : 100f);
			}
			if (GaugeModule == "gauge_module_shadow" && PM.shadow.IsChaosBoost)
			{
				ChaosMaturityDisplay += ((!Replenish) ? 20f : 100f);
			}
			if (GaugeModule == "gauge_module_silver" && PM.silver.HasSigilOfAwakening && !PM.silver.IsAwakened)
			{
				ESPMaturityDisplay += ((!Replenish) ? 10f : 100f);
			}
			if (GaugeModule == "gauge_module_omega")
			{
				AmmoDisplay += (Replenish ? ((int)Common_Lua.c_gauge_up) : 15);
			}
		}
	}

	private void ReplenishActionGauge(bool Conditions)
	{
		if (Time.time - GaugeWaitToRefill > GaugeHealDelay && ActionDisplay < MaxActionGauge && Conditions)
		{
			ActionDisplay += Time.deltaTime * GaugeHeal;
		}
	}

	public void DrainActionGauge(float Amount, bool UseTime = false, bool TimeIsUnscaled = false)
	{
		ActionDisplay -= ((!UseTime) ? Amount : (Amount * ((!TimeIsUnscaled) ? Time.deltaTime : Time.unscaledDeltaTime)));
		GaugeWaitToRefill = Time.time;
	}

	public void UpdateGemPanel(GameData.GlobalData GlobalData)
	{
		for (int i = 0; i < GlobalData.ObtainedGems.Count - 1; i++)
		{
			GemSlots[PM.sonic.GemSelector - 1].GetComponent<Image>().sprite = GemImages[PM.sonic.GemSelector];
		}
		GemPanelEffect.SetTrigger("On Effect");
		GemPanelEffect.transform.position = GemSlots[PM.sonic.GemSelector - 1].position;
		TriggerGemPanel();
	}

	public void TriggerGemPanel()
	{
		GemPanelShowTime = Time.time;
		if (!GemPanelShowed)
		{
			GemPanelAnimator.SetTrigger("On Show");
			GemPanelShowed = true;
		}
	}

	public void UpdateActionGaugePanel()
	{
		if (GaugeModule == "gauge_module_sonic" || GaugeModule == "gauge_module_shadow")
		{
			GaugeBase.sprite = GaugeBaseType[0];
			GaugeOutline.sprite = GaugeOutlineType[0];
			SilverUpgradeLimit.SetActive(value: false);
		}
		else if (GaugeModule == "gauge_module_silver")
		{
			GaugeBase.sprite = GaugeBaseType[(!PM.silver.HasSigilOfAwakening) ? 1 : 2];
			GaugeOutline.sprite = GaugeOutlineType[(!PM.silver.HasSigilOfAwakening) ? 1 : 2];
			SilverUpgradeLimit.SetActive(PM.silver.HasSigilOfAwakening);
		}
		else
		{
			GaugeBase.sprite = GaugeBaseType[1];
			GaugeOutline.sprite = GaugeOutlineType[1];
			SilverUpgradeLimit.SetActive(value: false);
		}
	}

	public void PlayFadeOut()
	{
		fadeOutAnim.Play();
	}

	public float StartMessageBox(string[] Texts, string[] Sounds, float Duration = 3f)
	{
		if ((bool)MsgBoxObject)
		{
			Object.Destroy(MsgBoxObject);
		}
		MessageBox component = Object.Instantiate(MessageBoxPrefabs[Singleton<Settings>.Instance.settings.TextBoxType], Vector3.zero, Quaternion.identity).GetComponent<MessageBox>();
		MsgBoxObject = component.gameObject;
		List<AudioClip> list = new List<AudioClip>();
		float num = Duration;
		if (Sounds != null)
		{
			num = 0f;
			for (int i = 0; i < Sounds.Length; i++)
			{
				list.Add(Resources.Load("Win32-Xenon/sound/voice/" + Singleton<Settings>.Instance.AudioLanguage() + "/" + Sounds[i]) as AudioClip);
				num += list[i].length;
			}
			component.Clips = list.ToArray();
		}
		else
		{
			component.Duration = num;
		}
		if (Singleton<Settings>.Instance.settings.TextBoxType != 2)
		{
			component.Texts = Texts;
		}
		component.transform.SetParent(base.transform, worldPositionStays: false);
		return num;
	}

	public void ShowResultsScreen(PlayerBase PlayerBase)
	{
		IsResultsScreen = true;
		ResultsScreen component = Object.Instantiate(ResultsScreenPrefab, Vector3.zero, Quaternion.identity).GetComponent<ResultsScreen>();
		component.Player = PlayerBase;
		component.transform.SetParent(base.transform, worldPositionStays: false);
		HudAnimator.SetTrigger("On Hide");
	}

	public void OnCombo(int ComboBonus, int Level)
	{
		ComboPanel.OnComboFinish(ComboBonus, Level);
	}

	public void OnCheckpoint(float time)
	{
		if (Singleton<Settings>.Instance.settings.DisplayType != 1)
		{
			CheckpointAnimator.SetTrigger("On Save Time");
			CheckpointAnimator.transform.GetComponent<Text>().text = Game.FormatTime(time);
		}
	}
}
