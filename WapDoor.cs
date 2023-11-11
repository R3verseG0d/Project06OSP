using UnityEngine;

public class WapDoor : MonoBehaviour
{
	[Header("Optional")]
	public bool StartOpen;

	[Header("Prefab")]
	public Animator Animator;

	public AudioSource Audio;

	public GameObject FX;

	public Renderer ObjRenderer;

	public Color GlowClosedColor;

	public Gradient GlowOpenColor;

	private bool Opened;

	private bool Closed;

	private float StartTime;

	private MaterialPropertyBlock PropBlock;

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
		ObjRenderer.GetPropertyBlock(PropBlock, 2);
		PropBlock.SetColor("_Color", (!Opened || Closed) ? GlowClosedColor : GlowOpenColor.Evaluate(Time.time - StartTime));
		ObjRenderer.SetPropertyBlock(PropBlock, 2);
	}

	private void OnEventSignal()
	{
		GateOpen();
	}

	private void GateOpen()
	{
		if (!Closed)
		{
			Opened = true;
			Animator.SetTrigger("On Trigger");
			Audio.Play();
			FX.SetActive(value: true);
			StartTime = Time.time;
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
}
