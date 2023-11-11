using UnityEngine;

public class Dtd_BilliardSwitch : EventStation
{
	[Header("Framework")]
	public string Event;

	[Header("Optional")]
	public bool ResetBilliard;

	public bool AttractionArea;

	public float AttractRadius;

	public Vector3 AttractPoint;

	[Header("Prefab")]
	public AudioSource Audio;

	internal Dtd_SwitchCounter Counter;

	private bool CalledEvent;

	public void SetParameters(string _Event)
	{
		Event = _Event;
	}

	private void FixedUpdate()
	{
		if (AttractionArea)
		{
			AttackSphere();
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		Dtd_Billiard component = collider.GetComponent<Dtd_Billiard>();
		if (!component)
		{
			return;
		}
		if (!ResetBilliard)
		{
			Audio.PlayOneShot(Audio.clip, Audio.volume * 2f);
			if ((bool)Counter)
			{
				Counter.AddCount();
			}
			else if (!CalledEvent)
			{
				CallEvent(Event);
				CalledEvent = true;
			}
			component.OnSwitch();
		}
		else
		{
			component.Reset();
		}
	}

	public void AttackSphere()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + AttractPoint, AttractRadius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Dtd_Billiard component = array[i].GetComponent<Dtd_Billiard>();
				if ((bool)component)
				{
					component.RigidBody.AddForce((base.transform.position - component.transform.position).normalized.MakePlanar() * 10f, ForceMode.Acceleration);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (AttractionArea)
		{
			Vector3 vector = base.transform.position + AttractPoint;
			Gizmos.DrawLine(base.transform.position, vector);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(vector, AttractRadius);
		}
	}
}
