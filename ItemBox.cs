using UnityEngine;

public class ItemBox : ObjectBase
{
	public enum Type
	{
		Default = 0,
		Rings5 = 1,
		Rings10 = 2,
		Rings20 = 3,
		Life = 4,
		SpeedUp = 5,
		GaugeUp = 6,
		Invincible = 7,
		Shield = 8
	}

	[Header("Framework")]
	public Type Item;

	[Header("Prefab")]
	public bool GroundItem;

	public Collider Collider;

	public Transform ItemDisplay;

	public Animator GlassAnimator;

	public GameObject HomingTarget;

	public GameObject FX;

	public GameObject AppearFX;

	public Renderer BoxRenderer;

	public Texture2D[] BoxTextures;

	public Texture2D[] LifeBoxTextures;

	internal bool IsCaged;

	private MaterialPropertyBlock PropBlock;

	private bool Collected;

	private bool CollectedLife;

	public void SetParameters(int _Item)
	{
		Item = (Type)_Item;
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
		BoxRenderer.GetPropertyBlock(PropBlock);
		PropBlock.SetTexture("_MainTex", (Item == Type.Life) ? LifeBoxTextures[Singleton<Settings>.Instance.settings.DisplayType] : BoxTextures[(int)Item]);
		BoxRenderer.SetPropertyBlock(PropBlock);
		CollectedLife = Item == Type.Life && Singleton<GameManager>.Instance.LifeItemIDs.Contains(base.gameObject.name);
		if (CollectedLife)
		{
			ItemDisplay.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (Collected && FX.activeSelf)
		{
			FX.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (!Collected && !CollectedLife)
		{
			ItemDisplay.Rotate(Vector3.up * 90f * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!IsCaged)
		{
			if ((bool)GetPlayer(collider))
			{
				OnCollect(collider.transform);
			}
			AmigoAIBase component = collider.GetComponent<AmigoAIBase>();
			if ((bool)component)
			{
				OnCollect(component.FollowTarget.transform);
			}
		}
	}

	private void SpawnFX()
	{
		AppearFX.SetActive(value: true);
	}

	private void OnHit(HitInfo HitInfo)
	{
		if (!IsCaged)
		{
			OnCollect(HitInfo.player, HomingHit: true);
		}
	}

	private void OnCollect(Transform Player, bool HomingHit = false)
	{
		if (Collected || IsCaged)
		{
			return;
		}
		PlayerBase player = GetPlayer(Player);
		if (!player || (player.GetState() == "Homing" && !HomingHit))
		{
			return;
		}
		Collected = true;
		GlassAnimator.SetTrigger("OnItemGet");
		Collider.enabled = false;
		if (!CollectedLife)
		{
			player.SetUIItemBox((int)Item);
			switch (Item)
			{
			case Type.Default:
				player.AddRing(5);
				player.AddScore(50);
				break;
			case Type.Rings5:
				player.AddRing(5);
				player.AddScore(50);
				break;
			case Type.Rings10:
				player.AddRing(10);
				player.AddScore(100);
				break;
			case Type.Rings20:
				player.AddRing(20);
				player.AddScore(200);
				break;
			case Type.Life:
				player.AddLife();
				player.AddScore(200);
				Singleton<GameManager>.Instance.LifeItemIDs.Add(base.gameObject.name);
				break;
			case Type.SpeedUp:
				player.GrantPowerUp("SpeedUp");
				player.AddScore(200);
				break;
			case Type.GaugeUp:
				player.HUD.AddChaosDriveEnergy(Replenish: true);
				break;
			case Type.Invincible:
				if ((player.GetPrefab("sonic_new") && !Player.GetComponent<SonicNew>().IsSuper) || !player.GetPrefab("sonic_new"))
				{
					player.GrantPowerUp("Invincible");
				}
				player.AddScore(200);
				break;
			case Type.Shield:
				player.GrantPowerUp("Shield");
				player.AddScore(200);
				break;
			}
		}
		FX.SetActive(value: true);
		Invoke("DisableFX", 1f);
		if (!CollectedLife)
		{
			Object.Destroy(ItemDisplay.gameObject);
		}
		Object.Destroy(HomingTarget.gameObject);
		Object.Destroy(GroundItem ? GlassAnimator.transform.gameObject : base.gameObject, 1f);
	}

	private void DisableFX()
	{
		FX.SetActive(value: false);
	}
}
