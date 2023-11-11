using UnityEngine;

public class ReturnCollisionMessage : MonoBehaviour
{
	public Transform RedirectTo;

	public bool CollisionStay;

	public bool CollisionExit;

	public bool Trigger;

	public bool TriggerStay;

	public bool TriggerExit;

	public bool ReturnOnHit;

	public bool ReturnOnExplosion;

	private void OnCollisionEnter(Collision collision)
	{
		RedirectTo.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
	}

	private void OnCollisionStay(Collision collision)
	{
		if (CollisionStay)
		{
			RedirectTo.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (CollisionExit)
		{
			RedirectTo.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (Trigger)
		{
			RedirectTo.SendMessage("OnTriggerEnter", collider, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (TriggerStay)
		{
			RedirectTo.SendMessage("OnTriggerStay", collider, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (TriggerExit)
		{
			RedirectTo.SendMessage("OnTriggerExit", collider, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (ReturnOnHit)
		{
			RedirectTo.SendMessage("OnHit", HitInfo, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		if (ReturnOnExplosion)
		{
			RedirectTo.SendMessage("OnHit", HitInfo, SendMessageOptions.DontRequireReceiver);
		}
	}
}
