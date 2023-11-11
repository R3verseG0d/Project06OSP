using System.Collections;
using UnityEngine;

public class GoalRing : ObjectBase
{
	[Header("Framework")]
	public Transform Mesh;

	public AudioSource LoopAudio;

	public AudioClip GoalGet;

	public Light Light;

	public ParticleSystem[] FX;

	public GameObject GoalRingFX;

	internal bool IsTriggered;

	private void Update()
	{
		Mesh.Rotate(Vector3.up * 90f * Time.deltaTime);
		if (!IsTriggered)
		{
			Light.intensity = Mathf.Lerp(0.9f, 1.2f, Mathf.Abs(Mathf.Cos(Time.time * 2.5f)));
			Light.intensity *= Light.intensity;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && Singleton<GameManager>.Instance.GameState != GameManager.State.Result && !IsTriggered)
		{
			player.OnGoal();
			ActivateGoal();
		}
	}

	public void ActivateGoal()
	{
		StartCoroutine(FadeAway());
		GoalRingFX.SetActive(value: true);
		Singleton<AudioManager>.Instance.PlayClip(GoalGet);
		for (int i = 0; i < FX.Length; i++)
		{
			FX[i].Stop();
		}
		LoopAudio.Stop();
		IsTriggered = true;
	}

	private IEnumerator FadeAway()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		float Intensity = 2.5f;
		Light.intensity = Intensity;
		while (Timer <= 1f)
		{
			Timer = (Time.time - StartTime) * 2f;
			float num = Mathf.Max(0f, 1f - Timer);
			Mesh.localScale = Vector3.one * num;
			Light.intensity = Mathf.Lerp(Intensity, 0f, Timer);
			yield return new WaitForFixedUpdate();
		}
		Object.Destroy(base.gameObject, 1f);
	}
}
