using UnityEngine;

public class EffectsBase : MonoBehaviour
{
	[Header("Effects - Base")]
	public PlayerManager PM;

	public Transform HipBone;

	public GameObject[] GrindContactFX;

	public GameObject[] GrindFX;

	public ParticleSystem WaterRipples;

	public GameObject WaterSplashFX;

	[Header("Sinking - Base")]
	public GameObject JumpSandParticle;

	public GameObject SinkSandParticle;

	public GameObject SinkLavaParticle;

	[Header("Optional - Base")]
	public Transform[] BreathPoints;

	public GameObject BreathFX;

	public ParticleSystem[] JumpFX;

	public ParticleSystem[] JumpSA2FX;

	public Renderer JumpballRenderer;

	internal GameObject RailParticles;

	internal GameObject ActiveSinkSandParticle;

	internal GameObject ActiveSinkLavaParticle;

	internal bool DashPadRoll;

	internal bool UseJumpFX;

	private RaycastHit WaterHit;

	private Vector3[] JumpSA2Pos;

	private bool JumpStartFX;

	private bool SpawnWaterSplash;

	private float JumpballBlink;

	public virtual void Start()
	{
		UseJumpFX = JumpFX != null;
		if (UseJumpFX && Singleton<Settings>.Instance.settings.SpinEffect == 2)
		{
			JumpSA2Pos = new Vector3[3];
		}
		if (BreathPoints != null && (bool)BreathFX && PM.Base.StageManager._Stage == StageManager.Stage.wap)
		{
			CreateBreathFX();
		}
	}

	public virtual void Update()
	{
		if (JumpFX != null)
		{
			UpdateJumpFX();
		}
		if (PM.Base.GetState() == "Result" && (bool)PM.Base.ShieldObject)
		{
			PM.Base.ShieldObject.transform.localPosition = new Vector3(HipBone.localPosition.x * (PM.Base.GetPrefab("sonic_fast") ? 0f : 0.75f), HipBone.localPosition.y - 0.25f, HipBone.localPosition.z);
		}
		ParticleSystem.EmissionModule emission = WaterRipples.emission;
		if (Physics.Linecast(base.transform.position + base.transform.up * 0.75f, base.transform.position + base.transform.up * -0.25f, out WaterHit, PM.Base.Trigger_Mask.value, QueryTriggerInteraction.Collide))
		{
			if (WaterHit.transform.gameObject.tag == "Water")
			{
				emission.enabled = true;
				WaterRipples.transform.position = WaterHit.point;
				WaterRipples.transform.rotation = Quaternion.LookRotation(WaterHit.normal) * Quaternion.Euler(-90f, 0f, 0f);
				if (!SpawnWaterSplash && !PM.Base.IsGrounded())
				{
					SpawnWaterSplash = true;
					Object.Instantiate(WaterSplashFX, WaterHit.point, Quaternion.identity);
				}
			}
		}
		else
		{
			emission.enabled = false;
			SpawnWaterSplash = false;
		}
		if (ActiveSinkLavaParticle != null != PM.Base.IsSinking)
		{
			if (PM.Base.IsSinking && PM.Base.ColName == "2820000d")
			{
				ActiveSinkLavaParticle = Object.Instantiate(SinkLavaParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
				ActiveSinkLavaParticle.transform.SetParent(base.transform);
			}
			else
			{
				RemoveSinkParticles("Lava");
			}
		}
		if (ActiveSinkSandParticle != null != PM.Base.IsSinking && !PM.Base.IsDead)
		{
			if (PM.Base.IsSinking && PM.Base.ColName == "40000009")
			{
				ActiveSinkSandParticle = Object.Instantiate(SinkSandParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
				ActiveSinkSandParticle.transform.SetParent(base.transform);
			}
			else
			{
				RemoveSinkParticles("Sand");
			}
		}
	}

	private void FixedUpdate()
	{
		if (UseJumpFX && Singleton<Settings>.Instance.settings.SpinEffect == 2)
		{
			JumpSA2Pos[0] = Vector3.Lerp(JumpSA2Pos[0], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 25f);
			JumpSA2Pos[1] = Vector3.Lerp(JumpSA2Pos[1], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 19f);
			JumpSA2Pos[2] = Vector3.Lerp(JumpSA2Pos[2], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 15f);
			for (int i = 0; i < JumpSA2FX.Length; i++)
			{
				JumpSA2FX[i].transform.position = JumpSA2Pos[i];
			}
		}
	}

	internal void RemoveSinkParticles(string Type)
	{
		if (Type == "Lava" && (bool)ActiveSinkLavaParticle)
		{
			ParticleSystem[] componentsInChildren = ActiveSinkLavaParticle.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
			}
			ActiveSinkLavaParticle.transform.SetParent(null);
			Object.Destroy(ActiveSinkLavaParticle, 3f);
			ActiveSinkLavaParticle = null;
		}
		if (Type == "Sand" && (bool)ActiveSinkSandParticle)
		{
			ParticleSystem[] componentsInChildren2 = ActiveSinkSandParticle.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].Stop();
			}
			ActiveSinkSandParticle.transform.SetParent(null);
			Object.Destroy(ActiveSinkSandParticle, 3f);
			ActiveSinkSandParticle = null;
			if (PM.Base.SinkPosition > 0.5f)
			{
				Object.Instantiate(JumpSandParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
			}
		}
	}

