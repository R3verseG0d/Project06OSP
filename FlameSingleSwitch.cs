public class FlameSingleSwitch : EventStation
{
	private int count_flame_switch_all;

	private int count_flame_switch_current;

	private void Start()
	{
		count_flame_switch_all = 5;
		count_flame_switch_current = 0;
	}

	public void OnOff(bool _TurnedOn)
	{
		if (_TurnedOn)
		{
			count_flame_switch_current++;
		}
		else
		{
			count_flame_switch_current--;
		}
		if (count_flame_switch_current == count_flame_switch_all)
		{
			CallEvent("door_open02");
		}
	}
}
