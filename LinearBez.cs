using System;
using UnityEngine;

[Serializable]
public class LinearBez
{
	public Vector3[] knots;

	private float[] param_data;

	private float length;

	public LinearBez(Vector3[] _knots)
	{
		knots = _knots;
		param_data = new float[_knots.Length];
		Parametrize();
	}

	private void Parametrize()
	{
		int num = knots.Length - 1;
		length = 0f;
		Vector3 b = GetPosition(0f);
		param_data[0] = 0f;
		for (int i = 0; i < num; i++)
		{
			float num2 = (1f + (float)i) / (float)num;
			param_data[i + 1] = num2;
			Vector3 position = GetPosition(num2);
			length += Vector3.Distance(position, b);
			b = position;
		}
	}

	public float Length()
	{
		return length;
	}

	public float GetTime(int index)
	{
		if (index < 0)
		{
			return param_data[0];
		}
		return param_data[index];
	}

	public int GetSegment(float t)
	{
		int num = knots.Length - 1;
		float num2 = t * (float)num;
		int num3 = (int)num2;
		t = num2 - (float)num3;
		if (num3 > num - 1)
		{
			t = 1f;
			return num - 1;
		}
		return num3;
	}

	public int GetSegment(ref float t)
	{
		int num = knots.Length - 1;
		float num2 = t * (float)num;
		int num3 = (int)num2;
		num2 -= (float)num3;
		t = num2;
		if (num3 > num - 1)
		{
			t = 1f;
			return num - 1;
		}
		return num3;
	}

	public Vector3 GetPosition(float t)
	{
		if (t <= 0f)
		{
			return knots[0];
		}
		if (t >= 1f)
		{
			return knots[knots.Length - 1];
		}
		int segment = GetSegment(ref t);
		if (t == 0f)
		{
			return knots[segment];
		}
		return Vector3.Lerp(knots[segment], knots[segment + 1], t);
	}

	public Vector3 GetTangent(float t)
	{
		if (t <= 0f)
		{
			return (knots[1] - knots[0]).normalized;
		}
		if (t >= 1f)
		{
			return (knots[knots.Length - 1] - knots[knots.Length - 2]).normalized;
		}
		int segment = GetSegment(ref t);
		return (knots[segment + 1] - knots[segment]).normalized;
	}
}
