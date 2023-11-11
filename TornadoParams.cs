using System;
using UnityEngine;

[Serializable]
public class TornadoParams
{
	public int ShootIndex;

	public GameObject Target;

	public string TargetName;

	public float Time;

	public Vector3 TargetPos;

	public TornadoParams(int _ShootIndex, GameObject _Target, string _TargetName, float _Time, Vector3 _TargetPos)
	{
		ShootIndex = _ShootIndex;
		Target = _Target;
		TargetName = _TargetName;
		Time = _Time;
		TargetPos = _TargetPos;
	}
}
