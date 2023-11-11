using UnityEngine;

public class Decalog : MonoBehaviour
{
	private void Start()
	{
		Camera.main.gameObject.GetComponent<SplashSmoke>().enabled = Singleton<Settings>.Instance.settings.CameraLensFX == 1;
	}
}
