using System.Collections.Generic;
using UnityEngine;

public class ESPSwing : PsiObject
{
	[Header("Framework")]
	public float Length;

	[Header("Prefab")]
	public LayerMask AttackMask;

	public Common_PsiMarkSphere ESPMark;

	public Renderer[] Renderers;

	public AudioSource[] Audio;

	public TubeRenderer[] TubeRenderers;

	public Transform[] RopePoints;

	public Transform TopPivot;

	public Transform OriginPoint;

	public Transform CamPos;

	public Transform CamPosReverse;

	public Transform CamTgt;

	public Transform CamTgtReverse;

	private List<CRSpline> Splines;

	private CameraParameters CameraParams;

	private AnimationCurve Movement;

	private AnimationCurve RotCharge;

	private PlayerBase Player;

	private Vector3 Center;

	private bool IsLifting;

	private bool IsReverse;

	private float Progress;

	private float RotFactor;

	private float RotDecay;

	private float HitTimer;

	private float[] TheseLengths;

	public void SetParameters(float _Length)
	{
		Length = _Length;
	}

	private void Start()
	{
		ESPMark.Target = base.gameObject;
		TopPivot.localPosition = new Vector3(0f, Length, 0f);
		OriginPoint.localPosition = new Vector3(0f, 0f - Length, 0f);
		CameraParams = new CameraParameters(30, Vector3.zero, Vector3.zero, CamPos, CamTgt);
		Progress = 0f;
		RotFactor = 0f;
		Movement = new AnimationCurve();
		Keyframe[] array = new Keyframe[5];
		array[0].time = 0f;
		array[0].value = 0f;
		array[1].time = 0.5f;
		array[1].value = 80.5f;
		array[2].time = 1f;
		array[2].value = 0f;
		array[3].time = 1.5f;
		array[3].value = -80.5f;
		array[4].time = 2f;
		array[4].value = 0f;
		Movement.keys = array;
		for (int i = 0; i < Movement.keys.Length; i++)
		{
			Movement.SmoothTangents(i, 0f);
		}
		Movement.preWrapMode = WrapMode.Loop;
		Movement.postWrapMode = WrapMode.Loop;
		RotCharge = new AnimationCurve();
		Keyframe[] array2 = new Keyframe[5];
		array2[0].time = 0f;
		array2[0].value = 0f;
		array2[1].time = 0.5f;
		array2[1].value = 1f;
		array2[2].time = 1f;
		array2[2].value = 0f;
		array2[3].time = 1.5f;
		array2[3].value = 1f;
		array2[4].time = 2f;
		array2[4].value = 0f;
		RotCharge.keys = array2;
		for (int j = 0; j < RotCharge.keys.Length; j++)
		{
			RotCharge.SmoothTangents(j, 0f);
		}
		SetupSpline();
	}

	private void SetupSpline()
	{
		Splines = new List<CRSpline>();
		for (int i = 0; i < TubeRenderers.Length; i++)
		{
			CRSpline cRSpline = new CRSpline();
			List<Vector3> knots = cRSpline.knots;
			knots.Add(TopPivot.position);
			knots.Add(TopPivot.position);
			Center = RopePoints[i].position + (TopPivot.position - RopePoints[i].position).normalized * (Vector3.Distance(TopPivot.position, RopePoints[i].position) / 2f) + (base.transform.position - RopePoints[i].position).normalized * 0.5f;
			knots.Add(Center);
			knots.Add(RopePoints[i].position);
			knots.Add(RopePoints[i].position);
			Splines.Add(cRSpline);
		}
		TheseLengths = new float[TubeRenderers.Length];
		for (int j = 0; j < TheseLengths.Length; j++)
		{
			TheseLengths[j] = Vector3.Distance(TopPivot.position, Center) + Vector3.Distance(Center, RopePoints[j].position);
		}
	}

