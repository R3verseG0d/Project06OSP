using System.Collections.Generic;

public class ReflectionSystem
{
	private static ReflectionSystem Instance;

	internal HashSet<ReflectionArea> ReflectionAreas = new HashSet<ReflectionArea>();

	public static ReflectionSystem instance
	{
		get
		{
			if (Instance == null)
			{
				Instance = new ReflectionSystem();
			}
			return Instance;
		}
	}

	public void Add(ReflectionArea o)
	{
		ReflectionAreas.Remove(o);
		ReflectionAreas.Add(o);
	}

	public void Remove(ReflectionArea o)
	{
		ReflectionAreas.Remove(o);
	}
}
