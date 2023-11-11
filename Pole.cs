using UnityEngine;

public class Pole : ObjectBase
{
	[Header("Framework")]
	public float Radius;

	public float Height;

	public float Power;

	public float Pitch;

	public float Yaw;

	public float Rotation_Time;

	public float Out_Time;

	[Header("Optional")]
	public bool BigPole;

	[Header("Prefab")]
	public CapsuleCollider Collider;

	public Animation PoleSignAnimation;

	public Transform PlayerPosition;

	public GameObject PoleSign;

	public AudioSource PoleLandAudio;

	public ParticleSystem PoleFX;

	public AudioClip[] ClipPool;

	public AudioClip[] WindSounds;

	public AudioSource PoleSource;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private Vector3 StartLaunchVelocity;

	private Quaternion MeshLaunchRot;

	private bool CanJump;

	private bool PlayPoleSignAnim;

	private bool Falling;

	private bool PlayedJumpOffAnim;

	private int PoleState;

	private float StartTime;

	private float LaunchTime;

	private float MeshPitch;

	public void SetParameters(float _Radius, float _Height, float _Power, float _Pitch, float _Yaw, float _Rotation_Time, float _Out_Time)
	{
		Radius = _Radius;
		Height = _Height;
		Power = _Power;
		Pitch = _Pitch;
		Yaw = _Yaw;
		Rotation_Time = _Rotation_Time;
		Out_Time = _Out_Time;
		PlayerPosition.localPosition = new Vector3(PlayerPosition.localPosition.x, PlayerPosition.localPosition.y, Height * 0.5f);
		PoleSign.transform.localPosition = new Vector3(PoleSign.transform.localPosition.x, PoleSign.transform.localPosition.y, Height * 0.5f);
	}

