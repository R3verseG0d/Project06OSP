using System.Collections;
using UnityEngine;

public class FreezedMantle : MonoBehaviour
{
	[Header("Framework Settings")]
	public Renderer[] FireWallRenderers;

	public GameObject[] LavaColliders;

	public ParticleSystem[] SkyboxFX;

	public GameObject[] DisableObjects;

	[Header("Prefab")]
	public Animator Animator;

	public Renderer MantleRenderer;

	public ParticleSystem[] FX;

	public ParticleSystem EmergeFX;

	public float ScrollSpeed = 0.1f;

	public float NoiseScrollSpeed = 0.0125f;

	public float CurrentFireInt = 3f;

	private MaterialPropertyBlock PropBlock;

	private float FireFactor;

	private float ScrollVal;

	private float NoiseScrollVal;

	private bool StartCoolOff;

	private bool IsTriggered;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
		FireFactor = 1f;
	}

	private void Update()
	{
		if (StartCoolOff)
		{
			FireFactor = Mathf.Lerp(FireFactor, 0f, Time.deltaTime);
			for (int i = 0; i < FireWallRenderers.Length; i++)
			{
				FireWallRenderers[i].GetPropertyBlock(PropBlock);
				PropBlock.SetFloat("_FireInt", Mathf.Lerp(0f, CurrentFireInt, FireFactor));
				FireWallRenderers[i].SetPropertyBlock(PropBlock);
			}
		}
		ScrollVal += Time.deltaTime * ScrollSpeed * FireFactor;
		NoiseScrollVal += Time.deltaTime * NoiseScrollSpeed * FireFactor;
		MantleRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_ScrollVal", ScrollVal);
		PropBlock.SetFloat("_NoiseScrollVal", NoiseScrollSpeed);
		MantleRenderer.SetPropertyBlock(PropBlock);
	}

	private void OnEventSignal()
	{
		if (!IsTriggered)
		{
			StartCoroutine(OnCoolOff());
			IsTriggered = true;
		}
	}

	private IEnumerator OnCoolOff()
	{
		yield return new WaitForSeconds(1.75f);
		Animator.SetTrigger("On Cool Off");
		Invoke("CoolOff", 2.5f);
		for (int i = 0; i < SkyboxFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = SkyboxFX[i].emission;
			emission.enabled = false;
		}
		EmergeFX.Play();
	}

	private void CoolOff()
	{
		StartCoolOff = true;
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = false;
		}
		for (int j = 0; j < DisableObjects.Length; j++)
		{
			DisableObjects[j].SetActive(value: false);
		}
		LavaColliders[0].SetActive(value: false);
		LavaColliders[1].SetActive(value: true);
	}
}
