using System.Collections.Generic;
using UnityEngine;

public class Freight : MonoBehaviour
{
	[Header("Framework")]
	public string Path;

	public float Speed;

	public float InitialProgress;

	public int Num;

	public int StartHalfway;

	public bool Loop;

	public string On1;

	public string On2;

	public string On3;

	public string On4;

	public string On5;

	public string On6;

	public string On7;

	public string On8;

	public string On9;

	public string On10;

	[Header("Prefab")]
	public GameObject CargoObj;

	public Transform[] Wheels;

	public AudioSource HornSource;

	public AudioSource TrainRunSource;

	[Header("Object Framework")]
	public float CargoOffset;

	internal BezierCurve Curve;

	internal float Progress;

	private List<Cargo> JointedCargos = new List<Cargo>();

	private bool StartProgress;

	public void SetParameters(string _Path, float _Speed, float _InitialProgress, int _Num, int _StartHalfway, bool _Loop, string _On1, string _On2, string _On3, string _On4, string _On5, string _On6, string _On7, string _On8, string _On9, string _On10)
	{
		Path = _Path;
		Speed = _Speed;
		InitialProgress = _InitialProgress;
		Num = _Num;
		StartHalfway = _StartHalfway;
		Loop = _Loop;
		On1 = _On1;
		On2 = _On2;
		On3 = _On3;
		On4 = _On4;
		On5 = _On5;
		On6 = _On6;
		On7 = _On7;
		On8 = _On8;
		On9 = _On9;
		On10 = _On10;
	}

	private void Start()
	{
		Curve = GameObject.Find(Path).GetComponent<BezierCurve>();
		if (StartHalfway == 1)
		{
			Progress = 0.5f;
		}
		else
		{
			Progress = Curve.FindNearestPointToProgress(base.transform.position);
			Progress += InitialProgress;
		}
		base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
		float num = CargoOffset;
		string[] array = new string[10] { On1, On2, On3, On4, On5, On6, On7, On8, On9, On10 };
		for (int i = 1; i < Num; i++)
		{
			if (i > 1)
			{
				num += CargoOffset * ((i < 2) ? 1f : 0.9f);
			}
			Cargo component = Object.Instantiate(CargoObj, Curve.GetTangent(0f), Quaternion.LookRotation(Curve.GetTangent(0f))).GetComponent<Cargo>();
			component.Curve = Curve;
			component.Speed = Speed;
			component.Progress = Progress - num;
			component.Loop = Loop;
			component.CargoID = array[i];
			JointedCargos.Add(component);
		}
		TrainRunSource.time = Random.Range(0.1f, TrainRunSource.clip.length);
	}

	private void Update()
	{
		if (StartProgress)
		{
			Progress += Speed / Curve.Length() * Time.deltaTime;
		}
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
			Wheels[i].Rotate(Speed * Time.deltaTime * 75f, 0f, 0f);
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
	}

	private void Horn()
	{
		HornSource.Play();
	}
}
