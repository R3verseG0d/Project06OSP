using UnityEngine;

public class Lotus : PsiObject
{
	[Header("Framework")]
	public float Spd1;

	public float Spd2;

	public float Spd3;

	public float Charge1;

	public float Charge2;

	public float Charge3;

	public bool ESPMode;

	public float NoConTime;

	[Header("Prefab")]
	public Animator Animator;

	public Renderer Renderer;

	public Common_PsiMarkSphere ESPMark;

	public Collider TriggerCollider;

	public Collider Collider;

	public Transform MarkPoint;

	public ParticleSystem LotusFX;

	public ParticleSystem FullChargeFX;

	public AudioSource Audio;

	public AudioClip[] Clips;

	private PlayerManager PM;

	private Vector3 LaunchVelocity;

	private float LaunchSpeed;

	private float ChargeTime;

	public int IntensityState;

	public bool Falling;

	private float StateTime;

	private int State;

	public void SetParameters(float _Spd1, float _Spd2, float _Spd3, float _Charge1, float _Charge2, float _Charge3, bool _ESPMode, float _NoConTime)
	{
		Spd1 = _Spd1;
		Spd2 = _Spd2;
		Spd3 = _Spd3;
		Charge1 = _Charge1;
		Charge2 = _Charge2;
		Charge3 = _Charge3;
		ESPMode = _ESPMode;
		NoConTime = _NoConTime;
	}

	public override void Awake()
	{
		if (ESPMode)
		{
			base.Awake();
		}
	}

	private void Start()
	{
		if (ESPMode && !ESPMark.gameObject.activeSelf)
		{
			ESPMark.gameObject.SetActive(value: true);
			ESPMark.Target = base.gameObject;
			Collider.enabled = true;
			TriggerCollider.enabled = false;
		}
	}

	private void StateLotusStart()
	{
		PM.Base.SetState("Lotus");
		State = 0;
		Falling = false;
		if (!ESPMode)
		{
			switch (IntensityState)
			{
			case 1:
				LaunchSpeed = Spd1;
				ChargeTime = Charge1;
				break;
			case 2:
				LaunchSpeed = Spd2;
				ChargeTime = Charge2;
				break;
			case 3:
				LaunchSpeed = Spd3;
				ChargeTime = Charge3;
				break;
			default:
				LaunchSpeed = Spd1;
				ChargeTime = Charge1;
				break;
			}
			LaunchSpeed -= ChargeTime;
			float num = 1f / ChargeTime;
			Animator.speed = ((ChargeTime < 0.25f) ? 4f : num);
		}
		else
		{
			LaunchSpeed = Spd1;
			ChargeTime = Charge1;
			LaunchSpeed -= ChargeTime;
			float speed = 1f / Charge3;
			Animator.speed = speed;
			PM.silver.PsiFX = true;
		}
		PM.transform.position = MarkPoint.position + MarkPoint.up * 0.25f;
		StateTime = Time.time;
		LaunchVelocity = PM.RBody.velocity;
		PM.RBody.velocity = LaunchVelocity;
		Audio.PlayOneShot(Clips[0], Audio.volume);
	}

