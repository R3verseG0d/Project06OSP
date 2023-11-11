using UnityEngine;

public class WvoDoor : MonoBehaviour
{
	public enum Content
	{
		Closed = 0,
		Open = 1
	}

	[Header("Framework")]
	public bool Signal;

	public Content Mode;

	[Header("Prefab")]
	public Animator Animator;

	[Header("Optional")]
	public ParticleSystem[] FX;

	public void SetParameters(bool _Signal, int _Mode)
	{
		Signal = _Signal;
		Mode = (Content)(_Mode - 1);
	}

	private void Start()
	{
		switch (Mode)
		{
		case Content.Closed:
			Animator.SetTrigger("On Start Closed");
			break;
		case Content.Open:
			Animator.SetTrigger("On Start Open");
			break;
		}
	}

	private void GateClose()
	{
		if (!Signal)
		{
			return;
		}
		Animator.SetTrigger("On Close");
		if (FX != null)
		{
			for (int i = 0; i < FX.Length; i++)
			{
				FX[i].Play();
			}
		}
	}

	private void OnEventSignal()
	{
		if (Signal)
		{
			Animator.SetTrigger("On Open");
		}
	}
}
