using System;
using UnityEngine;

[Serializable]
public class FlameCoreParams
{
	public int ShootIndex;

	public GameObject Target;

	public FlameCoreParams(int _ShootIndex, GameObject _Target)
	{
		ShootIndex = _ShootIndex;
		Target = _Target;
	}
}
