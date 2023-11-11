using System.Collections.Generic;
using UnityEngine;

public class FlagCollector : EventStation
{
	[Header("Framework")]
	public List<string> Flags;

	public string EventName;

	private void Start()
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		List<int> list = new List<int>();
		for (int i = 0; i < Flags.Count; i++)
		{
			if (gameData.HasFlag(Flags[i]))
			{
				list.Add(0);
			}
		}
		if (EventName != "" && list.Count >= Flags.Count)
		{
			CallEvent(EventName);
		}
	}
}
