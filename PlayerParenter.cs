using UnityEngine;

public class PlayerParenter : ObjectBase
{
	[Header("Framework")]
	public bool UseTrigger;

	public bool UseCollision;

	private void OnTriggerEnter(Collider collider)
	{
		if (UseTrigger)
		{
			if (collider.gameObject.tag == "Player" && collider.gameObject.GetComponent<PlayerBase>().IsGrounded())
			{
				collider.gameObject.GetComponent<PlayerBase>().ParentPlayer(base.transform);
			}
			if (collider.gameObject.tag == "Amigo" && collider.gameObject.GetComponent<AmigoAIBase>().IsGrounded())
			{
				collider.gameObject.GetComponent<AmigoAIBase>().ParentAmigo(base.transform);
			}
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (!UseTrigger)
		{
			return;
		}
		if (collider.gameObject.tag == "Player")
		{
			if (collider.gameObject.GetComponent<PlayerBase>().IsGrounded() && collider.gameObject.GetComponent<PlayerBase>().RaycastHit.transform == base.transform && !collider.gameObject.transform.parent)
			{
				collider.gameObject.GetComponent<PlayerBase>().ParentPlayer(base.transform);
			}
			else if (!collider.gameObject.GetComponent<PlayerBase>().IsGrounded() || collider.gameObject.GetComponent<PlayerBase>().RaycastHit.transform != base.transform)
			{
				collider.gameObject.GetComponent<PlayerBase>().UnparentPlayer(base.transform);
			}
		}
		if (collider.gameObject.tag == "Amigo")
		{
			if (collider.gameObject.GetComponent<AmigoAIBase>().IsGrounded() && collider.gameObject.GetComponent<AmigoAIBase>().RaycastHit.transform == base.transform && !collider.gameObject.transform.parent)
			{
				collider.gameObject.GetComponent<AmigoAIBase>().ParentAmigo(base.transform);
			}
			else if (!collider.gameObject.GetComponent<AmigoAIBase>().IsGrounded() || collider.gameObject.GetComponent<AmigoAIBase>().RaycastHit.transform != base.transform)
			{
				collider.gameObject.GetComponent<AmigoAIBase>().UnparentAmigo(base.transform);
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (UseTrigger)
		{
			if (collider.gameObject.tag == "Player")
			{
				collider.gameObject.GetComponent<PlayerBase>().UnparentPlayer(base.transform);
			}
			if (collider.gameObject.tag == "Amigo")
			{
				collider.gameObject.GetComponent<AmigoAIBase>().UnparentAmigo(base.transform);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (UseCollision)
		{
			if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerBase>().IsGrounded())
			{
				collision.gameObject.GetComponent<PlayerBase>().ParentPlayer(base.transform);
			}
			if (collision.gameObject.tag == "Amigo" && collision.gameObject.GetComponent<AmigoAIBase>().IsGrounded())
			{
				collision.gameObject.GetComponent<AmigoAIBase>().ParentAmigo(base.transform);
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!UseCollision)
		{
			return;
		}
		if (collision.gameObject.tag == "Player")
		{
			if (collision.gameObject.GetComponent<PlayerBase>().IsGrounded() && collision.gameObject.GetComponent<PlayerBase>().RaycastHit.transform == base.transform && !collision.gameObject.transform.parent)
			{
				collision.gameObject.GetComponent<PlayerBase>().ParentPlayer(base.transform);
			}
			else if (!collision.gameObject.GetComponent<PlayerBase>().IsGrounded() || collision.gameObject.GetComponent<PlayerBase>().RaycastHit.transform != base.transform)
			{
				collision.gameObject.GetComponent<PlayerBase>().UnparentPlayer(base.transform);
			}
		}
		if (collision.gameObject.tag == "Amigo")
		{
			if (collision.gameObject.GetComponent<AmigoAIBase>().IsGrounded() && collision.gameObject.GetComponent<AmigoAIBase>().RaycastHit.transform == base.transform && !collision.gameObject.transform.parent)
			{
				collision.gameObject.GetComponent<AmigoAIBase>().ParentAmigo(base.transform);
			}
			else if (!collision.gameObject.GetComponent<AmigoAIBase>().IsGrounded() || collision.gameObject.GetComponent<AmigoAIBase>().RaycastHit.transform != base.transform)
			{
				collision.gameObject.GetComponent<AmigoAIBase>().UnparentAmigo(base.transform);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (UseCollision)
		{
			if (collision.gameObject.tag == "Player")
			{
				collision.gameObject.GetComponent<PlayerBase>().UnparentPlayer(base.transform);
			}
			if (collision.gameObject.tag == "Amigo")
			{
				collision.gameObject.GetComponent<AmigoAIBase>().UnparentAmigo(base.transform);
			}
		}
	}
}
