using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Zoom : MonoBehaviour
{
	public Image Source;

	public float distance;

	public float scale = 4f;

	private void Start()
	{
		Source.material.SetFloat("_ZoomDistance", distance);
		Source.material.SetFloat("_ZoomScale", scale);
	}

	private void Update()
	{
		Source.material.SetFloat("_ZoomDistance", distance);
		Source.material.SetFloat("_ZoomScale", scale);
	}
}
