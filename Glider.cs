using System.Collections;
using STHLua;
using UnityEngine;

public class Glider : VehicleBase
{
	[Header("Prefab")]
	public Transform Pivot;

	[Header("Parts")]
	public Animator Body;

	public Animator Cargo;

	public Transform[] Propellers;

	public ParticleSystem[] EngineLowFX;

	public ParticleSystem[] EngineAccelFX;

	public ParticleSystem[] BoostFX;

	public Light[] EngineLowLights;

	public Light[] EngineAccelLights;

	public Light[] BoostLights;

	[Header("Sounds")]
	public AudioSource EngineRev;

	public AudioSource WindCurve;

	public AudioSource Wind;

	public AudioClip BoosterSound;

	private Vector3 BodyDir;

	private Vector3 Forward;

	private Vector3 ReflVel;

	private Vector2 RotAxes;

	private Vector2 SteerAxes;

	private bool UseBoost;

	private float CurSpeed;

	private float RawSpeed;

	private float BoostTime;

	private float ButtonCooler = 0.25f;

	private float ShootTime;

	private float Proper0;

	private float Proper1;

	private float DamageTime;

	private float DamageAmount;

	private int ButtonCount;

	private int MissileIndex;

	public override void Awake()
	{
		base.Awake();
		MaxHP = Vehicle_Param_Glider_Lua.f_max_hp;
		CurHP = Vehicle_Param_Glider_Lua.f_start_hp;
		MaxAmmo = 2;
		CurAmmo = MaxAmmo;
		ShotName = Vehicle_Param_Glider_Lua.s_missile_shotname;
	}

	public override void Start()
	{
		base.Start();
		AttackBox.size = new Vector3(Vehicle_Param_Glider_Lua.f_attack_box_x, Vehicle_Param_Glider_Lua.f_attack_box_y, Vehicle_Param_Glider_Lua.f_attack_box_z);
		RotAxes = new Vector2(base.transform.eulerAngles.x, base.transform.eulerAngles.y);
		Forward = base.transform.forward;
	}

	private float InputY()
	{
		return Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * ((Singleton<Settings>.Instance.settings.InvertGliderY == 1) ? 1f : (-1f));
	}

