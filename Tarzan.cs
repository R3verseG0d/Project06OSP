using System.Collections.Generic;
using UnityEngine;

public class Tarzan : ObjectBase
{
	[Header("Framework")]
	public float Top;

	public float Bottom;

	public float JumpSpeed;

	public float JumpCheckSpeed;

	public bool FixDirection;

	public float HittutaSpeed;

	public float Weight;

	public float HitAngle;

	public float JumpCheckAngle;

	public Vector3 TargetObj;

	public float CamZ;

	public float CamY;

	public bool IsRouge;

	[Header("Optional")]
	public float YOffset;

	public bool FixedTarget;

	[Header("Prefab")]
	public TubeRenderer TubeRenderer;

	public Transform TopPivot;

	public Transform PlayerPoint;

	public Transform Collider;

	public Renderer BudRenderer;

	public Light BudLight;

	public Transform BudPoint;

	public Transform BudObj;

	public Transform BudSplineBalance;

	public ParticleSystem PeakFX;

	public GameObject SuccessJumpFX;

	public AudioSource Audio;

	public AudioClip[] Clips;

	public AudioSource SwingWindAudio;

	private CRSpline Spline;

	private CameraParameters CameraParams;

	private MaterialPropertyBlock PropBlock;

	private PlayerManager PM;

	private AnimationCurve Movement;

	private AnimationCurve BudMovement;

	private AnimationCurve Volume;

	private Vector3 LaunchVelocity;

	private Vector3 StartLaunchVelocity;

	private Quaternion MeshLaunchRot;

	private Vector3 Center;

	private bool IsSwinging;

	private bool Falling;

	private bool CanJump;

	private bool PlayPeakFX;

	private float StartTime;

	private float RotFactor;

	private float CurveTime;

	private float GlowInt;

	private float ThisLength;

	private int TarzanState;

	private int TarzanAnim;

	public void SetParameters(float _Top, float _Bottom, float _JumpSpeed, float _JumpCheckSpeed, bool _FixDirection, float _HittutaSpeed, float _Weight, float _HitAngle, float _JumpCheckAngle, Vector3 _TargetObj, float _CamZ, float _CamY, bool _IsRouge)
	{
		Top = _Top;
		Bottom = _Bottom;
		JumpSpeed = _JumpSpeed;
		JumpCheckSpeed = _JumpCheckSpeed;
		FixDirection = _FixDirection;
		HittutaSpeed = _HittutaSpeed;
		Weight = _Weight;
		HitAngle = _HitAngle;
		JumpCheckAngle = _JumpCheckAngle;
		TargetObj = _TargetObj;
		CamZ = _CamZ;
		CamY = _CamY;
		IsRouge = _IsRouge;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		BudObj.localPosition = new Vector3(0f, 0f - Bottom, 0f);
		TopPivot.localPosition = new Vector3(0f, Top, 0f);
		PlayerPoint.localPosition = new Vector3(0f, 0f - Top + Bottom, 0f);
		BudSplineBalance.localPosition = new Vector3(0f, (Top - Bottom) / 2f, 0f);
		Collider.localPosition = new Vector3(0f, 0f - Bottom, 0f);
		GlowInt = 1f;
		CameraParams = new CameraParameters(100, new Vector3(1f, -1.5f, -4f), new Vector3(0f, 0.75f, 3f));
		Movement = new AnimationCurve();
		Keyframe[] array = new Keyframe[5];
		array[0].time = 0f;
		array[0].value = 0f;
		array[1].time = 0.5f;
		array[1].value = HitAngle;
		array[2].time = 1f;
		array[2].value = 0f;
		array[3].time = 1.5f;
		array[3].value = 0f - HitAngle;
		array[4].time = 2f;
		array[4].value = 0f;
		Movement.keys = array;
		for (int i = 0; i < Movement.keys.Length; i++)
		{
			Movement.SmoothTangents(i, 0f);
		}
		Movement.preWrapMode = WrapMode.Loop;
		Movement.postWrapMode = WrapMode.Loop;
		BudMovement = new AnimationCurve();
		Keyframe[] array2 = new Keyframe[7];
		array2[0].time = 0f;
		array2[0].value = 0f - HitAngle;
		array2[1].time = 0.25f;
		array2[1].value = 0f - HitAngle;
		array2[2].time = 0.75f;
		array2[2].value = 0f;
		array2[3].time = 1f;
		array2[3].value = HitAngle;
		array2[4].time = 1.25f;
		array2[4].value = HitAngle;
		array2[5].time = 1.75f;
		array2[5].value = 0f;
		array2[6].time = 2f;
		array2[6].value = 0f - HitAngle;
		BudMovement.keys = array2;
		for (int j = 0; j < BudMovement.keys.Length; j++)
		{
			BudMovement.SmoothTangents(j, 0f);
		}
		BudMovement.preWrapMode = WrapMode.Loop;
		BudMovement.postWrapMode = WrapMode.Loop;
		Volume = new AnimationCurve();
		Keyframe[] array3 = new Keyframe[5];
		array3[0].time = 0f;
		array3[0].value = 0.75f;
		array3[1].time = 0.5f;
		array3[1].value = 0f;
		array3[2].time = 1f;
		array3[2].value = 0.75f;
		array3[3].time = 1.5f;
		array3[3].value = 0f;
		array3[4].time = 2f;
		array3[4].value = 0.75f;
		Volume.keys = array3;
		for (int k = 0; k < Volume.keys.Length; k++)
		{
			Volume.SmoothTangents(k, 0f);
		}
		Volume.preWrapMode = WrapMode.Loop;
		Volume.postWrapMode = WrapMode.Loop;
		SetupSpline();
	}

