using System;
using UnityEngine;

[Serializable]
public class PathData
{
	public Vector3[] position;

	public Vector3[] normal;

	public PathData(Vector3[] _position, Vector3[] _normal)
	{
		position = _position;
		normal = _normal;
	}
}
