using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[Header("Framework")]
	public SonicNew sonic;

	public SonicFast sonic_fast;

	public Princess princess;

	public SnowBoard snow_board;

	public Shadow shadow;

	public Silver silver;

	public Tails tails;

	public Amy amy;

	public Knuckles knuckles;

	public Blaze blaze;

	public Rouge rouge;

	public Omega omega;

	public MetalSonic metal_sonic;

	[Header("Common")]
	public PlayerEvents PlayerEvents;

	public Rigidbody RBody;

	internal PlayerBase Base;

	internal EffectsBase FXBase;

	private void Awake()
	{
		Base = GetComponent<PlayerBase>();
		FXBase = GetComponent<EffectsBase>();
	}
}
