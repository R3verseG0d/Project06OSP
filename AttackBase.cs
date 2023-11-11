using UnityEngine;

public class AttackBase : MonoBehaviour
{
	[Header("Attack Base")]
	public LayerMask AttackMask;

	internal Transform Player;

	internal LayerMask HomingBlock_Mask => LayerMask.GetMask("Default", "PlayerCollision", "BreakableObj", "InvisibleCollision", "Object/PlayerOnlyCol");

	internal LayerMask Switch_Mask => LayerMask.GetMask("PlayerOnly");

	public bool AttackSphere(float Speed, int Damage, string Message, float Radius = 0.5f, string Type = "")
	{
		HitInfo value = new HitInfo(Player, base.transform.forward * Speed, Damage);
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				if (Type == "ChaosLance" && (bool)array[i].GetComponent<EnemyBase>())
				{
					array[i].GetComponent<EnemyBase>().Blocking = false;
				}
				if (Type == "ChaosSpear")
				{
					string methodName = (((Message != "OnFlash" && (bool)array[i].GetComponent<ReturnTriggerMessage>() && array[i].GetComponent<ReturnTriggerMessage>().Stun) || (Message == "OnFlash" && (bool)array[i].GetComponent<ReturnTriggerMessage>() && (bool)array[i].GetComponentInParent<eCannon>())) ? "FullStun" : Message);
					array[i].SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					array[i].SendMessage(Message, value, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		return array.Length != 0;
	}

	public bool SwitchAttackSphere(float Radius = 0.5f)
	{
		HitInfo value = new HitInfo(Player, Vector3.zero);
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, Switch_Mask.value, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				if ((bool)array[i].GetComponentInParent<Common_Switch>())
				{
					array[i].SendMessageUpwards("OnSwitch", SendMessageOptions.DontRequireReceiver);
					return true;
				}
				if ((bool)array[i].GetComponentInParent<FlameSingle>())
				{
					array[i].SendMessageUpwards("OnHit", value, SendMessageOptions.DontRequireReceiver);
					return true;
				}
			}
		}
		return false;
	}

	public bool BulletAttackSphere(bool DealDamage)
	{
		HitInfo value = new HitInfo(Player, base.transform.forward * 5f, DealDamage ? 1 : 0);
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.25f, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].SendMessage(((array[i].gameObject.layer == LayerMask.NameToLayer("EnemyTrigger") && DealDamage) || array[i].gameObject.layer != LayerMask.NameToLayer("EnemyTrigger")) ? "OnHit" : "", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool PsychicAttackSphere(float Speed, bool OnlyFlash = false, float Radius = 0.5f)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius, AttackMask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != null))
			{
				continue;
			}
			if (!OnlyFlash)
			{
				EnemyBase componentInParent = array[i].GetComponentInParent<EnemyBase>();
				if (((!componentInParent) ? "OnHit" : (componentInParent.Stuned ? "OnHit" : "OnFlash")) == "OnHit")
				{
					array[i].SendMessage("OnHit", new HitInfo(Player, base.transform.forward * Speed), SendMessageOptions.DontRequireReceiver);
					continue;
				}
				string methodName = (array[i].GetComponentInParent<eCannon>() ? "FullStun" : "OnFlash");
				array[i].SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				string methodName2 = (array[i].GetComponentInParent<eCannon>() ? "FullStun" : "OnFlash");
				array[i].SendMessage(methodName2, SendMessageOptions.DontRequireReceiver);
			}
		}
		return array.Length != 0;
	}

	public bool AttackSphere_Dir(float Radius, float SpreadMultp, int Damage = 1, string Type = "")
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != null))
			{
				continue;
			}
			Vector3 vector = (array[i].transform.position - base.transform.position).MakePlanar();
			if (vector == Vector3.zero)
			{
				vector = base.transform.forward.MakePlanar();
			}
			Vector3 force = (vector + Vector3.up * Random.Range(0.1f, 0.25f)).normalized * SpreadMultp;
			HitInfo value = new HitInfo(Player, force, Damage);
			if (Type == "ChaosBlast" || Type == "ChaosLance")
			{
				array[i].SendMessageUpwards("DestroySearchlight", SendMessageOptions.DontRequireReceiver);
				if (Type == "ChaosBlast" && (bool)array[i].GetComponentInParent<ItemBox>())
				{
					array[i].SendMessageUpwards("OnHit", new HitInfo(Player, Vector3.zero), SendMessageOptions.DontRequireReceiver);
				}
				if ((bool)array[i].GetComponent<EnemyBase>())
				{
					array[i].GetComponent<EnemyBase>().Blocking = false;
				}
			}
			array[i].SendMessage("OnHit", value, SendMessageOptions.DontRequireReceiver);
		}
		return array.Length != 0;
	}

	public bool AttackEnemyProjectile(float Radius)
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, Radius);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && ((bool)array[i].GetComponentInParent<DarkShot>() || ((bool)array[i].GetComponentInParent<Forearm>() && array[i].GetComponentInParent<Forearm>().Launched) || (bool)array[i].GetComponentInParent<Missile>() || ((bool)array[i].GetComponentInParent<TimedBomb>() && array[i].GetComponentInParent<TimedBomb>().Launched)))
			{
				array[i].SendMessage("AutoDestroy", SendMessageOptions.DontRequireReceiver);
				return true;
			}
		}
		return false;
	}

	public GameObject FindTarget(float Range = 7.5f)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("HomingTarget");
		GameObject result = null;
		float num = Range;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Vector3 position = gameObject.transform.position;
			float num2 = Vector3.Distance(base.transform.position, position);
			if (num2 < num && !Physics.Linecast(base.transform.position, gameObject.transform.position, HomingBlock_Mask))
			{
				result = gameObject;
				num = num2;
			}
		}
		return result;
	}
}
