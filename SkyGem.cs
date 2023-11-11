using UnityEngine;

public class SkyGem : MonoBehaviour
{
	[Header("Framework")]
	public Rigidbody Rigidbody;

	public Collider Collider;

	public Transform Mesh;

	internal SonicNew Player;

	private float StartTime;

	private void Start()
	{
		base.transform.SetParent(null);
		base.transform.up = Player.transform.forward;
		StartTime = Time.time;
		Collider.enabled = true;
	}

	private void FixedUpdate()
	{
		if (Time.time - StartTime > 5f || Player.ActiveGem != SonicNew.Gem.Sky)
		{
			DestroyItem(PullPlayer: false);
		}
		Mesh.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized);
	}

	public void SetGem(SonicNew _Player, Vector3 Velocity)
	{
		Player = _Player;
		Rigidbody.isKinematic = false;
		Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		Rigidbody.AddForce(Velocity, ForceMode.Impulse);
	}

	private void OnCollisionEnter(Collision collider)
	{
		if (!(collider.gameObject.tag == "Player") && collider.gameObject.layer != LayerMask.NameToLayer("PlayerPushColl"))
		{
			DestroyItem();
		}
	}

	public void DestroyItem(bool PullPlayer = true)
	{
		if (PullPlayer)
		{
			float num = Mathf.Lerp(25f, 50f, (base.transform.position - Player.transform.position).magnitude / 80f);
			float timer = Mathf.Clamp(Vector3.Distance(base.transform.position, Player.transform.position) / num, 0f, 80f);
			Player.OnGunDriveMove(base.transform.position, timer, num);
		}
		Player.SkyGemObject = null;
		Object.Destroy(base.gameObject);
	}
}
