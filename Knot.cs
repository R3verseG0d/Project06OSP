using System;
using UnityEngine;

[Serializable]
public class Knot
{
	public enum HandleType
	{
		Free = 0,
		Aligned = 1,
		Broken = 2,
		Auto = 3
	}

	public Vector3 position;

	public Vector3 ctrl1;

	public Vector3 ctrl2;

	public HandleType type;

	public Knot(Vector3 _position, Vector3 _ctrl1, Vector3 _ctrl2)
	{
		position = _position;
		ctrl1 = _ctrl1;
		ctrl2 = _ctrl2;
	}
}
