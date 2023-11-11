using UnityEngine;

public class Kdv_Rainbow : MonoBehaviour
{
	[Header("Prefab")]
	public Transform Mesh;

	private Transform PlayerCamera;

	private void Start()
	{
		PlayerCamera = Camera.main.transform;
	}

	private void Update()
	{
		if (PlayerCamera != null)
		{
			float num = Vector3.Distance(Mesh.position, PlayerCamera.position) * 0.01f;
			float num2 = Mathf.Lerp(1f, 5f, num / 3.5f - 0.035f);
			Mesh.localScale = Vector3.Lerp(Mesh.localScale, Vector3.one * num2, Time.deltaTime * 2f);
		}
	}
}
