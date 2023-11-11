using UnityEngine;

public class BoardFX : MonoBehaviour
{
	[Header("Framework")]
	public GameObject FX;

	public Transform Pivot;

	internal PlayerManager PM;

	private GameObject FXObj;

	private ParticleSystem[] FXs;

	private void Start()
	{
		FXObj = Object.Instantiate(FX, Pivot.position, Pivot.rotation);
		FXObj.transform.SetParent(Pivot);
		FXs = FXObj.GetComponentsInChildren<ParticleSystem>();
	}

	private void Update()
	{
		for (int i = 0; i < FXs.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FXs[i].emission;
			emission.enabled = PM.Base.GetState() == "Ground";
		}
	}
}
