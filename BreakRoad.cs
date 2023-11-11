using UnityEngine;

public class BreakRoad : ObjectBase
{
	[Header("Prefab")]
	public Animator Animator;

	public AudioSource Audio;

	public AudioSource Audio2;

	public GameObject CollapseFX;

	[Header("Optional")]
	public GameObject[] EnemiesToKill;

	private bool Collapsed;

	private float StartTime;

	private void Collapse()
	{
		if (Collapsed)
		{
			return;
		}
		StartTime = Time.time;
		Audio.Play();
		Audio2.Play();
		Animator.SetTrigger("On Collapse");
		if ((bool)CollapseFX)
		{
			CollapseFX.SetActive(value: true);
		}
		if (EnemiesToKill != null)
		{
			for (int i = 0; i < EnemiesToKill.Length; i++)
			{
				EnemiesToKill[i].SendMessage("OnHit", new HitInfo(base.transform, Vector3.zero, 10), SendMessageOptions.DontRequireReceiver);
			}
			EnemiesToKill = null;
		}
		Collapsed = true;
	}

	private void OnHit(HitInfo HitInfo)
	{
		Collapse();
	}

	private void OnEventSignal()
	{
		Collapse();
	}
}
