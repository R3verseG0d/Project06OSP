using UnityEngine;

public class ChaosEaster : ObjectBase
{
	public Animator Animator;

	public Collider Collider;

	private bool Activated;

	private void OnTriggerEnter(Collider collider)
	{
		if ((bool)GetPlayer(collider) && !Activated)
		{
			Animator.SetTrigger("On Start");
			Collider.enabled = false;
			Activated = true;
		}
	}
}
