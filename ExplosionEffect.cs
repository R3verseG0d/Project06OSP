using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
	[Header("Framework")]
	public MonoBehaviour HurtPlayerScript;

	public Collider TriggerCollider;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
		Object.Destroy(base.gameObject, 2f);
	}

	private void Update()
	{
		float num = Time.time - StartTime;
		if (num > 0.1f)
		{
			if (HurtPlayerScript != null)
			{
				HurtPlayerScript.enabled = false;
			}
			SetTriggerCol(Enabled: false);
		}
		else if (num < 0.1f && num > 0.025f)
		{
			SetTriggerCol(Enabled: true);
		}
	}

	private void SetTriggerCol(bool Enabled)
	{
		if (TriggerCollider != null)
		{
			TriggerCollider.enabled = Enabled;
		}
	}
}
