using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider")]
public class DynamicBoneCollider : DynamicBoneColliderBase
{
	[Tooltip("The radius of the sphere or capsule.")]
	public float m_Radius = 0.5f;

	[Tooltip("The height of the capsule.")]
	public float m_Height;

	[Tooltip("The other radius of the capsule.")]
	public float m_Radius2;

	private float m_ScaledRadius;

	private float m_ScaledRadius2;

	private Vector3 m_C0;

	private Vector3 m_C1;

	private float m_C01Distance;

	private int m_CollideType;

	private void OnValidate()
	{
		m_Radius = Mathf.Max(m_Radius, 0f);
		m_Height = Mathf.Max(m_Height, 0f);
		m_Radius2 = Mathf.Max(m_Radius2, 0f);
	}

	public override void Prepare()
	{
		float num = Mathf.Abs(base.transform.lossyScale.x);
		float num2 = m_Height * 0.5f;
		if (m_Radius2 <= 0f || Mathf.Abs(m_Radius - m_Radius2) < 0.01f)
		{
			m_ScaledRadius = m_Radius * num;
			float num3 = num2 - m_Radius;
			if (num3 <= 0f)
			{
				m_C0 = base.transform.TransformPoint(m_Center);
				if (m_Bound == Bound.Outside)
				{
					m_CollideType = 0;
				}
				else
				{
					m_CollideType = 1;
				}
				return;
			}
			Vector3 center = m_Center;
			Vector3 center2 = m_Center;
			switch (m_Direction)
			{
			case Direction.X:
				center.x += num3;
				center2.x -= num3;
				break;
			case Direction.Y:
				center.y += num3;
				center2.y -= num3;
				break;
			case Direction.Z:
				center.z += num3;
				center2.z -= num3;
				break;
			}
			m_C0 = base.transform.TransformPoint(center);
			m_C1 = base.transform.TransformPoint(center2);
			m_C01Distance = (m_C1 - m_C0).magnitude;
			if (m_Bound == Bound.Outside)
			{
				m_CollideType = 2;
			}
			else
			{
				m_CollideType = 3;
			}
			return;
		}
		float num4 = Mathf.Max(m_Radius, m_Radius2);
		if (num2 - num4 <= 0f)
		{
			m_ScaledRadius = num4 * num;
			m_C0 = base.transform.TransformPoint(m_Center);
			if (m_Bound == Bound.Outside)
			{
				m_CollideType = 0;
			}
			else
			{
				m_CollideType = 1;
			}
			return;
		}
		m_ScaledRadius = m_Radius * num;
		m_ScaledRadius2 = m_Radius2 * num;
		float num5 = num2 - m_Radius;
		float num6 = num2 - m_Radius2;
		Vector3 center3 = m_Center;
		Vector3 center4 = m_Center;
		switch (m_Direction)
		{
		case Direction.X:
			center3.x += num5;
			center4.x -= num6;
			break;
		case Direction.Y:
			center3.y += num5;
			center4.y -= num6;
			break;
		case Direction.Z:
			center3.z += num5;
			center4.z -= num6;
			break;
		}
		m_C0 = base.transform.TransformPoint(center3);
		m_C1 = base.transform.TransformPoint(center4);
		m_C01Distance = (m_C1 - m_C0).magnitude;
		if (m_Bound == Bound.Outside)
		{
			m_CollideType = 4;
		}
		else
		{
			m_CollideType = 5;
		}
	}

	public override bool Collide(ref Vector3 particlePosition, float particleRadius)
	{
		switch (m_CollideType)
		{
		case 0:
			return OutsideSphere(ref particlePosition, particleRadius, m_C0, m_ScaledRadius);
		case 1:
			return InsideSphere(ref particlePosition, particleRadius, m_C0, m_ScaledRadius);
		case 2:
			return OutsideCapsule(ref particlePosition, particleRadius, m_C0, m_C1, m_ScaledRadius, m_C01Distance);
		case 3:
			return InsideCapsule(ref particlePosition, particleRadius, m_C0, m_C1, m_ScaledRadius, m_C01Distance);
		case 4:
			return OutsideCapsule2(ref particlePosition, particleRadius, m_C0, m_C1, m_ScaledRadius, m_ScaledRadius2, m_C01Distance);
		case 5:
			return InsideCapsule2(ref particlePosition, particleRadius, m_C0, m_C1, m_ScaledRadius, m_ScaledRadius2, m_C01Distance);
		default:
			return false;
		}
	}

