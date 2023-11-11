using UnityEngine;

public class AuraManager : MonoBehaviour
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public ParticleSystem[] AuraFX;

	private Vector3 Magnitude;

	private void Start()
	{
		for (int i = 0; i < AuraFX.Length; i++)
		{
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = AuraFX[i].forceOverLifetime;
			forceOverLifetime.enabled = true;
			forceOverLifetime.space = ParticleSystemSimulationSpace.World;
		}
	}

	private void FixedUpdate()
	{
		Magnitude = Vector3.Lerp(Magnitude, -_Rigidbody.velocity, Time.deltaTime * 10f);
		for (int i = 0; i < AuraFX.Length; i++)
		{
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = AuraFX[i].forceOverLifetime;
			forceOverLifetime.x = Magnitude.x;
			forceOverLifetime.y = Magnitude.y;
			forceOverLifetime.z = Magnitude.z;
		}
	}
}
