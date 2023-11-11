using System.Collections.Generic;
using UnityEngine;

public class UpgradeModels : MonoBehaviour
{
	[Header("Framework")]
	public List<Table> ItemTable;

	internal List<Renderer> Renderers = new List<Renderer>();

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.UpgradeModels == 1 || ItemTable == null)
		{
			return;
		}
		foreach (Table item in ItemTable)
		{
			GameObject gameObject = Object.Instantiate(item.Model, item.Node.position, item.Node.rotation);
			gameObject.transform.SetParent(item.Node);
			Renderers.Add(gameObject.GetComponentInChildren<Renderer>());
		}
	}
}
