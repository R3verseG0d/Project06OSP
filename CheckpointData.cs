using UnityEngine;

public class CheckpointData
{
	public Vector3 Position;

	public Quaternion Rotation;

	public int PlayerNo;

	public string PlayerPrefab;

	public string SavePoint;

	public bool Saved;

	public CheckpointData(Transform _Transform, int _PlayerNo, string _PlayerPrefab, string _SavePoint)
	{
		Position = _Transform.position;
		Rotation = _Transform.rotation;
		PlayerNo = _PlayerNo;
		PlayerPrefab = _PlayerPrefab;
		SavePoint = _SavePoint;
		Saved = true;
	}
}
