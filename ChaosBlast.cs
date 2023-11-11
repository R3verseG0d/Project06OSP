using STHLua;
using UnityEngine;

public class ChaosBlast : AttackBase
{
	[Header("Framework")]
	public float Radius;

	public AnimationCurve SizeOverLifetime;

	internal bool FullPower;

	private float StartTimer;

	private void Start()
	{
		StartTimer = Time.time;
	}

	private void FixedUpdate()
	{
		Radius = SizeOverLifetime.Evaluate(Time.time - StartTimer);
		AttackSphere_Dir(Radius, Shadow_Lua.c_blast_power, (!FullPower) ? Shadow_Lua.c_blast_damage : 10, "ChaosBlast");
		SwitchAttackSphere(Radius);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, Radius);
	}
}
