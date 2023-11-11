using UnityEngine;

public class JumpSplinter : ObjectBase
{
	[Header(".SET Params")]
	public Vector3 TargetPos;

	[Header("Other")]
	public ChainJump ChainJumpParam;

	public Transform FirstJumpPos;

	public Animator[] Animations;

	public void SetParameters(Vector3 _TargetPos)
	{
		TargetPos = _TargetPos;
		ChainJumpParam.LandPosition = _TargetPos;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!(player == null) && !player.IsDead && player.GetPrefab("sonic_new"))
		{
			collider.transform.position = FirstJumpPos.position + FirstJumpPos.up * 0.25f;
			for (int i = 0; i < Animations.Length; i++)
			{
				Animations[i].SetTrigger("Play Animation");
			}
		}
	}
}
