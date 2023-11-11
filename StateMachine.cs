using UnityEngine;

public class StateMachine : MonoBehaviour
{
	public delegate void PlayerState();

	public event PlayerState playerState;

	public void Initialize(PlayerState startState)
	{
		this.playerState = startState;
	}

	public void UpdateStateMachine()
	{
		try
		{
			this.playerState();
		}
		catch
		{
			Debug.LogError("State Machine not initialized with a state, please initialize State Machine on the Awake or Start method with a state.");
		}
	}

	public void ChangeState(PlayerState NextState)
	{
		base.gameObject.SendMessage(this.playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
		this.playerState = NextState;
		base.gameObject.SendMessage(this.playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
	}

	public void ChangeState(GameObject Caller, PlayerState NextState)
	{
		base.gameObject.SendMessage(this.playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
		Caller.SendMessage(this.playerState.Method.Name + "End", SendMessageOptions.DontRequireReceiver);
		this.playerState = NextState;
		base.gameObject.SendMessage(this.playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
		Caller.SendMessage(this.playerState.Method.Name + "Start", SendMessageOptions.DontRequireReceiver);
	}
}
