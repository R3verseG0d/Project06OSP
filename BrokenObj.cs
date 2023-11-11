using UnityEngine;

public class BrokenObj : MonoBehaviour
{
	public float ForceMultiplier = 1f;

	public Rigidbody[] m_Pieces;

	public Transform m_PieceParent;

	public bool Explosive;

	public LayerMask LayerMask;

	[Header("Optional")]
	public bool IgnoreColOnStart;

	public float IgnoreRadius;

	public LayerMask IgnoreLayers;

	private void Start()
	{
		Object.Destroy(base.gameObject, 10f);
		if (!IgnoreColOnStart)
		{
			return;
		}
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, IgnoreRadius, IgnoreLayers);
		foreach (Collider collider in array)
		{
			Collider[] array2 = componentsInChildren;
			foreach (Collider collider2 in array2)
			{
				Physics.IgnoreCollision(collider, collider2);
			}
		}
	}

	public Rigidbody[] AffectedPieces()
	{
		if (m_Pieces == null || m_Pieces.Length == 0)
		{
			return m_PieceParent.GetComponentsInChildren<Rigidbody>();
		}
		return m_Pieces;
	}

	public void OnCreate(HitInfo HitInfo)
	{
		if (Explosive)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 5f, LayerMask);
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].GetComponent<Rigidbody>())
				{
					if ((bool)array[i].transform.parent.GetComponent<Rigidbody>())
					{
						array[i].transform.parent.GetComponent<Rigidbody>().AddExplosionForce(50f * ForceMultiplier, HitInfo.force, 20f, 0f, ForceMode.VelocityChange);
					}
				}
				else
				{
					array[i].GetComponent<Rigidbody>().AddExplosionForce(50f * ForceMultiplier, HitInfo.force, 20f, 0f, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			Rigidbody[] array2 = AffectedPieces();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].maxAngularVelocity = 90f;
				Vector3 normalized = Vector3.Slerp(HitInfo.force.normalized, (array2[j].worldCenterOfMass - HitInfo.player.position).normalized, 0.75f).normalized;
				array2[j].AddForce((HitInfo.force.normalized + normalized).normalized * HitInfo.force.magnitude * ForceMultiplier, ForceMode.VelocityChange);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (IgnoreColOnStart)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, IgnoreRadius);
		}
	}
}
