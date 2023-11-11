using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public enum State
	{
		Normal = 0,
		Talk = 1,
		FirstPerson = 2,
		OverTheShoulder = 3,
		OverTheShoulderFadeIn = 4,
		Cinematic = 5,
		ObjectEvent = 6,
		EventFadeIn = 7,
		EventBFadeIn = 8,
		EventCFadeIn = 9,
		Event = 10,
		EventFadeOut = 11,
		EventCFadeOut = 12,
		EventDFadeOut = 13,
		Death = 14
	}

	[Header("Framework")]
	public StateMachine StateMachine;

	public Camera Camera;

	public Camera SkyboxCamera;

	public Camera OutlineCamera;

	public CameraEffects CameraEffects;

	public Animator Animator;

	public LayerMask layerMask;

	[Header("Debug")]
	public bool TrailerCamera;

	private float TrailerDist;

	private Vector2 TrailerRot;

	internal StageManager StageManager;

	internal CameraParameters parameters;

	internal PlayerBase PlayerBase;

	internal State CameraState;

	internal Transform Target;

	internal bool MultDistance;

	internal bool IsOnEvent;

	internal bool IsGlider;

	internal bool UncancelableEvent;

	internal float Distance;

	internal float DistanceToTarget;

	internal float AltitudeMultpUp;

	internal float AltitudeMultpDown;

	internal float CinematicTime;

	internal float ObjectEventTime;

	private bool IsMachSpeed;

	private bool HasObjPosition;

	private bool HasObjTarget;

	private float SpringK;

	private float Altitude;

	private float UpAltitude = 1f;

	private float DownAltitude = 1f;

	private float DefaultSpringK = 0.98f;

	private float EventFadeTime;

	private float LeftStickX;

	private float LeftStickY;

	private float RightStickX;

	private float RightStickY;

	private float TalkCamHeight;

	private float TalkCamDist;

	private float RotYAxis;

	private float RotXAxis;

	private float CineTime;

	private float ObjEvtTime;

	private void Start()
	{
		Camera.fieldOfView = ((Singleton<Settings>.Instance.settings.CameraType != 2) ? 45f : 55f);
		SkyboxCamera.fieldOfView = Camera.fieldOfView;
		OutlineCamera.fieldOfView = Camera.fieldOfView;
		TrailerDist = 3f;
		StateMachine.Initialize(StateNormal);
	}

	public float SetDistance()
	{
		float result = 0f;
		if (Singleton<GameManager>.Instance.GameStory == GameManager.Story.Sonic)
		{
			if (StageManager._Stage == StageManager.Stage.wvo && StageManager.StageSection == StageManager.Section.B)
			{
				result = 3.5f;
				IsMachSpeed = true;
			}
			else if (StageManager._Stage == StageManager.Stage.csc && StageManager.StageSection == StageManager.Section.E)
			{
				result = 1.25f;
				IsMachSpeed = true;
			}
			else if (StageManager._Stage == StageManager.Stage.rct && StageManager.StageSection == StageManager.Section.B)
			{
				result = 7f;
				IsMachSpeed = true;
			}
			else if (StageManager._Stage == StageManager.Stage.kdv && StageManager.StageSection == StageManager.Section.C)
			{
				result = 2.5f;
				IsMachSpeed = true;
			}
		}
		if (!IsMachSpeed)
		{
			result = ((Singleton<Settings>.Instance.settings.CameraType == 2) ? 3f : ((Singleton<Settings>.Instance.settings.CameraType == 1) ? 5.5f : 7f));
		}
		return result;
	}

	private float CamDistance()
	{
		return SetDistance() * (MultDistance ? 2f : 1f);
	}

	private void StateNormalStart()
	{
	}

	public void StateNormal()
	{
		CameraState = State.Normal;
		CameraNormalUpdate();
	}

	private void StateNormalEnd()
	{
	}

	private void StateTalkStart()
	{
		TalkCamHeight = (PlayerBase.GetPrefab("tails") ? 0.375f : (PlayerBase.GetPrefab("omega") ? 0.95f : 0.45f));
		TalkCamDist = (PlayerBase.GetPrefab("omega") ? 1.5f : 1f);
	}

	public void StateTalk()
	{
		CameraState = State.Talk;
		base.transform.forward = -PlayerBase.transform.forward;
		base.transform.position = PlayerBase.transform.position + PlayerBase.transform.up * TalkCamHeight - -PlayerBase.transform.forward * TalkCamDist;
	}

	private void StateTalkEnd()
	{
	}

	private void StateFirstPersonStart()
	{
		base.transform.position = Target.position - base.transform.forward.MakePlanar() * 2f;
	}

	public void StateFirstPerson()
	{
		CameraState = State.FirstPerson;
		Vector3 zero = Vector3.zero;
		Vector3 normalized = (Target.position - base.transform.position).normalized;
		base.transform.RotateAround(Target.position, Vector3.up, Time.fixedDeltaTime * LeftStickX * 80f);
		base.transform.RotateAround(Target.position, base.transform.right, Time.fixedDeltaTime * LeftStickY * 80f);
		DistanceToTarget = Vector3.Distance(base.transform.position, Target.position);
		if (DistanceToTarget > 1.5f)
		{
			zero += normalized * (DistanceToTarget - 1.5f);
		}
		else if (DistanceToTarget < 1.5f)
		{
			zero -= normalized * (1.5f - DistanceToTarget);
		}
		float num = Target.position.y - base.transform.position.y;
		float num2 = num + 1f;
		float num3 = num - 1f;
		if (num2 < 0f)
		{
			zero += Target.up * num2;
		}
		else if (num3 > 0f)
		{
			zero += Target.up * num3;
		}
		base.transform.position += zero * DefaultSpringK;
		base.transform.forward = (Target.position - base.transform.position).normalized;
	}

	private void StateFirstPersonEnd()
	{
	}

	private void StateOverTheShoulderStart()
	{
		base.transform.position = Target.position - base.transform.forward.MakePlanar() * 1f;
		RotYAxis = base.transform.eulerAngles.y;
		RotXAxis = base.transform.eulerAngles.x;
	}

	private void StateOverTheShoulder()
	{
		CameraState = State.OverTheShoulder;
		RotYAxis += Time.fixedDeltaTime * RightStickX * 70f;
		RotXAxis += Time.fixedDeltaTime * RightStickY * 70f;
		RotXAxis = ClampAngle(RotXAxis, -40f, 40f);
		Quaternion rotation = Quaternion.Euler(RotXAxis, RotYAxis, 0f);
		base.transform.rotation = rotation;
		base.transform.position = Target.position - base.transform.forward * 1f;
	}

	private void StateOverTheShoulderEnd()
	{
	}

	private float ClampAngle(float Angle, float Min, float Max)
	{
		if (Angle < -360f)
		{
			Angle += 360f;
		}
		if (Angle > 360f)
		{
			Angle -= 360f;
		}
		return Mathf.Clamp(Angle, Min, Max);
	}

	private void StateOverTheShoulderFadeInStart()
	{
		EventFadeTime = Time.time;
	}

	public void StateOverTheShoulderFadeIn()
	{
		CameraState = State.OverTheShoulderFadeIn;
		float num = (Time.time - EventFadeTime) * 0.75f;
		num *= num;
		Vector3 normalized = (Target.position - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward.MakePlanar() * 1f, num);
		if (num > 0.5f)
		{
			StateMachine.ChangeState(StateOverTheShoulder);
		}
	}

	private void StateOverTheShoulderFadeInEnd()
	{
	}

	private void StateCinematicStart()
	{
		CineTime = Time.time;
		CinematicLogic();
	}

	private void StateCinematic()
	{
		CameraState = State.Cinematic;
		CinematicLogic();
		if (Time.time - CineTime > CinematicTime)
		{
			StateMachine.ChangeState(StateEventFadeOut);
		}
	}

	private void StateCinematicEnd()
	{
		Animator.SetTrigger("State Ended");
	}

	private void CinematicLogic()
	{
		base.transform.forward = PlayerBase.transform.forward;
		base.transform.position = PlayerBase.transform.position + PlayerBase.transform.up * 0.85f - PlayerBase.transform.forward * 6.5f;
	}

	private void StateObjectEventStart()
	{
		ObjEvtTime = Time.time;
		ObjectEventLogic();
	}

	private void StateObjectEvent()
	{
		CameraState = State.ObjectEvent;
		ObjectEventLogic();
		if (Time.time - ObjEvtTime > ObjectEventTime)
		{
			StateMachine.ChangeState(StateNormal);
			if (!TrailerCamera)
			{
				base.transform.position = Target.position - PlayerBase.transform.forward * Distance;
				base.transform.forward = (Target.position - base.transform.position).normalized;
			}
			else
			{
				TrailerDist = 3f;
				TrailerRot = Vector2.zero;
			}
		}
	}

	private void StateObjectEventEnd()
	{
		Animator.SetTrigger("State Ended");
	}

	private void ObjectEventLogic()
	{
		base.transform.forward = (parameters.Target - parameters.Target).normalized;
		base.transform.position = parameters.Position;
	}

	private void StateEventFadeInStart()
	{
		EventFadeTime = Time.time;
	}

	private void StateEventFadeIn()
	{
		CameraState = State.EventFadeIn;
		float num = (Time.time - EventFadeTime) * 1.5f;
		num *= num;
		Vector3 normalized = (((!HasObjTarget) ? parameters.Position : parameters.ObjTarget.position) - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		if (parameters.Mode == 11)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance + Vector3.up * Distance / 7.5f, num);
		}
		else
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance, num);
		}
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateEvent);
		}
	}

	private void StateEventFadeInEnd()
	{
	}

	private void StateEventBFadeInStart()
	{
		EventFadeTime = Time.time;
	}

	private void StateEventBFadeIn()
	{
		CameraState = State.EventBFadeIn;
		float num = (Time.time - EventFadeTime) * 1.5f;
		num *= num;
		Vector3 normalized = (((!HasObjTarget) ? parameters.Target : parameters.ObjTarget.position) - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, (!HasObjPosition) ? parameters.Position : parameters.ObjPosition.position, num);
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateEvent);
		}
	}

	private void StateEventBFadeInEnd()
	{
	}

	private void StateEventCFadeInStart()
	{
		EventFadeTime = Time.time;
	}

	private void StateEventCFadeIn()
	{
		CameraState = State.EventCFadeIn;
		float num = (Time.time - EventFadeTime) * 1.5f;
		num *= num;
		Vector3 normalized = (parameters.Position - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance * (PlayerBase.GetPrefab("sonic_fast") ? 4f : 1f), num);
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateEvent);
		}
	}

	private void StateEventCFadeInEnd()
	{
	}

	private void StateEventStart()
	{
		EventLogic();
	}

	private void StateEvent()
	{
		CameraState = State.Event;
		EventLogic();
	}

	private void StateEventEnd()
	{
	}

	private void EventLogic()
	{
		if (parameters.Mode == 1 || parameters.Mode == 10 || parameters.Mode == 11)
		{
			if (parameters.Mode == 11)
			{
				DistanceToTarget = Vector3.Distance(base.transform.position, Target.position + Vector3.up * Distance / 7.5f);
				base.transform.position = Target.position - base.transform.forward * Distance + Vector3.up * Distance / 7.5f;
			}
			else
			{
				DistanceToTarget = Vector3.Distance(base.transform.position, Target.position);
				base.transform.position = Target.position - base.transform.forward * Distance;
			}
			if (!HasObjTarget)
			{
				base.transform.forward = (parameters.Position - base.transform.position).normalized;
			}
		}
		else if (parameters.Mode == 2)
		{
			base.transform.position = ProjectPositionOnRail(Target.position);
			base.transform.forward = (Target.position - base.transform.position).normalized;
		}
		else if (parameters.Mode == 3 || parameters.Mode == 30 || parameters.Mode == 31)
		{
			if (!HasObjPosition)
			{
				base.transform.position = parameters.Position;
			}
			if (!HasObjTarget)
			{
				base.transform.forward = (parameters.Target - base.transform.position).normalized;
			}
		}
		else if (parameters.Mode == 4 || parameters.Mode == 40 || parameters.Mode == 41 || parameters.Mode == 42)
		{
			if (!HasObjPosition)
			{
				base.transform.position = parameters.Position;
			}
		}
		else if ((parameters.Mode == 5 || parameters.Mode == 50) && PlayerBase.GetPrefab("sonic_fast"))
		{
			DistanceToTarget = Vector3.Distance(base.transform.position, Target.position);
			base.transform.position = Target.position - base.transform.forward * Distance * 4f;
			base.transform.forward = (parameters.Position - base.transform.position).normalized;
		}
	}

	public Vector3 ProjectPositionOnRail(Vector3 Pos)
	{
		return ProjectOnSegment(parameters.Position, parameters.Target, Pos);
	}

	public Vector3 ProjectOnSegment(Vector3 V1, Vector3 V2, Vector3 Pos)
	{
		Vector3 rhs = Pos - V1;
		Vector3 normalized = (V2 - V1).normalized;
		float num = Vector3.Dot(normalized, rhs);
		if (num < 0f)
		{
			return V1;
		}
		if (num * num > (V2 - V1).sqrMagnitude)
		{
			return V2;
		}
		Vector3 vector = normalized * num;
		return V1 + vector;
	}

	private void StateEventFadeOutStart()
	{
		if (TrailerCamera)
		{
			StateMachine.ChangeState(StateNormal);
		}
		EventFadeTime = Time.time;
	}

	public void StateEventFadeOut()
	{
		CameraState = State.EventFadeOut;
		float num = (Time.time - EventFadeTime) * 1.5f;
		num *= num;
		Vector3 normalized = (Target.position - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance, num);
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateNormal);
		}
	}

	private void StateEventFadeOutEnd()
	{
	}

	private void StateEventCFadeOutStart()
	{
		if (TrailerCamera)
		{
			StateMachine.ChangeState(StateNormal);
		}
		EventFadeTime = Time.time;
	}

	private void StateEventCFadeOut()
	{
		CameraState = State.EventCFadeOut;
		float num = (Time.time - EventFadeTime) * 1.5f;
		Vector3 normalized = (Target.position - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance, num);
		CameraNormalUpdate(num);
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateNormal);
		}
	}

	private void StateEventCFadeOutEnd()
	{
	}

	private void StateEventDFadeOutStart()
	{
		if (TrailerCamera)
		{
			StateMachine.ChangeState(StateNormal);
		}
		EventFadeTime = Time.time;
	}

	public void StateEventDFadeOut()
	{
		CameraState = State.EventDFadeOut;
		float num = (Time.time - EventFadeTime) * 1.5f;
		num *= num;
		Vector3 normalized = (Target.position - base.transform.position).normalized;
		Vector3 forward = Vector3.Slerp(base.transform.forward, normalized, num);
		forward.y = Mathf.Lerp(base.transform.forward.y, normalized.y, num);
		base.transform.forward = forward;
		base.transform.position = Vector3.Lerp(base.transform.position, Target.position - base.transform.forward * Distance, num);
		if (num > DefaultSpringK)
		{
			StateMachine.ChangeState(StateNormal);
		}
	}

	private void StateEventDFadeOutEnd()
	{
	}

	private void StateDeathStart()
	{
	}

	private void StateDeath()
	{
		CameraState = State.Death;
	}

	private void StateDeathEnd()
	{
	}

	private void CameraCollision(Vector3 position, Vector3 direction, float distance)
	{
		int num = 0;
		RaycastHit hitInfo;
		while (Physics.SphereCast(position, 0.25f, direction, out hitInfo, distance, layerMask) && num < 50)
		{
			if (((bool)hitInfo.collider.GetComponent<PhysicsObj>() && hitInfo.collider.GetComponent<PhysicsObj>().IsPsychokinesis) || ((bool)hitInfo.collider.GetComponent<BrokenSnowBall>() && hitInfo.collider.GetComponent<BrokenSnowBall>().IsPsychokinesis) || ((bool)hitInfo.collider.GetComponent<Parasol>() && hitInfo.collider.GetComponent<Parasol>().IsPsychokinesis))
			{
				return;
			}
			Vector3 normalized = Vector3.Cross(Vector3.Cross(direction, -hitInfo.normal).normalized, hitInfo.normal).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(direction, -normalized).normalized, base.transform.forward).normalized;
			base.transform.position += normalized2 * Time.fixedDeltaTime;
			direction = (base.transform.position - position).normalized;
			num++;
		}
		CameraPush(position, direction, distance);
	}

	private void CameraPush(Vector3 position, Vector3 direction, float distance)
	{
		if (Physics.SphereCast(position, 0.25f, direction, out var hitInfo, distance, layerMask) && (!hitInfo.collider.GetComponent<PhysicsObj>() || !hitInfo.collider.GetComponent<PhysicsObj>().IsPsychokinesis) && (!hitInfo.collider.GetComponent<BrokenSnowBall>() || !hitInfo.collider.GetComponent<BrokenSnowBall>().IsPsychokinesis) && (!hitInfo.collider.GetComponent<Parasol>() || !hitInfo.collider.GetComponent<Parasol>().IsPsychokinesis) && hitInfo.distance < distance)
		{
			base.transform.position += -direction * (distance - hitInfo.distance);
		}
	}

	private void CameraNormalUpdate(float Speed = 0.98f)
	{
		if (!TrailerCamera)
		{
			SpringK = ((Singleton<Settings>.Instance.settings.CameraType == 0) ? 0.98f : (PlayerBase.GetPrefab("sonic_fast") ? 0.2f : 0.18f));
			Speed = SpringK;
			Vector3 zero = Vector3.zero;
			Vector3 normalized = (Target.position - base.transform.position).normalized;
			base.transform.RotateAround(Target.position, Camera.transform.up, Time.fixedDeltaTime * RightStickX * 125f);
			float num = Vector3.Dot(normalized, Vector3.up);
			if ((!MultDistance && (!(num < -0.75f) || !(RightStickY > 0.2f))) || MultDistance)
			{
				base.transform.RotateAround(Target.position, Camera.transform.right, Time.fixedDeltaTime * ((!MultDistance || (MultDistance && (IsMachSpeed || parameters.Mode == 102))) ? RightStickY : 0.5f) * 125f);
			}
			if (Singleton<RInput>.Instance.P.GetButton("Left Bumper") && !Singleton<RInput>.Instance.P.GetButton("Right Bumper"))
			{
				RightStickX = Mathf.Lerp(RightStickX, 1f * (float)((Singleton<Settings>.Instance.settings.InvertCamX == 1) ? 1 : (-1)), Time.fixedDeltaTime * 10f);
			}
			else if (Singleton<RInput>.Instance.P.GetButton("Right Bumper") && !Singleton<RInput>.Instance.P.GetButton("Left Bumper"))
			{
				RightStickX = Mathf.Lerp(RightStickX, -1f * (float)((Singleton<Settings>.Instance.settings.InvertCamX == 1) ? 1 : (-1)), Time.fixedDeltaTime * 10f);
			}
			DistanceToTarget = Vector3.Distance(base.transform.position, Target.position);
			if (DistanceToTarget > Distance)
			{
				zero += normalized * (DistanceToTarget - Distance);
			}
			else if (DistanceToTarget < Distance)
			{
				zero -= normalized * (Distance - DistanceToTarget);
			}
			float num2 = Target.position.y - base.transform.position.y;
			float num3 = num2 + Altitude * AltitudeMultpDown;
			float num4 = num2 - Altitude * AltitudeMultpUp;
			if (num3 < 0f)
			{
				zero.y += num3;
			}
			else if (num4 > 0f)
			{
				zero.y += num4;
			}
			base.transform.position += zero * Speed;
		}
	}

	public void SetParams(CameraParameters _Parameters)
	{
		if (CameraState == State.Death || CameraState == State.OverTheShoulder || CameraState == State.Cinematic || UncancelableEvent)
		{
			return;
		}
		parameters = _Parameters;
		HasObjPosition = parameters.ObjPosition;
		HasObjTarget = parameters.ObjTarget;
		if (_Parameters.Mode == 1 || _Parameters.Mode == 11)
		{
			StateMachine.ChangeState(StateEventFadeIn);
			if (_Parameters.Mode == 11)
			{
				MultDistance = true;
			}
		}
		else if (_Parameters.Mode == 30)
		{
			StateMachine.ChangeState(StateEventBFadeIn);
		}
		else if (_Parameters.Mode == 10 || _Parameters.Mode == 2 || _Parameters.Mode == 3 || _Parameters.Mode == 31 || _Parameters.Mode == 4 || _Parameters.Mode == 40 || _Parameters.Mode == 41 || _Parameters.Mode == 42 || _Parameters.Mode == 5 || _Parameters.Mode == 100 || _Parameters.Mode == 104)
		{
			StateMachine.ChangeState(StateEvent);
		}
		else if (_Parameters.Mode == 50)
		{
			StateMachine.ChangeState(StateEventCFadeIn);
		}
		else if (_Parameters.Mode == 101 || _Parameters.Mode == 102 || _Parameters.Mode == 103)
		{
			if (CameraState == State.Talk || CameraState == State.FirstPerson || CameraState == State.OverTheShoulder || CameraState == State.OverTheShoulderFadeIn || CameraState == State.Cinematic || CameraState == State.ObjectEvent || CameraState == State.EventFadeIn || CameraState == State.EventBFadeIn || CameraState == State.EventCFadeIn || CameraState == State.Event)
			{
				StateMachine.ChangeState(StateEventFadeOut);
			}
			MultDistance = true;
		}
	}

	public void DestroyParams(CameraParameters _Parameters)
	{
		if (parameters == _Parameters && (CameraState == State.Event || CameraState == State.EventFadeIn) && !UncancelableEvent && CameraState != State.OverTheShoulder && CameraState != State.Cinematic)
		{
			if (_Parameters.Mode == 1 || _Parameters.Mode == 10 || _Parameters.Mode == 11 || _Parameters.Mode == 2 || _Parameters.Mode == 3 || _Parameters.Mode == 30 || _Parameters.Mode == 4 || _Parameters.Mode == 5 || _Parameters.Mode == 50 || _Parameters.Mode == 100 || _Parameters.Mode == 104)
			{
				StateMachine.ChangeState(StateEventFadeOut);
			}
			else if (_Parameters.Mode == 40)
			{
				StateMachine.ChangeState(StateEventCFadeOut);
			}
			else if (_Parameters.Mode == 41 || _Parameters.Mode == 31)
			{
				StateMachine.ChangeState(StateNormal);
				base.transform.position = Target.position - PlayerBase.transform.forward * Distance;
				base.transform.forward = (Target.position - base.transform.position).normalized;
			}
			else if (_Parameters.Mode == 42)
			{
				StateMachine.ChangeState(StateEventDFadeOut);
			}
		}
		if (_Parameters.Mode == 11 || _Parameters.Mode == 101 || _Parameters.Mode == 102 || _Parameters.Mode == 103)
		{
			MultDistance = false;
		}
	}

	public void OnPlayerDeath()
	{
		StateMachine.ChangeState(StateDeath);
	}

	public void OnPlayerTalkEnd()
	{
		StateMachine.ChangeState(StateNormal);
		if (!TrailerCamera)
		{
			base.transform.position = Target.position - PlayerBase.transform.forward * Distance;
			base.transform.forward = (Target.position - base.transform.position).normalized;
		}
		else
		{
			TrailerDist = 3f;
			TrailerRot = Vector2.zero;
		}
	}

	private void CameraParam()
	{
		if (PlayerBase.GetPrefab("sonic_fast"))
		{
			Distance = Mathf.Lerp(Distance, CamDistance() * ((!(PlayerBase.CurSpeed > 85f)) ? 1f : ((!MultDistance) ? 0.1f : 0.25f)) + 1.5f, Time.deltaTime * ((PlayerBase.CurSpeed > 85f) ? 1f : 2.5f));
		}
		else if (PlayerBase.GetPrefab("snow_board"))
		{
			Distance = Mathf.Lerp(Distance, CamDistance() * ((!PlayerBase.IsGrounded() || CameraState != 0) ? 1f : 0.625f) + 1.5f, Time.deltaTime);
		}
		else if (PlayerBase.GetPrefab("shadow") || PlayerBase.GetPrefab("silver"))
		{
			Distance = Mathf.Lerp(Distance, CamDistance() * ((PlayerBase.GetState() != "ChaosBoost" && PlayerBase.GetState() != "ESPAwaken") ? 1f : (PlayerBase.GetPrefab("shadow") ? 0.625f : 1.375f)), Time.deltaTime * 5f);
		}
		else if (PlayerBase.GetPrefab("blaze"))
		{
			Distance = Mathf.Lerp(Distance, CamDistance() * ((PlayerBase.GetState() != "HeatBurst") ? 1f : 1.5f), Time.deltaTime * 2.5f);
		}
		else
		{
			Distance = Mathf.Lerp(Distance, CamDistance(), Time.deltaTime * 2.5f);
		}
		Altitude = ((Singleton<Settings>.Instance.settings.CameraType == 0) ? 4f : ((Singleton<Settings>.Instance.settings.CameraType == 1) ? 2f : 1f));
		if (CameraState == State.EventFadeIn || CameraState == State.EventBFadeIn || CameraState == State.EventCFadeIn || CameraState == State.Event || CameraState == State.EventFadeOut || CameraState == State.EventCFadeOut || CameraState == State.EventDFadeOut)
		{
			UpAltitude = 2f;
			DownAltitude = 2f;
		}
		else
		{
			if (!IsMachSpeed)
			{
				UpAltitude = Mathf.Lerp(UpAltitude, (PlayerBase.GetState() != "Jump" && PlayerBase.GetState() != "AccelJump" && PlayerBase.GetState() != "AfterHoming" && PlayerBase.GetState() != "BoundAttack" && PlayerBase.GetState() != "Homing" && PlayerBase.GetState() != "Upheave" && PlayerBase.GetState() != "Fly" && PlayerBase.GetState() != "AerialTailSwipe") ? 1f : 0.15f, Time.deltaTime * 2.5f);
			}
			else
			{
				UpAltitude = Mathf.MoveTowards(UpAltitude, 0f, Time.deltaTime * 2.5f);
			}
			DownAltitude = Mathf.MoveTowards(DownAltitude, 1f, Time.deltaTime * 2.5f);
		}
		float num = 1f / Altitude;
		AltitudeMultpUp = (IsMachSpeed ? UpAltitude : ((PlayerBase.IsOnWall && PlayerBase.GetState() != "Path") ? Mathf.Lerp(AltitudeMultpUp, num * 0.5f, Time.deltaTime * 0.5f) : UpAltitude));
		AltitudeMultpDown = (IsMachSpeed ? DownAltitude : ((PlayerBase.IsOnWall && PlayerBase.GetState() != "Path") ? Mathf.Lerp(AltitudeMultpDown, num * 0.5f, Time.deltaTime * 0.5f) : DownAltitude));
	}

	public void PlayCinematic(float Timer, string Animation)
	{
		CinematicTime = Timer;
		Animator.SetTrigger(Animation);
		StateMachine.ChangeState(StateCinematic);
	}

	public void PlayObjectEvent(float Timer, string Animation, CameraParameters _Parameters)
	{
		ObjectEventTime = Timer;
		Animator.SetTrigger(Animation);
		parameters = _Parameters;
		StateMachine.ChangeState(StateObjectEvent);
	}

	public void PlayShakeMotion(float Timer = 0f, float Intensity = 1f, bool FastShake = false)
	{
		StopCoroutine(TriggerShake(Timer, Intensity, FastShake));
		StartCoroutine(TriggerShake(Timer, Intensity, FastShake));
	}

	public void PlayImpactShakeMotion()
	{
		Animator.SetTrigger("Impact Shake");
	}

	private IEnumerator TriggerShake(float Timer, float Intensity, bool FastShake)
	{
		yield return new WaitForSeconds(Timer);
		Animator.SetFloat("Shake Intensity", Intensity);
		Animator.SetTrigger((!FastShake) ? "Shake" : "Fast Shake");
	}

	private void Update()
	{
		UpdateCameraForward();
		if (!TrailerCamera)
		{
			CameraParam();
			if (PlayerBase.GetPrefab("sonic_fast"))
			{
				Camera.fieldOfView = ((Singleton<GameManager>.Instance.GameState != GameManager.State.Result) ? Mathf.Lerp(Camera.fieldOfView, (!(PlayerBase.CurSpeed > 85f)) ? ((Singleton<Settings>.Instance.settings.CameraType != 2) ? 45f : 55f) : ((Singleton<Settings>.Instance.settings.CameraType != 2) ? 65f : 75f), Time.deltaTime * ((PlayerBase.CurSpeed > 85f) ? 1f : 2.5f)) : 55f);
				SkyboxCamera.fieldOfView = Camera.fieldOfView;
				if ((bool)OutlineCamera)
				{
					OutlineCamera.fieldOfView = Camera.fieldOfView;
				}
			}
		}
		else if (CameraState == State.Normal)
		{
			if (Singleton<RInput>.Instance.P.GetButton("Left Bumper"))
			{
				TrailerDist += Time.deltaTime * 4f;
			}
			else if (Singleton<RInput>.Instance.P.GetButton("Right Bumper"))
			{
				TrailerDist -= Time.deltaTime * 4f;
			}
			TrailerDist = Mathf.Clamp(TrailerDist, 0.75f, 20f);
			TrailerRot.y += Time.deltaTime * ((0f - Singleton<RInput>.Instance.P.GetAxis("Right Stick X")) * (float)((Singleton<Settings>.Instance.settings.InvertCamX == 1) ? 1 : (-1))) * 90f;
			TrailerRot.x += Time.deltaTime * (Singleton<RInput>.Instance.P.GetAxis("Right Stick Y") * (float)((Singleton<Settings>.Instance.settings.InvertCamY == 1) ? 1 : (-1))) * 90f;
			TrailerRot.x = ClampAngle(TrailerRot.x, -40f, 40f);
			Quaternion rotation = Quaternion.Euler(TrailerRot.x, TrailerRot.y, 0f);
			base.transform.rotation = rotation;
			base.transform.position = PlayerBase.transform.position + Vector3.up * 0.3f - base.transform.forward * TrailerDist;
		}
		if (PlayerBase.GetState() != "Tarzan" && CameraState == State.Event && parameters.Mode == 100)
		{
			UncancelableEvent = false;
			DestroyParams(parameters);
		}
	}

	private void FixedUpdate()
	{
		StateMachine.UpdateStateMachine();
		if (!TrailerCamera)
		{
			LeftStickX = Mathf.Lerp(LeftStickX, Singleton<RInput>.Instance.P.GetAxis("Left Stick X"), Time.fixedDeltaTime * 15f);
			LeftStickY = Mathf.Lerp(LeftStickY, 0f - Singleton<RInput>.Instance.P.GetAxis("Left Stick Y"), Time.fixedDeltaTime * 15f);
			if (!Singleton<RInput>.Instance.P.GetButton("Left Bumper") && !Singleton<RInput>.Instance.P.GetButton("Right Bumper"))
			{
				RightStickX = Mathf.Lerp(RightStickX, (0f - Singleton<RInput>.Instance.P.GetAxis("Right Stick X")) * (float)((Singleton<Settings>.Instance.settings.InvertCamX == 1 && CameraState != State.OverTheShoulder) ? 1 : (-1)), Time.fixedDeltaTime * 10f);
			}
			RightStickY = Mathf.Lerp(RightStickY, (0f - Singleton<RInput>.Instance.P.GetAxis("Right Stick Y")) * (float)((Singleton<Settings>.Instance.settings.InvertCamY == 1 || CameraState == State.OverTheShoulder) ? 1 : (-1)), Time.fixedDeltaTime * 10f);
			Vector3 normalized = (base.transform.position - Target.position).normalized;
			if (CameraState == State.Normal || CameraState == State.EventFadeOut || CameraState == State.EventCFadeOut)
			{
				CameraCollision(Target.position, normalized, Vector3.Distance(Target.position, base.transform.position));
			}
			else if ((CameraState == State.Event && (parameters.Mode == 1 || parameters.Mode == 10 || parameters.Mode == 11)) || CameraState == State.EventFadeIn)
			{
				CameraPush(Target.position, normalized, Vector3.Distance(Target.position, base.transform.position));
			}
		}
		IsOnEvent = CameraState == State.Talk || CameraState == State.FirstPerson || CameraState == State.OverTheShoulder || CameraState == State.OverTheShoulderFadeIn || CameraState == State.Cinematic;
	}

	private void UpdateCameraForward()
	{
		if (CameraState == State.Normal || CameraState == State.FirstPerson)
		{
			if (CameraState == State.Normal && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && Singleton<RInput>.Instance.P.GetButtonDown("Left Trigger"))
			{
				if (!TrailerCamera)
				{
					if (!MultDistance)
					{
						base.transform.position = Target.position - PlayerBase.transform.forward * Distance;
					}
					else
					{
						base.transform.position = Target.position - PlayerBase.transform.forward * Distance + base.transform.up * 4f;
					}
				}
				else
				{
					TrailerDist = 3f;
					base.transform.position = PlayerBase.transform.position + Vector3.up * 0.3f - PlayerBase.transform.forward * TrailerDist;
					base.transform.forward = PlayerBase.transform.forward;
					TrailerRot = new Vector2(base.transform.eulerAngles.x, base.transform.eulerAngles.y);
				}
			}
			if (!TrailerCamera)
			{
				base.transform.forward = (Target.position - base.transform.position).normalized;
			}
		}
		else if (CameraState == State.Event)
		{
			if (parameters.Mode == 1 || parameters.Mode == 10 || parameters.Mode == 11)
			{
				if (HasObjTarget)
				{
					base.transform.forward = (parameters.ObjTarget.position - base.transform.position).normalized;
				}
			}
			else if (parameters.Mode == 3 || parameters.Mode == 30 || parameters.Mode == 31)
			{
				if (HasObjPosition)
				{
					base.transform.position = parameters.ObjPosition.position;
				}
				if (HasObjTarget)
				{
					base.transform.forward = (parameters.ObjTarget.position - base.transform.position).normalized;
				}
			}
			else if (parameters.Mode == 4 || parameters.Mode == 40 || parameters.Mode == 41 || parameters.Mode == 42)
			{
				if (HasObjPosition)
				{
					base.transform.position = parameters.ObjPosition.position;
				}
				base.transform.forward = (Target.position - base.transform.position).normalized;
			}
			else if (parameters.Mode == 100)
			{
				Vector3 position = PlayerBase.transform.position + PlayerBase.transform.right * parameters.Position.x + PlayerBase.transform.up * parameters.Position.y + PlayerBase.transform.forward * parameters.Position.z;
				Vector3 vector = PlayerBase.transform.position + PlayerBase.transform.right * parameters.Target.x + PlayerBase.transform.up * parameters.Target.y + PlayerBase.transform.forward * parameters.Target.z;
				base.transform.position = position;
				base.transform.forward = (vector - base.transform.position).normalized;
			}
			else if (parameters.Mode == 104)
			{
				Vector3 position2 = Target.position + Vector3.right * parameters.Position.x + Vector3.up * parameters.Position.y + Vector3.forward * parameters.Position.z;
				Vector3 vector2 = Target.position + Vector3.right * parameters.Target.x + Vector3.up * parameters.Target.y + Vector3.forward * parameters.Target.z;
				base.transform.position = position2;
				base.transform.forward = (vector2 - base.transform.position).normalized;
			}
			else if ((parameters.Mode == 5 || parameters.Mode == 50) && !PlayerBase.GetPrefab("sonic_fast"))
			{
				base.transform.position = Target.position - base.transform.forward * Distance;
				base.transform.forward = (parameters.Position - base.transform.position).normalized;
			}
		}
		else if (CameraState == State.Death)
		{
			base.transform.forward = (Target.position - base.transform.position).normalized;
		}
	}
}
