using UnityEngine;

public class Common_Switch : EventStation
{
	public enum Mode
	{
		ForeverOn = 0,
		Stay = 1,
		Timed = 2,
		Switchable = 3
	}

	[Header("Framework")]
	public Mode Type;

	public string EventColli;

	public string Event_Off;

	public float Time;

	public Vector3 Target;

	[Header("Prefab")]
	public Renderer Renderer;

	public AudioSource Audio;

	public AudioClip[] Sounds;

	public ParticleSystem SwitchFX;

	public GameObject HomingTarget;

	public Transform Orb;

	internal bool IsActivated;

	internal bool IsCaged;

	private float Timer;

	private MaterialPropertyBlock PropBlock;

	public void SetParameters(int _Type, string _EventColli, string _Event_Off, float _Time, Vector3 _Target)
	{
		Type = (Mode)(_Type - 1);
		EventColli = _EventColli;
		Event_Off = _Event_Off;
		Time = _Time;
		Target = _Target;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Update()
	{
		Renderer.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Intensity", IsActivated ? 1f : 0f);
		Renderer.SetPropertyBlock(PropBlock);
		Orb.Rotate(0f, -120f * UnityEngine.Time.deltaTime, 0f);
		if (Type == Mode.Timed && IsActivated && UnityEngine.Time.time - Timer > Time)
		{
			IsActivated = false;
			Audio.PlayOneShot(Sounds[1], Audio.volume);
			PlayFX();
		}
	}

	private void PlayFX()
	{
		SwitchFX.Play();
	}

	private void TriggerSwitch()
	{
		if ((Type == Mode.ForeverOn && !IsActivated) || Type != 0)
		{
			PlayFX();
		}
		if (Type == Mode.ForeverOn)
		{
			HomingTarget.tag = "Untagged";
		}
		if (Type == Mode.Timed)
		{
			Timer = UnityEngine.Time.time;
		}
		if (!IsActivated)
		{
			IsActivated = true;
			CallEvent(EventColli);
			Audio.PlayOneShot(Sounds[0], Audio.volume);
		}
		else if (Type == Mode.Switchable)
		{
			IsActivated = false;
			CallEvent(Event_Off);
			Audio.PlayOneShot(Sounds[1], Audio.volume);
		}
	}

	public void SetToActiveForeverMode()
	{
		Type = Mode.ForeverOn;
		SetActivation(Result: true);
	}

	public void SetActivation(bool Result)
	{
		IsActivated = Result;
		if (Type == Mode.ForeverOn)
		{
			HomingTarget.tag = (Result ? "Untagged" : "HomingTarget");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if ((bool)GetPlayer(collision.transform) && !IsCaged)
		{
			TriggerSwitch();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (!(collision.gameObject.tag != "Player") && !IsCaged && Type == Mode.Stay && IsActivated)
		{
			IsActivated = false;
			CallEvent(Event_Off);
			Audio.PlayOneShot(Sounds[1], Audio.volume);
			PlayFX();
		}
	}

	private void OnSwitch()
	{
		if (Type != Mode.Stay && !IsCaged)
		{
			TriggerSwitch();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Target != Vector3.zero)
		{
			Gizmos.DrawLine(base.transform.position, Target);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(Target, 0.25f);
		}
	}
}
