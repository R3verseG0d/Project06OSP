using System.Collections;
using UnityEngine;

public class FlameSequence : ObjectBase
{
	[Header("Framework")]
	public float PrepareTime;

	public string Event;

	public string GuidePath;

	public float GuideSpeed;

	[Header("Prefab")]
	public Animator Animator;

	public ParticleSystem ActivateFX;

	public AudioSource Audio;

	public Light Light;

	public GameObject HomingTarget;

	public Renderer Renderer;

	public ParticleSystem[] FX;

	public ParticleSystem[] BeaconFX;

	public GameObject GuideObj;

	private FlameSingleSwitch Constructor;

	private FlameGuide Guide;

	private bool TurnedOn;

	private bool Vanish;

	private float StartTime;

	private float PingPongInt;

	private MaterialPropertyBlock PropBlock;

	public void SetParameters(float _PrepareTime, string _Event)
	{
		PrepareTime = _PrepareTime;
		Event = _Event;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		Constructor = Object.FindObjectOfType<FlameSingleSwitch>();
	}

	private void Update()
	{
		if (!Vanish && TurnedOn && Time.time - StartTime > PrepareTime)
		{
			HomingTarget.tag = "HomingTarget";
			TurnedOn = false;
			Guide.KillGuide();
			Guide = null;
			ManageSequence();
		}
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = TurnedOn && !Vanish;
		}
		for (int j = 0; j < BeaconFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = BeaconFX[j].emission;
			emission2.enabled = !TurnedOn && !Vanish;
		}
		if (!Vanish)
		{
			PingPongInt = Mathf.Lerp(1.5f, 2f, Mathf.Abs(Mathf.Cos(Time.time * 2f)));
			Light.intensity = Mathf.Lerp(Light.intensity, TurnedOn ? PingPongInt : 0f, Time.deltaTime * 5f);
		}
	}

	private IEnumerator FadeAway()
	{
		Vanish = true;
		Animator.SetTrigger("On Vanish");
		Guide = null;
		float StartFadeTime = Time.time;
		float Timer = 0f;
		while (Timer <= 1f)
		{
			Timer = Time.time - StartFadeTime;
			Renderer.GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_TintColor", Color.Lerp(PropBlock.GetColor("_TintColor"), Color.clear, Timer * 0.075f));
			Renderer.SetPropertyBlock(PropBlock);
			Light.intensity = Mathf.Lerp(Light.intensity, 0f, Timer * 0.075f);
			yield return new WaitForFixedUpdate();
		}
		Object.Destroy(base.gameObject);
	}

	private void OnFlash()
	{
		Activate();
	}

	private void OnHit(HitInfo HitInfo)
	{
		Activate();
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetState() == "Glide")
		{
			Activate();
		}
	}

	private void Activate()
	{
		if (!TurnedOn)
		{
			StartTime = Time.time;
			TurnedOn = true;
			HomingTarget.tag = "Untagged";
			ActivateFX.Play();
			Audio.Play();
			if (GuidePath != "")
			{
				Guide = Object.Instantiate(GuideObj, base.transform.position, Quaternion.identity).GetComponent<FlameGuide>();
				Guide.Spline = GuidePath;
				Guide.Speed = GuideSpeed;
			}
			ManageSequence();
		}
	}

	private void ManageSequence()
	{
		if (!(Event == ""))
		{
			Constructor.OnOff(TurnedOn);
		}
	}

	private void OnEventSignal()
	{
		StartCoroutine(FadeAway());
	}
}
