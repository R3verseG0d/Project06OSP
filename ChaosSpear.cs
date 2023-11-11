using STHLua;
using UnityEngine;

public class ChaosSpear : AttackBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public GameObject ExplosionFX;

	public GameObject MaxExplosionFX;

	public GameObject LanceExplosionFX;

	public GameObject LanceMaxExplosionFX;

	public GameObject[] SpearEffects;

	internal GameObject ClosestTarget;

	internal bool ChaosLance;

	internal bool MaxPower;

	internal bool DamageEnemies;

	internal bool FullPower;

	private float StartTime;

	private void Start()
	{
		if (!ChaosLance)
		{
			SpearEffects[DamageEnemies ? 1 : 0].SetActive(value: true);
		}
		else
		{
			SpearEffects[(!MaxPower) ? 2 : 3].SetActive(value: true);
		}
		StartTime = Time.time;
	}

	private void FixedUpdate()
	{
		if ((bool)ClosestTarget)
		{
			base.transform.forward = ClosestTarget.transform.position - base.transform.position;
		}
		_Rigidbody.MovePosition(base.transform.position + base.transform.forward * Shadow_Lua.c_chaos_spear_speed * 0.01f);
		if (Time.time - StartTime > Shadow_Lua.c_chaos_spear_atime || AttackSphere(Shadow_Lua.c_chaos_spear_power, FullPower ? 10 : ((!ChaosLance) ? Shadow_Lua.c_chaos_spear_damage : ((!MaxPower) ? Shadow_Lua.c_chaos_spear_damage : 10)), ChaosLance ? "OnHit" : ((!DamageEnemies) ? "OnFlash" : "OnHit"), 0.5f, (!ChaosLance && !DamageEnemies) ? "ChaosSpear" : (ChaosLance ? "ChaosLance" : "")) || SwitchAttackSphere())
		{
			Explode();
		}
	}

	private void Explode()
	{
		Object.Instantiate(ChaosLance ? ((!MaxPower) ? LanceExplosionFX : LanceMaxExplosionFX) : ((!DamageEnemies) ? ExplosionFX : MaxExplosionFX), base.transform.position, Quaternion.identity);
		if (ChaosLance && MaxPower)
		{
			AttackSphere_Dir(3f, 3f, (!FullPower) ? 1 : 10, (!ChaosLance) ? "" : "ChaosLance");
		}
		Object.Destroy(base.gameObject);
	}
}
