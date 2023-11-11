using UnityEngine;

public class GadgetMissile : AttackBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public GameObject ExplosionFX;

	private GameObject ClosestTarget;

	private void Start()
	{
		Invoke("Explode", 3f);
	}

	private void FixedUpdate()
	{
		ClosestTarget = FindTarget(15f);
		if ((bool)ClosestTarget && ClosestTarget.layer == LayerMask.NameToLayer("Enemy"))
		{
			base.transform.forward = Vector3.Lerp(base.transform.forward, (ClosestTarget.transform.position - base.transform.position).normalized, Time.fixedDeltaTime * 10f);
		}
		_Rigidbody.MovePosition(base.transform.position += base.transform.forward * 60f * Time.fixedDeltaTime);
		if (AttackSphere(30f, 2, "OnHit") || AttackEnemyProjectile(0.5f))
		{
			Explode();
		}
	}

	private void Explode()
	{
		Object.Instantiate(ExplosionFX, base.transform.position, Quaternion.identity);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 1f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && (bool)array[i].GetComponentInParent<SearchLight>())
			{
				array[i].GetComponentInParent<SearchLight>().DestroySearchlight();
			}
		}
		Object.Destroy(base.gameObject);
	}
}
