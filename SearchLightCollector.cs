using System.Collections.Generic;
using UnityEngine;

public class SearchLightCollector : MonoBehaviour
{
	[Header("Framework")]
	public List<SearchLight> Towers;

	public GameObject[] ObjectGroup;

	private bool DestroyedAll;

	private void Update()
	{
		if (Towers.Count != 0)
		{
			for (int i = 0; i < Towers.Count; i++)
			{
				if (!Towers[i])
				{
					Towers.RemoveAt(i);
				}
			}
		}
		if ((Towers.Count == 0 || Towers == null || IsAllHeadsDestroyed()) && !DestroyedAll)
		{
			DestroyedAll = true;
			for (int j = 0; j < ObjectGroup.Length; j++)
			{
				ObjectGroup[j].SetActive(!ObjectGroup[j].activeSelf);
			}
			Object.Destroy(base.gameObject);
		}
	}

	private bool IsAllHeadsDestroyed()
	{
		for (int i = 0; i < Towers.Count; i++)
		{
			if (!Towers[i].DestroyedHead)
			{
				return false;
			}
		}
		return true;
	}
}
