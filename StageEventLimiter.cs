using UnityEngine;

public class StageEventLimiter : MonoBehaviour
{
	public enum Type
	{
		Destroy = 0,
		Switch = 1
	}

	public float ActivateLimit;

	public Type LimiterType;

	public bool SkipOnDialogueOff;

	public bool SkipOnCutscenesOff;

	[Header("Switch Options")]
	public GameObject EventOnObj;

	public GameObject EventOffObj;

	private void Awake()
	{
		if ((Singleton<Settings>.Instance.settings.Dialogue == 0 && SkipOnDialogueOff) || (Singleton<Settings>.Instance.settings.Cutscenes == 0 && SkipOnCutscenesOff))
		{
			ActivateContext();
			if (LimiterType == Type.Destroy)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				EventOffObj.SetActive(value: true);
			}
		}
		else if (Singleton<GameManager>.Instance.PlayedEventLimit)
		{
			if (LimiterType == Type.Destroy)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				EventOffObj.SetActive(value: true);
			}
		}
		else
		{
			if (LimiterType == Type.Switch)
			{
				EventOnObj.SetActive(value: true);
			}
			Invoke("ActivateContext", ActivateLimit);
		}
	}

	private void ActivateContext()
	{
		Singleton<GameManager>.Instance.PlayedEventLimit = true;
	}
}
