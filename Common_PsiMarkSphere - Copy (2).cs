using System.Collections;
using UnityEngine;

public class Common_PsiMarkSphere : EventStation
{
	[Header("Framework")]
	public float Radius;

	public string Event;

	[Header("Prefab")]
	public Renderer Renderer;

	public AudioSource Audio;

	public AnimationCurve LightAnimation;

	public Light ActivateLight;

	public ParticleSystem[] FX;

	internal GameObject Target;

	internal Transform Player;

	internal bool Appear;

	private float PsiInt;

	private float ActivateLightInt;

	private MaterialPropertyBlock PropBlock;

	public void SetParameters(float _Radius, string _Event)
	{
		Radius = _Radius;
		Event = _Event;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		Player = GameObject.FindGameObjectWithTag("Player").transform;
		Renderer.GetPropertyBlock(PropBlock);
		PsiInt = 0f;
		PropBlock.SetFloat("_Intensity", PsiInt);
		Renderer.SetPropertyBlock(PropBlock);
		ActivateLightInt = ActivateLight.intensity;
	}

	private void Update()
	{
		if (!Player)
		{
			Player = GameObject.FindGameObjectWithTag("Player").transform;
		}
		float num = Vector3.Distance(base.transform.position, Player.position);
		if (num < Radius && !Appear)
		{
			ManageFX(Result: true);
			Appear = true;
		}
		else if (num > Radius && Appear)
		{
			ManageFX(Result: false);
			Appear = false;
		}
		PsiInt = Mathf.Lerp(PsiInt, Appear ? 1f : 0f, Time.deltaTime * 5f);
		Renderer.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Intensity", PsiInt);
		Renderer.SetPropertyBlock(PropBlock);
	}

	private void ManageFX(bool Result)
	{
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = Result;
		}
	}

	public void OnHotspot()
	{
		if (!Target)
		{
			CallEvent(Event);
		}
		else
		{
			Target.SendMessage("OnEventSignal", SendMessageOptions.DontRequireReceiver);
		}
		Audio.Play();
		StartCoroutine(PlayLightFX());
	}

	public void OnRelease()
	{
		if ((bool)Target)
		{
			Target.SendMessage("OnESPRelease", SendMessageOptions.DontRequireReceiver);
		}
	}

	private IEnumerator PlayLightFX()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		ActivateLight.enabled = true;
		while (Timer <= 1f)
		{
			Timer = Time.time - StartTime;
			ActivateLight.intensity = ActivateLightInt * LightAnimation.Evaluate(Timer);
			yield return new WaitForFixedUpdate();
		}
		ActivateLight.enabled = false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, Radius);
	}
}
