using UnityEngine;

public class Crater : ObjectBase
{
	[Header("Framework")]
	public int AttackPower;

	public float WaitTime;

	public float Time;

	[Header("Prefab")]
	public LayerMask AttackMask;

	public Renderer Renderer;

	public Collider Collider;

	public AudioSource Audio;

	public ParticleSystem[] FX;

	private bool Wait;

	private bool Fire;

	private float PingPongInt;

	private float Timer;

	private MaterialPropertyBlock PropBlock;

	public void SetParameters(int _AttackPower, float _WaitTime, float _Time)
	{
		AttackPower = _AttackPower;
		WaitTime = _WaitTime;
		Time = _Time;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		Timer = 0f;
		Wait = true;
	}

	private void Update()
	{
		Timer += UnityEngine.Time.deltaTime;
		if (Wait && Timer > WaitTime)
		{
			Fire = true;
			Audio.Play();
			Timer = 0f;
			Wait = false;
		}
		if (Fire && Timer > Time)
		{
			Wait = true;
			Timer = 0f;
			Fire = false;
		}
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = Fire;
		}
		Renderer.GetPropertyBlock(PropBlock);
		if (!Fire)
		{
			PropBlock.SetFloat("_FireInt", Mathf.MoveTowards(PropBlock.GetFloat("_FireInt"), (Timer > WaitTime - 1f) ? 3f : 0f, UnityEngine.Time.deltaTime * 3f));
		}
		else
		{
			AttackSphere_Dir(base.transform.position + base.transform.up * 0.5f);
			PropBlock.SetFloat("_FireInt", 3f);
		}
		Renderer.SetPropertyBlock(PropBlock);
		Collider.enabled = Fire;
		PingPongInt = Mathf.Lerp(2f, 4f, Mathf.Abs(Mathf.Cos(UnityEngine.Time.time * 15f)));
	}

	public bool AttackSphere_Dir(Vector3 Position)
	{
		Collider[] array = Physics.OverlapSphere(Position, 2.5f, AttackMask);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
				if (vector == Vector3.zero)
				{
					vector = base.transform.forward.MakePlanar();
				}
				Vector3 force = (vector + Vector3.up * Random.Range(0.1f, 0.25f)).normalized * 5f;
				HitInfo value = new HitInfo(base.transform, force, AttackPower);
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	private void OnTriggerStay(Collider collider)
	{
		if (!Fire && (bool)GetPlayer(collider) && Timer > 0.5f && Timer < WaitTime - 1f)
		{
			Timer = WaitTime - 1f;
		}
	}
}
