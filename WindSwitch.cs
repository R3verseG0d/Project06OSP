using System.Collections;
using UnityEngine;

public class WindSwitch : ObjectBase
{
	[Header("Framework Settings")]
	public WindRoad[] WindRoads;

	[Header("Prefab")]
	public Animator Animator;

	public Transform EaglePillarTransform;

	public Transform BallTransform;

	public AudioClip[] Clips;

	public AudioSource Audio;

	public ParticleSystem WindSwitchFX;

	public ParticleSystem EaglePillarFX;

	private Quaternion OrigEaglePillarRot;

	private float Timer;

	private int Active = -1;

	private void Start()
	{
		OrigEaglePillarRot = EaglePillarTransform.rotation;
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (Time.time - Timer > 1f)
		{
			OnActivate();
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if ((bool)GetPlayer(collider) && Time.time - Timer > 1f)
		{
			OnActivate();
		}
	}

	private void Update()
	{
		if (BallTransform.gameObject.tag == "Untagged" && Time.time - Timer > 1f)
		{
			BallTransform.gameObject.tag = "HomingTarget";
		}
	}

	private void OnActivate()
	{
		Animator.SetTrigger("On Trigger");
		BallTransform.gameObject.tag = "Untagged";
		WindSwitchFX.Play();
		Timer = Time.time;
		Active++;
		if (Active > WindRoads.Length - 1)
		{
			Active = 0;
		}
		if (WindRoads == null)
		{
			return;
		}
		for (int i = 0; i < WindRoads.Length; i++)
		{
			if (i == Active)
			{
				if (!WindRoads[i].Enabled)
				{
					StartCoroutine(PointPillar(i));
				}
				WindRoads[i].OnPathEnable();
			}
			else
			{
				WindRoads[i].OnPathDisable();
			}
		}
	}

	public void PlayClips(int Index)
	{
		Audio.PlayOneShot(Clips[Index], Audio.volume);
	}

	private IEnumerator PointPillar(int Index)
	{
		EaglePillarFX.Play();
		Vector3 TargetRot = (WindRoads[Index].Road.RailPathData[0].position - EaglePillarTransform.position).MakePlanar();
		while (EaglePillarTransform.rotation != Quaternion.LookRotation(TargetRot))
		{
			EaglePillarTransform.rotation = Quaternion.RotateTowards(EaglePillarTransform.rotation, Quaternion.LookRotation(TargetRot), Time.fixedDeltaTime * 100f);
			yield return new WaitForFixedUpdate();
		}
	}
}
