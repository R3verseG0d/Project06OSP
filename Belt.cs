using UnityEngine;

public class Belt : ObjectBase
{
	[Header("Framework")]
	public float Forward;

	public float Backward;

	public float Time1;

	[Header("Prefab")]
	public AudioSource BuzzerSource;

	public AudioSource BeltSwitchSource;

	public Renderer Renderer;

	public Renderer SirenRenderer;

	private PlayerBase PlayerBase;

	private bool SwitchOpposite;

	private bool PlaySound;

	private bool PlaySwitchSound;

	private float StartTime;

	private MaterialPropertyBlock PropBlock;

	public void SetParameters(float _Forward, float _Backward, float _Time1)
	{
		Forward = _Forward;
		Backward = _Backward;
		Time1 = _Time1;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		SwitchOpposite = false;
		StartTime = Time.time;
	}

	private void Update()
	{
		float num = Time.time - StartTime;
		if (num > Time1 - 3.5f && !PlaySound)
		{
			BuzzerSource.Play();
			PlaySound = true;
		}
		if (num > Time1)
		{
			StartTime = Time.time;
			SwitchOpposite = !SwitchOpposite;
			PlaySound = false;
			if (!PlaySwitchSound)
			{
				BeltSwitchSource.Play();
				PlaySwitchSound = true;
			}
		}
		else
		{
			PlaySwitchSound = false;
		}
		SirenRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Intensity", (num < Time1 - 3.5f) ? 0f : Mathf.PingPong(Time.time * ((num < Time1 - 1.75f) ? 18f : 40f), 3f));
		SirenRenderer.SetPropertyBlock(PropBlock);
	}

	private void FixedUpdate()
	{
		Renderer.GetPropertyBlock(PropBlock, 0);
		PropBlock.SetVector("_MainTex_ST", new Vector4(1f, (!SwitchOpposite) ? 1f : (-1f), 0f, ((!SwitchOpposite) ? (0f - Backward) : (0f - Forward)) * 0.43f * Time.time));
		PropBlock.SetVector("_Normal_ST", new Vector4(1f, (!SwitchOpposite) ? 1f : (-1f), 0f, ((!SwitchOpposite) ? (0f - Backward) : (0f - Forward)) * 0.43f * Time.time));
		Renderer.SetPropertyBlock(PropBlock, 0);
		if ((bool)PlayerBase && PlayerBase.IsGrounded())
		{
			PlayerBase.transform.position += base.transform.forward * ((!SwitchOpposite) ? Forward : (0f - Backward)) * 2.5f * Time.fixedDeltaTime;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.tag == "Player" && !PlayerBase)
		{
			PlayerBase = collision.gameObject.GetComponent<PlayerBase>();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			PlayerBase = null;
		}
	}
}
