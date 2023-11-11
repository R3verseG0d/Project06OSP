using UnityEngine;

public class Common_ChaosEmerald : ObjectBase
{
	public enum Type
	{
		White = 0,
		Sky = 1,
		Yellow = 2,
		Purple = 3,
		Green = 4,
		Blue = 5,
		Red = 6
	}

	[Header("Framework")]
	public Type EmeraldType;

	[Header("Prefab")]
	public GameObject[] Emeralds;

	public Transform Mesh;

	private bool Collected;

	private float YLerp;

	public void SetParameters(int _EmeraldType)
	{
		EmeraldType = (Type)(_EmeraldType - 1);
	}

	private void Start()
	{
		for (int i = 0; i < Emeralds.Length; i++)
		{
			Emeralds[i].SetActive(i == (int)EmeraldType);
		}
	}

	private void Update()
	{
		if (!Collected)
		{
			YLerp = Mathf.Lerp(YLerp, Mathf.PingPong(Time.time / 5f, 0.3f) - 0.15f, Time.deltaTime * 2.5f);
			Mesh.localPosition = new Vector3(Mesh.localPosition.x, YLerp, Mesh.localPosition.z);
			Mesh.Rotate(0f, 120f * Time.deltaTime, 0f);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !Collected)
		{
			Object.Destroy(base.gameObject);
			Collected = true;
		}
	}
}
