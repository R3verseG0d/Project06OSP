using System.Collections.Generic;
using STHLua;
using UnityEngine;

public class Hover : VehicleBase
{
	[Header("Prefab")]
	public List<AxleInfo> AxleInfos;

	public float MaxMotorTorque;

	[Range(0f, 1f)]
	public float TractionControl;

	public float SlipLimit;

	[Header("Parts")]
	public SkinnedMeshRenderer Renderer;

	public SkinnedMeshRenderer[] PropellerRenderers;

	public Transform[] Propellers;

	public ParticleSystem[] HoverFX;

	public ParticleSystem[] PropellerFireFX;

	public ParticleSystem[] GunSmokeFX;

	[Header("Terrain")]
	public ParticleSystem LandConcreteFX;

	public ParticleSystem LandSandFX;

	public ParticleSystem LandDirtFX;

	public ParticleSystem LandGrassFX;

	[Header("Sounds")]
	public AudioSource AccelPropeller;

	public AudioSource Brake;

	public AudioSource ReverseEngine;

	public AudioClip JumpSound;

	private bool ReleasedKey;

	private bool Hovering;

	private bool IsHover;

	private bool Grounded;

	private bool FlipBack;

	private bool Braking;

	private bool Recharged;

	private bool Jumped;

	private bool HasJumped;

	private float HoverAccel;

	private float CurrentTorque;

	private float MotorTorque;

	private float BrakeTorque;

	private float SteerXAxis;

	private float PropellerX;

	private float ButtonCooler = 0.25f;

	private float CurrentTopSpeed;

	private float JumpTime;

	private float GunTimeStamp;

	private float RechargeTime;

	private int ButtonCount;

	private int GunIndex;

	private LayerMask ShieldWater_Mask => LayerMask.GetMask("Water", "TriggerCollider");

	public override void Awake()
	{
		base.Awake();
		MaxHP = Vehicle_Param_Hover_Lua.f_max_hp;
		CurHP = Vehicle_Param_Hover_Lua.f_start_hp;
		MaxAmmo = Vehicle_Param_Hover_Lua.i_Missile_Bullet;
		CurAmmo = MaxAmmo;
		ShotName = Vehicle_Param_Hover_Lua.s_missile_shotname;
	}

	public override void Start()
	{
		base.Start();
		AttackBox.center = new Vector3(0f, Vehicle_Param_Hover_Lua.f_attack_box_y * 0.5f, 0f);
		AttackBox.size = new Vector3(Vehicle_Param_Hover_Lua.f_attack_box_x, Vehicle_Param_Hover_Lua.f_attack_box_y, Vehicle_Param_Hover_Lua.f_attack_box_z);
		CurrentTopSpeed = Vehicle_Param_Hover_Lua.f_MaxSpeed;
		for (int i = 0; i < PropellerRenderers.Length; i++)
		{
			PropellerRenderers[i].GetPropertyBlock(PropBlock, 1);
			PropBlock.SetFloat("_Intensity", 0f);
			PropellerRenderers[i].SetPropertyBlock(PropBlock, 1);
		}
		Renderer.GetPropertyBlock(PropBlock, 3);
		PropBlock.SetFloat("_Intensity", 0f);
		Renderer.SetPropertyBlock(PropBlock, 3);
	}

	public void ApplyPosToVisuals(Transform Part, bool GrabChildren = false)
	{
		if (!GrabChildren)
		{
			Part.localEulerAngles = new Vector3(Part.localEulerAngles.x, (0f - MoveAxes.x) * Vehicle_Param_Hover_Lua.f_SteerAngle, Part.localEulerAngles.z);
			return;
		}
		PropellerX = Mathf.Lerp(PropellerX, Jumped ? ((0f - Vehicle_Param_Hover_Lua.f_SteerAngle) * 2f) : 0f, Time.fixedDeltaTime * 10f);
		Part.GetChild(0).localEulerAngles = new Vector3(PropellerX, Part.GetChild(0).localEulerAngles.y, Part.GetChild(0).localEulerAngles.z);
	}

