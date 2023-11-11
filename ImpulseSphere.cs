using UnityEngine;

public class ImpulseSphere : MonoBehaviour
{
	[Header("Framework")]
	public float Radius;

	public float Impulse;

	public void SetParameters(float _Radius, float _Impulse)
	{
		Radius = _Radius;
		Impulse = _Impulse;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, Radius);
	}

	private void OnEventSignal()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody componentInParent = array[i].GetComponentInParent<Rigidbody>();
			if ((bool)componentInParent)
			{
				componentInParent.AddExplosionForce(Impulse / 2.5f, base.transform.position, Radius, 0f, ForceMode.Impulse);
			}
		}
	}
}
