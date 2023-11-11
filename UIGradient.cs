using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class UIGradient : BaseMeshEffect
{
	public Color m_color1 = Color.white;

	public Color m_color2 = Color.white;

	[Range(-180f, 180f)]
	public float m_angle;

	public bool m_ignoreRatio = true;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (base.enabled)
		{
			Rect rect = base.graphic.rectTransform.rect;
			Vector2 dir = UIGradientUtils.RotationDir(m_angle);
			if (!m_ignoreRatio)
			{
				dir = UIGradientUtils.CompensateAspectRatio(rect, dir);
			}
			UIGradientUtils.Matrix2x3 matrix2x = UIGradientUtils.LocalPositionMatrix(rect, dir);
			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);
				Vector2 vector = matrix2x * vertex.position;
				ref Color32 color = ref vertex.color;
				color *= Color.Lerp(m_color2, m_color1, vector.y);
				vh.SetUIVertex(vertex, i);
			}
		}
	}
}
