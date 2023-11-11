using UnityEngine;

public class VehicleBase : ObjectBase
{
	public enum Type
	{
		Bike = 0,
		Glider = 1,
		HoverCraft = 2,
		Jeep = 3
	}

	[Header("Vehicle Base")]
	public Type VehicleType;

	public Rigidbody _Rigidbody;

	public Vector3 CenterOfMass;

	public Collider[] VehicleCols;

	public BoxCollider AttackBox;

	public GameObject Missile;

	public GameObject MissileShootFX;

	public GameObject Ragdoll;

	public Transform PlayerPoint;

	public Transform[] WeaponPoints;

	public Animator[] WeaponAnimators;

	public Renderer[] WeaponRenderers;

	public ParticleSystem[] SmokeFX;

	public ParticleSystem[] FireFX;

	public Color[] BoostGlows;

	internal bool IsGetOut;

	internal bool IsShoot;

	[Header("Base Audio")]
	public AudioSource Audio;

	public AudioSource IdleEngine;

	public AudioClip EngineTurnOn;

	public AudioClip WeaponReload;

	public AudioClip JumpOff;

	public AudioClip Crash;

	internal MaterialPropertyBlock PropBlock;

	internal PlayerManager PM;

	internal Vector2 MoveAxes;

	internal bool IsMounted;

	internal bool IsShooting;

	internal bool IsLaunched;

	internal bool GoingReverse;

	internal bool SkipStateJump;

	internal float MountedTime;

	internal float LaunchTime;

	internal float MaxHP;

	internal float CurHP;

	internal int CurAmmo;

	internal int MaxAmmo;

	internal string ShotName;

	private Collider[] PlayerCols;

	private Vector3 JumpOffVel;

	private bool VehicleReverse;

	internal bool VehicleStart;

	internal bool VehicleDamage;

	internal int VehicleState;

	private float VehicleTime;

	internal LayerMask Collision_Mask => LayerMask.GetMask("Ignore Raycast", "Water", "UI", "PlayerCollision", "BreakableObj", "Object/PlayerOnlyCol");

