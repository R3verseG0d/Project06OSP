using UnityEngine;
using UnityEngine.UI;

public class ControllerIcon : MonoBehaviour
{
	[Header("Framework")]
	public Image ButtonSlot;

	public Sprite XboxButton;

	public Sprite PS3Button;

	private void Update()
	{
		if (IsXbox() && ButtonSlot.sprite != XboxButton)
		{
			ButtonSlot.sprite = XboxButton;
		}
		else if (!IsXbox() && ButtonSlot.sprite != PS3Button)
		{
			ButtonSlot.sprite = PS3Button;
		}
	}

	private bool IsXbox()
	{
		if (Singleton<Settings>.Instance.settings.ButtonIcons == 1)
		{
			return false;
		}
		return true;
	}
}
