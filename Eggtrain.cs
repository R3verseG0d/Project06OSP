using System.Collections.Generic;
using UnityEngine;

public class Eggtrain : EventStation
{
	public enum Mode
	{
		Mach = 0,
		Escape = 1,
		Chase = 2
	}

	[Header("Framework")]
	public string Path;

	public float Speed;

	public float InitialProgress;

	public int Num;

	public float Interval;

	public bool Loop;

	public Mode Type;

	public float BombTime;

	public int HP1;

	public int HP2;

	public string Event;

	public string TimeOver;

	public int Break;

	public float WaitTime;

	public float Distance;

	public bool Left;

	public Vector3 m_Camera;

	public bool NoDmg;

	[Header("Prefab")]
	public Animator[] Models;

	public GameObject CargoObj;

	public GameObject CargoHealthyObj;

	public Transform[] Wheels;

	public Animator ChaseCamAnimator;

	public Transform ChaseCamPosition;

	public Transform CamTarget;

	public ParticleSystem DamageFX;

	public ParticleSystem[] BarrierSignalFX;

	public GameObject ExplosionsFX;

	public AudioSource HornSource;

	public AudioSource DamageSource;

	public AudioSource TrainRunSource;

	public AudioSource BarrierSignalSource;

	[Header("Object Framework")]
	public GameObject TimeOverHint;

	public float CargoOffset;

	internal bool Damaged;

	private List<Cargo> JointedCargos = new List<Cargo>();

	private BezierCurve Curve;

	private PlayerManager PM;

	private Transform Player;

	private Vector3 RepelStartVelocity;

	private Vector3 RepelVelocity;

	private Quaternion RepelMeshLaunchRot;

	private bool StartProgress;

	private bool GoFast;

	private bool TimeOverEvent;

	private bool TimeOverDialogue;

	private bool ChaseDamaged;

	private bool RepelFalling;

	private float FinalSpeed;

	private float Progress;

	private float PlayerRadius;

	private float WaitTimeStart;

	private float RepelTime;

	private float DamageTime;

	private int HP;

	private int MaxHP;

	public void SetParameters(string _Path, float _Speed, float _InitialProgress, int _Num, float _Interval, bool _Loop, int _Type, float _BombTime, int _HP1, int _HP2, string _Event, string _TimeOver, int _Break, float _WaitTime, float _Distance, bool _Left, Vector3 _Camera)
	{
		Path = _Path;
		Speed = _Speed;
		InitialProgress = _InitialProgress;
		Num = _Num;
		Interval = _Interval;
		Loop = _Loop;
		Type = (Mode)(_Type - 1);
		BombTime = _BombTime;
		HP1 = _HP1;
		HP2 = _HP2;
		Event = _Event;
		TimeOver = _TimeOver;
		Break = _Break;
		WaitTime = _WaitTime;
		Distance = _Distance;
		Left = _Left;
		m_Camera = _Camera;
	}

	private void Start()
	{
		Curve = GameObject.Find(Path).GetComponent<BezierCurve>();
		Progress = InitialProgress;
		base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
		float num = CargoOffset;
		for (int i = 1; i < Num; i++)
		{
			if (i > 1)
			{
				num += CargoOffset * ((i < 2) ? 1f : 0.9f);
			}
			Cargo component = Object.Instantiate((Type == Mode.Mach) ? CargoObj : CargoHealthyObj, Curve.GetTangent(0f), Quaternion.LookRotation(Curve.GetTangent(0f))).GetComponent<Cargo>();
			component.Curve = Curve;
			component.Speed = Speed;
			component.Progress = Progress - num;
			component.Loop = Loop;
			component.UseHP = Type == Mode.Chase;
			if (component.UseHP)
			{
				component.HP = HP2;
				component.MaxHP = HP2;
			}
			JointedCargos.Add(component);
		}
		if (Type != Mode.Escape)
		{
			PlayerRadius = Distance * (float)(Num - 1);
		}
		else
		{
			DamageTime = Time.time;
		}
		if (Type != 0)
		{
			HP = HP1;
			MaxHP = HP1;
		}
		Models[0].gameObject.SetActive(Break < 2);
		Models[1].gameObject.SetActive(Break == 2);
		Models[2].gameObject.SetActive(Break > 2);
		TrainRunSource.time = Random.Range(0.1f, TrainRunSource.clip.length);
	}

