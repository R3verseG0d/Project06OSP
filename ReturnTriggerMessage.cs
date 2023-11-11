using UnityEngine;

public class ReturnTriggerMessage : MonoBehaviour
{
	public Transform ReturnTo;

	public bool Stun;

	private void OnHit(HitInfo HitInfo)
	{
		if (Stun)
		{
			ReturnTo.SendMessage("StunEnemy", base.gameObject.name);
		}
		else
		{
			ReturnTo.SendMessage("OnHit", HitInfo);
		}
	}

	private void OnFlash()
	{
		ReturnTo.SendMessage("OnFlash", SendMessageOptions.DontRequireReceiver);
	}

	private void OnHitDestroy(Transform _Transform)
	{
		ReturnTo.SendMessage("OnHitDestroy", _Transform);
	}

	private void OnPsychoShock(Transform _Transform)
	{
		ReturnTo.SendMessage("OnPsychoShock", _Transform, SendMessageOptions.DontRequireReceiver);
	}

	private void OnAttract(Transform _Transform)
	{
		ReturnTo.SendMessage("OnAttract", _Transform, SendMessageOptions.DontRequireReceiver);
	}

	private void FullStun()
	{
		ReturnTo.SendMessage("FullStun", SendMessageOptions.DontRequireReceiver);
	}

	private void SetChainProperties(int Context)
	{
		ReturnTo.SendMessage("SetChainProperties", Context, SendMessageOptions.DontRequireReceiver);
	}
}
