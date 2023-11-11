using UnityEngine;

public class RailSystem : MonoBehaviour
{
	public enum Mode
	{
		Single = 0,
		Dual = 1
	}

	public BezierCurve bezierCurve;

	public BezierCurve bezierCurve_R;

	public float ColliderWidth = 0.2f;

	public Mode SplineMode;

	[SerializeField]
	internal RailData[] RailPathData;

	private void Start()
	{
		if (RailPathData == null)
		{
			RailPathData = GetRailDataArray();
		}
	}

	public RailData[] GetRailPathData()
	{
		return RailPathData;
	}

	public RailData GetRailData(float time)
	{
		Vector3 zero = Vector3.zero;
		Vector3 forward = Vector3.forward;
		Vector3 normal = Vector3.up;
		if (SplineMode == Mode.Single)
		{
			zero = bezierCurve.GetPosition(time);
			forward = bezierCurve.GetTangent(time);
		}
		else
		{
			Vector3 position = bezierCurve.GetPosition(time);
			Vector3 position2 = bezierCurve_R.GetPosition(time);
			zero = (position + position2) * 0.5f;
			forward = ((bezierCurve.GetTangent(time) + bezierCurve_R.GetTangent(time)) * 0.5f).normalized;
			normal = Vector3.Cross(forward, (position2 - zero).normalized);
		}
		return new RailData(zero, normal, forward);
	}

	public float Length()
	{
		if (SplineMode == Mode.Single)
		{
			return bezierCurve.Length();
		}
		return (bezierCurve.Length() + bezierCurve_R.Length()) * 0.5f;
	}

	public Vector3[] TangentArray(float Distance = 1f)
	{
		int num = Mathf.FloorToInt(bezierCurve.Length() / Distance);
		Vector3[] array = new Vector3[num + 1];
		array[0] = bezierCurve.GetTangent(0f);
		for (int i = 0; i < num; i++)
		{
			float num2 = ((float)i + 1f) / ((float)num * 1f);
			if (num2 > 1f)
			{
				Debug.LogError("Time is greater than 1.0f; index: " + i + ", count: " + num);
				num2 = 1f;
			}
			array[i] = bezierCurve.GetTangent(num2);
		}
		return array;
	}

	public RailData[] GetRailDataArray(float Distance = 1f)
	{
		int num = Mathf.FloorToInt(Length() / Distance);
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
}
