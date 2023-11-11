using System;
using UnityEngine;

[Serializable]
public class LightPreset
{
	public string PresetName;

	[ColorUsage(false)]
	public Color LightColor;

	public float Alpha;

	public Vector3 Direction_3dsmax;
}
