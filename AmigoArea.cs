using UnityEngine;

public class AmigoArea : MonoBehaviour
{
	public enum Type
	{
		Wait = 0,
		Follow = 1,
		Delete = 2
	}

	[Header("Framework")]
	public Type Behaviour;

	public Vector3 NewPos;

	public bool TriggerByPlayer;

	private void OnTriggerEnter(Collider collider)
	{
		if (TriggerByPlayer)
		{
			if ((bool)collider.GetComponent<PlayerBase>())
			{
				OnArea(Object.FindObjectOfType<AmigoAIBase>());
			}
		}
		else
		{
			OnArea(collider.GetComponent<AmigoAIBase>());
		}
	}

	private void OnArea(AmigoAIBase Amigo = null)
	{
		if ((bool)Amigo)
		{
			switch (Behaviour)
			{
			case Type.Wait:
				Amigo.TotalControlLock = true;
				break;
			case Type.Follow:
				Amigo.TotalControlLock = false;
				break;
			case Type.Delete:
				Amigo.DestroyAmigo();
				break;
			}
			if (NewPos != Vector3.zero)
			{
				Amigo.transform.position = NewPos;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (NewPos != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, NewPos);
			Gizmos.DrawWireSphere(NewPos, 0.5f);
		}
	}
}
