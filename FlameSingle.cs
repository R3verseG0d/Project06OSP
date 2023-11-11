using UnityEngine;

public class FlameSingle : MonoBehaviour
{
	[Header("Framework")]
	public float ShineTime;

	public bool ESPMode;

	public float Light_R;

	public float Light_G;

	public float Light_B;

	public float Light_Range;

	public Vector3 SignalObj;

	public string GuidePath;

	public float GuideSpeed;

	[Header("Optional")]
	public GameObject ActivateObj;

	[Header("Prefab")]
	public Collider[] Colliders;

	public Renderer Renderer;

	public AudioSource Audio;

	public AudioClip[] Clips;

	public GameObject HomingTarget;

	public GameObject BreakObj;

	public Light Light;

	public ParticleSystem ActivateFX;

	public ParticleSystem DestroyFX;

	public ParticleSystem RespawnFX;

	public ParticleSystem[] FX;

	public Transform Pivot;

	public GameObject GuideObj;

	private bool Activated;

	private bool Destroyed;

	private float PitchX;

	private float PitchY;

	private float PitchZ;

	private float StartTime;

	private float PingPongInt;

	public void SetParameters(float _ShineTime, bool _ESPMode, float _Light_R, float _Light_G, float _Light_B, float _Light_Range, Vector3 _SignalObj)
	{
		ShineTime = _ShineTime;
		ESPMode = _ESPMode;
		Light_R = _Light_R;
		Light_G = _Light_G;
		Light_B = _Light_B;
		Light_Range = _Light_Range;
		SignalObj = _SignalObj;
	}

	private void Start()
	{
		PitchX = Random.Range(0f, 360f);
		PitchY = Random.Range(0f, 360f);
		PitchZ = Random.Range(0f, 360f);
		Light.color = new Color(Light_R, Light_G, Light_B);
		Light.range = Light_Range * 2f;
	}

	private void Update()
	{
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = Activated;
		}
		PingPongInt = Mathf.Lerp(1.5f, 2.5f, Mathf.Abs(Mathf.Cos(Time.time * 2f)));
		Light.intensity = Mathf.Lerp(Light.intensity, Activated ? PingPongInt : 0f, Time.deltaTime * 5f);
		if (Destroyed && Time.time - StartTime > 20f)
		{
			SwitchColliders(Enabled: true);
			Renderer.enabled = true;
			HomingTarget.tag = "HomingTarget";
			RespawnFX.Play();
			Destroyed = false;
		}
		if (Activated && !Destroyed && Time.time - StartTime > ShineTime)
		{
			Break();
		}
	}

	private void LateUpdate()
	{
		PitchX += Time.deltaTime * 10f;
		PitchY += Time.deltaTime * 10f;
		PitchZ += Time.deltaTime * 10f;
		Pivot.Rotate(PitchX, PitchY, PitchZ);
	}

	private void OnFlash()
	{
		Break();
	}

	private void OnHit(HitInfo HitInfo)
	{
		Activate();
	}

	private void Activate()
	{
		if (!Activated)
		{
			StartTime = Time.time;
			ActivateFX.Play();
			Audio.PlayOneShot(Clips[0], Audio.volume);
			if (GuidePath != "")
			{
				FlameGuide component = Object.Instantiate(GuideObj, base.transform.position, Quaternion.identity).GetComponent<FlameGuide>();
				component.Spline = GuidePath;
				component.Speed = GuideSpeed;
			}
			if ((bool)ActivateObj)
			{
				ActivateObj.SetActive(value: true);
			}
			Activated = true;
		}
	}

	private void Break()
	{
		Object.Instantiate(BreakObj, base.transform.position, base.transform.rotation).SendMessage("OnCreate", new HitInfo(base.transform, Vector3.zero));
		Activated = false;
		SwitchColliders(Enabled: false);
		Renderer.enabled = false;
		HomingTarget.tag = "Untagged";
		DestroyFX.Play();
		Audio.PlayOneShot(Clips[1], Audio.volume);
		StartTime = Time.time;
		Destroyed = true;
	}

	private void SwitchColliders(bool Enabled)
	{
		for (int i = 0; i < Colliders.Length; i++)
		{
			Colliders[i].enabled = Enabled;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (SignalObj != Vector3.zero)
		{
			Gizmos.DrawLine(base.transform.position, SignalObj);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(SignalObj, 0.5f);
		}
	}
}
