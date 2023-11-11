using System.Collections.Generic;
using UnityEngine;

public class Common_Laser : MonoBehaviour
{
	[Header("Framework")]
	public Vector3 Pair;

	public int Num;

	public float Interval;

	public float OnTime;

	public float OffTime;

	public float Range;

	public float Speed;

	public float RadRange;

	public float RadSpeed;

	[Header("Framework Settings")]
	public Common_Laser PairSettings;

	[Header("Prefab")]
	public GameObject LaserOutput;

	public BoxCollider Collider;

	public CapsuleCollider SingleCollider;

	public AudioClip[] Clips;

	public AudioSource Audio;

	public AudioSource AudioMove;

	internal List<Transform> OutputObjs = new List<Transform>();

	internal List<Transform> LaserObjs = new List<Transform>();

	internal List<Renderer> RenderObjs = new List<Renderer>();

	private MaterialPropertyBlock PropBlock;

	private Vector3 NeutralPosition;

	private Vector3 TopPosition;

	private bool IsTurnedOn;

	private bool SlowFade;

	private float StartTime;

	private float FadeInt;

	private float TimePassed;

	public void SetParameters(Vector3 _Pair, int _Num, float _Interval, float _OnTime, float _OffTime, float _Range, float _Speed, float _RadRange, float _RadSpeed)
	{
		Pair = _Pair;
		Num = _Num;
		Interval = _Interval;
		OnTime = _OnTime;
		OffTime = _OffTime;
		Range = _Range;
		Speed = _Speed;
		RadRange = _RadRange;
		RadSpeed = _RadSpeed;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
		OutputObjs = new List<Transform>();
		for (int i = 0; i < (PairSettings ? PairSettings.Num : Num); i++)
		{
			GameObject gameObject = Object.Instantiate(LaserOutput, base.transform.position + base.transform.up * (PairSettings ? PairSettings.Interval : Interval) * i, base.transform.rotation);
			OutputObjs.Add(gameObject.transform);
			gameObject.transform.SetParent(base.transform);
		}
		if (!PairSettings)
		{
			for (int j = 0; j < OutputObjs.Count; j++)
			{
				LaserObjs.Add(OutputObjs[j].GetChild(0));
				RenderObjs.Add(OutputObjs[j].GetChild(0).GetComponent<Renderer>());
			}
		}
	}

	private void Start()
	{
		if ((bool)PairSettings)
		{
			if ((PairSettings.OnTime != 0f && PairSettings.OffTime == 0f) || (PairSettings.OnTime == 0f && PairSettings.OffTime == 0f))
			{
				GateClose();
				PairSettings.GateClose();
				PairSettings.ManageLaserGlow();
			}
			else if ((PairSettings.OnTime == 0f && PairSettings.OffTime != 0f) || (PairSettings.OnTime != 0f && PairSettings.OffTime != 0f))
			{
				GateOpen();
				PairSettings.GateOpen();
				PairSettings.ManageLaserGlow();
			}
			float y = Vector3.Distance(PairSettings.OutputObjs[0].position, OutputObjs[0].position);
			float x = PairSettings.Interval * (float)(PairSettings.Num - 1);
			ManageColliders(Setup: true, new Vector2(x, y), Enabled: false);
			for (int i = 0; i < PairSettings.OutputObjs.Count; i++)
			{
				for (int j = 0; j < OutputObjs.Count; j++)
				{
					PairSettings.OutputObjs[i].GetChild(0).localScale = new Vector3(1f, 1f, Vector3.Distance(PairSettings.OutputObjs[i].position, OutputObjs[i].position) * 6.3f);
					PairSettings.OutputObjs[i].LookAt(OutputObjs[i].position);
					OutputObjs[i].LookAt(PairSettings.OutputObjs[i].position);
				}
			}
			StartTime = Time.time;
			if (Num < 2)
			{
				Audio.enabled = false;
			}
			if (PairSettings.Range != 0f)
			{
				NeutralPosition = base.transform.position;
				TopPosition = base.transform.position + base.transform.up * PairSettings.Range;
				AudioMove.enabled = true;
			}
		}
		else
		{
			Audio.enabled = false;
			if (Range != 0f)
			{
				NeutralPosition = base.transform.position;
				TopPosition = base.transform.position + base.transform.up * Range;
			}
		}
	}

