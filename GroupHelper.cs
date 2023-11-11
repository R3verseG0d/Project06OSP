using UnityEngine;

public class GroupHelper : MonoBehaviour
{
	[Header("Framework")]
	public GameObject[] Objects;

	public string OptionalSendMessage;

	private bool HasStarted;

	private void StartGroup()
	{
		if (HasStarted)
		{
			return;
		}
		if (Objects != null)
		{
			for (int i = 0; i < Objects.Length; i++)
			{
				Objects[i].SetActive(value: true);
				if (OptionalSendMessage != "")
				{
					Objects[i].SendMessage(OptionalSendMessage, 0, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		HasStarted = true;
	}
}