	private void Update()
	{
		if (Type == Mode.Mach || Type == Mode.Chase)
		{
			for (int i = 0; i < JointedCargos.Count; i++)
			{
				if (JointedCargos[i] == null)
				{
					PlayerRadius -= Interval;
					JointedCargos.RemoveAt(i);
				}
			}
			if (Player == null)
			{
				Player = GameObject.FindGameObjectWithTag("Player").transform;
			}
			float num = ((Vector3.Distance(base.transform.position, Player.position) > PlayerRadius) ? 1f : (-1f));
			if (Type == Mode.Chase)
			{
				FinalSpeed = Mathf.Lerp(FinalSpeed, Damaged ? 0f : ((num > 0f) ? Speed : 120f), Time.deltaTime * ((!Damaged) ? 1.5f : 0.5f));
				if (!Damaged && HP <= 0)
				{
					DamageFX.Play();
					DamageSource.pitch = Random.Range(0.75f, 1f);
					DamageSource.Play();
					ChaseCamAnimator.SetTrigger("On Neutralized");
					Object.FindObjectOfType<StageManager>().StageState = StageManager.State.Event;
					Object.FindObjectOfType<PlayerBase>().SetCameraParams(new CameraParameters(3, m_Camera, base.transform.position, ChaseCamPosition, CamTarget));
					ExplosionsFX.SetActive(value: true);
					Invoke("OnNeutralized", 3f);
					Damaged = true;
				}
			}
			else
			{
				FinalSpeed = Mathf.Lerp(FinalSpeed, (num > 0f) ? Speed : 120f, Time.deltaTime * 3f);
			}
		}
		else
		{
			FinalSpeed = Speed * (GoFast ? 5f : 1f);
			if (TimeOverEvent && Time.time - WaitTimeStart > WaitTime && !Damaged)
			{
				CallEvent(TimeOver);
				PlaySignal(Enable: false);
				TimeOverEvent = false;
			}
			if (TimeOverDialogue && Time.time - WaitTimeStart > WaitTime / 1.5f && !Damaged)
			{
				TimeOverHint.SetActive(value: true);
				TimeOverDialogue = false;
			}
			if (!Damaged && HP <= 0)
			{
				Models[Break - 1].gameObject.SetActive(value: false);
				Models[Break].gameObject.SetActive(value: true);
				PM.Base.StateMachine.ChangeState(base.gameObject, StateTrainRepel);
				DamageFX.Play();
				PlaySignal(Enable: false);
				Invoke("OnDamage", 1f);
				Damaged = true;
			}
		}
		if (StartProgress)
		{
			Progress += FinalSpeed / Curve.Length() * Time.deltaTime;
			for (int j = 0; j < JointedCargos.Count; j++)
			{
				JointedCargos[j].Speed = FinalSpeed;
			}
			for (int k = 0; k < Wheels.Length; k++)
			{
				Wheels[k].Rotate(FinalSpeed * Time.deltaTime * 75f, 0f, 0f);
			}
		}
		if (Progress >= 1f)
		{
			if (!Loop)
			{
				if (Type != Mode.Chase)
				{
					StartProgress = false;
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
			else
			{
				Progress = 0f;
			}
		}
		base.transform.position = Curve.GetPosition(Progress);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Curve.GetTangent(Progress)), Time.deltaTime * 5f);
		TrainRunSource.volume = Mathf.Lerp(TrainRunSource.volume, StartProgress ? 0.75f : 0f, Time.deltaTime * 5f);
	}

	private void StateTrainRepelStart()
	{
		PM.Base.SetState("TrainRepel");
		RepelTime = Time.time;
		RepelVelocity = base.transform.right * (Left ? (-10f) : 10f) + Vector3.up * 5f;
		RepelStartVelocity = RepelVelocity.normalized;
		RepelMeshLaunchRot = Quaternion.LookRotation(RepelStartVelocity) * Quaternion.Euler(30f, 0f, 0f);
		if (PM.Base.IsGrounded())
		{
			PM.transform.position = base.transform.position + base.transform.up * 3.5f + base.transform.right * (Left ? (-3f) : 3f);
		}
		PM.transform.forward = RepelVelocity.MakePlanar();
		PM.Base.MaxRayLenght = 0.75f;
		RepelFalling = false;
	}

