using System;
using UnityEngine;

[Serializable]
public class ObjectParams
{
	public enum Type
	{
		Enable = 0,
		Disable = 1,
		Switch = 2
	}

	public GameObject Object;

	public string OptionalMessage;

	public Type Mode;

	public float Timer;

	public float SwitchTimer;

	public string OptionalSwitchMessage;

	public Transform NewParent;

	public string OptionalParentMessage;
}
