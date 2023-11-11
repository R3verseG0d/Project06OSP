using System.Collections;
using UnityEngine;

public class Pod : MonoBehaviour
{
	[Header("Framework")]
	public Rigidbody RigidBody;

	public Renderer Renderer;

	public StateMachine StateMachine;

	public Transform Mesh;

	public Transform BulletMuzzle;

	public GameObject HomingTarget;

	public GameObject PodBeamPrefab;

	public GameObject VulcanBulletPrefab;

	public GameObject ParalysisEffectPrefab;

	public GameObject BonusObject;

	public GameObject ExplosionFX;

	internal Transform Pivot;

	private LaserBeam PodBeam;

	private GameObject ParalysisEffect;

	private float StateTime;

	private bool ActivateLaser;

	private bool IsHeightLaser;

	private Transform HomingLaserTarget;

	private Transform VulcanTarget;

	private float VulcanTimer;

	private int AttackCount;

	private void Start()
	{
		PodBeam = Object.Instantiate(PodBeamPrefab, base.transform.position, base.transform.rotation, base.transform).GetComponent<LaserBeam>();
		StateDeployStart();
		StateMachine.Initialize(StateDeploy);
	}

	private void FixedUpdate()
	{
		StateMachine.UpdateStateMachine();
	}

	private void Update()
	{
		Mesh.Rotate(0f, 0f, 180f * Time.deltaTime);
	}

	private void StateDeployStart()
	{
		StateTime = Time.time;
	}

	private void StateDeploy()
	{
		if (Time.time - StateTime > 1f)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateDeployEnd()
	{
	}

	private void StateWaitStart()
	{
	}

	private void StateWait()
	{
		if ((bool)Pivot)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position, Time.deltaTime * 8f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Pivot.rotation, Time.deltaTime * 25f);
		}
	}

	private void StateWaitEnd()
	{
	}

	private void StateLaserStart()
	{
		StateTime = Time.time;
		ActivateLaser = false;
	}

	private void StateLaser()
	{
		if (Time.time - StateTime > 1f && !ActivateLaser)
		{
			PodBeam.State = 0;
			ActivateLaser = true;
		}
		if (ActivateLaser)
		{
			RaycastHit hitInfo;
			Vector3 targetPos = ((!Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, 65f)) ? (base.transform.position + base.transform.forward * 65f) : hitInfo.point);
			PodBeam.transform.position = base.transform.position;
			PodBeam.UpdateBeam(targetPos);
		}
		if ((bool)Pivot)
		{
			if (!IsHeightLaser)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position + Pivot.forward * 1f + Pivot.up * 1f, Time.deltaTime * 6f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(-Vector3.up), Time.deltaTime * 25f);
			}
			else
			{
				base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position - Pivot.forward * 0.75f - Vector3.up * 1.25f, Time.deltaTime * 6f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Pivot.forward.MakePlanar()), Time.deltaTime * 25f);
			}
		}
	}

	private void StateLaserEnd()
	{
		PodBeam.State = 2;
	}

	private void StateHomingLaserStart()
	{
		StateTime = Time.time;
		ActivateLaser = false;
	}

	private void StateHomingLaser()
	{
		if (Time.time - StateTime > 1f && !ActivateLaser)
		{
			ActivateLaser = true;
			PodBeam.State = 0;
		}
		if (ActivateLaser)
		{
			RaycastHit hitInfo;
			Vector3 targetPos = ((!Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, 65f)) ? (base.transform.position + base.transform.forward * 65f) : hitInfo.point);
			PodBeam.transform.position = base.transform.position;
			PodBeam.UpdateBeam(targetPos);
		}
		if ((bool)Pivot)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position - Vector3.up * 1f, Time.deltaTime * 6f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((HomingLaserTarget.position - base.transform.position).normalized), Time.deltaTime * 25f);
		}
		if (Vector3.Distance(base.transform.position, HomingLaserTarget.position) > 30f)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateHomingLaserEnd()
	{
		PodBeam.State = 2;
		HomingLaserTarget = null;
	}

	private void StateVulcanStart()
	{
		StateTime = Time.time;
		VulcanTimer = Time.time + 1f;
		AttackCount = 0;
	}

	private void StateVulcan()
	{
		if (Time.time > VulcanTimer + (float)AttackCount * 0.125f && (float)AttackCount < 16f)
		{
			Object.Instantiate(VulcanBulletPrefab, BulletMuzzle.position, Quaternion.LookRotation((VulcanTarget.position + VulcanTarget.up * 0.5f - BulletMuzzle.position).normalized));
			AttackCount++;
		}
		if ((bool)Pivot)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position + Pivot.forward * 1f + Pivot.up * 1f, Time.deltaTime * 6f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation((VulcanTarget.position - base.transform.position).normalized), Time.deltaTime * 25f);
		}
		float num = Vector3.Distance(base.transform.position, VulcanTarget.position);
		if ((float)AttackCount >= 16f || num > 30f)
		{
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateVulcanEnd()
	{
		VulcanTarget = null;
	}

	private void StateStunedStart()
	{
		StateTime = Time.time;
	}

	private void StateStuned()
	{
		if ((bool)Pivot)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, Pivot.position, Time.deltaTime * 8f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Pivot.rotation, Time.deltaTime * 25f);
		}
		if (Time.time - StateTime > 7.5f)
		{
			Object.Destroy(ParalysisEffect);
			StateMachine.ChangeState(StateWait);
		}
	}

	private void StateStunedEnd()
	{
	}

	private void ShootLaser(bool Toggle)
	{
		IsHeightLaser = Toggle;
		StateMachine.ChangeState(StateLaser);
	}

	private void ShootHomingLaser(Transform Target)
	{
		HomingLaserTarget = Target;
		StateMachine.ChangeState(StateHomingLaser);
	}

	private void StopLaser()
	{
		StateMachine.ChangeState(StateWait);
	}

	private void ShootVulcan(Transform Target)
	{
		VulcanTarget = Target;
		StateMachine.ChangeState(StateVulcan);
	}

	private void OnFlash()
	{
		if (!ParalysisEffect)
		{
			ParalysisEffect = Object.Instantiate(ParalysisEffectPrefab, base.transform.position, Quaternion.identity);
		}
		ParalysisEffect.transform.SetParent(base.transform);
		StateMachine.ChangeState(StateStuned);
	}

	private void Destroy()
	{
		Object.Destroy(PodBeam.gameObject);
		Object.Destroy(base.gameObject);
	}

	private void OnDestroyPod()
	{
		base.transform.SetParent(null);
		Object.Destroy(HomingTarget);
		StartCoroutine(DestroyTimed(Random.Range(0.05f, 0.5f)));
	}

	private void OnHit(HitInfo HitInfo)
	{
		Object.Instantiate(ExplosionFX, base.transform.position, base.transform.rotation);
		Object.Instantiate(BonusObject, base.transform.position, Random.rotation);
		Destroy();
	}

	private void OnExplosion(HitInfo HitInfo)
	{
		OnHit(HitInfo);
	}

	private IEnumerator DestroyTimed(float Timer)
	{
		yield return new WaitForSeconds(Timer);
		OnHit(new HitInfo(base.transform, Vector3.zero));
	}
}
