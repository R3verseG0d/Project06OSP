using UnityEngine;

[ExecuteInEditMode]
public class PsiProjector : MonoBehaviour
{
	public Projector Source;

	public float Intensity;

	private void Start()
	{
		Source.material.SetFloat("_Int", Intensity);
	}

	private void Update()
	{
		Source.material.SetFloat("_Int", Intensity);
	}
}
