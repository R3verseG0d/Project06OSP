using UnityEngine;

public class PillarA : PsiObject
{
	public enum Mode
	{
		Normal = 0,
		ESPMode = 1
	}

	[Header("Framework")]
	public float Height;

	public float Time;

	public Mode Type;

	public bool NoEffect;

	[Header("Prefab")]
	public Renderer Renderer;

	public Common_PsiMarkSphere ESPMark;

	public Rigidbody RigidBody;

	public Animator Animator;

	public AudioSource Audio;

	public AudioClip[] Clips;

	public Renderer SandRenderer;

	private Vector3 OriginalPos;

	private bool IsTriggered;

	private bool ReachedApex;

	private bool InProcess;

	private float StartTime;

	private float SandAlpha;

	public void SetParameters(float _Height, float _Time, int _Type, bool _NoEffect)
	{
		Height = _Height;
		Time = _Time;
		Type = (Mode)(_Type - 1);
		NoEffect = _NoEffect;
	}

	private void Start()
	{
		OriginalPos = base.transform.position;
		base.transform.position += base.transform.up * (0f - Height);
		if (Type == Mode.ESPMode && !ESPMark.gameObject.activeSelf)
		{
			ESPMark.gameObject.SetActive(value: true);
			ESPMark.Target = base.gameObject;
		}
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
			Audio.PlayOneShot(Clips[(Type == Mode.ESPMode) ? 1u : 0u], Audio.volume);
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
		if (!NoEffect)
		{
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
		if (Type == Mode.ESPMode)
		{
			OnPsiFX(Renderer, IsTriggered && !ReachedApex);
		}
	}

	private void OnEventSignal()
	{
		if (!IsTriggered)
		{
			SandRenderer.enabled = !NoEffect;
			if (!NoEffect)
			{
				SandAlpha = 2f;
			}
			Animator.SetTrigger("On Shake");
			IsTriggered = true;
		}
	}
}
