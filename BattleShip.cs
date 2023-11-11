using UnityEngine;

public class BattleShip : MonoBehaviour
{
	[Header("Framework")]
	public string Path;

	public float Distance;

	[Header("Framework")]
	public bool StartInProgress;

	public float CustomProgress;

	private BezierCurve Curve;

	private PlayerBase Player;

	private bool Move;

	private float Progress;

	public void SetParameters(string _Path, float _Distance)
	{
		Path = _Path;
		Distance = _Distance;
	}

	private void Start()
	{
		Curve = GameObject.Find(Path).GetComponent<BezierCurve>();
		if (StartInProgress)
		{
			Progress = CustomProgress;
		}
	}

	private void Update()
	{
		if (Move)
		{
			float num = ((Vector3.Distance(base.transform.position, Player.transform.position) > Distance) ? 1f : (-1f));
			float num2 = Mathf.Lerp(Mathf.Max(Distance / 2.75f, Player.CurSpeed), Distance / 3f, Vector3.Distance(base.transform.position, Player.transform.position) * num / Distance);
			Progress += num2 / Curve.Length() * Time.deltaTime;
			if (Progress > 1f)
			{
				Progress = 1f;
			}
			base.transform.position = Curve.GetPosition(Progress);
			base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
		}
	}

	private void Shoot(GameObject Target)
	{
		if (!Move)
		{
			Player = Target.GetComponent<PlayerBase>();
			Move = true;
		}
	}
}
