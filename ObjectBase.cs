using UnityEngine;

public class ObjectBase : MonoBehaviour
{
	public PlayerBase GetPlayer(Collider Object)
	{
		return Object.GetComponent<PlayerBase>();
	}

	public PlayerBase GetPlayer(Transform Object)
	{
		return Object.GetComponent<PlayerBase>();
	}
}
