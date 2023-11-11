using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : Singleton<DebugManager>
{
	private PlayerCamera Camera;

	private Canvas UICanvas;

	private UI HUD;

	protected DebugManager()
	{
	}

	public void StartDebugManager()
	{
	}

	private void Update()
	{
		if (Singleton<RInput>.Instance.P.GetButtonDown("Back"))
		{
			if (!Camera)
			{
				Camera = Object.FindObjectOfType<PlayerCamera>();
			}
			Camera.TrailerCamera = !Camera.TrailerCamera;
		}
		if (Singleton<RInput>.Instance.P.GetButtonDown("Left Bumper") && Singleton<RInput>.Instance.P.GetButtonDown("Right Bumper"))
		{
			if (!HUD)
			{
				HUD = Object.FindObjectOfType<UI>();
			}
			if ((bool)HUD && !UICanvas)
			{
				UICanvas = HUD.GetComponent<Canvas>();
			}
			UICanvas.enabled = !UICanvas.enabled;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SceneManager.LoadScene("JordanCutsceneWvo", LoadSceneMode.Single);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SceneManager.LoadScene("JordanShadowCutscene", LoadSceneMode.Single);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SceneManager.LoadScene("JordanSilverCutscene", LoadSceneMode.Single);
		}
	}
}
