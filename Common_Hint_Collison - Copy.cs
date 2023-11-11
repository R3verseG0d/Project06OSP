using UnityEngine;

public class Common_Hint_Collison : ButtonIconBase
{
	[Header("Framework")]
	public string Message;

	public void SetParameters(string _Message)
	{
		Message = _Message;
	}

	private void Awake()
	{
		if (Singleton<Settings>.Instance.settings.Dialogue == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (base.enabled)
		{
			PlayerBase player = GetPlayer(collider);
			if ((bool)player && !player.IsDead && !(player.GetState() == "Result") && !(Message == "") && !player.HUD.MsgBoxObject)
			{
				player.HUD.StartMessageBox(GetText(Message), GetSound(Message));
				Object.Destroy(base.gameObject);
			}
		}
	}
}
