using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIBase
{
	public enum State
	{
		Open = 0,
		Idle = 1,
		Resume = 2,
		StartOver = 3,
		StartOverOptions = 4,
		Exit = 5
	}

	[Header("Framework")]
	public StateMachine StateMachine;

	public RectTransform Selector;

	public RectTransform[] OptionsPos;

	public RectTransform[] RestartOptionsPos;

	public Text[] StartOverTexts;

	public Animator PauseMenuAnimator;

	public Animator SelectorAnimator;

	public AudioSource Audio;

	public AudioClip[] AudioClip;

	[Header("E3")]
	public bool IsE3Menu;

	public Text[] OptionNormal;

	public Text[] OptionHighlights;

	[Header("Retail Campaign Themes")]
	public Image BG;

	public Image[] TopBorders;

	public Image[] BottomBorders;

	public Image[] Arrows;

	public Image[] Selectors;

	public Sprite[] BGThemes;

	public Sprite[] TopBordersThemes;

	public Sprite[] BottomBordersThemes;

	public Sprite[] ArrowsThemes;

	public Sprite[] SelectorsThemes;

	[Header("E3 Campaign Themes")]
	public Image E3Header;

	public Image E3BottomHandle;

	public Image E3BG;

	public Image E3LightPanelBG;

	public Image E3LightPanel;

	public Image[] E3Arrows;

	public Sprite[] E3HeaderThemes;

	public Sprite[] E3BottomHandleThemes;

	public Sprite[] E3BGThemes;

	public Sprite[] E3LightPanelThemes;

	public Sprite[] E3ArrowsThemes;

	internal UI HUD;

	internal State PauseState;

	private bool UsingYAxis;

	private bool StartYScrolling;

	private bool FastYScroll;

	private bool CannotRestart;

	private float YAxis;

	private float AxisYTime;

	private int OptionSelector;

	private float OPStartTime;

	private float RStartTime;

	private bool RestartStage;

	private float SOStartTime;

	private float EStartTime;

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.PauseMenuType == 0)
		{
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Shadow:
			{
				BG.sprite = BGThemes[0];
				for (int m = 0; m < TopBorders.Length; m++)
				{
					TopBorders[m].sprite = TopBordersThemes[0];
				}
				for (int n = 0; n < BottomBorders.Length; n++)
				{
					BottomBorders[n].sprite = BottomBordersThemes[0];
				}
				for (int num = 0; num < Arrows.Length; num++)
				{
					Arrows[num].sprite = ArrowsThemes[0];
				}
				for (int num2 = 0; num2 < Selectors.Length; num2++)
				{
					Selectors[num2].sprite = SelectorsThemes[0];
				}
				break;
			}
			case GameManager.Story.Silver:
			{
				BG.sprite = BGThemes[1];
				for (int i = 0; i < TopBorders.Length; i++)
				{
					TopBorders[i].sprite = TopBordersThemes[1];
				}
				for (int j = 0; j < BottomBorders.Length; j++)
				{
					BottomBorders[j].sprite = BottomBordersThemes[1];
				}
				for (int k = 0; k < Arrows.Length; k++)
				{
					Arrows[k].sprite = ArrowsThemes[1];
				}
				for (int l = 0; l < Selectors.Length; l++)
				{
					Selectors[l].sprite = SelectorsThemes[1];
				}
				break;
			}
			}
			Selectors[0].color = PauseSelectorColor();
			Selectors[1].color = PauseSelectorGlowColor();
		}
		else
		{
			switch (Singleton<GameManager>.Instance.GameStory)
			{
			case GameManager.Story.Shadow:
			{
				E3Header.sprite = E3HeaderThemes[0];
				E3BottomHandle.sprite = E3BottomHandleThemes[0];
				E3BG.sprite = E3BGThemes[0];
				E3LightPanel.sprite = E3LightPanelThemes[0];
				for (int num4 = 0; num4 < E3Arrows.Length; num4++)
				{
					E3Arrows[num4].sprite = E3ArrowsThemes[0];
				}
				break;
			}
			case GameManager.Story.Silver:
			{
				E3Header.sprite = E3HeaderThemes[1];
				E3BottomHandle.sprite = E3BottomHandleThemes[1];
				E3BG.sprite = E3BGThemes[1];
				E3LightPanel.sprite = E3LightPanelThemes[1];
				for (int num3 = 0; num3 < E3Arrows.Length; num3++)
				{
					E3Arrows[num3].sprite = E3ArrowsThemes[1];
				}
				break;
			}
			}
			E3BG.color = PauseE3BGColor();
			E3LightPanelBG.color = PauseE3LightPanelBGColor();
			E3LightPanel.color = PauseE3LightPanelColor();
		}
		Singleton<GameManager>.Instance.GameState = GameManager.State.Paused;
		Audio.ignoreListenerPause = true;
		AudioListener.pause = true;
		HUD.StageManager.BGMPlayer.volume = 0.25f;
		StateOpenStart();
		StateMachine.Initialize(StateOpen);
		OPStartTime = Time.unscaledTime;
		CannotRestart = Singleton<GameManager>.Instance.GetStoryData().Lives < 1;
		StartOverTexts[0].enabled = !CannotRestart;
		StartOverTexts[1].enabled = CannotRestart;
		PauseMenuAnimator.SetTrigger("On Open");
		Audio.PlayOneShot(AudioClip[0], Audio.volume);
	}

	private void Update()
	{
		StateMachine.UpdateStateMachine();
		UpdateUI();
		YAxis = 0f - Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") + (0f - Singleton<RInput>.Instance.P.GetAxis("D-Pad Y"));
		if (YAxis == 0f)
		{
			UsingYAxis = false;
			StartYScrolling = false;
			FastYScroll = false;
			AxisYTime = Time.unscaledTime;
		}
		else if (Time.unscaledTime - AxisYTime > ((!FastYScroll) ? 0.5f : 0.125f) && !StartYScrolling)
		{
			FastYScroll = true;
			StartYScrolling = true;
			AxisYTime = Time.unscaledTime;
			UsingYAxis = false;
			StartYScrolling = false;
		}
	}

	private void LateUpdate()
	{
		if (!IsE3Menu)
		{
			return;
		}
		if (PauseState == State.Idle || PauseState == State.StartOverOptions)
		{
			OptionNormal[0].enabled = PauseState == State.Idle && OptionSelector != 0;
			OptionNormal[1].enabled = PauseState == State.Idle && OptionSelector != 1;
			OptionNormal[2].enabled = PauseState == State.Idle && OptionSelector != 2;
			OptionHighlights[0].enabled = PauseState == State.Idle && OptionSelector == 0;
			OptionHighlights[1].enabled = PauseState == State.Idle && OptionSelector == 1 && !CannotRestart;
			OptionHighlights[2].enabled = PauseState == State.Idle && OptionSelector == 1 && CannotRestart;
			OptionHighlights[3].enabled = PauseState == State.Idle && OptionSelector == 2;
			OptionNormal[3].enabled = PauseState == State.StartOverOptions && OptionSelector != 0;
			OptionNormal[4].enabled = PauseState == State.StartOverOptions && OptionSelector != 1;
			OptionHighlights[4].enabled = PauseState == State.StartOverOptions && OptionSelector == 0;
			OptionHighlights[5].enabled = PauseState == State.StartOverOptions && OptionSelector == 1;
		}
		else
		{
			for (int i = 0; i < OptionHighlights.Length; i++)
			{
				OptionHighlights[i].enabled = false;
			}
			for (int j = 0; j < OptionNormal.Length; j++)
			{
				OptionNormal[j].enabled = true;
			}
		}
	}

	private void UpdateUI()
	{
		if (PauseState != State.Resume)
		{
			Selector.anchoredPosition = new Vector3(Selector.anchoredPosition.x, (PauseState == State.StartOverOptions || (PauseState == State.StartOver && !HUD.StageManager.FirstSection)) ? RestartOptionsPos[OptionSelector].anchoredPosition.y : OptionsPos[OptionSelector].anchoredPosition.y);
		}
	}

	private void StateOpenStart()
	{
		PauseState = State.Open;
	}

	private void StateOpen()
	{
		Time.timeScale = 0f;
		if (Time.unscaledTime - OPStartTime > 0.25f)
		{
			StateMachine.ChangeState(StateIdle);
		}
	}

	private void StateOpeningEnd()
	{
	}

	private void StateIdleStart()
	{
		PauseState = State.Idle;
		DoSelector(100, "On Switch");
		OptionSelector = 0;
	}

	private void StateIdle()
	{
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(2, "On Switch");
			if (YAxis > 0f)
			{
				if (OptionSelector < 2)
				{
					OptionSelector++;
				}
				else
				{
					OptionSelector = 0;
				}
			}
			else if (YAxis < 0f)
			{
				if (OptionSelector > 0)
				{
					OptionSelector--;
				}
				else
				{
					OptionSelector = 2;
				}
			}
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") || Singleton<RInput>.Instance.P.GetButtonDown("Start"))
		{
			PauseMenuAnimator.SetTrigger("On Hide");
			StateMachine.ChangeState(StateResume);
			DoSelector(1, "On Hide");
		}
		if (!Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
		{
			return;
		}
		Singleton<GameManager>.Instance.GetStoryData();
		if (OptionSelector == 0 || OptionSelector == 2)
		{
			DoSelector(3, "On Hide");
		}
		else if (OptionSelector == 1)
		{
			if (!CannotRestart)
			{
				DoSelector(3, HUD.StageManager.FirstSection ? "On Hide" : "On Switch");
			}
			else
			{
				Audio.PlayOneShot(AudioClip[4], Audio.volume);
			}
		}
		if (OptionSelector == 0)
		{
			PauseMenuAnimator.SetTrigger("On Hide");
			StateMachine.ChangeState(StateResume);
		}
		else if (OptionSelector == 1)
		{
			if (!CannotRestart)
			{
				if (HUD.StageManager.FirstSection)
				{
					PauseMenuAnimator.SetTrigger("On Hide");
					StateMachine.ChangeState(StateStartOver);
				}
				else
				{
					StateMachine.ChangeState(StateStartOverOptions);
				}
			}
		}
		else if (OptionSelector == 2)
		{
			StateMachine.ChangeState(StateExit);
		}
	}

	private void StateIdleEnd()
	{
	}

	private void StateResumeStart()
	{
		PauseState = State.Resume;
		RStartTime = Time.unscaledTime;
	}

	private void StateResume()
	{
		if (Time.unscaledTime - RStartTime > 0.275f)
		{
			Singleton<GameManager>.Instance.GameState = ((HUD.StageManager._Stage != StageManager.Stage.twn) ? GameManager.State.Playing : GameManager.State.Hub);
			HUD.IsPaused = false;
			HUD.StageManager.BGMPlayer.volume = 1f;
			AudioListener.pause = false;
			Time.timeScale = 1f;
			Object.Destroy(base.gameObject);
		}
	}

	private void StateResumeEnd()
	{
	}

	private void StateStartOverOptionsStart()
	{
		PauseState = State.StartOverOptions;
		PauseMenuAnimator.SetTrigger("On Restart Options");
		OptionSelector = 0;
	}

	private void StateStartOverOptions()
	{
		if (!UsingYAxis && YAxis != 0f)
		{
			UsingYAxis = true;
			DoSelector(2, "On Switch");
			if (YAxis > 0f)
			{
				if (OptionSelector < 1)
				{
					OptionSelector++;
				}
				else
				{
					OptionSelector--;
				}
			}
			else if (YAxis < 0f)
			{
				if (OptionSelector > 0)
				{
					OptionSelector--;
				}
				else
				{
					OptionSelector++;
				}
			}
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
		{
			StateMachine.ChangeState(StateIdle);
			PauseMenuAnimator.SetTrigger("On Restart Close");
			DoSelector(1, "On Switch");
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Start"))
		{
			PauseMenuAnimator.SetTrigger("On Restart Hide");
			StateMachine.ChangeState(StateResume);
			DoSelector(1, "On Hide");
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") || Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
		{
			RestartStage = OptionSelector == 1;
			DoSelector(3, "On Hide");
			PauseMenuAnimator.SetTrigger("On Restart Hide");
			StateMachine.ChangeState(StateStartOver);
		}
	}

	private void StateStartOverOptionsEnd()
	{
	}

	private void StateStartOverStart()
	{
		PauseState = State.StartOver;
		SOStartTime = Time.unscaledTime;
		PauseMenuAnimator.SetTrigger("On Fade Out");
	}

	private void StateStartOver()
	{
		if (Time.unscaledTime - SOStartTime > 0.35f)
		{
			Singleton<GameManager>.Instance.OnStageRestart(RestartStage);
			Time.timeScale = 1f;
			AudioListener.pause = false;
		}
	}

	private void StateStartOverEnd()
	{
	}

	private void StateExitStart()
	{
		PauseState = State.Exit;
		EStartTime = Time.unscaledTime;
		PauseMenuAnimator.SetTrigger("On Hide");
		PauseMenuAnimator.SetTrigger("On Fade Out");
	}

	private void StateExit()
	{
		if (Time.unscaledTime - EStartTime > 0.35f)
		{
			Singleton<GameManager>.Instance.Exit();
			Time.timeScale = 1f;
			AudioListener.pause = false;
		}
	}

	private void StateExitEnd()
	{
	}

	private void DoSelector(int ClipIndex, string TriggerName)
	{
		if (ClipIndex < AudioClip.Length)
		{
			Audio.PlayOneShot(AudioClip[ClipIndex], Audio.volume);
		}
		SelectorAnimator.SetTrigger(TriggerName);
	}
}
