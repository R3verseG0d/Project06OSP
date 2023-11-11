using UnityEngine;

public class Common_Hint : ButtonIconBase
{
	[Header("Framework")]
	public string hintText;

	[Header("Prefab")]
	public Animator Animator;

	public AudioSource Audio;

	public ParticleSystem HintFX;

	private float HintEnd;

	public void SetParameters(string _hintText)
	{
		hintText = _hintText;
	}

	private void Start()
	{
		if (Singleton<Settings>.Instance.settings.Hints == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(HintEnd + 1f > Time.time))
		{
			PlayerBase player = GetPlayer(collider);
			if ((bool)player && !player.IsDead && !(player.GetState() == "Result") && !(hintText == ""))
			{
				HintEnd = player.HUD.StartMessageBox(GetText(hintText), GetSound(hintText));
				Animator.SetTrigger("On Hint");
				Invoke("OnHintEnd", HintEnd);
				HintEnd += Time.time;
				HintFX.Play();
				Audio.Play();
			}
		}
	}

	public void OnHintEnd()
	{
		Animator.SetTrigger("On Hint End");
	}
}
