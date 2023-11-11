using UnityEngine;

[ExecuteInEditMode]
public class ReflectionArea : MonoBehaviour
{
	public Renderer Renderer;

	public Vector3 size = Vector3.one;

	public Bounds bounds
	{
		get
		{
			Vector3[] array = new Vector3[12];
			Vector3 vector = Vector3.up * size.y;
			array[0] = Vector3.left * size.x * 0.5f;
			array[1] = Vector3.right * size.x * 0.5f;
			array[2] = Vector3.up * size.y * 0.5f;
			array[3] = Vector3.down * size.y * 0.5f;
			array[4] = Vector3.back * size.z * 0.5f;
			array[5] = Vector3.forward * size.z * 0.5f;
			array[6] = vector + Vector3.left * size.x * 0.5f;
			array[7] = vector + Vector3.right * size.x * 0.5f;
			array[8] = vector + Vector3.up * size.y * 0.5f;
			array[9] = vector + Vector3.down * size.y * 0.5f;
			array[10] = vector + Vector3.back * size.z * 0.5f;
			array[11] = vector + Vector3.forward * size.z * 0.5f;
			return GeometryUtility.CalculateBounds(array, base.transform.localToWorldMatrix);
		}
	}

	public Bounds plane => new Bounds(base.transform.position, new Vector3(size.x, 0f, size.z));

	public void OnEnable()
	{
		ReflectionSystem.instance.Add(this);
	}

	public void OnDisable()
	{
		ReflectionSystem.instance.Remove(this);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.up * size.y * 0.5f, size);
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Vector3.zero, plane.size);
		Gizmos.DrawLine(Vector3.left * size.x * 0.5f, Vector3.right * size.x * 0.5f);
		Gizmos.DrawLine(Vector3.back * size.z * 0.5f, Vector3.forward * size.z * 0.5f);
	}
}
