using UnityEngine;

public static class Math3D
{
	public static Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 pnt)
	{
		Vector3 vector = end - start;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num = Vector3.Dot(pnt - start, vector);
		if (num < 0f)
		{
			return start;
		}
		if (num > magnitude)
		{
			return end;
		}
		num = Mathf.Clamp(num, 0f, magnitude);
		return start + vector * num;
	}

	public static bool LineSphereIntersection(Vector3 linePointA, Vector3 linePointB, Vector3 spherePoint, float sphereRadius)
	{
		return Vector3.Distance(ClosestPointOnLine(linePointA, linePointB, spherePoint), spherePoint) <= sphereRadius;
	}

	public static bool LinePlaneIntersection(Vector3 linePointA, Vector3 linePointB, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		intersection = Vector3.zero;
		Vector3 normalized = (planePoint - linePointB).normalized;
		if (Vector3.Dot(planeNormal, normalized) >= 0f)
		{
			Vector3 vector = linePointB - linePointA;
			Vector3 normalized2 = vector.normalized;
			float num = Vector3.Dot(planeNormal, normalized2);
			if (num < 0f)
			{
				Vector3 normalized3 = (planePoint - linePointA).normalized;
				float num2 = Vector3.Dot(planeNormal, normalized3);
				if (num2 < 0f)
				{
					float num3 = num2 / num;
					if (num3 <= vector.sqrMagnitude)
					{
						intersection = linePointA + normalized2 * num3;
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool RayPlaneIntersection(Ray ray, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
	{
		intersection = Vector3.zero;
		float num = Vector3.Dot(planeNormal, ray.direction);
		if (num < 0f)
		{
			float num2 = Vector3.Dot(planeNormal, (planePoint - ray.origin).normalized);
			if (num2 < 0f)
			{
				float num3 = num2 / num;
				intersection = ray.origin + ray.direction * num3;
				return true;
			}
		}
		return false;
	}

	public static Vector3 SetVectorLength(Vector3 vector, float size)
	{
		return Vector3.Normalize(vector) * size;
	}

	public static bool TriRayIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
	{
		Vector3 vector = p2 - p1;
		Vector3 vector2 = p3 - p1;
		Vector3 rhs = Vector3.Cross(ray.direction, vector2);
		float num = Vector3.Dot(vector, rhs);
		if (num > 0f - Mathf.Epsilon && num < Mathf.Epsilon)
		{
			return false;
		}
		float num2 = 1f / num;
		Vector3 lhs = ray.origin - p1;
		float num3 = Vector3.Dot(lhs, rhs) * num2;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 rhs2 = Vector3.Cross(lhs, vector);
		float num4 = Vector3.Dot(ray.direction, rhs2) * num2;
		if (num4 < 0f || num3 + num4 > 1f)
		{
			return false;
		}
		if (Vector3.Dot(vector2, rhs2) * num2 > Mathf.Epsilon)
		{
			return true;
		}
		return false;
	}
}
