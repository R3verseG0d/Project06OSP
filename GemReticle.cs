using STHEngine;
using UnityEngine;

public class GemReticle : MonoBehaviour
{
	[Header("Framework")]
	public GameObject IndicatorObject;

	public GameObject OnScreenUI;

	public GameObject[] OffScreenUI;

	internal UI HUD;

	private bool OutOfScreen;

	private void Update()
	{
		if (!HUD.PM)
		{
			return;
		}
		if ((bool)HUD.PM.sonic && (bool)HUD.PM.sonic.SkyGemObject && HUD.PM.sonic.ThrewGem)
		{
			Vector3 position = HUD.PM.sonic.SkyGemObject.transform.position;
			position = HUD.PM.Base.Camera.Camera.WorldToViewportPoint(position);
			OutOfScreen = position.x > 1f || position.y > 1f || position.x < 0f || position.y < 0f;
			if (position.z < 0f)
			{
				position.x = 1f - position.x;
				position.y = 1f - position.y;
				position.z = 0f;
				position = ExtensionMethods.Vector3Maxamize(position);
			}
			position = HUD.PM.Base.Camera.Camera.ViewportToScreenPoint(position);
			position.x = Mathf.Clamp(position.x, 25f, (float)Screen.width - 25f);
			position.y = Mathf.Clamp(position.y, 25f, (float)Screen.height - 25f);
			IndicatorObject.transform.position = position;
			if (OutOfScreen)
			{
				Vector3 vector = HUD.PM.Base.Camera.Camera.transform.InverseTransformPoint(HUD.PM.sonic.SkyGemObject.transform.position);
				float z = (0f - Mathf.Atan2(vector.x, vector.y)) * 57.29578f - 90f;
				IndicatorObject.transform.eulerAngles = new Vector3(0f, 0f, z);
				OffScreenUI[1].transform.eulerAngles = Vector3.zero;
			}
			else
			{
				IndicatorObject.transform.eulerAngles = Vector3.zero;
			}
			OnScreenUI.SetActive(!OutOfScreen);
			OffScreenUI[0].SetActive(OutOfScreen);
			OffScreenUI[1].SetActive(OutOfScreen);
		}
		else
		{
			OutOfScreen = false;
		}
		IndicatorObject.SetActive((bool)HUD.PM.sonic && (bool)HUD.PM.sonic.SkyGemObject && HUD.PM.sonic.ThrewGem);
	}
}
