using UnityEngine;

public class BrokenStairs : ObjectBase
{
	[Header("Framework")]
	public float Time;

	[Header("Prefab")]
	public Collider Collider;

	public Animation Animation;

	public AudioSource Audio;

	public ParticleSystem FX;

	private bool Broken;

	private bool PlayAnimation;

	private float StartTime;

	public void SetParameters(float _Time)
	{
		Time = _Time;
	}

	private void Update()
	{
		if (Broken && UnityEngine.Time.time - StartTime > Time && !PlayAnimation)
		{
			Animation.Play();
			Audio.Play();
			FX.Play();
			PlayAnimation = true;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!Broken)
		{
			PlayerBase player = GetPlayer(collision.transform);
			if ((bool)player && player.IsGrounded() && !(player.RaycastHit.collider != Collider))
			{
				StartTime = UnityEngine.Time.time;
				Broken = true;
			}
		}
	}
}