	public void UpdateJumpFX()
	{
		if (Singleton<Settings>.Instance.settings.SpinEffect != 2)
		{
			for (int i = 0; i < JumpFX.Length; i++)
			{
				ParticleSystem.EmissionModule emission = JumpFX[i].emission;
				emission.enabled = UseJumpFX && (PM.Base.GetState() == "Jump" || PM.Base.GetState() == "BallFall") && PM.Base.JumpAnimation == 1 && Singleton<Settings>.Instance.settings.SpinEffect != 1;
				if (UseJumpFX && (PM.Base.GetState() == "Jump" || PM.Base.GetState() == "BallFall") && Singleton<Settings>.Instance.settings.SpinEffect != 1)
				{
					if (PM.Base.JumpAnimation == 1)
					{
						if (!JumpStartFX)
						{
							JumpStartFX = true;
							JumpFX[i].Play();
						}
					}
					else if (PM.Base.JumpAnimation == 2)
					{
						JumpStartFX = false;
						JumpFX[i].Stop();
					}
				}
				else
				{
					JumpStartFX = false;
					JumpFX[i].Stop();
				}
			}
		}
		else
		{
			for (int j = 0; j < JumpSA2FX.Length; j++)
			{
				ParticleSystem.EmissionModule emission2 = JumpSA2FX[j].emission;
				emission2.enabled = UseJumpFX && ((PM.Base.GetPrefab("silver") && PM.silver.HasLotusOfResilience) || !PM.Base.GetPrefab("silver")) && (PM.Base.GetState() == "Jump" || PM.Base.GetState() == "TrickJump" || PM.Base.GetState() == "AfterHoming" || PM.Base.GetState() == "BallFall");
			}
		}
	}

	public void UpdateJumpBallFX(bool Conditions, Renderer Mesh = null)
	{
		if (!JumpballRenderer || Singleton<Settings>.Instance.settings.SpinEffect != 1)
		{
			return;
		}
		if (Conditions)
		{
			JumpballBlink += Time.deltaTime * 19f;
			if (JumpballBlink >= 1f)
			{
				JumpballBlink = 0f;
			}
			if ((bool)Mesh)
			{
				Mesh.enabled = JumpballBlink <= 0.5f;
			}
			else
			{
				JumpballRenderer.enabled = JumpballBlink <= 0.5f;
			}
		}
		else if ((bool)Mesh)
		{
			if (Mesh.enabled)
			{
				Mesh.enabled = false;
			}
		}
		else if (JumpballRenderer.enabled)
		{
			JumpballRenderer.enabled = false;
		}
	}

	public void CreateBreathFX()
	{
		for (int i = 0; i < BreathPoints.Length; i++)
		{
			ParticleSystem component = Object.Instantiate(BreathFX, BreathPoints[i].position, BreathPoints[i].rotation).GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			main.duration = Random.Range(2.5f, 3.5f);
			component.transform.SetParent(BreathPoints[i]);
			component.Play();
		}
	}

	public void CreateRailFX(int RailType)
	{
		DestroyRailFX();
		RailParticles = Object.Instantiate(GrindFX[RailType], base.transform.position + base.transform.up * -0.25f, base.transform.rotation);
		RailParticles.transform.SetParent(base.transform);
		RailParticles.GetComponent<GrindFX>().Player = PM.Base;
		CreateRailContactFX(RailType);
	}

	public void CreateRailContactFX(int RailType)
	{
		Object.Instantiate(GrindContactFX[RailType], base.transform.position + base.transform.up * -0.25f, base.transform.rotation);
	}

	public void DestroyRailFX()
	{
		if ((bool)RailParticles)
		{
			RailParticles.GetComponent<GrindFX>().StopRailFX();
			RailParticles.transform.SetParent(null);
			Object.Destroy(RailParticles, 1f);
			RailParticles = null;
		}
	}
}