	public override void Update()
	{
		base.Update();
		for (int i = 0; i < EngineLowFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = EngineLowFX[i].emission;
			emission.enabled = !IsMounted || (IsMounted && !UseBoost && (VehicleStart || (!VehicleStart && !Singleton<RInput>.Instance.P.GetButton("Button A"))));
		}
		for (int j = 0; j < EngineAccelFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission2 = EngineAccelFX[j].emission;
			emission2.enabled = IsMounted && !VehicleStart && !UseBoost && Singleton<RInput>.Instance.P.GetButton("Button A");
		}
		for (int k = 0; k < BoostFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission3 = BoostFX[k].emission;
			emission3.enabled = IsMounted && !VehicleStart && UseBoost;
		}
		for (int l = 0; l < EngineLowLights.Length; l++)
		{
			EngineLowLights[l].intensity = Mathf.Lerp(EngineLowLights[l].intensity, (!IsMounted || (IsMounted && !UseBoost && !Singleton<RInput>.Instance.P.GetButton("Button A"))) ? 3f : 0f, Time.deltaTime * 15f);
		}
		for (int m = 0; m < EngineAccelLights.Length; m++)
		{
			EngineAccelLights[m].intensity = Mathf.Lerp(EngineAccelLights[m].intensity, (IsMounted && !VehicleStart && !UseBoost && Singleton<RInput>.Instance.P.GetButton("Button A")) ? 3f : 0f, Time.deltaTime * 15f);
		}
		for (int n = 0; n < BoostLights.Length; n++)
		{
			BoostLights[n].intensity = Mathf.Lerp(BoostLights[n].intensity, (IsMounted && !VehicleStart && UseBoost) ? 3f : 0f, Time.deltaTime * 15f);
		}
		IdleEngine.pitch = Mathf.Lerp(0.85f, 1.3f, CurSpeed / 28f);
		IdleEngine.volume = Mathf.Lerp(IdleEngine.volume, IsMounted ? 0.4f : 0.1f, Time.deltaTime * 10f);
		EngineRev.pitch = Mathf.Lerp(0.85f, 1.3f, CurSpeed / 28f);
		EngineRev.volume = Mathf.Lerp(EngineRev.volume, (!IsMounted) ? 0f : (Singleton<RInput>.Instance.P.GetButton("Button A") ? 0.3f : 0.15f), Time.deltaTime * 10f);
		Wind.volume = Mathf.Lerp(0f, 0.8f, CurSpeed / 49f);
		Forward = Vector3.Lerp(Forward, base.transform.forward, Time.deltaTime * ((Singleton<RInput>.Instance.P.GetAxis("Left Stick X") != 0f || Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") != 0f) ? 0.25f : 1f));
		WindCurve.volume = Mathf.Lerp(0.75f, 0f, Vector3.Dot(Forward, base.transform.forward));
		CurSpeed = Mathf.Lerp(CurSpeed, (!IsMounted) ? 0f : (UseBoost ? 49f : (Singleton<RInput>.Instance.P.GetButton("Button A") ? 28f : 14f)), Time.deltaTime * ((!UseBoost && !VehicleDamage) ? 1f : 3f));
		MoveAxes.x = Mathf.Lerp(MoveAxes.x, (IsMounted && !VehicleStart) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, Time.deltaTime * 3f);
		MoveAxes.y = Mathf.Lerp(MoveAxes.y, (IsMounted && !VehicleStart) ? InputY() : 0f, Time.deltaTime * 3f);
		SteerAxes.x = Mathf.Lerp(SteerAxes.x, (IsMounted && !VehicleStart) ? (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * 4f) : 0f, Time.deltaTime * 1.5f);
		SteerAxes.y = Mathf.Lerp(SteerAxes.y, (IsMounted && !VehicleStart) ? (InputY() * 4f) : 0f, Time.deltaTime * 2.5f);
		RotAxes.y += SteerAxes.x * Time.deltaTime * 25f;
		RotAxes.x += SteerAxes.y * Time.deltaTime * 25f;
		RotAxes.x = ClampAngle(Mathf.Lerp(RotAxes.x, 0f, Time.deltaTime * 2.5f), -45f, 45f);
		base.transform.rotation = Quaternion.Euler(RotAxes.x, RotAxes.y, 0f);
		float num = Time.deltaTime * 7.5f;
		num = num * num * (3f - 2f * num);
		BodyDir.z = Mathf.Lerp(BodyDir.z, (0f - SteerAxes.x) * 15f, num);
		Pivot.localEulerAngles = BodyDir;
		Proper0 = Mathf.Lerp(Proper0, 30f + ((!IsMounted) ? (-90f) : ((!VehicleStart) ? (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * 30f) : 0f)), Time.deltaTime * 3f);
		Propellers[0].localEulerAngles = new Vector3(Proper0, 0f, 0f);
		Proper1 = Mathf.Lerp(Proper1, 30f + ((!IsMounted) ? (-90f) : ((!VehicleStart) ? ((0f - Singleton<RInput>.Instance.P.GetAxis("Left Stick X")) * 30f) : 0f)), Time.deltaTime * 3f);
		Propellers[1].localEulerAngles = new Vector3(Proper1, 0f, 0f);
		if (!IsMounted)
		{
			return;
		}
		if ((bool)PM)
		{
			float num2 = Time.deltaTime * 2f;
			num2 = num2 * num2 * (3f - 2f * num2);
			PM.Base.CamOffset.x = Mathf.Lerp(PM.Base.CamOffset.x, (VehicleState == 1 && !VehicleStart) ? (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * 2f) : 0f, num2);
			PM.Base.CamOffset.y = Mathf.Lerp(PM.Base.CamOffset.y, Common_Lua.c_camera.y + 0.25f + ((VehicleState == 1 && !VehicleStart) ? ((0f - InputY()) * 2f) : 0f), num2);
		}
		if (VehicleDamage && Time.time - DamageTime > DamageAmount)
		{
			VehicleDamage = false;
		}
		if (!VehicleStart && !VehicleDamage && Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !UseBoost && Time.time - BoostTime > Vehicle_Param_Glider_Lua.f_boost_effect_time)
		{
			if (ButtonCooler > 0f && ButtonCount == 1)
			{
				BoostTime = Time.time;
				PM.Base.PlayAnimation(GetVehicleName() + " Boost", "On Vehicle Boost");
				Cargo.SetTrigger("On Boost");
				Audio.PlayOneShot(BoosterSound, Audio.volume);
				UseBoost = true;
			}
			else
			{
				ButtonCooler = 0.25f;
				ButtonCount++;
			}
		}
		if (ButtonCooler > 0f)
		{
			ButtonCooler -= 1f * Time.deltaTime;
		}
		else
		{
			ButtonCount = 0;
		}
		if (UseBoost && Time.time - BoostTime > Vehicle_Param_Glider_Lua.f_boost_hold_time)
		{
			BoostTime = Time.time;
			if (!VehicleDamage)
			{
				PM.Base.Animator.SetTrigger("On Vehicle Boost End");
				Cargo.SetTrigger("On Boost End");
			}
			UseBoost = false;
		}
		if (IsShoot && !VehicleStart && !VehicleDamage && !UseBoost && Time.time - MountedTime > 0.5f && Time.time - ShootTime > 0.25f && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && CurAmmo > 0)
		{
			ShootTime = Time.time;
			if (!PM.shadow.IsChaosBoost)
			{
				WeaponAnimators[MissileIndex].SetTrigger("On Demount");
			}
			PM.Base.PlayAnimation(GetVehicleName() + ((MissileIndex == 0) ? " Missile R" : " Missile L"), (MissileIndex == 0) ? "On Vehicle Missile R" : "On Vehicle Missile L");
			Object.Instantiate(Missile, WeaponPoints[MissileIndex].position, Quaternion.LookRotation(WeaponPoints[MissileIndex].forward)).GetComponentInChildren<GadgetMissile>().Player = PM.transform;
			Object.Instantiate(MissileShootFX, WeaponPoints[MissileIndex].position, Quaternion.LookRotation(WeaponPoints[MissileIndex].forward)).transform.SetParent(base.transform);
			MissileIndex++;
			StartCoroutine(Reload());
			if (MissileIndex > WeaponPoints.Length - 1)
			{
				MissileIndex = 0;
			}
			if (!PM.shadow.IsChaosBoost)
			{
				CurAmmo--;
			}
		}
	}

	private void FixedUpdate()
	{
		_Rigidbody.velocity = base.transform.forward * RawSpeed + ReflVel;
		float num = Mathf.Clamp01(Time.time - DamageTime);
		ReflVel = Vector3.MoveTowards(ReflVel, Vector3.zero, num * 0.5f);
		RawSpeed = Mathf.MoveTowards(RawSpeed, CurSpeed, num * 0.5f);
	}

	private IEnumerator Reload()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		while (Timer <= Vehicle_Param_Glider_Lua.f_missile_recharge_time)
		{
			Timer = Time.time - StartTime;
			yield return new WaitForFixedUpdate();
		}
		if (CurAmmo == MaxAmmo)
		{
			yield break;
		}
		if (CurAmmo == 1 && MissileIndex == 1)
		{
			MissileIndex = 0;
			WeaponAnimators[CurAmmo - 1].SetTrigger("On Mount");
		}
		else
		{
			if (CurAmmo == 0 && MissileIndex == 1)
			{
				MissileIndex = 0;
			}
			WeaponAnimators[CurAmmo].SetTrigger("On Mount");
		}
		CurAmmo++;
		Audio.PlayOneShot(WeaponReload, Audio.volume * 0.5f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (IsMounted && !VehicleStart && !VehicleDamage && (collision.gameObject.layer == LayerMask.NameToLayer("Default") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerCollision") || (collision.gameObject.layer == LayerMask.NameToLayer("BreakableObj") && !UseBoost) || (bool)collision.gameObject.GetComponent<HurtVehicle>()))
		{
			if (collision.gameObject.layer == LayerMask.NameToLayer("BreakableObj"))
			{
				collision.gameObject.SendMessage("OnHit", new HitInfo(PM ? PM.transform : base.transform, Vector3.zero, Hit()), SendMessageOptions.DontRequireReceiver);
			}
			ReflVel = Vector3.Reflect(_Rigidbody.velocity, collision.contacts[0].normal);
			RawSpeed = 0f;
			OnVehicleHit(Damage() * ((!UseBoost) ? 1f : 2f));
			OnCrash();
		}
	}

	public override int Hit()
	{
		if (UseBoost)
		{
			return 10;
		}
		return 0;
	}

	public override float Damage()
	{
		return 5f;
	}

	private void OnCrash()
	{
		VehicleDamage = true;
		DamageTime = Time.time;
		DamageAmount = ((!UseBoost) ? 1f : 2f);
		Body.SetTrigger((!UseBoost) ? "On Damage A" : "On Damage B");
		Cargo.SetTrigger((!UseBoost) ? "On Damage A" : "On Damage B");
		PM.Base.PlayAnimation(GetVehicleName() + ((!UseBoost) ? " Damage A" : " Damage B"), (!UseBoost) ? "On Vehicle Damage A" : "On Vehicle Damage B");
		Audio.PlayOneShot(Crash, Audio.volume * 0.5f);
	}

	public override void OnDamage(float Damage)
	{
		if (!VehicleDamage)
		{
			base.OnDamage(Damage);
			OnCrash();
		}
	}

	public override void OnMount()
	{
		PlayWeaponAnimation("On Mount");
		Cargo.SetTrigger("On Mount");
	}

	public override void OnDemount()
	{
		PlayWeaponAnimation("On Demount");
		Cargo.SetTrigger("On Demount");
	}
}
