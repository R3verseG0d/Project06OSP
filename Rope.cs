using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : ObjectBase
{
	[Header("Framework")]
	public Vector3 Position;

	public float SpeedLow;

	public float SpeedMedium;

	public float SpeedHigh;

	public float TimeLow;

	public float TimeMedium;

	public float TimeHigh;

	[Header("Prefab")]
	public Transform Point;

	public CRSpline Spline;

	public TubeRenderer TubeRenderer;

	public AudioSource Audio;

	public AudioClip ClipLow;

	public AudioClip ClipHigh;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 Center;

	private bool Charging;

	private float ThisLength;

	private int Force;

	private bool Falling;

	private float StateTime;

	private float LaunchTime;

	private float LaunchSpeed;

	private int State;

	private void Start()
	{
		SetupSpline();
	}

	public void SetParameters(Vector3 _Position, float _SpeedLow, float _SpeedMedium, float _SpeedHigh, float _TimeLow, float _TimeMedium, float _TimeHigh)
	{
		Position = _Position;
		SpeedLow = _SpeedLow;
		SpeedMedium = _SpeedMedium;
		SpeedHigh = _SpeedHigh;
		TimeLow = _TimeLow;
		TimeMedium = _TimeMedium;
		TimeHigh = _TimeHigh;
		SetupSpline();
	}

	private void SetupSpline()
	{
		Spline = new CRSpline();
		List<Vector3> knots = Spline.knots;
		knots.Add(base.transform.position);
		knots.Add(base.transform.position);
		Center = (base.transform.position + Position) * 0.5f - Vector3.up * 0.25f;
		Point.position = Center;
		knots.Add(Point.position);
		knots.Add(Position);
		knots.Add(Position);
		ThisLength = Vector3.Distance(base.transform.position, Center) + Vector3.Distance(Center, Position);
	}

	private void FixedUpdate()
	{
		Spline.knots[2] = Point.position;
	}

	private IEnumerator Charge(PlayerBase PlayerBase)
	{
		float StartTime = Time.time;
		Point.right = (Position - base.transform.position).normalized;
		Vector3 StartPosition = Center;
		Vector3 TargetPosition = Center - Vector3.up * 2f;
		Charging = true;
		while (Time.time - StartTime < 0.25f)
		{
			float num = 1f - (Time.time - StartTime) / 0.25f;
			float t = 1f - num * num * num;
			Point.position = Vector3.Lerp(StartPosition, TargetPosition, t);
			yield return new WaitForFixedUpdate();
		}
		float num2 = 0f;
		float num3 = 0f;
		if (Force == 0)
		{
			num2 = TimeLow;
			num3 = SpeedLow;
			Audio.PlayOneShot(ClipLow, Audio.volume);
		}
		else if (Force == 1)
		{
			num2 = TimeMedium;
			num3 = SpeedMedium;
			Audio.PlayOneShot(ClipLow, Audio.volume);
		}
		else if (Force == 2)
		{
			num2 = TimeHigh;
			num3 = SpeedHigh;
			Audio.PlayOneShot(ClipHigh, Audio.volume);
		}
		Force++;
		if (Force > 2)
		{
			Force = 0;
		}
		StartCoroutine(BounceBack(PlayerBase, num3));
		StateTime = Time.time;
		LaunchTime = num2 * 0.5f;
		LaunchSpeed = num3;
		State = 1;
	}

	private IEnumerator BounceBack(PlayerBase PlayerBase, float Speed)
	{
		float StartTime = Time.time;
		Vector3 StartPosition = Center - Vector3.up * 2f;
		Vector3 TargetPosition = Center + Vector3.up * 2.25f;
		float BounceSpeed = Speed;
		bool Execute = true;
		while (Execute)
		{
			float num = (Time.time - StartTime) / Speed * 25f;
			Vector3 vector = Vector3.Lerp(StartPosition, Center, Mathf.Max(0f, num - 1f));
			if (num >= 2f)
			{
				Point.position = Center;
				Execute = false;
			}
			BounceSpeed -= 9.81f * Time.fixedDeltaTime;
			if (Point.position.y > TargetPosition.y)
			{
				BounceSpeed = (0f - Mathf.Abs(BounceSpeed)) * 0.5f;
				Point.position = new Vector3(Point.position.x, TargetPosition.y, Point.position.z);
			}
			else if (Point.position.y < vector.y)
			{
				BounceSpeed = Mathf.Abs(BounceSpeed) * 0.5f;
				Point.position = new Vector3(Point.position.x, vector.y, Point.position.z);
			}
			Point.position += Vector3.up * BounceSpeed * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	private void StateRopeStart()
	{
		PM.Base.SetState("Rope");
		State = 0;
		Falling = false;
		LaunchVelocity = PM.RBody.velocity;
		PM.RBody.velocity = LaunchVelocity;
	}

	private void StateRope()
	{
		PM.Base.SetState("Rope");
		PM.Base.LockControls = State == 0 || (State == 1 && Time.time - StateTime < LaunchTime);
		if (State == 0)
		{
			PM.Base.PlayAnimation("Crouch", "On Crouch");
			PM.transform.position = Point.position + Point.up * 0.25f;
			LaunchVelocity = Vector3.zero;
			PM.RBody.velocity = LaunchVelocity;
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
		}
		else
		{
			bool flag = Time.time - StateTime < LaunchTime;
			if (flag)
			{
				PM.Base.CurSpeed = 0f;
				LaunchVelocity = Vector3.up * LaunchSpeed;
				PM.RBody.velocity = LaunchVelocity;
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
			}
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
			if (LaunchVelocity.y < 0f)
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
			if (PM.Base.IsGrounded() && !flag)
			{
				PM.Base.PositionToPoint();
				PM.Base.SetMachineState("StateGround");
				PM.Base.DoLandAnim();
				PM.PlayerEvents.CreateLandFXAndSound();
			}
		}
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
	}

	private void StateRopeEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() == "Vehicle") && !player.IsDead && (player.GetPrefab("sonic_new") || player.GetPrefab("shadow")) && (!Charging || !(player.GetState() == "Rope") || !(player._Rigidbody.velocity.y > 0f)))
		{
			PM = collider.GetComponent<PlayerManager>();
			Point.position = player.transform.position - player.transform.up * 0.25f;
			player.StateMachine.ChangeState(base.gameObject, StateRope);
			StartCoroutine(Charge(player));
		}
	}

	private void LateUpdate()
	{
		UpdateMesh();
	}

	private void UpdateMesh()
	{
		if (TubeRenderer == null)
		{
			return;
		}
		if (ThisLength == 0f)
		{
			Vector3 vector = (base.transform.position + Position) * 0.5f - Vector3.up;
			ThisLength = Vector3.Distance(base.transform.position, vector) + Vector3.Distance(vector, Position);
		}
		int num = (int)ThisLength * 2;
		float r = 0.0625f;
		TubeRenderer.crossSegments = 6;
		TubeRenderer.vertices = new TubeVertex[num + 1];
		TubeRenderer.uvLength = ThisLength * 2f;
		for (int i = 0; i < num + 1; i++)
		{
			float t = (float)i / ((float)num * 1f);
			Vector3 position = Spline.GetPosition(t);
			Vector3 tangent = Spline.GetTangent(t);
			Vector3 pt = base.transform.InverseTransformPoint(position);
			Vector3 vector2 = base.transform.InverseTransformDirection(tangent);
			Color c = Color.white;
			if (i == 0 || i == num)
			{
				c = Color.black;
			}
			TubeRenderer.vertices[i] = new TubeVertex(pt, r, c);
			if (vector2 != Vector3.zero)
			{
				TubeRenderer.vertices[i].rotation = Quaternion.LookRotation(vector2);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Spline != null)
		{
			Spline.GizmoDraw();
			if (Application.isEditor)
			{
				if (Spline.knots[1] != base.transform.position || Spline.knots[Spline.knots.Count - 1] != Position)
				{
					SetupSpline();
				}
				UpdateMesh();
			}
		}
		Vector3 vector = Point.position - Vector3.up * 2f + Vector3.up * SpeedLow * TimeLow;
		Vector3 vector2 = Point.position - Vector3.up * 2f + Vector3.up * SpeedMedium * TimeMedium;
		Vector3 vector3 = Point.position - Vector3.up * 2f + Vector3.up * SpeedHigh * TimeHigh;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Point.position - Vector3.up * 2f, vector);
		Gizmos.DrawWireSphere(vector, 0.25f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawWireSphere(vector2, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawWireSphere(vector3, 1f);
	}
}
