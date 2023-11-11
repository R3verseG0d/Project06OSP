using UnityEngine;

public class Ring : ObjectBase
{
	[Header("Framework")]
	public bool GroundLightDash;

	public float SplineTime;

	public string SplineName;

	[Header("Prefab")]
	public GameObject Mesh;

	public Transform AttractRot;

	public AudioSource Audio;

	public GameObject RingFX;

	public GameObject AppearFX;

	private Transform Player;

	private bool Collected;

	private bool AttractToPlayer;

	private bool IsMachAttract;

	private float StartTime;

	public void SetParameters(bool _GroundLightDash, float _SplineTime, string _SplineName)
	{
		GroundLightDash = _GroundLightDash;
		SplineTime = _SplineTime;
		SplineName = _SplineName;
	}

	private void Update()
	{
		if (AttractToPlayer)
		{
			float num = Mathf.Clamp01((Time.time - StartTime) * ((!IsMachAttract) ? 0.75f : 1f));
			float num2 = Mathf.Lerp(2f, 60f, num);
			AttractRot.forward = Vector3.Lerp(AttractRot.forward, (Player.position + Player.up * 0.25f - base.transform.position).normalized, Time.deltaTime * num2);
			base.transform.position += AttractRot.forward * num * Time.deltaTime * 100f;
		}
	}

	private void SpawnFX()
	{
		AppearFX.SetActive(value: true);
	}

	private void OnAttract(Transform _Transform)
	{
		if (!AttractToPlayer && !Collected)
		{
			AttractToPlayer = true;
			base.gameObject.tag = "Untagged";
			AttractRot.rotation = Random.rotation;
			Player = _Transform;
			if (Player.gameObject.name.Contains("Sonic_Fast"))
			{
				IsMachAttract = true;
			}
			StartTime = Time.time;
		}
	}

	private void OnCollect()
	{
		base.gameObject.tag = "Untagged";
		Mesh.SetActive(value: false);
		RingFX.SetActive(value: true);
		Object.Destroy(base.gameObject, 2f);
		Collected = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!Collected)
		{
			PlayerBase player = GetPlayer(collider);
			if ((bool)player)
			{
				player.AddRing(1, Audio);
				player.AddScore(10);
				OnCollect();
			}
			AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
			if ((bool)component)
			{
				component.AddRing(1, Audio);
				component.AddScore(10);
				OnCollect();
			}
		}
	}
}
