using UnityEngine;

public class GadgetBullet : AttackBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public GameObject BulletHitFX;

	internal bool DealDamage;

	private void Start()
	{
		Object.Destroy(base.transform.gameObject, 3f);
	}

	private void FixedUpdate()
	{
		_Rigidbody.MovePosition(base.transform.position += base.transform.forward * 75f * Time.fixedDeltaTime);
		if (BulletAttackSphere(DealDamage) || SwitchAttackSphere(0.25f) || AttackEnemyProjectile(0.25f))
		{
			BulletHit();
		}
	}

	private void BulletHit()
	{
		Object.Instantiate(BulletHitFX, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
