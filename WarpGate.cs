using System.Collections;
using UnityEngine;

public class WarpGate : EventStation
{
	[Header("Framework")]
	public string StageName;

	public int Appearances;

	[Header("Prefab")]
	public Renderer Renderer;

	public ParticleSystem InteractFX;

	public AudioSource Audio;

	private MaterialPropertyBlock PropBlock;

	private Vector4[] UVSet = new Vector4[11]
	{
		new Vector4(1f, 1f, 0f, -0.67f),
		new Vector4(1f, 1f, 0.75f, 0f),
		new Vector4(1f, 1f, 0.25f, 0f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0f, -0.335f),
		new Vector4(1f, 1f, 0.75f, -0.335f),
		new Vector4(1f, 1f, 0.25f, -0.335f),
		new Vector4(1f, 1f, 0.5f, -0.335f),
		new Vector4(1f, 1f, 0.5f, 0f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.25f, -0.67f)
	};

	private bool Interacted;

	public void SetParameters(string _StageName, int _Appearances)
	{
		StageName = _StageName;
		Appearances = _Appearances - 1;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
		Renderer.GetPropertyBlock(PropBlock);
		PropBlock.SetVector("_MainTex_ST", UVSet[Appearances]);
		Renderer.SetPropertyBlock(PropBlock);
	}

	public IEnumerator ExecuteFunction(PlayerBase Player)
	{
		float StartTime = Time.time;
		float Timer = 0f;
		bool DoState = false;
		Singleton<GameManager>.Instance.StoredPlayerVars = null;
		InteractFX.Play();
		while (Timer <= 1f)
		{
			Timer = Time.time - StartTime;
			if (Timer > 0.25f)
			{
				Player.HUD.PlayFadeOut();
			}
			yield return new WaitForFixedUpdate();
		}
		if (!DoState)
		{
			CallEvent(StageName, Player.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !Interacted)
		{
			StartCoroutine(ExecuteFunction(player));
			Audio.Play();
			Interacted = true;
		}
	}
}
