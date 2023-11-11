using System.Collections.Generic;
using UnityEngine;

public class PlayerVoice : ButtonIconBase
{
	public string VoicePattern;

	public int VoiceGroupCount;

	public PlayerVoice MulticastTo;

	internal PlayerBase Player;

	internal List<AudioClip> WaitVoiceClip = new List<AudioClip>();

	private List<AudioClip[]> VoiceClip = new List<AudioClip[]>();

	private int QuoteIndex;

	private void Start()
	{
		if ((bool)MulticastTo)
		{
			MulticastTo.Player = Player;
		}
		LoadVoiceClips();
		LoadWaitVoiceClips();
	}

	public void Play(int Group, bool RandomPlayChance = false, bool RandomMulticast = false, bool Uncut = false)
	{
		if ((bool)Player.HUD.MsgBoxObject)
		{
			return;
		}
		int num = Random.Range(0, VoiceClip[Group].Length + 1);
		if (VoiceClip[Group].Length != 0)
		{
			int num2 = Random.Range(0, VoiceClip[Group].Length + 1);
			if (((RandomPlayChance && num2 > 0) || !RandomPlayChance) && ((RandomMulticast && num != 0) || !RandomMulticast))
			{
				Singleton<AudioManager>.Instance.PlayVoiceClip(Player.VoicesAudio, VoiceClip[Group][0], 1f, Uncut);
			}
		}
		if ((bool)MulticastTo && ((RandomMulticast && num == 0) || !RandomMulticast))
		{
			MulticastTo.Play(Group, RandomPlayChance, (!RandomMulticast) ? true : false, Uncut);
		}
	}

	public void PlayRandom(int Group, bool RandomPlayChance = false, bool RandomMulticast = false)
	{
		if ((bool)Player.HUD.MsgBoxObject)
		{
			return;
		}
		int num = Random.Range(0, VoiceClip[Group].Length + 1);
		if (VoiceClip[Group].Length != 0)
		{
			int num2 = Random.Range(0, VoiceClip[Group].Length + 1);
			if (((RandomPlayChance && num2 > 0) || !RandomPlayChance) && ((RandomMulticast && num != 0) || !RandomMulticast))
			{
				Singleton<AudioManager>.Instance.PlayVoiceClip(Player.VoicesAudio, VoiceClip[Group][Random.Range(0, VoiceClip[Group].Length)]);
			}
		}
		if ((bool)MulticastTo && ((RandomMulticast && num == 0) || !RandomMulticast))
		{
			MulticastTo.PlayRandom(Group, RandomPlayChance, (!RandomMulticast) ? true : false);
		}
	}

	public void PlayGoal()
	{
		if (VoiceClip[11].Length != 0)
		{
			Singleton<AudioManager>.Instance.PlayVoiceClip(Player.VoicesAudio, VoiceClip[11][0]);
		}
		if ((bool)MulticastTo)
		{
			MulticastTo.PlayGoal();
		}
	}

	public void PlayRandom(int[] Group, bool RandomPlayChance = false, bool RandomMulticast = false)
	{
		if ((bool)Player.HUD.MsgBoxObject)
		{
			return;
		}
		int num = Random.Range(0, Group.Length);
		int num2 = Random.Range(0, VoiceClip[Group[num]].Length + 1);
		if (VoiceClip[Group[num]].Length != 0)
		{
			int num3 = Random.Range(0, VoiceClip[Group[num]].Length + 1);
			if (((RandomPlayChance && num3 > 0) || !RandomPlayChance) && ((RandomMulticast && num2 != 0) || !RandomMulticast))
			{
				Singleton<AudioManager>.Instance.PlayVoiceClip(Player.VoicesAudio, VoiceClip[Group[num]][Random.Range(0, VoiceClip[Group[num]].Length)]);
			}
		}
		if ((bool)MulticastTo && ((RandomMulticast && num2 == 0) || !RandomMulticast))
		{
			MulticastTo.PlayRandom(Group, RandomPlayChance, (!RandomMulticast) ? true : false);
		}
	}

	private void LoadVoiceClips()
	{
		string text = "Win32-Xenon/sound/player/player_" + Player.PlayerName + "/" + Singleton<Settings>.Instance.AudioLanguage() + "/";
		for (int i = 0; i < VoiceGroupCount; i++)
		{
			string voicePattern = VoicePattern;
			voicePattern = voicePattern.Replace("[g]", i.ToString("D2"));
			List<AudioClip> list = new List<AudioClip>();
			int num = 0;
			while (true)
			{
				string text2 = ((num != 0) ? voicePattern.Replace("[gc]", (num + 1).ToString()) : voicePattern.Replace("[gc]", ""));
				AudioClip audioClip = Resources.Load<AudioClip>(text + text2);
				if (!(audioClip != null))
				{
					break;
				}
				list.Add(audioClip);
				num++;
			}
			VoiceClip.Add(list.ToArray());
		}
	}

	public void PlayWait()
	{
		if (Player.StageManager.StageState == StageManager.State.Event)
		{
			return;
		}
		if ((bool)MulticastTo && MulticastTo.WaitVoiceClip.Count != 0)
		{
			QuoteIndex++;
			if (QuoteIndex > 1)
			{
				QuoteIndex = 0;
			}
			if (QuoteIndex < 1 || WaitVoiceClip.Count == 0)
			{
				MulticastTo.PlayWait();
			}
		}
		if (WaitVoiceClip.Count != 0 && (!MulticastTo || ((bool)MulticastTo && ((MulticastTo.WaitVoiceClip.Count != 0 && QuoteIndex > 0) || MulticastTo.WaitVoiceClip.Count == 0))))
		{
			string message = "hint_" + Player.StageManager._Stage.ToString() + "01_w0" + (int)Player.StageManager.StageSection + "_" + VoicePattern.Split("_"[0])[2].Split("["[0])[0];
			Player.HUD.StartMessageBox(GetText(message), GetSound(message));
		}
	}

	public void PlayGlobalWait(string ClipName)
	{
		if (Player.StageManager.StageState != StageManager.State.Event)
		{
			Player.HUD.StartMessageBox(GetText(ClipName), GetSound(ClipName));
		}
	}

	private void LoadWaitVoiceClips()
	{
		string text = "Win32-Xenon/sound/voice/" + Singleton<Settings>.Instance.AudioLanguage() + "/";
		for (int i = 0; i < 1; i++)
		{
			string text2 = Player.StageManager._Stage.ToString() + "01_w[gc]_" + VoicePattern.Split("_"[0])[2].Split("["[0])[0];
			List<AudioClip> list = new List<AudioClip>();
			string text3 = text2.Replace("[gc]", "0" + (int)Player.StageManager.StageSection);
			AudioClip audioClip = Resources.Load<AudioClip>(text + text3);
			if (audioClip != null)
			{
				list.Add(audioClip);
			}
			WaitVoiceClip = list;
		}
	}
}
