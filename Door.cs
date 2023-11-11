using System.Collections;
using UnityEngine;

public class Door : EventStation
{
	[Header("Prefab")]
	public Animator Animator;

	public AudioSource Audio;

	public bool StartOpen;

	[Header("Optional")]
	public GameObject[] Particles;

	public Renderer PsiRenderer;

	public Renderer ObjRenderer;

	public Color GlowClosedColor;

	public Color GlowOpenColor;

	[Header("Experimental")]
	public GameObject[] OpenEventObjects;

	private MaterialPropertyBlock PropBlock;

	private bool Opened;

	private bool Closed;

	private bool DoPsiFX;

	private bool ActivatedPsiFX;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		if (StartOpen)
		{
			Opened = true;
			Animator.SetTrigger("On Start Open");
		}
	}

	private void Update()
	{
		if ((bool)ObjRenderer)
		{
			ObjRenderer.GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_Color", (!Opened || Closed) ? GlowClosedColor : GlowOpenColor);
			ObjRenderer.SetPropertyBlock(PropBlock);
		}
		if ((bool)PsiRenderer)
		{
			PsiRenderer.GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_ExtFresColor", new Vector4(0f, 255f, 216f, 1f));
			PropBlock.SetFloat("_ExtFresPower", 0.5f);
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), DoPsiFX ? 0.0025f : 0f, Time.deltaTime * 10f));
			PropBlock.SetColor("_OutlineColor", new Vector4(0f, 255f, 168f, 1f));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", DoPsiFX ? 1f : 0f);
			PsiRenderer.SetPropertyBlock(PropBlock);
		}
	}

	private void DoEvent()
	{
		CallEvent(base.gameObject.name.Split("_"[0])[1]);
	}

	private void OnEventSignal()
	{
		GateOpen();
	}

	private void GateOpen()
	{
		if (Closed)
		{
			return;
		}
		Opened = true;
		Animator.SetTrigger("On Trigger");
		Audio.Play();
		if (Particles != null)
		{
			GameObject[] particles = Particles;
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].SetActive(value: true);
			}
			Particles = null;
		}
		if (OpenEventObjects != null)
		{
			for (int j = 0; j < OpenEventObjects.Length; j++)
			{
				OpenEventObjects[j].SetActive(value: true);
			}
			OpenEventObjects = null;
		}
	}

	private void GateClose()
	{
		if (Opened)
		{
			Closed = true;
			Animator.SetTrigger("On Close");
			Audio.Play();
		}
	}

	private void PsiEffect(bool Enable)
	{
		if ((bool)PsiRenderer && !ActivatedPsiFX)
		{
			ActivatedPsiFX = true;
			if (Enable)
			{
				StartCoroutine(OnPsiFX());
			}
			else if (!Enable && DoPsiFX)
			{
				StopCoroutine(OnPsiFX());
				DoPsiFX = false;
			}
		}
	}

	private IEnumerator OnPsiFX()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		DoPsiFX = true;
		while (Timer <= 0.5f)
		{
			Timer = Time.time - StartTime;
			yield return new WaitForFixedUpdate();
		}
		DoPsiFX = false;
	}
}
