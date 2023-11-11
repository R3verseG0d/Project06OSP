using System;
using UnityEngine;

public class UpDownReel : ObjectBase
{
	[Header("Framework")]
	public float Height;

	public float Height2;

	public float Time;

	[Header("Prefab")]
	public Renderer Rope;

	public Transform Handle;

	public Transform HandleModel;

	public Transform HomingTarget;

	public Animation Animation;

	public AudioSource Audio;

	public AudioSource AudioLoop;

	private PlayerManager PM;

	private MaterialPropertyBlock PropBlock;

	private Vector3 TopPos;

	private Vector3 BottomPos;

	private bool Raise;

	private bool ReachedApex;

	private float CurLerpTime;

	public void SetParameters(float _Height, float _Height2, float _Time)
	{
		Height = _Height;
		Height2 = _Height2;
		Time = _Time;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		TopPos = base.transform.position + base.transform.up * (Height2 + 1f);
		BottomPos = base.transform.position + base.transform.up * (Height + 1f);
		Handle.position = BottomPos;
		CurLerpTime = 1f;
	}

	private void StateUpReelStart()
	{
		PM.Base.SetState("UpReel");
		PM.transform.SetParent(HandleModel);
		PM.RBody.isKinematic = true;
		PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		PM.RBody.velocity = Vector3.zero;
		PM.transform.localPosition = new Vector3(0f, -0.75f, 0f);
		PM.transform.localRotation = Quaternion.identity;
		PM.RBody.velocity = Vector3.zero;
	}

	private void StateUpReel()
	{
		PM.Base.SetState("UpReel");
		PM.Base.PlayAnimation("Up Reel", "On Up Reel");
		PM.Base.LockControls = true;
		PM.Base.GeneralMeshRotation = PM.transform.rotation;
		PM.Base.CurSpeed = 0f;
	}

	private void StateUpReelExit()
	{
	}

	private void Update()
	{
		Rope.transform.localScale = new Vector3(Rope.transform.localScale.x, Rope.transform.localScale.y, Vector3.Distance(base.transform.position, HandleModel.position + HandleModel.up * 1f) * 6.35f);
		Rope.GetPropertyBlock(PropBlock);
		PropBlock.SetFloat("_Intensity", Mathf.Lerp(0.75f, 1f, Mathf.Abs(Mathf.Cos(UnityEngine.Time.time * 10f))));
		Rope.SetPropertyBlock(PropBlock);
		if ((bool)PM && Singleton<GameManager>.Instance.GameState != GameManager.State.Paused && PM.Base.GetState() == "UpReel" && Singleton<RInput>.Instance.P.GetButtonDown("Button A"))
		{
			PM.Base.SetMachineState("StateJump");
			OnUpReelExit(PM.Base);
		}
	}

	private void FixedUpdate()
	{
		CurLerpTime += UnityEngine.Time.fixedDeltaTime;
		if (CurLerpTime > 1f)
		{
			CurLerpTime = 1f;
		}
		float num = CurLerpTime / Time;
		num = 1f - Mathf.Cos(num * (float)Math.PI * 0.5f);
		if (Raise)
		{
			AudioLoop.mute = ReachedApex;
			Handle.position = Vector3.MoveTowards(Handle.position, TopPos, num);
			if (Handle.position == TopPos && !ReachedApex)
			{
				Animation.Play();
				Audio.Play();
				ReachedApex = true;
			}
			if ((bool)PM && PM.Base.GetState() != "UpReel" && CurLerpTime > 0.1f)
			{
				OnUpReelExit(PM.Base);
			}
		}
		else
		{
			AudioLoop.mute = Handle.position == BottomPos;
			Handle.position = Vector3.MoveTowards(Handle.position, BottomPos, num);
			if (Handle.position == BottomPos && HomingTarget.tag == "Untagged")
			{
				HomingTarget.tag = "HomingTarget";
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !player.IsDead && !(player.GetState() == "Vehicle") && !player.GetPrefab("omega") && !player.GetPrefab("rouge"))
		{
			PM = collider.GetComponent<PlayerManager>();
			if (PM.Base.GetState() != "UpReel")
			{
				PM.Base.StateMachine.ChangeState(base.gameObject, StateUpReel);
				HomingTarget.tag = "Untagged";
				Raise = true;
				CurLerpTime = 0f;
			}
		}
	}

	private void OnUpReelExit(PlayerBase Player)
	{
		if (!(Player != PM.Base))
		{
			PM.RBody.isKinematic = false;
			PM.RBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			PM.Base.transform.SetParent(null);
			PM = null;
			HomingTarget.tag = "HomingTarget";
			Raise = false;
			ReachedApex = false;
			CurLerpTime = 0f;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position + base.transform.up * Height2, 0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position + base.transform.up * Height, 0.5f);
	}
}
