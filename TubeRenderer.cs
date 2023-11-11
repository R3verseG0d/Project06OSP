using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class TubeRenderer : MonoBehaviour
{
	public TubeVertex[] vertices;

	public int crossSegments = 3;

	public float uvLength;

	private Vector3[] crossPoints;

	private int lastCrossSegments;

	private Mesh mesh;

	private void Reset()
	{
		vertices = new TubeVertex[2]
		{
			new TubeVertex(Vector3.zero, 1f, Color.white),
			new TubeVertex(new Vector3(1f, 0f, 0f), 1f, Color.white)
		};
	}

	private void LateUpdate()
	{
		if (vertices == null || vertices.Length <= 1)
		{
			return;
		}
		if (mesh == null)
		{
			mesh = new Mesh();
			GetComponent<MeshFilter>().mesh = mesh;
		}
		if (crossSegments != lastCrossSegments)
		{
			crossPoints = new Vector3[crossSegments];
			float num = (float)Math.PI * 2f / (float)crossSegments;
			for (int i = 0; i < crossSegments; i++)
			{
				crossPoints[i] = new Vector3(Mathf.Cos(num * (float)i), Mathf.Sin(num * (float)i), 0f);
			}
			lastCrossSegments = crossSegments;
		}
		Vector3[] array = new Vector3[vertices.Length * crossSegments];
		Vector2[] array2 = new Vector2[vertices.Length * crossSegments];
		Color[] array3 = new Color[vertices.Length * crossSegments];
		int[] array4 = new int[vertices.Length * crossSegments * 6];
		int[] array5 = new int[crossSegments];
		int[] array6 = new int[crossSegments];
		_ = Quaternion.identity;
		for (int j = 0; j < vertices.Length; j++)
		{
			for (int k = 0; k < crossSegments; k++)
			{
				int num2 = j * crossSegments + k;
				array[num2] = vertices[j].point + vertices[j].rotation * -crossPoints[k] * vertices[j].radius;
				array2[num2] = new Vector2((0f + (float)k) / (float)crossSegments, (0f + (float)j) / (float)vertices.Length * uvLength);
				array3[num2] = vertices[j].color;
				array5[k] = array6[k];
				array6[k] = j * crossSegments + k;
			}
			if (j > 0)
			{
				for (int l = 0; l < crossSegments; l++)
				{
					int num3 = (j * crossSegments + l) * 6;
					array4[num3] = array5[l];
					array4[num3 + 1] = array5[(l + 1) % crossSegments];
					array4[num3 + 2] = array6[l];
					array4[num3 + 3] = array4[num3 + 2];
					array4[num3 + 4] = array4[num3 + 1];
					array4[num3 + 5] = array6[(l + 1) % crossSegments];
				}
			}
		}
		mesh.Clear();
		mesh.vertices = array;
		mesh.triangles = array4;
		mesh.RecalculateNormals();
		mesh.uv = array2;
		mesh.colors = array3;
	}

	private void SetPoints(Vector3[] points, float radius, Color col)
	{
		if (points.Length >= 2)
		{
			vertices = new TubeVertex[points.Length + 2];
			Vector3 vector = (points[0] - points[1]) * 0.01f;
			vertices[0] = new TubeVertex(vector + points[0], 0f, col);
			Vector3 vector2 = (points[points.Length - 1] - points[points.Length - 2]) * 0.01f;
			vertices[vertices.Length - 1] = new TubeVertex(vector2 + points[points.Length - 1], 0f, col);
			for (int i = 0; i < points.Length; i++)
			{
				vertices[i + 1] = new TubeVertex(points[i], radius, col);
			}
		}
	}
}
