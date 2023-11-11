using UnityEngine;

public class ChaosDrive : ObjectBase
{
	public Rigidbody _Rigidbody;

	public Transform Mesh;

	public GameObject driveGetPrefab;

	private PlayerBase Target;

	private bool Grabbed;

	private float StartTime;

	private void Start()
	{
		StartTime = Time.time;
	}

	private void FixedUpdate()
	{
		if (!Target || !Target.GetPrefab(Target.StageManager.Player.ToString().ToLower()))
		{
			Target = Object.FindObjectOfType<PlayerBase>();
		}
		float num = Time.time - StartTime;
		if (!Grabbed)
		{
			float num2 = Mathf.Min(num * 7.5f, 40f);
			base.transform.forward = Vector3.Lerp(base.transform.forward, (Target.transform.position + Target.transform.up * 0.25f - base.transform.position).normalized, Time.fixedDeltaTime * num2);
			float num3 = Mathf.Min(num * 20f, 20f) + Mathf.Min(num * 5f, 30f);
			_Rigidbody.MovePosition(base.transform.position + base.transform.forward * num3 * Time.fixedDeltaTime);
			Mesh.RotateAround(Mesh.position, Mesh.right, 30f * num3 * Time.fixedDeltaTime);
		}
		if (Singleton<GameManager>.Instance.GameState == GameManager.State.Result)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if ((bool)player && !Grabbed)
		{
			Grabbed = true;
			player.HUD.AddChaosDriveEnergy();
			Object.Instantiate(driveGetPrefab, collider.transform.position + collider.transform.up * 0.25f, base.transform.rotation).transform.parent = player.transform;
			Object.Destroy(base.gameObject);
		}
	}
}
