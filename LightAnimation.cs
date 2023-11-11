using UnityEngine;

public class LightAnimation : ObjectBase
{
	[Header("Framework")]
	public string MainLight;

	public string SubLight;

	public string Ambient;

	private SceneParameters SceneParams;

	public void SetParameters(string _MainLight, string _SubLight, string _Ambient)
	{
		MainLight = _MainLight;
		SubLight = _SubLight;
		Ambient = _Ambient;
	}

	private void Start()
	{
		SceneParams = Object.FindObjectOfType<SceneParameters>();
	}

	private void OnTriggerStay(Collider collider)
	{
		if ((bool)GetPlayer(collider))
		{
			SceneParams.SetLightAnimationPreset(MainLight, SubLight, Ambient);
			if (!SceneParams.LightAnimationChange)
			{
				SceneParams.LightAnimationChange = true;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if ((bool)GetPlayer(collider))
		{
			SceneParams.LightAnimationChange = false;
		}
	}
}
