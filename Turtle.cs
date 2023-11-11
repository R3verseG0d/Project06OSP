using UnityEngine;

public class Turtle : ObjectBase
{
	[Header("Framework")]
	public float RedownTime;

	public float DamageTime;

	public float TurnTime;

	public float SpeedSwim;

	public bool DoReverse;

	public bool DoLoop;

	public bool WaitFruit;

	public bool WaitRide;

	public string PathName;

	[Header("Prefab")]
	public Animator Animator;

	public ParticleSystem WaterFX;

	public ParticleSystem EmergeFX;

	public AudioSource WaterAudio;

	public AudioSource Audio;

	public AudioClip[] Clips;

	private BezierCurve Curve;

	private bool Resurface;

	private bool Submerge;

	private bool WaitToEmerge;

	private bool StandingOnShell;

	private bool Hurting;

	private bool NoticedFruit;

	private bool PlayFX;

	private bool StartProgress;

	private float StartTime;

	private float SubmergeStartTime;

	private float Progress;

	public void SetParameters(float _RedownTime, float _DamageTime, float _TurnTime, float _SpeedSwim, bool _DoReverse, bool _DoLoop, bool _WaitFruit, bool _WaitRide, string _PathName)
	{
		RedownTime = _RedownTime;
		DamageTime = _DamageTime;
		TurnTime = _TurnTime;
		SpeedSwim = _SpeedSwim;
		DoReverse = _DoReverse;
		DoLoop = _DoLoop;
		WaitFruit = _WaitFruit;
		WaitRide = _WaitRide;
		PathName = _PathName;
	}

	private void Start()
	{
		Animator.SetTrigger(WaitFruit ? "On Wait Fruit" : "On Wait Ride");
		if (!WaitFruit)
		{
			StartTime = Time.time;
		}
		PlayFX = !WaitFruit;
		if (PathName != "")
		{
			Curve = GameObject.Find(PathName).GetComponent<BezierCurve>();
			if (!WaitRide)
			{
				StartProgress = true;
			}
		}
	}

	private void Update()
	{
		if (Resurface && Time.time - StartTime > 5f)
		{
			StartTime = Time.time;
			Resurface = false;
			NoticedFruit = true;
		}
		if (Hurting && Time.time - StartTime > 5f)
		{
			Hurting = false;
		}
		if (Submerge && Time.time - StartTime > 5f)
		{
			StartTime = Time.time;
			WaitToEmerge = true;
			Submerge = false;
		}
		if (Submerge && PlayFX && Time.time - StartTime > 3f)
		{
			PlayFX = false;
		}
		if (WaitToEmerge)
		{
			OnEmerge();
			WaitToEmerge = false;
		}
		if (PathName == "")
		{
			if (!Submerge && ((WaitFruit && NoticedFruit) || !WaitFruit) && Time.time - StartTime > RedownTime)
			{
				OnSubmerge();
			}
			if (StandingOnShell && !Submerge && Time.time - StartTime > DamageTime)
			{
				OnSubmerge();
			}
		}
		else if (StartProgress)
		{
			Progress += SpeedSwim / Curve.Length() * Time.deltaTime;
			if (Progress > 1f)
			{
				Progress = ((!DoLoop) ? 1f : 0f);
			}
			base.transform.position = Curve.GetPosition(Progress);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Curve.GetTangent(Progress)), Time.deltaTime * TurnTime);
		}
		ParticleSystem.EmissionModule emission = WaterFX.emission;
		emission.enabled = PlayFX;
		WaterAudio.volume = Mathf.Lerp(WaterAudio.volume, PlayFX ? 1f : 0f, Time.deltaTime * 2f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((bool)GetPlayer(collision.transform) && !(PathName == "") && WaitRide && !StartProgress)
		{
			Animator.SetTrigger("On Swim");
			StartProgress = true;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if ((bool)GetPlayer(collision.transform) && !Resurface && !Submerge && !Hurting && !(PathName != "") && !StandingOnShell)
		{
			StandingOnShell = true;
			StartTime = Time.time;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (PathName == "" && collision.gameObject.tag == "Player")
		{
			StandingOnShell = false;
		}
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!Hurting && !Resurface && !Submerge && !(PathName != ""))
		{
			Animator.SetTrigger("On Damage");
			StartTime = Time.time;
			Hurting = true;
		}
	}

	public void OnEmerge()
	{
		Resurface = true;
		PlayFX = true;
		StartTime = Time.time;
		Animator.SetTrigger("On Resurface");
		Audio.PlayOneShot(Clips[0], Audio.volume);
		Invoke("PlayEmergeFX", 1f);
	}

	private void PlayEmergeFX()
	{
		EmergeFX.Play();
	}

	private void OnSubmerge()
	{
		Submerge = true;
		StartTime = Time.time;
		Animator.SetTrigger("On Submerge");
		Audio.PlayOneShot(Clips[1], Audio.volume);
	}
}
