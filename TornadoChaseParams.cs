using System;
using UnityEngine;

[Serializable]
public class TornadoChaseParams
{
	public int ShootIndex;

	public Vector3 TargetPos;

	public TornadoChaseParams(int _ShootIndex, Vector3 _TargetPos)
	{
		ShootIndex = _ShootIndex;
		TargetPos = _TargetPos;
	}
}
