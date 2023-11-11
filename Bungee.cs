using UnityEngine;

public class Bungee : ObjectBase
{
	[Header("Framework")]
	public float Spd;

	public Vector3 TargetObj;

	public float MotSpd;

	[Header("Prefab")]
	public Animator Animator;

	public Transform PlayerPoint;

	public AudioClip BungeeSound;

	public AudioClip[] JumpSounds;

	public AudioClip[] WindSounds;

	public AudioSource Audio;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 StartLaunchVelocity;

	private Quaternion MeshLaunchRot;

	private float StartTime;

	private float BungeeTime;

	private bool Falling;

	private bool Activated;

	private int BungeeState;

	public void SetParameters(float _Spd, Vector3 _TargetObj, float _MotSpd)
	{
		Spd = _Spd;
		TargetObj = _TargetObj;
		MotSpd = _MotSpd;
	}

	private void StateBungeeStart()
	{
		PM.Base.SetState("Bungee");
		BungeeState = 0;
		BungeeTime = Time.time;
		PM.transform.SetParent(PlayerPoint, worldPositionStays: false);
	}

	private void StateBungee()
	{
		PM.Base.SetState("Bungee");
		if (BungeeState == 0)
		{
			PM.Base.PlayAnimation((Time.time - BungeeTime < 3.25f) ? "Tarzan Fall" : "Tarzan Pull", (Time.time - BungeeTime < 3f) ? "On Tarzan Fall" : "On Tarzan Pull");
			PM.Base.LockControls = true;
			PM.Base.CurSpeed = 0f;
			PM.RBody.isKinematic = true;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			PM.transform.position = PlayerPoint.position;
			PM.transform.forward = -base.transform.forward;
			PM.Base.Mesh.transform.localPosition = new Vector3(0.045f, 0.8f, -0.175f);
			PM.RBody.velocity = Vector3.zero;
			PM.Base.GeneralMeshRotation = PM.transform.rotation;
			return;
		}
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
		PM.RBody.isKinematic = false;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
		if (PM.RBody.velocity.y > 0f)
		{
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
			PM.Base.LockControls = true;
			MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(LaunchVelocity.MakePlanar()) : (Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
		}
		else
		{
			PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
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
		PM.RBody.velocity = LaunchVelocity;
		if ((PM.Base.FrontalCollision || PM.Base.IsGrounded()) && Time.time - StartTime > 0.1f)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateBungeeEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() == "Bungee") && !Activated)
		{
			Animator.SetTrigger("On Trigger");
			Audio.PlayOneShot(BungeeSound, Audio.volume);
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateBungee);
			player.Camera.PlayObjectEvent(6.5f, "Bungee", new CameraParameters(101, base.transform.position, base.transform.position + base.transform.forward * 1f));
			Activated = true;
		}
	}

	public void OnJumpFrame()
	{
		BungeeState = 1;
		StartTime = Time.time;
		LaunchVelocity = (TargetObj - PM.transform.position).normalized * (Spd * 5f);
		StartLaunchVelocity = LaunchVelocity.normalized;
		PM.transform.SetParent(null);
		MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(30f, 0f, 0f);
		PM.transform.forward = LaunchVelocity.MakePlanar();
		Audio.PlayOneShot(JumpSounds[0], Audio.volume * 2f);
		Audio.PlayOneShot(JumpSounds[1], Audio.volume);
		Falling = false;
	}

	public void OnWindSound()
	{
		Audio.PlayOneShot(WindSounds[Random.Range(0, WindSounds.Length)], Audio.volume);
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetObj != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, TargetObj);
			Gizmos.DrawWireSphere(TargetObj, 1f);
		}
	}
}
