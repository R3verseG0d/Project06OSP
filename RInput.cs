using Rewired;
using UnityEngine;

public class RInput : Singleton<RInput>
{
	internal Player P;

	protected RInput()
	{
	}

	private void Awake()
	{
		Object.Instantiate(Resources.Load("DefaultPrefabs/Rewired Input Manager"), Vector3.zero, Quaternion.identity);
		P = ReInput.players.GetPlayer(0);
	}
}