	private void SetupSpline()
	{
		Spline = new CRSpline();
		List<Vector3> knots = Spline.knots;
		knots.Add(TopPivot.position);
		knots.Add(TopPivot.position);
		Center = BudSplineBalance.position;
		knots.Add(Center);
		knots.Add(BudObj.position);
		knots.Add(BudObj.position);
		ThisLength = Vector3.Distance(TopPivot.position, Center) + Vector3.Distance(Center, BudObj.position);
	}

	private void FixedUpdate()
	{
		RotFactor = (IsSwinging ? 1f : Mathf.MoveTowards(RotFactor, 0f, Time.fixedDeltaTime * 0.25f));
		if (RotFactor > 0f)
		{
			CurveTime += Time.fixedDeltaTime * 0.75f * HittutaSpeed;
			TopPivot.localRotation = Quaternion.Euler(Movement.Evaluate(CurveTime) * RotFactor, 0f, 0f);
			SwingWindAudio.volume = Volume.Evaluate(CurveTime) * RotFactor;
			BudPoint.localRotation = Quaternion.Euler(BudMovement.Evaluate(CurveTime) * RotFactor, 0f, 0f);
			BudObj.localRotation = BudPoint.localRotation;
		}
		else
		{
			CurveTime = 0f;
			SwingWindAudio.volume = 0f;
		}
		UpdateMesh();
		Spline.knots[2] = BudSplineBalance.position;
		Spline.knots[Spline.knots.Count - 1] = BudObj.position;
		Spline.knots[Spline.knots.Count - 2] = BudObj.position;
	}

