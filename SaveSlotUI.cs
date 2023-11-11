using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
	[Header("Framework")]
	public string FileNumber;

	[Header("Prefab")]
	public Animator Animator;

	public Text FileText;

	public GameObject PanelHL;

	public GameObject PanelHLIncompatible;

	public GameObject CreateNewText;

	public GameObject IncompatibleText;

	public GameObject[] Banners;

	public Text TotalPlaytimeText;

	public Text LastSaveText;

	public CanvasGroup Opacity;

	public void SetUp(float Playtime, string LastSave, int BannerIndex, string FileVersion)
	{
		if (FileVersion == Application.version)
		{
			int num = Mathf.FloorToInt(Playtime / 3600f);
			int num2 = Mathf.FloorToInt(Playtime / 60f);
			int num3 = Mathf.FloorToInt(Playtime - (float)(num2 * 60));
			TotalPlaytimeText.text = $"{num:0}:{num2 % 60:00}:{num3:00}";
			LastSaveText.text = LastSave;
		}
		SetCommon(BannerIndex, FileVersion);
	}

	public void SetBanner(int Index)
	{
		for (int i = 0; i < Banners.Length; i++)
		{
			Banners[i].SetActive(i == Index);
		}
	}

	public void SetUpEmpty()
	{
		SetCommon(10, Application.version);
	}

	private void SetCommon(int Index, string Version)
	{
		Opacity.alpha = ((Index == 10) ? 0.5f : 1f);
		FileText.text = "File " + FileNumber;
		if (Version == Application.version)
		{
			SetBanner(Index);
		}
		if ((bool)PanelHL)
		{
			PanelHL.SetActive(Version == Application.version);
		}
		if ((bool)PanelHLIncompatible)
		{
			PanelHLIncompatible.SetActive(Version != Application.version);
		}
		if ((bool)CreateNewText)
		{
			CreateNewText.SetActive((Index == 10 && Version == Application.version) ? true : false);
		}
		if ((bool)IncompatibleText)
		{
			IncompatibleText.SetActive(Version != Application.version);
		}
	}
}
