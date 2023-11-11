using System.Collections.Generic;
using UnityEngine;

public class ObjectEventTrigger : EventStation
{
	public enum Mode
	{
		StageEvent = 0,
		ObjectGroupEvent = 1
	}

	[Header("Manual Stage Setup")]
	public Mode EventMode;

	public List<GameObject> LockObjects;

	[Header("Stage Event (Call a stage event)")]
	public string EventName;

	[Header("Object Group Event (Activate these)")]
	public GameObject[] ObjectGroup;

	private bool Triggered;

	private void Update()
	{
		if (LockObjects.Count != 0)
		{
			for (int i = 0; i < LockObjects.Count; i++)
			{
				if (LockObjects[i] == null)
				{
					LockObjects.RemoveAt(i);
				}
			}
		}
		if ((LockObjects.Count != 0 && LockObjects != null) || Triggered)
		{
			return;
		}
		Triggered = true;
		if (EventMode == Mode.StageEvent)
		{
			CallEvent(EventName);
		}
		else
		{
			for (int j = 0; j < ObjectGroup.Length; j++)
			{
				ObjectGroup[j].SetActive(!ObjectGroup[j].activeSelf);
			}
		}
		Object.Destroy(base.gameObject);
	}
}
