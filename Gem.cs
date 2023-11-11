using UnityEngine;

public class Gem : ButtonIconBase
{
	public enum Type
	{
		Blue = 0,
		Red = 1,
		Green = 2,
		Purple = 3,
		Sky = 4,
		White = 5,
		Yellow = 6,
		Rainbow = 7
	}

	[Header("Framework")]
	public Type GemType;

	public Renderer Renderer;

	public Transform Mesh;

	public Material[] Colors;

	public GameObject[] FX;

	public GameObject GemGetFX;

	private bool Collected;

	private string GemID;

	private float YLerp;

	private void Start()
	{
		if (HasGem(GemType.ToString()))
		{
			Object.Destroy(base.gameObject);
		}
		Renderer.material = Colors[(int)GemType];
		for (int i = 0; i < FX.Length; i++)
		{
			if (i == (int)GemType)
			{
				FX[i].SetActive(value: true);
			}
		}
	}

	private void Update()
	{
		if (!Collected)
		{
			YLerp = Mathf.Lerp(YLerp, Mathf.PingPong(Time.time / 5f, 0.3f) - 0.15f, Time.deltaTime * 2.5f);
			Mesh.localPosition = new Vector3(Mesh.localPosition.x, YLerp, Mesh.localPosition.z);
			Mesh.Rotate(0f, 150f * Time.deltaTime, 0f);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && player.GetPrefab("sonic_new") && !Collected)
		{
			SetGemBool(GemType.ToString(), player.PlayerManager);
			player.HUD.StartMessageBox(GetText(GemType.ToString()), null, 8f);
			Mesh.gameObject.SetActive(value: false);
			GemGetFX.SetActive(value: true);
			Object.Destroy(base.gameObject, 2f);
			Collected = true;
		}
	}

	private bool HasGem(string Bool)
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		gameData.InitGems();
		switch (Bool)
		{
		case "Blue":
			return gameData.ObtainedGems.Contains(1);
		case "Red":
			return gameData.ObtainedGems.Contains(2);
		case "Green":
			return gameData.ObtainedGems.Contains(3);
		case "Purple":
			return gameData.ObtainedGems.Contains(4);
		case "Sky":
			return gameData.ObtainedGems.Contains(5);
		case "White":
			return gameData.ObtainedGems.Contains(6);
		case "Yellow":
			return gameData.ObtainedGems.Contains(7);
		case "Rainbow":
			if (!gameData.HasFlag("wvo_sn") || !gameData.HasFlag("dtd_sn") || !gameData.HasFlag("wap_sn") || !gameData.HasFlag("csc_sn") || !gameData.HasFlag("flc_sn") || !gameData.HasFlag("rct_sn") || !gameData.HasFlag("tpj_sn") || !gameData.HasFlag("kdv_sn") || !gameData.HasFlag("aqa_sn") || !gameData.HasFlag("wvo_tl"))
			{
				return true;
			}
			return gameData.ObtainedGems.Contains(8);
		default:
			return false;
		}
	}

	private int GemIndex()
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		for (int i = 0; i < gameData.ObtainedGems.Count; i++)
		{
			if (GemType == Type.Blue)
			{
				return 1;
			}
			if (GemType == Type.Red)
			{
				return 2;
			}
			if (GemType == Type.Green)
			{
				return 3;
			}
			if (GemType == Type.Purple)
			{
				return 4;
			}
			if (GemType == Type.Sky)
			{
				return 5;
			}
			if (GemType == Type.White)
			{
				return 6;
			}
			if (GemType == Type.Yellow)
			{
				return 7;
			}
			if (GemType == Type.Rainbow)
			{
				return 8;
			}
		}
		return 0;
	}

	private void SetGemBool(string Bool, PlayerManager PM)
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		if (Bool == "Blue")
		{
			gameData.ObtainedGems.Add(1);
		}
		if (Bool == "Red")
		{
			gameData.ObtainedGems.Add(2);
		}
		if (Bool == "Green")
		{
			gameData.ObtainedGems.Add(3);
		}
		if (Bool == "Purple")
		{
			gameData.ObtainedGems.Add(4);
		}
		if (Bool == "Sky")
		{
			gameData.ObtainedGems.Add(5);
		}
		if (Bool == "White")
		{
			gameData.ObtainedGems.Add(6);
		}
		if (Bool == "Yellow")
		{
			gameData.ObtainedGems.Add(7);
		}
		if (Bool == "Rainbow")
		{
			gameData.ObtainedGems.Add(8);
		}
		gameData.ObtainedGems.Sort();
		Singleton<GameManager>.Instance.SetGameData(gameData);
		PM.sonic.GemData = gameData;
		PM.sonic.GemSelector = gameData.ObtainedGems[gameData.ObtainedGems.IndexOf(GemIndex())];
		PM.sonic.ActiveGem = (SonicNew.Gem)PM.sonic.GemSelector;
		for (int i = 0; i < gameData.ObtainedGems.Count; i++)
		{
			if (gameData.ObtainedGems[i] == PM.sonic.GemSelector)
			{
				PM.sonic.ObtainedGemIndex = i;
			}
		}
		PM.Base.HUD.UpdateGemPanel(gameData);
	}
}
