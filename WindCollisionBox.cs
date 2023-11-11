using UnityEngine;

public class WindCollisionBox : ObjectBase
{
	public enum Type
	{
		Wind = 0,
		Leaves = 1,
		Underflow = 2
	}

	[Header("Framework")]
	public float Power;

	public float FloatHeight;

	public Type Particle;

	[Header("Prefab")]
	public BoxCollider Collider;

	public ParticleSystem[] Particles;

	private PlayerManager PM;

	private Vector3 AirMotionVelocity;

	private float PingPongVel;

	private float StartingYPos;

	public void SetParameters(float _Power, float _FloatHeight, int _Particle)
	{
		Power = _Power;
		FloatHeight = _FloatHeight;
		Particle = (Type)(_Particle - 1);
	}

	private void Start()
	{
		switch (Particle)
		{
		case Type.Wind:
			Particles[1].gameObject.SetActive(value: false);
			Particles[2].gameObject.SetActive(value: false);
			break;
		case Type.Leaves:
			Particles[2].gameObject.SetActive(value: false);
			break;
		case Type.Underflow:
			Particles[1].gameObject.SetActive(value: false);
			break;
		}
		for (int i = 0; i < Particles.Length; i++)
		{
			ParticleSystem.ShapeModule shape = Particles[i].shape;
			shape.position = Vector3.up * Collider.size.y * 0.5f;
			shape.scale = new Vector3(Collider.size.x, Collider.size.y, Collider.size.z);
		}
	}

	private void Update()
	{
		if ((bool)PM && PM.Base.GetState() == "Float" && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			PM.Base.SetMachineState("StateJump");
		}
	}

	private void StateFloatStart()
	{
		PM.Base.SetState("Float");
		AirMotionVelocity = PM.RBody.velocity;
		AirMotionVelocity.y = 0f;
		PM.RBody.velocity = AirMotionVelocity;
		PingPongVel = 0f;
	}

	private void StateFloat()
	{
		PM.Base.SetState("Float");
		PM.Base.PlayAnimation("Float", "On Float");
		Vector3 vector = new Vector3(AirMotionVelocity.x, 0f, AirMotionVelocity.z);
		if (PM.RBody.velocity.magnitude != 0f)
		{
			vector = PM.transform.forward * (PM.Base.CurSpeed / 2f);
			AirMotionVelocity = new Vector3(vector.x, AirMotionVelocity.y, vector.z);
		}
		PingPongVel = Mathf.Lerp((0f - Power) * 0.25f, Power * 1.5f, Mathf.Abs(Mathf.Cos(Time.time * 5.5f)));
		AirMotionVelocity.y = PingPongVel * Time.fixedDeltaTime;
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
		PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
		PM.RBody.velocity = AirMotionVelocity;
		PM.Base.DoWallNormal();
		if (PM.transform.position.y > StartingYPos + FloatHeight)
		{
			PM.Base.SetMachineState("StateAir");
		}
		if (PM.Base.IsGrounded() && PM.Base.ShouldAlignOrFall(Align: false))
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
	}

	private void StateFloatEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!(player == null) && !player.IsDead && !(player.GetState() == "Float"))
		{
			PM = collider.GetComponent<PlayerManager>();
			StartingYPos = PM.transform.position.y;
			player.StateMachine.ChangeState(base.gameObject, StateFloat);
		}
	}
}
