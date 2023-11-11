using UnityEngine;

public class Dtd_SwitchCounter : EventStation
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

	[Header("Framework Settings")]
	public Dtd_BilliardSwitch Target0Settings;

	public Dtd_BilliardSwitch Target1Settings;

	public Dtd_BilliardSwitch Target2Settings;

	public Dtd_BilliardSwitch Target3Settings;

	public Dtd_BilliardSwitch Target4Settings;

	public Dtd_BilliardSwitch Target5Settings;

	public Dtd_BilliardSwitch Target6Settings;

	public Dtd_BilliardSwitch Target7Settings;

	public Dtd_BilliardSwitch Target8Settings;

	internal int TotalCount;

	private bool CalledEvent;

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
			Target0Settings.Counter = this;
		}
		if ((bool)Target1Settings)
		{
			Target1Settings.Counter = this;
		}
		if ((bool)Target2Settings)
		{
			Target2Settings.Counter = this;
		}
		if ((bool)Target3Settings)
		{
			Target3Settings.Counter = this;
		}
		if ((bool)Target4Settings)
		{
			Target4Settings.Counter = this;
		}
		if ((bool)Target5Settings)
		{
			Target5Settings.Counter = this;
		}
		if ((bool)Target6Settings)
		{
			Target6Settings.Counter = this;
		}
		if ((bool)Target7Settings)
		{
			Target7Settings.Counter = this;
		}
		if ((bool)Target8Settings)
		{
			Target8Settings.Counter = this;
		}
	}

	public void AddCount()
	{
		TotalCount++;
		if (TotalCount >= Count && !CalledEvent)
		{
			CallEvent(Event);
			CalledEvent = true;
		}
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
