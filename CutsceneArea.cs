using UnityEngine;

public class CutsceneArea : ObjectBase
{
	[Header("Framework")]
	public float Timer;

	public bool SetPosition;

	public bool SetDirection;

	public bool SnapToGround;

	public string AnimState;

	public string AnimTrigger;

	public bool CallAnimOnce;

	public bool DontCallAnim;

	public bool CallCameraCinematic;

	public float CinematicTime;

	public string CinematicTrigger;

	[Header("Optional")]
	public bool PlayTalkFace;

	public Vector2 TalkPoints;

	public Vector2 FaceIndex;

	public bool ManipulateIsVisible;

	public bool IsVisible;

	public int CutsceneID = int.MaxValue;

	private PlayerManager PM;

	private HeadController Head;

	private bool TalkPoint1Done;

	private bool TalkPoint2Done;

	private bool StateEnded;

	private bool Activated;

	private float StartTime;

	private void StateCutsceneStart()
	{
		PM.Base.SetState("Cutscene");
		StartTime = Time.time;
		if (CallAnimOnce && !DontCallAnim)
		{
			PM.Base.PlayAnimation(AnimState, AnimTrigger);
			if (CutsceneID != int.MaxValue)
			{
				PM.Base.Animator.SetInteger("Cutscene ID", CutsceneID);
			}
		}
		if (SnapToGround && !PM.Base.IsGrounded() && Physics.Raycast(PM.transform.position, -PM.transform.up, out var hitInfo, 5f, PM.Base.Collision_Mask))
		{
			PM.transform.position = hitInfo.point + PM.transform.up * 0.25f;
			Debug.DrawLine(PM.transform.position, hitInfo.point, Color.white);
		}
		if (PlayTalkFace && Singleton<Settings>.Instance.settings.Dialogue != 0)
		{
			Head = PM.GetComponent<HeadController>();
		}
		if (CallCameraCinematic)
		{
			PM.Base.Camera.PlayCinematic(CinematicTime, CinematicTrigger);
		}
	}

	private void StateCutscene()
	{
		PM.Base.SetState("Cutscene");
		if (!CallAnimOnce && !DontCallAnim)
		{
			PM.Base.PlayAnimation(AnimState, AnimTrigger);
			if (CutsceneID != int.MaxValue)
			{
				PM.Base.Animator.SetInteger("Cutscene ID", CutsceneID);
			}
		}
		PM.Base.LockControls = true;
		PM.Base.CurSpeed = 0f;
		PM.RBody.velocity = Vector3.zero;
		if (SetPosition)
		{
			PM.transform.position = base.transform.position + base.transform.up * 0.25f;
		}
		if (SetDirection)
		{
			PM.transform.forward = base.transform.forward;
		}
		if (PlayTalkFace && (bool)Head && Singleton<Settings>.Instance.settings.Dialogue != 0)
		{
			if (Time.time - StartTime > TalkPoints.x && !TalkPoint1Done)
			{
				Head.PlayFaceAnim((int)FaceIndex.x);
				TalkPoint1Done = true;
			}
			if (Time.time - StartTime > TalkPoints.y && !TalkPoint2Done)
			{
				Head.PlayFaceAnim((int)FaceIndex.y);
				TalkPoint2Done = true;
			}
		}
		if (ManipulateIsVisible)
		{
			PM.Base.IsVisible = IsVisible;
		}
		PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, PM.Base.IsGrounded() ? PM.Base.RaycastHit.normal : Vector3.up) * PM.transform.rotation;
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.transform.forward, PM.transform.up);
		if (Time.time - StartTime > Timer && !StateEnded)
		{
			if (PM.Base.IsGrounded())
			{
				PM.Base.PositionToPoint();
				PM.Base.SetMachineState("StateGround");
			}
			else
			{
				PM.Base.SetMachineState("StateAir");
			}
			Object.Destroy(base.gameObject);
			StateEnded = true;
		}
	}

	private void StateCutsceneEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !(player.GetState() == "Vehicle") && !Activated)
		{
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateCutscene);
			Activated = true;
		}
	}
}
