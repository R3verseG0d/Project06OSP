using System.Collections;
using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Jeep : VehicleBase
{
	[Header("Prefab")]
	public List<AxleInfo> AxleInfos;

	public float MaxMotorTorque;

	public float BoosterTorque;

	[Range(0f, 1f)]
	public float TractionControl;

	public float SlipLimit;

	public Transform PlayerSubPoint;

	[Header("Parts")]
	public SkinnedMeshRenderer Renderer;

	public Animator Animator;

	public Transform Handle;

	public Transform[] FrontGuards;

	public Transform[] LeftSuspensions;

	public Transform[] RightSuspensions;

	public TrailRenderer[] BoosterTrails;

	public Light[] BoosterLights;

	public ParticleSystem[] BoosterFX;

	[Header("Terrain")]
	public ParticleSystem[] FrontWheelSmokeConcreteFX;

	public ParticleSystem[] RearWheelSmokeConcreteFX;

	public ParticleSystem[] FrontWheelSmokeSandFX;

	public ParticleSystem[] RearWheelSmokeSandFX;

	public ParticleSystem[] FrontWheelSmokeDirtFX;

	public ParticleSystem[] RearWheelSmokeDirtFX;

	public ParticleSystem[] FrontWheelSmokeSnowFX;

	public ParticleSystem[] RearWheelSmokeSnowFX;

	public ParticleSystem[] FrontWheelSmokeGrassFX;

	public ParticleSystem[] RearWheelSmokeGrassFX;

	public ParticleSystem LandConcreteFX;

	public ParticleSystem LandSandFX;

	public ParticleSystem LandDirtFX;

	public ParticleSystem LandSnowFX;

	public ParticleSystem LandGrassFX;

	[Header("Sounds")]
	public AudioSource TopSpeedEngine;

	public AudioSource Brake;

	public AudioSource ReverseEngine;

	public AudioSource RoadNoise;

	public AudioClip BoosterSound;

	public AudioClip LandSound;

	private bool UseBooster;

	private bool Grounded;

	private bool FlipBack;

	private bool AirLaunch;

	private float CurrentTorque;

	private float MotorTorque;

	private float BrakeTorque;

	private float SteerXAxis;

	private float BoosterTime;

	private float ButtonCooler = 0.25f;

	private float ShootTime;

	private float CurrentTopSpeed;

	private float SteeringAngle;

	private float TrailAlpha;

	private int ButtonCount;

	private int MissileIndex;

	public override void Awake()
	{
		base.Awake();
		MaxHP = Vehicle_Param_Jeep_Lua.f_max_hp;
		CurHP = Vehicle_Param_Jeep_Lua.f_start_hp;
		MaxAmmo = 2;
		CurAmmo = MaxAmmo;
		ShotName = Vehicle_Param_Jeep_Lua.s_missile_shotname;
	}

	public override void Start()
	{
		base.Start();
		AttackBox.center = new Vector3(0f, Vehicle_Param_Jeep_Lua.f_attack_box_y * 0.5f, 0f);
		AttackBox.size = new Vector3(Vehicle_Param_Jeep_Lua.f_attack_box_x, Vehicle_Param_Jeep_Lua.f_attack_box_y, Vehicle_Param_Jeep_Lua.f_attack_box_z);
		CurrentTorque = MaxMotorTorque - TractionControl * MaxMotorTorque;
		CurrentTopSpeed = Vehicle_Param_Jeep_Lua.f_MaxSpeed;
		BoosterTime = Time.time;
		Renderer.GetPropertyBlock(PropBlock, 3);
		PropBlock.SetFloat("_Intensity", 0f);
		Renderer.SetPropertyBlock(PropBlock, 3);
	}

	public void ApplyPosToVisuals(WheelCollider Collider, Transform Part, bool RotateOnY = false)
	{
		if (Collider.transform.childCount != 0)
		{
			Collider.GetWorldPose(out var pos, out var _);
			Part.position = pos;
			if (RotateOnY)
			{
				Part.localEulerAngles = new Vector3(Part.localEulerAngles.x, SteeringAngle * SteerXAxis * MoveAxes.x, Part.localEulerAngles.z);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		float z = base.transform.InverseTransformDirection(_Rigidbody.velocity).z;
		MoveAxes.x = Mathf.Lerp(MoveAxes.x, Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (IsMounted ? 1f : 0f), Time.deltaTime * ((Singleton<RInput>.Instance.P.GetButton("Button X") && z > 0f) ? 1f : 3f));
		foreach (AxleInfo axleInfo in AxleInfos)
		{
			if (axleInfo.Steer)
			{
				SteeringAngle = Vehicle_Param_Jeep_Lua.f_SteerAngle;
			}
		}
		Handle.localEulerAngles = new Vector3(Handle.localEulerAngles.x, Handle.localEulerAngles.y, (!GoingReverse) ? ((0f - MoveAxes.x) * (SteeringAngle * 1.25f)) : 0f);
		for (int i = 0; i < BoosterLights.Length; i++)
		{
			BoosterLights[i].intensity = Mathf.Lerp(BoosterLights[i].intensity, UseBooster ? 2f : 0f, Time.deltaTime * 10f);
		}
		for (int j = 0; j < BoosterFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission = BoosterFX[j].emission;
			emission.enabled = UseBooster;
		}
		TrailAlpha = Mathf.Lerp(TrailAlpha, UseBooster ? 1f : 0f, Time.deltaTime * 10f);
		Gradient gradient = new Gradient();
		gradient.SetKeys(new GradientColorKey[2]
		{
			new GradientColorKey(new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0f),
			new GradientColorKey(new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue), 1f)
		}, new GradientAlphaKey[2]
		{
			new GradientAlphaKey(TrailAlpha, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		for (int k = 0; k < BoosterTrails.Length; k++)
		{
			BoosterTrails[k].colorGradient = gradient;
		}
		AxleInfos[0].LeftWheel.GetGroundHit(out var hit);
		AxleInfos[0].RightWheel.GetGroundHit(out var hit2);
		AxleInfos[1].LeftWheel.GetGroundHit(out var hit3);
		AxleInfos[1].RightWheel.GetGroundHit(out var hit4);
		float num = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Jeep_Lua.f_MaxSpeed);
		ParticleSystem.EmissionModule emission2 = FrontWheelSmokeConcreteFX[0].emission;
		emission2.enabled = AxleInfos[0].LeftWheel.isGrounded && hit.collider.transform.tag == "Normal";
		emission2.rateOverTime = num;
		ParticleSystem.EmissionModule emission3 = FrontWheelSmokeSandFX[0].emission;
		emission3.enabled = AxleInfos[0].LeftWheel.isGrounded && hit.collider.transform.tag == "Sand";
		emission3.rateOverTime = num;
		for (int l = 0; l < 2; l++)
		{
			ParticleSystem.EmissionModule emission4 = FrontWheelSmokeDirtFX[l].emission;
			emission4.enabled = AxleInfos[0].LeftWheel.isGrounded && hit.collider.transform.tag == "Dirt";
			emission4.rateOverTime = num;
		}
		for (int m = 0; m < 2; m++)
		{
			ParticleSystem.EmissionModule emission5 = FrontWheelSmokeSnowFX[m].emission;
			emission5.enabled = AxleInfos[0].LeftWheel.isGrounded && hit.collider.transform.tag == "Snow";
			emission5.rateOverTime = num;
		}
		for (int n = 0; n < 3; n++)
		{
			ParticleSystem.EmissionModule emission6 = FrontWheelSmokeGrassFX[n].emission;
			emission6.enabled = AxleInfos[0].LeftWheel.isGrounded && hit.collider.transform.tag == "Grass";
			emission6.rateOverTime = num;
		}
		ParticleSystem.EmissionModule emission7 = FrontWheelSmokeConcreteFX[1].emission;
		emission7.enabled = AxleInfos[0].RightWheel.isGrounded && hit2.collider.transform.tag == "Normal";
		emission7.rateOverTime = num;
		ParticleSystem.EmissionModule emission8 = FrontWheelSmokeSandFX[1].emission;
		emission8.enabled = AxleInfos[0].RightWheel.isGrounded && hit2.collider.transform.tag == "Sand";
		emission8.rateOverTime = num;
		for (int num2 = 2; num2 < 4; num2++)
		{
			ParticleSystem.EmissionModule emission9 = FrontWheelSmokeDirtFX[num2].emission;
			emission9.enabled = AxleInfos[0].RightWheel.isGrounded && hit2.collider.transform.tag == "Dirt";
			emission9.rateOverTime = num;
		}
		for (int num3 = 2; num3 < 4; num3++)
		{
			ParticleSystem.EmissionModule emission10 = FrontWheelSmokeSnowFX[num3].emission;
			emission10.enabled = AxleInfos[0].RightWheel.isGrounded && hit2.collider.transform.tag == "Snow";
			emission10.rateOverTime = num;
		}
		for (int num4 = 3; num4 < 6; num4++)
		{
			ParticleSystem.EmissionModule emission11 = FrontWheelSmokeGrassFX[num4].emission;
			emission11.enabled = AxleInfos[0].RightWheel.isGrounded && hit2.collider.transform.tag == "Grass";
			emission11.rateOverTime = num;
		}
		ParticleSystem.EmissionModule emission12 = RearWheelSmokeConcreteFX[0].emission;
		emission12.enabled = AxleInfos[1].LeftWheel.isGrounded && hit3.collider.transform.tag == "Normal";
		emission12.rateOverTime = num;
		ParticleSystem.EmissionModule emission13 = RearWheelSmokeSandFX[0].emission;
		emission13.enabled = AxleInfos[1].LeftWheel.isGrounded && hit3.collider.transform.tag == "Sand";
		emission13.rateOverTime = num;
		for (int num5 = 0; num5 < 2; num5++)
		{
			ParticleSystem.EmissionModule emission14 = RearWheelSmokeDirtFX[num5].emission;
			emission14.enabled = AxleInfos[1].LeftWheel.isGrounded && hit3.collider.transform.tag == "Dirt";
			emission14.rateOverTime = num;
		}
		for (int num6 = 0; num6 < 2; num6++)
		{
			ParticleSystem.EmissionModule emission15 = RearWheelSmokeSnowFX[num6].emission;
			emission15.enabled = AxleInfos[1].LeftWheel.isGrounded && hit3.collider.transform.tag == "Snow";
			emission15.rateOverTime = num;
		}
		for (int num7 = 0; num7 < 3; num7++)
		{
			ParticleSystem.EmissionModule emission16 = RearWheelSmokeGrassFX[num7].emission;
			emission16.enabled = AxleInfos[1].LeftWheel.isGrounded && hit3.collider.transform.tag == "Grass";
			emission16.rateOverTime = num;
		}
		ParticleSystem.EmissionModule emission17 = RearWheelSmokeConcreteFX[1].emission;
		emission17.enabled = AxleInfos[1].RightWheel.isGrounded && hit4.collider.transform.tag == "Normal";
		emission17.rateOverTime = num;
		ParticleSystem.EmissionModule emission18 = RearWheelSmokeSandFX[1].emission;
		emission18.enabled = AxleInfos[1].RightWheel.isGrounded && hit4.collider.transform.tag == "Sand";
		emission18.rateOverTime = num;
		for (int num8 = 2; num8 < 4; num8++)
		{
			ParticleSystem.EmissionModule emission19 = RearWheelSmokeDirtFX[num8].emission;
			emission19.enabled = AxleInfos[1].RightWheel.isGrounded && hit4.collider.transform.tag == "Dirt";
			emission19.rateOverTime = num;
		}
		for (int num9 = 2; num9 < 4; num9++)
		{
			ParticleSystem.EmissionModule emission20 = RearWheelSmokeSnowFX[num9].emission;
			emission20.enabled = AxleInfos[1].RightWheel.isGrounded && hit4.collider.transform.tag == "Snow";
			emission20.rateOverTime = num;
		}
		for (int num10 = 3; num10 < 6; num10++)
		{
			ParticleSystem.EmissionModule emission21 = RearWheelSmokeGrassFX[num10].emission;
			emission21.enabled = AxleInfos[1].RightWheel.isGrounded && hit4.collider.transform.tag == "Grass";
			emission21.rateOverTime = num;
		}
		Renderer.GetPropertyBlock(PropBlock, 3);
		PropBlock.SetFloat("_Intensity", Mathf.Lerp(PropBlock.GetFloat("_Intensity"), (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X")) ? 1f : 0f, Time.deltaTime * 25f));
		Renderer.SetPropertyBlock(PropBlock, 3);
		IdleEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 2f) + 0.8f;
		IdleEngine.pitch = Mathf.Clamp(IdleEngine.pitch, IdleEngine.pitch, 1.4f);
		IdleEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(1f, 0f, Mathf.Clamp01(z * 0.1f)) : Mathf.Lerp(IdleEngine.volume, 0f, Time.deltaTime * 10f));
		TopSpeedEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 2f) + 0.5f;
		if (TopSpeedEngine.pitch > 1.4f)
		{
			TopSpeedEngine.pitch = 1.4f;
		}
		TopSpeedEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(0f, 1f, Mathf.Clamp01(z * 0.1f)) : Mathf.Lerp(TopSpeedEngine.volume, 0f, Time.deltaTime * 10f));
		ReverseEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Jeep_Lua.f_MaxSpeed * 2f) + 0.4f;
		ReverseEngine.volume = Mathf.Lerp(ReverseEngine.volume, (IsMounted && GoingReverse) ? 1f : 0f, Time.deltaTime * 10f);
		WheelHit[] array = new WheelHit[2];
		AxleInfos[1].LeftWheel.GetGroundHit(out array[0]);
		AxleInfos[1].RightWheel.GetGroundHit(out array[1]);
		Brake.volume = Mathf.Lerp(Brake.volume, (IsMounted && (AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded) && ((Singleton<RInput>.Instance.P.GetButton("Button X") && (AxleInfos[1].LeftWheel.rpm == 0f || AxleInfos[1].RightWheel.rpm == 0f)) || ((array[0].forwardSlip >= SlipLimit || array[1].forwardSlip >= SlipLimit) && CurrentTorque >= 0f))) ? 1f : 0f, Time.deltaTime * 10f);
		RoadNoise.volume = ((AxleInfos[0].LeftWheel.isGrounded || AxleInfos[0].RightWheel.isGrounded || AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded) ? Mathf.Lerp(0f, 1f, Mathf.Clamp01(SqrMag() * 0.05f)) : Mathf.Lerp(RoadNoise.volume, 0f, Time.deltaTime * 10f));
		if (AxleInfos[0].LeftWheel.isGrounded || AxleInfos[0].RightWheel.isGrounded || AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded)
		{
			if (!Grounded)
			{
				Audio.PlayOneShot(LandSound, Audio.volume);
				if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -Vector3.up, out var hitInfo, 5f, base.Collision_Mask))
				{
					switch (hitInfo.transform.tag)
					{
					case "Sand":
						LandSandFX.transform.position = hitInfo.point;
						LandSandFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandSandFX.Play();
						break;
					case "Dirt":
						LandDirtFX.transform.position = hitInfo.point;
						LandDirtFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandDirtFX.Play();
						break;
					case "Snow":
						LandSnowFX.transform.position = hitInfo.point;
						LandSnowFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandSnowFX.Play();
						break;
					case "Grass":
						LandGrassFX.transform.position = hitInfo.point;
						LandGrassFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandGrassFX.Play();
						break;
					default:
						LandConcreteFX.transform.position = hitInfo.point;
						LandConcreteFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandConcreteFX.Play();
						break;
					}
				}
				Grounded = true;
			}
		}
		else
		{
			Grounded = false;
		}
		SteerXAxis = Mathf.Lerp(SteerXAxis, (!IsMounted || (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X") && z < 0f)) ? 1f : Mathf.Lerp(1f, 0.5f, SqrMag() / Vehicle_Param_Jeep_Lua.f_MaxSpeed), Time.deltaTime * 2f);
		if (IsMounted)
		{
			if ((bool)PM)
			{
				float num11 = Time.deltaTime * 2f;
				num11 = num11 * num11 * (3f - 2f * num11);
				float num12 = Mathf.Lerp(0f, 1f, ScaledVelSpd() / Vehicle_Param_Jeep_Lua.f_MaxSpeed);
				PM.Base.CamOffset.x = Mathf.Lerp(PM.Base.CamOffset.x, (VehicleState == 1) ? (Singleton<RInput>.Instance.P.GetAxis("Left Stick X") + 0.35f) : 0f, num11);
				PM.Base.CamOffset.y = Mathf.Lerp(PM.Base.CamOffset.y, Common_Lua.c_camera.y + ((AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded) ? 0.1f : (-0.25f)), num11);
				PM.Base.CamOffset.z = Mathf.Lerp(PM.Base.CamOffset.z, UseBooster ? 2f : num12, UseBooster ? (Time.deltaTime * 2f) : num11);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !UseBooster && Time.time - BoosterTime > Vehicle_Param_Jeep_Lua.f_boost_effect_time)
			{
				if (ButtonCooler > 0f && ButtonCount == 1 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					BoosterTime = Time.time;
					Animator.SetTrigger("On Booster");
					Audio.PlayOneShot(BoosterSound, Audio.volume * 0.8f);
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
			if (UseBooster && Time.time - BoosterTime > Vehicle_Param_Jeep_Lua.f_boost_hold_time)
			{
				BoosterTime = Time.time;
				Animator.SetTrigger("On Booster Hide");
				UseBooster = false;
			}
			if (!UseBooster)
			{
				if (Singleton<RInput>.Instance.P.GetButton("Button A") && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					MotorTorque = MaxMotorTorque;
					GoingReverse = false;
				}
				else if (z < 0f && Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					GoingReverse = true;
					if (z > -15f)
					{
						MotorTorque = 0f - Vehicle_Param_Jeep_Lua.f_MaxBreakingTorque;
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
			CurrentTopSpeed = ((!UseBooster) ? Mathf.MoveTowards(CurrentTopSpeed, Vehicle_Param_Jeep_Lua.f_MaxSpeed, Time.deltaTime * 10f) : 80f);
			if (Singleton<RInput>.Instance.P.GetButton("Button X") && z > 0f && !UseBooster)
			{
				BrakeTorque = Vehicle_Param_Jeep_Lua.f_MaxBreakingTorque;
			}
			else
			{
				BrakeTorque = 0f;
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && !FlipBack && ((!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded) || Vector3.Dot(base.transform.up, Vector3.down) > 0f))
			{
				FlipBack = true;
				_Rigidbody.velocity = Vector3.up * 7.5f;
			}
			if (IsShoot && Time.time - MountedTime > 0.5f && Time.time - ShootTime > 0.25f && Singleton<RInput>.Instance.P.GetButtonDown("Right Trigger") && CurAmmo > 0)
			{
				ShootTime = Time.time;
				if (!PM.shadow.IsChaosBoost)
				{
					WeaponAnimators[MissileIndex].SetTrigger("On Demount");
				}
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
		else
		{
			MotorTorque = 0f;
			BrakeTorque = 500f;
			GoingReverse = false;
			UseBooster = false;
		}
	}

	private void FixedUpdate()
	{
		ApplyPosToVisuals(AxleInfos[0].LeftWheel, FrontGuards[0], RotateOnY: true);
		ApplyPosToVisuals(AxleInfos[0].RightWheel, FrontGuards[1], RotateOnY: true);
		ApplyPosToVisuals(AxleInfos[0].LeftWheel, LeftSuspensions[0]);
		ApplyPosToVisuals(AxleInfos[0].RightWheel, RightSuspensions[0]);
		ApplyPosToVisuals(AxleInfos[1].LeftWheel, LeftSuspensions[1]);
		ApplyPosToVisuals(AxleInfos[1].RightWheel, RightSuspensions[1]);
		float num = Mathf.Lerp(1f, 0.75f, SqrMag() / Vehicle_Param_Jeep_Lua.f_MaxSpeed);
		foreach (AxleInfo axleInfo in AxleInfos)
		{
			if (axleInfo.Steer)
			{
				axleInfo.LeftWheel.steerAngle = SteeringAngle * SteerXAxis * MoveAxes.x * num;
				axleInfo.RightWheel.steerAngle = SteeringAngle * SteerXAxis * MoveAxes.x * num;
			}
			WheelHit hit;
			if (axleInfo.Motor)
			{
				axleInfo.LeftWheel.GetGroundHit(out hit);
				axleInfo.RightWheel.GetGroundHit(out hit);
				axleInfo.LeftWheel.motorTorque = (axleInfo.LeftWheel.GetGroundHit(out hit) ? MotorTorque : 0f);
				axleInfo.RightWheel.motorTorque = (axleInfo.RightWheel.GetGroundHit(out hit) ? MotorTorque : 0f);
				AdjustTorque(hit.forwardSlip);
			}
			if (axleInfo.Brake)
			{
				axleInfo.LeftWheel.brakeTorque = BrakeTorque;
				axleInfo.RightWheel.brakeTorque = BrakeTorque;
			}
			ApplyPosAndRotToVisuals(axleInfo.LeftWheel);
			ApplyPosAndRotToVisuals(axleInfo.RightWheel);
			float num2 = 1f;
			float num3 = 1f;
			if (axleInfo.LeftWheel.GetGroundHit(out hit))
			{
				num2 = (0f - axleInfo.LeftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.LeftWheel.radius) / axleInfo.LeftWheel.suspensionDistance;
			}
			if (axleInfo.RightWheel.GetGroundHit(out hit))
			{
				num3 = (0f - axleInfo.RightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.RightWheel.radius) / axleInfo.RightWheel.suspensionDistance;
			}
			float num4 = (num2 - num3) * axleInfo.AntiRoll;
			if (axleInfo.LeftWheel.GetGroundHit(out hit))
			{
				_Rigidbody.AddForceAtPosition(axleInfo.LeftWheel.transform.up * (0f - num4), axleInfo.LeftWheel.transform.position);
			}
			if (axleInfo.RightWheel.GetGroundHit(out hit))
			{
				_Rigidbody.AddForceAtPosition(axleInfo.RightWheel.transform.up * num4, axleInfo.RightWheel.transform.position);
			}
		}
		if (AirLaunch)
		{
			base.transform.forward = _Rigidbody.velocity.normalized;
			if (Time.time - LaunchTime > 1f && (AxleInfos[0].LeftWheel.isGrounded || AxleInfos[0].RightWheel.isGrounded || AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded))
			{
				AirLaunch = false;
			}
		}
		if (FlipBack)
		{
			Vector3 vector = Vector3.Cross(base.transform.up, Vector3.up);
			Vector3 vector2 = vector.normalized * vector.magnitude * 25f;
			vector2.x *= 0.25f;
			vector2.y = 0f;
			vector2.z *= 0.25f;
			vector2 -= _Rigidbody.angularVelocity;
			_Rigidbody.AddTorque(vector2 * _Rigidbody.mass * 0.02f, ForceMode.Impulse);
			if (AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded)
			{
				FlipBack = false;
			}
		}
		if (!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded && !FlipBack)
		{
			base.transform.Rotate(0f, MoveAxes.x * SteerXAxis, 0f);
			Vector3 vector3 = Vector3.Cross(base.transform.up, Vector3.up);
			Vector3 vector4 = vector3.normalized * vector3.magnitude * 30f;
			vector4.x *= 0.25f;
			vector4.y = 0f;
			vector4.z *= 0.25f;
			vector4 -= _Rigidbody.angularVelocity;
			_Rigidbody.AddTorque(vector4 * _Rigidbody.mass * 0.02f, ForceMode.Impulse);
		}
		if (ScaledVelSpd() > CurrentTopSpeed && !AirLaunch)
		{
			_Rigidbody.velocity = CurrentTopSpeed / 2.2369363f * _Rigidbody.velocity.normalized;
		}
		if (!AxleInfos[0].LeftWheel.isGrounded || !AxleInfos[0].RightWheel.isGrounded || !AxleInfos[1].LeftWheel.isGrounded || !AxleInfos[1].RightWheel.isGrounded)
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

	public override void OnJumpPanel(Vector3 Velocity)
	{
		base.OnJumpPanel(Velocity);
		AirLaunch = true;
		FlipBack = false;
	}

	private IEnumerator Reload()
	{
		float StartTime = Time.time;
		float Timer = 0f;
		while (Timer <= Vehicle_Param_Jeep_Lua.f_missile_recharge_time)
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

	private void OnCollisionStay(Collision collision)
	{
		if (AirLaunch && !(collision.transform.parent == base.transform) && collision.gameObject.layer != LayerMask.NameToLayer("PlayerCollision") && Time.time - LaunchTime > 1f)
		{
			AirLaunch = false;
		}
	}

	public override int Hit()
	{
		if (ScaledVelSpd() < Vehicle_Param_Jeep_Lua.f_speed_low)
		{
			return Vehicle_Param_Jeep_Lua.i_damage_low;
		}
		if (ScaledVelSpd() < Vehicle_Param_Jeep_Lua.f_speed_mid)
		{
			return Vehicle_Param_Jeep_Lua.i_damage_mid;
		}
		return Vehicle_Param_Jeep_Lua.i_damage_high;
	}

	public override float Damage()
	{
		return Vehicle_Param_Jeep_Lua.i_damage_high;
	}

	public override void OnPlayerState()
	{
		if (((!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded) || Vector3.Dot(base.transform.up, Vector3.down) > 0f) && !FlipBack)
		{
			FlipBack = true;
			_Rigidbody.velocity = Vector3.up * 7.5f;
		}
		AmigoAI amigoAI = Object.FindObjectOfType<AmigoAI>();
		if ((bool)amigoAI && amigoAI.enabled && amigoAI.PlayerAmigoIndex < 2)
		{
			amigoAI.OnVehicleSub(PlayerSubPoint, VehicleCols, GetComponent<VehicleBase>());
		}
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
