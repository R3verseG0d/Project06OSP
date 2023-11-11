using UnityEngine;

public class FlyBillboard : MonoBehaviour
{
	internal float Speed;

	internal string Spline;

	private BezierCurve Curve;

	private float Progress;

	private void Start()
	{
		Curve = GameObject.Find(Spline).GetComponent<BezierCurve>();
	}

	private void Update()
	{
		Progress += Speed / Curve.Length() * Time.deltaTime;
		if (Progress > 1f)
		{
			Object.Destroy(base.gameObject);
		}
		base.transform.position = Curve.GetPosition(Progress);
		float num = Mathf.PerlinNoise(base.transform.position.x * 0.001f, base.transform.position.z * 0.001f) * 2f - 1f;
		float num2 = Mathf.PerlinNoise(base.transform.position.z * 0.001f, base.transform.position.y * 0.001f) * 2f - 1f;
		float num3 = Mathf.PerlinNoise(base.transform.position.z * 0.001f, base.transform.position.y * 0.001f) * 2f - 1f;
		Quaternion quaternion = Quaternion.AngleAxis(num * 25f * Time.deltaTime, Vector3.left);
		Quaternion quaternion2 = Quaternion.AngleAxis(num2 * 25f * Time.deltaTime, Vector3.up);
		Quaternion quaternion3 = Quaternion.AngleAxis(num3 * 25f * Time.deltaTime, Vector3.forward);
		base.transform.rotation *= quaternion * quaternion2 * quaternion3;
	}

	private void OnActorCreate(FlyBillboardParams Params)
	{
		Speed = Params.PathSpd;
		Spline = Params.Path;
	}
}
