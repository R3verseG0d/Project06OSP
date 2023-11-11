using System.Collections.Generic;
using UnityEngine;

public class ScrollBuild : MonoBehaviour
{
	public GameObject Building;

	public Vector3 StartPos;

	public Vector3 EndPos;

	public float Speed;

	public int Rate;

	private List<Transform> Buildings = new List<Transform>();

	private void Start()
	{
		for (int i = 0; i < Rate; i++)
		{
			GameObject gameObject = Object.Instantiate(Building, base.transform.position, base.transform.rotation);
			gameObject.transform.position = Vector3.MoveTowards(StartPos, EndPos, Vector3.Distance(StartPos, EndPos) * (float)i / (float)Rate);
			gameObject.transform.localScale = new Vector3(1f, 1f, (Random.value > 0.25f) ? (-1f) : 1f);
			Buildings.Add(gameObject.transform);
		}
	}

	private void Update()
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i].position != EndPos)
			{
				Buildings[i].position = Vector3.MoveTowards(Buildings[i].position, EndPos, Speed * Time.deltaTime);
			}
			else
			{
				Buildings[i].position = StartPos;
			}
		}
	}
}
