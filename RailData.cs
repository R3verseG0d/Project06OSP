using System;
using UnityEngine;

[Serializable]
public class RailData
{
	[SerializeField]
	public Vector3 position;

	[SerializeField]
	public Vector3 normal;

	[SerializeField]
	public Vector3 tangent;

	public RailData(Vector3 _position, Vector3 _normal, Vector3 _tangent)
	{
		position = _position;
		normal = _normal;
		tangent = _tangent;
	}

	public RailData(Vector3 _position, Vector3 _tangent)
	{
		position = _position;
		tangent = _tangent;
	}
}
