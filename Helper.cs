using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Helper
{
	public static byte[] ReadAllBytes(this BinaryReader Reader)
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			byte[] array = new byte[4096];
			int count;
			while ((count = Reader.Read(array, 0, array.Length)) != 0)
			{
				memoryStream.Write(array, 0, count);
			}
			return memoryStream.ToArray();
		}
	}

	public static Vector3 RotateAround(this Vector3 Position, Vector3 Point, Vector3 Axis, float Angle)
	{
		Quaternion quaternion = Quaternion.AngleAxis(Angle, Axis);
		Vector3 vector = Position - Point;
		vector = quaternion * vector;
		return Point + vector;
	}

	public static Vector3 MakePlanar(this Vector3 Direction)
	{
		Direction.y = 0f;
		return Direction.normalized;
	}

	public static int ToInt(this string String)
	{
		if (int.TryParse(String, out var result))
		{
			return result;
		}
		return -1;
	}

	public static bool IsValid(this Quaternion Quat)
	{
		bool num = float.IsNaN(Quat.x + Quat.y + Quat.z + Quat.w);
		bool flag = Quat.x == 0f && Quat.y == 0f && Quat.z == 0f && Quat.w == 0f;
		return !(num || flag);
	}

	public static Transform FindInChildren(this Transform InTransform, string InName)
	{
		foreach (Transform item in InTransform)
		{
			if (item.name == InName)
			{
				return item;
			}
		}
		return null;
	}

	public static GameObject FindInChildren(this GameObject InTransform, string InName)
	{
		foreach (Transform item in InTransform.transform)
		{
			if (item.name == InName)
			{
				return item.gameObject;
			}
		}
		return null;
	}

	public static Transform FindInChildren(this Transform InTransform, string InName, bool Recursive = false)
	{
		foreach (Transform item in InTransform)
		{
			if (item.name == InName)
			{
				return item;
			}
		}
		if (Recursive)
		{
			foreach (Transform item2 in InTransform)
			{
				Transform transform2 = item2.FindInChildren(InName, Recursive: true);
				if (transform2 != null)
				{
					return transform2;
				}
			}
		}
		return null;
	}

	public static GameObject InstantiatePrefab(this GameObject LoadedPrefab, string Path, Vector3 Position = default(Vector3))
	{
		if (LoadedPrefab == null)
		{
			LoadedPrefab = Resources.Load<GameObject>(Path);
		}
		return Object.Instantiate(LoadedPrefab, Position, Quaternion.identity);
	}

	public static void SaveClass<T>(this T _Class, string FilePath)
	{
		FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
		new BinaryFormatter().Serialize(fileStream, _Class);
		fileStream.Close();
	}

	public static T LoadClass<T>(this T _Class, string FilePath) where T : class
	{
		if (!File.Exists(FilePath))
		{
			return null;
		}
		FileStream fileStream = new FileStream(FilePath, FileMode.Open);
		_Class = new BinaryFormatter().Deserialize(fileStream) as T;
		fileStream.Close();
		return _Class;
	}

	public static T LoadClass<T>(string FilePath) where T : class
	{
		if (!File.Exists(FilePath))
		{
			return null;
		}
		FileStream fileStream = new FileStream(FilePath, FileMode.Open);
		T result = new BinaryFormatter().Deserialize(fileStream) as T;
		fileStream.Close();
		return result;
	}

	public static void DeleteClass(string FilePath)
	{
		if (File.Exists(FilePath))
		{
			File.Delete(FilePath);
		}
	}

	public static bool IsExtention(this string Path, string Ext)
	{
		string[] array = Path.Split('.');
		return array[array.Length - 1].ToLower().Equals(Ext);
	}

	public static void Skip(this StringReader SReader, int Count)
	{
		for (int i = 0; i < Count; i++)
		{
			SReader.Read();
		}
	}

	public static string GetValue(this Dictionary<string, string> _Dictionary, string Key, Dictionary<string, string> OptDictionary)
	{
		if (_Dictionary.TryGetValue(Key, out var value))
		{
			return value;
		}
		if (OptDictionary != null && OptDictionary.TryGetValue(Key, out value))
		{
			return value;
		}
		return "Key not found.";
	}
}
