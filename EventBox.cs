using UnityEngine;

public class EventBox : EventStation
{
	[Header("Framework")]
	public string OnIntersect;

	public GameObject[] NullEventObjs;

	public void SetParameters(string _OnIntersect)
	{
		OnIntersect = _OnIntersect;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!player || player.IsDead)
		{
			return;
		}
		if (OnIntersect == "" && NullEventObjs != null)
		{
			for (int i = 0; i < NullEventObjs.Length; i++)
			{
				NullEventObjs[i].SetActive(!NullEventObjs[i].activeSelf);
			}
		}
		if (player.GetPrefab("sonic_new"))
		{
			Singleton<GameManager>.Instance.KeepPlayerAttributes(new PlayerAttributesData[4]
			{
				new PlayerAttributesData("GemGeneral", player.PlayerManager.sonic.IsSuper, player.HUD.ActionDisplay, player.PlayerManager.sonic.GemSelector),
				new PlayerAttributesData("GemLevels", null, null, player.HUD.ActiveGemLevel),
				new PlayerAttributesData("GemDisplay", null, player.HUD.GemDisplay),
				new PlayerAttributesData("Shield/" + player.PlayerPrefab, player.HasShield)
			});
		}
		else if (player.GetPrefab("shadow"))
		{
			Singleton<GameManager>.Instance.KeepPlayerAttributes(new PlayerAttributesData[4]
			{
				new PlayerAttributesData("BoostGeneral", player.PlayerManager.shadow.IsChaosBoost, player.HUD.ActionDisplay, player.PlayerManager.shadow.ChaosBoostLevel),
				new PlayerAttributesData("BoostDisplay", _VarBool: false, player.HUD.ChaosMaturityDisplay),
				new PlayerAttributesData("UninhibitMode", player.PlayerManager.shadow.IsFullPower),
				new PlayerAttributesData("Shield/" + player.PlayerPrefab, player.HasShield)
			});
		}
		else if (player.GetPrefab("silver"))
		{
			Singleton<GameManager>.Instance.KeepPlayerAttributes(new PlayerAttributesData[3]
			{
				new PlayerAttributesData("ESPGeneral", player.PlayerManager.silver.IsAwakened, player.HUD.ActionDisplay),
				new PlayerAttributesData("ESPDisplay", _VarBool: false, player.HUD.ESPMaturityDisplay),
				new PlayerAttributesData("Shield/" + player.PlayerPrefab, player.HasShield)
			});
		}
		else
		{
			Singleton<GameManager>.Instance.KeepPlayerAttributes(new PlayerAttributesData[6]
			{
				new PlayerAttributesData("GemGeneral", _VarBool: false, player.HUD.ActionDisplay),
				new PlayerAttributesData("GemLevels", null, null, player.HUD.ActiveGemLevel),
				new PlayerAttributesData("GemDisplay", null, player.HUD.GemDisplay),
				new PlayerAttributesData("BoostGeneral", _VarBool: false, player.HUD.ActionDisplay),
				new PlayerAttributesData("BoostDisplay", _VarBool: false, player.HUD.ChaosMaturityDisplay),
				new PlayerAttributesData("Shield/" + player.PlayerPrefab, player.HasShield)
			});
		}
		CallEvent(OnIntersect, collider.gameObject);
		Object.Destroy(base.gameObject);
	}
}
