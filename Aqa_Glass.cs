using STHEngine;
using UnityEngine;

public class Aqa_Glass : MonoBehaviour
{
	[Header("Framework")]
	public float Speed;

	[Header("Prefab")]
	public AudioSource Audio;

	public Animator Animator;

	public GameObject Glass;

	public GameObject BrokenPrefab;

	private bool Close;

	private bool Broken;

	public void SetParameters(float _Speed)
	{
		Speed = _Speed;
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!Broken)
		{
			Vector3 position = Glass.transform.position;
			Object.Destroy(Glass);
			GameObject gameObject = Object.Instantiate(BrokenPrefab, position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", HitInfo, SendMessageOptions.DontRequireReceiver);
			Broken = true;
		}
	}

	private void OnEventSignal()
	{
		if (!Close)
		{
			Animator.SetTrigger("On Close");
			Animator.speed = Speed;
			Audio.Play();
			Close = true;
		}
	}
}
