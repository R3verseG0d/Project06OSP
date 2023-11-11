using UnityEngine;

public class DropItems : MonoBehaviour
{
	[Header("Framework")]
	public int DropRate;

	public int ItemType;

	private bool Collected;

	public void SetParameters(int _DropRate, int _ItemType)
	{
		DropRate = _DropRate;
		ItemType = _ItemType;
	}

	private void OnHit(HitInfo HitInfo)
	{
		OnCollect();
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}

	private void OnCollect()
	{
		if (!Collected)
		{
			Collected = true;
			CreateObjects(DropRate);
		}
	}

	public void CreateObjects(int Rate)
	{
		float num = 360f / (float)Rate;
		float num2 = Random.Range(0f, 360f);
		for (int i = 0; i < Rate; i++)
		{
			num2 += num;
			Vector3 vector = Quaternion.Euler(new Vector3(0f, num2, 0f)) * Vector3.forward;
			LoadObject(ItemType).GetComponent<Rigidbody>().velocity = vector * 3f + Vector3.up * Random.Range(5f, 10f);
		}
	}

	private GameObject LoadObject(int Type)
	{
		string text = "";
		switch (Type)
		{
		case 3:
			text = "LostRing";
			break;
		}
		return Object.Instantiate(Resources.Load("DefaultPrefabs/Objects/Common/" + text), base.transform.position, base.transform.rotation) as GameObject;
	}
}
