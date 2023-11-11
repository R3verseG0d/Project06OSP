using System.Collections.Generic;
using UnityEngine;

public class Common_Cage : MonoBehaviour
{
	[Header("Framework")]
	public float Scale;

	[Header("Prefab")]
	public Animator Animator;

	public Light PointLight;

	public AudioSource Audio;

	public Collider CageCollider;

	[Header("Object Framework")]
	public GameObject[] ObjectsInside;

	private List<Common_Switch> Switches;

	private List<ItemBox> ItemBoxes;

	private List<Lever> Rct_Levers;

	private bool Opened;

	private bool IsSetUp;

	public void SetParameters(float _Scale)
	{
		Scale = _Scale;
	}

	private void Start()
	{
		if (IsSetUp)
		{
			return;
		}
		base.transform.localScale = Vector3.one * Scale;
		PointLight.range *= Scale;
		if (ObjectsInside != null)
		{
			Switches = new List<Common_Switch>();
			ItemBoxes = new List<ItemBox>();
			Rct_Levers = new List<Lever>();
			for (int i = 0; i < ObjectsInside.Length; i++)
			{
				Collider[] componentsInChildren = ObjectsInside[i].GetComponentsInChildren<Collider>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					Physics.IgnoreCollision(componentsInChildren[j], CageCollider);
				}
				Common_Switch component = ObjectsInside[i].GetComponent<Common_Switch>();
				if ((bool)component)
				{
					component.IsCaged = true;
					Switches.Add(component);
				}
				ItemBox component2 = ObjectsInside[i].GetComponent<ItemBox>();
				if ((bool)component2)
				{
					component2.IsCaged = true;
					ItemBoxes.Add(component2);
				}
				Lever component3 = ObjectsInside[i].GetComponent<Lever>();
				if ((bool)component3)
				{
					component3.IsCaged = true;
					Rct_Levers.Add(component3);
				}
			}
		}
		IsSetUp = true;
	}

	private void Update()
	{
		if (Opened && PointLight.intensity != 0f)
		{
			PointLight.intensity = Mathf.MoveTowards(PointLight.intensity, 0f, Time.deltaTime * 5f);
		}
	}

	private void OnEventSignal()
	{
		Opened = true;
		Audio.Play();
		Animator.SetTrigger("On Signal");
		if (Switches.Count != 0)
		{
			for (int i = 0; i < Switches.Count; i++)
			{
				Switches[i].IsCaged = false;
			}
		}
		if (ItemBoxes.Count != 0)
		{
			for (int j = 0; j < ItemBoxes.Count; j++)
			{
				ItemBoxes[j].IsCaged = false;
			}
		}
		if (Rct_Levers.Count != 0)
		{
			for (int k = 0; k < Rct_Levers.Count; k++)
			{
				Rct_Levers[k].IsCaged = false;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero + Vector3.up * 1.75f * Scale, Vector3.one * 3.5f * Scale);
	}
}
