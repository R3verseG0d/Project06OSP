using STHLua;
using UnityEngine;

public class Bike : VehicleBase
{
	[Header("Prefab")]
	public WheelCollider[] Wheels;

	public float MaxMotorTorque;

	public float BoosterTorque;

	[Range(0f, 1f)]
	public float TractionControl;

	public float SlipLimit;

	[Header("Parts")]
	public SkinnedMeshRenderer Renderer;

	public Light[] BoosterLights;

	public ParticleSystem[] BoosterFireFX;

	public ParticleSystem[] GunSmokeFX;

	[Header("Terrain")]
	public ParticleSystem[] WheelSmokeConcreteFX;

	public ParticleSystem[] WheelSmokeDirtFX;

	public ParticleSystem[] WheelSmokeGrassFX;

	public ParticleSystem BrakeSmokeConcreteFX;

	public ParticleSystem[] BrakeSmokeDirtFX;

	public ParticleSystem[] BrakeSmokeGrassFX;

	public ParticleSystem LandConcreteFX;

	public ParticleSystem LandDirtFX;

	public ParticleSystem LandGrassFX;

	[Header("Sounds")]
	public AudioSource TopSpeedEngine;

	public AudioSource Engine;

	public AudioSource Brake;

	public AudioSource ReverseEngine;

	public AudioClip BoosterSound;

	public AudioClip LandSound;

	private bool UseBooster;

	private bool Grounded;

	private bool Braking;

	private bool Recharged;

	private bool AirLaunch;

	private bool AirFalling;

	private float CurrentTorque;

	private float MotorTorque;

	private float BrakeTorque;

	private float SteerXAxis;

	private float SteerYAxis;

	private float BoosterTime;

	private float ButtonCooler = 0.25f;

	private float Speed;

	public float CurrentTopSpeed;

	private float SteeringAngle;

	private float GunTimeStamp;

	private float RechargeTime;

	private int ButtonCount;

	private int GunIndex;

	public override void Awake()
	{
		base.Awake();
		MaxHP = Vehicle_Param_Bike_Lua.f_max_hp;
		CurHP = Vehicle_Param_Bike_Lua.f_start_hp;
		MaxAmmo = Vehicle_Param_Bike_Lua.i_Missile_Bullet;
		CurAmmo = MaxAmmo;
		ShotName = Vehicle_Param_Bike_Lua.s_missile_shotname;
	}

	public override void Start()
	{
		base.Start();
		AttackBox.center = new Vector3(0f, Vehicle_Param_Bike_Lua.f_attack_box_y * 0.5f, 0f);
		AttackBox.size = new Vector3(Vehicle_Param_Bike_Lua.f_attack_box_x, Vehicle_Param_Bike_Lua.f_attack_box_y, Vehicle_Param_Bike_Lua.f_attack_box_z);
		CurrentTorque = MaxMotorTorque - TractionControl * MaxMotorTorque;
		CurrentTopSpeed = Vehicle_Param_Bike_Lua.f_MaxSpeed;
		Renderer.GetPropertyBlock(PropBlock, 4);
		PropBlock.SetFloat("_Intensity", 0f);
		Renderer.SetPropertyBlock(PropBlock, 4);
	}

