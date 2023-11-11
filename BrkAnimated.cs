using UnityEngine;

public class BrkAnimated : MonoBehaviour
{
	public Animator Animator;

	public GameObject Object;

	public GameObject BrkObject;

	public GameObject[] FX;

	private bool Broken;

	private void OnHit(HitInfo HitInfo)
	{
		if (!Broken)
		{
			OnBreak();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (Broken)
		{
			if (collision.relativeVelocity.magnitude > 5f)
			{
				HitInfo value = new HitInfo(base.transform, collision.relativeVelocity * 2f, 10);
				collision.collider.SendMessage("OnHitDestroy", value, SendMessageOptions.DontRequireReceiver);
			}
			else if (collision.collider.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.collider.transform.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
			{
				HitInfo value2 = new HitInfo(base.transform, Vector3.zero, 10);
				collision.collider.SendMessage("OnHitDestroy", value2, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void OnEvent(string Type)
	{
		if (!Broken && Type == "OnBreak")
		{
			OnBreak();
		}
	}

	private void OnBreak()
	{
		if ((bool)Object)
		{
			Object.SetActive(value: false);
		}
		if ((bool)BrkObject)
		{
			BrkObject.SetActive(value: true);
		}
		Animator.SetTrigger("On Trigger");
		Broken = true;
		GameObject[] fX = FX;
		for (int i = 0; i < fX.Length; i++)
		{
			fX[i].SetActive(value: true);
		}
	}
}
