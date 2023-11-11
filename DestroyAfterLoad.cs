using UnityEngine;

public class DestroyAfterLoad : MonoBehaviour
{
	public void DestroyThis()
	{
		Object.Destroy(base.gameObject);
	}
}
