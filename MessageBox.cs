using TMPro;
using UnityEngine;

public class MessageBox : UIBase
{
	[Header("Framework")]
	public Animator Animator;

	public AudioSource Audio;

	public TextMeshProUGUI TextComp;

	public UICornersGradient[] UI;

	internal AudioClip[] Clips;

	internal string[] Texts;

	internal float Duration;

	private int Index;

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.TextBoxType == 0)
		{
			UI[0].m_topLeftColor = TextBoxBGColor(0);
			UI[0].m_topRightColor = TextBoxBGColor(1);
			UI[0].m_bottomRightColor = TextBoxBGColor(2);
			UI[0].m_bottomLeftColor = TextBoxBGColor(3);
			UI[1].m_topLeftColor = TextBoxOutlineColor(0);
			UI[1].m_topRightColor = TextBoxOutlineColor(1);
			UI[1].m_bottomRightColor = TextBoxOutlineColor(2);
			UI[1].m_bottomLeftColor = TextBoxOutlineColor(3);
			UI[2].m_topLeftColor = TextBoxBaseColor(0);
			UI[2].m_topRightColor = TextBoxBaseColor(1);
			UI[2].m_bottomRightColor = TextBoxBaseColor(2);
			UI[2].m_bottomLeftColor = TextBoxBaseColor(3);
		}
		if (Clips != null)
		{
			PlayMessage();
			return;
		}
		if ((bool)TextComp)
		{
			TextComp.text = Texts[Index];
		}
		Invoke("EndMessageBox", Duration);
	}

	private void EndMessageBox()
	{
		if ((bool)Animator)
		{
			Animator.SetTrigger("Box Out");
		}
		else
		{
			Object.Destroy(base.gameObject, 0.5f);
		}
	}

	private void PlayMessage()
	{
		if (Index == Clips.Length)
		{
			EndMessageBox();
			return;
		}
		if ((bool)TextComp)
		{
			TextComp.text = Texts[Index];
		}
		Audio.PlayOneShot(Clips[Index], Audio.volume);
		Invoke("PlayMessage", Clips[Index].length);
		Index++;
	}
}
