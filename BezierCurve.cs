using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BezierCurve : MonoBehaviour
{
	public List<Knot> knots;

	private List<Knot> lastKnots;

	private float length;

	private float[] lookup;

	public void Reset()
	{
		knots = new List<Knot>();
		knots.Add(new Knot(Vector3.left * 2f, Vector3.left * 3f, Vector3.left + Vector3.up));
		knots.Add(new Knot(Vector3.right * 2f, Vector3.right + Vector3.down, Vector3.right * 3f));
	}

	public void AddKnot()
	{
		Vector3 position = knots[knots.Count - 1].position;
		Vector3 normalized = (position - knots[knots.Count - 2].position).normalized;
		knots.Add(new Knot(position + normalized * 3f, position + normalized * 2f, position + normalized * 4f));
	}

	public void InsertKnot(int index)
	{
		int num = knots.Count - 1;
		if (index > num)
		{
			AddKnot();
		}
		else if (index == 0)
		{
			Vector3 position = knots[0].position;
			Vector3 normalized = (position - knots[1].position).normalized;
			knots.Insert(index, new Knot(position + normalized * 3f, position + normalized * 4f, position + normalized * 2f));
		}
		else
		{
			Vector3 vector = (knots[index - 1].position + knots[index].position) * 0.5f;
			Vector3 normalized2 = (knots[index - 1].ctrl2 - vector).normalized;
			Vector3 normalized3 = (knots[index].ctrl1 - vector).normalized;
			knots.Insert(index, new Knot(vector, vector + normalized2, vector + normalized3));
		}
	}

	public void UpdateKnot(int index)
	{
		Knot knot = knots[index];
		Knot.HandleType type = knot.type;
		bool flag = index == 0;
		bool flag2 = index == knots.Count - 1;
		switch (type)
		{
		case Knot.HandleType.Free:
			return;
		case Knot.HandleType.Aligned:
		{
			Vector3 vector3 = knot.position - knot.ctrl1;
			Vector3 vector4 = knot.ctrl2 - knot.position;
			Vector3 vector5 = (vector3 + vector4) * 0.5f;
			knot.ctrl1 = knot.position - vector5;
			knot.ctrl2 = knot.position + vector5;
			return;
		}
		case Knot.HandleType.Broken:
		case Knot.HandleType.Auto:
			if (!flag)
			{
				Vector3 vector = knots[index - 1].position - knot.position;
				knot.ctrl1 = knot.position + vector * 0.25f;
			}
			if (!flag2)
			{
				Vector3 vector2 = knots[index + 1].position - knot.position;
				knot.ctrl2 = knot.position + vector2 * 0.25f;
			}
			if (type == Knot.HandleType.Broken)
			{
				return;
			}
			break;
		}
		if (type == Knot.HandleType.Auto)
		{
			Vector3 vector6 = knot.position - knot.ctrl1;
			Vector3 vector7 = knot.ctrl2 - knot.position;
			Vector3 vector8 = (vector6 + vector7) * 0.5f;
			knot.ctrl1 = knot.position - vector8;
			knot.ctrl2 = knot.position + vector8;
		}
	}

	public int GetSegmentAtTime(ref float t)
	{
		int num = knots.Count - 1;
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

	private void CopyList()
	{
		lastKnots = new List<Knot>();
		for (int i = 0; i < knots.Count; i++)
		{
			lastKnots.Add(new Knot(knots[i].position, knots[i].ctrl1, knots[i].ctrl2));
		}
	}

	private bool ShouldUpdate()
	{
		if (lastKnots == null || knots.Count != lastKnots.Count)
		{
			CopyList();
			return true;
		}
		for (int i = 0; i < knots.Count; i++)
		{
			if (knots[i].position != lastKnots[i].position || knots[i].ctrl1 != lastKnots[i].ctrl1 || knots[i].ctrl2 != lastKnots[i].ctrl2)
			{
				CopyList();
				return true;
			}
		}
		return false;
	}

	private void Parametrize()
	{
		int num = (knots.Count - 1) * 25;
		length = 0f;
		List<float> list = new List<float>();
		Vector3 b = InterpolatePosition(0f);
		list.Add(0f);
		for (int i = 0; i < num; i++)
		{
			float t = (1f + (float)i) / (float)num;
			Vector3 vector = InterpolatePosition(t);
			length += Vector3.Distance(vector, b);
			b = vector;
			list.Add(length);
		}
		lookup = list.ToArray();
	}

	public float Length()
	{
		if (ShouldUpdate())
		{
			Parametrize();
		}
		return length;
	}

	public float[] LookupTable()
	{
		if (ShouldUpdate() || lookup == null)
		{
			Parametrize();
		}
		return lookup;
	}

	private float LengthTime(int index)
	{
		return (float)index / ((float)LookupTable().Length - 1f);
	}

	public float GetUnscaledTime(float t)
	{
		if (t == 0f || t == 1f)
		{
			return t;
		}
		float num = Length() * t;
		float[] array = LookupTable();
		int num2 = -1;
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = array[i];
			if (num3 == num)
			{
				return LengthTime(i);
			}
			if (num3 > num)
			{
				num2 = i;
				break;
			}
		}
		if (num2 == 0)
		{
			num2++;
		}
		float num4 = array[num2 - 1];
		float num5 = array[num2] - num4;
		float t2 = (num - num4) / num5;
		return Mathf.Lerp(LengthTime(num2 - 1), LengthTime(num2), t2);
	}

	public Vector3 GetPosition(float t, bool worldSpace = true)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (worldSpace)
		{
			return base.transform.TransformPoint(InterpolatePosition(GetUnscaledTime(t)));
		}
		return InterpolatePosition(GetUnscaledTime(t));
	}

	public Vector3 InterpolatePosition(float t)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (knots == null)
		{
			Debug.LogError("CubicBezierSpline knots are null");
			return Vector3.zero;
		}
		if (knots.Count < 2)
		{
			return Vector3.zero;
		}
		if (t == 0f)
		{
			return knots[0].position;
		}
		if (t == 1f)
		{
			return knots[knots.Count - 1].position;
		}
		int segmentAtTime = GetSegmentAtTime(ref t);
		if (t == 0f)
		{
			return knots[segmentAtTime].position;
		}
		float num = 1f - t;
		return num * num * num * knots[segmentAtTime].position + 3f * num * num * t * knots[segmentAtTime].ctrl2 + 3f * num * t * t * knots[segmentAtTime + 1].ctrl1 + t * t * t * knots[segmentAtTime + 1].position;
	}

	public Vector3 GetTangent(float t, bool worldSpace = true)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (knots == null)
		{
			Debug.LogError("CubicBezierSpline knots are null");
			return Vector3.zero;
		}
		if (knots.Count < 2)
		{
			return Vector3.zero;
		}
		float t2 = GetUnscaledTime(t);
		int segmentAtTime = GetSegmentAtTime(ref t2);
		Vector3 zero = Vector3.zero;
		zero = ((t != 0f) ? InterpolateTangent(t2, knots[segmentAtTime].position, knots[segmentAtTime].ctrl2, knots[segmentAtTime + 1].ctrl1, knots[segmentAtTime + 1].position).normalized : (knots[segmentAtTime].ctrl2 - knots[segmentAtTime].position).normalized);
		if (worldSpace)
		{
			return base.transform.TransformDirection(zero);
		}
		return zero;
	}

	public float FindNearestPointToProgress(Vector3 WorldPos, float Accuracy = 100f)
	{
		float result = -1f;
		float num = AccuracyToStepSize(Accuracy);
		float num2 = float.PositiveInfinity;
		for (float num3 = 0f; num3 < 1f; num3 += num)
		{
			Vector3 position = GetPosition(num3);
			float sqrMagnitude = (WorldPos - position).sqrMagnitude;
			if (sqrMagnitude < num2)
			{
				num2 = sqrMagnitude;
				result = num3;
			}
		}
		return result;
	}

	private float AccuracyToStepSize(float Accuracy)
	{
		if (Accuracy <= 0f)
		{
			return 0.2f;
		}
		return Mathf.Clamp(1f / Accuracy, 0.001f, 0.2f);
	}

	private Vector3 InterpolateTangent(float t, Vector3 p0, Vector3 h0, Vector3 h1, Vector3 p1)
	{
		float num = 1f - t;
		float num2 = num * 6f * t;
		num = num * num * 3f;
		float num3 = t * t * 3f;
		return (0f - num) * p0 + num * h0 - num2 * h0 - num3 * h1 + num2 * h1 + num3 * p1;
	}

	public void LogDistance()
	{
		Vector3 b = GetPosition(0f);
		for (int i = 1; i <= 20; i++)
		{
			float t = (float)i / 20f;
			Vector3 position = GetPosition(t);
			Debug.Log(Vector3.Distance(position, b));
			b = position;
		}
		Debug.Log("Total Length: " + Length());
	}
}
