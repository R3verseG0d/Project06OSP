using UnityEngine;

public class HeadController : MonoBehaviour
{
	[Header("Framework")]
	public PlayerManager PM;

	private bool ResultsMode;

	private void Update()
	{
		if (!(PM.Base.GetState() != "Cutscene"))
		{
			return;
		}
		if ((PM.Base.GetPrefab("sonic_new") && !PM.sonic.IsSuper) || (PM.Base.GetPrefab("sonic_fast") && !PM.sonic_fast.IsSuper) || (!PM.Base.GetPrefab("sonic_new") && !PM.Base.GetPrefab("sonic_fast")))
		{
			if (PM.Base.GetState() == "Ground" || PM.Base.GetState() == "Result")
			{
				if (PM.Base.GetState() != "Result")
				{
					if (PM.Base.TargetDirection.magnitude != 0f && PM.Base.CurSpeed != 0f)
					{
						PlayFaceAnim(1);
					}
					else
					{
						UpdateIdleGestures();
					}
					return;
				}
				PM.Base.Animator.SetBool("Face Results", value: true);
				if (!ResultsMode)
				{
					PM.Base.Animator.SetTrigger("On Face Results");
					ResultsMode = true;
				}
			}
			else if (PM.Base.GetState() == "Talk")
			{
				PlayFaceAnim(3);
			}
			else if ((PM.Base.GetPrefab("shadow") && PM.Base.GetState() == "UninhibitBreak") || (PM.Base.GetPrefab("amy") && PM.Base.GetState() == "HammerSpin" && PM.amy.HammerSpinState == 2))
			{
				PlayFaceAnim(5);
			}
			else
			{
				PlayFaceAnim(1);
			}
		}
		else
		{
			PlayFaceAnim(1);
		}
	}

	private void UpdateIdleGestures()
	{
		if (PM.Base.GetPrefab("sonic_new"))
		{
			if (Singleton<Settings>.Instance.settings.TGSSonic == 0)
			{
				PlayFaceAnim((IsAnimState("Idle_Face2") || IsAnimState("Idle_Face4")) ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
			}
			else
			{
				PlayFaceAnim((IsAnimState("Idle_Face1") || IsAnimState("Idle_Face4")) ? 2 : 0);
			}
		}
		else if (PM.Base.GetPrefab("princess"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face1") ? 1 : (IsAnimState("Idle_Face2") ? 2 : 0));
		}
		else if (PM.Base.GetPrefab("tails"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face3") ? 2 : 0);
		}
		else if (PM.Base.GetPrefab("silver"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face2") ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
		}
		else if (PM.Base.GetPrefab("shadow"))
		{
			PlayFaceAnim((IsAnimState("Idle_Face1") || IsAnimState("Idle_Face3")) ? 2 : (IsAnimState("Idle_Face2") ? 1 : 0));
		}
		else if (PM.Base.GetPrefab("rouge"))
		{
			PlayFaceAnim(0);
		}
		else if (PM.Base.GetPrefab("knuckles"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face1") ? 1 : (IsAnimState("Idle_Face2") ? 2 : 0));
		}
		else if (PM.Base.GetPrefab("blaze"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face1") ? 2 : (IsAnimState("Idle_Face3") ? 1 : 0));
		}
		else if (PM.Base.GetPrefab("amy"))
		{
			PlayFaceAnim(IsAnimState("Idle_Face4") ? 4 : 0);
		}
	}

	private bool IsAnimState(string StateName)
	{
		if (PM.Base.Animator.GetCurrentAnimatorStateInfo(1).IsName(StateName))
		{
			return true;
		}
		return false;
	}

	public void PlayFaceAnim(int Index)
	{
		PlayAnimation(Index);
	}

	private void PlayAnimation(int AnimIndex)
	{
		PM.Base.Animator.SetInteger("Face Index", AnimIndex);
	}
}
