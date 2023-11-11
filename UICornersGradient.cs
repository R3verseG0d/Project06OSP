using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/4 Corners Gradient")]
public class UICornersGradient : BaseMeshEffect
{
	public Color m_topLeftColor = Color.white;

	public Color m_topRightColor = Color.white;

	public Color m_bottomRightColor = Color.white;

	public Color m_bottomLeftColor = Color.white;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (base.enabled)
		{
			UIGradientUtils.Matrix2x3 matrix2x = UIGradientUtils.LocalPositionMatrix(base.graphic.rectTransform.rect, Vector2.right);
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				Vector2 t = matrix2x * vertex.position;
				ref Color32 color = ref vertex.color;
				color *= UIGradientUtils.Bilerp(m_bottomLeftColor, m_bottomRightColor, m_topLeftColor, m_topRightColor, t);
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
