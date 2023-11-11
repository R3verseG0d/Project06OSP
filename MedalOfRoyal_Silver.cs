using UnityEngine;

public class MedalOfRoyal_Silver : ObjectBase
{
	[Header("Framework")]
	public string Global_Flag;

	[Header("Prefab")]
	public GameObject Mesh;

	public GameObject MedalFX;

	[Header("Optional")]
	public GameObject ActivateObj;

	private bool Collected;

	public void SetParameters(string _Global_Flag)
	{
		Global_Flag = _Global_Flag;
	}

	private void Start()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !Collected)
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			gameData.ActivateFlag(Global_Flag);
			Singleton<GameManager>.Instance.SetGameData(gameData);
			player.HUD.UpdateCollectibles();
			if ((bool)ActivateObj)
			{
				ActivateObj.SetActive(value: true);
			}
			Mesh.SetActive(value: false);
			MedalFX.SetActive(value: true);
			Object.Destroy(base.gameObject, 1f);
			Collected = true;
		}
	}
}
