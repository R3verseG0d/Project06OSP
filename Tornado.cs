using System.Collections;
using UnityEngine;

public class Tornado : MonoBehaviour
{
	[Header("Prefab")]
	public GameObject[] ShootObjects;

	public GameObject[] Tornados;

	[Header("Optional")]
	public bool NightLight;

	private void Start()
	{
		Tornados[NightLight ? 1 : 0].SetActive(value: true);
	}

	private void TornadoShoot(TornadoParams Params)
	{
		if (Params.ShootIndex != 0)
		{
			StartCoroutine(StartAttack(Target: (!Params.Target && Params.TargetName == "") ? Params.TargetPos : (Params.Target ? Params.Target.transform.position : GameObject.Find(Params.TargetName).transform.position), ShootIndex: Params.ShootIndex, Time: Params.Time));
		}
	}

	private IEnumerator StartAttack(int ShootIndex, Vector3 Target, float Time)
	{
		yield return new WaitForSeconds(Time / 2f);
		GameObject gameObject = Object.Instantiate(ShootObjects[ShootIndex - 1], base.transform.position + base.transform.up * Random.Range(150f, 200f), Random.rotation);
		Rigidbody component = gameObject.FindInChildren("RootMesh").GetComponent<Rigidbody>();
		StartCoroutine(PreThrow(gameObject));
		Vector3 force = (Target - gameObject.transform.position).normalized * 100f;
		force.y += Random.Range(5f, 10f);
		component.AddForce(force, ForceMode.VelocityChange);
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
}
