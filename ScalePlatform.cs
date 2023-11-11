using UnityEngine;

public class ScalePlatform : MonoBehaviour
{
	[Header("Object")]
	public Rigidbody RBody;

	internal Seesaw Seesaw;

	internal bool FirstScale;

	internal bool SecondScale;

	public void Collapse()
	{
		RBody.isKinematic = false;
		RBody.useGravity = true;
		base.transform.SetParent(null);
		Object.Destroy(base.gameObject, 15f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (FirstScale)
			{
				Seesaw.LowerPlatform1 = true;
				Seesaw.PlayAudio(0, 0);
			}
			else
			{
				Seesaw.LowerPlatform2 = true;
				Seesaw.PlayAudio(1, 0);
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (FirstScale)
			{
				Seesaw.LowerPlatform1 = false;
			}
			else
			{
				Seesaw.LowerPlatform2 = false;
			}
		}
	}
}
