using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text Gradient")]
public class UITextGradient : BaseMeshEffect
{
	public Color m_color1 = Color.white;

	public Color m_color2 = Color.white;

	[Range(-180f, 180f)]
	public float m_angle;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (base.enabled)
		{
			_ = base.graphic.rectTransform.rect;
			Vector2 dir = UIGradientUtils.RotationDir(m_angle);
			UIGradientUtils.Matrix2x3 matrix2x = UIGradientUtils.LocalPositionMatrix(new Rect(0f, 0f, 1f, 1f), dir);
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				Vector2 vector = UIGradientUtils.VerticePositions[i % 4];
				Vector2 vector2 = matrix2x * vector;
				ref Color32 color = ref vertex.color;
				color *= Color.Lerp(m_color2, m_color1, vector2.y);
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
