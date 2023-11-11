using System;
using UnityEngine;

public static class UIGradientUtils
{
	public struct Matrix2x3
	{
		public float m00;

		public float m01;

		public float m02;

		public float m10;

		public float m11;

		public float m12;

		public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
		{
			this.m00 = m00;
			this.m01 = m01;
			this.m02 = m02;
			this.m10 = m10;
			this.m11 = m11;
			this.m12 = m12;
		}

		public static Vector2 operator *(Matrix2x3 m, Vector2 v)
		{
			float x = m.m00 * v.x - m.m01 * v.y + m.m02;
			float y = m.m10 * v.x + m.m11 * v.y + m.m12;
			return new Vector2(x, y);
		}
	}

	private static Vector2[] ms_verticesPositions = new Vector2[4]
	{
		Vector2.up,
		Vector2.one,
		Vector2.right,
		Vector2.zero
	};

	public static Vector2[] VerticePositions => ms_verticesPositions;

	public static Matrix2x3 LocalPositionMatrix(Rect rect, Vector2 dir)
	{
		float x = dir.x;
		float y = dir.y;
		Vector2 min = rect.min;
		Vector2 size = rect.size;
		float num = 0.5f;
		float num2 = min.x / size.x + num;
		float num3 = min.y / size.y + num;
		float m = x / size.x;
		float m2 = y / size.y;
		float m3 = 0f - (num2 * x - num3 * y - num);
		float m4 = y / size.x;
		float m5 = x / size.y;
		float m6 = 0f - (num2 * y + num3 * x - num);
		return new Matrix2x3(m, m2, m3, m4, m5, m6);
	}

	public static Vector2 RotationDir(float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = Mathf.Cos(f);
		float y = Mathf.Sin(f);
		return new Vector2(x, y);
	}

	public static Vector2 CompensateAspectRatio(Rect rect, Vector2 dir)
	{
		float num = rect.height / rect.width;
		dir.x *= num;
		return dir.normalized;
	}

	public static float InverseLerp(float a, float b, float v)
	{
		if (a == b)
		{
			return 0f;
		}
		return (v - a) / (b - a);
	}

	public static Color Bilerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
	{
		Color a3 = Color.LerpUnclamped(a1, a2, t.x);
		Color b3 = Color.LerpUnclamped(b1, b2, t.x);
		return Color.LerpUnclamped(a3, b3, t.y);
	}

	public static void Lerp(UIVertex a, UIVertex b, float t, ref UIVertex c)
	{
		c.position = Vector3.LerpUnclamped(a.position, b.position, t);
		c.normal = Vector3.LerpUnclamped(a.normal, b.normal, t);
		c.color = Color32.LerpUnclamped(a.color, b.color, t);
		c.tangent = Vector3.LerpUnclamped(a.tangent, b.tangent, t);
		c.uv0 = Vector3.LerpUnclamped(a.uv0, b.uv0, t);
		c.uv1 = Vector3.LerpUnclamped(a.uv1, b.uv1, t);
	}
}
