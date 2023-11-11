using UnityEngine;

public class Common_RainbowRing : ObjectBase
{
	[Header("Framework")]
	public float Inclination;

	public float Speed;

	public float Timer;

	[Header("Prefab")]
	public float LockTimer = 1f;

	public Animation Animation;

	public AudioSource Audio;

	public ParticleSystem FX;

	[Header("Optional")]
	public GameObject CamEventObj;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Quaternion MeshLaunchRot;

	private bool Used;

	private bool OnStop;

	private float StartTime;

	public void SetParameters(float _Inclination, float _Speed, float _Timer)
	{
		Inclination = _Inclination;
		Speed = _Speed;
		Timer = _Timer;
	}

	private void StateRainbowRingStart()
	{
		PM.Base.SetState("RainbowRing");
		StartTime = Time.time;
		LaunchVelocity = MeshDirection() * Speed;
		PM.transform.position = base.transform.position - base.transform.up * 0.25f;
		MeshLaunchRot = Quaternion.LookRotation(LaunchVelocity.normalized) * Quaternion.Euler(90f, 0f, 0f);
		PM.transform.forward = LaunchVelocity.MakePlanar();
		OnStop = false;
	}

	private void StateRainbowRing()
	{
		PM.Base.SetState("RainbowRing");
		bool flag = Time.time - StartTime < Mathf.Min(LockTimer, Timer);
		if (flag)
		{
			PM.Base.PlayAnimation("Rainbow Trick", "On Rainbow Ring");
		}
		else if (PM.RBody.velocity.y > -0.1f)
		{
			PM.Base.PlayAnimation("Rainbow Trick", "On Rainbow Ring");
		}
		else
		{
			PM.Base.PlayAnimation("Falling", "On Fall");
		}
		if (flag)
		{
			PM.RBody.velocity = LaunchVelocity;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.Base.LockControls = true;
		}
		else
		{
			if (Time.time - StartTime > Timer)
			{
				if (!OnStop)
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
			else
			{
				LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
				PM.transform.forward = LaunchVelocity.MakePlanar();
				PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
				PM.Base.LockControls = true;
				if (LaunchVelocity.y < 0f)
				{
					MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(LaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 10f);
				}
			}
			PM.RBody.velocity = LaunchVelocity;
		}
		if (PM.Base.LockControls)
		{
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
		}
		if (PM.Base.IsGrounded() && !flag)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateRainbowRingEnd()
	{
	}

	private Vector3 MeshDirection()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = (0f - Inclination) * 0.75f;
		return Quaternion.Euler(eulerAngles) * Vector3.forward;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() == "Vehicle") && !player.IsDead && !Used)
		{
			Used = true;
			Animation.Play();
			player.AddScore(800, InstantChain: true);
			Audio.Play();
			FX.Play();
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateRainbowRing);
			if ((bool)CamEventObj)
			{
				CamEventObj.SetActive(value: true);
				CamEventObj.GetComponent<CameraEvents>().DestroyOnExit = true;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + MeshDirection() * Speed * Mathf.Min(LockTimer, Timer);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, vector);
		int num = 240;
		Vector3 vector2 = MeshDirection() * Speed;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		float num2 = Mathf.Min(LockTimer, Timer);
		for (int i = 0; i < num; i++)
		{
			num2 += Time.fixedDeltaTime;
			if (num2 > Timer)
			{
				Gizmos.color = Color.red;
			}
			vector2.y -= ((num2 > Timer) ? 25f : 9.81f) * Time.fixedDeltaTime;
			vector3 += vector2 * Time.fixedDeltaTime;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
