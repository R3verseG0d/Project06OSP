using UnityEngine;

public class PlayerGoal : MonoBehaviour
{
	[Header("Framework")]
	public Vector3 Cam_Pos;

	public Vector3 Cam_Tgt;

	public void SetParameters(Vector3 _Cam_Pos, Vector3 _Cam_Tgt)
	{
		Cam_Pos = _Cam_Pos;
		Cam_Tgt = _Cam_Tgt;
	}

	private void OnDrawGizmosSelected()
	{
		Debug.DrawLine(base.transform.position, Cam_Pos);
		Debug.DrawLine(Cam_Pos, Cam_Tgt);
	}
}
