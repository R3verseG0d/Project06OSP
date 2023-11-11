using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Car : VehicleBase
{
	[Header("Prefab")]
	public List<AxleInfo> AxleInfos;

	public float MaxMotorTorque;

	public float BoosterTorque;

	[Range(0f, 1f)]
	public float TractionControl;

	public float SlipLimit;

	public bool _IsGetOut;

	[Header("Parts")]
	public Transform Handle;

	public TrailRenderer[] BoosterTrails;

	public Light[] FrontLights;

	public Light[] RearLights;

	public Light[] BoosterLights;

	public ParticleSystem[] BoosterFX;

	public ParticleSystem[] FrontWheelSmokeFX;

	public ParticleSystem[] RearWheelSmokeFX;

	public ParticleSystem LandFX;

	[Header("Sounds")]
	public AudioSource TopSpeedEngine;

	public AudioSource Brake;

	public AudioSource ReverseEngine;

	public AudioClip BoosterSound;

	public AudioClip LandSound;

	private bool UseFrontLights;

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

	private float CurrentTopSpeed;

	private float SteeringAngle;

	private float TrailAlpha;

	private int ButtonCount;

	public override void Awake()
	{
		base.Awake();
		MaxHP = Vehicle_Param_Car_Lua.f_max_hp;
		CurHP = Vehicle_Param_Car_Lua.f_start_hp;
		MaxAmmo = 2;
		CurAmmo = MaxAmmo;
		ShotName = Vehicle_Param_Car_Lua.s_missile_shotname;
	}

	public override void Start()
	{
		base.Start();
		AttackBox.center = new Vector3(0f, 0.1f, 0f);
		AttackBox.size = new Vector3(Vehicle_Param_Car_Lua.f_attack_box_x, Vehicle_Param_Car_Lua.f_attack_box_y, Vehicle_Param_Car_Lua.f_attack_box_z);
		CurrentTorque = MaxMotorTorque - TractionControl * MaxMotorTorque;
		CurrentTopSpeed = Vehicle_Param_Car_Lua.f_MaxSpeed;
		BoosterTime = Time.time;
		IsGetOut = _IsGetOut;
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
				SteeringAngle = Vehicle_Param_Car_Lua.f_SteerAngle;
			}
		}
		Handle.localEulerAngles = new Vector3(Handle.localEulerAngles.x, Handle.localEulerAngles.y, (0f - MoveAxes.x) * (SteeringAngle * 1.25f));
		for (int i = 0; i < FrontLights.Length; i++)
		{
			FrontLights[i].intensity = Mathf.Lerp(FrontLights[i].intensity, UseFrontLights ? 2f : 0f, Time.deltaTime * 20f);
		}
		for (int j = 0; j < RearLights.Length; j++)
		{
			RearLights[j].intensity = Mathf.Lerp(RearLights[j].intensity, (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X")) ? 2f : 0f, Time.deltaTime * 25f);
		}
		for (int k = 0; k < BoosterLights.Length; k++)
		{
			BoosterLights[k].intensity = Mathf.Lerp(BoosterLights[k].intensity, UseBooster ? 2f : 0f, Time.deltaTime * 10f);
		}
		for (int l = 0; l < BoosterFX.Length; l++)
		{
			ParticleSystem.EmissionModule emission = BoosterFX[l].emission;
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
		for (int m = 0; m < BoosterTrails.Length; m++)
		{
			BoosterTrails[m].colorGradient = gradient;
		}
		ParticleSystem.EmissionModule emission2 = FrontWheelSmokeFX[0].emission;
		emission2.enabled = AxleInfos[0].LeftWheel.isGrounded;
		emission2.rateOverTime = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed);
		ParticleSystem.EmissionModule emission3 = FrontWheelSmokeFX[1].emission;
		emission3.enabled = AxleInfos[0].RightWheel.isGrounded;
		emission3.rateOverTime = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed);
		ParticleSystem.EmissionModule emission4 = RearWheelSmokeFX[0].emission;
		emission4.enabled = AxleInfos[1].LeftWheel.isGrounded;
		emission4.rateOverTime = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed);
		ParticleSystem.EmissionModule emission5 = RearWheelSmokeFX[1].emission;
		emission5.enabled = AxleInfos[1].RightWheel.isGrounded;
		emission5.rateOverTime = Mathf.Lerp(0f, 50f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed);
		IdleEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Car_Lua.f_MaxSpeed * 2f) + 0.8f;
		IdleEngine.pitch = Mathf.Clamp(IdleEngine.pitch, IdleEngine.pitch, 1.4f);
		IdleEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(1f, 0f, Mathf.Clamp01(z * 0.1f)) : Mathf.Lerp(IdleEngine.volume, 0f, Time.deltaTime * 10f));
		TopSpeedEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Car_Lua.f_MaxSpeed * 2f) + 0.5f;
		if (TopSpeedEngine.pitch > 1.4f)
		{
			TopSpeedEngine.pitch = 1.4f;
		}
		TopSpeedEngine.volume = ((IsMounted && !GoingReverse) ? Mathf.Lerp(0f, 1f, Mathf.Clamp01(z * 0.1f)) : Mathf.Lerp(TopSpeedEngine.volume, 0f, Time.deltaTime * 10f));
		ReverseEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Car_Lua.f_MaxSpeed * 2f) + 0.4f;
		ReverseEngine.volume = Mathf.Lerp(ReverseEngine.volume, (IsMounted && GoingReverse) ? 1f : 0f, Time.deltaTime * 10f);
		WheelHit[] array = new WheelHit[2];
		AxleInfos[1].LeftWheel.GetGroundHit(out array[0]);
		AxleInfos[1].RightWheel.GetGroundHit(out array[1]);
		Brake.volume = Mathf.Lerp(Brake.volume, (IsMounted && (AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded) && ((Singleton<RInput>.Instance.P.GetButton("Button X") && (AxleInfos[1].LeftWheel.rpm == 0f || AxleInfos[1].RightWheel.rpm == 0f)) || ((array[0].forwardSlip >= SlipLimit || array[1].forwardSlip >= SlipLimit) && CurrentTorque >= 0f))) ? 1f : 0f, Time.deltaTime * 10f);
		if (AxleInfos[0].LeftWheel.isGrounded || AxleInfos[0].RightWheel.isGrounded || AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded)
		{
			if (!Grounded)
			{
				Audio.PlayOneShot(LandSound, Audio.volume);
				if (Physics.Raycast(base.transform.position + base.transform.up * 0.5f, -Vector3.up, out var hitInfo, 5f, base.Collision_Mask))
				{
					LandFX.transform.position = hitInfo.point;
					LandFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
				}
				LandFX.Play();
				Grounded = true;
			}
		}
		else
		{
			Grounded = false;
		}
		SteerXAxis = Mathf.Lerp(SteerXAxis, (!IsMounted || (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X") && z < 0f)) ? 1f : Mathf.Lerp(1f, 0.5f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed), Time.deltaTime * 2f);
		if (IsMounted)
		{
			if ((bool)PM)
			{
				float num = Time.deltaTime * 2f;
				num = num * num * (3f - 2f * num);
				float num2 = Mathf.Lerp(1f, 2f, ScaledVelSpd() / Vehicle_Param_Car_Lua.f_MaxSpeed);
				PM.Base.CamOffset.x = Mathf.Lerp(PM.Base.CamOffset.x, (VehicleState == 1) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, num);
				PM.Base.CamOffset.y = Mathf.Lerp(PM.Base.CamOffset.y, Common_Lua.c_camera.y + ((AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded) ? 0.2f : (-0.15f)), num);
				PM.Base.CamOffset.z = Mathf.Lerp(PM.Base.CamOffset.z, UseBooster ? 2.5f : num2, UseBooster ? (Time.deltaTime * 2f) : num);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && !UseBooster && Time.time - BoosterTime > Vehicle_Param_Car_Lua.f_boost_effect_time)
			{
				if (ButtonCooler > 0f && ButtonCount == 1 && !Singleton<RInput>.Instance.P.GetButton("Button X"))
				{
					BoosterTime = Time.time;
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
			if (UseBooster && Time.time - BoosterTime > Vehicle_Param_Car_Lua.f_boost_hold_time)
			{
				BoosterTime = Time.time;
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
						MotorTorque = 0f - Vehicle_Param_Car_Lua.f_MaxBreakingTorque;
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
			CurrentTopSpeed = ((!UseBooster) ? Mathf.MoveTowards(CurrentTopSpeed, Vehicle_Param_Car_Lua.f_MaxSpeed, Time.deltaTime * 15f) : 80f);
			if (Singleton<RInput>.Instance.P.GetButton("Button X") && z > 0f && !UseBooster)
			{
				BrakeTorque = Vehicle_Param_Car_Lua.f_MaxBreakingTorque;
			}
			else
			{
				BrakeTorque = 0f;
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B"))
			{
				UseFrontLights = !UseFrontLights;
			}
		}
		else
		{
			MotorTorque = 0f;
			BrakeTorque = 500f;
			UseFrontLights = false;
			GoingReverse = false;
			UseBooster = false;
		}
	}

	private void FixedUpdate()
	{
		float num = Mathf.Lerp(1f, 0.75f, SqrMag() / Vehicle_Param_Car_Lua.f_MaxSpeed);
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
		if (!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded)
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
		if (ScaledVelSpd() < Vehicle_Param_Car_Lua.f_speed_low)
		{
			return Vehicle_Param_Car_Lua.i_damage_low;
		}
		if (ScaledVelSpd() < Vehicle_Param_Car_Lua.f_speed_mid)
		{
			return Vehicle_Param_Car_Lua.i_damage_mid;
		}
		return Vehicle_Param_Car_Lua.i_damage_high;
	}

	public override float Damage()
	{
		return Vehicle_Param_Car_Lua.i_damage_high;
	}
}
