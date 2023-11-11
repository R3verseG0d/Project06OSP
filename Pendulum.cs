using UnityEngine;

public class Pendulum : PsiObject
{
	[Header("Framework")]
	public float Length;

	[Header("Prefab")]
	public Transform Root;

	public Transform Pivot;

	public ParticleSystem FX;

	public GameObject Chain;

	[Header("Pendulum")]
	public Renderer[] Renderers;

	public Rigidbody RigidBody;

	public AudioSource Audio;

	private GameObject LastChainObj;

	private float Offset;

	private bool Rotate;

	private bool Damaged;

	private bool Setup;

	private Transform PlayerTransform;

	internal bool IsPsychokinesis;

	private bool IsUpheave;

	public void SetParameters(float _Length)
	{
		Length = _Length;
	}

	private void Start()
	{
		if (!Setup)
		{
			int num = Mathf.RoundToInt(Length * 4.55f);
			Pivot.position = Root.position + Root.up * Length;
			for (int i = 0; i < num; i++)
			{
				Rotate = !Rotate;
				GameObject gameObject = Object.Instantiate(Chain, Pivot.position - Pivot.up * Offset, Pivot.rotation * Quaternion.Euler(0f, Rotate ? 90f : 0f, 0f));
				gameObject.transform.SetParent((!LastChainObj) ? Pivot : LastChainObj.transform);
				if ((bool)LastChainObj)
				{
					gameObject.GetComponent<HingeJoint>().connectedBody = LastChainObj.GetComponent<Rigidbody>();
				}
				gameObject.GetComponent<ReturnTriggerMessage>().ReturnTo = base.transform;
				LastChainObj = gameObject;
				Offset += 0.215f;
				if (i == num - 1)
				{
					base.transform.GetComponent<HingeJoint>().connectedBody = LastChainObj.GetComponent<Rigidbody>();
					base.transform.SetParent(LastChainObj.transform);
				}
			}
			Setup = true;
		}
		IsPsychokinesis = false;
		RigidBody.useGravity = true;
		RigidBody.constraints = RigidbodyConstraints.None;
	}

	private void Update()
	{
		for (int i = 0; i < Renderers.Length; i++)
		{
			OnPsiFX(Renderers[i], IsPsychokinesis || IsUpheave);
		}
		if (IsUpheave && !PlayerTransform)
		{
			OnUpheaveRelease();
		}
	}

	private void ReleasePendulum()
	{
		if (!Damaged)
		{
			FX.Play();
			Object.Destroy(GetComponent<HingeJoint>());
			GameObject gameObject = GameObject.Find("Physics");
			base.transform.SetParent(gameObject.transform);
			gameObject.SendMessage("AddObject", SendMessageOptions.DontRequireReceiver);
			Damaged = true;
		}
	}

	private float ForceMag(bool IsPsycho = false)
	{
		if (IsPsycho)
		{
			if (!IsPsychokinesis)
			{
				return 0.1f;
			}
			if (!Damaged)
			{
				return 0.45f;
			}
			return 0.325f;
		}
		if (!IsPsychokinesis)
		{
			return 0.05f;
		}
		if (!Damaged)
		{
			return 0.225f;
		}
		return 0.1625f;
	}

	private void OnHit(HitInfo HitInfo)
	{
		Vector3 normalized = Vector3.Slerp(HitInfo.force.normalized, (RigidBody.worldCenterOfMass - HitInfo.player.position).normalized, 0.75f).normalized;
		RigidBody.AddForce((HitInfo.force.normalized + normalized).normalized * HitInfo.force.magnitude * ForceMag(), ForceMode.VelocityChange);
	}

	private void StunEnemy(string Name)
	{
		ReleasePendulum();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(RigidBody.velocity.magnitude < 2f))
		{
			collision.transform.SendMessage("OnHit", new HitInfo(base.transform, RigidBody.velocity.normalized * RigidBody.velocity.magnitude, 10), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnUpheave(HitInfo HitInfo)
	{
		IsUpheave = true;
		PlayerTransform = HitInfo.player;
		RigidBody.useGravity = false;
		if ((!Damaged && Vector3.Distance(base.transform.position, Pivot.position) > Length) || Damaged)
		{
			HitInfo.force.x = 0f;
			HitInfo.force.z = 0f;
		}
		RigidBody.velocity = HitInfo.force;
		RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	private void OnUpheaveRelease()
	{
		IsUpheave = false;
		RigidBody.useGravity = true;
		RigidBody.constraints = RigidbodyConstraints.None;
		RigidBody.velocity = Vector3.zero;
	}

	private void OnPsychoThrow(HitInfo HitInfo)
	{
		RigidBody.AddForce(HitInfo.force * ForceMag(IsPsycho: true), ForceMode.VelocityChange);
		if (!Damaged)
		{
			Audio.Play();
		}
		IsPsychokinesis = false;
	}

	private void OnPsychokinesis(Transform _PlayerPos)
	{
		IsPsychokinesis = true;
	}

	private void OnReleasePsycho()
	{
		IsPsychokinesis = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (Length != 0f)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Root.position, Root.position + Root.up * Length);
			Gizmos.DrawWireSphere(Root.position + Root.up * Length, 1f);
		}
	}
}
