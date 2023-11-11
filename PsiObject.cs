using UnityEngine;

public class PsiObject : ObjectBase
{
	internal int PsiThrowDamage = 1;

	private MaterialPropertyBlock PropBlock;

	public virtual void Awake()
	{
		PropBlock = new MaterialPropertyBlock();
	}

	public void OnPsiFX(Renderer Renderer, bool Conditions, int Index = int.MaxValue)
	{
		if (Index == int.MaxValue)
		{
			Renderer.GetPropertyBlock(PropBlock);
			PropBlock.SetColor("_ExtFresColor", new Vector4(0f, 255f, 216f, 1f));
			PropBlock.SetFloat("_ExtFresPower", 2f);
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), Conditions ? 0.005f : 0f, Time.deltaTime * 10f));
			PropBlock.SetColor("_OutlineColor", new Vector4(0f, 255f, 168f, 1f));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", Conditions ? 1f : 0f);
			Renderer.SetPropertyBlock(PropBlock);
		}
		else
		{
			Renderer.GetPropertyBlock(PropBlock, Index);
			PropBlock.SetColor("_ExtFresColor", new Vector4(0f, 255f, 216f, 1f));
			PropBlock.SetFloat("_ExtFresPower", 2f);
			PropBlock.SetFloat("_ExtFresThre", Mathf.Lerp(PropBlock.GetFloat("_ExtFresThre"), Conditions ? 0.005f : 0f, Time.deltaTime * 10f));
			PropBlock.SetColor("_OutlineColor", new Vector4(0f, 255f, 168f, 1f));
			PropBlock.SetFloat("_OutlinePulseSpd", 0f);
			PropBlock.SetFloat("_OutlineInt", Conditions ? 1f : 0f);
			Renderer.SetPropertyBlock(PropBlock, Index);
		}
	}
}
