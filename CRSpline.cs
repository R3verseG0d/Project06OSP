using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CRSpline
{
	public List<Vector3> knots;

	private List<Vector3> lastKnots;

	public CRSpline()
	{
		knots = new List<Vector3>();
	}

	public Vector3 GetPosition(float t)
	{
		if (knots.Count < 4)
		{
			return Vector3.zero;
		}
		int num = knots.Count - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = knots[num2];
		Vector3 vector2 = knots[num2 + 1];
		Vector3 vector3 = knots[num2 + 2];
		Vector3 vector4 = knots[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	public Vector3 GetTangent(float t)
	{
		if (knots.Count < 4)
		{
			return Vector3.zero;
		}
		int num = knots.Count - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = knots[num2];
		Vector3 vector2 = knots[num2 + 1];
		Vector3 vector3 = knots[num2 + 2];
		Vector3 vector4 = knots[num2 + 3];
		return (1.5f * (-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * num3 + 0.5f * vector3 - 0.5f * vector).normalized;
	}

	public Vector3 GetVelocity(float t)
	{
		if (knots.Count < 4)
		{
			return Vector3.zero;
		}
		int num = knots.Count - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = knots[num2];
		Vector3 vector2 = knots[num2 + 1];
		Vector3 vector3 = knots[num2 + 2];
		Vector3 vector4 = knots[num2 + 3];
		return 1.5f * (-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * num3 + 0.5f * vector3 - 0.5f * vector;
	}

	public void GizmoDraw()
	{
		if (knots.Count < 4)
		{
			return;
		}
		Gizmos.color = Color.white;
		Vector3 to = GetPosition(0f);
		for (int i = 1; i <= 20; i++)
		{
			float t = (float)i / 20f;
			Vector3 position = GetPosition(t);
			Gizmos.DrawLine(position, to);
			to = position;
		}
		for (int j = 0; j < knots.Count; j++)
		{
			if (j == 0)
			{
				Gizmos.color = Color.green;
			}
			else if (j == knots.Count - 1)
			{
				Gizmos.color = Color.red;
			}
			else
			{
				Gizmos.color = Color.blue;
			}
			Gizmos.DrawWireSphere(knots[j], 0.2f);
		}
	}

	public void GizmoDraw(float t)
	{
		if (knots.Count >= 4)
		{
			Gizmos.color = Color.white;
			Vector3 to = GetPosition(0f);
			for (int i = 1; i <= 20; i++)
			{
				float t2 = (float)i / 20f;
				Vector3 position = GetPosition(t2);
				Gizmos.DrawLine(position, to);
				to = position;
			}
			Gizmos.color = Color.blue;
			Vector3 position2 = GetPosition(t);
			Gizmos.DrawLine(position2, position2 + GetVelocity(t));
		}
	}
}
