using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoChase : MonoBehaviour
{
	[Header("Framework")]
	public float PosY;

	[Header("Optional Framework")]
	public AudioSource Ambience;

	[Header("Prefab")]
	public GameObject[] ShootObjects;

	public Rigidbody[] CarryingObjects;

	public Rigidbody _Rigidbody;

	public float StartDistance;

	public float MinBreakpoint;

	public float LgBreakpoint;

	public float BurnRange;

	public Vector3 Camera;

	public BezierCurve DeathCurve;

	public Renderer[] Renderers;

	public ParticleSystem[] FX;

	public Transform IgniteFX;

	public Light Light;

	private List<Vector3> CarryingObjsRot = new List<Vector3>();

	private PlayerManager PM;

	private Vector3 StartPos;

	private bool Dissipate;

	private float Speed;

	private float CarrySpeed;

	private MaterialPropertyBlock PropBlock;

	private float TornadoProgress;

	public void SetParameters(float _PosY)
	{
		PosY = _PosY;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void StateTornadoDeathStart()
	{
		PM.Base.SetState("TornadoDeath");
		PM.Base.PlayerVoice.PlayRandom(0);
		StartCoroutine(PM.Base.RestartStage());
	}

	private void StateTornadoDeath()
	{
		PM.Base.SetState("TornadoDeath");
		PM.Base.PlayAnimation("Death Fall", "On Death Air");
		PM.Base.LockControls = true;
		PM.Base._Rigidbody.velocity = Vector3.zero;
		float num = Mathf.PerlinNoise(base.transform.position.x * 0.001f, base.transform.position.z * 0.001f) * 2f - 1f;
		float num2 = Mathf.PerlinNoise(base.transform.position.z * 0.001f, base.transform.position.y * 0.001f) * 2f - 1f;
		float num3 = Mathf.PerlinNoise(base.transform.position.z * 0.001f, base.transform.position.y * 0.001f) * 2f - 1f;
		Quaternion quaternion = Quaternion.AngleAxis(num * 50f, Vector3.left);
		Quaternion quaternion2 = Quaternion.AngleAxis(num2 * 50f, Vector3.up);
		Quaternion quaternion3 = Quaternion.AngleAxis(num3 * 50f, Vector3.forward);
		PM.Base.GeneralMeshRotation *= quaternion * quaternion2 * quaternion3;
		PM.transform.rotation = Quaternion.FromToRotation(PM.transform.up, Vector3.up) * PM.transform.rotation;
		TornadoProgress += 85f / DeathCurve.Length() * Time.fixedDeltaTime;
		if (TornadoProgress > 1f)
		{
			TornadoProgress = 1f;
		}
		PM.transform.position = DeathCurve.GetPosition(TornadoProgress);
	}

	private void StateTornadoDeathEnd()
	{
	}

	private void Start()
	{
		PM = Object.FindObjectOfType<PlayerManager>();
		StartPos = base.transform.position;
		for (int i = 0; i < CarryingObjects.Length; i++)
		{
			Vector3 item = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
			CarryingObjsRot.Add(item);
		}
		for (int j = 0; j < Renderers.Length; j++)
		{
			Renderers[j].GetPropertyBlock(PropBlock);
			PropBlock.SetFloat("_Int", 0f);
			Renderers[j].SetPropertyBlock(PropBlock);
		}
		Light.intensity = 0f;
		IgniteFX.SetParent(null);
	}

	private void Update()
	{
		if (!PM)
		{
			return;
		}
		if (!Dissipate)
		{
			if ((PM.transform.position - StartPos).magnitude > StartDistance)
			{
				if (PM.Base.GetState() != "Result" && !PM.Base.IsDead)
				{
					if (CalcDistance() < MinBreakpoint + BurnRange && PM.Base.HUD.Rings != 0)
					{
						PM.Base.OnBulletHit(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 4f, 3);
					}
					if (CalcDistance() < MinBreakpoint)
					{
						KillPlayer();
					}
				}
				if (PM.Base.GetState() != "Result" && !PM.Base.IsDead)
				{
					if (PM.Base.GetState() == "WallSlam" || PM.Base.GetState() == "WallSlam")
					{
						_Rigidbody.velocity = new Vector3((0f - PM.Base.WalkSpeed) * 0.25f, 0f, 0f);
					}
					else if (PM.Base.CurSpeed > PM.Base.TopSpeed && CalcDistance() > LgBreakpoint)
					{
						_Rigidbody.velocity = new Vector3(0f - PM.Base.CurSpeed, 0f, 0f);
					}
					else
					{
						_Rigidbody.velocity = new Vector3((0f - PM.Base.TopSpeed) * ((CalcDistance() < MinBreakpoint + BurnRange) ? 0.65f : 1f), 0f, 0f);
					}
				}
				else
				{
					_Rigidbody.velocity = Vector3.zero;
				}
			}
			CarrySpeed = Mathf.Lerp(CarrySpeed, 2.5f, Time.deltaTime);
			for (int i = 0; i < CarryingObjects.Length; i++)
			{
				CarryingObjects[i].transform.Rotate(CarryingObjsRot[i] * Time.deltaTime * CarrySpeed);
			}
			for (int j = 0; j < Renderers.Length; j++)
			{
				Renderers[j].GetPropertyBlock(PropBlock);
				PropBlock.SetFloat("_Int", Mathf.Lerp(PropBlock.GetFloat("_Int"), 1f, Time.deltaTime * 2f));
				Renderers[j].SetPropertyBlock(PropBlock);
			}
			Light.intensity = Mathf.Lerp(Light.intensity, 7.5f, Time.deltaTime * 2f);
		}
		else
		{
			if ((bool)Ambience)
			{
				Ambience.volume = Mathf.Lerp(Ambience.volume, 0f, Time.deltaTime * 2f);
			}
			for (int k = 0; k < Renderers.Length; k++)
			{
				Renderers[k].GetPropertyBlock(PropBlock);
				PropBlock.SetFloat("_Int", Mathf.Lerp(PropBlock.GetFloat("_Int"), 0f, Time.deltaTime * 2f));
				Renderers[k].SetPropertyBlock(PropBlock);
			}
			Light.intensity = Mathf.Lerp(Light.intensity, 0f, Time.deltaTime * 2f);
		}
	}

	private void KillPlayer()
	{
		PM.Base.OnDeathEnter(100);
		PM.Base.StateMachine.ChangeState(base.gameObject, StateTornadoDeath);
		PM.Base.SetCameraParams(new CameraParameters(4, base.transform.position + Camera, PM.transform.position + PM.transform.up * 0.25f));
		PM.Base.Camera.UncancelableEvent = true;
	}

	private IEnumerator PreThrow(GameObject Car)
	{
		HingeJoint[] Hingejoints = Car.GetComponentsInChildren<HingeJoint>();
		HingeJoint[] array = Hingejoints;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].breakForce *= 10000f;
		}
		yield return new WaitForSeconds(1f);
		array = Hingejoints;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].breakForce /= 10000f;
		}
		yield return null;
	}

	private float CalcDistance()
	{
		return (PM.transform.position - base.transform.position).magnitude;
	}

	private void TornadoShoot(TornadoChaseParams Params)
	{
		if (Params.ShootIndex != 0)
		{
			bool num = Params.TargetPos == Vector3.zero;
			Vector3 vector = ((!num) ? (base.transform.forward * Random.Range(-2.5f, 2.5f)) : new Vector3(0f, 0f, PM.transform.position.z));
			GameObject gameObject = Object.Instantiate(ShootObjects[Params.ShootIndex], base.transform.position + base.transform.up * PosY + vector, Random.rotation);
			Rigidbody component = gameObject.FindInChildren("RootMesh").GetComponent<Rigidbody>();
			StartCoroutine(PreThrow(gameObject));
			Speed = Mathf.Clamp(Speed, PM.Base.CurSpeed, 60f);
			Vector3 force = -Vector3.right * Speed * 2.25f;
			force.y += Random.Range(1.5f, 3f);
			if (!num)
			{
				force.z += Random.Range(-7.5f, 7.5f);
				component.AddForce(force, ForceMode.VelocityChange);
			}
			else
			{
				component.AddForceAtPosition(force, PM.transform.position + new Vector3(0f - PM.Base.CurSpeed, 0f, 0f), ForceMode.VelocityChange);
			}
		}
		else
		{
			Trailer component2 = Object.Instantiate(ShootObjects[Params.ShootIndex], base.transform.position + base.transform.up * PosY * 2f, Quaternion.identity).GetComponent<Trailer>();
			component2.Player = PM.Base;
			component2.Tornado = this;
			component2.GetComponent<Rigidbody>().AddForce(-Vector3.right * 170.05f, ForceMode.VelocityChange);
		}
	}

	public void OnDissipate()
	{
		Dissipate = true;
		for (int i = 0; i < FX.Length; i++)
		{
			ParticleSystem.EmissionModule emission = FX[i].emission;
			emission.enabled = false;
		}
		for (int j = 0; j < CarryingObjects.Length; j++)
		{
			CarryingObjects[j].transform.SetParent(null);
			CarryingObjects[j].transform.GetComponentInChildren<MeshCollider>().enabled = true;
			CarryingObjects[j].isKinematic = false;
			CarryingObjects[j].collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			CarryingObjects[j].AddForce((CarryingObjects[j].position - base.transform.position).normalized * 25f, ForceMode.VelocityChange);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawLine(base.transform.position, base.transform.position + Camera);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position + Camera, 1f);
	}
}
