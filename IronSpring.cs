using UnityEngine;

public class IronSpring : PsiObject
{
	[Header("Framework")]
	public Vector3 Target;

	public float Pitch;

	public float Yaw;

	public float Power;

	public float Out_Time;

	public float On_Time;

	[Header("Prefab")]
	public Animator Animator;

	public Renderer Renderer;

	public Common_PsiMarkSphere ESPMark;

	public Collider StandCollider;

	public Transform Point;

	public AudioSource Audio;

	public AudioClip[] Clips;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 StartLaunchVelocity;

	private Quaternion MeshLaunchRot;

	private float StartTime;

	private int State;

	private bool Falling;

	public void SetParameters(Vector3 _Target, float _Pitch, float _Yaw, float _Power, float _Out_Time, float _On_Time)
	{
		Target = _Target;
		Pitch = _Pitch;
		Yaw = _Yaw;
		Power = _Power;
		Out_Time = _Out_Time;
		On_Time = _On_Time;
	}

	private void Start()
	{
		ESPMark.Target = base.gameObject;
	}

	private void StateIronSpringStart()
	{
		PM.Base.SetState("IronSpring");
		State = 0;
		StartTime = Time.time;
		Animator.SetBool("Charging", value: true);
		Animator.SetTrigger("On Trigger");
		Audio.PlayOneShot(Clips[0], Audio.volume);
		PM.transform.SetParent(Point);
		PM.silver.PsiFX = true;
	}

	private void StateIronSpring()
	{
		PM.Base.SetState("IronSpring");
		if (State == 0)
		{
			PM.Base.LockControls = true;
			PM.RBody.velocity = Vector3.zero;
			PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, PM.Base.RaycastHit.normal) * PM.transform.rotation;
			return;
		}
		bool num = Time.time - StartTime < On_Time;
		if (num)
		{
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
			Falling = false;
		}
		else if (PM.RBody.velocity.y > -0.1f)
		{
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
			Falling = false;
		}
		else if (!Falling)
		{
			Falling = true;
			PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
		}
		if (num)
		{
			MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			PM.RBody.velocity = LaunchVelocity;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.Base.LockControls = true;
		}
		else
		{
			if (Time.time - StartTime < Out_Time)
			{
				PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
				PM.transform.forward = LaunchVelocity.MakePlanar();
				LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				PM.Base.LockControls = true;
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
			}
			else
			{
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
			PM.RBody.velocity = LaunchVelocity;
		}
		if (PM.Base.LockControls)
		{
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
			PM.Base.TargetDirection = Vector3.zero;
		}
		if (Time.time - StartTime > 0.5f)
		{
			StandCollider.enabled = true;
		}
		if (PM.Base.IsGrounded() && Time.time - StartTime > 0.1f)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			StandCollider.enabled = true;
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
		if (PM.Base.FrontalCollision && Time.time - StartTime > 0.1f)
		{
			PM.Base.SetMachineState("StateAir");
			StandCollider.enabled = true;
		}
	}

	private void StateIronSpringEnd()
	{
		StandCollider.enabled = true;
	}

	private void Update()
	{
		if ((bool)PM && PM.Base.GetState() == "IronSpring" && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && State == 0 && Time.time - StartTime > 3f && !Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
		{
			Animator.SetBool("Charging", value: false);
			StartTime = Time.time;
			LaunchVelocity = Direction(Point) * Power;
			StartLaunchVelocity = LaunchVelocity.normalized;
			PM.transform.position = Point.position - Point.up * 0.25f;
			float num = Vector3.Distance(PM.transform.forward.normalized, LaunchVelocity.MakePlanar().normalized);
			float num2 = Vector3.Dot(PM.transform.right.MakePlanar(), LaunchVelocity.MakePlanar());
			MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(30f, 0f, (num - ((num > 1.75f) ? num : 0f)) * ((num2 > 0f) ? 40f : (-40f)));
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.Base.MaxRayLenght = 0.75f;
			PM.Base.transform.SetParent(null);
			PM.silver.PsiFX = false;
			StandCollider.enabled = false;
			Audio.Stop();
			Audio.PlayOneShot(Clips[1], Audio.volume);
			Falling = false;
			State = 1;
		}
		OnPsiFX(Renderer, (bool)PM && PM.Base.GetState() == "IronSpring" && State == 0, 2);
	}

	private Vector3 Direction(Transform _transform)
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f - Pitch;
		eulerAngles.y += 180f - Yaw;
		return Quaternion.Euler(eulerAngles) * Vector3.forward;
	}

	private void OnEventSignal()
	{
		PM = ESPMark.Player.GetComponent<PlayerManager>();
		PM.Base.StateMachine.ChangeState(base.gameObject, StateIronSpring);
	}

	private void OnDrawGizmosSelected()
	{
		if (Target != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(Target, 1f);
		}
		Vector3 vector = Point.position + Direction(Point) * Power * On_Time;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(Point.position, vector);
		int num = 4 * (int)Power;
		float num2 = 0.01f;
		Vector3 vector2 = Direction(Point) * Power;
		Gizmos.color = Color.green;
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
