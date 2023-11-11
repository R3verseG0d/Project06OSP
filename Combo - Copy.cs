using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
	[Header("Framework")]
	public Animator Animator;

	public Text Text;

	public Image Image;

	public Image HighlightImage;

	public Sprite[] Sprites;

	public void OnComboFinish(int ComboBonus, int Level)
	{
		if (Level > 9)
		{
			Level = 9;
		}
		Animator.SetTrigger((Level == 9) ? "On Combo Finish Rainbow" : "On Combo Finish");
		Text.text = ComboBonus.ToString();
		Image.sprite = Sprites[Level];
		HighlightImage.sprite = Sprites[Level];
	}
}
