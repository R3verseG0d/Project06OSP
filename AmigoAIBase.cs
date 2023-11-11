using STHEngine;
using STHLua;
using UnityEngine;

public class AmigoAIBase : MonoBehaviour
{
	[Header("Amigo AI Base")]
	public Rigidbody _Rigidbody;

	public StateMachine StateMachine;

	public CapsuleCollider CapsuleCollider;

	public GameObject PushCollider;

	public Animator ReappearAnimator;

	[Header("Particles")]
	public GameObject TerrainPrefab;

	[Header("Sinking - Base")]
	public GameObject JumpSandParticle;

	public GameObject SinkSandParticle;

	public GameObject SinkLavaParticle;

	internal GameObject ActiveSinkSandParticle;

	internal GameObject ActiveSinkLavaParticle;

	[Header("Audio")]
	public AudioSource Audio;

	public AudioSource VoicesAudio;

	[Header("Princess")]
	public Animator EliseAnimator;

	[Header("Shadow")]
	public AudioSource Skate1;

	public AudioSource Skate2;

	public Color JetTrailColor;

	public TrailRenderer[] JetTrails;

	public ParticleSystem[] JetFireFX;

	[Header("Silver")]
	public SkinnedMeshRenderer[] SilverRenderers;

	public ParticleSystem[] PsiAuraFX;

	public Color[] PsiGlows;

	public Transform[] PsiPoints;

	public GameObject PsiTrailFX;

	public AudioSource PsiLoopSource;

	public AudioClip[] PsychoSounds;

	internal bool SilverHasLotus;

	[Header("Tails")]
	public AudioSource FlySource;

	[Header("Omega")]
	public Transform[] OmegaJetBones;

	public GameObject OmegaJetFX;

	public ParticleSystem[] OmegaJetFireFX;

	private GameObject NearbyCollectible;

	private bool UsePsiElements;

	private bool PlayedFlySource;

	private bool OmegaStartJetFire;

	private float JetTrailAlpha;

	internal PlayerBase FollowTarget;

	internal AmigoParams Params;

	internal Vector3 TargetDirection;

	internal Vector3 UpMeshRotation;

	internal Vector3 ForwardMeshRotation;

	internal Quaternion GeneralMeshRotation;

	internal Transform SwitchTransform;

	internal RaycastHit RaycastHit;

	internal RaycastHit FrontalHit;

	internal float MaxRayLenght;

	internal float CurSpeed;

	internal float TopSpeed;

	internal float ResultStartTime;

	internal int JumpAnimation;

	internal bool LockControls;

	internal bool TotalControlLock;

	internal bool FrontalCollision;

	internal bool IsDead;

	internal bool WalkSwitch;

	private float RunWalkAnimation;

	internal int PlayerAmigoIndex;

	private MaterialPropertyBlock PropBlock;

	internal bool IsSinking;

	internal float SinkPosition;

	internal float SinkStartTime;

	internal string ColName;

	internal LayerMask Collision_Mask => LayerMask.GetMask("Ignore Raycast", "Water", "UI", "PlayerCollision", "BreakableObj", "Object/PlayerOnlyCol", "Vehicle");

	internal LayerMask FrontalCol_Mask => LayerMask.GetMask("Default", "PlayerCollision", "BreakableObj", "Object/PlayerOnlyCol", "Vehicle");

	public virtual void Awake()
	{
		_Rigidbody.sleepThreshold = 0f;
		_Rigidbody.solverIterations = 20;
		_Rigidbody.maxAngularVelocity = float.PositiveInfinity;
		PropBlock = new MaterialPropertyBlock();
	}

