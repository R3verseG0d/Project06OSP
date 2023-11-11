using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreens : MonoBehaviour
{
	public enum Type
	{
		Disclaimer = 0,
		AutoSave = 1,
		Splash = 2
	}

	[Header("Framework")]
	public string GoTo;

	public Type Mode;

	public Animator Animator;

	public AudioSource Audio;

	public bool GameInitializer;

	public Animator DebugSignal;

	private bool PressedStart;

	private int SplashIndex;

	public string[] DebugCode;

	public int CodeIndex;

	public bool DebugEnabled;

	private void Start()
	{
		if (GameInitializer)
		{
			Cursor.visible = false;
			Singleton<Settings>.Instance.LoadSettings();
			Singleton<AudioManager>.Instance.StartAudioManager();
			Singleton<MSTManager>.Instance.StartMSTManager();
			Singleton<GameManager>.Instance.StartGameManager();
			DebugCode = new string[6] { "Button X", "Button B", "Button A", "Button Y", "Left Bumper", "Right Bumper" };
			CodeIndex = 0;
		}
		if ((bool)Animator && Mode == Type.Disclaimer)
		{
			SplashIndex = Random.Range(0, 3);
			Animator.SetInteger("Char Index", SplashIndex);
			Animator.SetTrigger("On Start");
		}
	}

	private void Update()
	{
		if ((bool)Animator)
		{
			if (Mode == Type.Disclaimer)
			{
				if (((SplashIndex == 0 && Animator.GetCurrentAnimatorStateInfo(0).IsName("FadeIn")) || (SplashIndex == 1 && Animator.GetCurrentAnimatorStateInfo(0).IsName("FadeIn_Red")) || (SplashIndex == 2 && Animator.GetCurrentAnimatorStateInfo(0).IsName("FadeIn_Silver"))) && !PressedStart && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
				{
					PressedStart = true;
					Animator.SetTrigger("On Start Press");
					if ((bool)Audio)
					{
						Audio.Play();
					}
				}
			}
			else if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Splash1") && !PressedStart && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
			{
				PressedStart = true;
				Animator.SetTrigger("On Start Press");
				if ((bool)Audio)
				{
					Audio.Play();
				}
			}
		}
		if (Mode == Type.Splash && !PressedStart && Singleton<RInput>.Instance.P.GetButtonDown("Start"))
		{
			PressedStart = true;
			NextScene();
		}
		if (!GameInitializer || DebugEnabled)
		{
			return;
		}
		if (Singleton<RInput>.Instance.P.GetAnyButtonDown())
		{
			if (Singleton<RInput>.Instance.P.GetButtonDown(DebugCode[CodeIndex]))
			{
				CodeIndex++;
			}
			else
			{
				CodeIndex = 0;
			}
		}
		if (CodeIndex == DebugCode.Length)
		{
			Singleton<DebugManager>.Instance.StartDebugManager();
			DebugSignal.SetTrigger("On Signal");
			DebugEnabled = true;
		}
	}

	public void NextScene()
	{
		SceneManager.LoadScene(GoTo);
	}
}
