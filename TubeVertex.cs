using System;
using UnityEngine;

[Serializable]
public class TubeVertex
{
	public Vector3 point = Vector3.zero;

	public Quaternion rotation = Quaternion.identity;

	public float radius = 1f;

	public Color color = Color.white;

	public TubeVertex(Vector3 pt, float r, Color c)
	{
		point = pt;
		radius = r;
		color = c;
	}
}
