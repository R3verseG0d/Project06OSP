using System.Collections.Generic;

public class ButtonIconBase : ObjectBase
{
	public string[] GetText(string Message)
	{
		Message message = (Singleton<MSTManager>.Instance.Hint.ContainsKey(Message) ? Singleton<MSTManager>.Instance.Hint[Message] : ((Singleton<Settings>.Instance.settings.ButtonIcons != 0) ? (Singleton<MSTManager>.Instance.HintPs3.ContainsKey(Message) ? Singleton<MSTManager>.Instance.HintPs3[Message] : Singleton<MSTManager>.Instance.Hint["hint_msg_missing"]) : (Singleton<MSTManager>.Instance.HintXenon.ContainsKey(Message) ? Singleton<MSTManager>.Instance.HintXenon[Message] : Singleton<MSTManager>.Instance.Hint["hint_msg_missing"])));
		string[] array = message.Text.Replace("\\f", "\f").Replace("\\n", "\n").Split('\f');
		List<string> list = new List<string>();
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Remove(0, 1);
			List<string> list2 = new List<string>();
			for (int j = 0; j < message.Placeholders.Count; j++)
			{
				if (message.Placeholders[j].Contains("picture"))
				{
					list2.Add(message.Placeholders[j]);
				}
			}
			if (list2.Count != 0)
			{
				for (int k = 0; k < text.Length; k++)
				{
					if (text[k] == '$')
					{
						num++;
						text = text.Remove(k, 1).Insert(k, ButtonText(list2[num]));
					}
				}
			}
			list.Add(text);
		}
		return list.ToArray();
	}

	public string[] GetSound(string Message)
	{
		Message message = (Singleton<MSTManager>.Instance.Hint.ContainsKey(Message) ? Singleton<MSTManager>.Instance.Hint[Message] : ((Singleton<Settings>.Instance.settings.ButtonIcons != 0) ? (Singleton<MSTManager>.Instance.HintPs3.ContainsKey(Message) ? Singleton<MSTManager>.Instance.HintPs3[Message] : Singleton<MSTManager>.Instance.Hint["hint_msg_missing"]) : (Singleton<MSTManager>.Instance.HintXenon.ContainsKey(Message) ? Singleton<MSTManager>.Instance.HintXenon[Message] : Singleton<MSTManager>.Instance.Hint["hint_msg_missing"])));
		if (message.Placeholders != null)
		{
			string[] array = message.Placeholders.ToArray();
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Contains("sound"))
				{
					list.Add(message.Placeholders[i].Replace("sound(", "").Replace(")", ""));
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			return list.ToArray();
		}
		return null;
	}

	private string ButtonText(string Picture)
	{
		if (Singleton<Settings>.Instance.settings.ButtonIcons == 0)
		{
			switch (Picture)
			{
			case "picture(button_a)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#28DD00>A</color>";
				}
				return "<sprite=0>";
			case "picture(button_b)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#DA0008>B</color>";
				}
				return "<sprite=1>";
			case "picture(button_x)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#0020DD>X</color>";
				}
				return "<sprite=2>";
			case "picture(button_y)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#FFC000>Y</color>";
				}
				return "<sprite=3>";
			case "picture(button_rt)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#FFC000>R</color>";
				}
				return "<sprite=4>";
			case "picture(button_lt)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#FFC000>L</color>";
				}
				return "<sprite=5>";
			}
		}
		else
		{
			switch (Picture)
			{
			case "picture(button_a)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#0020DD>X</color>";
				}
				return "<sprite=6>";
			case "picture(button_b)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#DA0008>O</color>";
				}
				return "<sprite=7>";
			case "picture(button_x)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<sprite=0>";
				}
				return "<sprite=8>";
			case "picture(button_y)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<sprite=1>";
				}
				return "<sprite=9>";
			case "picture(button_rt)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#FFC000>R2</color>";
				}
				return "<sprite=10>";
			case "picture(button_lt)":
				if (Singleton<Settings>.Instance.settings.TextBoxType != 0)
				{
					return "<color=#FFC000>L2</color>";
				}
				return "<sprite=11>";
			}
		}
		return "$";
	}
}
