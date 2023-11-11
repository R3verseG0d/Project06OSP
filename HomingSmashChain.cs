using UnityEngine;

public class HomingSmashChain : AttackBase
{
	[Header("Framework")]
	public float Radius;

	public GameObject Nuke;

	internal bool IsNuke;

	private void FixedUpdate()
	{
		AttackSphere_Dir(Radius, 10f);
	}

	public void Detonate()
	{
		if (IsNuke)
		{
			AttackSphere_Dir(6f, 10f);
			Object.Instantiate(Nuke, base.transform.position, base.transform.rotation);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(base.transform.position, Radius);
	}
}
