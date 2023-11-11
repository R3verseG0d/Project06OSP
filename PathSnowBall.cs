using UnityEngine;

public class PathSnowBall : MonoBehaviour
{
	[Header("Framework")]
	public string Path;

	public float Speed;

	public float Distance;

	[Header("Optional")]
	public bool PlayerLookAt;

	[Header("Prefab")]
	public Vector3 Camera;

	public LayerMask DestroyMask;

	public GameObject Model;

	public GameObject FX;

	public GameObject AvalancheFX;

	public AudioSource Audio;

	public Transform Point;

	public Transform PointPivot;

	public Transform PlayerPoint;

	private PlayerManager PM;

	private BezierCurve Curve;

	private GameObject Target;

	private bool Triggered;

	private bool TrappedPlayer;

	private float Progress;

	public void SetParameters(string _Path, float _Speed, float _Distance)
	{
		Path = _Path;
		Speed = _Speed;
		Distance = _Distance;
	}

	private void StateSnowBallDeathStart()
	{
		PM.Base.SetState("SnowBallDeath");
		PM.Base.PlayerVoice.PlayRandom(0);
		PM.transform.SetParent(PlayerPoint);
		PM.RBody.isKinematic = true;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		StartCoroutine(PM.Base.RestartStage());
	}

	private void StateSnowBallDeath()
	{
		PM.Base.SetState("SnowBallDeath");
		PM.Base.PlayAnimation("Death Fall", "On Death Air");
		PM.Base.LockControls = true;
		PM.Base.CurSpeed = 0f;
		PM.transform.position = PlayerPoint.position;
		PM.transform.rotation = PlayerPoint.rotation;
		PM.Base._Rigidbody.velocity = Vector3.zero;
		PM.Base.GeneralMeshRotation = PM.transform.rotation;
	}

	private void StateSnowBallDeathEnd()
	{
	}

	private void Start()
	{
		Curve = GameObject.Find(Path).GetComponent<BezierCurve>();
	}

	private void Update()
	{
		if (!Triggered)
		{
			return;
		}
		Progress += Speed / Curve.Length() * Time.deltaTime;
		if (Progress > 1f)
		{
			if (FX.activeSelf)
			{
				FX.SetActive(value: false);
			}
			if (AvalancheFX.activeSelf)
			{
				AvalancheFX.SetActive(value: false);
			}
			if (Audio.isPlaying)
			{
				Audio.Stop();
			}
			Progress = 1f;
			return;
		}
		float num = Mathf.Lerp(15f, 5f, Progress);
		float num2 = Mathf.Lerp(0.75f, 3.5f, Progress);
		Mathf.Lerp(1f, 2.5f, Progress);
		DestroySphere_Dir(Model.transform.position, Model.transform.localScale.x * 4.25f, 10f, 1);
		base.transform.position = Curve.GetPosition(Progress);
		base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress).MakePlanar());
		Vector3 localPosition = Model.transform.localPosition;
		localPosition.y = Mathf.Lerp(3.125f, 14.7f, Progress);
		Model.transform.localPosition = localPosition;
		Model.transform.Rotate(Speed * Time.deltaTime * num, 0f, 0f);
		Model.transform.localScale = new Vector3(num2, num2, num2);
		Point.transform.localPosition = localPosition;
		if (TrappedPlayer)
		{
			Point.transform.Rotate(Speed * Time.deltaTime * num, 0f, 0f);
		}
		Vector3 localPosition2 = PlayerPoint.localPosition;
		localPosition2.y = Mathf.Lerp(3.075f, 14.35f, Progress);
		PlayerPoint.localPosition = localPosition2;
		if (!(Progress > 0.04f))
		{
			return;
		}
		if (!AvalancheFX.activeSelf)
		{
			AvalancheFX.SetActive(value: true);
		}
		float num3 = Vector3.Dot(base.transform.forward, (base.transform.position + base.transform.forward * Distance - Target.transform.position).normalized);
		num3 = ((num3 > 0f) ? 1f : (-1f));
		if (!TrappedPlayer && num3 == 1f)
		{
			PlayerBase component = Target.GetComponent<PlayerBase>();
			if ((bool)component && !component.IsDead)
			{
				PM = Target.GetComponent<PlayerManager>();
				PointPivot.LookAt(component.transform);
				component.OnDeathEnter(100);
				component.StateMachine.ChangeState(base.gameObject, StateSnowBallDeath);
			}
			TrappedPlayer = true;
		}
	}

	private void Shoot(GameObject _Target)
	{
		Target = _Target;
		Model.SetActive(value: true);
		Triggered = true;
		if (PlayerLookAt)
		{
			SnowBoard snowBoard = Object.FindObjectOfType<SnowBoard>();
			if ((bool)snowBoard)
			{
				snowBoard.LookBack();
			}
		}
	}

	public bool DestroySphere_Dir(Vector3 Position, float Radius, float Force, int Damage)
	{
		Collider[] array = Physics.OverlapSphere(Position, Radius, DestroyMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
				if (vector == Vector3.zero)
				{
					vector = base.transform.forward.MakePlanar();
				}
				Vector3 force = (vector + Vector3.up * Random.Range(0.1f, 0.25f)).normalized * Force;
				HitInfo value = new HitInfo(base.transform, force, Damage);
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + Camera);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(Model.transform.position, Model.transform.localScale.x * 4.25f);
		Gizmos.DrawWireSphere(base.transform.position + Camera, 1f);
	}
}
