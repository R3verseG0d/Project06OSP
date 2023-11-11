using UnityEngine;

public class Bullet : ObjectBase
{
	public Rigidbody _Rigidbody;

	public GameObject BulletShell;

	public GameObject BulletHitFX;

	private void Start()
	{
		Object.Destroy(base.transform.gameObject, 3f);
	}

	private void FixedUpdate()
	{
		_Rigidbody.MovePosition(base.transform.position += base.transform.forward * 50f * Time.fixedDeltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		BulletHit();
		if (collision.gameObject.tag == "Vehicle")
		{
			collision.gameObject.transform.SendMessage("OnVehicleHit", 0.25f, SendMessageOptions.DontRequireReceiver);
		}
		PlayerBase player = GetPlayer(collision.transform);
		if ((bool)player)
		{
			player.OnBulletHit(base.transform.forward);
		}
	}

	private void BulletHit()
	{
		Object.Instantiate(BulletHitFX, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	private void OnPsychokinesis(Transform DummyTransform)
	{
		Object.Instantiate(BulletShell, base.transform.position, base.transform.rotation);
		BulletHit();
	}

	private void OnDeflect(Transform _PlayerPos)
	{
		if (base.enabled)
		{
			PlayerBase component = _PlayerPos.GetComponent<PlayerBase>();
			if ((bool)component && (bool)component.PlayerManager.silver && component.PlayerManager.silver.IsAwakened)
			{
				component.PlayerManager.silver.SilverEffects.CreatePsiDeflectFX(base.transform.position);
			}
			base.transform.forward = (base.transform.position - _PlayerPos.position).normalized;
		}
	}
}