	private void Update()
	{
		if (!PairSettings)
		{
			if (SlowFade)
			{
				ManageLaserGlow();
			}
			return;
		}
		if (IsTurnedOn && !Audio.isPlaying && Num > 1)
		{
			Audio.clip = Clips[1];
			Audio.loop = true;
			Audio.Play();
		}
		if (PairSettings.OnTime != 0f && PairSettings.OffTime != 0f)
		{
			if (IsTurnedOn && Time.time - StartTime > PairSettings.OnTime)
			{
				StartTime = Time.time;
				GateOpen();
				PairSettings.GateOpen();
			}
			else if (!IsTurnedOn && Time.time - StartTime > PairSettings.OffTime)
			{
				StartTime = Time.time;
				GateClose();
				PairSettings.GateClose();
			}
		}
		if (PairSettings.Range != 0f)
		{
			TimePassed += Time.deltaTime;
			base.transform.position = Vector3.MoveTowards(NeutralPosition, TopPosition, Mathf.PingPong(Time.time * PairSettings.Speed, PairSettings.Range));
			PairSettings.transform.position = Vector3.MoveTowards(PairSettings.NeutralPosition, PairSettings.TopPosition, Mathf.PingPong(Time.time * PairSettings.Speed, PairSettings.Range));
		}
	}

	public void ManageLaserGlow(float Int = 0f)
	{
		for (int i = 0; i < RenderObjs.Count; i++)
		{
			RenderObjs[i].GetPropertyBlock(PropBlock);
			if (!SlowFade)
			{
				PropBlock.SetFloat("_Intensity", IsTurnedOn ? 1f : 0f);
			}
			else
			{
				FadeInt = Mathf.Lerp(FadeInt, 0.25f, Time.deltaTime / 50f);
				PropBlock.SetFloat("_Intensity", Mathf.Lerp(1.25f, 0.25f, Mathf.Abs(Mathf.Cos(Time.time * Random.Range(10f, 20f)))) * FadeInt);
			}
			RenderObjs[i].SetPropertyBlock(PropBlock);
		}
	}

	public void SlowTurnOff()
	{
		if (IsTurnedOn)
		{
			SlowFade = true;
			FadeInt = 1f;
		}
	}

	public void GateOpen()
	{
		ManageColliders(Setup: false, Vector2.zero, Enabled: false);
		IsTurnedOn = false;
		SlowFade = false;
		if ((bool)PairSettings && Num > 1)
		{
			Audio.Stop();
			Audio.clip = Clips[2];
			Audio.loop = false;
			Audio.Play();
		}
		ManageLaserGlow();
	}

	public void GateClose()
	{
		ManageColliders(Setup: false, Vector2.zero, Enabled: true);
		IsTurnedOn = true;
		if ((bool)PairSettings && Num > 1)
		{
			Audio.Stop();
			Audio.clip = Clips[0];
			Audio.loop = false;
			Audio.Play();
		}
		ManageLaserGlow();
	}

	private void ManageColliders(bool Setup, Vector2 SetupParams, bool Enabled)
	{
		if (Num == 1)
		{
			if (Setup)
			{
				SingleCollider.radius = 0.2f;
				SingleCollider.height = SetupParams.y + 0.25f;
				SingleCollider.transform.rotation = Quaternion.LookRotation((PairSettings.transform.position - base.transform.position).normalized);
				SingleCollider.transform.position = SingleCollider.transform.position + SingleCollider.transform.forward * (SetupParams.y * 0.5f) + SingleCollider.transform.up * (SetupParams.x * 0.5f);
			}
			else
			{
				SingleCollider.enabled = Enabled;
			}
		}
		else if (Setup)
		{
			Collider.size = new Vector3(0.45f, 0.25f + SetupParams.x, SetupParams.y);
			Collider.transform.rotation = Quaternion.LookRotation((PairSettings.transform.position - base.transform.position).normalized);
			Collider.transform.position = Collider.transform.position + Collider.transform.forward * (SetupParams.y * 0.5f) + Collider.transform.up * (SetupParams.x * 0.5f);
		}
		else
		{
			Collider.enabled = Enabled;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Pair != Vector3.zero)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, Pair);
			Gizmos.DrawWireSphere(Pair, 0.5f);
			if ((bool)PairSettings)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.transform.position, PairSettings.transform.position);
				Gizmos.DrawWireSphere(PairSettings.transform.position, 0.75f);
			}
		}
		if (Range != 0f)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * Range);
			Gizmos.DrawWireSphere(base.transform.position + base.transform.up * Range, 0.5f);
		}
	}
}
