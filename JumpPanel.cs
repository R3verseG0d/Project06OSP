using UnityEngine;

public class JumpPanel : ObjectBase
{
	public enum Mode
	{
		FixedLanding = 0,
		FixedTarget = 1
	}

	[Header("Framework")]
	public float YOffset;

	public float Speed;

	public float Timer;

	public Vector3 TargetPosition;

	[Header("Prefab")]
	public bool UseTimerToExit;

	public bool UseTimerToRelease;

	public bool AlwaysLocked;

	public Mode JumpPanelMode;

	public GameObject Mesh;

	public bool AddYOffsetVel;

	public AudioSource Audio;

	public AudioClip Sound;

	public AudioClip[] WindSounds;

	[Header("Optional")]
	public bool IsNotVisible;

	public GameObject CamEventObj;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 StartLaunchVelocity;

	private Quaternion MeshLaunchRot;

	private bool Falling;

	private bool OnCStop;

	private float StartTime;

	private float Distance;

	private string LaunchAnimMode;

	public void SetParameters(float _YOffset, float _Speed, float _Timer, Vector3 _TargetPosition)
	{
		YOffset = _YOffset;
		Speed = _Speed;
		Timer = _Timer;
		TargetPosition = _TargetPosition;
		base.transform.rotation = Quaternion.LookRotation(MeshDirection());
	}

	private void Start()
	{
		if (IsNotVisible)
		{
			Mesh.SetActive(value: false);
		}
	}

