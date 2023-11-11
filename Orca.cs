using UnityEngine;

public class Orca : ObjectBase
{
	public int Type;

	public float Speed;

	public string Spline;

	public BezierCurve OrcaSpline;

	public float Progress;

	public Animator Animator;

	public LayerMask AttackMask;

	public Transform HeadBone;

	public Vector3 HeadTop;

	public Vector3 HeadBottom;

	public Vector3 TorsoTop;

	public Vector3 TorsoBottom;

	public Vector3 TailTop;

	public Vector3 TailBottom;

	public GameObject WaterSplashFX;

	public Vector3 TopPoint;

	public Vector3 BottomPoint;

	public Transform[] SpawnPoints;

	public LayerMask WaterMask;

	public Transform PlayerPoint;

	public Transform EventBoxTarget;

	internal bool GateIsClosed;

	private PlayerManager PM;

	private RaycastHit OrcaHit;

	private bool StartProgress;

	private bool OrcaDamaged;

	private bool LaunchPlayer;

	private bool FailedToCloseDoor;

	private float LaunchWaitTimer;

	public void SetParameters(int _Type, float _Speed, string _Spline)
	{
		Type = _Type;
		Speed = _Speed;
		Spline = _Spline;
		if (Type != 1)
		{
			return;
		}
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		Component[] components = PlayerPoint.GetComponents<Component>();
		foreach (Component component in components)
		{
			if (!(component is Transform))
			{
				Object.DestroyImmediate(component);
			}
		}
	}

	private void Start()
	{
		OrcaSpline = GameObject.Find("Stage/Splines/" + Spline).GetComponent<BezierCurve>();
	}

	private void StateOrcaStart()
	{
		PM.Base.SetState("Orca");
		PM.Base.PlayAnimation("Orca_Rodeo", "On Rodeo");
		PM.transform.SetParent(PlayerPoint);
		PM.transform.position = PlayerPoint.position;
		PM.transform.rotation = PlayerPoint.rotation;
		PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.5f, 0f);
	}

	private void StateOrca()
	{
		PM.Base.SetState("Orca");
		PM.Base.LockControls = true;
		PM.Base.CurSpeed = 0f;
		if (!LaunchPlayer)
		{
			PM.Base.PlayAnimation("Orca Rodeo", "On Rodeo");
			PM.RBody.isKinematic = true;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			PM.transform.position = PlayerPoint.position;
			PM.transform.rotation = PlayerPoint.rotation;
			PM.RBody.velocity = Vector3.zero;
			PM.Base.GeneralMeshRotation = PM.transform.rotation;
		}
		else
		{
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
			PM.transform.SetParent(null);
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation) * Quaternion.Euler(90f, 0f, 0f);
			PM.Base.Mesh.transform.localPosition = new Vector3(0f, 0.25f, 0f);
			PM.Base.Mesh.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
			PM.transform.rotation = Quaternion.LookRotation(EventBoxTarget.position - PM.transform.position);
			PM.transform.position = Vector3.MoveTowards(PM.transform.position, EventBoxTarget.position, Time.fixedDeltaTime * 40f);
		}
		if (!GateIsClosed && Progress >= 0.97f)
		{
			PM.transform.SetParent(null);
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.Base.SetMachineState("StateAir");
		}
	}

	private void StateOrcaEnd()
	{
	}

	private void FixedUpdate()
	{
		HeadTop = SpawnPoints[0].position + TopPoint;
		HeadBottom = SpawnPoints[0].position + BottomPoint;
		TorsoTop = SpawnPoints[1].position + TopPoint;
		TorsoBottom = SpawnPoints[1].position + BottomPoint;
		TailTop = SpawnPoints[2].position + TopPoint;
		TailBottom = SpawnPoints[2].position + BottomPoint;
		if (StartProgress)
		{
			Progress += Speed / OrcaSpline.Length() * Time.fixedDeltaTime;
			AttackSphere(HeadBone.position, 4f, base.transform.up * 20f, 1);
		}
		base.transform.position = OrcaSpline.GetPosition(Progress);
		base.transform.rotation = Quaternion.LookRotation(OrcaSpline.GetTangent(Progress));
		if (Progress >= 1f)
		{
			Object.Destroy(base.gameObject);
		}
		if (Type != 3)
		{
			return;
		}
		if (GateIsClosed && Progress >= 0.9425f)
		{
			Speed = 0f;
			if (!OrcaDamaged)
			{
				LaunchWaitTimer = Time.time;
				Animator.SetBool((Type == 3) ? "Orca Rodeo" : "Orca Swim", value: false);
				Animator.SetBool("Orca Damage", value: true);
				OrcaDamaged = true;
			}
			if (Time.time - LaunchWaitTimer > 0.05f && !LaunchPlayer)
			{
				PM.transform.SetParent(null);
				LaunchPlayer = true;
			}
		}
		if (Progress > 0.885f && !FailedToCloseDoor)
		{
			if ((bool)Object.FindObjectOfType<Tails>())
			{
				Object.FindObjectOfType<Tails>().gameObject.SetActive(value: false);
			}
			PM.Base.SetPlayer(PM.Base.PlayerNo, PM.Base.PlayerPrefab.ToString());
			FailedToCloseDoor = true;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!(player == null) && !player.IsDead && (player.GetPrefab("sonic_new") || player.GetPrefab("metal_sonic")))
		{
			OnEventSignal();
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StateOrca);
		}
	}

	public void OnEventSignal()
	{
		StartProgress = true;
		Animator.SetBool((Type == 3) ? "Orca Rodeo" : "Orca Swim", value: true);
		if (Type == 1)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
		}
	}

	private void GateClose()
	{
		Progress = 0.885f;
		GateIsClosed = true;
		if ((bool)Object.FindObjectOfType<Tails>() && Progress < 0.885f)
		{
			Object.FindObjectOfType<Tails>().gameObject.SetActive(value: false);
		}
		PM.Base.SetPlayer(PM.Base.PlayerNo, PM.Base.PlayerPrefab.ToString());
	}

	public bool AttackSphere(Vector3 Position, float Radius, Vector3 Force, int Damage)
	{
		HitInfo value = new HitInfo(base.transform, Force, Damage);
		Collider[] array = Physics.OverlapSphere(Position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public void SpawnWaterSplash(int PosType)
	{
		if (PosType == 0 && Physics.Linecast(HeadTop, HeadBottom, out OrcaHit, WaterMask, QueryTriggerInteraction.Collide))
		{
			Object.Instantiate(WaterSplashFX, OrcaHit.point, Quaternion.identity);
		}
		else if (PosType == 1 && Physics.Linecast(TorsoTop, TorsoBottom, out OrcaHit, WaterMask, QueryTriggerInteraction.Collide))
		{
			Object.Instantiate(WaterSplashFX, OrcaHit.point, Quaternion.identity);
		}
		else if (PosType == 2 && Physics.Linecast(TailTop, TailBottom, out OrcaHit, WaterMask, QueryTriggerInteraction.Collide))
		{
			Object.Instantiate(WaterSplashFX, OrcaHit.point, Quaternion.identity);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(SpawnPoints[0].position + TopPoint, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(SpawnPoints[0].position + BottomPoint, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(SpawnPoints[1].position + TopPoint, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(SpawnPoints[1].position + BottomPoint, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(SpawnPoints[2].position + TopPoint, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(SpawnPoints[2].position + BottomPoint, new Vector3(1f, 1f, 1f));
	}
}
