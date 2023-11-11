using STHEngine;
using UnityEngine;

public class BreakWall : ObjectBase
{
	[Header("Prefab")]
	public GameObject SaneModel;

	public GameObject DamagedModel;

	public GameObject BrokenPrefab;

	private int HP;

	private bool Destroyed;

	private bool Damaged;

	private void OnCollisionEnter(Collision collision)
	{
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player && player.GetPrefab("sonic_fast") && player.CurSpeed >= 30f)
		{
			player.OnHurtEnter();
			OnHit(new HitInfo(player.transform, player.transform.forward * player.CurSpeed));
		}
	}

	public void OnHit(HitInfo HitInfo)
	{
		if (Destroyed)
		{
			return;
		}
		HP++;
		if (HP > 1)
		{
			Destroyed = true;
			GameObject gameObject = Object.Instantiate(BrokenPrefab, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", HitInfo, SendMessageOptions.DontRequireReceiver);
			if ((bool)HitInfo.player && HitInfo.player.tag == "Player")
			{
				HitInfo.player.GetComponent<PlayerBase>().AddScore(100);
			}
			Object.Destroy(base.transform.gameObject);
		}
		else if (!Damaged)
		{
			SaneModel.SetActive(value: false);
			DamagedModel.SetActive(value: true);
			Damaged = true;
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}

	private void OnEventSignal()
	{
		if ((bool)BrokenPrefab)
		{
			OnHit(new HitInfo(base.transform, Vector3.zero));
		}
	}
}
