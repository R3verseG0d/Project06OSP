using System;
using System.IO;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
	[Header("Framework")]
	public Camera[] Cameras;

	public Vector2 Resolution;

	public bool NoAlpha;

	public KeyCode Key = KeyCode.F9;

	private int ScreenshotCount;

	private void Start()
	{
		if (Resolution == Vector2.zero)
		{
			Resolution.Set(1920f, 1080f);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(Key))
		{
			_ = string.Empty;
			string empty = string.Empty;
			string empty2 = string.Empty;
			do
			{
				ScreenshotCount++;
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				empty = "screenshot" + ScreenshotCount + ".png";
				empty2 = folderPath + "/" + empty;
				Debug.Log(empty2);
			}
			while (File.Exists(empty2));
			TakeScreenShot(Cameras, (int)Resolution.x, (int)Resolution.y, empty2);
		}
	}

	private void TakeScreenShot(Camera[] Cams, int Width, int Height, string Path)
	{
		foreach (Camera camera in Cams)
		{
			RenderTexture temp = (camera.targetTexture = (RenderTexture.active = RenderTexture.GetTemporary(Width, Height, 24, RenderTextureFormat.Default)));
			camera.Render();
			Texture2D texture2D = new Texture2D(Width, Height, NoAlpha ? TextureFormat.RGB24 : TextureFormat.RGBA32, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, Width, Height), 0, 0);
			texture2D.Apply();
			camera.targetTexture = null;
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(temp);
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(Path, bytes);
			UnityEngine.Object.Destroy(texture2D);
		}
	}

	private void Reset()
	{
		Resolution = new Vector2(1920f, 1080f);
	}
}
