using System.Collections.Generic;
using UnityEngine;

public class ItemDisplayManager : MonoBehaviour
{
	public GameObject canvas;

	public GameObject itemDisplayPrefab;

	public Sprite[] sprite;

	internal List<ItemDisplay> Images = new List<ItemDisplay>();

	public void GotItemBox(int item)
	{
		GameObject gameObject = Object.Instantiate(itemDisplayPrefab, base.transform);
		foreach (ItemDisplay image in Images)
		{
			if ((bool)image)
			{
				image.Padding++;
			}
		}
		ItemDisplay component = gameObject.GetComponent<ItemDisplay>();
		component.canvas = canvas.GetComponent<Canvas>();
		component.image.sprite = sprite[item];
		Images.Add(component);
		gameObject.SetActive(value: true);
	}
}