	private void Update()
	{
		GlowInt = ((IsSwinging && TopPivot.localEulerAngles.x > JumpCheckAngle && TopPivot.localEulerAngles.x < HitAngle) ? 6f : Mathf.Lerp(GlowInt, 1f, Time.deltaTime * 10f));
		if (TopPivot.localEulerAngles.x > JumpCheckAngle && TopPivot.localEulerAngles.x < HitAngle)
		{
			if (!PlayPeakFX)
			{
				PeakFX.Play();
				PlayPeakFX = true;
			}
		}
		else
		{
			PlayPeakFX = false;
		}
		CanJump = TopPivot.localEulerAngles.x > JumpCheckAngle && TopPivot.localEulerAngles.x < HitAngle;
		BudLight.intensity = GlowInt;
		BudRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Intensity", GlowInt);
		BudRenderer.SetPropertyBlock(PropBlock);
		if (!IsSwinging && RotFactor == 0f && Collider.gameObject.tag == "Untagged")
		{
			Collider.gameObject.tag = "HomingTarget";
		}
		if (IsSwinging && (!PM || ((bool)PM && PM.Base.GetState() != "Tarzan")))
		{
			IsSwinging = false;
		}
		if ((bool)PM && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && PM.Base.GetState() == "Tarzan" && TarzanState == 0 && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.transform.SetParent(null);
			PM.Base.Camera.UncancelableEvent = false;
			PM.Base.DestroyCameraParams(CameraParams);
			IsSwinging = false;
			if (CanJump)
			{
				StartTime = Time.time;
				LaunchVelocity = Direction() * JumpSpeed;
				StartLaunchVelocity = LaunchVelocity.normalized;
				MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity) * Quaternion.Euler(30f, 0f, 0f);
				PM.transform.forward = LaunchVelocity.MakePlanar();
				Falling = false;
				Audio.PlayOneShot(Clips[0], Audio.volume * 2f);
				Audio.PlayOneShot(Clips[1], Audio.volume);
				Object.Instantiate(SuccessJumpFX, PM.transform.position + PM.transform.up * 0.25f, PM.transform.rotation).transform.SetParent(PM.transform);
				TarzanState = 1;
			}
			else
			{
				PM.Base.SetMachineState("StateJump");
				PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
			}
		}
	}

	private void UpdateMesh()
	{
		if (TubeRenderer == null)
		{
			return;
		}
		if (ThisLength == 0f)
		{
			Vector3 vector = PlayerPoint.position + Vector3.up * Bottom;
			ThisLength = Vector3.Distance(TopPivot.position, vector) + Vector3.Distance(vector, BudObj.position);
		}
		int num = (int)ThisLength * 2;
		float r = 0.0525f;
		TubeRenderer.crossSegments = 12;
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
			if (i == 0)
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

	private Vector3 Direction()
	{
		Vector3 vector = ((!(TargetObj == Vector3.zero)) ? (TargetObj - PM.transform.position).normalized : (-base.transform.forward));
		return new Vector3(vector.x, (!FixedTarget) ? (0f - base.transform.forward.y + YOffset) : vector.y, vector.z).normalized;
	}

	private void StateTarzanStart()
	{
		PM.Base.SetState("Tarzan");
		PM.Base.SetCameraParams(CameraParams);
		PM.Base.Camera.UncancelableEvent = true;
		TarzanState = 0;
		PM.transform.SetParent(PlayerPoint);
		PM.transform.position = PlayerPoint.position;
		PM.transform.forward = -base.transform.forward;
		TarzanAnim = 0;
		PM.Base.PlayAnimation("Tarzan Back To Front", "On Tarzan");
	}

	private void StateTarzan()
	{
		PM.Base.SetState("Tarzan");
		if (TarzanState == 0)
		{
			if (TarzanAnim == 0 && TopPivot.localEulerAngles.x > JumpCheckAngle && TopPivot.localEulerAngles.x < HitAngle)
			{
				TarzanAnim = 1;
				PM.Base.Animator.SetInteger("Tarzan Animation", TarzanAnim);
				PM.Base.PlayAnimation("Tarzan Front To Back", "On Tarzan");
			}
			else if (TarzanAnim == 1 && TopPivot.localEulerAngles.x < 0f - JumpCheckAngle + 360f && TopPivot.localEulerAngles.x > 0f - HitAngle + 360f)
			{
				TarzanAnim = 0;
				PM.Base.Animator.SetInteger("Tarzan Animation", TarzanAnim);
				PM.Base.PlayAnimation("Tarzan Back To Front", "On Tarzan");
			}
			PM.Base.LockControls = true;
			PM.Base.CurSpeed = 0f;
			PM.RBody.isKinematic = true;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.5f, 0f);
			PM.transform.position = PlayerPoint.position;
			PM.transform.forward = -base.transform.forward;
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
		if ((TargetObj == Vector3.zero && Time.time - StartTime < 0.5f) || TargetObj != Vector3.zero)
		{
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.transform.forward = LaunchVelocity.MakePlanar();
			PM.Base.LockControls = true;
			if (TargetObj != Vector3.zero && !FixedTarget)
			{
				LaunchVelocity.y -= 9.81f * Time.fixedDeltaTime;
			}
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
		if (PM.Base.IsGrounded() && Time.time - StartTime > 0.1f)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.Base.DoLandAnim();
			PM.PlayerEvents.CreateLandFXAndSound();
		}
		if (PM.Base.FrontalCollision)
		{
			PM.Base.SetMachineState("StateAir");
		}
	}

	private void StateTarzanEnd()
	{
		PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
		if (PM.Base.Camera.CameraState == PlayerCamera.State.Event && PM.Base.Camera.parameters.Mode == 100)
		{
			PM.Base.DestroyCameraParams(CameraParams);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && (player.GetPrefab("princess") || player.GetPrefab("silver") || player.GetPrefab("rouge")) && (IsSwinging || RotFactor == 0f) && !IsSwinging)
		{
			Collider.gameObject.tag = "Untagged";
			IsSwinging = true;
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateTarzan);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetObj != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, TargetObj);
			Gizmos.DrawWireSphere(TargetObj, 1f);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Top, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Bottom, 0.5f);
	}
}
