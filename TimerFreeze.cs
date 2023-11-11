using UnityEngine;

public class TimerFreeze : MonoBehaviour
{
	[Header("Framework")]
	public float FreezeTime;

	public bool IgnoreEventLimit;

	public bool DontTriggerHUDFade;

	private UI HUD;

	private bool DoneFreezing;

	private float StartTime;

	private void Start()
	{
		if (!IgnoreEventLimit && Singleton<GameManager>.Instance.PlayedEventLimit)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		StartTime = Time.time;
		Singleton<GameManager>.Instance.CountTime = false;
		if (!DontTriggerHUDFade)
		{
			HUD = Object.FindObjectOfType<UI>();
			HUD.HudAnimator.SetTrigger("On Hide");
		}
	}

	private void Update()
	{
		if (Time.time - StartTime > FreezeTime && !DoneFreezing)
		{
			Singleton<GameManager>.Instance.CountTime = true;
			if (!DontTriggerHUDFade)
			{
				HUD.HudAnimator.SetTrigger("On Show");
			}
			Object.Destroy(base.gameObject);
			DoneFreezing = true;
		}
	}
}
