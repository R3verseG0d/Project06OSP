using UnityEngine;

public class HangingRock : MonoBehaviour
{
	[Header("Framework")]
	public bool NoCheckDamage;

	[Header("Prefab")]
	public GameObject[] ModelGroups;

	public Transform Cube;

	private bool Dropped;

	public void SetParameters(bool _NoCheckDamage)
	{
		NoCheckDamage = _NoCheckDamage;
	}

	private void OnHit()
	{
		Drop();
	}

	private void OnFlash()
	{
		Drop();
	}

	private void OnPsychokinesis(Transform PlayerPos)
	{
		Drop();
	}

	private void Drop()
	{
		if (!Dropped && !NoCheckDamage)
		{
			ModelGroups[0].SetActive(value: false);
			ModelGroups[1].SetActive(value: true);
			Cube.SetParent(null);
			Dropped = true;
		}
	}
}
