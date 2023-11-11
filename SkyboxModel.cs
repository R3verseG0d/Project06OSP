using UnityEngine;

public class SkyboxModel : MonoBehaviour
{
	private Transform PlayerCamera;

	private void Start()
	{
		PlayerCamera = Camera.main.transform;
	}

	private void Update()
	{
		if (PlayerCamera != null)
		{
			base.transform.position = new Vector3(PlayerCamera.position.x, base.transform.position.y, PlayerCamera.position.z);
		}
	}
}
