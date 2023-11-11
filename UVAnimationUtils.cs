using UnityEngine;

public class UVAnimationUtils : MonoBehaviour
{
	[Header("Framework")]
	public Material[] Materials;

	[Header("Texture Animation")]
	public bool UseTextureAnimation;

	public string TextureInputName;

	public Texture[] Textures;

	public float Speed;

	public bool Loop;

	public float ResetTime;

	[Header("Offset Animation")]
	public bool UseOffsetAnimation;

	public string OffsetInputName;

	public AnimationCurve XOffsetCurve;

	public AnimationCurve YOffsetCurve;

	[Header("Alpha Animation")]
	public bool UseAlphaAnimation;

	public string AlphaInputName;

	public AnimationCurve AlphaCurve;

	private float Timer;

	private float ResetTimer;

	private int ArrayPosition;

	private void Start()
	{
		Timer = Time.time;
		ResetTimer = Time.time;
	}

	private void Update()
	{
		if (Materials == null)
		{
			return;
		}
		for (int i = 0; i < Materials.Length; i++)
		{
			if (Textures != null && UseTextureAnimation)
			{
				if (!Loop && Time.time - ResetTimer > ResetTime)
				{
					ResetTimer = Time.time;
					Timer = Time.time;
					ArrayPosition = 0;
				}
				if (Time.time - Timer >= Speed)
				{
					Timer = Time.time;
					if (Loop)
					{
						if (ArrayPosition >= Textures.Length - 1)
						{
							ArrayPosition = 0;
						}
						else
						{
							ArrayPosition++;
						}
					}
					else if (ArrayPosition < Textures.Length - 1)
					{
						ArrayPosition++;
					}
					Materials[i].SetTexture(TextureInputName, Textures[ArrayPosition]);
				}
			}
			if (UseOffsetAnimation)
			{
				Materials[i].SetTextureOffset(OffsetInputName, new Vector2(XOffsetCurve.Evaluate(Time.time), YOffsetCurve.Evaluate(Time.time)));
			}
			if (UseAlphaAnimation)
			{
				Color32 color = Materials[i].GetColor(AlphaInputName);
				color.a = (byte)AlphaCurve.Evaluate(Time.time);
				Materials[i].SetColor(AlphaInputName, color);
			}
		}
	}
}
