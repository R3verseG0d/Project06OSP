using UnityEngine;

public class Common_DashRing : ObjectBase
{
	[Header("Framework")]
	public float Speed;

	public float Timer;

	[Header("Prefab")]
	public AudioSource Audio;

	public ParticleSystem DashRingFX;

	public Material GlowMaterial;

	public Gradient GlowGradient;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 PlaneDir;

	private Quaternion MeshLaunchRot;

	private bool Falling;

	private float timer;

	public void SetParameters(float _Speed, float _Timer)
	{
		Speed = _Speed;
		Timer = _Timer;
	}

	private void Update()
	{
		GlowMaterial.SetColor("_Color", GlowGradient.Evaluate(Mathf.Repeat(Time.time, 1f) / 1f));
	}

	private void StateDashRingStart()
	{
		PM.Base.SetState("DashRing");
		PM.Base.PlayAnimation("Spring Jump", "On Spring");
		timer = Time.time;
		LaunchVelocity = base.transform.forward * Speed;
		PM.transform.position = base.transform.GetChild(0).position - base.transform.GetChild(0).up * 0.25f;
		MeshLaunchRot = Quaternion.LookRotation(LaunchVelocity.normalized) * Quaternion.Euler(90f, 0f, 0f);
		PlaneDir = LaunchVelocity.MakePlanar();
		PM.transform.forward = PlaneDir;
		Falling = false;
	}

	private void StateDashRing()
	{
		PM.Base.SetState("DashRing");
		PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
		PM.transform.forward = PlaneDir;
		bool flag = Time.time - timer < Timer;
		PM.Base.LockControls = true;
		if (Time.time - timer < Timer)
		{
			PM.RBody.velocity = LaunchVelocity;
			PM.Base.GeneralMeshRotation = MeshLaunchRot;
		}
		else
		{
			LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
			PM.RBody.velocity = LaunchVelocity;
			if (PM.RBody.velocity.y < 0f && !Falling)
			{
				Falling = true;
				PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
			}
			if (PM.RBody.velocity.y <= 0f)
			{
				MeshLaunchRot = Quaternion.Slerp(MeshLaunchRot, Quaternion.LookRotation(PlaneDir), Time.fixedDeltaTime * 5f);
			}
			Quaternion identity = Quaternion.identity;
			identity.x = MeshLaunchRot.x;
			identity.z = MeshLaunchRot.z;
			PM.Base.GeneralMeshRotation = identity;
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

	private void StateDashRingEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() == "Vehicle") && !player.IsDead)
		{
			DashRingFX.Play();
			Audio.Play();
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateDashRing);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.GetChild(0).position + base.transform.GetChild(0).forward * Speed * Timer;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.GetChild(0).position, vector);
		int num = 4 * (int)Speed;
		Vector3 vector2 = base.transform.GetChild(0).forward * Speed;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		_ = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			vector2.y -= 9.81f * Time.fixedDeltaTime;
			vector3 += vector2 * Time.fixedDeltaTime;
			_ = vector2.normalized;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}
}
