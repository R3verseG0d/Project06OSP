using System.Collections.Generic;
using UnityEngine;

public class Switch_Collector : EventStation
{
	[Header("Framework")]
	public int Count;

	public Vector3 Target0;

	public Vector3 Target1;

	public Vector3 Target2;

	public Vector3 Target3;

	public Vector3 Target4;

	public Vector3 Target5;

	public Vector3 Target6;

	public Vector3 Target7;

	public Vector3 Target8;

	public string Event;

	[Header("Optional")]
	public bool AllActiveSetToForever;

	public bool ActivateInOrder;

	public int[] TargetOrder;

	[Header("Framework Settings")]
	public Common_Switch Target0Settings;

	public Common_Switch Target1Settings;

	public Common_Switch Target2Settings;

	public Common_Switch Target3Settings;

	public Common_Switch Target4Settings;

	public Common_Switch Target5Settings;

	public Common_Switch Target6Settings;

	public Common_Switch Target7Settings;

	public Common_Switch Target8Settings;

	private List<Common_Switch> Switches = new List<Common_Switch>();

	private List<bool> SwitchTriggered = new List<bool>();

	public List<int> SwitchCount = new List<int>();

	private bool TriggerEvent;

	public void SetParameters(int _Count, Vector3 _Target0, Vector3 _Target1, Vector3 _Target2, Vector3 _Target3, Vector3 _Target4, Vector3 _Target5, Vector3 _Target6, Vector3 _Target7, Vector3 _Target8, string _Event)
	{
		Count = _Count;
		Target0 = _Target0;
		Target1 = _Target1;
		Target2 = _Target2;
		Target3 = _Target3;
		Target4 = _Target4;
		Target5 = _Target5;
		Target6 = _Target6;
		Target7 = _Target7;
		Target8 = _Target8;
		Event = _Event;
	}