	public override void Update()
	{
		base.Update();
		float z = base.transform.InverseTransformDirection(_Rigidbody.velocity).z;
		MoveAxes.x = Mathf.Lerp(MoveAxes.x, Singleton<RInput>.Instance.P.GetAxis("Left Stick X") * (IsMounted ? 1f : 0f), Time.deltaTime * 3f);
		for (int i = 0; i < PropellerRenderers.Length; i++)
		{
			PropellerRenderers[i].GetPropertyBlock(PropBlock, 1);
			PropBlock.SetFloat("_Intensity", Mathf.Lerp(PropBlock.GetFloat("_Intensity"), (IsMounted && !Hovering && Singleton<RInput>.Instance.P.GetButton("Button A")) ? 1f : 0f, Time.deltaTime * 25f));
			PropellerRenderers[i].SetPropertyBlock(PropBlock, 1);
		}
		for (int j = 0; j < PropellerFireFX.Length; j++)
		{
			ParticleSystem.EmissionModule emission = PropellerFireFX[j].emission;
			emission.enabled = IsMounted && !Hovering && Singleton<RInput>.Instance.P.GetButton("Button A");
		}
		for (int k = 0; k < HoverFX.Length; k++)
		{
			ParticleSystem.EmissionModule emission2 = HoverFX[k].emission;
			emission2.enabled = IsMounted && ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && _Rigidbody.velocity.y < 0f;
			IsHover = emission2.enabled;
		}
		Renderer.GetPropertyBlock(PropBlock, 3);
		PropBlock.SetFloat("_Intensity", Mathf.Lerp(PropBlock.GetFloat("_Intensity"), (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X")) ? 1f : 0f, Time.deltaTime * 25f));
		Renderer.SetPropertyBlock(PropBlock, 3);
		IdleEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Hover_Lua.f_MaxSpeed) + 0.6f;
		IdleEngine.pitch = Mathf.Clamp(IdleEngine.pitch, IdleEngine.pitch, 1f);
		IdleEngine.volume = Mathf.Lerp(IdleEngine.volume, (IsMounted && !GoingReverse) ? 0.8f : 0f, Time.deltaTime * 10f);
		AccelPropeller.pitch = Mathf.Lerp(AccelPropeller.pitch, (IsMounted && ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && _Rigidbody.velocity.y < 0f) ? (Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Hover_Lua.f_MaxSpeed) + 0.5f) : 0.25f, Time.deltaTime * 2f);
		AccelPropeller.volume = Mathf.Lerp(AccelPropeller.volume, (IsMounted && ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && _Rigidbody.velocity.y < 0f) ? 0.25f : 0f, Time.deltaTime * 2f);
		ReverseEngine.pitch = Mathf.Abs(base.transform.InverseTransformDirection(_Rigidbody.velocity).z / Vehicle_Param_Hover_Lua.f_MaxSpeed) + 0.4f;
		ReverseEngine.volume = Mathf.Lerp(ReverseEngine.volume, (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X") && GoingReverse) ? 0.8f : 0f, Time.deltaTime * 5f);
		Brake.volume = Mathf.Lerp(Brake.volume, (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X") && !GoingReverse) ? 0.5f : 0f, Time.deltaTime * 5f);
		if (AxleInfos[0].LeftWheel.isGrounded || AxleInfos[0].RightWheel.isGrounded || AxleInfos[1].LeftWheel.isGrounded || AxleInfos[1].RightWheel.isGrounded)
		{
			if (!Grounded)
			{
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
					case "Grass":
						LandGrassFX.transform.position = hitInfo.point;
						LandGrassFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
						LandGrassFX.Play();
						break;
					default:
						if (hitInfo.transform.tag != "Water")
						{
							LandConcreteFX.transform.position = hitInfo.point;
							LandConcreteFX.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
							LandConcreteFX.Play();
						}
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
		SteerXAxis = Mathf.Lerp(SteerXAxis, (!IsMounted || (IsMounted && Singleton<RInput>.Instance.P.GetButton("Button X") && z < 0f)) ? 1f : Mathf.Lerp(1f, 0.75f, SqrMag() / Vehicle_Param_Hover_Lua.f_MaxSpeed), Time.deltaTime * 2f);
		HoverAccel = Mathf.Lerp(HoverAccel, IsMounted ? Singleton<RInput>.Instance.P.GetAxis("Left Stick Y") : 0f, Time.deltaTime * 0.5f);
		if (IsMounted)
		{
			if ((bool)PM)
			{
				float num = Time.deltaTime * 2f;
				num = num * num * (3f - 2f * num);
				float b = Mathf.Lerp(0f, 1f, ScaledVelSpd() / Vehicle_Param_Hover_Lua.f_MaxSpeed);
				PM.Base.CamOffset.x = Mathf.Lerp(PM.Base.CamOffset.x, (VehicleState == 1) ? Singleton<RInput>.Instance.P.GetAxis("Left Stick X") : 0f, num);
				PM.Base.CamOffset.y = Mathf.Lerp(PM.Base.CamOffset.y, Common_Lua.c_camera.y + ((AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded) ? 0.1f : (-0.25f)), num);
				PM.Base.CamOffset.z = Mathf.Lerp(PM.Base.CamOffset.z, b, num);
			}
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button A") && AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded)
			{
				if (ButtonCooler > 0f && ButtonCount == 1)
				{
					if (!Jumped)
					{
						Audio.PlayOneShot(JumpSound, Audio.volume);
						JumpTime = Time.time;
						if (!HasJumped)
						{
							PM.Base.PlayAnimation(GetVehicleName() + " Jump", "On Vehicle Launch");
							HasJumped = true;
						}
						Jumped = true;
					}
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
					MotorTorque = 0f - Vehicle_Param_Hover_Lua.f_MaxBreakingTorque;
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
			CurrentTopSpeed = Mathf.MoveTowards(CurrentTopSpeed, Vehicle_Param_Hover_Lua.f_MaxSpeed, Time.deltaTime * 10f);
			if (Singleton<RInput>.Instance.P.GetButton("Button X") && z > 0f)
			{
				BrakeTorque = Vehicle_Param_Hover_Lua.f_MaxBreakingTorque;
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
			if (Singleton<RInput>.Instance.P.GetButtonDown("Button B") && !FlipBack && ((!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded) || Vector3.Dot(base.transform.up, Vector3.down) > 0f))
			{
				FlipBack = true;
				_Rigidbody.velocity = Vector3.up * 7.5f;
			}
			if (IsShoot && Time.time - MountedTime > 0.5f)
			{
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
						GunTimeStamp = Time.time + Vehicle_Param_Hover_Lua.f_Missile_Interval;
						GadgetBullet componentInChildren = Object.Instantiate(Missile, WeaponPoints[GunIndex].position, Quaternion.LookRotation(WeaponPoints[GunIndex].forward)).GetComponentInChildren<GadgetBullet>();
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
				}
				else
				{
					if (IsShooting)
					{
						RechargeTime = Time.time;
						PlayWeaponAnimation((CurAmmo > 0) ? "On Shoot Stop" : "On Demount");
						if (CurAmmo == 0)
						{
							for (int l = 0; l < GunSmokeFX.Length; l++)
							{
								GunSmokeFX[l].Play();
							}
						}
						IsShooting = false;
					}
					if (Time.time - RechargeTime > Vehicle_Param_Hover_Lua.f_Missile_RecoveryTime && !Recharged && CurAmmo != MaxAmmo)
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
			}
			if (HasJumped && Time.time - JumpTime > 0.25f && AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded)
			{
				PM.Base.PlayAnimation(GetVehicleName(), "On Vehicle Air End");
				HasJumped = false;
			}
		}
		else
		{
			MotorTorque = 0f;
			BrakeTorque = 500f;
			GoingReverse = false;
			IsShooting = false;
		}
	}

	private void FixedUpdate()
	{
		ApplyPosToVisuals(Propellers[0]);
		ApplyPosToVisuals(Propellers[1]);
		ApplyPosToVisuals(Propellers[0], GrabChildren: true);
		ApplyPosToVisuals(Propellers[1], GrabChildren: true);
		foreach (AxleInfo axleInfo in AxleInfos)
		{
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
			float num = 1f;
			float num2 = 1f;
			if (axleInfo.LeftWheel.GetGroundHit(out hit))
			{
				num = (0f - axleInfo.LeftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.LeftWheel.radius) / axleInfo.LeftWheel.suspensionDistance;
			}
			if (axleInfo.RightWheel.GetGroundHit(out hit))
			{
				num2 = (0f - axleInfo.RightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.RightWheel.radius) / axleInfo.RightWheel.suspensionDistance;
			}
			float num3 = (num - num2) * axleInfo.AntiRoll;
			if (axleInfo.LeftWheel.GetGroundHit(out hit))
			{
				_Rigidbody.AddForceAtPosition(axleInfo.LeftWheel.transform.up * (0f - num3), axleInfo.LeftWheel.transform.position);
			}
			if (axleInfo.RightWheel.GetGroundHit(out hit))
			{
				_Rigidbody.AddForceAtPosition(axleInfo.RightWheel.transform.up * num3, axleInfo.RightWheel.transform.position);
			}
		}
		if (IsMounted)
		{
			if (!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded && !FlipBack)
			{
				if (!Singleton<RInput>.Instance.P.GetButton("Button A"))
				{
					ReleasedKey = true;
					Hovering = true;
				}
				if (ReleasedKey && Singleton<RInput>.Instance.P.GetButton("Button A") && _Rigidbody.velocity.y < 0f)
				{
					Vector3 velocity = _Rigidbody.velocity;
					Vector3 vector = new Vector3(velocity.x, 0f, velocity.z);
					if (_Rigidbody.velocity.magnitude != 0f)
					{
						vector += base.transform.forward * HoverAccel;
						velocity = new Vector3(vector.x, velocity.y, vector.z);
					}
					velocity.y = Mathf.Lerp(velocity.y, Vehicle_Param_Hover_Lua.f_parts_jet_down_range, Time.fixedDeltaTime * 20f);
					velocity.x = Mathf.Lerp(velocity.x, 0f, Time.fixedDeltaTime);
					velocity.z = Mathf.Lerp(velocity.z, 0f, Time.fixedDeltaTime);
					_Rigidbody.velocity = velocity;
					Quaternion rotation = base.transform.rotation;
					rotation.x = Mathf.MoveTowards(rotation.x, 0f, Time.fixedDeltaTime);
					rotation.z = Mathf.MoveTowards(rotation.z, 0f, Time.fixedDeltaTime);
					base.transform.rotation = rotation;
					if (HasJumped)
					{
						PM.Base.PlayAnimation(GetVehicleName(), "On Vehicle Air End");
						HasJumped = false;
					}
				}
				else
				{
					Vector3 vector2 = Vector3.Cross(base.transform.up, Vector3.up);
					Vector3 vector3 = vector2.normalized * vector2.magnitude * 30f;
					vector3.x *= 0.25f;
					vector3.y = 0f;
					vector3.z *= 0.25f;
					vector3 -= _Rigidbody.angularVelocity;
					_Rigidbody.AddTorque(vector3 * _Rigidbody.mass * 0.02f, ForceMode.Impulse);
				}
			}
			else
			{
				ReleasedKey = false;
				Hovering = false;
			}
		}
		if (FlipBack)
		{
			Vector3 vector4 = Vector3.Cross(base.transform.up, Vector3.up);
			Vector3 vector5 = vector4.normalized * vector4.magnitude * 25f;
			vector5.x *= 0.25f;
			vector5.y = 0f;
			vector5.z *= 0.25f;
			vector5 -= _Rigidbody.angularVelocity;
			_Rigidbody.AddTorque(vector5 * _Rigidbody.mass * 0.02f, ForceMode.Impulse);
			if (AxleInfos[0].LeftWheel.isGrounded && AxleInfos[0].RightWheel.isGrounded && AxleInfos[1].LeftWheel.isGrounded && AxleInfos[1].RightWheel.isGrounded)
			{
				FlipBack = false;
			}
		}
		base.transform.Rotate(0f, MoveAxes.x * 2f * SteerXAxis, 0f);
		if (Jumped)
		{
			if (Time.time - JumpTime < Vehicle_Param_Hover_Lua.f_boost_effect_time)
			{
				Vector3 velocity2 = _Rigidbody.velocity;
				velocity2.y = 0f;
				velocity2.y += 10f;
				_Rigidbody.velocity = velocity2;
			}
			else
			{
				Jumped = false;
			}
		}
		if (ScaledVelSpd() > CurrentTopSpeed)
		{
			_Rigidbody.velocity = CurrentTopSpeed / 2.2369363f * _Rigidbody.velocity.normalized;
		}
		if ((!AxleInfos[0].LeftWheel.isGrounded || !AxleInfos[0].RightWheel.isGrounded || !AxleInfos[1].LeftWheel.isGrounded || !AxleInfos[1].RightWheel.isGrounded) && !IsHover)
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
		_Rigidbody.velocity = Velocity;
		base.transform.forward = _Rigidbody.velocity.normalized;
		FlipBack = false;
	}

	public override int Hit()
	{
		if (ScaledVelSpd() < Vehicle_Param_Hover_Lua.f_speed_low)
		{
			return Vehicle_Param_Hover_Lua.i_damage_low;
		}
		if (ScaledVelSpd() < Vehicle_Param_Hover_Lua.f_speed_mid)
		{
			return Vehicle_Param_Hover_Lua.i_damage_mid;
		}
		return Vehicle_Param_Hover_Lua.i_damage_high;
	}

	public override float Damage()
	{
		return Vehicle_Param_Hover_Lua.i_damage_high;
	}

	public override void OnPlayerState()
	{
		if (((!AxleInfos[0].LeftWheel.isGrounded && !AxleInfos[0].RightWheel.isGrounded && !AxleInfos[1].LeftWheel.isGrounded && !AxleInfos[1].RightWheel.isGrounded) || Vector3.Dot(base.transform.up, Vector3.down) > 0f) && !FlipBack)
		{
			FlipBack = true;
			_Rigidbody.velocity = Vector3.up * 7.5f;
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
