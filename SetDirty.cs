using UnityEngine;
using UnityEngine.UI;

public class SetDirty : MonoBehaviour
{
	public Graphic m_graphic;

	private void Reset()
	{
		m_graphic = GetComponent<Graphic>();
	}

	private void Update()
	{
		m_graphic.SetVerticesDirty();
	}
}
