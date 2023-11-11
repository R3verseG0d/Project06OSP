using UnityEngine;

public class BreakTower : MonoBehaviour
{
	[Header("Prefab")]
	public float FXEnableTime;

	public Vector3 FXLocalPos;

	[Header("Prefab")]
	public GameObject MainObj;

	public GameObject BrokenObj;

	[Header("Broken Obj")]
	public Animation Animation;

	public GameObject FX;

	private bool Destroyed;

	private float EnableTime;

	private void Update()
	{
		if (Destroyed)
		{
			if ((bool)FX && Time.time - EnableTime > FXEnableTime && !FX.activeSelf)
			{
				FX.SetActive(value: true);
			}
			if ((bool)Animation && !Animation.isPlaying)
			{
				Animation = null;
			}
		}
	}

	public void OnHit(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			MainObj.SetActive(value: false);
			BrokenObj.SetActive(value: true);
			EnableTime = Time.time;
			FX.transform.localPosition = FXLocalPos;
			Destroyed = true;
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (!Destroyed)
		{
			OnHit(HitInfo);
		}
	}

	private void OnEventSignal()
	{
		if (!Destroyed)
		{
			OnHit(new HitInfo(base.transform, Vector3.zero));
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 vector = base.transform.position + base.transform.up * FXLocalPos.y + base.transform.right * FXLocalPos.x + base.transform.forward * FXLocalPos.z;
		Gizmos.DrawLine(base.transform.position, vector);
		Gizmos.DrawWireSphere(vector, 1f);
	}
}
