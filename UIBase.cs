using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
	public static Color32[] SonicBaseRetail = new Color32[4]
	{
		new Color32(12, 30, 75, byte.MaxValue),
		new Color32(12, 30, 75, byte.MaxValue),
		new Color32(150, 180, 213, byte.MaxValue),
		new Color32(180, 222, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] SonicBaseE3 = new Color32[4]
	{
		new Color32(12, 31, 54, byte.MaxValue),
		new Color32(12, 31, 54, byte.MaxValue),
		new Color32(159, 190, 219, byte.MaxValue),
		new Color32(159, 190, 219, byte.MaxValue)
	};

	public static Color32[] SonicOutlineRetail = new Color32[4]
	{
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] SonicOutlineE3 = new Color32[4]
	{
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue)
	};

	public static Color32 SonicActionGauge = new Color32(109, 247, 248, byte.MaxValue);

	public static Color32 SonicActionGaugeGlow = new Color32(184, 85, 85, 0);

	public static Color32[] ShadowBaseRetail = new Color32[4]
	{
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(80, 40, 40, byte.MaxValue),
		new Color32(132, 83, 83, byte.MaxValue)
	};

	public static Color32[] ShadowOutlineRetail = new Color32[4]
	{
		new Color32(195, 60, 50, byte.MaxValue),
		new Color32(195, 60, 50, byte.MaxValue),
		new Color32(195, 60, 50, byte.MaxValue),
		new Color32(195, 60, 50, byte.MaxValue)
	};

	public static Color32 ShadowActionGauge = new Color32(248, 160, 48, byte.MaxValue);

	public static Color32 ShadowActionGaugeGlow = new Color32(184, 105, 65, 0);

	public static Color32[] SilverBaseRetail = new Color32[4]
	{
		new Color32(30, 40, 30, byte.MaxValue),
		new Color32(30, 40, 30, byte.MaxValue),
		new Color32(235, 235, 235, byte.MaxValue),
		new Color32(240, 240, 240, byte.MaxValue)
	};

	public static Color32[] SilverOutlineRetail = new Color32[4]
	{
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue)
	};

	public static Color32 SilverActionGauge = new Color32(109, 247, 198, byte.MaxValue);

	public static Color32 SilverActionGaugeGlow = new Color32(105, 184, 145, 0);

	public static Color32[] SonicBaseRadarmapRetail = new Color32[4]
	{
		new Color32(180, 222, byte.MaxValue, byte.MaxValue),
		new Color32(12, 30, 75, byte.MaxValue),
		new Color32(150, 180, 213, byte.MaxValue),
		new Color32(12, 30, 75, byte.MaxValue)
	};

	public static Color32[] SonicBaseRadarmapE3 = new Color32[4]
	{
		new Color32(159, 190, 219, byte.MaxValue),
		new Color32(12, 31, 54, byte.MaxValue),
		new Color32(159, 190, 219, byte.MaxValue),
		new Color32(12, 31, 54, byte.MaxValue)
	};

	public static Color32[] ShadowBaseRadarmapRetail = new Color32[4]
	{
		new Color32(132, 83, 83, byte.MaxValue),
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(80, 40, 40, byte.MaxValue),
		new Color32(0, 0, 0, byte.MaxValue)
	};

	public static Color32[] SilverBaseRadarmapRetail = new Color32[4]
	{
		new Color32(240, 240, 240, byte.MaxValue),
		new Color32(30, 40, 30, byte.MaxValue),
		new Color32(235, 235, 235, byte.MaxValue),
		new Color32(30, 40, 30, byte.MaxValue)
	};

	public static Color32[] SonicTextBoxBG = new Color32[4]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 113),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 144),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194)
	};

	public static Color32[] SonicTextBoxOutline = new Color32[4]
	{
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue),
		new Color32(130, 206, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] SonicTextBoxBase = new Color32[4]
	{
		new Color32(32, 88, 160, 180),
		new Color32(152, 180, 208, byte.MaxValue),
		new Color32(152, 180, 208, 180),
		new Color32(32, 88, 160, byte.MaxValue)
	};

	public static Color32[] ShadowTextBoxBG = new Color32[4]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 113),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 144),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194)
	};

	public static Color32[] ShadowTextBoxOutline = new Color32[4]
	{
		new Color32(205, 70, 60, byte.MaxValue),
		new Color32(205, 70, 60, byte.MaxValue),
		new Color32(205, 70, 60, byte.MaxValue),
		new Color32(205, 70, 60, byte.MaxValue)
	};

	public static Color32[] ShadowTextBoxBase = new Color32[4]
	{
		new Color32(20, 20, 20, 180),
		new Color32(142, 93, 93, byte.MaxValue),
		new Color32(142, 93, 93, 180),
		new Color32(20, 20, 20, byte.MaxValue)
	};

	public static Color32[] SilverTextBoxBG = new Color32[4]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 113),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 144),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 194)
	};

	public static Color32[] SilverTextBoxOutline = new Color32[4]
	{
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue),
		new Color32(141, 217, 184, byte.MaxValue)
	};

	public static Color32[] SilverTextBoxBase = new Color32[4]
	{
		new Color32(70, 75, 70, 180),
		new Color32(192, 192, 192, byte.MaxValue),
		new Color32(192, 192, 192, 180),
		new Color32(70, 75, 70, byte.MaxValue)
	};

	public static Color32 SonicPauseSelector = new Color32(136, 158, 214, byte.MaxValue);

	public static Color32 SonicPauseSelectorGlow = new Color32(0, byte.MaxValue, byte.MaxValue, 0);

	public static Color32 SonicPauseE3BG = new Color32(162, 211, byte.MaxValue, 166);

	public static Color32 SonicPauseE3LightPanelBG = new Color32(92, 124, 155, byte.MaxValue);

	public static Color32 SonicPauseE3LightPanel = new Color32(192, 224, byte.MaxValue, byte.MaxValue);

	public static Color32 ShadowPauseSelector = new Color32(214, 158, 150, byte.MaxValue);

	public static Color32 ShadowPauseSelectorGlow = new Color32(byte.MaxValue, 0, 0, 0);

	public static Color32 ShadowPauseE3BG = new Color32(231, 156, 157, 166);

	public static Color32 ShadowPauseE3LightPanelBG = new Color32(155, 104, 92, byte.MaxValue);

	public static Color32 ShadowPauseE3LightPanel = new Color32(byte.MaxValue, 204, 192, byte.MaxValue);

	public static Color32 SilverPauseSelector = new Color32(160, 214, 210, byte.MaxValue);

	public static Color32 SilverPauseSelectorGlow = new Color32(0, byte.MaxValue, 224, 0);

	public static Color32 SilverPauseE3BG = new Color32(223, byte.MaxValue, 245, 166);

	public static Color32 SilverPauseE3LightPanelBG = new Color32(123, 145, 143, byte.MaxValue);

	public static Color32 SilverPauseE3LightPanel = new Color32(223, 245, 243, byte.MaxValue);

	public Color32 BaseColor(int Index)
	{
		Color32 result = ((Singleton<Settings>.Instance.settings.DisplayType == 0) ? SonicBaseRetail[Index] : SonicBaseE3[Index]);
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowBaseRetail[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverBaseRetail[Index];
		}
		return result;
	}

	public Color32 OutlineColor(int Index)
	{
		Color32 result = ((Singleton<Settings>.Instance.settings.DisplayType == 0) ? SonicOutlineRetail[Index] : SonicOutlineE3[Index]);
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowOutlineRetail[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverOutlineRetail[Index];
		}
		return result;
	}

	public Color32 ActionGaugeColor()
	{
		Color32 result = SonicActionGauge;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowActionGauge;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverActionGauge;
		}
		return result;
	}

	public Color32 ActionGaugeGlowColor()
	{
		Color32 result = SonicActionGaugeGlow;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowActionGaugeGlow;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverActionGaugeGlow;
		}
		return result;
	}

	public Color32 TextBoxBGColor(int Index)
	{
		Color32 result = SonicTextBoxBG[Index];
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowTextBoxBG[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverTextBoxBG[Index];
		}
		return result;
	}

	public Color32 TextBoxOutlineColor(int Index)
	{
		Color32 result = SonicTextBoxOutline[Index];
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowTextBoxOutline[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverTextBoxOutline[Index];
		}
		return result;
	}

	public Color32 TextBoxBaseColor(int Index)
	{
		Color32 result = SonicTextBoxBase[Index];
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowTextBoxBase[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverTextBoxBase[Index];
		}
		return result;
	}

	public Color32 BaseRadarmapColor(int Index)
	{
		Color32 result = ((Singleton<Settings>.Instance.settings.DisplayType == 0) ? SonicBaseRadarmapRetail[Index] : SonicBaseRadarmapE3[Index]);
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowBaseRadarmapRetail[Index];
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverBaseRadarmapRetail[Index];
		}
		return result;
	}

	public Color32 PauseSelectorColor()
	{
		Color32 result = SonicPauseSelector;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowPauseSelector;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverPauseSelector;
		}
		return result;
	}

	public Color32 PauseSelectorGlowColor()
	{
		Color32 result = SonicPauseSelectorGlow;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowPauseSelectorGlow;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverPauseSelectorGlow;
		}
		return result;
	}

	public Color32 PauseE3BGColor()
	{
		Color32 result = SonicPauseE3BG;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowPauseE3BG;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverPauseE3BG;
		}
		return result;
	}

	public Color32 PauseE3LightPanelBGColor()
	{
		Color32 result = SonicPauseE3LightPanelBG;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowPauseE3LightPanelBG;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverPauseE3LightPanelBG;
		}
		return result;
	}

	public Color32 PauseE3LightPanelColor()
	{
		Color32 result = SonicPauseE3LightPanel;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowPauseE3LightPanel;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverPauseE3LightPanel;
		}
		return result;
	}

	public int GetFlagCount()
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		string text = SceneManager.GetActiveScene().name;
		string text2 = text.Split('_')[0];
		string text3 = text.Split('_')[2];
		List<int> list = new List<int>();
		if (gameData.Flags != null)
		{
			foreach (string flag in gameData.Flags)
			{
				if (flag.Contains(Game.MedalStageID[text2 + "_" + text3]))
				{
					list.Add(1);
				}
			}
		}
		return list.Count;
	}

	public void Counter(string Number, Image[] Dest, Sprite[] Numbers)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		if (Dest.Length == 0 || Dest.Length >= 9)
		{
			MonoBehaviour.print("Set the array size " + Number.Length);
		}
		else if (Dest.Length == 1)
		{
			num = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
		}
		else if (Dest.Length == 2)
		{
			num = int.Parse(Number[1].ToString());
			num2 = int.Parse(Number[0].ToString());
			Dest[1].sprite = Numbers[num];
			Dest[0].sprite = Numbers[num2];
		}
		else if (Dest.Length == 3)
		{
			num = int.Parse(Number[2].ToString());
			num2 = int.Parse(Number[1].ToString());
			num3 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
		}
		else if (Dest.Length == 4)
		{
			num = int.Parse(Number[3].ToString());
			num2 = int.Parse(Number[2].ToString());
			num3 = int.Parse(Number[1].ToString());
			num4 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
			Dest[3].sprite = Numbers[num4];
		}
		else if (Dest.Length == 5)
		{
			num = int.Parse(Number[4].ToString());
			num2 = int.Parse(Number[3].ToString());
			num3 = int.Parse(Number[2].ToString());
			num4 = int.Parse(Number[1].ToString());
			num5 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
			Dest[3].sprite = Numbers[num4];
			Dest[4].sprite = Numbers[num5];
		}
		else if (Dest.Length == 6)
		{
			num = int.Parse(Number[5].ToString());
			num2 = int.Parse(Number[4].ToString());
			num3 = int.Parse(Number[3].ToString());
			num4 = int.Parse(Number[2].ToString());
			num5 = int.Parse(Number[1].ToString());
			num6 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
			Dest[3].sprite = Numbers[num4];
			Dest[4].sprite = Numbers[num5];
			Dest[5].sprite = Numbers[num6];
		}
		else if (Dest.Length == 7)
		{
			num = int.Parse(Number[6].ToString());
			num2 = int.Parse(Number[5].ToString());
			num3 = int.Parse(Number[4].ToString());
			num4 = int.Parse(Number[3].ToString());
			num5 = int.Parse(Number[2].ToString());
			num6 = int.Parse(Number[1].ToString());
			num7 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
			Dest[3].sprite = Numbers[num4];
			Dest[4].sprite = Numbers[num5];
			Dest[5].sprite = Numbers[num6];
			Dest[6].sprite = Numbers[num7];
		}
		else if (Dest.Length == 8)
		{
			num = int.Parse(Number[7].ToString());
			num2 = int.Parse(Number[6].ToString());
			num3 = int.Parse(Number[5].ToString());
			num4 = int.Parse(Number[4].ToString());
			num5 = int.Parse(Number[3].ToString());
			num6 = int.Parse(Number[2].ToString());
			num7 = int.Parse(Number[1].ToString());
			num8 = int.Parse(Number[0].ToString());
			Dest[0].sprite = Numbers[num];
			Dest[1].sprite = Numbers[num2];
			Dest[2].sprite = Numbers[num3];
			Dest[3].sprite = Numbers[num4];
			Dest[4].sprite = Numbers[num5];
			Dest[5].sprite = Numbers[num6];
			Dest[6].sprite = Numbers[num7];
			Dest[7].sprite = Numbers[num8];
		}
	}
}
