using System;
using UnityEngine;

[Serializable]
public class CameraParameters
{
	public int Mode;

	public Vector3 Position;

	public Vector3 Target;

	public Transform ObjPosition;

	public Transform ObjTarget;

	public CameraParameters(int _Mode, Vector3 _Position, Vector3 _Target, Transform _ObjPosition = null, Transform _ObjTarget = null)
	{
		Mode = _Mode;
		Position = _Position;
		Target = _Target;
		ObjPosition = _ObjPosition;
		ObjTarget = _ObjTarget;
	}
}
