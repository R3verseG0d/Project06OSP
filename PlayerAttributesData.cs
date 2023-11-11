using System;

[Serializable]
public class PlayerAttributesData
{
	public string VarName;

	public bool VarBool;

	public float VarFloat;

	public int VarInt;

	public bool[] VarBoolArray;

	public float[] VarFloatArray;

	public int[] VarIntArray;

	public PlayerAttributesData(string _VarName, bool _VarBool = false, float _VarFloat = 0f, int _VarInt = 0)
	{
		VarName = _VarName;
		VarBool = _VarBool;
		VarFloat = _VarFloat;
		VarInt = _VarInt;
	}

	public PlayerAttributesData(string _VarName, bool[] _VarBoolArray = null, float[] _VarFloatArray = null, int[] _VarIntArray = null)
	{
		VarName = _VarName;
		VarBoolArray = _VarBoolArray;
		VarFloatArray = _VarFloatArray;
		VarIntArray = _VarIntArray;
	}
}
