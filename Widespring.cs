using UnityEngine;

public class Widespring : ObjectBase
{
	[Header("Framework")]
	public float Speed;

	public float Timer;

	[Header("Prefab")]
	public Animation Animation;

	public AudioSource Audio;

	public AudioClip Sound;

	public AudioClip[] WindSounds;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 LastForward;

	private Transform CurrentLaunchPos;

	private float StartTime;

	private bool Falling;

	public void SetParameters(float _Speed, float _Timer)
	{
		Speed = _Speed;
		Timer = _Timer;
	}

	private void StateWideSpringStart()
	{
		PM.Base.SetState("WideSpring");
		PM.Base.PlayAnimation("Spring Jump", "On Spring");
		StartTime = Time.time;
		LaunchVelocity = Vector3.up * Speed;
		LastForward = PM.transform.forward;
		PM.transform.position = CurrentLaunchPos.position + Vector3.up * 0.25f;
		Falling = false;
	}

	private void StateWideSpring()
	{
		PM.Base.SetState("WideSpring");
		bool flag = Time.time - StartTime < Timer;
		if (Time.time - StartTime < Timer)
		{
			PM.Base.CurSpeed = 0f;
			PM.transform.up = Vector3.up;
			PM.transform.forward = LastForward;
			PM.RBody.velocity = LaunchVelocity;
			PM.Base.LockControls = true;
		}
		else
		{
			Vector3 vector = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
			if (PM.RBody.velocity.magnitude != 0f)
			{
				vector = PM.transform.forward * PM.Base.CurSpeed;
				LaunchVelocity = new Vector3(vector.x, LaunchVelocity.y, vector.z);
			}
			LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
			LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
			PM.RBody.velocity = LaunchVelocity;
			PM.Base.DoWallNormal();
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
		}
		if (PM.RBody.velocity.y < 0f)
		{
			if (!Falling)
			{
				Falling = true;
				PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
		}
		else
		{
			Falling = false;
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
		}
		if (PM.Base.LockControls)
		{
			PM.Base.TargetDirection = Vector3.zero;
		}
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
		if (PM.Base.IsGrounded() && !flag)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateWideSpringEnd()
	{
	}

	private void OnTrigger(AudioSource _Audio)
	{
		Animation.Play();
		Audio.PlayOneShot(Sound, Audio.volume);
		_Audio.PlayOneShot(WindSounds[Random.Range(0, WindSounds.Length)], _Audio.volume);
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetState() != "Vehicle" && !player.IsDead)
		{
			OnTrigger(player.Audio);
			CurrentLaunchPos = collider.transform;
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateWideSpring);
		}
		AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
		if ((bool)component && component.GetAmigoState() != "Vehicle" && !component.IsDead)
		{
			OnTrigger(component.Audio);
			component.OnWideSpring(collider.transform.position + Vector3.up * 0.25f, Vector3.up * Speed, Timer);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.GetChild(2).position + (Vector3.up + base.transform.up * 0.05f) * Speed * Timer;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.GetChild(2).position, vector);
		float num = 0.01f;
		Vector3 vector2 = (Vector3.up + base.transform.up * 0.05f) * Speed;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		_ = Vector3.zero;
		while (vector3.y > base.transform.position.y)
		{
			vector2.y -= 25f * num;
			vector3 += vector2 * num;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
