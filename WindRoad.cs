using UnityEngine;

public class WindRoad : MonoBehaviour
{
	[Header("Framework")]
	public float AppearTime;

	public float DisappearTime;

	public string Path;

	[Header("Prefab")]
	public StateMachine StateMachine;

	public ParticleSystem WindRoadLeavesFX;

	public ParticleSystem WindRoadWindFX;

	public MeshCollider Collider;

	public RailSystem Road;

	internal bool Enabled;

	private ParticleSystem.MainModule windmain;

	private Gradient WindGradient;

	private ParticleSystem.ColorOverLifetimeModule WindParticleColor;

	private GameObject Targets;

	private Material Material;

	private Color FillColor;

	private float WindSpeed;

	private float StateStartTime;

	private float Timer;

	private float Alpha;

	public void SetParameters(float _AppearTime, float _DisappearTime, string _Path)
	{
		AppearTime = _AppearTime;
		DisappearTime = _DisappearTime;
		Path = _Path;
		base.transform.GetComponent<RailSystem>().bezierCurve = GameObject.Find(Path).GetComponent<BezierCurve>();
	}

	private void Awake()
	{
		Targets = base.gameObject.FindInChildren("targets");
		Material = base.transform.GetComponentInChildren<Renderer>().material;
		CreateParticlesSystem();
		StateDisabledStart();
		StateMachine.Initialize(StateDisabled);
	}

