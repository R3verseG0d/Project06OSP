using UnityEngine;

public class ObjectSignalDestroyer : MonoBehaviour
{
	[Header("Destroys objects on Game.Signal")]
	public GameObject[] Objects;

	private void OnEventSignal()
	{
		for (int i = 0; i < Objects.Length; i++)
		{
			if ((bool)Objects[i])
			{
				Object.Destroy(Objects[i]);
			}
		}
	}
}