	private void Start()
	{
		if ((bool)Target0Settings)
		{
			Switches.Add(Target0Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target1Settings)
		{
			Switches.Add(Target1Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target2Settings)
		{
			Switches.Add(Target2Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target3Settings)
		{
			Switches.Add(Target3Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target4Settings)
		{
			Switches.Add(Target4Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target5Settings)
		{
			Switches.Add(Target5Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target6Settings)
		{
			Switches.Add(Target6Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target7Settings)
		{
			Switches.Add(Target7Settings);
			SwitchTriggered.Add(item: false);
		}
		if ((bool)Target8Settings)
		{
			Switches.Add(Target8Settings);
			SwitchTriggered.Add(item: false);
		}
	}

	private void Update()
	{
		if (TriggerEvent)
		{
			return;
		}
		if ((bool)Target0Settings)
		{
			if (Target0Settings.IsActivated && !SwitchTriggered[0])
			{
				SwitchCount.Add(0);
				SwitchTriggered[0] = true;
			}
			else if (!Target0Settings.IsActivated && SwitchTriggered[0])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[0] = false;
			}
		}
		if ((bool)Target1Settings)
		{
			if (Target1Settings.IsActivated && !SwitchTriggered[1])
			{
				SwitchCount.Add(1);
				SwitchTriggered[1] = true;
			}
			else if (!Target1Settings.IsActivated && SwitchTriggered[1])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[1] = false;
			}
		}
		if ((bool)Target2Settings)
		{
			if (Target2Settings.IsActivated && !SwitchTriggered[2])
			{
				SwitchCount.Add(2);
				SwitchTriggered[2] = true;
			}
			else if (!Target2Settings.IsActivated && SwitchTriggered[2])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[2] = false;
			}
		}
		if ((bool)Target3Settings)
		{
			if (Target3Settings.IsActivated && !SwitchTriggered[3])
			{
				SwitchCount.Add(3);
				SwitchTriggered[3] = true;
			}
			else if (!Target3Settings.IsActivated && SwitchTriggered[3])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[3] = false;
			}
		}
		if ((bool)Target4Settings)
		{
			if (Target4Settings.IsActivated && !SwitchTriggered[4])
			{
				SwitchCount.Add(4);
				SwitchTriggered[4] = true;
			}
			else if (!Target4Settings.IsActivated && SwitchTriggered[4])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[4] = false;
			}
		}
		if ((bool)Target5Settings)
		{
			if (Target5Settings.IsActivated && !SwitchTriggered[5])
			{
				SwitchCount.Add(5);
				SwitchTriggered[5] = true;
			}
			else if (!Target5Settings.IsActivated && SwitchTriggered[5])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[5] = false;
			}
		}
		if ((bool)Target6Settings)
		{
			if (Target6Settings.IsActivated && !SwitchTriggered[6])
			{
				SwitchCount.Add(6);
				SwitchTriggered[6] = true;
			}
			else if (!Target6Settings.IsActivated && SwitchTriggered[6])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[6] = false;
			}
		}
		if ((bool)Target7Settings)
		{
			if (Target7Settings.IsActivated && !SwitchTriggered[7])
			{
				SwitchCount.Add(7);
				SwitchTriggered[7] = true;
			}
			else if (!Target7Settings.IsActivated && SwitchTriggered[7])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[7] = false;
			}
		}
		if ((bool)Target8Settings)
		{
			if (Target8Settings.IsActivated && !SwitchTriggered[8])
			{
				SwitchCount.Add(8);
				SwitchTriggered[8] = true;
			}
			else if (!Target8Settings.IsActivated && SwitchTriggered[8])
			{
				SwitchCount.RemoveAt(0);
				SwitchTriggered[8] = false;
			}
		}
		if (SwitchCount.Count < Count)
		{
			return;
		}
		if (AllActiveSetToForever)
		{
			if ((bool)Target0Settings)
			{
				Target0Settings.SetToActiveForeverMode();
			}
			if ((bool)Target1Settings)
			{
				Target1Settings.SetToActiveForeverMode();
			}
			if ((bool)Target2Settings)
			{
				Target2Settings.SetToActiveForeverMode();
			}
			if ((bool)Target3Settings)
			{
				Target3Settings.SetToActiveForeverMode();
			}
			if ((bool)Target4Settings)
			{
				Target4Settings.SetToActiveForeverMode();
			}
			if ((bool)Target5Settings)
			{
				Target5Settings.SetToActiveForeverMode();
			}
			if ((bool)Target6Settings)
			{
				Target6Settings.SetToActiveForeverMode();
			}
			if ((bool)Target7Settings)
			{
				Target7Settings.SetToActiveForeverMode();
			}
			if ((bool)Target8Settings)
			{
				Target8Settings.SetToActiveForeverMode();
			}
			CompleteCollection();
		}
		else if (ActivateInOrder)
		{
			bool flag = true;
			for (int i = 0; i < SwitchCount.Count; i++)
			{
				if (SwitchCount[i] != TargetOrder[i])
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if ((bool)Target0Settings)
				{
					Target0Settings.SetActivation(Result: false);
				}
				if ((bool)Target1Settings)
				{
					Target1Settings.SetActivation(Result: false);
				}
				if ((bool)Target2Settings)
				{
					Target2Settings.SetActivation(Result: false);
				}
				if ((bool)Target3Settings)
				{
					Target3Settings.SetActivation(Result: false);
				}
				if ((bool)Target4Settings)
				{
					Target4Settings.SetActivation(Result: false);
				}
				if ((bool)Target5Settings)
				{
					Target5Settings.SetActivation(Result: false);
				}
				if ((bool)Target6Settings)
				{
					Target6Settings.SetActivation(Result: false);
				}
				if ((bool)Target7Settings)
				{
					Target7Settings.SetActivation(Result: false);
				}
				if ((bool)Target8Settings)
				{
					Target8Settings.SetActivation(Result: false);
				}
			}
			else
			{
				CompleteCollection();
			}
		}
		else
		{
			CompleteCollection();
		}
	}

	private void CompleteCollection()
	{
		CallEvent(Event);
		TriggerEvent = true;
	}

	private void OnDrawGizmosSelected()
	{
		if (Target0 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target0);
			Gizmos.DrawWireSphere(Target0, 0.25f);
		}
		if (Target1 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target1);
			Gizmos.DrawWireSphere(Target1, 0.25f);
		}
		if (Target2 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target2);
			Gizmos.DrawWireSphere(Target2, 0.25f);
		}
		if (Target3 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target3);
			Gizmos.DrawWireSphere(Target3, 0.25f);
		}
		if (Target4 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target4);
			Gizmos.DrawWireSphere(Target4, 0.25f);
		}
		if (Target5 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target5);
			Gizmos.DrawWireSphere(Target5, 0.25f);
		}
		if (Target6 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target6);
			Gizmos.DrawWireSphere(Target6, 0.25f);
		}
		if (Target7 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target7);
			Gizmos.DrawWireSphere(Target7, 0.25f);
		}
		if (Target8 != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Target8);
			Gizmos.DrawWireSphere(Target8, 0.25f);
		}
	}
}
