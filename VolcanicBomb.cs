using UnityEngine;

public class VolcanicBomb : ObjectBase
{
	[Header("Framework")]
	public Vector3 VolcanoObj;

	public float CycleTime;

	public float VolcanoTime;

	public float BombHeight;

	public float BombRange;

	[Header("Optional")]
	public float Speed = 30f;

	[Header("Prefab")]
	public GameObject MeteorPrefab;

	private Transform Player;

	private bool PlayerInArea;

	private float StartTime;

	private float VolcanoStartTime;

	public void SetParameters(Vector3 _VolcanoObj, float _CycleTime, float _VolcanoTime, float _BombHeight, float _BombRange)
	{
		VolcanoObj = _VolcanoObj;
		CycleTime = _CycleTime;
		VolcanoTime = _VolcanoTime;
		BombHeight = _BombHeight;
		BombRange = _BombRange;
	}

	private void Update()
	{
		if (PlayerInArea && Time.time - VolcanoStartTime < VolcanoTime && Time.time - StartTime > CycleTime)
		{
			Vector3 volcanoObj = VolcanoObj;
			volcanoObj.y += BombHeight;
			Vector3 position = volcanoObj + Random.insideUnitSphere * BombRange;
			Object.Instantiate(MeteorPrefab, position, Random.rotation).GetComponent<BossBomb>().RBody.velocity = (Player.position - volcanoObj).normalized * Speed;
			StartTime = Time.time;
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			PlayerInArea = true;
			Player = collider.transform;
			StartTime = Time.time;
			VolcanoStartTime = Time.time;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			PlayerInArea = false;
			Player = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (VolcanoObj != Vector3.zero)
		{
			Vector3 volcanoObj = VolcanoObj;
			volcanoObj.y += BombHeight;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, volcanoObj);
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(volcanoObj, 1f);
		}
	}
}
