using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
	[Header("Framework")]
	public string Path;

	public float Speed;

	public float InitialProgress;

	public int Num;

	public Vector3 m_Camera;

	public bool Loop;

	[Header("Prefab")]
	public GameObject CargoObj;

	public Transform[] Wheels;

	public GameObject[] Renderers;

	public GameObject FrontCoverBrk;

	public GameObject[] WindowsBrk;

	public Transform FrontCoverBrkPoint;

	public Transform[] PartBPoints;

	public Transform[] PartCPoints;

	public AudioSource HornSource;

	public AudioSource StopSource;

	public AudioSource TrainRunSource;

	[Header("Object Framework")]
	public float CargoOffset;

	internal BezierCurve Curve;

	internal float Progress;

	private List<Cargo> JointedCargos = new List<Cargo>();

	private bool StartProgress;

	private float SplineSpeed;

	public void SetParameters(string _Path, float _Speed, float _InitialProgress, int _Num, bool _Loop, Vector3 _Camera)
	{
		Path = _Path;
		Speed = _Speed;
		InitialProgress = _InitialProgress;
		Num = _Num;
		Loop = _Loop;
		m_Camera = _Camera;
	}

	private void Start()
	{
		Curve = GameObject.Find(Path).GetComponent<BezierCurve>();
		Progress = InitialProgress;
		base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
		float num = CargoOffset;
		for (int i = 1; i < Num; i++)
		{
			if (i > 1)
			{
				num += CargoOffset;
			}
			Cargo component = Object.Instantiate(CargoObj, Curve.GetTangent(0f), Quaternion.LookRotation(Curve.GetTangent(0f))).GetComponent<Cargo>();
			component.Curve = Curve;
			component.Speed = Speed;
			component.SplineSpeed = SplineSpeed;
			component.Progress = Progress - num;
			component.Loop = Loop;
			JointedCargos.Add(component);
		}
		TrainRunSource.time = Random.Range(0.1f, TrainRunSource.clip.length);
	}

	private void Update()
	{
		SplineSpeed = Mathf.Lerp(SplineSpeed, StartProgress ? Speed : 0f, Time.deltaTime * 5f);
		Progress += SplineSpeed / Curve.Length() * Time.deltaTime;
		if (Progress >= 1f)
		{
			if (!Loop)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				Progress = 0f;
			}
		}
		for (int i = 0; i < Wheels.Length; i++)
		{
			Wheels[i].Rotate(SplineSpeed * Time.deltaTime * 75f, 0f, 0f);
		}
		base.transform.position = Curve.GetPosition(Progress);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Curve.GetTangent(Progress)), Time.deltaTime * 5f);
		TrainRunSource.volume = Mathf.Lerp(TrainRunSource.volume, StartProgress ? 0.75f : 0f, Time.deltaTime * 5f);
	}

	private void Go()
	{
		StartProgress = true;
		for (int i = 0; i < JointedCargos.Count; i++)
		{
			JointedCargos[i].StartProgress = true;
		}
	}

	private void Stop()
	{
		StartProgress = false;
		for (int i = 0; i < JointedCargos.Count; i++)
		{
			JointedCargos[i].StartProgress = false;
		}
		StopSource.Play();
	}

	private void Horn()
	{
		HornSource.Play();
	}

	private void Camera()
	{
		Object.FindObjectOfType<PlayerBase>().SetCameraParams(new CameraParameters(3, m_Camera, Vector3.zero, null, base.transform));
	}

	private void Bomb()
	{
		Object.FindObjectOfType<StageManager>().StageState = StageManager.State.Event;
		StartCoroutine(Object.FindObjectOfType<PlayerBase>().RestartStage());
		Renderers[0].SetActive(value: false);
		Renderers[1].SetActive(value: true);
		Object.Instantiate(FrontCoverBrk, FrontCoverBrkPoint.position, FrontCoverBrkPoint.rotation);
		for (int i = 0; i < PartBPoints.Length; i++)
		{
			Object.Instantiate(WindowsBrk[0], PartBPoints[i].position, PartBPoints[i].rotation);
		}
		for (int j = 0; j < PartCPoints.Length; j++)
		{
			Object.Instantiate(WindowsBrk[1], PartCPoints[j].position, PartCPoints[j].rotation);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (m_Camera != Vector3.zero)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(base.transform.position, m_Camera);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(m_Camera, 0.5f);
		}
	}
}
