using System.Collections;
using UnityEngine;

public class FlameCore : EventStation
{
	public enum Type
	{
		Signal = 0,
		Auto = 1
	}

	[Header("Framework")]
	public float AttackTime1;

	public float AttackTime2;

	public float AttackRange;

	public Type FirstMode;

	public string Event;

	public string UpgradeEvent;

	[Header("Prefab")]
	public LayerMask AttackBlock;

	public GameObject Mesh;

	public GameObject Brk;

	public Common_PsiMarkSphere ESPMark;

	public Renderer[] Renderers;

	public Renderer[] BrkRenderers;

	public Transform AttackObj;

	public Renderer AttackRenderer;

	public Renderer SphereAttackRenderer;

	public ParticleSystem AttackFX;

	public ParticleSystem SphereAttackFX;

	public ParticleSystem CoolOffFX;

	public ParticleSystem[] FX;

	public Color OutlineColor;

	public Light Light;

	public AudioClip[] Clips;

	public AudioSource Audio;

	public AudioSource WaitAudio;

	public Transform RotatePoint;

	private PlayerCamera Camera;

	private MaterialPropertyBlock PropBlock;

	private Color FireAlpha;

	private float AttackTime;

	private float RotateInt;

	private float BurnFactor;

	private float FreezeBlend;

	private float OutlineInt;

	private float FireParticleAmount;

	private int TimeIndex;

	private bool IsTriggered;

	private bool LoopAttacks;

	private bool ChargeAttack;

	private bool IsAttacking;

	public void SetParameters(float _AttackTime1, float _AttackTime2, float _AttackRange, int _FirstMode, string _Event)
	{
		AttackTime1 = _AttackTime1;
		AttackTime2 = _AttackTime2;
		AttackRange = _AttackRange;
		FirstMode = (Type)(_FirstMode - 1);
		Event = _Event;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		ESPMark.Target = base.gameObject;
		RotateInt = 1f;
		FreezeBlend = 1f;
		FireAlpha = new Vector4(1f, 1f, 1f, 0.25f);
		FireParticleAmount = 100f;
		Camera = Object.FindObjectOfType<PlayerCamera>();
		if (FirstMode == Type.Auto)
		{
			AttackTime = Time.time;
			LoopAttacks = true;
		}
	}

	private void Update()
	{
		RotatePoint.Rotate(0f, -5f * Time.deltaTime * RotateInt, 0f);
		if (IsTriggered)
		{
			WaitAudio.volume = Mathf.Lerp(WaitAudio.volume, 0f, Time.deltaTime);
			Light.intensity = Mathf.Lerp(Light.intensity, 0f, Time.deltaTime);
			RotateInt = Mathf.Lerp(RotateInt, 0f, Time.deltaTime);
			FreezeBlend = Mathf.Lerp(FreezeBlend, 0f, Time.deltaTime);
		}
		else
		{
			if (ChargeAttack)
			{
				FireAlpha.a = Mathf.Lerp(FireAlpha.a, 1f, Mathf.Clamp01(Time.deltaTime / 6f));
				FireParticleAmount = Mathf.Lerp(FireParticleAmount, 100f, Mathf.Clamp01(Time.deltaTime / 6f));
				Light.intensity = Mathf.Lerp(Light.intensity, 6f, Mathf.Clamp01(Time.deltaTime / 6f));
				BurnFactor = Mathf.MoveTowards(BurnFactor, 1f, Mathf.Clamp01(Time.deltaTime / 6f));
				if (Time.time - AttackTime >= 6f)
				{
					StartCoroutine(OnAttack());
					Camera.PlayShakeMotion(0f, 0.75f);
					AttackFX.Play();
					Audio.PlayOneShot(Clips[0], Audio.volume);
					AttackTime = Time.time;
					ChargeAttack = false;
				}
			}
			else
			{
				FireAlpha.a = Mathf.Lerp(FireAlpha.a, 0.25f, Time.deltaTime * 0.7f);
				FireParticleAmount = Mathf.Lerp(FireParticleAmount, 25f, Time.deltaTime * 0.7f);
				Light.intensity = Mathf.Lerp(Light.intensity, 3f, Time.deltaTime * 0.7f);
				BurnFactor = Mathf.MoveTowards(BurnFactor, 0f, Time.deltaTime * 0.7f);
				if (LoopAttacks && Time.time - AttackTime > 8f + GetAttackTime())
				{
					AttackTime = Time.time;
					ChargeAttack = true;
				}
			}
			for (int i = 0; i < FX.Length; i++)
			{
				ParticleSystem.MainModule main = FX[i].main;
				main.startColor = FireAlpha;
				if (i == 0 || i == 1)
				{
					ParticleSystem.EmissionModule emission = FX[i].emission;
					emission.rateOverTime = FireParticleAmount;
				}
			}
		}
		OutlineInt = ((!IsTriggered) ? Mathf.Lerp(0.25f, 1.5f, BurnFactor) : Mathf.Lerp(OutlineInt, 0f, Time.deltaTime * 2f));
		for (int j = 0; j < Renderers.Length; j++)
		{
			Renderers[j].GetPropertyBlock(PropBlock);
			PropBlock.SetFloat("_BurnBlend", BurnFactor);
			PropBlock.SetFloat("_FreezeBlend", FreezeBlend);
			PropBlock.SetColor("_FresCol", OutlineColor);
			PropBlock.SetFloat("_FresInt", OutlineInt);
			PropBlock.SetColor("_OutlineColor", OutlineColor);
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", OutlineInt);
			Renderers[j].SetPropertyBlock(PropBlock);
		}
	}

