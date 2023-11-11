using UnityEngine;

public class Common_Thorn : MonoBehaviour
{
	public enum Mode
	{
		AlwaysExposed = 0,
		Alternative = 1,
		AlwaysHidden = 2
	}

	[Header("Framework")]
	public Mode Type;

	public float OutTime;

	public float InTime;

	[Header("Prefab")]
	public Animator Animator;

	private bool SwitchUp;

	private float StartTime;

	public void SetParameters(int _Type, float _OutTime, float _InTime)
	{
		Type = (Mode)(_Type - 1);
		OutTime = _OutTime;
		InTime = _InTime;
	}

	private void Start()
	{
		if (Type == Mode.Alternative)
		{
			StartTime = Time.time;
		}
		else if (Type == Mode.AlwaysExposed)
		{
			Animator.SetTrigger("On Show");
		}
		else
		{
			Animator.SetTrigger("On Hide");
		}
	}

	private void Update()
	{
		if (Type == Mode.Alternative)
		{
			if (!SwitchUp && Time.time - StartTime > OutTime)
			{
				StartTime = Time.time;
				Animator.SetTrigger("On Show");
				SwitchUp = true;
			}
			else if (SwitchUp && Time.time - StartTime > InTime)
			{
				StartTime = Time.time;
				Animator.SetTrigger("On Hide");
				SwitchUp = false;
			}
		}
	}
}
