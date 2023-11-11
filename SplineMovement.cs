using UnityEngine;

public class SplineMovement : MonoBehaviour
{
	public enum Action
	{
		Stop = 0,
		Reset = 1,
		Delete = 2,
		PingPong = 3
	}

	public enum Path
	{
		None = 0,
		CommonPathObj = 1
	}

	[Header("Framework")]
	public float Speed;

	public string Spline;

	[Header("Settings")]
	public Action FinishAction;

	public Path PathType;

	public bool ApplyRotation;

	public bool SmoothMovement;

	[Header("Optional")]
	public Animator Animator;

	public string TriggerName;

	public bool UseSpeedCurve;

	public AnimationCurve SpeedOverProgress = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	private BezierCurve Curve;

	private float Progress;

	private float SpeedCurveTime;

	private bool SwitchDir;

	public void SetParameters(float _Speed, string _Spline)
	{
		Speed = _Speed;
		Spline = _Spline;
	}

	private void Start()
	{
		if (!(Spline == ""))
		{
			if (PathType == Path.None)
			{
				Curve = GameObject.Find(Spline).GetComponent<BezierCurve>();
			}
			else
			{
				Curve = GameObject.Find("Stage/Splines/common_path_obj/" + Spline).GetComponent<BezierCurve>();
			}
			if ((bool)Animator)
			{
				Animator.SetTrigger(TriggerName);
			}
		}
	}

	private void Update()
	{
		if (Spline == "")
		{
			return;
		}
		if (UseSpeedCurve)
		{
			SpeedCurveTime += Time.deltaTime;
			Progress = SpeedOverProgress.Evaluate(SpeedCurveTime);
			Keyframe keyframe = SpeedOverProgress[SpeedOverProgress.length - 1];
			if (SpeedCurveTime > keyframe.time && FinishAction == Action.Delete)
			{
				Object.Destroy(base.gameObject);
			}
			if (SpeedCurveTime < keyframe.time)
			{
				base.transform.position = Curve.GetPosition(Progress);
				if (ApplyRotation)
				{
					base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
				}
			}
			return;
		}
		if (!SwitchDir)
		{
			Progress += Speed / Curve.Length() * Time.deltaTime;
		}
		else
		{
			Progress -= Speed / Curve.Length() * Time.deltaTime;
		}
		if (Progress > 1f)
		{
			switch (FinishAction)
			{
			case Action.Stop:
				Progress = 1f;
				break;
			case Action.Reset:
				Progress = 0f;
				break;
			case Action.Delete:
				Object.Destroy(base.gameObject);
				break;
			case Action.PingPong:
				SwitchDir = true;
				break;
			}
		}
		if (Progress < 0f && SwitchDir)
		{
			SwitchDir = false;
		}
		if (SmoothMovement)
		{
			float num = Progress / 1f;
			num = num * num * (3f - 2f * num);
			base.transform.position = Curve.GetPosition(num);
			if (ApplyRotation)
			{
				base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(num));
			}
		}
		else
		{
			base.transform.position = Curve.GetPosition(Progress);
			if (ApplyRotation)
			{
				base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
			}
		}
	}
}
