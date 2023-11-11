using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
	public float time;

	private void Start()
	{
		Object.Destroy(base.gameObject, time);
	}
}
