using UnityEngine;

public class FlameGuide : MonoBehaviour
{
	public enum Type
	{
		Single = 0,
		Sequence = 1
	}

	[Header("Prefab")]
	public Type GuideType;

	public ParticleSystem[] FXs;

	internal float Speed;

	internal string Spline;

	private BezierCurve Curve;

	private bool Finished;

	private float Progress;

	private float StartTime;

	private void Start()
	{
		Curve = GameObject.Find(Spline).GetComponent<BezierCurve>();
		StartTime = Time.time;
	}

	private void Update()
	{
		switch (GuideType)
		{
		case Type.Single:
			if (Progress > 1f && !Finished)
			{
				for (int i = 0; i < FXs.Length; i++)
				{
					ParticleSystem.EmissionModule emission = FXs[i].emission;
					emission.enabled = false;
				}
				Object.Destroy(base.gameObject, 1f);
				Finished = true;
				Progress = 1f;
			}
			else
			{
				Progress += Speed / Curve.Length() * Time.deltaTime;
			}
			break;
		case Type.Sequence:
			Progress = Mathf.Lerp(0f, 1f, Mathf.Clamp01((Time.time - StartTime) * 0.75f));
			break;
		}
		base.transform.position = Curve.GetPosition(Progress);
	}

	public void KillGuide()
	{
		for (int i = 0; i < FXs.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FXs[i].emission;
			emission.enabled = false;
		}
		Object.Destroy(base.gameObject, 1f);
	}
}
