using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Plane Collider")]
public class DynamicBonePlaneCollider : DynamicBoneColliderBase
{
	private Plane m_Plane;

	private void OnValidate()
	{
	}

	public override void Prepare()
	{
		Vector3 inNormal = Vector3.up;
		switch (m_Direction)
		{
		case Direction.X:
			inNormal = base.transform.right;
			break;
		case Direction.Y:
			inNormal = base.transform.up;
			break;
		case Direction.Z:
			inNormal = base.transform.forward;
			break;
		}
		Vector3 inPoint = base.transform.TransformPoint(m_Center);
		m_Plane.SetNormalAndPosition(inNormal, inPoint);
	}

	public override bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		float distanceToPoint = m_Plane.GetDistanceToPoint(particlePosition);
		if (m_Bound == Bound.Outside)
		{
			if (distanceToPoint < 0f)
			{
				particlePosition -= m_Plane.normal * distanceToPoint;
				return true;
			}
		}
		else if (distanceToPoint > 0f)
		{
			particlePosition -= m_Plane.normal * distanceToPoint;
			return true;
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Prepare();
			if (m_Bound == Bound.Outside)
			{
				Gizmos.color = Color.yellow;
			}
			else
			{
				Gizmos.color = Color.magenta;
			}
			Vector3 vector = base.transform.TransformPoint(m_Center);
			Gizmos.DrawLine(vector, vector + m_Plane.normal);
		}
	}
}
