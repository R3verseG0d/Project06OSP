using UnityEngine;

public class JumpBoard : ObjectBase
{
	[Header("Framework")]
	public float YOffset;

	public float Speed;

	public float Timer;

	public Vector3 TargetPosition;

	[Header("Prefab")]
	public AudioClip sound;

	public AudioClip[] windSounds;

	public AudioSource thisAudio;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Quaternion MeshLaunchRot;

	private bool Falling;

	private float StartTime;

	public void SetParameters(float _YOffset, float _Speed, float _Timer, Vector3 _TargetPosition)
	{
		YOffset = _YOffset;
		Speed = _Speed;
		Timer = _Timer;
		TargetPosition = _TargetPosition;
	}

	private void StateJumpBoardStart()
	{
		PM.Base.SetState("JumpPanel");
		StartTime = Time.time;
		LaunchVelocity = Direction(base.transform) * Speed;
		PM.transform.position = base.transform.GetChild(0).position + PM.transform.up * 0.25f;
		MeshLaunchRot = Quaternion.LookRotation(LaunchVelocity.normalized) * Quaternion.Euler(90f, 0f, 0f);
		PM.transform.forward = LaunchVelocity.MakePlanar();
		Falling = false;
	}

	private void StateJumpBoard()
	{
		PM.Base.SetState("JumpPanel");
		PM.Base.LockControls = true;
		bool flag = Time.time - StartTime < Timer;
		if (flag)
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
		PM.transform.forward = LaunchVelocity.MakePlanar();
		if (!flag)
		{
			LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
		}
		MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(LaunchVelocity.MakePlanar()), Time.fixedDeltaTime * 5f);
		PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
		PM.RBody.velocity = LaunchVelocity;
		if (PM.Base.LockControls)
		{
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
			PM.Base.TargetDirection = Vector3.zero;
		}
		if (PM.Base.IsGrounded() && !flag)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
		if (PM.Base.FrontalCollision && !flag)
		{
			PM.Base.SetMachineState("StateAir");
		}
	}

	private void StateJumpBoardEnd()
	{
	}

	private Vector3 Direction(Transform _transform)
	{
		if (TargetPosition == Vector3.zero)
		{
			return base.transform.forward;
		}
		Vector3 normalized = (TargetPosition - _transform.position).normalized;
		return new Vector3(normalized.x, base.transform.forward.y + YOffset, normalized.z).normalized;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead)
		{
			PM = collider.GetComponent<PlayerManager>();
			thisAudio.PlayOneShot(sound, thisAudio.volume);
			player.Audio.PlayOneShot(windSounds[Random.Range(0, windSounds.Length)], player.Audio.volume);
			player.StateMachine.ChangeState(base.gameObject, StateJumpBoard);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetPosition != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(TargetPosition, 1f);
		}
		Vector3 vector = base.transform.position + Direction(base.transform) * Speed * Timer;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, vector);
		float num = 0.01f;
		Vector3 vector2 = Direction(base.transform) * Speed;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		while (vector3.y > base.transform.position.y)
		{
			vector2.y -= 9.81f * num;
			vector3 += vector2 * num;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
