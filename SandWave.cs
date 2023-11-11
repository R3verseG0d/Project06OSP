using System.Collections.Generic;
using UnityEngine;

public class SandWave : MonoBehaviour
{
	[Header("Framework")]
	public float Speed;

	[Header("Prefab")]
	public Vector3 StartPos;

	public Vector3 EndPos;

	public Collider Collider;

	public GameObject Wave3M;

	public GameObject Wave5M;

	private PlayerBase PlayerBase;

	private List<Transform> Buildings = new List<Transform>();

	private Vector3 AddedPos;

	public void SetParameters(float _Speed)
	{
		Speed = _Speed;
	}

	private void Start()
	{
		for (int i = 0; i < 3; i++)
		{
			GameObject gameObject = Object.Instantiate((Random.value > 0.25f) ? Wave3M : Wave5M, base.transform.position, base.transform.rotation);
			gameObject.transform.position = Vector3.MoveTowards(StartPos, EndPos, Vector3.Distance(StartPos, EndPos) * (float)i / 3f);
			Buildings.Add(gameObject.transform);
		}
	}

	private void Update()
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i].position != EndPos)
			{
				bool flag = Vector3.Distance(EndPos, Buildings[i].position) < 17.5f;
				Buildings[i].localScale = new Vector3(1f, Mathf.Lerp(Buildings[i].localScale.y, flag ? 0f : 1f, Time.deltaTime * (flag ? 1.75f : 1f)), 1f);
				Buildings[i].position = Vector3.MoveTowards(Buildings[i].position, EndPos, Speed * Time.deltaTime);
			}
			else
			{
				Buildings[i].localScale = new Vector3(1f, 0f, 1f);
				Buildings[i].position = StartPos;
			}
		}
	}

	private void FixedUpdate()
	{
		if ((bool)PlayerBase && PlayerBase.IsGrounded() && PlayerBase.RaycastHit.collider == Collider)
		{
			PlayerBase.transform.position += -base.transform.forward * Speed * 0.5f * Time.fixedDeltaTime;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Player" && !PlayerBase)
		{
			PlayerBase = collision.gameObject.GetComponent<PlayerBase>();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			PlayerBase = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (StartPos != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(StartPos, 1f);
			if (EndPos != Vector3.zero)
			{
				Gizmos.DrawLine(StartPos, EndPos);
			}
		}
		if (EndPos != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(EndPos, 1f);
			if (StartPos != Vector3.zero)
			{
				Gizmos.DrawLine(EndPos, StartPos);
			}
		}
	}
}
