using UnityEngine;

public class ChangeLight : ObjectBase
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
			SceneParams.SetLightPreset(MainLight, SubLight, Ambient);
			if (!SceneParams.LightChange)
			{
				SceneParams.LightChange = true;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if ((bool)GetPlayer(collider))
		{
			SceneParams.LightChange = false;
		}
	}

	private void OFF()
	{
		SceneParams.LightChange = false;
	}
}
