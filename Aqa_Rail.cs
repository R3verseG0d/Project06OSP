using UnityEngine;

public class Aqa_Rail : MonoBehaviour
{
	[Header("Framework")]
	public float Scale;

	private bool IsScaled;

	private void Start()
	{
		if (!IsScaled)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, Scale);
			IsScaled = true;
		}
	}
}
