using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : ObjectBase
{
	[Header("Framework")]
	public Renderer Renderer;

	public Animator Animator;

	public AudioSource Audio;

	public AudioClip Sound;

	public ParticleSystem[] ParticleFX;

	public Color InativeColor;

	public Color ActiveColor;

	private MaterialPropertyBlock PropBlock;

	private bool IsActive;

	private bool IsAlreadyTriggered;

	private float StartTime;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		Renderer.GetPropertyBlock(PropBlock);
		PropBlock.SetColor("_Color", IsAlreadyTriggered ? ActiveColor : InativeColor);
		Renderer.SetPropertyBlock(PropBlock);
	}

	private void Update()
	{
		if (IsActive)
		{
			Renderer.GetPropertyBlock(PropBlock);
			if (IsAlreadyTriggered)
			{
				PropBlock.SetColor("_Color", ActiveColor);
			}
			else
			{
				float t = Mathf.Clamp(Time.time - StartTime, 0f, 1f);
				PropBlock.SetColor("_Color", Color.Lerp(InativeColor, ActiveColor, t));
			}
			Renderer.SetPropertyBlock(PropBlock);
			Animator.SetBool("On Active", IsActive);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !IsActive)
		{
			ParticleFX[0].Play();
			ParticleFX[1].Play();
			Audio.PlayOneShot(Sound, Audio.volume);
			string text = SceneManager.GetActiveScene().name;
			string text2 = text.Split("_"[0])[0] + "_" + text.Split("_"[0])[1];
			CheckpointData checkpointData = new CheckpointData(base.transform, player.PlayerNo, player.PlayerPrefab.ToString(), base.gameObject.name + "/" + text2);
			if (Singleton<GameManager>.Instance._PlayerData.checkpoint != null && checkpointData.SavePoint == Singleton<GameManager>.Instance._PlayerData.checkpoint.SavePoint)
			{
				IsAlreadyTriggered = true;
			}
			if (!IsAlreadyTriggered)
			{
				Singleton<GameManager>.Instance.SaveTime = Singleton<GameManager>.Instance._PlayerData.time;
				Object.FindObjectOfType<UI>().OnCheckpoint(Singleton<GameManager>.Instance.SaveTime);
			}
			Animator.SetTrigger("On Checkpoint");
			StartTime = Time.time;
			Singleton<GameManager>.Instance._PlayerData.checkpoint = checkpointData;
			IsActive = true;
		}
	}
}