	public override void Update()
	{
		base.Update();
		Speed = base.transform.InverseTransformDirection(_Rigidbody.velocity).z;
		Wheels[1].GetGroundHit(out var hit);
		MoveAxes.x = Mathf.Lerp(MoveAxes.x, Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (IsMounted ? 1f : 0f), Time.deltaTime * ((Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f) ? 1.5f : (TightenSteer() ? 4f : 3f)));
		SteeringAngle = Vehicle_Param_Bike_Lua.f_SteerAngle;
		for (int i = 0; i < BoosterLights.Length; i++)
		{
			BoosterLights[i].intensity = Mathf.Lerp(BoosterLights[i].intensity, UseBooster ? 2f : 0f, Time.deltaTime * 25f);
		}
		for (int j = 0; j < BoosterFireFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission = BoosterFireFX[j].emission;
			emission.enabled = UseBooster;
		}
		Wheels[0].GetGroundHit(out var hit2);
		Wheels[1].GetGroundHit(out var hit3);
		float num = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Jeep_Lua.f_MaxSpeed);
		ParticleSystem.EmissionModule emission2 = WheelSmokeConcreteFX[0].emission;
		emission2.enabled = Wheels[0].isGrounded && hit2.collider.transform.tag == "Normal";
		emission2.rateOverTime = num;
		for (int k = 0; k < 2; k++)
		{
			ParticleSystem.EmissionModule emission3 = WheelSmokeDirtFX[k].emission;
			emission3.enabled = Wheels[0].isGrounded && hit2.collider.transform.tag == "Dirt";
			emission3.rateOverTime = num;
		}
		for (int l = 0; l < 3; l++)
		{
			ParticleSystem.EmissionModule emission4 = WheelSmokeGrassFX[l].emission;
			emission4.enabled = Wheels[0].isGrounded && hit2.collider.transform.tag == "Grass";
			emission4.rateOverTime = num;
		}
		ParticleSystem.EmissionModule emission5 = WheelSmokeConcreteFX[1].emission;
		emission5.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Normal";
		emission5.rateOverTime = num;
		for (int m = 2; m < 4; m++)
		{
			ParticleSystem.EmissionModule emission6 = WheelSmokeDirtFX[m].emission;
			emission6.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Dirt";
			emission6.rateOverTime = num;
		}
		for (int n = 3; n < 6; n++)
		{
			ParticleSystem.EmissionModule emission7 = WheelSmokeGrassFX[n].emission;
			emission7.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Grass";
			emission7.rateOverTime = num;
		}
		ParticleSystem.EmissionModule emission8 = BrakeSmokeConcreteFX.emission;
		emission8.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Normal" && IsMounted && ((Singleton<RInput>.Instance.P.GetButton("Button X") && Wheels[1].rpm == 0f) || ((hit.forwardSlip >= SlipLimit || hit.forwardSlip <= (0f - SlipLimit) * ((Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f) ? 0.25f : 1f) || hit.sidewaysSlip >= SlipLimit * 0.25f || hit.sidewaysSlip <= (0f - SlipLimit) * 0.25f) && CurrentTorque >= 0f));
		for (int num2 = 0; num2 < BrakeSmokeDirtFX.Length; num2++)
		{
			ParticleSystem.EmissionModule emission9 = BrakeSmokeDirtFX[num2].emission;
			emission9.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Dirt" && IsMounted && ((Singleton<RInput>.Instance.P.GetButton("Button X") && Wheels[1].rpm == 0f) || ((hit.forwardSlip >= SlipLimit || hit.forwardSlip <= (0f - SlipLimit) * ((Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f) ? 0.25f : 1f) || hit.sidewaysSlip >= SlipLimit * 0.25f || hit.sidewaysSlip <= (0f - SlipLimit) * 0.25f) && CurrentTorque >= 0f));
		}
		for (int num3 = 0; num3 < BrakeSmokeGrassFX.Length; num3++)
		{
			ParticleSystem.EmissionModule emission10 = BrakeSmokeGrassFX[num3].emission;
			emission10.enabled = Wheels[1].isGrounded && hit3.collider.transform.tag == "Grass" && IsMounted && ((Singleton<RInput>.Instance.P.GetButton("Button X") && Wheels[1].rpm == 0f) || ((hit.forwardSlip >= SlipLimit || hit.forwardSlip <= (0f - SlipLimit) * ((Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f) ? 0.25f : 1f) || hit.sidewaysSlip >= SlipLimit * 0.25f || hit.sidewaysSlip <= (0f - SlipLimit) * 0.25f) && CurrentTorque >= 0f));
		}
		Renderer.GetPropertyBlock(PropBlock, 4);
		PropBlock.SetFloat("_Intensity", Mathf.Lerp(PropBlock.GetFloat("_Intensity"), (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X")) ? 1f : 0f, Time.deltaTime * 25f));
		Renderer.SetPropertyBlock(PropBlock, 4);
		IdleEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 2f) + 0.5f;
		IdleEngine.pitch = Mathf.Clamp(IdleEngine.pitch, IdleEngine.pitch, 1.4f);
		IdleEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(1f, 0f, Mathf.Clamp01(Speed * 0.1f)) : Mathf.Lerp(IdleEngine.volume, 0f, Time.deltaTime * 10f));
		TopSpeedEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 2f);
		if (TopSpeedEngine.pitch > 1.15f)
		{
			TopSpeedEngine.pitch = 1.15f;
		}
		TopSpeedEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(0f, 0.75f, Mathf.Clamp01(Speed * 0.1f)) : Mathf.Lerp(TopSpeedEngine.volume, 0f, Time.deltaTime * 10f));
		Engine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 1.5f);
		if (Engine.pitch > 0.8f)
		{
			Engine.pitch = 0.8f;
		}
		Engine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(0f, 0.5f, Mathf.Clamp01(Speed * 0.1f)) : Mathf.Lerp(Engine.volume, 0f, Time.deltaTime * 10f));
		ReverseEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Bike_Lua.f_MaxSpeed * 2f) + 0.6f;
		ReverseEngine.volume = Mathf.Lerp(ReverseEngine.volume, (IsMounted && GoingReverse) ? 0.5f : 0f, Time.deltaTime * 10f);
		Brake.volume = Mathf.Lerp(Brake.volume, (IsMounted && Wheels[1].isGrounded && ((Singleton<RInput>.Instance.P.GetButton("Button X") && Wheels[1].rpm == 0f) || ((hit.forwardSlip >= SlipLimit || hit.forwardSlip <= (0f - SlipLimit) * ((Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f) ? 0.25f : 1f) || hit.sidewaysSlip >= SlipLimit * 0.25f || hit.sidewaysSlip <= (0f - SlipLimit) * 0.25f) && CurrentTorque >= 0f))) ? 1f : 0f, Time.deltaTime * 10f);
		if (Wheels[0].isGrounded || Wheels[1].isGrounded)
		{
			if (!Grounded)
			{
				Audio.PlayOneShot(LandSound, Audio.volume);
				if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -Vector3.up, out var hitInfo, 5f, base.Collision_Mask))
				{
					string text = hitInfo.transform.tag;
					if (!(text == "Dirt"))
					{
						if (text == "Grass")
						{
							LandGrassFX.transform.position = hitInfo.point;
							LandGrassFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
							LandGrassFX.Play();
						}
						else
						{
							LandConcreteFX.transform.position = hitInfo.point;
							LandConcreteFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
							LandConcreteFX.Play();
						}
					}
					else
					{
						LandDirtFX.transform.position = hitInfo.point;
						LandDirtFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandDirtFX.Play();
					}
				}
				Grounded = true;
			}
		}
		else
		{
			Grounded = false;
		}
		SteerXAxis = Mathf.Lerp(SteerXAxis, (!IsMounted || (IsMounted && !Singleton<RInput>.Instance.P.GetButton("Button X") && Singleton<RInput>.Instance.P.GetButton("Button A"))) ? 1.25f : 1.75f, Time.deltaTime * 5f);
		SteerYAxis = Mathf.Lerp(SteerYAxis, (IsMounted && !Wheels[0].isGrounded && !Wheels[1].isGrounded) ? (Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") * 4f) : 0f, Time.deltaTime);
		if (IsMounted)
		{
			if ((bool)PM)
			{
				float num4 = Time.deltaTime * 2f;
				num4 = num4 * num4 * (3f - 2f * num4);
				float num5 = Mathf.Lerp(0.5f, 1.5f, ScaledVelSpd() / Vehicle_Param_Bike_Lua.f_MaxSpeed);
				PM.Base.CamOffset.x = Mathf.Lerp(PM.Base.CamOffset.x, (VehicleState == 1) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, num4);
				PM.Base.CamOffset.y = Mathf.Lerp(PM.Base.CamOffset.y, Common_Lua.c_camera.y + ((Wheels[0].isGrounded && Wheels[1].isGrounded) ? 0.1f : (-0.25f)), num4);
				PM.Base.CamOffset.z = Mathf.Lerp(PM.Base.CamOffset.z, UseBooster ? 2.5f : num5, UseBooster ? (Time.deltaTime * 2f) : num4);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !UseBooster && Time.time - BoosterTime > Vehicle_Param_Bike_Lua.f_boost_hold_time)
			{
				if (ButtonCooler > 0f && ButtonCount == 1 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					BoosterTime = Time.time;
					PM.Base.PlayAnimation(GetVehicleName() + " Dash", "On Vehicle Boost");
					Audio.PlayOneShot(BoosterSound, Audio.volume);
					float num6 = Mathf.Lerp(1f, 0f, Speed / Vehicle_Param_Bike_Lua.f_MaxSpeed * 1.25f);
					_Rigidbody.AddForce(_Rigidbody.velocity.normalized * (Vehicle_Param_Bike_Lua.f_boost_power_bias * num6), ForceMode.VelocityChange);
					Wheels[1].motorTorque = Vehicle_Param_Bike_Lua.f_MaxSpeed * ((Speed < Vehicle_Param_Bike_Lua.f_MaxSpeed) ? 1f : 1.25f);
					Wheels[1].motorTorque = Vehicle_Param_Bike_Lua.f_MaxSpeed * ((Speed < Vehicle_Param_Bike_Lua.f_MaxSpeed) ? 1f : 1.25f);
					UseBooster = true;
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
			if (UseBooster && Time.time - BoosterTime > Vehicle_Param_Bike_Lua.f_boost_hold_time)
			{
				BoosterTime = Time.time;
				if (!Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					PM.Base.Animator.SetTrigger("On Vehicle Boost End");
				}
				UseBooster = false;
			}
			if (!UseBooster)
			{
				if (Singleton<RInput>.Instance.P.GetButton("Button A") && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					MotorTorque = MaxMotorTorque;
					GoingReverse = false;
				}
				else if (Speed < 0f && Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					GoingReverse = true;
					if (Speed > -8f)
					{
						MotorTorque = 0f - Vehicle_Param_Bike_Lua.f_MaxBreakingTorque;
					}
					else
					{
						MotorTorque = 0f;
					}
				}
				else
				{
					MotorTorque = 0f;
					GoingReverse = false;
				}
			}
			else
			{
				MotorTorque = BoosterTorque;
				GoingReverse = false;
			}
			CurrentTopSpeed = ((!UseBooster) ? Mathf.MoveTowards(CurrentTopSpeed, Vehicle_Param_Bike_Lua.f_MaxSpeed, Time.deltaTime * 10f) : (Vehicle_Param_Bike_Lua.f_MaxSpeed * 1.25f));
			if (Singleton<RInput>.Instance.P.GetButton("Button X") && Speed > 0f && !UseBooster)
			{
				BrakeTorque = Vehicle_Param_Bike_Lua.f_MaxBreakingTorque * 2f;
				if (!Braking)
				{
					PM.Base.Animator.ResetTrigger("On Vehicle Brake End");
					PM.Base.PlayAnimation(GetVehicleName() + " Brake", "On Vehicle Brake");
					Braking = true;
				}
			}
			else
			{
				BrakeTorque = 0f;
				if (Braking)
				{
					if (!GoingReverse)
					{
						PM.Base.Animator.ResetTrigger("On Vehicle Brake");
						PM.Base.Animator.SetTrigger("On Vehicle Brake End");
					}
					Braking = false;
				}
			}
			if (AirLaunch && Time.time - LaunchTime > 1f && (Wheels[0].isGrounded || Wheels[1].isGrounded))
			{
				PM.Base.Animator.ResetTrigger("On Vehicle Air End");
				PM.Base.Animator.SetTrigger("On Vehicle Air End");
				AirLaunch = false;
			}
			if (!AirLaunch)
			{
				if (!AirFalling && !Wheels[0].isGrounded && !Wheels[1].isGrounded)
				{
					PM.Base.PlayAnimation(GetVehicleName() + " Fall", "On Vehicle Fall");
					AirFalling = true;
				}
				else if (AirFalling && (Wheels[0].isGrounded || Wheels[1].isGrounded))
				{
					PM.Base.Animator.ResetTrigger("On Vehicle Air End");
					PM.Base.Animator.SetTrigger("On Vehicle Air End");
					AirFalling = false;
				}
			}
			if (!IsShoot || !(Time.time - MountedTime > 0.5f))
			{
				return;
			}
			if (Singleton<RInput>.Instance.P.GetButton("Right Trigger") && Time.time - RechargeTime > 0.5f && CurAmmo > 0)
			{
				if (!IsShooting)
				{
					Recharged = false;
					PlayWeaponAnimation("On Shoot");
					IsShooting = true;
				}
				if (Time.time > GunTimeStamp)
				{
					GunTimeStamp = Time.time + Vehicle_Param_Bike_Lua.f_Missile_Interval;
					GadgetBullet componentInChildren = Object.Instantiate(Missile, WeaponPoints[GunIndex].position, Quaternion.LookRotation(WeaponPoints[GunIndex].forward) * Quaternion.Euler(-1f, 0f, 0f)).GetComponentInChildren<GadgetBullet>();
					componentInChildren.Player = PM.transform;
					componentInChildren.DealDamage = true;
					Object.Instantiate(MissileShootFX, WeaponPoints[GunIndex].position, Quaternion.LookRotation(WeaponPoints[GunIndex].forward)).transform.SetParent(base.transform);
					if (!PM.shadow.IsChaosBoost)
					{
						CurAmmo--;
					}
					GunIndex++;
					if (GunIndex > WeaponPoints.Length - 1)
					{
						GunIndex = 0;
					}
				}
				return;
			}
			if (IsShooting)
			{
				RechargeTime = Time.time;
				PlayWeaponAnimation((CurAmmo > 0) ? "On Shoot Stop" : "On Demount");
				if (CurAmmo == 0)
				{
					for (int num7 = 0; num7 < GunSmokeFX.Length; num7++)
					{
						GunSmokeFX[num7].Play();
					}
				}
				IsShooting = false;
			}
			if (Time.time - RechargeTime > Vehicle_Param_Bike_Lua.f_Missile_RecoveryTime && !Recharged && CurAmmo != MaxAmmo)
			{
				if (CurAmmo == 0)
				{
					PlayWeaponAnimation("On Mount");
				}
				CurAmmo = MaxAmmo;
				RechargeTime = Time.time;
				Audio.PlayOneShot(WeaponReload, Audio.volume);
				Recharged = true;
			}
		}
		else
		{
			MotorTorque = 0f;
			BrakeTorque = Vehicle_Param_Bike_Lua.f_MaxBreakingTorque * 2f;
			GoingReverse = false;
			IsShooting = false;
			UseBooster = false;
			AirLaunch = false;
			AirFalling = false;
		}
	}

	private void FixedUpdate()
	{
		WheelFrictionCurve sidewaysFriction = Wheels[0].sidewaysFriction;
		sidewaysFriction.extremumValue = ((!IsMounted || (IsMounted && (!Singleton<RInput>.Instance.P.GetButton("Button X") || (Singleton<RInput>.Instance.P.GetButton("Button X") && Speed < 0f)))) ? Mathf.Lerp(sidewaysFriction.extremumValue, 3f, Time.fixedDeltaTime) : 1.5f);
		Wheels[0].sidewaysFriction = sidewaysFriction;
		Wheels[1].GetGroundHit(out var hit);
		Wheels[1].motorTorque = (Wheels[1].GetGroundHit(out hit) ? MotorTorque : 0f);
		Wheels[1].motorTorque = (Wheels[1].GetGroundHit(out hit) ? MotorTorque : 0f);
		WheelFrictionCurve sidewaysFriction2 = Wheels[1].sidewaysFriction;
		sidewaysFriction2.extremumValue = ((!IsMounted || (IsMounted && (!Singleton<RInput>.Instance.P.GetButton("Button X") || (Singleton<RInput>.Instance.P.GetButton("Button X") && Speed < 0f)))) ? Mathf.Lerp(sidewaysFriction2.extremumValue, 3f, Time.fixedDeltaTime) : 0.1f);
		Wheels[1].sidewaysFriction = sidewaysFriction2;
		AdjustTorque(hit.forwardSlip);
		Wheels[1].brakeTorque = BrakeTorque;
		float num = Mathf.Clamp(SqrMag() / Vehicle_Param_Bike_Lua.f_MaxSpeed * 2.5f, 0f - SteerXAxis, SteerXAxis);
		Quaternion quaternion = Quaternion.AngleAxis(SteerYAxis * Time.fixedDeltaTime * 100f, base.transform.right);
		Quaternion quaternion2 = Quaternion.AngleAxis(MoveAxes.x * ((Speed < 0f && Wheels[0].isGrounded && Wheels[1].isGrounded) ? (-1f) : 1f) * num, base.transform.up) * base.transform.rotation;
		base.transform.rotation = quaternion * quaternion2;
		Vector3 vector = Vector3.Cross(base.transform.up, Vector3.up);
		Vector3 vector2 = vector.normalized * vector.magnitude * 30f;
		vector2.x *= (TightenSteer() ? 0.5f : 0.2f);
		vector2.y = 0f;
		vector2.z *= 0.25f;
		vector2 -= _Rigidbody.angularVelocity;
		_Rigidbody.AddTorque(vector2 * _Rigidbody.mass * 0.02f, ForceMode.Impulse);
		ApplyPosAndRotToVisuals(Wheels[0]);
		ApplyPosAndRotToVisuals(Wheels[1]);
		if (ScaledVelSpd() > CurrentTopSpeed && !AirLaunch)
		{
			_Rigidbody.velocity = CurrentTopSpeed / 2.2369363f * _Rigidbody.velocity.normalized;
		}
		if (!Wheels[0].isGrounded || !Wheels[1].isGrounded)
		{
			_Rigidbody.AddForce(Physics.gravity * 0.25f, ForceMode.Acceleration);
		}
	}

	private void AdjustTorque(float ForwardSlip)
	{
		if (ForwardSlip >= SlipLimit && !GoingReverse && CurrentTorque >= 0f)
		{
			CurrentTorque -= 10f * TractionControl;
			return;
		}
		CurrentTorque += 10f * TractionControl;
		if (CurrentTorque > MaxMotorTorque)
		{
			CurrentTorque = MaxMotorTorque;
		}
	}

	private bool TightenSteer()
	{
		float axis = Singleton<RInput>.Instance.P.GetAxis("Left Stick X");
		float num = Vector3.Dot(Vector3.up, base.transform.right);
		if (Wheels[0].isGrounded && Wheels[1].isGrounded)
		{
			if (axis != 0f && (!(num > 0f) || !(axis > 0f)))
			{
				if (num < 0f)
				{
					return axis < 0f;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override void OnJumpPanel(Vector3 Velocity)
	{
		base.OnJumpPanel(Velocity);
		PM.Base.PlayAnimation(GetVehicleName() + " Launch", "On Vehicle Launch");
		AirLaunch = true;
	}

	public override int Hit()
	{
		if (ScaledVelSpd() < Vehicle_Param_Bike_Lua.f_speed_low)
		{
			return Vehicle_Param_Bike_Lua.i_damage_low;
		}
		if (ScaledVelSpd() < Vehicle_Param_Bike_Lua.f_speed_mid)
		{
			return Vehicle_Param_Bike_Lua.i_damage_mid;
		}
		return Vehicle_Param_Bike_Lua.i_damage_high;
	}

	public override float Damage()
	{
		return Vehicle_Param_Bike_Lua.i_damage_high;
	}

	public override void OnMount()
	{
		PlayWeaponAnimation("On Mount");
	}

	public override void OnDemount()
	{
		PlayWeaponAnimation("On Demount");
	}
}
