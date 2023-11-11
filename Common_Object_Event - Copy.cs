using UnityEngine;

public class Common_Object_Event : EventStation
{
	[Header("Framework")]
	public string Event;

	private bool DontCallEvent;

	public void SetParameters(string _Event)
	{
		Event = _Event;
	}

	private void OnTriggerEnter(Collider collider)
	{
		Eggtrain component = collider.GetComponent<Eggtrain>();
		if ((!component || !component.Damaged) && collider.gameObject.name.Contains(Event.Split("_"[0])[0]) && !DontCallEvent)
		{
			CallEvent(Event);
			Object.Destroy(base.gameObject);
		}
	}

	private void GateOpen()
	{
		DontCallEvent = false;
	}

	private void GateClose()
	{
		DontCallEvent = true;
	}
}