	public virtual void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public virtual void Start()
	{
		_Rigidbody.centerOfMass = CenterOfMass;
		if (VehicleType == Type.HoverCraft)
		{
			return;
		}
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, 100000f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform.gameObject.layer == LayerMask.NameToLayer("VehicleWater"))
			{
				Collider[] array2 = componentsInChildren;
				foreach (Collider collider in array2)
				{
					Physics.IgnoreCollision(array[i], collider);
				}
			}
		}
	}

	public virtual void Update()
	{
		CurHP = Mathf.Clamp(CurHP, 0f, MaxHP);
		for (int i = 0; i < SmokeFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = SmokeFX[i].emission;
			emission.enabled = CurHP < MaxHP / 2f;
		}
		for (int j = 0; j < FireFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = FireFX[j].emission;
			emission2.enabled = CurHP < MaxHP / 4f;
		}
		if (WeaponRenderers != null)
		{
			for (int k = 0; k < WeaponRenderers.Length; k++)
			{
				WeaponRenderers[k].GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_ExtFresColor", BoostGlows[0]);
				PropBlock.SetColor("_ExtGlowColor", BoostGlows[1]);
				PropBlock.SetFloat("_ExtPulseSpd", 0.5f);
				PropBlock.SetFloat("_ExtFresPower", 1f);
				PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), (IsMounted && (bool)PM && PM.shadow.IsChaosBoost) ? 1f : 0f, Time.deltaTime * 8f));
				PropBlock.SetColor("_OutlineColor", BoostGlows[0]);
				PropBlock.SetColor("_OutlinePulseColor", BoostGlows[1]);
				PropBlock.SetFloat("_OutlinePulseSpd", 0.5f);
				PropBlock.SetFloat("_OutlineInt", (IsMounted && (bool)PM && PM.shadow.IsChaosBoost) ? 1f : 0f);
				WeaponRenderers[k].SetPropertyBlock(PropBlock);
			}
		}
		if (IsMounted)
		{
			PM.Base.HUD.HealthDisplay = CurHP;
			PM.Base.HUD.AmmoDisplay = CurAmmo;
			if (((bool)PM && PM.Base.StageManager.StageState != StageManager.State.Event && IsGetOut && !VehicleStart && !VehicleDamage && Time.time - MountedTime > 0.25f && Singleton<RInput>.Instance.P.GetButtonDown("Button Y")) || PM.Base.StageManager.StageState == StageManager.State.Event || !PM)
			{
				Demount();
			}
		}
		if (IsLaunched && Time.time - LaunchTime > 1f)
		{
			IsLaunched = false;
		}
		if (CurHP == 0f)
		{
			Demount(IsDestroy: true);
			Object.Instantiate(Ragdoll, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}
	}

	public void Mount(PlayerManager Manager, bool StartInVehicle = false)
	{
		PM = Manager;
		SkipStateJump = StartInVehicle;
		MountedTime = Time.time;
		if (PlayerCols == null)
		{
			PlayerCols = PM.GetComponentsInChildren<Collider>();
		}
		OnMount();
		PM.Base.StateMachine.ChangeState(base.gameObject, StateVehicle);
	}

	public void Demount(bool IsDestroy = false)
	{
		if (!IsDestroy)
		{
			IsMounted = false;
			OnDemount();
			MountedTime = Time.time;
		}
		if (!PM)
		{
			return;
		}
		PM.Base.HUD.CloseVehicle();
		if (IsShoot)
		{
			PM.Base.HUD.CloseWeapons();
		}
		PM.RBody.isKinematic = false;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		if (GetVehicleName() != "Glider" || IsDestroy)
		{
			PM.Base.transform.SetParent(null);
		}
		if (GetVehicleName() == "Glider")
		{
			PM.Base.Camera.IsGlider = false;
		}
		if (!IsDestroy)
		{
			VehicleState = 2;
			VehicleTime = Time.time;
			if (GetVehicleName() != "Glider")
			{
				JumpOffVel = Vector3.zero;
				JumpOffVel.y = 10f;
				PM.RBody.velocity = JumpOffVel;
				PM.Base.PlayAnimation("Rolling", "On Roll");
				PM.Base.Audio.PlayOneShot(JumpOff, PM.Base.Audio.volume);
			}
			else
			{
				PM.Base.PlayAnimation(GetVehicleName() + " Landing", "On Vehicle Exit");
			}
		}
		else
		{
			if (IsGetOut)
			{
				PM.Base.SetMachineState("StateAir");
			}
			else
			{
				PM.Base.OnDeathEnter(1);
			}
			PM.shadow.CurVehicle = null;
			PM = null;
		}
	}

	public void PlayWeaponAnimation(string TriggerName)
	{
		if (IsShoot)
		{
			WeaponAnimators[0].SetTrigger(TriggerName);
			WeaponAnimators[1].SetTrigger(TriggerName);
		}
	}

	public void ApplyPosAndRotToVisuals(WheelCollider Collider)
	{
		if (Collider.transform.childCount != 0)
		{
			Transform child = Collider.transform.GetChild(0);
			Collider.GetWorldPose(out var pos, out var quat);
			child.transform.position = pos;
			child.transform.rotation = quat;
		}
	}

	public float ClampAngle(float Angle, float Min, float Max)
	{
		if (Angle < -360f)
		{
			Angle += 360f;
		}
		if (Angle > 360f)
		{
			Angle -= 360f;
		}
		return Mathf.Clamp(Angle, Min, Max);
	}

	public float SqrMag()
	{
		return _Rigidbody.velocity.magnitude * 3.6f;
	}

	public float ScaledVelSpd()
	{
		return _Rigidbody.velocity.magnitude * 2.2369363f;
	}

	public virtual void OnJumpPanel(Vector3 Velocity)
	{
		_Rigidbody.velocity = Velocity;
		base.transform.forward = _Rigidbody.velocity.normalized;
		LaunchTime = Time.time;
		IsLaunched = true;
	}

	public void OnVehicleHit(float Damage)
	{
		CurHP -= Damage;
	}

	public virtual void OnDamage(float Damage)
	{
		OnVehicleHit(Damage);
	}

	public virtual int Hit()
	{
		return 0;
	}

	public virtual float Damage()
	{
		return 0f;
	}

	public virtual void OnMount()
	{
	}

	public virtual void OnDemount()
	{
	}

	private void StateVehicleStart()
	{
		PM.Base.SetState("Vehicle");
		PM.transform.SetParent(PlayerPoint);
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2);
			}
		}
		PM.RBody.isKinematic = true;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		PM.RBody.velocity = Vector3.zero;
		PM.transform.localRotation = Quaternion.identity;
		if (SkipStateJump)
		{
			VehicleState = 1;
			if (GetVehicleName() == "Glider")
			{
				PM.Base.Camera.IsGlider = true;
				VehicleStart = true;
				VehicleTime = Time.time;
				PM.Base.Animator.SetInteger("Vehicle Type", GetVehicle());
				PM.Base.PlayAnimation(GetVehicleName() + " Start", "On Vehicle Start");
			}
			else
			{
				PM.Base.Animator.SetInteger("Vehicle Type", GetVehicle());
				PM.Base.PlayAnimation(GetVehicleName(), "On Vehicle");
			}
			OnVehicleStats();
			IsMounted = true;
		}
		else
		{
			VehicleTime = Time.time;
			VehicleState = 0;
			PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
			if (PM.Base.IsGrounded())
			{
				PM.Base.Audio.PlayOneShot(JumpOff, PM.Base.Audio.volume);
			}
		}
		PM.shadow.CurVehicle = this;
	}

	private void StateVehicle()
	{
		if ((bool)PM)
		{
			PM.Base.SetState("Vehicle");
			PM.Base.LockControls = true;
			PM.Base.Animator.SetFloat("Y Vel", _Rigidbody.velocity.y);
			PM.Base.CurSpeed = 0f;
			if (VehicleState == 1)
			{
				PM.Base.GeneralMeshRotation = PM.transform.rotation;
			}
			else
			{
				PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
			}
		}
		if (VehicleState == 0)
		{
			float num = Time.time - VehicleTime;
			PM.RBody.MovePosition(Vector3.MoveTowards(PM.transform.position + Vector3.up * 0.215f, PlayerPoint.position, num * 0.43f));
			if (num > 1f)
			{
				PM.Base.Animator.SetInteger("Vehicle Type", GetVehicle());
				PM.Base.PlayAnimation(GetVehicleName(), "On Vehicle");
				OnVehicleStats();
				if (GetVehicleName() == "Glider")
				{
					PM.Base.Camera.IsGlider = true;
				}
				Audio.PlayOneShot(EngineTurnOn, Audio.volume);
				OnPlayerState();
				IsMounted = true;
				VehicleState = 1;
			}
			return;
		}
		if (VehicleState == 1)
		{
			if (VehicleStart && Time.time - VehicleTime > 2f)
			{
				PM.Base.Animator.SetInteger("Vehicle Type", GetVehicle());
				PM.Base.PlayAnimation(GetVehicleName(), "On Vehicle");
				VehicleStart = false;
			}
			else
			{
				PM.Base.Animator.SetFloat("Vehicle Steer X", MoveAxes.x);
				PM.Base.Animator.SetFloat("Vehicle Steer Y", MoveAxes.y);
				if (!VehicleDamage)
				{
					if (!GoingReverse)
					{
						if (VehicleReverse)
						{
							PM.Base.PlayAnimation(GetVehicleName() + " Reverse End", "On Vehicle Reverse End");
							VehicleReverse = false;
						}
					}
					else if (!VehicleReverse)
					{
						PM.Base.PlayAnimation(GetVehicleName() + " Reverse Start", "On Vehicle Reverse");
						VehicleReverse = true;
					}
				}
			}
			SetPlayerTransform();
			return;
		}
		if (GetVehicleName() != "Glider")
		{
			JumpOffVel.y = Mathf.Lerp(JumpOffVel.y, 0f, Mathf.Clamp01((Time.time - VehicleTime) * 0.25f));
		}
		else if (Time.time - VehicleTime > 0.2f)
		{
			JumpOffVel = -Vector3.up * 25f * Time.fixedDeltaTime;
		}
		else
		{
			SetPlayerTransform();
		}
		PM.RBody.velocity = JumpOffVel;
		if (!(Time.time - VehicleTime > 0.5f))
		{
			return;
		}
		if (GetVehicleName() == "Glider")
		{
			PM.Base.transform.SetParent(null);
		}
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2, ignore: false);
			}
		}
		PM.Base.SetMachineState("StateAir");
		PM.shadow.CurVehicle = null;
		PM = null;
	}

	private void StateVehicleEnd()
	{
		Collider[] playerCols = PlayerCols;
		foreach (Collider collider in playerCols)
		{
			Collider[] vehicleCols = VehicleCols;
			foreach (Collider collider2 in vehicleCols)
			{
				Physics.IgnoreCollision(collider, collider2, ignore: false);
			}
		}
		if ((bool)PM)
		{
			PM.shadow.CurVehicle = null;
			PM = null;
		}
	}

	private void SetPlayerTransform()
	{
		if (PM.transform.localPosition != Vector3.zero)
		{
			PM.transform.localPosition = Vector3.zero;
		}
		if (PM.transform.localRotation != Quaternion.identity)
		{
			PM.transform.localRotation = Quaternion.identity;
		}
	}

	public virtual void OnPlayerState()
	{
	}

	private void OnVehicleStats()
	{
		PM.Base.HUD.OpenVehicle(MaxHP);
		if (IsShoot)
		{
			PM.Base.HUD.OpenWeapons(MaxAmmo, ShotName);
		}
	}

	private int GetVehicle()
	{
		switch (VehicleType)
		{
		case Type.Glider:
			return 1;
		case Type.HoverCraft:
			return 2;
		case Type.Jeep:
			return 3;
		default:
			return 0;
		}
	}

	public string GetVehicleName()
	{
		string result = "";
		switch (VehicleType)
		{
		case Type.Bike:
			result = "Bike";
			break;
		case Type.Glider:
			result = "Glider";
			break;
		case Type.HoverCraft:
			result = "Hover";
			break;
		case Type.Jeep:
			result = "Jeep";
			break;
		}
		return result;
	}

	private void OnTriggerStay(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && player.GetPrefab("shadow") && !(player.GetState() == "Vehicle") && !IsMounted && Time.time - MountedTime > 0.25f && Singleton<RInput>.Instance.P.GetButtonDown("Button Y"))
		{
			Mount(collider.GetComponent<PlayerManager>());
		}
	}

	public void OnImmediateDismount()
	{
		if ((bool)PM)
		{
			PM.Base.transform.SetParent(null);
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.Base.HUD.CloseVehicle();
			if (IsShoot)
			{
				PM.Base.HUD.CloseWeapons();
			}
			IsMounted = false;
			PM.shadow.CurVehicle = null;
			PM = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position + CenterOfMass, 0.5f);
	}
}
