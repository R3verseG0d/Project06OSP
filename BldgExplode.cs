using UnityEngine;

public class BldgExplode : MonoBehaviour
{
	[Header("Framework")]
	public float Time;

	[Header("Prefab")]
	public GameObject Prefab;

	private bool IsTriggered;

	private bool Explode;

	private float StartTime;

	public void SetParameters(float _Time)
	{
		Time = _Time;
	}

	private void Update()
	{
		if (IsTriggered && !Explode && UnityEngine.Time.time - StartTime > Time)
		{
			Object.Instantiate(Prefab, base.transform.position, base.transform.rotation);
			Explode = true;
		}
	}

	private void OnEventSignal()
	{
		if (!IsTriggered)
		{
			StartTime = UnityEngine.Time.time;
			IsTriggered = true;
		}
	}
}
