using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
	public StateMachine StateMachine;

	public Animator TitleScreenAnimator;

	public AudioSource MusicAudio;

	public AudioSource Audio;

	public AudioClip[] Clips;

	[Header("Save Select")]
	public Animator SaveSelectAnimator;

	public SaveSlotUI[] Slots;

	public GameObject[] SaveIcons;

	private float StartTime;

	private float XAxis;

	private float YAxis;

	private float AxisXTime;

	private float AxisYTime;

	private int Index;

	private bool UsingXAxis;

	private bool UsingYAxis;

	private bool StartXScrolling;

	private bool StartYScrolling;

	private bool FastXScroll;

	private bool FastYScroll;

	private bool ShowSaveSelect;

	private bool ShowMenuSelect;

	private bool MenuSelected;

	private List<GameData.SaveData> Datas;

	private bool GoToMainMenu;

	private bool CreatedSave;

	private int SaveSelectState;

	private int SaveSelectIndex;

	private int OptionSelectIndex;

	private void Start()
	{
		StateIdleStart();
		StateMachine.Initialize(StateIdle);
	}

	private void Update()
	{
		StateMachine.UpdateStateMachine();
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

	private void StateIdleStart()
	{
		StartTime = Time.time;
		ShowSaveSelect = false;
	}

	private void StateIdle()
	{
		if (!ShowSaveSelect && Time.time - StartTime > 30f)
		{
			StateMachine.ChangeState(StateToVideo);
		}
		if (!ShowSaveSelect && Time.time - StartTime > 0.4f && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
		{
			Audio.pitch = 0.9f;
			Audio.PlayOneShot(Clips[0], Audio.volume);
			TitleScreenAnimator.SetTrigger("On Press");
			StartTime = Time.time;
			ShowSaveSelect = true;
		}
		if (ShowSaveSelect && Time.time - StartTime > 1f)
		{
			StateMachine.ChangeState(StateStarter);
		}
	}

	private void StateIdleEnd()
	{
	}

	private void StateMenuStart()
	{
		StartTime = Time.time;
		Index = 0;
		TitleScreenAnimator.SetInteger("Menu Index", Index);
		TitleScreenAnimator.SetTrigger("On Menu");
		MenuSelected = false;
	}

	private void StateMenu()
	{
		if (!MenuSelected)
		{
			if (!UsingYAxis && YAxis != 0f)
			{
				UsingYAxis = true;
				Audio.pitch = 1f;
				Audio.PlayOneShot(Clips[2], Audio.volume);
				if (YAxis < 0f)
				{
					if (Index > 0)
					{
						Index--;
					}
					else
					{
						Index = 1;
					}
				}
				if (YAxis > 0f)
				{
					if (Index < 1)
					{
						Index++;
					}
					else
					{
						Index = 0;
					}
				}
				TitleScreenAnimator.SetInteger("Menu Index", Index);
				TitleScreenAnimator.SetTrigger("On Menu");
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				if (Index == 0)
				{
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[5], Audio.volume);
					TitleScreenAnimator.SetTrigger("On Menu New Game Press");
				}
				else
				{
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[4], Audio.volume);
					TitleScreenAnimator.SetTrigger("On Menu Continue Press");
				}
				StartTime = Time.time;
				MenuSelected = true;
			}
			return;
		}
		if (Index == 0)
		{
			if (Time.time - StartTime > 0.75f)
			{
				StateMachine.ChangeState(StateStarter);
			}
			return;
		}
		if (!GoToMainMenu && Time.time - StartTime > 0.75f)
		{
			StartTime = Time.time;
			TitleScreenAnimator.SetTrigger("On Menu Continue");
			GoToMainMenu = true;
		}
		if (GoToMainMenu)
		{
			if (Time.time - StartTime > 1f)
			{
				MusicAudio.volume = Mathf.Lerp(MusicAudio.volume, 0f, Time.deltaTime * 2.5f);
			}
			if (Time.time - StartTime > 3f)
			{
				SceneManager.LoadScene("MainMenu");
			}
		}
	}

	private void StateMenuEnd()
	{
	}

	private void StateStarterStart()
	{
		SaveSelectAnimator.SetTrigger("On Show");
		Datas = new List<GameData.SaveData>();
		for (int i = 1; i < 6; i++)
		{
			string text = GameData.GetSavesPath() + GameData.GetSaveFile(i);
			if (File.Exists(text))
			{
				GameData.SaveData saveData = Helper.LoadClass<GameData.SaveData>(text);
				Datas.Add(saveData);
				Slots[i - 1].SetUp(saveData.game.Playtime, saveData.game.LastSave, saveData.game.Banner, saveData.version);
			}
			else
			{
				Datas.Add(null);
				Slots[i - 1].SetUpEmpty();
			}
		}
		for (int j = 0; j < Slots.Length; j++)
		{
			Slots[j].Animator.SetBool("Selected", value: false);
		}
		StartTime = Time.time;
		SaveSelectState = 0;
		SaveSelectIndex = 0;
		OptionSelectIndex = 0;
	}

	private void StateStarter()
	{
		if (SaveSelectState == -1)
		{
			if (Time.time - StartTime > 0.5f)
			{
				if (!MenuSelected)
				{
					StateMachine.ChangeState(StateIdle);
				}
				else
				{
					StateMachine.ChangeState(StateMenu);
				}
			}
		}
		else if (SaveSelectState == 0)
		{
			if (!(Time.time - StartTime > 0.5f))
			{
				return;
			}
			if (!UsingYAxis && YAxis != 0f)
			{
				UsingYAxis = true;
				Audio.pitch = 1f;
				Audio.PlayOneShot(Clips[2], Audio.volume);
				if (YAxis < 0f)
				{
					if (SaveSelectIndex > 0)
					{
						SaveSelectIndex--;
					}
					else
					{
						SaveSelectIndex = 4;
					}
				}
				if (YAxis > 0f)
				{
					if (SaveSelectIndex < 4)
					{
						SaveSelectIndex++;
					}
					else
					{
						SaveSelectIndex = 0;
					}
				}
			}
			Singleton<GameData>.Instance.SaveIndex = SaveSelectIndex + 1;
			for (int i = 0; i < Slots.Length; i++)
			{
				Slots[i].Animator.SetBool("Selected", i == SaveSelectIndex);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				SaveSelectAnimator.SetTrigger("On End");
				Audio.pitch = 1f;
				Audio.PlayOneShot(Clips[1], Audio.volume);
				StartTime = Time.time;
				SaveSelectState = -1;
				for (int j = 0; j < Slots.Length; j++)
				{
					Slots[j].Animator.SetBool("Selected", value: false);
				}
			}
			if (!Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				return;
			}
			if (Datas[SaveSelectIndex] != null)
			{
				if (Datas[SaveSelectIndex].version == Application.version)
				{
					Singleton<GameData>.Instance.LoadGameData();
					SaveSelectAnimator.SetTrigger("On End");
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[4], Audio.volume);
					StartTime = Time.time;
					SaveSelectState = 2;
				}
				else
				{
					Audio.PlayOneShot(Clips[1], Audio.volume);
				}
			}
			else
			{
				SaveSelectAnimator.SetTrigger("On Option Show");
				Audio.pitch = 1f;
				Audio.PlayOneShot(Clips[3], Audio.volume);
				StartTime = Time.time;
				OptionSelectIndex = 0;
				SaveSelectState = 1;
			}
		}
		else if (SaveSelectState == 1)
		{
			if (!CreatedSave)
			{
				if (!(Time.time - StartTime > 0.5f))
				{
					return;
				}
				if (!UsingXAxis && XAxis != 0f)
				{
					UsingXAxis = true;
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[2], Audio.volume);
					if (XAxis < 0f)
					{
						if (OptionSelectIndex > 0)
						{
							OptionSelectIndex--;
						}
						else
						{
							OptionSelectIndex = SaveIcons.Length - 1;
						}
					}
					if (XAxis > 0f)
					{
						if (OptionSelectIndex < SaveIcons.Length - 1)
						{
							OptionSelectIndex++;
						}
						else
						{
							OptionSelectIndex = 0;
						}
					}
				}
				for (int k = 0; k < SaveIcons.Length; k++)
				{
					SaveIcons[k].SetActive(k == OptionSelectIndex);
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
				{
					SaveSelectAnimator.SetTrigger("On Option End");
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[4], Audio.volume);
					StartTime = Time.time;
					Singleton<GameData>.Instance.CreateGameData(OptionSelectIndex);
					GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
					Slots[SaveSelectIndex].SetUp(gameData.Playtime, gameData.LastSave, OptionSelectIndex, Application.version);
					CreatedSave = true;
				}
				if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
				{
					SaveSelectAnimator.SetTrigger("On Option End");
					Audio.pitch = 1f;
					Audio.PlayOneShot(Clips[1], Audio.volume);
					StartTime = Time.time;
					SaveSelectState = 0;
				}
			}
			else if (SaveSelectState == 1 && (double)(Time.time - StartTime) > 0.75)
			{
				SaveSelectAnimator.SetTrigger("On End");
				StartTime = Time.time;
				SaveSelectState = 2;
			}
		}
		else
		{
			if (SaveSelectState != 2)
			{
				return;
			}
			if (!GoToMainMenu && Time.time - StartTime > 0.75f)
			{
				StartTime = Time.time;
				if (!MenuSelected)
				{
					TitleScreenAnimator.SetTrigger("On Close");
				}
				else
				{
					TitleScreenAnimator.SetTrigger("On Menu New Game");
				}
				GoToMainMenu = true;
			}
			if (GoToMainMenu)
			{
				if (Time.time - StartTime > 1f)
				{
					MusicAudio.volume = Mathf.Lerp(MusicAudio.volume, 0f, Time.deltaTime * 2.5f);
				}
				if (Time.time - StartTime > 3f)
				{
					SceneManager.LoadScene("MainMenu");
				}
			}
		}
	}

	private void StateStarterEnd()
	{
	}

	private void StateToVideoStart()
	{
		TitleScreenAnimator.SetTrigger("On Video");
		StartTime = Time.time;
	}

	private void StateToVideo()
	{
		MusicAudio.volume = Mathf.Lerp(MusicAudio.volume, 0f, Time.deltaTime * 3.5f);
		if (Time.time - StartTime > 2f)
		{
			SceneManager.LoadScene("TitleVideo");
		}
	}

	private void StateToVideoEnd()
	{
	}
}
