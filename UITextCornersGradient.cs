using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text 4 Corners Gradient")]
public class UITextCornersGradient : BaseMeshEffect
{
	public Color m_topLeftColor = Color.white;

	public Color m_topRightColor = Color.white;

	public Color m_bottomRightColor = Color.white;

	public Color m_bottomLeftColor = Color.white;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (base.enabled)
		{
			_ = base.graphic.rectTransform.rect;
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				Vector2 t = UIGradientUtils.VerticePositions[i % 4];
				ref Color32 color = ref vertex.color;
				color *= UIGradientUtils.Bilerp(m_bottomLeftColor, m_bottomRightColor, m_topLeftColor, m_topRightColor, t);
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
