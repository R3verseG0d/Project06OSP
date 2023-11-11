using System.Collections;
using UnityEngine;

public class CrisisCityLightning : MonoBehaviour
{
	[Header("Framework")]
	public ParticleSystem AreaSpot;

	public Transform[] AreaPositions;

	public Renderer SkyRenderer;

	public float[] ExecTimes;

	public float LoopTime;

	public AnimationCurve[] SkyLayers;

	public AudioSource Audio;

	public AudioClip[] Clips;

	private MaterialPropertyBlock PropBlock;

	private float LastTime;

	private float SkyLayer1Time;

	private float SkyLayer2Time;

	private float SkyLayer3Time;

	private int RandomFlashLayer;

	private void Start()
	{
		PropBlock = new MaterialPropertyBlock();
		LastTime = Time.time;
		SkyLayer1Time = 1f;
		SkyLayer2Time = 1f;
		SkyLayer3Time = 1f;
		RandomFlashLayer = 0;
	}

	private void Update()
	{
		SkyLayer1Time += Time.deltaTime;
		SkyLayer2Time += Time.deltaTime;
		SkyLayer3Time += Time.deltaTime;
		if (Time.time - LastTime > LoopTime)
		{
			StartCoroutine(OnExecute());
		}
		SkyRenderer.GetPropertyBlock(PropBlock, 0);
		PropBlock.SetFloat("_Layer1Int", SkyLayers[RandomFlashLayer].Evaluate(SkyLayer1Time));
		PropBlock.SetFloat("_Layer2Int", SkyLayers[RandomFlashLayer].Evaluate(SkyLayer2Time));
		PropBlock.SetFloat("_Layer3Int", SkyLayers[RandomFlashLayer].Evaluate(SkyLayer3Time));
		SkyRenderer.SetPropertyBlock(PropBlock, 0);
	}

	private IEnumerator OnExecute()
	{
		LastTime = Time.time;
		LoopTime = ExecTimes[Random.Range(0, ExecTimes.Length)];
		float StartTime = Time.time;
		float Timer = 0f;
		bool InArea = Random.value > 0.65f;
		Audio.PlayOneShot(Clips[InArea ? 1 : 0], InArea ? 1.25f : Random.Range(0.5f, 1f));
		Audio.pitch = Random.Range(0.5f, 1.5f);
		while (Timer <= (InArea ? 0.25f : 0.01f))
		{
			Timer = Time.time - StartTime;
			yield return new WaitForFixedUpdate();
		}
		if (InArea)
		{
			AreaSpot.transform.position = AreaPositions[Random.Range(0, AreaPositions.Length)].position;
			AreaSpot.Play();
			yield break;
		}
		int num = Random.Range(0, 3);
		RandomFlashLayer = Random.Range(0, 2);
		switch (num)
		{
		case 0:
			SkyLayer1Time = 0f;
			break;
		case 1:
			SkyLayer2Time = 0f;
			break;
		case 2:
			SkyLayer3Time = 0f;
			break;
		}
	}
}
