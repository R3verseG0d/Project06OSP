using UnityEngine;

public class AmigoCollision : ObjectBase
{
	[Header("Framework")]
	public Vector3 Target;

	public bool Chase;

	[Header("Framework Settings")]
	public PlayerStart TargetSettings;

	[Header("Prefab/Optional")]
	public GameObject[] ActivateObjects;

	public GameObject[] DisableObjects;

	[Header("Prefab")]
	public GameObject Amigo;

	private bool Triggered;

	public void SetParameters(Vector3 _Target, bool _Chase)
	{
		Target = _Target;
		Chase = _Chase;
	}

	private void OnTriggerEnter(Collider collider)
	{
		PlayerBase player = GetPlayer(collider);
		if (!player || player.IsDead || Triggered)
		{
			return;
		}
		Triggered = true;
		if (Chase)
		{
			player.AmigoIndex++;
			bool flag = false;
			Vector3 position = player.transform.position;
			Quaternion rotation = player.transform.rotation;
			Collider[] array = Physics.OverlapSphere(Target + Vector3.up * 0.25f, 1f, 1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && (bool)array[i].GetComponent<AmigoSwitch>())
				{
					Object.Destroy(array[i].gameObject);
					position = array[i].transform.position;
					rotation = array[i].transform.rotation;
					flag = true;
				}
			}
			GameObject gameObject = Object.Instantiate(Amigo, position, rotation);
			gameObject.GetComponent<BoxCollider>().enabled = false;
			AmigoAIBase componentInChildren = gameObject.GetComponentInChildren<AmigoAIBase>();
			componentInChildren.SwitchTransform = gameObject.transform;
			componentInChildren.CapsuleCollider.enabled = true;
			componentInChildren.PushCollider.SetActive(value: true);
			componentInChildren._Rigidbody.isKinematic = false;
			componentInChildren._Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			componentInChildren.FollowTarget = player;
			componentInChildren.TopSpeed = player.TopSpeed;
			componentInChildren.PlayerAmigoIndex = player.AmigoIndex;
			if (!flag)
			{
				gameObject.transform.position = componentInChildren.AmigoPoint();
			}
			AmigoSwitch component = gameObject.GetComponent<AmigoSwitch>();
			component.enabled = false;
			if ((bool)TargetSettings)
			{
				for (int j = 0; j < component.Characters.Length; j++)
				{
					if (component.Characters[j].name == TargetSettings.GetPlayerName())
					{
						component.Characters[j].SetActive(value: true);
						componentInChildren.Params = component.Characters[j].GetComponent<AmigoParams>();
					}
				}
			}
			gameObject.GetComponentInChildren<AmigoAI>().enabled = true;
		}
		else
		{
			AmigoAI[] array2 = Object.FindObjectsOfType<AmigoAI>();
			if (array2 != null)
			{
				for (int k = 0; k < array2.Length; k++)
				{
					if (array2[k].enabled)
					{
						array2[k].DestroyAmigo();
					}
				}
			}
			Collider[] array3 = Physics.OverlapSphere(Target + Vector3.up * 0.25f, 1f, 1, QueryTriggerInteraction.Collide);
			for (int l = 0; l < array3.Length; l++)
			{
				if (array3[l] != null && (bool)array3[l].GetComponent<AmigoSwitch>())
				{
					Object.Destroy(array3[l].gameObject);
				}
			}
			SwitchToAmigo(player);
		}
		if (ActivateObjects != null)
		{
			for (int m = 0; m < ActivateObjects.Length; m++)
			{
				ActivateObjects[m].SetActive(value: true);
			}
		}
		if (DisableObjects != null)
		{
			for (int n = 0; n < DisableObjects.Length; n++)
			{
				DisableObjects[n].SetActive(value: false);
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void SwitchToAmigo(PlayerBase Player)
	{
		PlayerBase component = (Object.Instantiate(Resources.Load("DefaultPrefabs/Player/" + TargetSettings.GetPlayerName()), TargetSettings.transform.position + Vector3.up * 0.25f, TargetSettings.transform.rotation) as GameObject).GetComponent<PlayerBase>();
		component.SetPlayer(TargetSettings.Player_No, TargetSettings.GetPlayerName());
		component.StartPlayer(TalkStart: true);
		component.HUD.UseCrosshair(EndCrosshair: true, Reset: true);
		if (Player.HasShield)
		{
			component.HasShield = true;
			component.ShieldObject = Player.ShieldObject;
			component.ShieldObject.transform.position = component.transform.position + component.transform.up * ((!component.GetPrefab("omega")) ? 0.25f : 0.5f);
			component.ShieldObject.transform.rotation = Quaternion.identity;
			Player.ShieldObject.transform.SetParent(component.transform);
			component.ShieldObject.transform.localScale = Vector3.one * ((!component.GetPrefab("omega")) ? 1f : 1.5f);
		}
		if (Player.GetState() != "Orca")
		{
			Object.Destroy(Player.gameObject);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, Target);
		Gizmos.DrawWireSphere(Target, 1f);
		if ((bool)TargetSettings)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(base.transform.position, TargetSettings.transform.position);
			Gizmos.DrawWireSphere(TargetSettings.transform.position, 1.25f);
		}
	}
}
