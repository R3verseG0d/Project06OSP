using STHEngine;
using UnityEngine;

public class MultiReticle : MonoBehaviour
{
	[Header("Framework")]
	public Animator Animator;

	public GameObject LockOnObject;

	public AudioClip Sound;

	internal UI HUD;

	internal GameObject Target;

	private int Behaviour;

	private float LockOnZRot;

	private bool OutOfScreen;

	private bool Vanish;

	private void Start()
	{
		if (HUD.PM.Base.GetPrefab("shadow"))
		{
			Behaviour = 0;
		}
		else if (HUD.PM.Base.GetPrefab("omega"))
		{
			Behaviour = 1;
		}
		base.transform.position = HUD.PM.Base.Camera.Camera.ViewportToScreenPoint(Target.transform.position);
		if ((bool)Sound)
		{
			Singleton<AudioManager>.Instance.PlayClip(Sound, 0.75f);
		}
	}

	private void Update()
	{
		if (Behaviour == 0)
		{
			if (!Vanish && (bool)Target && HUD.PM.shadow.SpearTargets != null && HUD.PM.Base.GetPrefab("shadow") && HUD.PM.shadow.SpearTargets.Contains(Target) && HUD.PM.shadow.GetState() == "ChaosSpear" && HUD.PM.shadow.SpearState == 0)
			{
				Vector3 position = Target.transform.position;
				position = HUD.PM.Base.Camera.Camera.WorldToViewportPoint(position);
				OutOfScreen = position.x > 0.95f || position.y > 0.95f || position.x < 0.05f || position.y < 0.05f;
				if (position.z < 0f)
				{
					position.x = 1f - position.x;
					position.y = 1f - position.y;
					position.z = 0f;
					position = ExtensionMethods.Vector3Maxamize(position);
				}
				position = HUD.PM.Base.Camera.Camera.ViewportToScreenPoint(position);
				position.x = Mathf.Clamp(position.x, 20f, (float)Screen.width - 20f);
				position.y = Mathf.Clamp(position.y, 20f, (float)Screen.height - 20f);
				base.transform.position = position;
				LockOnObject.SetActive(!OutOfScreen);
			}
			else
			{
				if (Vanish)
				{
					return;
				}
				Animator.SetTrigger("On Vanish");
				if (HUD.MultiReticles != null)
				{
					for (int i = 0; i < HUD.MultiReticles.Count; i++)
					{
						if (HUD.MultiReticles[i] == Target)
						{
							HUD.MultiReticles.RemoveAt(i);
						}
					}
				}
				Object.Destroy(base.gameObject, 1f);
				Vanish = true;
			}
		}
		else
		{
			if (Behaviour != 1)
			{
				return;
			}
			if (!Vanish && (bool)Target && HUD.PM.omega.LaserTargets != null && HUD.PM.Base.GetPrefab("omega") && HUD.PM.omega.LaserTargets.Contains(Target) && HUD.PM.omega.LockOnTargets)
			{
				LockOnZRot += Time.deltaTime * 90f;
				LockOnObject.transform.localRotation = Quaternion.Euler(0f, 0f, LockOnZRot);
				Vector3 position2 = Target.transform.position;
				position2 = HUD.PM.Base.Camera.Camera.WorldToViewportPoint(position2);
				OutOfScreen = position2.x > 0.95f || position2.y > 0.95f || position2.x < 0.05f || position2.y < 0.05f;
				if (position2.z < 0f)
				{
					position2.x = 1f - position2.x;
					position2.y = 1f - position2.y;
					position2.z = 0f;
					position2 = ExtensionMethods.Vector3Maxamize(position2);
				}
				position2 = HUD.PM.Base.Camera.Camera.ViewportToScreenPoint(position2);
				position2.x = Mathf.Clamp(position2.x, 20f, (float)Screen.width - 20f);
				position2.y = Mathf.Clamp(position2.y, 20f, (float)Screen.height - 20f);
				base.transform.position = position2;
				LockOnObject.SetActive(!OutOfScreen);
			}
			else
			{
				if (Vanish)
				{
					return;
				}
				Animator.SetTrigger("On Vanish");
				if (HUD.MultiReticles != null)
				{
					for (int j = 0; j < HUD.MultiReticles.Count; j++)
					{
						if (HUD.MultiReticles[j] == Target)
						{
							HUD.MultiReticles.RemoveAt(j);
						}
					}
				}
				Object.Destroy(base.gameObject, 1f);
				Vanish = true;
			}
		}
	}
}
