using System.Collections;
using Ara;
using UnityEngine;

public class KnifeSlash : AttackBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public float Speed;

	[Header("Prefab")]
	public Collider Collider;

	public ParticleSystem[] FX;

	public Transform Mesh;

	public Light PointLight;

	public AraTrail[] Trails;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		Invoke("FadeAway", 1f);
	}

	private void FixedUpdate()
	{
		_Rigidbody.MovePosition(base.transform.position + base.transform.forward * Speed * 0.01f);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (((int)AttackMask & (1 << collider.gameObject.layer)) != 0)
		{
			collider.SendMessage("OnHit", new HitInfo(Player, base.transform.forward * (Speed / 2f), 3), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void FadeAway()
	{
		StartCoroutine(Fade());
	}

	private IEnumerator Fade()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		for (int i = 0; i < Trails.Length; i++)
		{
			Trails[i].emit = false;
		}
		for (int j = 0; j < FX.Length; j++)
		{
			ParticleSystem.EmissionModule emission = FX[j].emission;
			emission.enabled = false;
		}
		Collider.enabled = false;
		while (Timer <= 0.25f)
		{
			Timer = Time.time - StartTime;
			float num = Mathf.Lerp(1f, 0f, Timer * 4f);
			Mesh.localScale = new Vector3(1f, num, num);
			PointLight.intensity = Mathf.Lerp(PointLight.intensity, 0f, Timer * 4f);
			yield return new WaitForFixedUpdate();
		}
		Object.Destroy(base.gameObject);
	}
}
