using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class MSTManager : Singleton<MSTManager>
{
	public const string TextPath = "Win32-Xenon\\text\\english";

	private Dictionary<string, TextAsset> Files = new Dictionary<string, TextAsset>();

	public Dictionary<string, Message> Hint;

	public Dictionary<string, Message> HintXenon;

	public Dictionary<string, Message> HintPs3;

	public Dictionary<string, Message> HintOptionsDemo;

	public Dictionary<string, Message> System = new Dictionary<string, Message>();

	protected MSTManager()
	{
	}

	public void StartMSTManager()
	{
	}

	private void Awake()
	{
		Object[] array = Resources.LoadAll("Win32-Xenon\\text\\english", typeof(TextAsset));
		for (int i = 0; i < array.Length; i++)
		{
			TextAsset textAsset = (TextAsset)array[i];
			Files.Add(textAsset.name, textAsset);
		}
		Hint = AsDictionary("msg_hint.e");
		HintXenon = AsDictionary("msg_hint_xenon.e");
		HintPs3 = AsDictionary("msg_hint_ps3.e");
		HintOptionsDemo = AsDictionary("msg_options_demo.e");
		System = AsDictionary("msg_system.e");
	}

	public string GetText(string TextName)
	{
		return Hint[TextName].Text.Remove(0, 1);
	}

	public string GetSystem(string TextName, bool ToUpper)
	{
		if (!ToUpper)
		{
			return System[TextName].Text;
		}
		return System[TextName].Text.ToUpper();
	}

	private Dictionary<string, Message> AsDictionary(string FileName)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(Files[FileName].text);
		Dictionary<string, Message> dictionary = new Dictionary<string, Message>();
		foreach (XmlNode item2 in xmlDocument.DocumentElement.SelectNodes("message"))
		{
			Message message = new Message();
			message.Name = item2.Attributes["name"].Value;
			message.Text = item2.InnerText;
			if (item2.Attributes["placeholder"] != null)
			{
				string[] array = item2.Attributes["placeholder"].Value.Split(',');
				foreach (string item in array)
				{
					message.Placeholders.Add(item);
				}
			}
			dictionary.Add(message.Name, message);
		}
		return dictionary;
	}
}