	private void StateJumpPanelStart()
	{
		PM.Base.SetState("JumpPanel");
		StartTime = Time.time;
		LaunchVelocity = Direction(base.transform) * Speed;
		StartLaunchVelocity = ((!PM.Base.GetPrefab("sonic_fast")) ? LaunchVelocity.normalized : LaunchVelocity.MakePlanar());
		PM.transform.position = base.transform.position + PM.transform.up * 0.25f;
		MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 30f : 90f, 0f, 0f);
		PM.transform.forward = LaunchVelocity.MakePlanar();
		PM.Base.MaxRayLenght = (PM.Base.GetPrefab("sonic_fast") ? 1.75f : 0.75f);
		Falling = false;
		OnCStop = false;
	}

	private void StateJumpPanel()
	{
		PM.Base.SetState("JumpPanel");
		bool flag = Time.time - StartTime < ((!UseTimerToExit) ? Timer : 0f);
		if (flag)
		{
			if (LaunchAnimMode == "jumppanel_a" || LaunchAnimMode == "jumppanel_c")
			{
				PM.Base.PlayAnimation("Spring Jump", "On Spring");
			}
			else if (PM.Base.GetPrefab("silver") || PM.Base.GetPrefab("omega"))
			{
				PM.Base.PlayAnimation("Spring Jump", "On Spring");
			}
			else
			{
				PM.Base.PlayAnimation("Rolling", "On Roll");
			}
			Falling = false;
		}
		else if (LaunchAnimMode == "jumppanel_a" || LaunchAnimMode == "jumppanel_c")
		{
			if (PM.RBody.velocity.y > -0.1f)
			{
				PM.Base.PlayAnimation("Spring Jump", "On Spring");
				Falling = false;
			}
			else if (!Falling)
			{
				Falling = true;
				PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
		}
		else if (LaunchAnimMode == "jumppanel_b" || LaunchAnimMode == "jumppanel_d")
		{
			if (PM.Base.GetPrefab("silver") || PM.Base.GetPrefab("omega"))
			{
				if (PM.RBody.velocity.y > -0.1f)
				{
					PM.Base.PlayAnimation("Spring Jump", "On Spring");
					Falling = false;
				}
				else if (!Falling)
				{
					Falling = true;
					PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
				}
			}
			else if (PM.RBody.velocity.y > -0.1f)
			{
				PM.Base.PlayAnimation("Rolling", "On Roll");
			}
			else
			{
				PM.Base.PlayAnimation("Falling", "On Fall");
			}
		}
		if (flag)
		{
			if ((LaunchAnimMode == "jumppanel_b" || LaunchAnimMode == "jumppanel_d") && Time.time - StartTime > Timer * 0.3f)
			{
				LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			}
			else if (LaunchAnimMode == "jumppanel_a" || LaunchAnimMode == "jumppanel_c")
			{
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f), Time.fixedDeltaTime * 5f);
			}
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.RBody.velocity = LaunchVelocity;
			PM.Base.LockControls = true;
		}
		else
		{
			if (LaunchAnimMode == "jumppanel_a" || LaunchAnimMode == "jumppanel_d")
			{
				if (UseTimerToRelease && !AlwaysLocked)
				{
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
					PM.Base.GeneralMeshRotation.x = MeshLaunchRot.x;
					PM.Base.GeneralMeshRotation.z = MeshLaunchRot.z;
					Vector3 vector = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
					if (PM.RBody.velocity.magnitude != 0f)
					{
						vector = PM.transform.forward * PM.Base.CurSpeed;
						LaunchVelocity = new Vector3(vector.x, LaunchVelocity.y, vector.z);
					}
					LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
					PM.Base.DoWallNormal();
					PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
				}
				else
				{
					PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
					PM.transform.forward = LaunchVelocity.MakePlanar();
					LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					PM.RBody.velocity = LaunchVelocity;
					PM.Base.LockControls = true;
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (LaunchAnimMode == "jumppanel_b")
			{
				if (!UseTimerToExit && !AlwaysLocked)
				{
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
					PM.Base.GeneralMeshRotation.x = MeshLaunchRot.x;
					PM.Base.GeneralMeshRotation.z = MeshLaunchRot.z;
					Vector3 vector2 = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
					if (PM.RBody.velocity.magnitude != 0f)
					{
						vector2 = PM.transform.forward * PM.Base.CurSpeed;
						LaunchVelocity = new Vector3(vector2.x, LaunchVelocity.y, vector2.z);
					}
					LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
					PM.Base.DoWallNormal();
					PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
				}
				else
				{
					PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
					PM.transform.forward = LaunchVelocity.MakePlanar();
					LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					PM.RBody.velocity = LaunchVelocity;
					PM.Base.LockControls = true;
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (LaunchAnimMode == "jumppanel_c")
			{
				if (PM.RBody.velocity.y < 0f && !PM.Base.GetPrefab("sonic_fast") && !AlwaysLocked)
				{
					if (!OnCStop)
					{
						if (PM.Base.CurSpeed > PM.Base.TopSpeed)
						{
							PM.Base.CurSpeed = PM.Base.TopSpeed;
						}
						OnCStop = true;
					}
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
					PM.Base.GeneralMeshRotation.x = MeshLaunchRot.x;
					PM.Base.GeneralMeshRotation.z = MeshLaunchRot.z;
					Vector3 vector3 = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
					if (PM.RBody.velocity.magnitude != 0f)
					{
						vector3 = PM.transform.forward * PM.Base.CurSpeed;
						LaunchVelocity = new Vector3(vector3.x, LaunchVelocity.y, vector3.z);
					}
					LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
					LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
					PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
					PM.Base.DoWallNormal();
				}
				else
				{
					PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
					PM.transform.forward = LaunchVelocity.MakePlanar();
					LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					PM.RBody.velocity = LaunchVelocity;
					PM.Base.LockControls = true;
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler((!PM.Base.GetPrefab("sonic_fast")) ? 90f : 0f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			PM.RBody.velocity = LaunchVelocity;
		}
		if (PM.Base.LockControls)
		{
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
			PM.Base.TargetDirection = Vector3.zero;
		}
		if (PM.Base.IsGrounded() && ((Timer != 0f && !UseTimerToExit) ? (!flag) : (Time.time - StartTime > 0.1f)))
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
		if (PM.Base.FrontalCollision && ((Timer != 0f && !UseTimerToExit) ? (!flag) : (Time.time - StartTime > 0.1f)))
		{
			PM.Base.SetMachineState("StateAir");
		}
	}

	private void StateJumpPanelEnd()
	{
	}

	private Vector3 MeshDirection()
	{
		if (TargetPosition == Vector3.zero)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.x = 0f - YOffset;
			return Quaternion.Euler(eulerAngles) * Vector3.forward;
		}
		Distance = Vector3.Distance(base.transform.position, TargetPosition);
		if ((TargetPosition.y > base.transform.position.y && Distance < 50f) || YOffset == 0f)
		{
			return new Vector3(base.transform.forward.x, (TargetPosition - base.transform.position).normalized.y, base.transform.forward.z);
		}
		Vector3 eulerAngles2 = base.transform.eulerAngles;
		eulerAngles2.x = 0f - YOffset;
		return Quaternion.Euler(eulerAngles2) * Vector3.forward;
	}

	private Vector3 Direction(Transform _transform)
	{
		if (TargetPosition == Vector3.zero)
		{
			return base.transform.forward;
		}
		Vector3 normalized = (TargetPosition - _transform.position).normalized;
		if (JumpPanelMode == Mode.FixedTarget)
		{
			return normalized;
		}
		return new Vector3(normalized.x, base.transform.forward.y + (AddYOffsetVel ? YOffset : 0f), normalized.z).normalized;
	}

	private string AnimMode(Transform _Transform)
	{
		if (TargetPosition != Vector3.zero)
		{
			if (!(Vector3.Distance(base.transform.GetChild(0).position, TargetPosition) < 50f))
			{
				if (!(Vector3.Dot(Direction(_Transform), Vector3.up) > 0.5f))
				{
					return "jumppanel_a";
				}
				return "jumppanel_c";
			}
			return "jumppanel_a";
		}
		if (!(Vector3.Dot(Direction(_Transform), Vector3.up) > 0.75f))
		{
			return "jumppanel_b";
		}
		return "jumppanel_d";
	}

	private void OnTrigger(AudioSource _Audio)
	{
		if (!IsNotVisible)
		{
			Audio.PlayOneShot(Sound, Audio.volume);
			_Audio.PlayOneShot(WindSounds[Random.Range(0, WindSounds.Length)], _Audio.volume);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		VehicleBase componentInParent = collider.GetComponentInParent<VehicleBase>();
		if (collider.gameObject.tag != "Player" && (bool)componentInParent && !componentInParent.IsLaunched)
		{
			componentInParent.OnJumpPanel(Direction(componentInParent.transform) * Speed);
			OnTrigger(Audio);
		}
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetState() != "Vehicle" && !player.IsDead)
		{
			PM = collider.GetComponent<PlayerManager>();
			OnTrigger(player.Audio);
			LaunchAnimMode = AnimMode(player.transform);
			player.StateMachine.ChangeState(base.gameObject, StateJumpPanel);
			if ((bool)CamEventObj)
			{
				CamEventObj.SetActive(value: true);
				CamEventObj.GetComponent<CameraEvents>().DestroyOnExit = true;
			}
		}
		AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
		if ((bool)component && component.GetAmigoState() != "Vehicle" && !component.IsDead)
		{
			OnTrigger(component.Audio);
			component.OnJumpPanel(base.transform.position, Direction(base.transform) * Speed, Timer, AnimMode(component.transform), UseTimerToExit, UseTimerToRelease, AlwaysLocked);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetPosition != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(TargetPosition, 1f);
		}
		Vector3 vector = base.transform.position + Direction(base.transform) * Speed * ((!UseTimerToExit) ? Timer : 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, vector);
		float num = 0.01f;
		Vector3 vector2 = Direction(base.transform) * Speed;
		Gizmos.color = ((!AlwaysLocked) ? Color.green : Color.white);
		Vector3 vector3 = vector;
		Vector3 from = vector;
		while (vector3.y > TargetPosition.y)
		{
			vector2.y -= 9.81f * num;
			vector3 += vector2 * num;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