	private void CreateParticlesSystem()
	{
		WindRoadLeavesFX.transform.position = Road.bezierCurve.GetPosition(0f);
		WindRoadLeavesFX.transform.rotation = Quaternion.identity;
		WindRoadWindFX.transform.position = Road.bezierCurve.GetPosition(0f);
		WindRoadWindFX.transform.rotation = Quaternion.identity;
		Vector3[] array = Road.TangentArray(2f);
		float num = 5f;
		FillColor = new Color(1f, 1f, 1f, 1f);
		ParticleSystem.MainModule main = WindRoadLeavesFX.main;
		float startSpeedMultiplier = main.startSpeedMultiplier;
		main.startSpeedMultiplier = 0f;
		float t = (main.startLifetimeMultiplier = (Road.bezierCurve.Length() + num) / startSpeedMultiplier);
		ParticleSystem.ColorOverLifetimeModule colorOverLifetime = WindRoadLeavesFX.colorOverLifetime;
		colorOverLifetime.enabled = true;
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(Color.white, 0f),
			new GradientColorKey(Color.white, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f - 1f / startSpeedMultiplier),
			new GradientAlphaKey(0f, 1f)
		});
		colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
		ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = WindRoadLeavesFX.velocityOverLifetime;
		velocityOverLifetime.enabled = true;
		velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
		AnimationCurve animationCurve = new AnimationCurve();
		AnimationCurve animationCurve2 = new AnimationCurve();
		AnimationCurve animationCurve3 = new AnimationCurve();
		for (int i = 0; i < array.Length; i++)
		{
			float time = (float)i / ((float)array.Length - 1f);
			Vector3 vector = array[i];
			animationCurve.AddKey(time, vector.x);
			animationCurve2.AddKey(time, vector.y);
			animationCurve3.AddKey(time, vector.z);
		}
		velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(startSpeedMultiplier, animationCurve);
		velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(startSpeedMultiplier, animationCurve2);
		velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(startSpeedMultiplier, animationCurve3);
		WindRoadLeavesFX.Simulate(t);
		WindRoadLeavesFX.gameObject.SetActive(value: false);
		windmain = WindRoadWindFX.main;
		WindSpeed = windmain.startSpeedMultiplier;
		windmain.startSpeedMultiplier = 0f;
		float startLifetimeMultiplier = (Road.bezierCurve.Length() + num) / WindSpeed;
		windmain.startLifetimeMultiplier = startLifetimeMultiplier;
		WindParticleColor = WindRoadWindFX.colorOverLifetime;
		WindParticleColor.enabled = true;
		WindGradient = new Gradient();
		WindGradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(Color.white, 0f),
			new GradientColorKey(Color.white, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f - 1f / WindSpeed),
			new GradientAlphaKey(0f, 1f)
		});
		WindParticleColor.color = new ParticleSystem.MinMaxGradient(WindGradient);
		ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime2 = WindRoadWindFX.velocityOverLifetime;
		velocityOverLifetime2.enabled = true;
		velocityOverLifetime2.space = ParticleSystemSimulationSpace.World;
		AnimationCurve animationCurve4 = new AnimationCurve();
		AnimationCurve animationCurve5 = new AnimationCurve();
		AnimationCurve animationCurve6 = new AnimationCurve();
		for (int j = 0; j < array.Length; j++)
		{
			float time2 = (float)j / ((float)array.Length - 1f);
			Vector3 vector2 = array[j];
			animationCurve4.AddKey(time2, vector2.x);
			animationCurve5.AddKey(time2, vector2.y);
			animationCurve6.AddKey(time2, vector2.z);
		}
		velocityOverLifetime2.x = new ParticleSystem.MinMaxCurve(WindSpeed, animationCurve4);
		velocityOverLifetime2.y = new ParticleSystem.MinMaxCurve(WindSpeed, animationCurve5);
		velocityOverLifetime2.z = new ParticleSystem.MinMaxCurve(WindSpeed, animationCurve6);
	}

	public void OnPathEnable()
	{
		if (!Enabled)
		{
			StateMachine.ChangeState(StateFadeIn);
		}
		Timer = Mathf.Clamp(Timer + AppearTime, 0f, DisappearTime);
	}

	public void OnPathDisable()
	{
		if (Enabled)
		{
			StateMachine.ChangeState(StateFadeOut);
		}
		Timer = 0f;
		Targets.SetActive(value: false);
	}

	private void StateFadeInStart()
	{
		StateStartTime = Time.time;
		WindRoadLeavesFX.gameObject.SetActive(value: true);
		Enabled = true;
		Collider.enabled = true;
		Targets.SetActive(value: true);
	}

	private void StateFadeIn()
	{
		Alpha = Mathf.Lerp(0f, 1f, (Time.time - StateStartTime) * 2f);
		if (Alpha >= 1f)
		{
			StateMachine.ChangeState(StateEnabled);
		}
	}

	private void StateFadeInEnd()
	{
	}

	private void StateFadeOutStart()
	{
		StateStartTime = Time.time;
		WindRoadLeavesFX.gameObject.SetActive(value: false);
	}

	private void StateFadeOut()
	{
		Alpha = Mathf.Lerp(1f, 0f, Time.time - StateStartTime);
		if (Alpha <= 0f)
		{
			StateMachine.ChangeState(StateDisabled);
		}
	}

	private void StateFadeOutEnd()
	{
	}

	private void StateEnabledStart()
	{
	}

	private void StateEnabled()
	{
		Timer -= Time.fixedDeltaTime;
		if (Timer <= 0f)
		{
			OnPathDisable();
			Timer = 0f;
		}
	}

	private void StateEnabledEnd()
	{
	}

	private void StateDisabledStart()
	{
		Enabled = false;
		Collider.enabled = false;
		Targets.SetActive(value: false);
	}

	private void StateDisabled()
	{
	}

	private void StateDisabledEnd()
	{
	}

	private void FixedUpdate()
	{
		StateMachine.UpdateStateMachine();
	}

	private void Update()
	{
		WindGradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(FillColor, 0f),
			new GradientColorKey(FillColor, 1f)
		}, new GradientAlphaKey[3]
		{
			new GradientAlphaKey(Alpha, 0f),
			new GradientAlphaKey(Alpha, 1f - 1f / WindSpeed),
			new GradientAlphaKey(0f, 1f)
		});
		WindParticleColor.color = new ParticleSystem.MinMaxGradient(WindGradient);
		Material.SetColor("_TintColor", new Color(1f, 1f, 1f, Alpha));
	}
}
