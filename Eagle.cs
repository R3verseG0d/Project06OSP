using UnityEngine;

public class Eagle : ObjectBase
{
	private enum State
	{
		Appear = 0,
		Wait = 1,
		Turn = 2,
		Fly = 3,
		FlyAway = 4
	}

	[Header("Framework")]
	public string TargetSpline;

	public float Speed;

	public bool LockPlayer;

	[Header("Prefab")]
	public StateMachine StateMachine;

	public Rigidbody _Rigidbody;

	public Transform AttachTransform;

	public Animator Animator;

	public AudioSource Audio;

	public AudioClip WaitFlap;

	public AudioClip FlyFlap;

	public ParticleSystem FeathersFX;

	public AnimationCurve TurnCurve;

	[Header("Optional")]
	public bool Is_KDV_B_Event;

	internal int EagleMode;

	private PlayerBase Player;

	private State EagleState;

	private BezierCurve Spline;

	private PlayerManager PM;

	private Quaternion Rotation = Quaternion.identity;

	private bool IsCarrying;

	private float PathTime;

	private int Animation;

	private int CurrPoint;

	private Vector3 TargetPosition;

	private float StartTime;

	private float RollDirection;

	private Vector3 Direction;

	private Vector3 LastDirection;

	private Vector3 FlyAwayDir;

	private Vector3 FlyDirection;

	private bool DetachedPlayer;

	private bool PlayerJumpedOff;

	private float GetBackTimer;

	public void SetParameters(float _Speed, string _TargetSpline, bool _LockPlayer)
	{
		Speed = _Speed;
		TargetSpline = _TargetSpline;
		LockPlayer = _LockPlayer;
	}

	private void Start()
	{
		Spline = GameObject.Find(TargetSpline).GetComponent<BezierCurve>();
		float num = Vector3.Distance(Spline.knots[0].position, Spline.knots[Spline.knots.Count - 1].position);
		EagleMode = ((!(num <= 1f)) ? 1 : 0);
		if (EagleMode == 1)
		{
			Animation = 1;
			TargetPosition = Spline.GetPosition(0f);
			StateMachine.Initialize(StateAppear);
		}
		else
		{
			Direction = Spline.GetTangent(0f);
			LastDirection = Direction;
			StateMachine.Initialize(StateFly);
		}
		Player = Object.FindObjectOfType<PlayerBase>();
	}

	private void WingFlap()
	{
		if (Random.Range(0, 2) == 0)
		{
			FeathersFX.Play();
		}
		Audio.PlayOneShot((Animation == 0) ? WaitFlap : FlyFlap, Audio.volume);
	}

	private void StateAppearStart()
	{
	}

	private void StateAppear()
	{
		EagleState = State.Appear;
		if (Vector3.Distance(TargetPosition, base.transform.position) < 0.25f)
		{
			StateMachine.ChangeState(StateWait);
		}
		Vector3 normalized = (TargetPosition - base.transform.position).normalized;
		Rotation = Quaternion.LookRotation(normalized.MakePlanar());
		_Rigidbody.MovePosition(base.transform.position + normalized * 17f * Time.fixedDeltaTime);
	}

	private void StateAppearEnd()
	{
	}

	private void StateWaitStart()
	{
		Animation = 0;
	}

	private void StateWait()
	{
		EagleState = State.Wait;
	}

	private void StateWaitEnd()
	{
	}

	private void StateTurnStart()
	{
		Animation = 2;
		StartTime = Time.time;
		Quaternion quaternion = Quaternion.LookRotation(base.transform.forward.MakePlanar());
		RollDirection = Vector3.Dot(quaternion * Vector3.right, Spline.GetTangent(0f).MakePlanar());
	}

	private void StateTurn()
	{
		EagleState = State.Turn;
		float num = (Time.time - StartTime) * 0.5f;
		if (num >= 1f)
		{
			StateMachine.ChangeState(StateFly);
		}
		PathTime += Mathf.Lerp(0f, Speed, num * 2f) / Spline.Length() * Time.fixedDeltaTime;
		PathTime = Mathf.Clamp(PathTime, 0f, 1f);
		Vector3 tangent = Spline.GetTangent(PathTime);
		Quaternion b = Quaternion.LookRotation(Vector3.Slerp(tangent.MakePlanar(), tangent, num * num));
		float num2 = 2.5f + num * 7.5f;
		Rotation = Quaternion.Slerp(Rotation, b, Time.fixedDeltaTime * num2);
		Vector3 up = Vector3.up;
		if (RollDirection >= 0f)
		{
			up += base.transform.right.MakePlanar();
		}
		else
		{
			up -= base.transform.right.MakePlanar();
		}
		up.Normalize();
		float num3 = Vector3.Distance(Vector3.up * num, Vector3.up * 0.1f) / 0.1f;
		up = Vector3.Slerp(Vector3.up, up, 1f - num3);
		Rotation = Quaternion.LookRotation(Rotation * Vector3.forward, up);
		_Rigidbody.MovePosition(Spline.GetPosition(PathTime));
	}

	private void StateTurnEnd()
	{
	}

	private void StateFlyStart()
	{
		Direction = Spline.GetTangent(0f);
		LastDirection = Direction;
	}

