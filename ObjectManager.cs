using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public enum Type
	{
		Static = 0,
		Physics = 1,
		Enemy = 2,
		Effects = 3
	}

	public Type ObjectType;

	[Header("Optional")]
	public bool OverrideDist;

	public float Dist;

	private Transform[] Objects;

	private Vector3[] Position;

	private Quaternion[] Rotation;

	private Transform Player;

	private float CheckTime;

	private void Start()
	{
		AddObject();
	}

	private void FixedUpdate()
	{
		if (Player == null)
		{
			Player = GameObject.FindGameObjectWithTag("Player").transform;
		}
		if (CheckTime < 0.03333334f)
		{
			CheckTime += Time.fixedDeltaTime;
			return;
		}
		CheckTime = 0f;
		Vector3 position = Player.position;
		for (int i = 0; i < Objects.Length; i++)
		{
			Transform transform = Objects[i];
			if ((bool)transform && !(transform == null) && !(transform == base.transform))
			{
				position.y = transform.position.y;
				float num = Vector3.Distance(position, transform.position);
				float num2 = (OverrideDist ? Dist : ((ObjectType == Type.Static || ObjectType == Type.Physics) ? 110f : ((ObjectType == Type.Effects) ? 165f : 55f)));
				if (num < num2 && !transform.gameObject.activeSelf)
				{
					EnableObject(transform);
				}
				else if (num > num2 + 5f && transform.gameObject.activeSelf)
				{
					DisableObject(transform, i);
				}
				if (transform.parent != base.transform)
				{
					AddObject();
				}
			}
		}
	}

	private void EnableObject(Transform Object)
	{
		Object.gameObject.SetActive(value: true);
		if (ObjectType == Type.Enemy)
		{
			Object.SendMessage("Transfer", 0, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void DisableObject(Transform Object, int i)
	{
		if (ObjectType == Type.Physics)
		{
			Rigidbody component = Object.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.velocity = Vector3.zero;
			}
			Object.position = Position[i];
			Object.rotation = Rotation[i];
		}
		if (ObjectType != Type.Enemy)
		{
			Object.gameObject.SetActive(value: false);
		}
		if (ObjectType == Type.Enemy)
		{
			Object.SendMessage("Reset", 0, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void AddObject()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in base.transform)
		{
			list.Add(item);
		}
		Objects = list.ToArray();
		if (ObjectType != Type.Physics)
		{
			return;
		}
		Position = new Vector3[Objects.Length];
		Rotation = new Quaternion[Objects.Length];
		for (int i = 0; i < Objects.Length; i++)
		{
			Transform transform = Objects[i];
			if ((bool)transform && !(transform == null))
			{
				Position[i] = transform.position;
				Rotation[i] = transform.rotation;
			}
		}
	}

	public void FindNewClosestPlayer(Transform player)
	{
		Player = null;
		Player = player;
	}
}