	private void FixedUpdate()
	{
		TopPivot.localRotation = Quaternion.Euler(Movement.Evaluate(Progress) * RotFactor, 0f, 0f);
		OriginPoint.localRotation = Quaternion.Euler((0f - Movement.Evaluate(Progress)) * RotFactor, 0f, 0f);
		if (IsLifting)
		{
			Progress = Mathf.Lerp(Progress, (!IsReverse) ? 0.5f : 1.5f, Time.fixedDeltaTime * 0.75f);
			RotFactor = RotCharge.Evaluate(Progress);
			RotDecay = 2.75f;
		}
		else
		{
			if (!IsReverse)
			{
				Progress += Time.fixedDeltaTime;
				if (Progress >= 2f)
				{
					Progress = 0f;
				}
			}
			else
			{
				Progress -= Time.fixedDeltaTime;
				if (Progress <= 0f)
				{
					Progress = 2f;
				}
			}
			RotDecay = Mathf.Lerp(RotDecay, 2.25f, Time.fixedDeltaTime);
			RotFactor = Mathf.Lerp(RotFactor, 0f, Time.fixedDeltaTime / RotDecay);
			HitTimer += Time.fixedDeltaTime;
			if (RotFactor > 0.1f && HitTimer > 0.5f && (AttackSphere(OriginPoint.position + OriginPoint.forward * 7.5f, OriginPoint.forward * 25f + OriginPoint.up * 2f) || AttackSphere(OriginPoint.position + OriginPoint.forward * -7.5f, OriginPoint.forward * -25f + OriginPoint.up * 2f)))
			{
				HitTimer = 0f;
				Audio[1].Play();
			}
		}
		for (int i = 0; i < Renderers.Length; i++)
		{
			OnPsiFX(Renderers[i], IsLifting);
		}
		UpdateMesh();
		for (int j = 0; j < Splines.Count; j++)
		{
			Splines[j].knots[2] = RopePoints[j].position + (TopPivot.position - RopePoints[j].position).normalized * (Vector3.Distance(TopPivot.position, RopePoints[j].position) / 2f) + (base.transform.position - RopePoints[j].position).normalized * 0.5f;
			Splines[j].knots[Splines[j].knots.Count - 1] = RopePoints[j].position;
			Splines[j].knots[Splines[j].knots.Count - 2] = RopePoints[j].position;
		}
	}

	private void Update()
	{
		for (int i = 0; i < Renderers.Length; i++)
		{
			OnPsiFX(Renderers[i], IsLifting);
		}
	}

	private void UpdateMesh()
	{
		if (TubeRenderers == null)
		{
			return;
		}
		for (int i = 0; i < TubeRenderers.Length; i++)
		{
			if (TheseLengths[i] == 0f)
			{
				Vector3 vector = RopePoints[i].position + (TopPivot.position - RopePoints[i].position).normalized * (Vector3.Distance(TopPivot.position, RopePoints[i].position) / 2f) + (base.transform.position - RopePoints[i].position).normalized * 0.5f;
				TheseLengths[i] = Vector3.Distance(TopPivot.position, vector) + Vector3.Distance(vector, RopePoints[i].position);
			}
			int num = (int)TheseLengths[i] * 2;
			float r = 0.0525f;
			TubeRenderers[i].crossSegments = 6;
			TubeRenderers[i].vertices = new TubeVertex[num + 1];
			TubeRenderers[i].uvLength = TheseLengths[i] * 2f;
			for (int j = 0; j < num + 1; j++)
			{
				float t = (float)j / ((float)num * 1f);
				Vector3 position = Splines[i].GetPosition(t);
				Vector3 tangent = Splines[i].GetTangent(t);
				Vector3 pt = base.transform.InverseTransformPoint(position);
				Vector3 vector2 = base.transform.InverseTransformDirection(tangent);
				Color c = Color.white;
				if (j == 0)
				{
					c = Color.black;
				}
				TubeRenderers[i].vertices[j] = new TubeVertex(pt, r, c);
				if (vector2 != Vector3.zero)
				{
					TubeRenderers[i].vertices[j].rotation = Quaternion.LookRotation(vector2);
				}
			}
		}
	}

	private void OnEventSignal()
	{
		if (!IsLifting && (bool)Player)
		{
			if (Vector3.Dot(base.transform.forward, Player.transform.forward) < 0f && !IsReverse)
			{
				Player.Camera.UncancelableEvent = false;
				CameraParams = new CameraParameters(30, Vector3.zero, Vector3.zero, CamPosReverse, CamTgtReverse);
				IsReverse = true;
			}
			else if (Vector3.Dot(base.transform.forward, Player.transform.forward) > 0f && IsReverse)
			{
				Player.Camera.UncancelableEvent = false;
				CameraParams = new CameraParameters(30, Vector3.zero, Vector3.zero, CamPos, CamTgt);
				IsReverse = false;
			}
			Player.SetCameraParams(CameraParams);
			Player.Camera.UncancelableEvent = true;
			Progress = 1f;
			IsLifting = true;
			Audio[0].Play();
		}
	}

	private void OnESPRelease()
	{
		IsLifting = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player" && !Player)
		{
			Player = collision.gameObject.GetComponent<PlayerBase>();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Player" && (bool)Player)
		{
			Player.Camera.UncancelableEvent = false;
			Player.DestroyCameraParams(CameraParams);
			Player = null;
		}
	}

	public bool AttackSphere(Vector3 Position, Vector3 Force)
	{
		HitInfo value = new HitInfo(base.transform, Force, 10);
		Collider[] array = Physics.OverlapSphere(Position, 3f, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + Vector3.up * Length);
		Gizmos.DrawWireSphere(base.transform.position + Vector3.up * Length, 1f);
	}
}
