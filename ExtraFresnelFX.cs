using UnityEngine;

public class ExtraFresnelFX : MonoBehaviour
{
	public enum Type
	{
		Timer = 0,
		AnimationCurve = 1
	}

	[Header("Framework")]
	public Type FXBehaviour;

	public Renderer[] Renderers;

	public UpgradeModels Upgrades;

	public Color[] PsiGlows;

	public bool OnlyUseOutline;

	[Header("Timer")]
	public Vector2 ThresholdInts;

	public Vector2 GlowTexInts;

	public float Timer;

	[Header("Animation Curve")]
	public AnimationCurve ThresholdCurve;

	public AnimationCurve GlowTexCurve;

	private float StartTime;

	private MaterialPropertyBlock PropBlock;

	private void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	private void Start()
	{
		StartTime = Time.time;
	}

	private void Update()
	{
		for (int i = 0; i < Renderers.Length; i++)
		{
			UpdateFX(Renderers[i]);
		}
		if ((bool)Upgrades && Upgrades.Renderers != null && Singleton<Settings>.Instance.settings.UpgradeModels == 0)
		{
			for (int j = 0; j < Upgrades.Renderers.Count; j++)
			{
				UpdateFX(Upgrades.Renderers[j]);
			}
		}
	}

	private void UpdateFX(Renderer Rend)
	{
		float num = Time.time - StartTime;
		Rend.GetPropertyBlock(PropBlock);
		if (!OnlyUseOutline)
		{
			PropBlock.SetColor("_ExtFresColor", PsiGlows[0]);
			PropBlock.SetColor("_ExtGlowColor", PsiGlows[1]);
			PropBlock.SetFloat("_ExtPulseSpd", 1f);
			PropBlock.SetFloat("_ExtFresPower", 1f);
		}
		PropBlock.SetColor("_OutlineColor", PsiGlows[0]);
		PropBlock.SetColor("_OutlinePulseColor", PsiGlows[1]);
		PropBlock.SetFloat("_OutlinePulseSpd", 1f);
		if (FXBehaviour == Type.Timer)
		{
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), (num > Timer) ? ThresholdInts.y : ThresholdInts.x, Time.deltaTime * 10f));
			PropBlock.SetFloat("_OutlineInt", Mathf.Lerp(PropBlock.GetFloat("_OutlineInt"), (num > Timer) ? ThresholdInts.y : ThresholdInts.x, Time.deltaTime * 10f));
			PropBlock.SetFloat("_GlowInt", Mathf.Lerp(PropBlock.GetFloat("_GlowInt"), (num > Timer) ? GlowTexInts.y : GlowTexInts.x, Time.deltaTime * 10f));
		}
		else
		{
			PropBlock.SetFloat("_ExtFresThre", ThresholdCurve.Evaluate(Time.time - StartTime));
			PropBlock.SetFloat("_OutlineInt", ThresholdCurve.Evaluate(Time.time - StartTime));
			PropBlock.SetFloat("_GlowInt", GlowTexCurve.Evaluate(Time.time - StartTime));
		}
		Rend.SetPropertyBlock(PropBlock);
	}
}
