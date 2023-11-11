using UnityEngine;

public class GoldMedalUI : MonoBehaviour
{
	[Header("Setup")]
	public Sprite[] Sprites;

	[Header("Framework")]
	public GoldMedal[] Medals;

	private bool Updated;

	private void Start()
	{
		if (!Updated)
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			for (int i = 0; i < Medals.Length; i++)
			{
				Medals[i].Icon.sprite = Sprites[gameData.HasFlag(Medals[i].ID) ? 1 : 0];
			}
			Updated = true;
		}
	}
}
