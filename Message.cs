using System;
using System.Collections.Generic;

[Serializable]
public class Message
{
	public string Name;

	public string Text;

	public List<string> Placeholders = new List<string>();
}
