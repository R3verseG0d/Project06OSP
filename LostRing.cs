using UnityEngine;

public class LostRing : ObjectBase
{
	[Header("Framework")]
	public Rigidbody _Rigidbody;

	public Renderer Renderer;

	public AudioClip[] ClipPool;

	public GameObject Mesh;

	public AudioSource Audio;

	public GameObject RingFX;

	private bool Collected;

	private float Timer;

	private float BlinkTimer;

	public void SetVelocity(Vector3 Velocity)
	{
		_Rigidbody.velocity = Velocity;
	}

	private void Update()
	{
		if (!Collected)
		{
			base.transform.rotation = Quaternion.identity;
		}
	}

	private void FixedUpdate()
	{
		if (Collected)
		{
			return;
		}
		Timer += Time.fixedDeltaTime;
		Vector3 velocity = _Rigidbody.velocity;
		velocity.y = _Rigidbody.velocity.y + -20f * Time.fixedDeltaTime;
		velocity.x = Mathf.Lerp(velocity.x, 0f, Time.fixedDeltaTime * 0.5f);
		velocity.z = Mathf.Lerp(velocity.z, 0f, Time.fixedDeltaTime * 0.5f);
		_Rigidbody.velocity = velocity;
		if (Timer > 5f)
		{
			BlinkTimer += Time.fixedDeltaTime * 15f;
			if (BlinkTimer >= 1f)
			{
				BlinkTimer = 0f;
				Renderer.enabled = true;
			}
			if (BlinkTimer >= 0.5f)
			{
				Renderer.enabled = false;
			}
		}
		if (Timer > 10f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void PlayRingSound(PlayerBase PlayerBase, bool CuttedSound = false)
	{
		Singleton<AudioManager>.Instance.PlayClip(PlayerBase.Audio, ClipPool[CuttedSound ? 1 : 0]);
	}

	private void OnCollect()
	{
		_Rigidbody.useGravity = false;
		_Rigidbody.isKinematic = true;
		_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		Mesh.SetActive(value: false);
		RingFX.SetActive(value: true);
		Object.Destroy(base.gameObject, 2f);
		Collected = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(Timer < 0.25f) && !Collected)
		{
			PlayerBase player = GetPlayer(collider);
			if ((bool)player)
			{
				player.AddRing(1, Audio);
				OnCollect();
			}
			AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
			if ((bool)component)
			{
				component.AddRing(1, Audio);
				OnCollect();
			}
		}
	}
}
