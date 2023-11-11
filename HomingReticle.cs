using STHEngine;
using UnityEngine;

public class HomingReticle : MonoBehaviour
{
	[Header("Framework")]
	public GameObject LockOnObject;

	public Animator LockOnAnimator;

	public GameObject Circle;

	public GameObject Cross;

	public GameObject Indicator;

	public float LockVelocity = 50f;

	public Vector3 StartLockOnOffset = new Vector3(5f, 5f, 5f);

	public Vector3 LockedOffset = Vector3.one;

	internal UI HUD;

	private GameObject LastTarget;

	private GameObject CurrentTarget;

	private float LockOnZRot;

	private bool OutOfScreen;

	private void Update()
	{
		if (CanLock())
		{
			CurrentTarget = HUD.PM.Base.HomingTarget;
			if (CurrentTarget != LastTarget)
			{
				Circle.transform.localScale = StartLockOnOffset;
				LockOnAnimator.SetTrigger("Start");
				LockOnZRot = 0f;
			}
			else
			{
				LockOnZRot += Time.deltaTime * 360f;
				Cross.transform.localRotation = Quaternion.Euler(0f, 0f, LockOnZRot);
				Circle.transform.localScale = Vector3.MoveTowards(Circle.transform.localScale, LockedOffset, LockVelocity * Time.deltaTime);
			}
			if ((bool)HUD.PM.Base.HomingTarget)
			{
				Vector3 position = HUD.PM.Base.HomingTarget.transform.position;
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
				LockOnObject.transform.position = position;
				if (OutOfScreen)
				{
					Vector3 vector = HUD.PM.Base.Camera.Camera.transform.InverseTransformPoint(HUD.PM.Base.HomingTarget.transform.position);
					float z = (0f - Mathf.Atan2(vector.x, vector.y)) * 57.29578f - 90f;
					Indicator.transform.eulerAngles = new Vector3(0f, 0f, z);
				}
				else
				{
					Indicator.transform.eulerAngles = Vector3.zero;
				}
				Circle.SetActive(!OutOfScreen);
				Indicator.SetActive(OutOfScreen);
			}
			else
			{
				OutOfScreen = false;
			}
			LastTarget = CurrentTarget;
		}
		else
		{
			LastTarget = null;
		}
		LockOnObject.SetActive(CanLock());
	}

	private bool CanLock()
	{
		if (HUD.PM.Base.GetPrefab("sonic_new") && (((HUD.PM.Base.GetState() == "Jump" || (HUD.PM.Base.GetState() == "Spring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "WideSpring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "JumpPanel" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "RainbowRing" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "GunDriveMove" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "Rope" && !HUD.PM.Base.LockControls)) && !HUD.PM.sonic.UsingPurpleGem && HUD.PM.Base.GetState() != "GunDriveMove" && HUD.PM.sonic.JumpLimit < 1) || HUD.PM.Base.GetState() == "AfterHoming" || (HUD.PM.Base.GetState() == "BoundAttack" && ((HUD.PM.sonic.BoundState == 0 && HUD.PM.sonic.CanJumpdash) || HUD.PM.sonic.BoundState == 1)) || (HUD.PM.Base.GetState() == "TrickJump" && !HUD.PM.sonic.UsingPurpleGem) || (HUD.PM.Base.GetState() == "HomingSmash" && !HUD.PM.Base.LockControls)) && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("princess") && (HUD.PM.Base.GetState() == "Jump" || (HUD.PM.Base.GetState() == "Spring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "WideSpring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "JumpPanel" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "RainbowRing" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "Lotus" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "Tarzan" && !HUD.PM.Base.LockControls) || HUD.PM.Base.GetState() == "AfterHoming") && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("shadow") && (HUD.PM.Base.GetState() == "Jump" || (HUD.PM.Base.GetState() == "Spring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "WideSpring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "JumpPanel" && !HUD.PM.Base.LockControls) || HUD.PM.Base.GetState() == "RainbowRing" || (HUD.PM.Base.GetState() == "Rope" && !HUD.PM.Base.LockControls && !HUD.PM.Base.LockControls) || HUD.PM.Base.GetState() == "AfterHoming" || (HUD.PM.Base.GetState() == "WideSpring" && !HUD.PM.Base.LockControls) || HUD.PM.Base.GetState() == "AfterHoming" || HUD.PM.Base.GetState() == "TrickJump") && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("silver") && HUD.PM.silver.ManipulateObjects && HUD.PM.silver.PickedObjects.Count != 0 && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("tails") && (HUD.PM.Base.GetState() == "Jump" || HUD.PM.Base.GetState() == "Fly") && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("knuckles") && HUD.PM.Base.GetState() == "Screwdriver" && HUD.PM.Base.PlayerManager.knuckles.ScrewState == 0 && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("blaze") && ((HUD.PM.Base.GetState() == "Ground" && !HUD.PM.Base.IsSinking) || HUD.PM.Base.GetState() == "Jump" || HUD.PM.Base.GetState() == "AccelJump" || HUD.PM.Base.GetState() == "AfterHoming") && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("rouge") && ((HUD.PM.Base.GetState() == "Ground" && !HUD.PM.Base.IsSinking) || HUD.PM.Base.GetState() == "Jump" || HUD.PM.Base.GetState() == "Glide") && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		if (HUD.PM.Base.GetPrefab("metal_sonic") && (HUD.PM.Base.GetState() == "Jump" || (HUD.PM.Base.GetState() == "Spring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "WideSpring" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "Rope" && !HUD.PM.Base.LockControls) || HUD.PM.Base.GetState() == "AfterHoming" || (HUD.PM.Base.GetState() == "GunDriveMove" && !HUD.PM.Base.LockControls) || (HUD.PM.Base.GetState() == "Rope" && !HUD.PM.Base.LockControls)) && (bool)HUD.PM.Base.HomingTarget)
		{
			return true;
		}
		return false;
	}
}
