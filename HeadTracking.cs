using STHEngine;
using UnityEngine;

public class HeadTracking : MonoBehaviour
{
	public Transform HeadObj;

	public PlayerBase Base;

	public bool LockY;

	private GameObject Target;

	private Quaternion TargetRotation;

	private float Lerp;

	private float VectorAngle;

	private void LateUpdate()
	{
		Target = FindInterestPoint();
		if ((bool)Target)
		{
			Vector3 vector = Target.transform.position - Target.transform.up * 0.75f;
			Vector3 from = HeadObj.position - vector;
			VectorAngle = Vector3.Angle(from, base.transform.forward);
			Vector3 vector2 = HeadObj.position - vector;
			Vector3 vector3 = -base.transform.forward;
			float value = Vector3.Angle(vector3, vector2.normalized);
			Vector3 forward = Quaternion.AngleAxis(axis: Vector3.Cross(vector3, vector2.normalized), angle: Mathf.Clamp(value, -35f, 35f)) * vector3;
			TargetRotation = Quaternion.Slerp(TargetRotation, Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 0f, -90f), Time.deltaTime * 10f);
			if (LockY)
			{
				TargetRotation = Quaternion.Euler(new Vector3(HeadObj.eulerAngles.x, TargetRotation.eulerAngles.y, HeadObj.eulerAngles.z));
			}
		}
		else
		{
			TargetRotation = Quaternion.Slerp(TargetRotation, HeadObj.rotation, Time.deltaTime * 10f);
		}
		bool flag = Base.GetState() == "Ground" && ((!Base.GetPrefab("snow_board") && !Base.GetPrefab("sonic_fast") && Base.Animator.GetCurrentAnimatorStateInfo(1).IsName("Idle")) || Base.GetPrefab("snow_board") || Base.GetPrefab("sonic_fast"));
		Lerp = Mathf.Lerp(Lerp, (flag && (bool)Target && VectorAngle > 60f) ? 1f : 0f, Time.deltaTime * 5f);
		HeadObj.rotation = Quaternion.Slerp(HeadObj.rotation, TargetRotation, Lerp);
	}

	private float ClampAngle(float Angle, float Min, float Max)
	{
		if (Angle < -360f)
		{
			Angle += 360f;
		}
		if (Angle > 360f)
		{
			Angle -= 360f;
		}
		return Mathf.Clamp(Angle, Min, Max);
	}

	internal GameObject FindInterestPoint()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("InterestPoint");
		GameObject result = null;
		float num = 12.5f;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			if (num2 < num && Base.GetState() == "Ground" && !Base.IsOnWall && !Physics.Linecast(base.transform.position, gameObject.transform.position, ExtensionMethods.HomingBlock_Mask))
			{
				result = gameObject;
				num = num2;
			}
		}
		return result;
	}
}
