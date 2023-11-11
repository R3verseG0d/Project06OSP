using UnityEngine;

public class Seesaw : MonoBehaviour
{
	[Header("Framework")]
	public float Height;

	public float Speed;

	public float RetSpeed;

	public float Time;

	public float MaxLen;

	public float Difference;

	[Header("Prefab")]
	public GameObject PlatformObj;

	public Transform[] SpawnPoints;

	public Transform[] Platforms;

	public Animator[] Animators;

	public LineRenderer[] Ropes;

	public AudioSource[] AudioPoints;

	public AudioClip[] ClipPool;

	internal bool StarCollapse1;

	internal bool StarCollapse2;

	internal bool LowerPlatform1;

	internal bool LowerPlatform2;

	private ScalePlatform Platform1;

	private ScalePlatform Platform2;

	private bool Rebalance;

	private float HeightPlatform1;

	private float HeightPlatform2;

	private float StartTime;

	public void SetParameters(float _Height, float _Speed, float _RetSpeed, float _Time, float _MaxLen, float _Difference)
	{
		Height = _Height;
		Speed = _Speed;
		RetSpeed = _RetSpeed;
		Time = _Time;
		MaxLen = _MaxLen;
		Difference = _Difference;
	}

	private void Start()
	{
		SpawnPlatforms();
	}

	private void Update()
	{
		Vector3 localPosition = Platforms[0].localPosition;
		Vector3 localPosition2 = Platforms[1].localPosition;
		if (LowerPlatform1 && !Rebalance)
		{
			HeightPlatform1 = Mathf.MoveTowards(HeightPlatform1, 0f - MaxLen, UnityEngine.Time.deltaTime * Speed);
			HeightPlatform2 = Mathf.MoveTowards(HeightPlatform2, MaxLen, UnityEngine.Time.deltaTime * Speed);
			if (HeightPlatform1 == 0f - MaxLen && !StarCollapse1)
			{
				StarCollapse1 = true;
				StartTime = UnityEngine.Time.time;
				Animators[0].SetTrigger("On Shake");
			}
			if (StarCollapse1 && UnityEngine.Time.time - StartTime > Time)
			{
				Rebalance = true;
				StarCollapse1 = false;
				Animators[0].SetTrigger("On Release");
				PlayAudio(0, 1);
				Platform1.Collapse();
				Platform1 = null;
			}
		}
		else if (LowerPlatform2 && !Rebalance)
		{
			HeightPlatform1 = Mathf.MoveTowards(HeightPlatform1, MaxLen, UnityEngine.Time.deltaTime * Speed);
			HeightPlatform2 = Mathf.MoveTowards(HeightPlatform2, 0f - MaxLen, UnityEngine.Time.deltaTime * Speed);
			if (HeightPlatform2 == 0f - MaxLen && !StarCollapse2)
			{
				StarCollapse2 = true;
				StartTime = UnityEngine.Time.time;
				Animators[1].SetTrigger("On Shake");
			}
			if (StarCollapse2 && UnityEngine.Time.time - StartTime > Time)
			{
				Rebalance = true;
				StarCollapse2 = false;
				Animators[1].SetTrigger("On Release");
				PlayAudio(1, 1);
				Platform2.Collapse();
				Platform2 = null;
			}
		}
		else
		{
			HeightPlatform1 = Mathf.MoveTowards(HeightPlatform1, 0f, UnityEngine.Time.deltaTime * RetSpeed);
			HeightPlatform2 = Mathf.MoveTowards(HeightPlatform2, 0f, UnityEngine.Time.deltaTime * RetSpeed);
			if (HeightPlatform1 == 0f && HeightPlatform2 == 0f && Rebalance)
			{
				Rebalance = false;
				Animators[0].SetTrigger("On Reset");
				Animators[1].SetTrigger("On Reset");
				SpawnPlatforms();
			}
			if (!Rebalance)
			{
				Animators[0].SetTrigger("On Reset");
				Animators[1].SetTrigger("On Reset");
			}
			StarCollapse1 = false;
			StarCollapse2 = false;
		}
		localPosition.y = HeightPlatform1 - Difference - Height;
		localPosition2.y = HeightPlatform2 + Difference - Height;
		Platforms[0].localPosition = localPosition;
		Platforms[1].localPosition = localPosition2;
		Ropes[0].SetPosition(1, Platforms[0].localPosition);
		Ropes[1].SetPosition(1, Platforms[1].localPosition);
	}

	private void SpawnPlatforms()
	{
		if (!Platform1)
		{
			Platform1 = Object.Instantiate(PlatformObj, SpawnPoints[0].position, SpawnPoints[0].rotation).GetComponent<ScalePlatform>();
			Platform1.Seesaw = this;
			Platform1.FirstScale = true;
			Platform1.transform.SetParent(SpawnPoints[0]);
		}
		if (!Platform2)
		{
			Platform2 = Object.Instantiate(PlatformObj, SpawnPoints[1].position, SpawnPoints[1].rotation).GetComponent<ScalePlatform>();
			Platform2.Seesaw = this;
			Platform2.SecondScale = true;
			Platform2.transform.SetParent(SpawnPoints[1]);
		}
	}

	public void PlayAudio(int AudioIndex, int ClipIndex)
	{
		AudioPoints[AudioIndex].PlayOneShot(ClipPool[ClipIndex], 1f);
	}
}
