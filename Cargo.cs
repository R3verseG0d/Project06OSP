using System.Collections;
using STHEngine;
using UnityEngine;

public class Cargo : MonoBehaviour
{
	[Header("Prefab")]
	public AudioSource TrainRunSource;

	public Transform[] Wheels;

	[Header("Freight")]
	public bool IsFreightCargo;

	public GameObject[] ObjectToSpawn;

	public Transform[] Points;

	public string CargoID;

	[Header("Eggman Train")]
	public bool IsEggtrainBomb;

	public GameObject ExplosionFX;

	public GameObject FireHazard;

	public GameObject SplinterFX;

	public GameObject[] SplinterObjs;

	public GameObject BrokenObj;

	public Renderer Renderer;

	public GameObject[] Models;

	public GameObject ShellObj;

	public ParticleSystem DamageFX;

	public AudioSource DamageSource;

	internal BezierCurve Curve;

	internal bool StartProgress;

	internal bool Loop;

	internal bool UseHP;

	internal float Speed;

	internal float SplineSpeed;

	internal float Progress;

	internal int HP;

	internal int MaxHP;

	private bool Damaged1;

	private bool Damaged2;

	private void Start()
	{
		base.transform.position = Curve.GetPosition(Progress);
		base.transform.rotation = Quaternion.LookRotation(Curve.GetTangent(Progress));
		if (IsFreightCargo)
		{
			for (int i = 0; i < CargoID.Length; i++)
			{
				_ = Random.value;
				if (CargoID[i] == '1')
				{
					Object.Instantiate(ObjectToSpawn[Random.Range(0, ObjectToSpawn.Length)], Points[i].position, Points[i].rotation, Points[i]);
				}
			}
		}
		TrainRunSource.time = Random.Range(0.1f, TrainRunSource.clip.length);
	}

	private void Update()
	{
		if (!IsEggtrainBomb && !IsFreightCargo)
		{
			SplineSpeed = Mathf.Lerp(SplineSpeed, StartProgress ? Speed : 0f, Time.deltaTime * 5f);
			Progress += SplineSpeed / Curve.Length() * Time.deltaTime;
		}
		else if (StartProgress)
		{
			Progress += Speed / Curve.Length() * Time.deltaTime;
		}
		if (IsEggtrainBomb && UseHP && HP <= 0)
		{
			GameObject gameObject = Object.Instantiate(BrokenObj, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", new HitInfo(base.transform, base.transform.position));
			for (int i = 0; i < Random.Range(8, 12); i++)
			{
				GameObject gameObject2 = Object.Instantiate(SplinterObjs[Random.Range(0, SplinterObjs.Length)], base.transform.position + base.transform.up * 4.25f, Quaternion.LookRotation(-base.transform.forward));
				ExtensionMethods.SetBrokenColFix(base.transform, gameObject2);
				float xAngle = Random.Range(-7.5f, 7.5f);
				float yAngle = Random.Range(-5f, 5f);
				gameObject2.transform.Rotate(xAngle, yAngle, 0f);
				gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * 25f, ForceMode.VelocityChange);
				Object.Instantiate(SplinterFX, gameObject2.transform.position, base.transform.rotation).transform.SetParent(gameObject2.transform);
			}
			Object.Instantiate(ExplosionFX, base.transform.position + base.transform.up * 4.25f, Quaternion.identity);
			Object.Destroy(base.gameObject);
		}
		if (Progress >= 1f)
		{
			if (!Loop)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				Progress = 0f;
			}
		}
		for (int j = 0; j < Wheels.Length; j++)
		{
			Wheels[j].Rotate((IsEggtrainBomb ? SplineSpeed : Speed) * Time.deltaTime * 75f, 0f, 0f);
		}
		base.transform.position = Curve.GetPosition(Progress);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Curve.GetTangent(Progress)), Time.deltaTime * 5f);
		TrainRunSource.volume = Mathf.Lerp(TrainRunSource.volume, StartProgress ? 0.75f : 0f, Time.deltaTime * 5f);
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (UseHP)
		{
			HP--;
			if ((float)HP < (float)MaxHP / 1.5f && !Damaged1)
			{
				Models[0].SetActive(value: false);
				Models[1].SetActive(value: true);
				OnDamage(0);
				Damaged1 = true;
			}
			if ((float)HP < (float)MaxHP / 3f && !Damaged2)
			{
				Models[0].SetActive(value: false);
				Models[1].SetActive(value: true);
				OnDamage(1);
				Damaged2 = true;
			}
		}
	}

	private void OnDamage(int BreakType)
	{
		switch (BreakType)
		{
		case 0:
			Models[0].SetActive(value: false);
			Models[1].SetActive(value: true);
			break;
		case 1:
		{
			Models[1].SetActive(value: false);
			Models[2].SetActive(value: true);
			GameObject gameObject = Object.Instantiate(ShellObj, base.transform.position, base.transform.rotation);
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
			gameObject.SendMessage("OnCreate", new HitInfo(base.transform, base.transform.position));
			break;
		}
		}
		DamageFX.Play();
		DamageSource.pitch = Random.Range(0.75f, 1f);
		DamageSource.Play();
	}

	public void CutBomb(float BombTime)
	{
		if (IsEggtrainBomb && !UseHP)
		{
			StartCoroutine(BombExplode(BombTime));
		}
	}

	private IEnumerator BombExplode(float BombTime)
	{
		float StartTime = Time.time;
		float Timer = 0f;
		float BlinkTimer = 0f;
		while (Timer <= BombTime)
		{
			Timer = Time.time - StartTime;
			BlinkTimer += Time.fixedDeltaTime * 15f;
			if (BlinkTimer >= 1f)
			{
				BlinkTimer = 0f;
			}
			Renderer.enabled = BlinkTimer <= 0.5f;
			yield return new WaitForFixedUpdate();
		}
		GameObject gameObject = Object.Instantiate(BrokenObj, base.transform.position, base.transform.rotation);
		Object.Instantiate(FireHazard, base.transform.position, base.transform.rotation);
		ExtensionMethods.SetBrokenColFix(base.transform, gameObject);
		gameObject.SendMessage("OnCreate", new HitInfo(base.transform, base.transform.position));
		for (int i = 0; i < Random.Range(8, 12); i++)
		{
			GameObject gameObject2 = Object.Instantiate(SplinterObjs[Random.Range(0, SplinterObjs.Length)], base.transform.position + base.transform.up * 4.25f, Quaternion.LookRotation(-base.transform.forward));
			ExtensionMethods.SetBrokenColFix(base.transform, gameObject2);
			float xAngle = Random.Range(-7.5f, 7.5f);
			float yAngle = Random.Range(-5f, 5f);
			gameObject2.transform.Rotate(xAngle, yAngle, 0f);
			gameObject2.GetComponent<Rigidbody>().AddForce(gameObject2.transform.forward * 25f, ForceMode.VelocityChange);
			Object.Instantiate(SplinterFX, gameObject2.transform.position, base.transform.rotation).transform.SetParent(gameObject2.transform);
		}
		Object.Instantiate(ExplosionFX, base.transform.position + base.transform.up * 4.25f, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}
}
