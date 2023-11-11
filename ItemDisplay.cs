using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
	public Image image;

	public Canvas canvas;

	internal int State;

	internal int Padding;

	private float StartPos;

	private float xPadding;

	private float Width;

	private float StartTime;

	private void Start()
	{
		Width = image.rectTransform.sizeDelta.x * canvas.scaleFactor;
		StartPos = image.rectTransform.position.x;
		image.rectTransform.localScale = Vector3.zero;
		StartTime = Time.time;
	}

	private void FixedUpdate()
	{
		xPadding = Mathf.Lerp(xPadding, Width * (float)Padding, Time.fixedDeltaTime * 15f);
		image.rectTransform.position = new Vector3(StartPos - xPadding, image.rectTransform.position.y, 0f);
		if (State == 0)
		{
			image.rectTransform.localScale = Vector3.Lerp(image.rectTransform.localScale, Vector3.one, Time.fixedDeltaTime * 12f);
			if (Time.time - StartTime > 4f)
			{
				State = 1;
				StartTime = Time.time;
			}
		}
		else if (State == 1)
		{
			float num = Time.time - StartTime;
			num *= num;
			image.color -= new Color(0f, 0f, 0f, Time.fixedDeltaTime * 2.5f);
			image.rectTransform.localScale = Vector3.Lerp(image.rectTransform.localScale, Vector3.zero, num * 4f);
			if (image.color.a < 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
