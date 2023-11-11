using System;
using UnityEngine;

[Serializable]
public class AxleInfo
{
	public WheelCollider LeftWheel;

	public WheelCollider RightWheel;

	public bool Motor;

	public bool Steer;

	public bool Brake;

	public float AntiRoll;
}
