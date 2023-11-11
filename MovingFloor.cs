using UnityEngine;

public class MovingFloor : MonoBehaviour
{
	[Header("Framework")]
	public float On_Time;

	public float Off_Time;

	public float Appear_Time;

	public float Disappear_Time;

	[Header("Prefab")]
	public Rigidbody Platform;

	public AudioSource Audio;

	private Vector3 OpenedPosition;

	private Vector3 ClosedPosition;

	private bool Closed;

	private bool InProcess;

	private float StartTime;

	private float WaitTime;

	private float TransitionTime;

	public void SetParameters(float _On_Time, float _Off_Time, float _Appear_Time, float _Disappear_Time)
	{
		On_Time = _On_Time;
		Off_Time = _Off_Time;
		Appear_Time = _Appear_Time;
		Disappear_Time = _Disappear_Time;
	}

	private void Start()
	{
		OpenedPosition = Platform.transform.position;
		ClosedPosition = Platform.transform.position + base.transform.forward * -6.3f;
		Platform.transform.position = (Closed ? ClosedPosition : OpenedPosition);
	}

	private void FixedUpdate()
	{
		if (!InProcess)
		{
			InProcess = true;
			Audio.Play();
			StartTime = Time.time;
			WaitTime = (Closed ? Off_Time : On_Time);
			TransitionTime = (Closed ? Appear_Time : Disappear_Time);
		}
		if (Time.time - StartTime < TransitionTime)
		{
			float t = (Time.time - StartTime) / TransitionTime;
			Vector3 position = (Closed ? Vector3.Lerp(OpenedPosition, ClosedPosition, t) : Vector3.Lerp(ClosedPosition, OpenedPosition, t));
			Platform.MovePosition(position);
			return;
		}
		Platform.transform.position = (Closed ? ClosedPosition : OpenedPosition);
		if (Time.time - StartTime - TransitionTime > WaitTime)
		{
			Closed = !Closed;
			InProcess = false;
		}
	}
}
