using UnityEngine;

public class ParticleSystemPlayer : MonoBehaviour
{
	public enum Mode
	{
		LoopInterval = 0
	}

	[Header("Framework")]
	public Mode Behaviour;

	public ParticleSystem[] FX;

	[Header("Loop Interval")]
	public float[] LoopTimeRanges;

	[Header("Optional")]
	public AudioSource Sound;

	public Vector2 PitchRanges;

	private float StartTime;

	private int Index;

	private void Start()
	{
		if (Behaviour == Mode.LoopInterval)
		{
			StartTime = Time.time;
		}
	}

	private void Update()
	{
		if (Behaviour != 0 || !(Time.time - StartTime > LoopTimeRanges[Index]))
		{
			return;
		}
		StartTime = Time.time;
		Index = Random.Range(0, LoopTimeRanges.Length);
		for (int i = 0; i < FX.Length; i++)
		{
			FX[i].Stop();
			FX[i].Play();
		}
		if ((bool)Sound)
		{
			if (PitchRanges != Vector2.zero)
			{
				Sound.pitch = Random.Range(PitchRanges.x, PitchRanges.y);
			}
			Sound.Play();
		}
	}
}
