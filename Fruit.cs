using UnityEngine;

public class Fruit : PsiObject
{
	[Header("Framework")]
	public Vector3 Turtle;

	[Header("Framework Settings")]
	public Turtle TurtleSettings;

	[Header("Optional")]
	public GameObject EnableObj;

	[Header("Prefab")]
	public Renderer Renderer;

	public GameObject FruitObj;

	public Collider SphereCollider;

	public Collider SolidCollider;

	public Transform HomingTarget;

	public Rigidbody Rigidbody;

	private bool Triggered;

	private bool IsPsychokinesis;

	private float DropTime;

	public void SetParameters(Vector3 _Turtle)
	{
		Turtle = _Turtle;
	}

	private void Start()
	{
		IsPsychokinesis = false;
	}

	private void Update()
	{
		OnPsiFX(Renderer, IsPsychokinesis && Time.time - DropTime < 0.5f);
	}

	private void OnHit(HitInfo HitInfo)
	{
		Drop();
	}

	private void OnFlash()
	{
		Drop();
	}

	private void OnPsychokinesis(Transform DummyTransform)
	{
		Drop(Psi: true);
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !(player.GetState() != "Glide"))
		{
			Drop();
		}
	}

	private void Drop(bool Psi = false)
	{
		if (!Triggered)
		{
			HomingTarget.tag = "Untagged";
			SphereCollider.enabled = false;
			SolidCollider.enabled = false;
			Rigidbody.useGravity = true;
			Rigidbody.isKinematic = false;
			if ((bool)TurtleSettings)
			{
				TurtleSettings.OnEmerge();
			}
			if ((bool)EnableObj)
			{
				EnableObj.SetActive(value: true);
			}
			DropTime = Time.time;
			Invoke("DisableFruit", 2f);
			IsPsychokinesis = Psi;
			Triggered = true;
		}
	}

	private void DisableFruit()
	{
		FruitObj.SetActive(value: false);
	}

	private void OnDrawGizmosSelected()
	{
		if (Turtle != Vector3.zero)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, Turtle);
			Gizmos.DrawWireSphere(Turtle, 1f);
		}
	}
}
