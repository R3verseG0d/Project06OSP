using UnityEngine;

public class InclinedBridge : ObjectBase
{
	[Header("Framework")]
	public float Time1;

	public float Time2;

	[Header("Prefab")]
	public Animator Animator;

	public Rigidbody RBody;

	public Transform BreakObj;

	public AudioSource Audio;

	public AudioSource AudioLoop;

	public AudioSource AudioEnd;

	public GameObject Collapse1FX;

	public GameObject Collapse2FX;

	public GameObject Collapse3FX;

	public bool DestroyObj;

	private bool Collapsing1;

	private bool Collapsing2;

	private float StartTime1;

	private float StartTime2;

	private bool FirstCollapse;

	private bool SecondCollapse;

	public void SetParameters(float _Time1, float _Time2)
	{
		Time1 = _Time1;
		Time2 = _Time2;
	}

	private void Update()
	{
		if (Collapsing1 && !FirstCollapse && Time.time - StartTime1 > Time1)
		{
			Animator.speed = 1f;
			if ((bool)Collapse2FX)
			{
				Collapse2FX.SetActive(value: true);
			}
			Audio.PlayOneShot(Audio.clip, Audio.volume);
			Collapsing1 = false;
			FirstCollapse = true;
		}
		if (Collapsing2 && FirstCollapse && !SecondCollapse && Time.time - StartTime2 > Time2)
		{
			Animator.speed = 1f;
			if ((bool)Collapse3FX)
			{
				Collapse3FX.SetActive(value: true);
			}
			Audio.PlayOneShot(Audio.clip, Audio.volume);
			Collapsing2 = false;
			SecondCollapse = true;
		}
	}

	private void Collapse()
	{
		Audio.PlayOneShot(Audio.clip, Audio.volume);
		Animator.SetTrigger("Player Collide");
		if ((bool)Collapse1FX)
		{
			Collapse1FX.SetActive(value: true);
		}
		AudioLoop.Play();
	}

	private void OnEventSignal()
	{
		Collapse();
	}

	private void PauseAnimation()
	{
		if (!FirstCollapse)
		{
			Collapsing1 = true;
			StartTime1 = Time.time;
		}
		if (FirstCollapse && !SecondCollapse)
		{
			Collapsing2 = true;
			StartTime2 = Time.time;
		}
		Animator.speed = 0f;
	}

	private void BreakBridge()
	{
		if ((bool)BreakObj)
		{
			BreakObj.SetParent(null);
		}
		else
		{
			base.transform.SetParent(null);
		}
		RBody.isKinematic = false;
		Animator.enabled = false;
		AudioEnd.Play();
		if (DestroyObj)
		{
			Object.Destroy(base.gameObject, 15f);
		}
	}
}