	private static bool OutsideSphere(ref Vector3 particlePosition, float particleRadius, Vector3 sphereCenter, float sphereRadius)
	{
		float num = sphereRadius + particleRadius;
		float num2 = num * num;
		Vector3 vector = particlePosition - sphereCenter;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude > 0f && sqrMagnitude < num2)
		{
			float num3 = Mathf.Sqrt(sqrMagnitude);
			particlePosition = sphereCenter + vector * (num / num3);
			return true;
		}
		return false;
	}

	private static bool InsideSphere(ref Vector3 particlePosition, float particleRadius, Vector3 sphereCenter, float sphereRadius)
	{
		float num = sphereRadius - particleRadius;
		float num2 = num * num;
		Vector3 vector = particlePosition - sphereCenter;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude > num2)
		{
			float num3 = Mathf.Sqrt(sqrMagnitude);
			particlePosition = sphereCenter + vector * (num / num3);
			return true;
		}
		return false;
	}

	private static bool OutsideCapsule(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius, float dirlen)
	{
		float num = capsuleRadius + particleRadius;
		float num2 = num * num;
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num3 = Vector3.Dot(vector2, vector);
		if (num3 <= 0f)
		{
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > 0f && sqrMagnitude < num2)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num / num4);
				return true;
			}
		}
		else
		{
			float num5 = dirlen * dirlen;
			if (num3 >= num5)
			{
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude2 = vector2.sqrMagnitude;
				if (sqrMagnitude2 > 0f && sqrMagnitude2 < num2)
				{
					float num6 = Mathf.Sqrt(sqrMagnitude2);
					particlePosition = capsuleP1 + vector2 * (num / num6);
					return true;
				}
			}
			else
			{
				Vector3 vector3 = vector2 - vector * (num3 / num5);
				float sqrMagnitude3 = vector3.sqrMagnitude;
				if (sqrMagnitude3 > 0f && sqrMagnitude3 < num2)
				{
					float num7 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition += vector3 * ((num - num7) / num7);
					return true;
				}
			}
		}
		return false;
	}

	private static bool InsideCapsule(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius, float dirlen)
	{
		float num = capsuleRadius - particleRadius;
		float num2 = num * num;
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num3 = Vector3.Dot(vector2, vector);
		if (num3 <= 0f)
		{
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > num2)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num / num4);
				return true;
			}
		}
		else
		{
			float num5 = dirlen * dirlen;
			if (num3 >= num5)
			{
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude2 = vector2.sqrMagnitude;
				if (sqrMagnitude2 > num2)
				{
					float num6 = Mathf.Sqrt(sqrMagnitude2);
					particlePosition = capsuleP1 + vector2 * (num / num6);
					return true;
				}
			}
			else
			{
				Vector3 vector3 = vector2 - vector * (num3 / num5);
				float sqrMagnitude3 = vector3.sqrMagnitude;
				if (sqrMagnitude3 > num2)
				{
					float num7 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition += vector3 * ((num - num7) / num7);
					return true;
				}
			}
		}
		return false;
	}

	private static bool OutsideCapsule2(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius0, float capsuleRadius1, float dirlen)
	{
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num = Vector3.Dot(vector2, vector);
		if (num <= 0f)
		{
			float num2 = capsuleRadius0 + particleRadius;
			float num3 = num2 * num2;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > 0f && sqrMagnitude < num3)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num2 / num4);
				return true;
			}
		}
		else
		{
			float num5 = dirlen * dirlen;
			if (num >= num5)
			{
				float num6 = capsuleRadius1 + particleRadius;
				float num7 = num6 * num6;
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude2 = vector2.sqrMagnitude;
				if (sqrMagnitude2 > 0f && sqrMagnitude2 < num7)
				{
					float num8 = Mathf.Sqrt(sqrMagnitude2);
					particlePosition = capsuleP1 + vector2 * (num6 / num8);
					return true;
				}
			}
			else
			{
				Vector3 vector3 = vector2 - vector * (num / num5);
				float sqrMagnitude3 = vector3.sqrMagnitude;
				float num9 = Vector3.Dot(vector2, vector / dirlen);
				float num10 = Mathf.Lerp(capsuleRadius0, capsuleRadius1, num9 / dirlen) + particleRadius;
				float num11 = num10 * num10;
				if (sqrMagnitude3 > 0f && sqrMagnitude3 < num11)
				{
					float num12 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition += vector3 * ((num10 - num12) / num12);
					return true;
				}
			}
		}
		return false;
	}

	private static bool InsideCapsule2(ref Vector3 particlePosition, float particleRadius, Vector3 capsuleP0, Vector3 capsuleP1, float capsuleRadius0, float capsuleRadius1, float dirlen)
	{
		Vector3 vector = capsuleP1 - capsuleP0;
		Vector3 vector2 = particlePosition - capsuleP0;
		float num = Vector3.Dot(vector2, vector);
		if (num <= 0f)
		{
			float num2 = capsuleRadius0 - particleRadius;
			float num3 = num2 * num2;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > num3)
			{
				float num4 = Mathf.Sqrt(sqrMagnitude);
				particlePosition = capsuleP0 + vector2 * (num2 / num4);
				return true;
			}
		}
		else
		{
			float num5 = dirlen * dirlen;
			if (num >= num5)
			{
				float num6 = capsuleRadius1 - particleRadius;
				float num7 = num6 * num6;
				vector2 = particlePosition - capsuleP1;
				float sqrMagnitude2 = vector2.sqrMagnitude;
				if (sqrMagnitude2 > num7)
				{
					float num8 = Mathf.Sqrt(sqrMagnitude2);
					particlePosition = capsuleP1 + vector2 * (num6 / num8);
					return true;
				}
			}
			else
			{
				Vector3 vector3 = vector2 - vector * (num / num5);
				float sqrMagnitude3 = vector3.sqrMagnitude;
				float num9 = Vector3.Dot(vector2, vector / dirlen);
				float num10 = Mathf.Lerp(capsuleRadius0, capsuleRadius1, num9 / dirlen) - particleRadius;
				float num11 = num10 * num10;
				if (sqrMagnitude3 > num11)
				{
					float num12 = Mathf.Sqrt(sqrMagnitude3);
					particlePosition += vector3 * ((num10 - num12) / num12);
					return true;
				}
			}
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
			switch (m_CollideType)
			{
			case 0:
			case 1:
				Gizmos.DrawWireSphere(m_C0, m_ScaledRadius);
				break;
			case 2:
			case 3:
				DrawCapsule(m_C0, m_C1, m_ScaledRadius, m_ScaledRadius);
				break;
			case 4:
			case 5:
				DrawCapsule(m_C0, m_C1, m_ScaledRadius, m_ScaledRadius2);
				break;
			}
		}
	}

	private static void DrawCapsule(Vector3 c0, Vector3 c1, float radius0, float radius1)
	{
		Gizmos.DrawLine(c0, c1);
		Gizmos.DrawWireSphere(c0, radius0);
		Gizmos.DrawWireSphere(c1, radius1);
	}
}
