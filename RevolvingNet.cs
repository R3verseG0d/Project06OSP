using UnityEngine;

public class RevolvingNet : MonoBehaviour
{
	public AudioSource Audio;

	public Animation Animation;

	private bool IsRotating;

	private void Update()
	{
		if (!Animation.isPlaying)
		{
			IsRotating = false;
		}
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (IsRotating)
		{
			return;
		}
		PlayerBase component = HitInfo.player.GetComponent<PlayerBase>();
		Animation.Play((Vector3.Dot(base.transform.forward, (base.transform.position - HitInfo.player.position).normalized) < 0f) ? "NetFront" : "NetBack");
		Audio.Play();
		if (component.IsGrounded())
		{
			component.CurSpeed += component.TopSpeed * 0.25f;
			if ((bool)component.PlayerManager.sonic)
			{
				component.PlayerManager.sonic.FirstKickSpeed += component.TopSpeed * 0.25f;
				component.PlayerManager.sonic.SlideSpeed += component.TopSpeed * 0.25f;
			}
			if ((bool)component.PlayerManager.princess)
			{
				component.PlayerManager.princess.SlideSpeed += component.TopSpeed * 0.25f;
			}
			if ((bool)component.PlayerManager.shadow)
			{
				component.PlayerManager.shadow.FirstKickSpeed += component.TopSpeed * 0.25f;
			}
		}
		IsRotating = true;
	}
}