	private void StateTrainRepel()
	{
		PM.Base.SetState("TrainRepel");
		PM.Base.LockControls = true;
		bool num = Time.time - RepelTime < 0.1f;
		if (num)
		{
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
			RepelFalling = false;
		}
		else if (PM.RBody.velocity.y > -0.1f)
		{
			PM.Base.PlayAnimation("Spring Jump", "On Spring");
			RepelFalling = false;
		}
		else if (!RepelFalling)
		{
			RepelFalling = true;
			PM.Base.PlayAnimation("Roll And Fall", "On Roll And Fall");
		}
		if (num)
		{
			RepelMeshLaunchRot = Quaternion.Slerp(RepelMeshLaunchRot, Quaternion.LookRotation(RepelStartVelocity) * Quaternion.Euler(90f, 0f, 0f), Time.fixedDeltaTime * 5f);
			PM.RBody.velocity = RepelVelocity;
			PM.transform.forward = RepelVelocity.MakePlanar();
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
		}
		else
		{
			PM.Base.CurSpeed = PM.RBody.velocity.magnitude;
			PM.transform.forward = RepelVelocity.MakePlanar();
			RepelVelocity.y -= 9.81f * Time.fixedDeltaTime;
			RepelMeshLaunchRot = Quaternion.Slerp(RepelMeshLaunchRot, (PM.RBody.velocity.y < 0f) ? Quaternion.LookRotation(RepelVelocity.MakePlanar()) : (Quaternion.LookRotation(RepelStartVelocity) * Quaternion.Euler(90f, 0f, 0f)), Time.fixedDeltaTime * 5f);
			PM.RBody.velocity = RepelVelocity;
		}
		PM.Base.GeneralMeshRotation = RepelMeshLaunchRot;
		PM.Base.TargetDirection = Vector3.zero;
		if (PM.Base.IsGrounded() && Time.time - RepelTime > 0.1f)
		{
			PM.Base.PositionToPoint();
			PM.Base.SetMachineState("StateGround");
			PM.PlayerEvents.CreateLandFXAndSound();
		}
		if (PM.Base.FrontalCollision && Time.time - RepelTime > 0.1f)
		{
			PM.Base.SetMachineState("StateAir");
		}
	}

	private void StateTrainRepelEnd()
	{
	}

	private void PlaySignal(bool Enable)
	{
		for (int i = 0; i < BarrierSignalFX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = BarrierSignalFX[i].emission;
			emission.enabled = Enable;
		}
		BarrierSignalSource.volume = (Enable ? 1f : 0f);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 30f);
		if (array == null)
		{
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			Common_Laser componentInParent = array[j].GetComponentInParent<Common_Laser>();
			if ((bool)componentInParent && !componentInParent.PairSettings)
			{
				componentInParent.SlowTurnOff();
			}
		}
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (NoDmg)
		{
			return;
		}
		if (Type == Mode.Escape && Time.time - DamageTime > 0.2f)
		{
			if (!TimeOverEvent || Damaged)
			{
				return;
			}
			HP--;
			if (!PM && HitInfo.player.tag == "Player")
			{
				PM = HitInfo.player.GetComponent<PlayerManager>();
			}
			Models[Break - 1].SetTrigger("On Hit");
			DamageFX.Play();
			DamageSource.pitch = Random.Range(0.75f, 1f);
			DamageSource.Play();
			DamageTime = Time.time;
		}
		if (Type == Mode.Chase && !Damaged)
		{
			HP--;
			if ((float)HP < (float)MaxHP / 2f && !ChaseDamaged)
			{
				Models[Break - 1].gameObject.SetActive(value: false);
				Models[Break].gameObject.SetActive(value: true);
				DamageFX.Play();
				DamageSource.pitch = Random.Range(0.75f, 1f);
				DamageSource.Play();
				ChaseDamaged = true;
			}
		}
	}

	private void OnDamage()
	{
		CallEvent(Event);
	}

	private void OnNeutralized()
	{
		CallEvent("eggman_train_destroy");
	}

	private void Go()
	{
		StartProgress = true;
		for (int i = 0; i < JointedCargos.Count; i++)
		{
			JointedCargos[i].StartProgress = true;
		}
	}

	private void Fast()
	{
		if (Type != Mode.Chase || !Damaged)
		{
			GoFast = true;
			StartProgress = true;
			for (int i = 0; i < JointedCargos.Count; i++)
			{
				JointedCargos[i].StartProgress = true;
			}
		}
	}

	private void Stop()
	{
		GoFast = false;
		StartProgress = false;
		for (int i = 0; i < JointedCargos.Count; i++)
		{
			JointedCargos[i].StartProgress = false;
		}
		TimeOverEvent = true;
		TimeOverDialogue = true;
		WaitTimeStart = Time.time;
		PlaySignal(Enable: true);
	}

	private void Horn()
	{
		if (Type != Mode.Chase || !Damaged)
		{
			HornSource.Play();
		}
	}

	private void Cut()
	{
		JointedCargos[JointedCargos.Count - 1].CutBomb(BombTime);
	}

	private void Camera()
	{
		if (Type != Mode.Chase || !Damaged)
		{
			Object.FindObjectOfType<StageManager>().StageState = StageManager.State.Event;
			Object.FindObjectOfType<PlayerBase>().SetCameraParams(new CameraParameters(3, m_Camera, base.transform.position, null, CamTarget));
		}
	}

	private void Bomb()
	{
		if (Type != Mode.Chase || !Damaged)
		{
			StartCoroutine(Object.FindObjectOfType<PlayerBase>().RestartStage());
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Type != Mode.Escape)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, PlayerRadius);
		}
		else if (m_Camera != Vector3.zero)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(base.transform.position, m_Camera);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(m_Camera, 0.5f);
		}
	}
}