	private void StateLotus()
	{
		PM.Base.SetState("Lotus");
		PM.Base.LockControls = State == 0 || (State == 1 && Time.time - StateTime < NoConTime);
		if (State == 0)
		{
			if (!ESPMode)
			{
				PM.Base.PlayAnimation("Crouch", "On Crouch");
			}
			LaunchVelocity = Vector3.zero;
			PM.RBody.velocity = LaunchVelocity;
			PM.transform.position = MarkPoint.position + MarkPoint.up * 0.25f;
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
			if (ESPMode)
			{
				float num = Time.time - StateTime;
				if (num < Charge1)
				{
					IntensityState = 0;
					LaunchSpeed = Spd1 / 2f;
					ChargeTime = Charge1 / 2f;
				}
				else if (num >= Charge1 && num < Charge2)
				{
					IntensityState = 1;
					LaunchSpeed = Spd1;
					ChargeTime = Charge1;
				}
				else if (num >= Charge2 && num < Charge3)
				{
					IntensityState = 2;
					LaunchSpeed = Spd2;
					ChargeTime = Charge2;
				}
				else if (num >= Charge3)
				{
					IntensityState = 3;
					LaunchSpeed = Spd3;
					ChargeTime = Charge3;
					if (!FullChargeFX.isPlaying)
					{
						FullChargeFX.Play();
					}
				}
			}
		}
		else
		{
			bool flag = Time.time - StateTime < NoConTime;
			if (flag)
			{
				PM.Base.CurSpeed = 0f;
				LaunchVelocity = Vector3.up * LaunchSpeed;
				PM.RBody.velocity = LaunchVelocity;
			}
			else
			{
				Vector3 vector = new Vector3(LaunchVelocity.x, 0f, LaunchVelocity.z);
				if (PM.RBody.velocity.magnitude != 0f)
				{
					vector = PM.transform.forward * PM.Base.CurSpeed;
					LaunchVelocity = new Vector3(vector.x, LaunchVelocity.y, vector.z);
				}
				LaunchVelocity.y -= 25f * Time.fixedDeltaTime;
				LaunchVelocity.y = PM.Base.LimitVel(LaunchVelocity.y);
				PM.RBody.velocity = LaunchVelocity;
				PM.Base.DoWallNormal();
			}
			PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
			if (LaunchVelocity.y < 0f)
			{
				if (!Falling)
				{
					Falling = true;
					PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
					if (ESPMode)
					{
						Collider.enabled = true;
						TriggerCollider.enabled = false;
					}
				}
			}
			else
			{
				Falling = false;
				PM.Base.PlayAnimation("Spring Jump", "On Spring");
			}
			if (PM.Base.IsGrounded() && !flag)
			{
				PM.Base.PositionToPoint();
				PM.Base.SetMachineState("StateGround");
				PM.Base.DoLandAnim();
				PM.PlayerEvents.CreateLandFXAndSound();
			}
		}
		PM.Base.GeneralMeshRotation = Quaternion.LookRotation(PM.Base.ForwardMeshRotation, PM.Base.UpMeshRotation);
	}

	private void StateLotusEnd()
	{
	}

	private void Update()
	{
		if (!ESPMode)
		{
			return;
		}
		if ((bool)PM)
		{
			if (PM.Base.GetState() == "Lotus" && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && State == 0 && !Singleton<RInput>.Instance.P.GetButton("Right Trigger"))
			{
				LotusNormalSpeed();
			}
			if (PM.Base.GetState() != "Lotus" && !Falling)
			{
				Falling = true;
				if (ESPMode)
				{
					Collider.enabled = true;
					TriggerCollider.enabled = false;
				}
			}
		}
		OnPsiFX(Renderer, (bool)PM && PM.Base.GetState() == "Lotus" && State == 0);
	}

	public void LotusNormalSpeed()
	{
		Animator.speed = 1f;
		State = 1;
		StateTime = Time.time;
		LotusFX.Play();
		Audio.Stop();
		Audio.PlayOneShot(Clips[1], Audio.volume);
		if (ESPMode)
		{
			Animator.SetTrigger("On Launch ESP");
			if (FullChargeFX.isPlaying)
			{
				FullChargeFX.Stop();
			}
			PM.silver.PsiFX = false;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!player || player.IsDead || (player.GetState() == "Lotus" && !Falling))
		{
			return;
		}
		if (!ESPMode)
		{
			if (player.GetState() != "Lotus")
			{
				IntensityState = 0;
			}
			if (IntensityState < 3)
			{
				IntensityState++;
			}
			Animator.SetTrigger("On Trigger");
		}
		else
		{
			Animator.SetTrigger("On Trigger ESP");
		}
		PM = collider.GetComponent<PlayerManager>();
		player.StateMachine.ChangeState(base.gameObject, StateLotus);
	}

	private void OnEventSignal()
	{
		Collider.enabled = false;
		TriggerCollider.enabled = true;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * Spd1);
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Spd1, 0.25f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position + Vector3.up * Spd1, base.transform.position + Vector3.up * Spd2);
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Spd2, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position + Vector3.up * Spd2, base.transform.position + Vector3.up * Spd3);
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Spd3, 0.75f);
	}
}
