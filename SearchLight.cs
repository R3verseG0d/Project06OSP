using STHEngine;
using UnityEngine;

public class SearchLight : EventStation
{
	[Header("Framework")]
	public float PitchAngle;

	public float PitchRange;

	public float PitchSpeed;

	public float PitchTime;

	public float YawAngle;

	public float YawRange;

	public float YawSpeed;

	public float YawTime;

	public float FindTime;

	public float LoseLength;

	public string OnIntersect;

	public string BrokenHead;

	public bool AllBrk;

	[Header("Prefab")]
	public Transform HeadPoint;

	public Transform LightPoint;

	public Renderer PillarRenderer;

	public GameObject[] HeadModels;

	public Transform BrkGlassPoint;

	public GameObject BrokenGlass;

	public GameObject BrokenPrefab;

	public AudioSource Audio;

	internal bool DestroyedHead;

	private MaterialPropertyBlock PropBlock;

	private GameObject Target;

	private bool FoundTarget;

	private bool FoundFirst;

	private bool HeadMoving;

	private bool HeadReverse;

	private bool LightMoving;

	private bool LightReverse;

	private bool Destroyed;

	private float FindWaitTime;

	private float HeadAngle;

	private float LightAngle;

	private float HeadAnglePos;

	private float HeadAngleNeg;

	private float LightAnglePos;

	private float LightAngleNeg;

	private float HeadWaitTime;

	private float LightWaitTime;

	public void SetParameters(float _PitchAngle, float _PitchRange, float _PitchSpeed, float _PitchTime, float _YawAngle, float _YawRange, float _YawSpeed, float _YawTime, float _FindTime, float _LoseLength, string _OnIntersect, string _BrokenHead, bool _AllBrk)
	{
		PitchAngle = _PitchAngle;
		PitchRange = _PitchRange;
		PitchSpeed = _PitchSpeed;
		PitchTime = _PitchTime;
		YawAngle = _YawAngle;
		YawRange = _YawRange;
		YawSpeed = _YawSpeed;
		YawTime = _YawTime;
		FindTime = _FindTime;
		LoseLength = _LoseLength;
		OnIntersect = _OnIntersect;
		BrokenHead = _BrokenHead;
		AllBrk = _AllBrk;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		HeadMoving = true;
		LightMoving = true;
		HeadAngle = YawAngle;
		LightAngle = PitchAngle;
		HeadPoint.localRotation = Quaternion.Euler(0f, HeadAngle, 0f);
		LightPoint.localRotation = Quaternion.Euler(LightAngle, 0f, 0f);
		HeadAnglePos = HeadAngle + YawRange;
		HeadAngleNeg = HeadAngle - YawRange;
		LightAnglePos = LightAngle + PitchRange;
		LightAngleNeg = LightAngle - PitchRange;
		HeadWaitTime = Time.time;
		LightWaitTime = Time.time;
	}

	private void Update()
	{
		if (!DestroyedHead)
		{
			if ((bool)Target && FoundTarget)
			{
				Vector3 normalized = (Target.transform.position - LightPoint.position).normalized;
				HeadPoint.rotation = Quaternion.RotateTowards(HeadPoint.rotation, Quaternion.LookRotation(normalized.MakePlanar()), Time.deltaTime * 15f);
				LightPoint.rotation = Quaternion.RotateTowards(LightPoint.rotation, Quaternion.LookRotation(normalized), Time.deltaTime * 15f);
				if (Vector3.Distance(Target.transform.position, LightPoint.position) > LoseLength * 5f)
				{
					HeadMoving = true;
					LightMoving = true;
					Target = null;
					FoundTarget = false;
				}
			}
			else
			{
				if (!HeadMoving)
				{
					if (Time.time - HeadWaitTime > YawTime)
					{
						HeadMoving = true;
					}
				}
				else
				{
					if (HeadAngle == HeadAnglePos)
					{
						HeadWaitTime = Time.time;
						HeadReverse = true;
						HeadMoving = false;
					}
					else
					{
						HeadAngle = Mathf.MoveTowards(HeadAngle, HeadReverse ? HeadAngleNeg : HeadAnglePos, Time.deltaTime * YawSpeed * 0.5f);
					}
					if (HeadAngle == HeadAngleNeg)
					{
						HeadWaitTime = Time.time;
						HeadReverse = false;
						HeadMoving = false;
					}
					else
					{
						HeadAngle = Mathf.MoveTowards(HeadAngle, (!HeadReverse) ? HeadAnglePos : HeadAngleNeg, Time.deltaTime * YawSpeed * 0.5f);
					}
					HeadPoint.localRotation = Quaternion.Euler(0f, HeadAngle, 0f);
				}
				if (!LightMoving)
				{
					if (Time.time - LightWaitTime > PitchTime)
					{
						LightMoving = true;
					}
				}
				else
				{
					if (LightAngle == LightAnglePos)
					{
						LightWaitTime = Time.time;
						LightReverse = true;
						LightMoving = false;
					}
					else
					{
						LightAngle = Mathf.MoveTowards(LightAngle, LightReverse ? LightAngleNeg : LightAnglePos, Time.deltaTime * PitchSpeed * 0.5f);
					}
					if (LightAngle == LightAngleNeg)
					{
						LightWaitTime = Time.time;
						LightReverse = false;
						LightMoving = false;
					}
					else
					{
						LightAngle = Mathf.MoveTowards(LightAngle, (!LightReverse) ? LightAnglePos : LightAngleNeg, Time.deltaTime * PitchSpeed * 0.5f);
					}
					LightPoint.localRotation = Quaternion.Euler(LightAngle, 0f, 0f);
				}
			}
		}
		PillarRenderer.GetPropertyBlock(PropBlock, 1);
		PropBlock.SetFloat("_Intensity", DestroyedHead ? Mathf.Lerp(PropBlock.GetFloat("_Intensity"), 0f, Time.deltaTime) : ((!FoundTarget) ? 1f : Mathf.PingPong(Time.time * 18f, 3f)));
		PillarRenderer.SetPropertyBlock(PropBlock, 1);
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!DestroyedHead)
		{
			if ((bool)HitInfo.player && HitInfo.player.tag == "Player")
			{
				HitInfo.player.GetComponent<PlayerBase>().AddScore(1000);
			}
			HeadModels[0].SetActive(value: false);
			HeadModels[1].SetActive(value: true);
			if (Audio.isPlaying)
			{
				Audio.Stop();
			}
			GameObject gameObject = Object.Instantiate(BrokenGlass, BrkGlassPoint.position, BrkGlassPoint.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", new HitInfo(HitInfo.player, BrkGlassPoint.forward * 2.5f), SendMessageOptions.DontRequireReceiver);
			CallEvent(BrokenHead);
			Target = null;
			DestroyedHead = true;
		}
	}

	public void DestroySearchlight()
	{
		if (!Destroyed)
		{
			Object.Instantiate(BrokenPrefab, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
			Destroyed = true;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if ((bool)GetPlayer(collider) && !DestroyedHead && !FoundTarget)
		{
			FindWaitTime = Time.time;
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !DestroyedHead && !FoundTarget && !FoundTarget && Time.time - FindWaitTime > FindTime)
		{
			if (!FoundFirst)
			{
				Audio.Play();
				CallEvent(OnIntersect);
				FoundFirst = true;
			}
			Target = player.gameObject;
			FoundTarget = true;
		}
	}
}
