using UnityEngine;

public class SplineObjectPlacer : MonoBehaviour
{
	public BezierCurve BezierCurve;

	public GameObject Prefab;

	public float Rate;

	[Header("Optional")]
	public Transform Parent;

	public bool ReadRotation;

	public bool IgnoreFirstAndFinalPoints;

	public Vector3 Offset;

	public int ContinueNameIndexFrom;

	[Header("Rings")]
	public bool ApplyRingParams;

	public bool IsGroundLightDash;

	private RailData[] RailPathData;

	private float[] RingSplineTime;

	public RailData GetRailData(float time)
	{
		_ = Vector3.zero;
		Vector3 forward = Vector3.forward;
		Vector3 up = Vector3.up;
		Vector3 position = BezierCurve.GetPosition(time);
		forward = BezierCurve.GetTangent(time);
		return new RailData(position, up, forward);
	}

	public float Length()
	{
		return BezierCurve.Length();
	}

	public RailData[] GetRailDataArray(float Distance = 1f)
	{
		int num = Mathf.FloorToInt(Length() * Distance);
		RailData[] array = new RailData[num + 1];
		array[0] = GetRailData(0f);
		for (int i = 0; i < num; i++)
		{
			float num2 = ((float)i + 1f) / ((float)num * 1f);
			if (num2 > 1f)
			{
				Debug.LogError("Time is greater than 1.0f; index: " + i + ", count: " + num);
				num2 = 1f;
			}
			array[i + 1] = GetRailData(num2);
		}
		return array;
	}

	public float[] GetTime(float Distance = 1f)
	{
		int num = Mathf.FloorToInt(Length() * Distance);
		float[] array = new float[num + 1];
		for (int i = 0; i < num; i++)
		{
			float num2 = ((float)i + 1f) / (float)num;
			if (num2 > 1f)
			{
				Debug.LogError("Time is greater than 1.0f; index: " + i + ", count: " + num);
				num2 = 1f;
			}
			array[i + 1] = num2;
		}
		return array;
	}
}
