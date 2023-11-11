using System.Collections.Generic;
using UnityEngine;

public class Aqa_Launcher : MonoBehaviour
{
	[Header("Framework")]
	public int Num;

	public int HP;

	public float Friction;

	public float Air_Friction;

	public float Speed;

	public float Time;

	public Vector3 Target;

	[Header("Prefab")]
	public Transform SpawnPoint;

	public Animator Animator;

	public Renderer Renderer;

	public AudioSource Audio;

	public Color NormalColor;

	public Color ChargeColor;

	public GameObject MercuryBall;

	public ParticleSystem ShootFX;

	public ParticleSystem AbsorbFX;

	private MaterialPropertyBlock PropBlock;

	private List<GameObject> Balls = new List<GameObject>();

	private float StartTime;

	public void SetParameters(int _Num, int _HP, float _Friction, float _Air_Friction, float _Speed, float _Time, Vector3 _Target)
	{
		Num = _Num;
		HP = _HP;
		Friction = _Friction;
		Air_Friction = _Air_Friction;
		Speed = _Speed;
		Time = _Time;
		Target = _Target;
	}

	private void Awake()
	{
		if (Target != Vector3.zero)
		{
			PropBlock = new MaterialPropertyBlock();
		}
	}

	private void Start()
	{
		if (!(Target != Vector3.zero))
		{
			return;
		}
		StartTime = UnityEngine.Time.time;
		if (Balls.Count != 0)
		{
			for (int i = 0; i < Balls.Count; i++)
			{
				Object.Destroy(Balls[i].gameObject);
			}
			Balls.Clear();
		}
	}

	private void Update()
	{
		if (!(Target != Vector3.zero))
		{
			return;
		}
		float num = UnityEngine.Time.time - StartTime;
		if (Balls.Count < Num && num > Time * 5f)
		{
			Aqa_Mercury_Small component = Object.Instantiate(MercuryBall, SpawnPoint.position, base.transform.rotation).GetComponent<Aqa_Mercury_Small>();
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				Physics.IgnoreCollision(component.GetComponent<Collider>(), collider);
			}
			component.transform.SetParent(base.transform);
			component.HP = HP;
			component.Friction = Friction;
			component.Air_Friction = Air_Friction;
			component.OnLaunch((Target - SpawnPoint.position).normalized * Speed);
			StartTime = UnityEngine.Time.time;
			ShootFX.Play();
			Audio.pitch = Random.Range(0.75f, 1.25f);
			Audio.Play();
			Animator.SetTrigger("On Launch");
			Balls.Add(component.gameObject);
		}
		if (Balls.Count != 0)
		{
			for (int j = 0; j < Balls.Count; j++)
			{
				if (Balls[j] == null)
				{
					Balls.RemoveAt(j);
				}
			}
		}
		Renderer.GetPropertyBlock(PropBlock, 1);
		PropBlock.SetColor("_Color", Color.Lerp(PropBlock.GetColor("_Color"), (num > Time * 5f - 1f) ? ChargeColor : NormalColor, UnityEngine.Time.deltaTime * 10f));
		Renderer.SetPropertyBlock(PropBlock, 1);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(Target != Vector3.zero))
		{
			Aqa_Mercury_Small component = collision.transform.GetComponent<Aqa_Mercury_Small>();
			if ((bool)component && component.Launch)
			{
				AbsorbFX.transform.position = component.transform.position;
				AbsorbFX.transform.rotation = component.transform.rotation;
				AbsorbFX.Play();
				Object.Destroy(component.gameObject);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Target != Vector3.zero)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, Target);
			Gizmos.DrawWireSphere(Target, 0.5f);
		}
	}
}
