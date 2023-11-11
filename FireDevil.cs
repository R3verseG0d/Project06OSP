using STHEngine;
using UnityEngine;

public class FireDevil : ObjectBase
{
	[Header("Framework")]
	public string PathName;

	public float Speed;

	public float Radius;

	[Header("Prefab")]
	public Rigidbody RBody;

	public GameObject Mesh;

	public GameObject ExplosionFX;

	private BezierCurve Curve;

	private GameObject Target;

	private bool Destroyed;

	private bool Attack;

	private float Progress;

	private float StartTime;

	private float AttackSpd;

	private void Start()
	{
		if (PathName != "")
		{
			Curve = GameObject.Find(PathName).GetComponent<BezierCurve>();
		}
	}

	private void Update()
	{
		if (Destroyed)
		{
			return;
		}
		if (Radius != 0f)
		{
			if (!Attack)
			{
				Target = FindClosestTarget();
				if ((bool)Target)
				{
					StartTime = Time.time;
					AttackSpd = 0f;
					base.transform.forward = (Target.transform.position - base.transform.position).normalized;
					Attack = true;
				}
			}
			else
			{
				float num = Time.time - StartTime;
				num *= num;
				AttackSpd = Mathf.Lerp(AttackSpd, 50f, Mathf.Clamp01(num) * Time.deltaTime * 5f);
				RBody.velocity = base.transform.forward * AttackSpd;
			}
		}
		if ((bool)Curve && !Attack)
		{
			RBody.velocity = Vector3.zero;
			Progress += Speed / Curve.Length() * Time.deltaTime;
			if (Progress > 1f)
			{
				Progress = 0f;
			}
			base.transform.position = Curve.GetPosition(Progress);
		}
	}

	private void Explode()
	{
		Object.Destroy(RBody);
		Mesh.SetActive(value: false);
		ExplosionFX.SetActive(value: true);
		Object.Destroy(base.gameObject, 2f);
		Destroyed = true;
	}

	private GameObject FindClosestTarget()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
		GameObject result = null;
		float num = Radius;
		Vector3 position = base.transform.position;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float sqrMagnitude = (gameObject.transform.position - position).sqrMagnitude;
			if (sqrMagnitude < num && !Physics.Linecast(base.transform.position, gameObject.transform.position, ExtensionMethods.HomingBlock_Mask) && gameObject.GetComponent<PlayerBase>().IsVisible)
			{
				result = gameObject;
				num = sqrMagnitude;
			}
		}
		return result;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (Attack && !Destroyed)
		{
			Explode();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Radius != 0f)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, Radius);
		}
	}
}
