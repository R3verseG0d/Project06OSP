using UnityEngine;

public class MemoryShard : ButtonIconBase
{
	[Header("Prefab")]
	public Transform Mesh;

	public ParticleSystem GetFX;

	public AudioSource Audio;

	public AudioSource AudioLoop;

	private bool Collected;

	private float YLerp;

	private void Start()
	{
		if (HasShard())
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		if (!Collected)
		{
			YLerp = Mathf.Lerp(YLerp, Mathf.PingPong(Time.time / 5f, 0.3f) - 0.15f, Time.deltaTime * 2.5f);
			Mesh.localPosition = new Vector3(Mesh.localPosition.x, YLerp, Mesh.localPosition.z);
		}
		else
		{
			AudioLoop.volume = Mathf.Lerp(AudioLoop.volume, 0f, Time.deltaTime * 5f);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && player.GetPrefab("shadow") && !Collected)
		{
			GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
			gameData.ActivateFlag(Game.MemoryShardLight);
			player.PlayerManager.shadow.HasLightMemoryShard = true;
			Singleton<GameManager>.Instance.SetGameData(gameData);
			player.HUD.StartMessageBox(GetText("MemoryShardLight"), null, 8f);
			Mesh.gameObject.SetActive(value: false);
			GetFX.Play();
			Audio.Play();
			Object.Destroy(base.gameObject, 2f);
			Collected = true;
		}
	}

	private bool HasShard()
	{
		GameData.GlobalData gameData = Singleton<GameManager>.Instance.GetGameData();
		if (!gameData.HasFlag("wap_sd") || !gameData.HasFlag("kdv_sd") || !gameData.HasFlag("csc_sd") || !gameData.HasFlag("flc_sd") || !gameData.HasFlag("rct_sd") || !gameData.HasFlag("aqa_sd") || !gameData.HasFlag("wvo_sd") || !gameData.HasFlag("dtd_sd") || !gameData.HasFlag("tpj_rg"))
		{
			return true;
		}
		return gameData.HasFlag(Game.MemoryShardLight);
	}
}
