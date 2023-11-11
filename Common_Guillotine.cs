using UnityEngine;

public class Common_Guillotine : MonoBehaviour
{
	public enum Type
	{
		Static = 0,
		Normal = 1,
		Distance = 2
	}

	[Header("Framework")]
	public int Length;

	public Type Mode;

	public float UpWidth;

	public float DownWidth;

	public float Time;

	public float Speed;

	public float Distance;

	[Header("Prefab")]
	public AudioSource Audio;

	public GameObject[] Guillotines;

	public GameObject Point;

	private Vector3 NeutralPosition;

	private Vector3 PositivePosition;

	private Vector3 NegativePosition;

	private Vector3 DistanceNegativePosition;

	private Vector3 PlayerPoint;

	private Transform Target;

	private bool IsMoving;

	private bool Reverse;

	private bool InRange;

	private float WaitTime;

	public void SetParameters(int _Length, float _Mode, float _UpWidth, float _DownWidth, float _Time, float _Speed, float _Distance)
	{
		Length = _Length;
		Mode = (Type)((int)_Mode - 1);
		UpWidth = _UpWidth;
		DownWidth = _DownWidth;
		Time = _Time;
		Speed = _Speed;
		Distance = _Distance;
	}

	private void Start()
	{
		Guillotines[Length - 1].SetActive(value: true);
		NeutralPosition = base.transform.position;
		PositivePosition = new Vector3(base.transform.position.x, base.transform.position.y + UpWidth, base.transform.position.z);
		NegativePosition = new Vector3(base.transform.position.x, base.transform.position.y - UpWidth, base.transform.position.z);
		DistanceNegativePosition = new Vector3(base.transform.position.x, base.transform.position.y - DownWidth, base.transform.position.z);
		WaitTime = UnityEngine.Time.time;
		if (Mode == Type.Static)
		{
			Audio.mute = true;
		}
	}

	private void Update()
	{
		switch (Mode)
		{
		case Type.Normal:
		{
			for (int j = 0; j < Guillotines.Length; j++)
			{
				if (IsMoving)
				{
					Guillotines[j].transform.Rotate(720f * (Reverse ? 1f : (-1f)) * UnityEngine.Time.deltaTime, 0f, 0f);
				}
			}
			if (!IsMoving)
			{
				if (UnityEngine.Time.time - WaitTime > Time)
				{
					Audio.mute = false;
					IsMoving = true;
				}
				break;
			}
			if (Point.transform.position == PositivePosition)
			{
				WaitTime = UnityEngine.Time.time;
				Reverse = true;
				Audio.mute = true;
				IsMoving = false;
			}
			else
			{
				Point.transform.position = Vector3.MoveTowards(Point.transform.position, Reverse ? NegativePosition : PositivePosition, UnityEngine.Time.deltaTime * (Speed * 0.5f));
			}
			if (Point.transform.position == NegativePosition)
			{
				WaitTime = UnityEngine.Time.time;
				Reverse = false;
				Audio.mute = true;
				IsMoving = false;
			}
			else
			{
				Point.transform.position = Vector3.MoveTowards(Point.transform.position, (!Reverse) ? PositivePosition : NegativePosition, UnityEngine.Time.deltaTime * (Speed * 0.5f));
			}
			break;
		}
		case Type.Distance:
		{
			if (!Target)
			{
				Target = GameObject.FindGameObjectWithTag("Player").transform;
			}
			PlayerPoint = Point.transform.InverseTransformPoint(Target.position);
			InRange = Vector3.Distance(base.transform.position + base.transform.up * (0f - UpWidth), Target.position) < Distance * 0.5f;
			if (InRange && Point.transform.position.y != PlayerPoint.y)
			{
				if (Point.transform.position.y < PositivePosition.y && Point.transform.position.y > DistanceNegativePosition.y)
				{
					Point.transform.position = Vector3.MoveTowards(Point.transform.position, new Vector3(base.transform.position.x, Target.position.y, base.transform.position.z), UnityEngine.Time.deltaTime * Speed);
				}
			}
			else
			{
				Point.transform.position = Vector3.MoveTowards(Point.transform.position, NeutralPosition, UnityEngine.Time.deltaTime * Speed);
			}
			for (int i = 0; i < Guillotines.Length; i++)
			{
				if (InRange)
				{
					if (Point.transform.position.y < PositivePosition.y && Point.transform.position.y > DistanceNegativePosition.y && PlayerPoint.y != 0f)
					{
						Guillotines[i].transform.Rotate(720f * ((PlayerPoint.y > 0f) ? (-1f) : 1f) * UnityEngine.Time.deltaTime, 0f, 0f);
						Audio.mute = false;
					}
					else
					{
						Audio.mute = true;
					}
				}
				else if (Point.transform.position != NeutralPosition)
				{
					Guillotines[i].transform.Rotate(720f * ((Point.transform.position.y < NeutralPosition.y) ? (-1f) : 1f) * UnityEngine.Time.deltaTime, 0f, 0f);
					Audio.mute = false;
				}
				else
				{
					Audio.mute = true;
				}
			}
			break;
		}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * UpWidth);
		Gizmos.DrawWireSphere(base.transform.position + base.transform.up * UpWidth, 0.25f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.up * ((Mode == Type.Normal) ? (0f - UpWidth) : (0f - DownWidth)));
		Gizmos.DrawWireSphere(base.transform.position + base.transform.up * ((Mode == Type.Normal) ? (0f - UpWidth) : (0f - DownWidth)), 0.25f);
	}
}
