using UnityEngine;

public class CameraTornadoFire : MonoBehaviour
{
	[Header("Framework")]
	public ParticleSystem FX;

	internal Transform Player;

	internal Transform Tornado;

	private void Start()
	{
		FX.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		ParticleSystem.ShapeModule shape = FX.shape;
		shape.radius = Mathf.Lerp(0.35f, 0.75f, Mathf.Clamp01(((Player.position - Tornado.position).magnitude - 70f) / 30f));
	}
}