	private Vector3 LaunchDirection()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f - Pitch;
		eulerAngles.y += 270f - Yaw;
		return Quaternion.Euler(eulerAngles) * Vector3.forward;
	}

	private Vector3 PlayerPoint()
	{
		return base.transform.position + base.transform.forward * (Height * 0.5f);
	}

	private void Start()
	{
		Collider.center = new Vector3(0f, 0f, Height * 0.5f);
		Collider.radius = Radius;
		Collider.height = Height;
	}

	private void Update()
	{
		if (!(PM != null) || Singleton<GameManager>.Instance.GameState == GameManager.State.Paused)
		{
			return;
		}
		if (PM.Base.GetState() == "Pole" && PoleState == 1 && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			if (CanJump)
			{
				PoleState = 2;
				CanJump = false;
				PoleSignAnimation.Stop();
				PoleSignAnimation.enabled = false;
				MeshLaunchRot = Quaternion.LookRotation(StartLaunchVelocity.MakePlanar());
				StartTime = Time.time;
				PM.Base.PlayAnimation((!BigPole) ? "Pole Jump" : "Big Pole Jump", (!BigPole) ? "On Pole Jump" : "On Big Pole Jump");
				PM.Base.Audio.PlayOneShot(WindSounds[Random.Range(0, WindSounds.Length)], PM.Base.Audio.volume);
				PoleSource.PlayOneShot(ClipPool[1], PoleSource.volume);
			}
			else
			{
				PoleSignAnimation.Stop();
				PoleSignAnimation.enabled = false;
				PM.Base.CurSpeed = PM.Base.TopSpeed * 0.5f;
				Vector3 velocity = PM.RBody.velocity;
				velocity.y = 12f;
				PM.RBody.velocity = velocity;
				PM.Base.SetMachineState("StateAir");
			}
			PoleSign.SetActive(value: false);
		}
		else if (PM.Base.GetState() != "Pole" && PoleSign.activeSelf)
		{
			PoleSign.SetActive(value: false);
			PoleSignAnimation.Stop();
			PoleSignAnimation.enabled = false;
		}
	}

	private void StatePoleStart()
	{
		PM.Base.SetState("Pole");
		PM.Base.PlayAnimation((!BigPole) ? "Pole" : "Big Pole", (!BigPole) ? "On Pole" : "On Big Pole");
		LaunchVelocity = LaunchDirection() * Power;
		PM.transform.forward = LaunchVelocity.MakePlanar();
		StartLaunchVelocity = LaunchVelocity.normalized;
		MeshLaunchRot = Quaternion.LookRotation(-base.transform.right, PM.transform.up);
		PM.transform.position = base.transform.GetChild(0).position + -base.transform.GetChild(0).up * 0.25f;
		PM.RBody.velocity = Vector3.zero;
		PoleSign.SetActive(value: false);
		StartTime = Time.time;
		PoleState = 0;
		PlayPoleSignAnim = false;
		Falling = false;
		PlayedJumpOffAnim = false;
		MeshPitch = 0f;
		PoleFX.transform.position = PoleSign.transform.position;
		PoleFX.transform.rotation = Quaternion.LookRotation(-base.transform.right, PM.transform.up);
		PoleFX.Play();
		PoleLandAudio.Play();
	}

	private void StatePole()
	{
		PM.Base.SetState("Pole");
		if (PoleState == 0 && Time.time - StartTime < 0.5f)
		{
			PM.Base.PlayAnimation((!BigPole) ? "Pole" : "Big Pole", (!BigPole) ? "On Pole" : "On Big Pole");
		}
		if (Time.time - StartTime > 0.615f && PoleState != 2)
		{
			PoleState = 1;
			PoleSign.SetActive(value: true);
			if (!PlayPoleSignAnim && Time.time - StartTime > 0.7f)
			{
				PoleSignAnimation.enabled = true;
				PoleSignAnimation.Play();
				PoleSignAnimation["PoleSign"].speed = 0.5f / Rotation_Time;
				PlayPoleSignAnim = true;
			}
			MeshPitch -= Time.fixedDeltaTime * (360f / Rotation_Time);
			MeshLaunchRot = Quaternion.LookRotation(-base.transform.right, PM.transform.up) * Quaternion.Euler(MeshPitch, 0f, 0f);
		}
		if (PoleState == 2)
		{
			if (Time.time - StartTime > 0.15f && !PlayedJumpOffAnim)
			{
				PlayedJumpOffAnim = true;
				LaunchTime = Time.time;
			}
			if (Time.time - StartTime > 0.15f)
			{
				bool flag = Time.time - LaunchTime < Power * 0.025f;
				if (Time.time - LaunchTime > Power * 0.025f)
				{
					if (Time.time - LaunchTime < Out_Time)
					{
						PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
						PM.transform.forward = LaunchVelocity.MakePlanar();
						LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						PM.Base.LockControls = true;
					}
					else
					{
						PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
						Vector3 vector = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
						if (PM.RBody.velocity.magnitude != 0f)
						{
							vector = PM.transform.forward * PM.Base.CurSpeed;
							LaunchVelocity = new Vector3(vector.x, LaunchVelocity.y, vector.z);
						}
						LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
						LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
						PM.Base.DoWallNormal();
						PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
					}
					MeshLaunchRot = PM.transform.rotation;
					if (PM.RBody.velocity.y < 0f && !Falling)
					{
						Falling = true;
						PM.Base.PlayAnimation("Pole Exit", "On Pole Exit");
					}
				}
				if (PM.Base.IsGrounded() && !flag)
				{
					PM.Base.PositionToPoint();
					PM.Base.SetMachineState("StateGround");
					PM.Base.DoLandAnim();
					PM.PlayerEvents.CreateLandFXAndSound();
				}
				PM.RBody.velocity = LaunchVelocity;
			}
		}
		else
		{
			PM.Base.LockControls = true;
		}
		PM.Base.GeneralMeshRotation = MeshLaunchRot;
	}

	private void StatePoleEnd()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!(player == null) && !player.IsDead && (player.GetPrefab("sonic_new") || player.GetPrefab("shadow")))
		{
			PM = collider.GetComponent<PlayerManager>();
			player.StateMachine.ChangeState(base.gameObject, StatePole);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = PlayerPoint() + LaunchDirection() * Power * (Power * 0.025f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(PlayerPoint(), vector);
		int num = 10 * (int)Power;
		float num2 = 0.01f;
		Vector3 vector2 = LaunchDirection() * Power;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 from = vector;
		for (int i = 0; i < num; i++)
		{
			vector2.y -= 20f * num2;
			vector3 += vector2 * num2;
			Gizmos.DrawLine(from, vector3);
			from = vector3;
		}
	}

	public void SwitchBool(int _switch)
	{
		CanJump = _switch == 1;
	}

	public void PlayPoleAudio()
	{
		PoleSource.PlayOneShot(ClipPool[0], PoleSource.volume);
	}
}
