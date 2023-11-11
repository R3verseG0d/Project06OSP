using System;

[Serializable]
public class FlyBillboardParams
{
	public float PathSpd;

	public string Path;

	public FlyBillboardParams(float _PathSpd, string _Path)
	{
		PathSpd = _PathSpd;
		Path = _Path;
	}
}
