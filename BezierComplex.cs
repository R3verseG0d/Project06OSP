using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BezierComplex
{
	public Vector3[] nodes = new Vector3[0];

	public Vector3[] handlesA = new Vector3[0];

	public Vector3[] handlesB = new Vector3[0];

	public Vector3[] GetPointAtDistance(float dist = 1f, Transform m_Transform = null)
	{
		List<Vector3> list = new List<Vector3>();
		dist /= (float)nodes.Length;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (; num <= 1f; num += num3)
		{
			num3 = 1f / (GetSegmentLengthAtTime(num) / dist);
			num2 = num;
		}
		float num4 = (1f - num2) * dist;
		num3 = 0f;
		num = 0f;
		float num5 = 0f;
		int num6 = 0;
		for (; num <= 1f; num += num3)
		{
			if (num5 > 1f)
			{
				if (num6 < 1)
				{
					num6++;
				}
				else
				{
					num = 1f;
				}
			}
			Vector3 pointAtTime = GetPointAtTime(num5);
			if ((bool)m_Transform)
			{
				list.Add(m_Transform.TransformPoint(pointAtTime));
			}
			else
			{
				list.Add(pointAtTime);
			}
			num3 = 1f / (GetSegmentLengthAtTime(num) / dist);
			num5 += num3 + num3 * num4 / dist;
		}
		return list.ToArray();
	}

	public Vector3[] GetTangentAtDistance(float dist = 1f, Transform m_Transform = null)
	{
		List<Vector3> list = new List<Vector3>();
		dist /= (float)nodes.Length;
		float num = 0f;
		float num2 = 0f;
		float num3;
		for (; num <= 1f; num += num3)
		{
			num3 = 1f / (GetSegmentLengthAtTime(num) / dist);
			num2 = num;
		}
		float num4 = (1f - num2) * dist;
		num = 0f;
		float num5 = 0f;
		int num6 = 0;
		float num7;
		for (; num <= 1f; num += num7)
		{
			if (num5 > 1f)
			{
				if (num6 < 1)
				{
					num6++;
				}
				else
				{
					num = 1f;
				}
			}
			list.Add(GetTangentAtTime(num5));
			num7 = 1f / (GetSegmentLengthAtTime(num) / dist);
			num5 += num7 + num7 * num4 / dist;
		}
		return list.ToArray();
	}

	public Vector3[] GetNormalAtDistance(float dist = 1f)
	{
		List<Vector3> list = new List<Vector3>();
		for (float num = 0f; num <= 1f; num += 0.1f / (GetSegmentLengthAtTime(num) / dist))
		{
			Vector3 tangentAtTime = GetTangentAtTime(num);
			list.Add(Vector3.Cross(GetTangentAtTime(num + 0.001f).normalized, tangentAtTime.normalized).normalized);
		}
		return list.ToArray();
	}

	public float GetSegmentLengthAtTime(float t, int m_Interp = 1000)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (nodes.Length < 2)
		{
			return 1f;
		}
		int segmentAtTime = GetSegmentAtTime(ref t);
		float num = 0f;
		Vector3 a = nodes[segmentAtTime];
		for (int i = 0; i <= m_Interp; i++)
		{
			Vector3 vector = ((segmentAtTime + 1 < nodes.Length) ? CalculatePosition((float)i / ((float)m_Interp * 1f), nodes[segmentAtTime], handlesB[segmentAtTime], handlesA[segmentAtTime + 1], nodes[segmentAtTime + 1]) : nodes[segmentAtTime]);
			num += Vector3.Distance(a, vector);
			a = vector;
		}
		return num;
	}

	public Vector3 GetPointAtTime(float t)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (nodes.Length < 2)
		{
			return Vector3.zero;
		}
		int segmentAtTime = GetSegmentAtTime(ref t);
		if (t == 0f)
		{
			return nodes[segmentAtTime];
		}
		return CalculatePosition(t, nodes[segmentAtTime], handlesB[segmentAtTime], handlesA[segmentAtTime + 1], nodes[segmentAtTime + 1]);
	}

	public int GetSegmentAtTime(ref float t)
	{
		int num = nodes.Length - 1;
		float num2 = t * (float)num;
		int num3 = (int)num2;
		num2 -= (float)num3;
		t = num2;
		return num3;
	}

	public int GetSegmentAtTime(float t)
	{
		int num = nodes.Length - 1;
		return (int)(t * (float)num);
	}

	private Vector3 CalculatePosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float num4 = num3 * num;
		float num5 = num2 * t;
		return num4 * p0 + 3f * num3 * t * p1 + 3f * num * num2 * p2 + num5 * p3;
	}

	public Vector3 GetTangentAtTime(float t)
	{
		t = Mathf.Clamp(t, 0f, 1f);
		if (nodes.Length < 2)
		{
			return Vector3.zero;
		}
		int segmentAtTime = GetSegmentAtTime(ref t);
		if (t == 0f)
		{
			if (segmentAtTime == nodes.Length - 1)
			{
				return nodes[segmentAtTime] - handlesA[segmentAtTime];
			}
			return (handlesB[segmentAtTime] - nodes[segmentAtTime]).normalized;
		}
		return CalculateTangent(t, nodes[segmentAtTime], handlesB[segmentAtTime], handlesA[segmentAtTime + 1], nodes[segmentAtTime + 1]).normalized;
	}

	private Vector3 CalculateTangent(float t, Vector3 p0, Vector3 h0, Vector3 h1, Vector3 p1)
	{
		float num = 1f - t;
		float num2 = num * 6f * t;
		num = num * num * 3f;
		float num3 = t * t * 3f;
		return (0f - num) * p0 + num * h0 - num2 * h0 - num3 * h1 + num2 * h1 + num3 * p1;
	}

	public Vector3[,] GetBezierInHandlesFormat()
	{
		int numberOfSegment = GetNumberOfSegment();
		if (numberOfSegment == 0)
		{
			return null;
		}
		Vector3[,] array = new Vector3[numberOfSegment, 4];
		for (int i = 0; i < numberOfSegment; i++)
		{
			array[i, 0] = nodes[i];
			array[i, 1] = nodes[i + 1];
			array[i, 2] = handlesB[i];
			array[i, 3] = handlesA[i + 1];
		}
		return array;
	}

	public void DeleteAllNodes()
	{
		nodes = new Vector3[0];
		handlesA = new Vector3[0];
		handlesB = new Vector3[0];
	}

	public void DeleteNode(int i)
	{
		if (i < 0 || i >= nodes.Length)
		{
			Debug.LogError("You are  trying to DELETE a node which does not exist. Max node index : " + (nodes.Length - 1) + " . Index wanted : " + i);
		}
		ArrayList arrayList = new ArrayList(nodes);
		ArrayList arrayList2 = new ArrayList(handlesA);
		ArrayList arrayList3 = new ArrayList(handlesB);
		arrayList.RemoveAt(i);
		arrayList2.RemoveAt(i);
		arrayList3.RemoveAt(i);
		nodes = (Vector3[])arrayList.ToArray(typeof(Vector3));
		handlesA = (Vector3[])arrayList2.ToArray(typeof(Vector3));
		handlesB = (Vector3[])arrayList3.ToArray(typeof(Vector3));
	}

	public void InsertNode(int i, Vector3 p, Vector3 h1, Vector3 h2)
	{
		if (i < 0 || i > nodes.Length)
		{
			Debug.LogError("You are  trying to delete a node which does not exist. Max node index (with added  node) : " + nodes.Length + " . Index wanted : " + i);
		}
		ArrayList arrayList = new ArrayList(nodes);
		ArrayList arrayList2 = new ArrayList(handlesA);
		ArrayList arrayList3 = new ArrayList(handlesB);
		if (i == nodes.Length)
		{
			arrayList.Add(p);
			arrayList2.Add(h1);
			arrayList3.Add(h2);
		}
		else
		{
			arrayList.Insert(i, p);
			arrayList2.Insert(i, h1);
			arrayList3.Insert(i, h2);
		}
		nodes = (Vector3[])arrayList.ToArray(typeof(Vector3));
		handlesA = (Vector3[])arrayList2.ToArray(typeof(Vector3));
		handlesB = (Vector3[])arrayList3.ToArray(typeof(Vector3));
	}

	public void MoveNode(Vector3 p, int i)
	{
		if (i < 0 || i >= nodes.Length)
		{
			Debug.LogError("You are  trying to MOVE a node which does not exist. Max node index : " + (nodes.Length - 1) + " . Index wanted : " + i);
		}
		Vector3 vector = p - nodes[i];
		handlesA[i] += vector;
		handlesB[i] += vector;
		nodes[i] = p;
	}

	public void MoveHandle(Vector3 p, int i, bool handleA)
	{
		if (i < 0 || i >= nodes.Length)
		{
			Debug.LogError("You are  trying to MOVE an handle which does not exist. Max node index : " + (nodes.Length - 1) + " . Index wanted : " + i);
		}
		if (handleA)
		{
			handlesA[i] = p;
		}
		else
		{
			handlesB[i] = p;
		}
	}

	public int GetNumberOfSegment()
	{
		int num = nodes.Length - 1;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}
}
