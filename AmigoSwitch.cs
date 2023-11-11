using UnityEngine;

public class AmigoSwitch : ObjectBase
{
	public GameObject[] Characters;

	internal int PlayerNo;

	internal string PlayerPrefab;

	private bool Switched;

	private void Start()
	{
		for (int i = 0; i < Characters.Length; i++)
		{
			if (Characters[i].name == PlayerPrefab)
			{
				Characters[i].SetActive(value: true);
				Characters[i].GetComponent<Animator>().SetTrigger("On Ground");
			}
		}
		PlayerBase playerBase = Object.FindObjectOfType<PlayerBase>();
		if ((bool)playerBase && PlayerNo == playerBase.PlayerNo)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() == "Vehicle") && !player.IsDead && !Switched)
		{
			Switched = true;
			SwitchToAmigo(player);
		}
	}

	public void SwitchToAmigo(PlayerBase Player)
	{
		PlayerBase component = (Object.Instantiate(Resources.Load("DefaultPrefabs/Player/" + PlayerPrefab), base.transform.position, base.transform.rotation) as GameObject).GetComponent<PlayerBase>();
		component.SetPlayer(PlayerNo, PlayerPrefab);
		component.StartPlayer(TalkStart: true);
		component.HUD.UseCrosshair(EndCrosshair: true, Reset: true);
		if (Player.HasShield)
		{
			component.HasShield = true;
			component.ShieldObject = Player.ShieldObject;
			component.ShieldObject.transform.position = component.transform.position + component.transform.up * ((!component.GetPrefab("omega")) ? 0.25f : 0.5f);
			component.ShieldObject.transform.rotation = Quaternion.identity;
			Player.ShieldObject.transform.SetParent(component.transform);
			component.ShieldObject.transform.localScale = Vector3.one * ((!component.GetPrefab("omega")) ? 1f : 1.5f);
		}
		Object.Destroy(base.gameObject);
		if (Player.GetState() != "Orca")
		{
			Object.Destroy(Player.gameObject);
		}
	}
}
