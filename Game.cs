using System.Collections.Generic;
using UnityEngine;

public static class Game
{
	public static string TitleBarName = "Sonic the Hedgehog: Nights of Kronos";

	public static Dictionary<string, string> MedalStageID = new Dictionary<string, string>
	{
		{ "wvo_sn", "503" },
		{ "dtd_sn", "504" },
		{ "wap_sn", "505" },
		{ "csc_sn", "506" },
		{ "flc_sn", "507" },
		{ "rct_sn", "508" },
		{ "tpj_sn", "509" },
		{ "kdv_sn", "510" },
		{ "aqa_sn", "511" },
		{ "wvo_tl", "512" },
		{ "wap_sd", "516" },
		{ "kdv_sd", "517" },
		{ "csc_sd", "518" },
		{ "flc_sd", "519" },
		{ "rct_sd", "520" },
		{ "aqa_sd", "521" },
		{ "wvo_sd", "522" },
		{ "dtd_sd", "523" },
		{ "tpj_rg", "524" },
		{ "csc_sv", "528" },
		{ "tpj_sv", "529" },
		{ "dtd_sv", "530" },
		{ "wap_sv", "531" },
		{ "rct_sv", "532" },
		{ "aqa_sv", "533" },
		{ "kdv_sv", "534" },
		{ "flc_sv", "535" },
		{ "wvo_bz", "536" }
	};

	public static string MenuLoadMode = "MenuLoadMode";

	public static string AutoSaveMode = "AutoSaveMode";

	public static string BlankLoadMode = "BlankLoadMode";

	public static string GoldMedalScene = "GoldMedalScreen";

	public static string CreditsScene = "Credits";

	public static string CreditsPlayed = "credits_played";

	public static string StgSnAllClear = "stg_sn_all_clear";

	public static string StgSdAllClear = "stg_sd_all_clear";

	public static string StgSvAllClear = "stg_sv_all_clear";

	public static string GotSnReward = "got_sn_reward";

	public static string GotSdReward = "got_sd_reward";

	public static string GotSvReward = "got_sv_reward";

	public static string MemoryShardLight = "equip_shadow_boost_lv4";

	public static string LotusOfResilience = "equip_silver_upgrade_lv1";

	public static string FlameOfControl = "equip_silver_upgrade_lv2";

	public static string SigilOfAwakening = "equip_silver_upgrade_lv3";

	public static void NewActor(string PrefabName, object Class = null)
	{
		(Object.Instantiate(Resources.Load(PrefabName), Vector3.zero, Quaternion.identity) as GameObject).SendMessage("OnActorCreate", Class, SendMessageOptions.DontRequireReceiver);
	}

	public static void Signal(string Object)
	{
		if ((bool)GameObject.Find(Object))
		{
			GameObject.Find(Object).SendMessage("OnEventSignal", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void ChangeArea(string Area, string Mode = "")
	{
		Singleton<GameManager>.Instance.OnChangeSection();
		Singleton<GameManager>.Instance.SetLoadingTo(Area, Mode);
	}

	public static void StartEntityByName(string Entity)
	{
		if ((bool)GameObject.Find(Entity))
		{
			GameObject.Find(Entity).SendMessage("StartGroup", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void ProcessMessage(string Object, string Message, object Class = null)
	{
		if ((bool)GameObject.Find(Object))
		{
			GameObject.Find(Object).SendMessage(Message, Class, SendMessageOptions.DontRequireReceiver);
		}
	}

	public static void Result()
	{
		if ((bool)GameObject.FindGameObjectWithTag("Player"))
		{
			GameObject.FindGameObjectWithTag("Player").SendMessage("OnGoal", SendMessageOptions.DontRequireReceiver);
		}
	}

	public static string FormatTime(float Time)
	{
		int num = Mathf.FloorToInt(Time / 60f);
		int num2 = Mathf.FloorToInt(Time - (float)num * 60f);
		int num3 = Mathf.FloorToInt(Time * 1000f % 1000f);
		return $"{num:00}'{num2:00}\"{num3:000}";
	}
}
