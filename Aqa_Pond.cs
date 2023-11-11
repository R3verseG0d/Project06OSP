using System.Collections.Generic;
using UnityEngine;

public class Aqa_Pond : MonoBehaviour
{
	[Header("Framework")]
	public float Length;

	public float Width;

	public float Height;

	public int Num;

	public int HP;

	public float Friction;

	public float Air_Friction;

	public float Vel;

	public float Time;

	[Header("Prefab")]
	public GameObject MercuryBall;

	private List<GameObject> Balls = new List<GameObject>();

	private float StartTime;

	private float SpawnTime;

	public void SetParameters(float _Length, float _Width, float _Height, int _Num, int _HP, float _Friction, float _Air_Friction, float _Vel, float _Time)
	{
		Length = _Length;
		Width = _Width;
		Height = _Height;
		Num = _Num;
		HP = _HP;
		Friction = _Friction;
		Air_Friction = _Air_Friction;
		Vel = _Vel;
		Time = _Time;
	}

	private void Start()
	{
		StartTime = UnityEngine.Time.time;
		SpawnTime = Time / (float)Num;
	}

	private void Update()
	{
		if (Balls.Count < Num && UnityEngine.Time.time - StartTime > SpawnTime)
		{
			Vector3 vector = base.transform.position + new Vector3(Random.Range(0f - Width, Width) / 2f, Height / 2f + Random.Range(0f - Height, Height) / 2f, Random.Range(0f - Length, Length) / 2f);
			Aqa_Mercury_Small component = Object.Instantiate(MercuryBall, vector + base.transform.up * 0.5f, base.transform.rotation).GetComponent<Aqa_Mercury_Small>();
			component.transform.SetParent(base.transform.parent);
			base.transform.parent.gameObject.SendMessage("AddObject", null, SendMessageOptions.DontRequireReceiver);
			component.HP = HP;
			component.Friction = Friction;
			component.Air_Friction = Air_Friction;
			component.OnAppear(Vel);
			StartTime = UnityEngine.Time.time;
			Balls.Add(component.gameObject);
		}
		if (Balls.Count == 0)
		{
			return;
		}
		for (int i = 0; i < Balls.Count; i++)
		{
			if (Balls[i] == null)
			{
				Balls.RemoveAt(i);
				StartTime = UnityEngine.Time.time;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(size: new Vector3(Width, Height, Length), center: base.transform.position + base.transform.up * Height / 2f);
	}
}
