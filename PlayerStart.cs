using UnityEngine;

public class PlayerStart : MonoBehaviour
{
	[Header("Framework")]
	public int Player_No;

	public string Player_Name;

	public bool Amigo;

	[Header("Optional")]
	public bool NotVisiblyInteractable;

	public void SetParameters(int _Player_No, string _Player_Name, bool _Amigo)
	{
		Player_No = _Player_No;
		Player_Name = _Player_Name;
		Amigo = _Amigo;
	}

	public string GetPlayerName()
	{
		string result = Player_Name;
		if (Player_Name.Contains("_wap"))
		{
			result = Player_Name.Replace("_wap", "");
		}
		if (Player_Name.Contains("_none"))
		{
			result = Player_Name.Replace("_none", "");
		}
		if (Player_Name.Contains("_jeep"))
		{
			result = Player_Name.Replace("_jeep", "");
		}
		if (Player_Name.Contains("_bike"))
		{
			result = Player_Name.Replace("_bike", "");
		}
		if (Player_Name.Contains("_hover"))
		{
			result = Player_Name.Replace("_hover", "");
		}
		if (Player_Name.Contains("_glider"))
		{
			result = Player_Name.Replace("_glider", "");
		}
		return result;
	}
}