	public void PlayAnimation(string AnimState, string TriggerName)
	{
		if (!Params.Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimState))
		{
			Params.Animator.SetTrigger(TriggerName);
		}
	}

	public Vector3 AmigoPoint()
	{
		Vector3 vector = ((PlayerAmigoIndex < 2) ? Common_Lua.c_amigo_point_1 : Common_Lua.c_amigo_point_0);
		Vector3 vector2 = FollowTarget.transform.right * vector.x + FollowTarget.transform.up * vector.y + FollowTarget.transform.forward * vector.z;
		if (GetAmigoState() == "Result" && FollowTarget.GetPrefab("silver"))
		{
			vector2 = FollowTarget.transform.forward * vector.x * 1.25f + FollowTarget.transform.up * vector.y + -FollowTarget.transform.right * vector.z;
		}
		return FollowTarget.transform.position + vector2;
	}

	public bool OutOfDistance()
	{
		return ExtensionMethods.HorizontalDistance(base.transform.position, AmigoPoint()) > (FollowTarget.WalkSwitch ? Amigo_Lua.c_distance_to_walk : Amigo_Lua.c_distance_to_run);
	}

	internal void AccelerationSystem()
	{
		if ((OutOfDistance() && FollowTarget.IsVisible && !FrontalCollision && !TotalControlLock && !NearbyCollectible) || (bool)NearbyCollectible)
		{
			float num = (WalkSwitch ? Params.WalkSpeed : TopSpeed) * ((ExtensionMethods.HorizontalDistance(base.transform.position, AmigoPoint()) > Amigo_Lua.c_distance_raycast && FollowTarget.CurSpeed > FollowTarget.WalkSpeed) ? 1.5f : 1f);
			if (CurSpeed > num)
			{
				CurSpeed = Mathf.MoveTowards(CurSpeed, num, Time.fixedDeltaTime * (IsGrounded() ? 20f : 35f));
			}
			else
			{
				CurSpeed += (TopSpeed - Params.WalkSpeed) / Params.RunAcc * Time.fixedDeltaTime;
			}
		}
		else if (CurSpeed > 0f)
		{
			CurSpeed -= 40f * Time.fixedDeltaTime * 1.5f;
		}
		if (CurSpeed <= 0f)
		{
			CurSpeed = 0f;
		}
	}

	internal float LimitVel(float Result, float Positive = 0f)
	{
		Result = Mathf.Clamp(Result, 0f - Common_Lua.c_vel_y_max, (Positive == 0f) ? Result : Positive);
		return Result;
	}

	internal void RotateAmigo()
	{
		TargetDirection = ((!NearbyCollectible) ? (AmigoPoint() - base.transform.position).MakePlanar().normalized : (NearbyCollectible.transform.position - base.transform.position).MakePlanar().normalized);
		TargetDirection = Quaternion.FromToRotation(Vector3.up, base.transform.up) * TargetDirection;
		WalkSwitch = (ExtensionMethods.HorizontalDistance(base.transform.position, AmigoPoint()) < Amigo_Lua.c_distance_raycast && FollowTarget.CurSpeed < FollowTarget.WalkSpeed && !NearbyCollectible) || (bool)NearbyCollectible;
		if (TargetDirection != Vector3.zero)
		{
			Quaternion b = Quaternion.LookRotation(TargetDirection, RaycastHit.normal);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 8f * Time.fixedDeltaTime);
		}
	}

	private void UpdateEnvironmentalHazards()
	{
		ColName = (RaycastHit.transform ? RaycastHit.transform.gameObject.name : "");
		if (IsGrounded() && (ColName == "40000009" || ColName == "2820000d"))
		{
			if (!IsSinking)
			{
				CurSpeed *= 0.5f;
				SinkStartTime = Time.time;
				IsSinking = true;
			}
		}
		else
		{
			IsSinking = false;
		}
		IsSinking = IsGrounded() && (ColName == "40000009" || ColName == "2820000d");
		if (IsSinking)
		{
			SinkPosition += Time.fixedDeltaTime / 1.5f;
			if (SinkPosition >= 1f)
			{
				if (ColName == "40000009")
				{
					RemoveSinkParticles("Sand");
				}
				SinkPosition = 1f;
			}
			CurSpeed *= 1f - SinkPosition * 0.1f;
		}
		else
		{
			SinkPosition = 0f;
		}
	}

	public GameObject FindCollectibleNearby(bool Conditions)
	{
		GameObject gameObject = null;
		if (Conditions)
		{
			float num = Amigo_Lua.c_ring_distance;
			GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
			foreach (GameObject gameObject2 in array)
			{
				float num2 = Vector3.Distance(base.transform.position, gameObject2.transform.position);
				if (num2 < num && (bool)gameObject2.GetComponentInParent<ItemBox>() && CanSetTarget(gameObject2))
				{
					gameObject = gameObject2;
					num = num2;
				}
			}
			if (!gameObject)
			{
				array = GameObject.FindGameObjectsWithTag("LightDashable");
				foreach (GameObject gameObject3 in array)
				{
					float num3 = Vector3.Distance(base.transform.position, gameObject3.transform.position);
					if (num3 < num && CanSetTarget(gameObject3))
					{
						gameObject = gameObject3;
						num = num3;
					}
				}
			}
		}
		return gameObject;
	}

	private bool CanSetTarget(GameObject Target)
	{
		return !Physics.Linecast(base.transform.position, Target.transform.position, ExtensionMethods.HomingBlock_Mask);
	}

	public bool IsGrounded()
	{
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.25f, -base.transform.up, out RaycastHit, MaxRayLenght, Collision_Mask))
		{
			Debug.DrawLine(base.transform.position, RaycastHit.point, Color.white);
			if ((bool)Params)
			{
				Params.GroundTag = RaycastHit.transform.tag;
			}
			return true;
		}
		return false;
	}

	public bool IsFrontalBump()
	{
		if (Physics.Raycast(base.transform.position, base.transform.forward, out FrontalHit, 0.325f, FrontalCol_Mask))
		{
			Debug.DrawLine(base.transform.position, FrontalHit.point, Color.red);
			CurSpeed = 0f;
			return true;
		}
		return false;
	}

	public void PositionToPoint()
	{
		if (IsGrounded())
		{
			_Rigidbody.position = RaycastHit.point + base.transform.up * 0.25f;
		}
	}

	public void ParentAmigo(Transform Parent)
	{
		base.transform.SetParent(Parent);
	}

	public void UnparentAmigo(Transform Parent)
	{
		if (base.transform.parent == Parent)
		{
			base.transform.SetParent(null);
		}
	}

	public virtual void FixedUpdate()
	{
		if ((FollowTarget.GetState() != "Vehicle" && !LockControls && !NearbyCollectible) || (bool)NearbyCollectible)
		{
			AccelerationSystem();
		}
		if (((CurSpeed > 0f || (CurSpeed <= 0f && FrontalCollision)) && OutOfDistance() && FollowTarget.GetState() != "VehicleSub" && FollowTarget.GetState() != "Talk" && FollowTarget.GetState() != "Path" && FollowTarget.GetState() != "Result" && FollowTarget.IsVisible && !LockControls && !TotalControlLock && !NearbyCollectible) || (bool)NearbyCollectible)
		{
			RotateAmigo();
		}
		NearbyCollectible = FindCollectibleNearby(FollowTarget.IsVisible && !LockControls && !TotalControlLock && GetAmigoState() == "Ground" && FollowTarget.GetState() == "Ground" && FollowTarget.CurSpeed == 0f && FollowTarget.TargetDirection == Vector3.zero);
		if (Physics.Raycast(base.transform.position + base.transform.up * 0.25f, base.transform.forward, out FrontalHit, 0.5f, FrontalCol_Mask))
		{
			Debug.DrawLine(base.transform.position + base.transform.up * 0.25f, FrontalHit.point, Color.red);
			FrontalCollision = true;
			CurSpeed = 0f;
		}
		else
		{
			FrontalCollision = false;
		}
		if (Params.UseJumpFX && Singleton<Settings>.Instance.settings.SpinEffect == 2)
		{
			Params.JumpSA2Pos[0] = Vector3.Lerp(Params.JumpSA2Pos[0], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 25f);
			Params.JumpSA2Pos[1] = Vector3.Lerp(Params.JumpSA2Pos[1], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 19f);
			Params.JumpSA2Pos[2] = Vector3.Lerp(Params.JumpSA2Pos[2], base.transform.position + base.transform.up * 0.25f, Time.fixedDeltaTime * 15f);
			for (int i = 0; i < Params.JumpSA2FX.Length; i++)
			{
				Params.JumpSA2FX[i].transform.position = Params.JumpSA2Pos[i];
			}
		}
		UpdateEnvironmentalHazards();
	}

	public virtual void Update()
	{
		UpdateAnimations();
		UpdateEffects();
		if (FollowTarget.GetState() != "Grinding" && FollowTarget.GetState() != "Talk" && FollowTarget.GetState() != "Path" && FollowTarget.GetState() != "Result" && FollowTarget.GetState() != "JumpPanel" && FollowTarget.IsGrounded() && GetAmigoState() != "VehicleSub" && GetAmigoState() != "Path" && GetAmigoState() != "DashPanel" && GetAmigoState() != "Spring" && GetAmigoState() != "WideSpring" && GetAmigoState() != "JumpPanel" && Vector3.Distance(base.transform.position, FollowTarget.transform.position) > Amigo_Lua.c_distance_is_far && FollowTarget.IsVisible && !TotalControlLock)
		{
			base.transform.position = AmigoPoint() + Vector3.up * 1.4f;
			CurSpeed = 0f;
			_Rigidbody.velocity = Vector3.zero;
			_Rigidbody.AddForce(Vector3.up * 4f, ForceMode.VelocityChange);
			SetAmigoMachineState("StateReappear");
			ReappearAnimator.SetTrigger("On Reappear");
		}
		ForwardMeshRotation = Vector3.Slerp(ForwardMeshRotation, base.transform.forward, Time.deltaTime * (IsGrounded() ? 50f : 15f));
		UpMeshRotation = Vector3.Slerp(UpMeshRotation, IsGrounded() ? base.transform.up : Vector3.up, Time.deltaTime * 10f);
		Params.Animator.transform.rotation = GeneralMeshRotation;
	}

	private void UpdateAnimations()
	{
		Params.Animator.SetFloat("Speed", CurSpeed);
		Params.Animator.SetFloat("Y Vel", _Rigidbody.velocity.y);
		if (Params.AmigoName != "princess" && (Params.AmigoName != "silver" || (Params.AmigoName == "silver" && SilverHasLotus)) && Params.AmigoName != "omega")
		{
			Params.Animator.SetFloat("Jump Animation", JumpAnimation);
		}
		Params.Animator.SetFloat("Run Walk Animation", RunWalkAnimation);
		RunWalkAnimation = Mathf.MoveTowards(RunWalkAnimation, WalkSwitch ? 1f : 0f, Time.deltaTime * 4f);
		Params.Animator.SetBool("Is Sinking", IsSinking && IsGrounded());
		if (Params.AmigoName == "silver")
		{
			Params.Animator.SetBool("Using Psychokinesis", UsePsiElements);
		}
	}

	private void UpdateEffects()
	{
		if (Params.JumpFX != null)
		{
			Params.UpdateJumpFX();
		}
		bool flag = GetAmigoState() == "Jump" && JumpAnimation == 1;
		Params.UpdateJumpBallFX(flag && (Params.AmigoName != "silver" || (Params.AmigoName == "silver" && SilverHasLotus)));
		if (ActiveSinkLavaParticle != null != IsSinking)
		{
			if (IsSinking && ColName == "2820000d")
			{
				ActiveSinkLavaParticle = Object.Instantiate(SinkLavaParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
				ActiveSinkLavaParticle.transform.SetParent(base.transform);
			}
			else
			{
				RemoveSinkParticles("Lava");
			}
		}
		if (ActiveSinkSandParticle != null != IsSinking && !IsDead)
		{
			if (IsSinking && ColName == "40000009")
			{
				ActiveSinkSandParticle = Object.Instantiate(SinkSandParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
				ActiveSinkSandParticle.transform.SetParent(base.transform);
			}
			else
			{
				RemoveSinkParticles("Sand");
			}
		}
		if (Params.AmigoName == "princess" && EliseAnimator != null)
		{
			if (Params.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash != EliseAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash)
			{
				EliseAnimator.Play(Params.Animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
			}
			AnimatorControllerParameter[] parameters = EliseAnimator.parameters;
			foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
			{
				switch (animatorControllerParameter.type)
				{
				case AnimatorControllerParameterType.Bool:
					EliseAnimator.SetBool(animatorControllerParameter.nameHash, Params.Animator.GetBool(animatorControllerParameter.nameHash));
					break;
				case AnimatorControllerParameterType.Float:
					EliseAnimator.SetFloat(animatorControllerParameter.nameHash, Params.Animator.GetFloat(animatorControllerParameter.nameHash));
					break;
				case AnimatorControllerParameterType.Int:
					EliseAnimator.SetInteger(animatorControllerParameter.nameHash, Params.Animator.GetInteger(animatorControllerParameter.nameHash));
					break;
				case AnimatorControllerParameterType.Trigger:
					if (Params.Animator.GetBool(animatorControllerParameter.nameHash))
					{
						EliseAnimator.SetTrigger(animatorControllerParameter.nameHash);
					}
					else
					{
						EliseAnimator.ResetTrigger(animatorControllerParameter.nameHash);
					}
					break;
				}
			}
		}
		if (Params.AmigoName == "shadow")
		{
			for (int j = 0; j < JetFireFX.Length; j++)
			{
				ParticleSystem.EmissionModule emission = JetFireFX[j].emission;
				emission.enabled = ((GetAmigoState() == "Ground" || GetAmigoState() == "Path" || GetAmigoState() == "DashPanel") && CurSpeed > 0f && !WalkSwitch) || (GetAmigoState() == "Result" && Time.time - ResultStartTime > 0.45f && Time.time - ResultStartTime < 0.9f);
			}
			Gradient gradient = new Gradient();
			JetTrailAlpha = Mathf.Lerp(JetTrailAlpha, (IsGrounded() && CurSpeed > 34f) ? 1f : 0f, Time.deltaTime * 10f);
			gradient.SetKeys(new GradientColorKey[2]
			{
				new GradientColorKey(JetTrailColor, 0f),
				new GradientColorKey(JetTrailColor, 1f)
			}, new GradientAlphaKey[3]
			{
				new GradientAlphaKey(JetTrailAlpha, 0f),
				new GradientAlphaKey(0f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			for (int k = 0; k < JetTrails.Length; k++)
			{
				JetTrails[k].colorGradient = gradient;
			}
		}
		if (Params.AmigoName == "silver")
		{
			if (((GetAmigoState() == "Ground" || GetAmigoState() == "Path" || GetAmigoState() == "DashPanel") && CurSpeed >= 14.5f) || (GetAmigoState() == "Result" && Time.time - ResultStartTime > 1f))
			{
				if (!UsePsiElements)
				{
					UsePsiElements = true;
					Audio.PlayOneShot(PsychoSounds[0], Audio.volume * 0.5f);
					for (int l = 0; l < PsiPoints.Length; l++)
					{
						Object.Instantiate(PsiTrailFX, PsiPoints[l].position, Quaternion.identity).transform.SetParent(PsiPoints[l]);
					}
				}
			}
			else if (UsePsiElements)
			{
				UsePsiElements = false;
				Audio.PlayOneShot(PsychoSounds[1], Audio.volume * 0.5f);
			}
			for (int m = 0; m < PsiAuraFX.Length; m++)
			{
				ParticleSystem.EmissionModule emission2 = PsiAuraFX[m].emission;
				emission2.enabled = UsePsiElements;
			}
			PsiLoopSource.volume = Mathf.Lerp(PsiLoopSource.volume, UsePsiElements ? 0.5f : 0f, Time.deltaTime * 10f);
			for (int n = 0; n < SilverRenderers.Length; n++)
			{
				SilverRenderers[n].GetPropertyBlock(PropBlock);
				PropBlock.SetColor("_ExtFresColor", PsiGlows[0]);
				PropBlock.SetColor("_ExtGlowColor", PsiGlows[1]);
				PropBlock.SetFloat("_ExtPulseSpd", 1f);
				PropBlock.SetFloat("_ExtFresPower", 1f);
				PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), UsePsiElements ? 0.75f : 0f, Time.deltaTime * 10f));
				PropBlock.SetFloat("_GlowInt", Mathf.Lerp(PropBlock.GetFloat("_GlowInt"), UsePsiElements ? 1f : 0f, Time.deltaTime * 10f));
				PropBlock.SetColor("_OutlineColor", PsiGlows[0]);
				PropBlock.SetColor("_OutlinePulseColor", PsiGlows[1]);
				PropBlock.SetFloat("_OutlinePulseSpd", 1f);
				PropBlock.SetFloat("_OutlineInt", UsePsiElements ? 1f : 0f);
				SilverRenderers[n].SetPropertyBlock(PropBlock);
			}
		}
		if (Params.AmigoName == "tails")
		{
			if ((GetAmigoState() == "Ground" || GetAmigoState() == "DashPanel" || GetAmigoState() == "Path") && CurSpeed >= 14.5f)
			{
				if (!PlayedFlySource)
				{
					PlayedFlySource = true;
					FlySource.volume = 1f;
				}
				FlySource.volume = Mathf.Lerp(FlySource.volume, 0f, Time.deltaTime);
			}
			else
			{
				PlayedFlySource = false;
				FlySource.volume = Mathf.Lerp(FlySource.volume, 0f, 5f * Time.deltaTime);
			}
		}
		if (!(Params.AmigoName == "omega"))
		{
			return;
		}
		for (int num = 0; num < OmegaJetFireFX.Length; num++)
		{
			ParticleSystem.EmissionModule emission3 = OmegaJetFireFX[num].emission;
			if ((GetAmigoState() == "Ground" || GetAmigoState() == "DashPanel" || GetAmigoState() == "Path") && CurSpeed >= 20f)
			{
				emission3.enabled = true;
				if (!OmegaStartJetFire)
				{
					OmegaStartJetFire = true;
					CreateOmegaJetFireFX();
				}
			}
			else
			{
				emission3.enabled = false;
				OmegaStartJetFire = false;
			}
		}
	}

	public void CreateOmegaJetFireFX()
	{
		for (int i = 0; i < OmegaJetBones.Length; i++)
		{
			Object.Instantiate(OmegaJetFX, OmegaJetBones[i].position, OmegaJetBones[i].rotation).transform.SetParent(OmegaJetBones[i]);
		}
	}

	internal void RemoveSinkParticles(string Type)
	{
		if (Type == "Lava" && (bool)ActiveSinkLavaParticle)
		{
			ParticleSystem[] componentsInChildren = ActiveSinkLavaParticle.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Stop();
			}
			ActiveSinkLavaParticle.transform.SetParent(null);
			Object.Destroy(ActiveSinkLavaParticle, 3f);
			ActiveSinkLavaParticle = null;
		}
		if (Type == "Sand" && (bool)ActiveSinkSandParticle)
		{
			ParticleSystem[] componentsInChildren2 = ActiveSinkSandParticle.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].Stop();
			}
			ActiveSinkSandParticle.transform.SetParent(null);
			Object.Destroy(ActiveSinkSandParticle, 3f);
			ActiveSinkSandParticle = null;
			if (SinkPosition > 0.5f)
			{
				Object.Instantiate(JumpSandParticle, base.transform.position - base.transform.up * 0.25f, Quaternion.identity);
			}
		}
	}

	public void AddScore(int Score, bool Rainbow = false)
	{
		FollowTarget.AddScore(Score, Rainbow);
	}

	public void AddRing(int Amount = 1, AudioSource ThisRingSource = null)
	{
		FollowTarget.AddRing(Amount, ThisRingSource);
	}

	public virtual void OnDashPanel(Quaternion Rot, Vector3 Normal, Vector3 Forward, float Speed, float Timer)
	{
	}

	public virtual void OnSpring(Vector3 Pos, Vector3 Vel, float Timer, string LaunchAnimMode, bool UseTimerToExit, bool UseTimerToRelease, bool AlwaysLocked)
	{
	}

	public virtual void OnWideSpring(Vector3 Pos, Vector3 Vel, float Timer)
	{
	}

	public virtual void OnJumpPanel(Vector3 Pos, Vector3 Vel, float Timer, string LaunchAnimMode, bool UseTimerToExit, bool UseTimerToRelease, bool AlwaysLocked)
	{
	}

	public virtual void OnDeathEnter(int DeathType)
	{
	}

	public void DoLandAnim()
	{
		if (CurSpeed <= 0f && _Rigidbody.velocity.y < -10f)
		{
			Params.Animator.SetTrigger("On Land");
		}
	}

	public virtual string GetAmigoState()
	{
		return "null";
	}

	public virtual void SetAmigoMachineState(string StateName)
	{
	}

	public void DestroyAmigo()
	{
		Object.Destroy(SwitchTransform.gameObject);
		Object.Destroy(base.gameObject);
	}
}
