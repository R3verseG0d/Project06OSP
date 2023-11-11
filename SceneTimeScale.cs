using UnityEngine;

public class SceneTimeScale : MonoBehaviour
{
	public float TimeScale;

	private void Update()
	{
		Time.timeScale = TimeScale;
	}
}