	private float GetAttackTime()
	{
		switch (TimeIndex)
		{
		case 1:
			return AttackTime1;
		case 2:
			return AttackTime2;
		default:
			return AttackTime1;
		}
	}

	private void TornadoShoot(FlameCoreParams Params)
	{
		if (Params.ShootIndex != 0)
		{
			if (!LoopAttacks)
			{
				ChargeAttack = true;
				AttackTime = Time.time;
				LoopAttacks = true;
			}
			TimeIndex = Params.ShootIndex;
		}
		else
		{
			LoopAttacks = false;
		}
	}

	private void OnEventSignal()
	{
		if (!IsTriggered)
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			if (Singleton<GameManager>.Instance.GameStory == GameManager.Story.Silver && gameData.HasFlag("csc_sv") && gameData.HasFlag("tpj_sv") && gameData.HasFlag("dtd_sv") && gameData.HasFlag("wap_sv") && gameData.HasFlag("rct_sv") && gameData.HasFlag("aqa_sv") && gameData.HasFlag("kdv_sv") && gameData.HasFlag("flc_sv") && gameData.HasFlag("wvo_bz") && gameData.HasFlag(Game.StgSvAllClear) && gameData.HasFlag(Game.GotSvReward) && !gameData.HasFlag(Game.SigilOfAwakening))
			{
				CallEvent(UpgradeEvent);
			}
			else
			{
				CallEvent(Event);
			}
			ESPMark.Radius = 0f;
			for (int i = 0; i < FX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = FX[i].emission;
				emission.enabled = false;
			}
			StartCoroutine(OnBreak());
			CoolOffFX.Play();
			AttackObj.gameObject.SetActive(value: false);
			Audio.PlayOneShot(Clips[3], Audio.volume * 1.5f);
			IsTriggered = true;
		}
	}

	private IEnumerator OnAttack()
	{
		float StartTime = Time.time;
		float Scale = 17f;
		AttackObj.localScale = new Vector3(17f, 17f, 17f);
		SphereAttackFX.Play();
		Color AtkColor = new Vector4(1f, 16f / 85f, 0f, 1f);
		AttackRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetColor("_Color", AtkColor);
		AttackRenderer.SetPropertyBlock(PropBlock);
		SphereAttackRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetColor("_Color", AtkColor);
		SphereAttackRenderer.SetPropertyBlock(PropBlock);
		IsAttacking = true;
		while (Scale != AttackRange)
		{
			float num = Mathf.Clamp01((Time.time - StartTime) / 3f);
			Scale = Mathf.MoveTowards(Scale, AttackRange, Mathf.Clamp01((Time.time - StartTime) / 2f));
			AttackObj.localScale = new Vector3(Scale, Scale, Scale);
			if (num > 0.75f)
			{
				AtkColor.a = Mathf.Lerp(AtkColor.a, 0f, Time.deltaTime * 30f);
				AttackRenderer.GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_Color", AtkColor);
				AttackRenderer.SetPropertyBlock(PropBlock);
				SphereAttackRenderer.GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_Color", AtkColor);
				SphereAttackRenderer.SetPropertyBlock(PropBlock);
			}
			yield return new WaitForFixedUpdate();
		}
		IsAttacking = false;
	}

	private IEnumerator OnBreak()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		bool DoEffects = false;
		while (Timer <= 4f)
		{
			Timer = Time.time - StartTime;
			if (Timer > 0.35f && !DoEffects)
			{
				Audio.PlayOneShot(Clips[1], Audio.volume);
				DoEffects = true;
			}
			yield return new WaitForFixedUpdate();
		}
		Mesh.SetActive(value: false);
		Brk.SetActive(value: true);
		ESPMark.gameObject.SetActive(value: false);
		Camera.PlayShakeMotion(0f, 0.5f);
		Audio.PlayOneShot(Clips[2], Audio.volume);
		StartCoroutine(BreakDisappear());
	}

	private IEnumerator BreakDisappear()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		while (Timer <= 4f)
		{
			Timer = Time.time - StartTime;
			if (Timer > 0.5f)
			{
				for (int i = 0; i < BrkRenderers.Length; i++)
				{
					BrkRenderers[i].GetPropertyBlock(PropBlock);
					PropBlock.SetFloat("_DissAmount", Mathf.Lerp(PropBlock.GetFloat("_DissAmount"), 1f, Time.fixedDeltaTime));
					BrkRenderers[i].SetPropertyBlock(PropBlock);
				}
			}
			yield return new WaitForFixedUpdate();
		}
		Brk.SetActive(value: false);
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && IsAttacking && !IsTriggered && !Physics.Linecast(base.transform.position, player.transform.position, AttackBlock))
		{
			player.OnHurtEnter(2);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, AttackRange);
	}
}
