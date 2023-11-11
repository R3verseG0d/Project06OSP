using UnityEngine;

public class Spring : ObjectBase
{
	public enum Mode
	{
		FixedLanding = 0,
		FixedTarget = 1
	}

	[Header("Framework")]
	public float Speed;

	public float Timer;

	public Vector3 TargetPosition;

	[Header("Prefab")]
	public bool UseTimerToExit;

	public bool UseTimerToRelease;

	public bool AlwaysLocked;

	public Mode SpringMode;

	public GameObject Mesh;

	public Animation Animation;

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

	private float StartTime;

	private bool Falling;

	private bool OnStop;

	private string LaunchAnimMode;

	public void SetParameters(float _Speed, float _Timer, Vector3 _TargetPosition)
	{
		Speed = _Speed;
		Timer = _Timer;
		TargetPosition = _TargetPosition;
	}

	private void Start()
	{
		if (IsNotVisible)
		{
			Mesh.SetActive(value: false);
		}
	}

	private void StateSpringStart()
	{
		PM.Base.SetState("Spring");
		StartTime = Time.time;
		LaunchVelocity = Direction(base.transform.GetChild(0)) * Speed;
		StartLaunchVelocity = LaunchVelocity.normalized;
		PM.transform.position = base.transform.GetChild(0).position - base.transform.GetChild(0).up * 0.25f;
		float num = Vector3.Distance(PM.transform.forward.normalized, LaunchVelocity.MakePlanar().normalized);
		float num2 = Vector3.Dot(PM.transform.right.MakePlanar(), LaunchVelocity.MakePlanar());
		MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(30f, 0f, (num - ((num > 1.75f) ? num : 0f)) * ((num2 > 0f) ? 40f : (-40f)));
		PM.transform.forward = LaunchVelocity.MakePlanar();
		PM.Base.MaxRayLenght = (PM.Base.GetPrefab("sonic_fast") ? 1.75f : 0.75f);
		Falling = false;
		OnStop = false;
	}

	private void StateSpring()
	{
		PM.Base.SetState("Spring");
		bool flag = Time.time - StartTime < (UseTimerToExit ? 0f : Timer);
		if (flag)
		{
			if (LaunchAnimMode == "spring_a" || LaunchAnimMode == "spring_b" || LaunchAnimMode == "spring_c")
			{
				PM.Base.PlayAnimation("Spring Jump", "On Spring");
			}
			else
			{
				PM.Base.PlayAnimation("Falling", "On Fall");
			}
			Falling = false;
		}
		else if (LaunchAnimMode == "spring_a" || LaunchAnimMode == "spring_b" || LaunchAnimMode == "spring_c")
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
		if (flag)
		{
			MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			PM.RBody.velocity = LaunchVelocity;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.Base.LockControls = true;
		}
		else
		{
			if (LaunchAnimMode == "spring_a")
			{
				PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
				PM.transform.forward = LaunchVelocity.MakePlanar();
				LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				PM.Base.LockControls = true;
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
			}
			else if (LaunchAnimMode == "spring_b")
			{
				if ((UseTimerToExit || UseTimerToRelease) && !AlwaysLocked)
				{
					if (Time.time - StartTime < Timer)
					{
						PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
						PM.transform.forward = LaunchVelocity.MakePlanar();
						LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
						PM.Base.LockControls = true;
						MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
					}
					else
					{
						if (!OnStop && !UseTimerToRelease)
						{
							if (PM.Base.CurSpeed > PM.Base.TopSpeed)
							{
								PM.Base.CurSpeed = PM.Base.TopSpeed;
							}
							OnStop = true;
						}
						MeshLaunchRot = Quaternion.RotateTowards(MeshLaunchRot, Quaternion.LookRotation(LaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 200f);
						PM.Base.GeneralMeshRotation = PM.transform.rotation;
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
						PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
						PM.Base.DoWallNormal();
					}
				}
				else
				{
					PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
					PM.transform.forward = LaunchVelocity.MakePlanar();
					LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					PM.Base.LockControls = true;
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
				}
			}
			else if (LaunchAnimMode == "spring_c")
			{
				if (!AlwaysLocked)
				{
					if (PM.RBody.velocity.y > 0f)
					{
						PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
						PM.transform.forward = LaunchVelocity.MakePlanar();
						LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						PM.Base.LockControls = true;
					}
					else
					{
						PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
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
				}
				else
				{
					PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
					PM.transform.forward = LaunchVelocity.MakePlanar();
					LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
					PM.Base.LockControls = true;
				}
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y > -0.1f) ? (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)) : Quaternion.LookRotation(LaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 5f);
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

	private void StateSpringEnd()
	{
	}

	private Vector3 Direction(Transform _transform)
	{
		if (SpringMode == Mode.FixedTarget)
		{
			return (TargetPosition - _transform.position).normalized;
		}
		if (TargetPosition == Vector3.zero)
		{
			return base.transform.forward;
		}
		Vector3 normalized = (TargetPosition - _transform.position).normalized;
		normalized.y = Mathf.Max(normalized.y, base.transform.forward.y);
		return normalized.normalized;
	}

	private string AnimMode()
	{
		if (TargetPosition != Vector3.zero)
		{
			if (!(!(Vector3.Distance(base.transform.GetChild(0).position, TargetPosition) < 25f)))
			{
				if (!(Vector3.Dot(Direction(base.transform.GetChild(0)), Vector3.up) < 0.5f))
				{
					return "spring_c";
				}
				return "spring_b";
			}
			if (!UseTimerToRelease)
			{
				return "spring_a";
			}
			return "spring_b";
		}
		if (!(Vector3.Dot(Direction(base.transform.GetChild(0)), Vector3.up) < 0.75f))
		{
			if (UseTimerToExit)
			{
				return "spring_b";
			}
			return "spring_c";
		}
		return "spring_b";
	}

	private void OnTrigger(AudioSource _Audio)
	{
		if (!IsNotVisible)
		{
			Animation.Play();
			Audio.PlayOneShot(Sound, Audio.volume);
			_Audio.PlayOneShot(WindSounds[Random.Range(0, WindSounds.Length)], _Audio.volume);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetState() != "Vehicle" && !player.IsDead)
		{
			OnTrigger(player.Audio);
			PM = collider.GetComponent<PlayerManager>();
			LaunchAnimMode = AnimMode();
			player.StateMachine.ChangeState(base.gameObject, StateSpring);
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
			component.OnSpring(base.transform.GetChild(0).position - base.transform.GetChild(0).up * 0.25f, Direction(base.transform.GetChild(0)) * Speed, Timer, AnimMode(), UseTimerToExit, UseTimerToRelease, AlwaysLocked);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetPosition != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(TargetPosition, 1f);
		}
		Vector3 vector = base.transform.GetChild(0).position + Direction(base.transform.GetChild(0)) * Speed * (UseTimerToExit ? 0f : Timer);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.GetChild(0).position, vector);
		int num = 4 * (int)Speed;
		float num2 = 0.01f;
		Vector3 vector2 = Direction(base.transform.GetChild(0)) * Speed;
		Gizmos.color = ((!AlwaysLocked) ? Color.green : Color.white);
		Vector3 vector3 = vector;
		Vector3 from = vector;
		for (int i = 0; i < num; i++)
		{
			vector2.y -= 9.81f * num2;
			vector3 += vector2 * num2;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
