using UnityEngine;

public class DummyRingBomb : ObjectBase
{
	[Header("Framework")]
	public Collider Collider;

	public GameObject DummyRingPrefab;

	public GameObject ParticlePrefab;

	internal Transform Player;

	internal GameObject ClosestTarget;

	private bool Destroyed;

	private void Start()
	{
		Collider.enabled = true;
		Object.Destroy(base.gameObject, 5f);
	}

	private void Update()
	{
		if ((bool)ClosestTarget)
		{
			base.transform.forward = ClosestTarget.transform.position - base.transform.position;
			GetComponent<Rigidbody>().velocity = base.transform.forward * GetComponent<Rigidbody>().velocity.magnitude;
		}
		Quaternion rotation = base.transform.rotation;
		rotation.x = 0f;
		rotation.z = 0f;
		base.transform.rotation = rotation;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!(collider.gameObject.tag == "Player") && !(collider.gameObject.tag == "Amigo") && collider.gameObject.layer != LayerMask.NameToLayer("PlayerPushColl") && collider.gameObject.layer != LayerMask.NameToLayer("BrokenObj") && !collider.GetComponentInParent<AmigoSwitch>() && !collider.GetComponentInParent<DummyRing>() && !Destroyed)
		{
			Destroyed = true;
			DropDummyRing();
			Object.Instantiate(ParticlePrefab, base.transform.position, Quaternion.identity);
			HitInfo value = new HitInfo(Player, GetComponent<Rigidbody>().velocity / 4f);
			collider.SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
			Object.Destroy(base.gameObject);
		}
	}

	public void DropDummyRing()
	{
		float num = Random.Range(0f, 360f);
		for (int i = 0; i < 10; i++)
		{
			num += 36f;
			Vector3 vector = Quaternion.Euler(new Vector3(0f, num, 0f)) * Vector3.forward;
			GameObject obj = Object.Instantiate(DummyRingPrefab, base.transform.position + vector * 0.5f + base.transform.up * 0.25f, Quaternion.identity);
			obj.GetComponent<DummyRing>().Player = Player;
			obj.GetComponent<DummyRing>().SetVelocity(vector * 6f + Vector3.up * 7.5f);
		}
	}
}
