using UnityEngine;

public class RadarMap : MonoBehaviour
{
	[Header("Prefab")]
	public GameObject[] Maps;

	public Camera TargetCamera;

	public Renderer MapRenderer;

	[Header("Icons")]
	public GameObject Player;

	public GameObject WarpGate;

	private PlayerBase PlayerBase;

	private WarpGate[] Gates;

	private UI HUD;

	private MaterialPropertyBlock PropBlock;

	private Transform MainCamera;

	private Transform PlayerIcon;

	public static Color32 SonicBlue = new Color32(0, 49, 66, byte.MaxValue);

	public static Color32 SonicGreen = new Color32(115, 162, 202, byte.MaxValue);

	public static Color32 SonicLightGray = new Color32(0, 62, 99, byte.MaxValue);

	public static Color32 SonicDarkGray = new Color32(0, 86, 135, byte.MaxValue);

	public static Color32 ShadowBlue = new Color32(66, 11, 0, byte.MaxValue);

	public static Color32 ShadowGreen = new Color32(202, 140, 115, byte.MaxValue);

	public static Color32 ShadowLightGray = new Color32(111, 38, 23, byte.MaxValue);

	public static Color32 ShadowDarkGray = new Color32(154, 34, 25, byte.MaxValue);

	public static Color32 SilverBlue = new Color32(41, 65, 63, byte.MaxValue);

	public static Color32 SilverGreen = new Color32(170, 188, 182, byte.MaxValue);

	public static Color32 SilverLightGray = new Color32(78, 99, 102, byte.MaxValue);

	public static Color32 SilverDarkGray = new Color32(90, 123, 112, byte.MaxValue);

	public void OpenMap(int Index)
	{
		Maps[Index].SetActive(value: true);
	}

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		PlayerBase = Object.FindObjectOfType<PlayerBase>();
		HUD = PlayerBase.HUD;
		MainCamera = Camera.main.transform;
		PlayerIcon = Object.Instantiate(Player, Vector3.zero, Quaternion.identity).transform;
		PlayerIcon.SetParent(base.transform);
		Gates = Object.FindObjectsOfType<WarpGate>();
		if (Gates != null)
		{
			for (int i = 0; i < Gates.Length; i++)
			{
				Transform transform = Object.Instantiate(WarpGate, Vector3.zero, Quaternion.identity).transform;
				transform.SetParent(base.transform);
				transform.position = new Vector3(Gates[i].transform.position.x, transform.position.y, Gates[i].transform.position.z);
			}
		}
		TargetCamera.backgroundColor = BlueColor();
		MapRenderer.GetPropertyBlock(PropBlock, 0);
		PropBlock.SetColor("_Color", BlueColor());
		MapRenderer.SetPropertyBlock(PropBlock, 0);
		MapRenderer.GetPropertyBlock(PropBlock, 1);
		PropBlock.SetColor("_Color", GreenColor());
		MapRenderer.SetPropertyBlock(PropBlock, 1);
		MapRenderer.GetPropertyBlock(PropBlock, 2);
		PropBlock.SetColor("_Color", LightGrayColor());
		MapRenderer.SetPropertyBlock(PropBlock, 2);
		MapRenderer.GetPropertyBlock(PropBlock, 3);
		PropBlock.SetColor("_Color", DarkGrayColor());
		MapRenderer.SetPropertyBlock(PropBlock, 3);
	}

	private void Update()
	{
		if (!PlayerBase)
		{
			PlayerBase = Object.FindObjectOfType<PlayerBase>();
		}
		TargetCamera.transform.position = new Vector3(PlayerBase.transform.position.x, TargetCamera.transform.position.y, PlayerBase.transform.position.z);
		TargetCamera.transform.rotation = Quaternion.LookRotation(MainCamera.forward.MakePlanar()) * Quaternion.Euler(90f, 0f, 0f);
		PlayerIcon.position = new Vector3(PlayerBase.transform.position.x, PlayerIcon.position.y, PlayerBase.transform.position.z);
		PlayerIcon.forward = PlayerBase.transform.forward.MakePlanar();
		float f = Vector3.Dot(Vector3.Cross(Vector3.forward, MainCamera.forward.MakePlanar()), Vector3.up);
		HUD.RadarCompass.eulerAngles = new Vector3(0f, 0f, Vector3.Angle(MainCamera.forward.MakePlanar(), Vector3.forward) * Mathf.Sign(f));
	}

	public Color32 BlueColor()
	{
		Color32 result = SonicBlue;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowBlue;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverBlue;
		}
		return result;
	}

	public Color32 GreenColor()
	{
		Color32 result = SonicGreen;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowGreen;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverGreen;
		}
		return result;
	}

	public Color32 LightGrayColor()
	{
		Color32 result = SonicLightGray;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowLightGray;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverLightGray;
		}
		return result;
	}

	public Color32 DarkGrayColor()
	{
		Color32 result = SonicDarkGray;
		if (Singleton<GameManager>.Instance.GetGameStory() == "Shadow")
		{
			result = ShadowDarkGray;
		}
		if (Singleton<GameManager>.Instance.GetGameStory() == "Silver")
		{
			result = SilverDarkGray;
		}
		return result;
	}
}
