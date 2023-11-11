using UnityEngine;

public class PillarB : MonoBehaviour
{
	[Header("Framework")]
	public float Height;

	public float Time;

	public bool NoEffect;

	[Header("Prefab")]
	public Rigidbody RigidBody;

	public Animator Animator;

	public AudioSource Audio;

	public Renderer SandRenderer;

	private Vector3 OriginalPos;

	private bool IsTriggered;

	private bool ReachedApex;

	private bool InProcess;

	private float StartTime;

	private float SandAlpha;

	public void SetParameters(float _Height, float _Time, bool _NoEffect)
	{
		Height = _Height;
		Time = _Time;
		NoEffect = _NoEffect;
	}

	private void Start()
	{
		OriginalPos = base.transform.position;
		base.transform.position += base.transform.up * (0f - Height);
	}

	private void FixedUpdate()
	{
		if (!IsTriggered)
		{
			return;
		}
		if (!InProcess)
		{
			InProcess = true;
			Audio.Play();
			StartTime = UnityEngine.Time.time;
		}
		if (UnityEngine.Time.time - StartTime < Time)
		{
			float t = (UnityEngine.Time.time - StartTime) / Time;
			Vector3 position = Vector3.Lerp(OriginalPos + base.transform.up * (0f - Height), OriginalPos, t);
			RigidBody.MovePosition(position);
			return;
		}
		if (!ReachedApex)
		{
			Animator.SetTrigger("On Stop");
			ReachedApex = true;
		}
		base.transform.position = OriginalPos;
	}

	private void Update()
	{
		if (NoEffect)
		{
			return;
		}
		SandAlpha = (IsTriggered ? Mathf.MoveTowards(SandAlpha, 0f, UnityEngine.Time.deltaTime * 0.25f) : 2f);
		Material[] materials = SandRenderer.materials;
		for (int i = 0; i < materials.Length; i++)
		{
			if (materials[i].HasProperty("_MaskInt"))
			{
				materials[i].SetFloat("_MaskInt", SandAlpha);
			}
		}
		SandRenderer.materials = materials;
		if (SandAlpha == 0f && SandRenderer.enabled)
		{
			SandRenderer.enabled = false;
		}
	}

	private void OnEventSignal()
	{
		SandRenderer.enabled = !NoEffect;
		if (!NoEffect)
		{
			SandAlpha = 2f;
		}
		Animator.SetTrigger("On Rise");
		IsTriggered = true;
	}
}
