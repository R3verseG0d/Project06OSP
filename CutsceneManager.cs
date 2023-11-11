using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = 60;
		Settings.SetLocalSettings();
	}
}
