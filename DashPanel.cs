using UnityEngine;

public class DashPanel : ObjectBase
{
	[Header("Framework")]
	public float Speed;

	public float Timer;

	[Header("Prefab")]
	public AudioSource Audio;

	private PlayerManager PM;

	private EffectsBase EffBase;

	private bool IsRoll;

	private float StartTime;

	public void SetParameters(float _Speed, float _Timer)
	{
		Speed = _Speed;
		Timer = _Timer;
	}

	private void StateDashPanelStart()
	{
		PM.Base.SetState("DashPanel");
		StartTime = Time.time;
		PM.Base.LockControls = true;
		PM.transform.rotation = base.transform.rotation;
		PM.Base.Animator.SetTrigger("Additive Idle");
	}

	private void StateDashPanel()
	{
		PM.Base.SetState("DashPanel");
		PM.Base.PlayAnimation(EffBase.DashPadRoll ? "Spin" : "Movement (Blend Tree)", EffBase.DashPadRoll ? "On Spin" : "On Ground");
		PM.Base.LockControls = true;
		PM.Base.MaxRayLenght = 1.25f;
		if (!PM.Base.GetPrefab("sonic_fast"))
		{
			float curSpeed = Speed * (EffBase.DashPadRoll ? 1.25f : ((Speed < PM.Base.TopSpeed) ? 1.5f : 1f));
			if (EffBase.DashPadRoll && Speed >= 55f)
			{
				curSpeed = 55f;
			}
			PM.Base.CurSpeed = curSpeed;
		}
		PM.RBody.velocity = Vector3.ProjectOnPlane(PM.transform.forward, PM.Base.RaycastHit.normal) * PM.Base.CurSpeed;
		PM.Base.TargetDirection = Vector3.zero;
		PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, PM.Base.RaycastHit.normal) * PM.transform.rotation;
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
		if (Time.time - StartTime > Timer * 0.5f)
		{
			PM.Base.MaxRayLenght = 1.25f;
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState(EffBase.DashPadRoll ? "StateSpinDashUncurl" : "StateGround");
		}
		if (!PM.Base.IsGrounded())
		{
			EffBase.DashPadRoll = false;
			PM.Base.SetMachineState("StateAir");
		}
		if (PM.Base.ShouldAlignOrFall(Align: false) && PM.Base.FrontalCollision)
		{
			PM.Base.MaxRayLenght = 1.25f;
			EffBase.DashPadRoll = false;
			if (PM.Base.IsOnWall)
			{
				PM.Base.SetMachineState("StateAir");
				return;
			}
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateHurt");
		}
	}

	private void StateDashPanelEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetState() != "Vehicle" && !player.IsDead)
		{
			Audio.Play();
			PM = collider.GetComponent<PlayerManager>();
			EffBase = collider.GetComponent<EffectsBase>();
			Vector3 up = base.transform.up;
			if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo, 1f))
			{
				up = hitInfo.normal;
			}
			PM.transform.up = up;
			if (player.GetPrefab("snow_board"))
			{
				PM.snow_board.ManageBoard(AddBoard: false);
			}
			if (player.GetState() == "Path")
			{
				player.PathSpeed = Speed * ((Speed < PM.Base.TopSpeed) ? 1.5f : 1f);
				if (Vector3.Dot(player.LinearBezier.GetTangent(player.PathTime) * player.PathMoveDir, base.transform.forward) < 0f)
				{
					player.PathMoveDir *= -1;
				}
				if (!player.PathDashpanel)
				{
					player.PathDashpanel = true;
				}
			}
			else if (player.GetState() == "Grinding")
			{
				player.GrindSpeed = Speed * ((Speed < PM.Base.TopSpeed) ? 1.5f : 1f);
				if (Vector3.Dot(player.RailData.tangent * player.GrindDir, base.transform.forward) < 0f)
				{
					player.GrindDir *= -1f;
				}
			}
			else
			{
				if ((player.GetPrefab("sonic_new") || player.GetPrefab("shadow") || player.GetPrefab("metal_sonic")) && (player.GetState() == "SpinDash" || (player.GetState() == "DashPanel" && EffBase.DashPadRoll)))
				{
					EffBase.DashPadRoll = true;
				}
				else
				{
					EffBase.DashPadRoll = false;
				}
				player.StateMachine.ChangeState(base.gameObject, StateDashPanel);
			}
		}
		AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
		if ((bool)component && component.GetAmigoState() != "Vehicle" && !component.IsDead)
		{
			Audio.Play();
			Vector3 normal = base.transform.up;
			if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -base.transform.up, out var hitInfo2, 1f))
			{
				normal = hitInfo2.normal;
			}
			component.OnDashPanel(base.transform.rotation, normal, base.transform.forward, Speed, Timer);
		}
	}
}