	private void StateFly()
	{
		EagleState = State.Fly;
		Animation = ((!IsCarrying) ? 1 : 2);
		float num = ((Direction.y > 0f || EagleMode == 0) ? (Speed * 0.75f) : Speed);
		PathTime += num / Spline.Length() * Time.fixedDeltaTime;
		_Rigidbody.MovePosition(Spline.GetPosition(PathTime));
		Direction = Spline.GetTangent(PathTime).normalized;
		if (Direction.y > 0f)
		{
			Direction = Vector3.Slerp(Direction.MakePlanar(), Direction, (num - 30f) / 30f);
		}
		Rotation = Quaternion.Slerp(Rotation, Quaternion.LookRotation(Direction), Time.fixedDeltaTime * 20f);
		LastDirection = Direction;
		Animator.SetFloat("Y Speed", Direction.y);
		if (PathTime > 1f)
		{
			if (EagleMode == 1)
			{
				StateMachine.ChangeState(StateFlyAway);
			}
			else
			{
				PathTime -= 1f;
			}
		}
	}

	private void StateFlyEnd()
	{
	}

	private void StateFlyAwayStart()
	{
		Animation = 1;
		DetachedPlayer = false;
		FlyAwayDir = Vector3.up;
		FlyDirection = (Rotation * Vector3.forward).MakePlanar();
		GetBackTimer = Time.time;
	}

	private void StateFlyAway()
	{
		EagleState = State.FlyAway;
		if (!DetachedPlayer)
		{
			if ((bool)PM)
			{
				if (!Is_KDV_B_Event)
				{
					PM.Base.SetMachineState("StateAir");
				}
				OnHoldExit(PM.Base, SetJumpOff: false);
			}
			if (!PlayerJumpedOff)
			{
				Object.Destroy(base.gameObject, 2f);
			}
			DetachedPlayer = true;
		}
		if (Time.time - GetBackTimer < 2f)
		{
			FlyDirection = Vector3.Slerp(FlyDirection, FlyAwayDir, Time.fixedDeltaTime * Speed * 0.25f).normalized;
			_Rigidbody.MovePosition(base.transform.position + FlyDirection * Speed * Time.fixedDeltaTime * 0.5f);
		}
		else if (PlayerJumpedOff)
		{
			Vector3 normalized = (TargetPosition - base.transform.position).normalized;
			Rotation = Quaternion.Slerp(Rotation, Quaternion.LookRotation(normalized.MakePlanar()), Time.fixedDeltaTime * 5f);
			_Rigidbody.MovePosition(base.transform.position + normalized * 17f * Time.fixedDeltaTime);
			if (Vector3.Distance(TargetPosition, base.transform.position) < 0.25f)
			{
				PathTime = 0f;
				StateMachine.ChangeState(StateWait);
			}
		}
		else
		{
			FlyDirection = Vector3.Slerp(FlyDirection, FlyAwayDir, Time.fixedDeltaTime * Speed * 0.25f).normalized;
			_Rigidbody.MovePosition(base.transform.position + FlyDirection * Speed * Time.fixedDeltaTime * 0.5f);
		}
	}

	private void StateFlyAwayEnd()
	{
		PlayerJumpedOff = false;
	}

	private void StateHoldStart()
	{
		PM.Base.SetState("Hold");
		PM.transform.SetParent(AttachTransform);
		PM.RBody.isKinematic = true;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		PM.RBody.velocity = Vector3.zero;
		PM.transform.localPosition = new Vector3(0f, -0.75f, 0f);
		PM.transform.localRotation = Quaternion.identity;
		PM.RBody.velocity = Vector3.zero;
	}

	private void StateHold()
	{
		PM.Base.SetState("Hold");
		PM.Base.PlayAnimation("Hold", "On Hold");
		PM.Base.LockControls = true;
		PM.Base.GeneralMeshRotation = PM.transform.rotation;
		PM.Base.CurSpeed = 0f;
	}

	private void StateHoldExit()
	{
	}

	private void FixedUpdate()
	{
		StateMachine.UpdateStateMachine();
		_Rigidbody.MoveRotation(Rotation);
	}

	private void Update()
	{
		if (!Player)
		{
			Player = Object.FindObjectOfType<PlayerBase>();
		}
		else
		{
			AttachTransform.tag = ((Player.GetPrefab("snow_board") || Player.GetPrefab("knuckles") || Player.GetPrefab("omega") || Player.GetPrefab("princess") || Player.GetPrefab("rouge") || Player.GetPrefab("tails") || IsCarrying || EagleState == State.FlyAway) ? "Untagged" : "HomingTarget");
		}
		Animator.SetInteger("Animation", Animation);
		if ((bool)PM && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && PM.Base.GetState() == "Hold" && !LockPlayer)
		{
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
			{
				PM.Base.SetMachineState("StateJump");
				OnHoldExit(PM.Base);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button X"))
			{
				PM.Base.SetMachineState("StateAir");
				OnHoldExit(PM.Base);
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!player || player.IsDead || player.GetState() == "Vehicle" || player.GetPrefab("snow_board") || player.GetPrefab("knuckles") || player.GetPrefab("omega") || player.GetPrefab("princess") || player.GetPrefab("rouge") || player.GetPrefab("tails") || IsCarrying || EagleState == State.FlyAway)
		{
			return;
		}
		IsCarrying = true;
		if (EagleState != State.Fly)
		{
			if (EagleState == State.Wait)
			{
				StateMachine.ChangeState(StateTurn);
			}
			else
			{
				StateMachine.ChangeState(StateFly);
			}
		}
		PM = collider.GetComponent<PlayerManager>();
		if (PM.Base.GetState() != "Hold")
		{
			player.StateMachine.ChangeState(base.gameObject, StateHold);
		}
	}

	private void OnHoldExit(PlayerBase Player, bool SetJumpOff = true)
	{
		if (!(Player != PM.Base))
		{
			IsCarrying = false;
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.Base.transform.SetParent(null);
			PM = null;
			if (EagleMode == 1 && (EagleState == State.Fly || EagleState == State.Turn))
			{
				StateMachine.ChangeState(StateFlyAway);
			}
			PlayerJumpedOff = SetJumpOff;
		}
	}
}
