using UnityEngine;

public class GaugeController : MonoBehaviour
{
	public float FillAmount;

	public Renderer _renderer;

	private Transform MainCamera;

	private void Start()
	{
		MainCamera = Camera.main.transform;
	}

	private void Update()
	{
		base.transform.forward = -MainCamera.forward;
		_renderer.material.SetFloat("_Fill", FillAmount);
	}
}
