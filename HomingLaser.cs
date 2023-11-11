using STHLua;
using UnityEngine;

public class HomingLaser : AttackBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public GameObject ExplosionFX;

	internal GameObject ClosestTarget;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void FixedUpdate()
	{
		if ((bool)ClosestTarget)
		{
			base.transform.forward = Vector3.Lerp(base.transform.forward, (ClosestTarget.transform.position - base.transform.position).normalized, Time.fixedDeltaTime * 10f);
		}
		_Rigidbody.MovePosition(base.transform.position + base.transform.forward * Omega_Lua.c_omega_laser_speed * 0.01f);
		if (Time.time - StartTime > Omega_Lua.c_omega_laser_atime || AttackSphere(Omega_Lua.c_omega_laser_power, Omega_Lua.c_omega_laser_damage, "OnHit") || !ClosestTarget)
		{
			Explode();
		}
	}

	private void Explode()
	{
		Object.Instantiate(ExplosionFX, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
