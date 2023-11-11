using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class BinaryHelper
{
	public static void Seek(this BinaryReader binaryRead, long iLongPos, SeekOrigin m_SeekOrigin = SeekOrigin.Begin)
	{
		binaryRead.BaseStream.Seek(iLongPos, m_SeekOrigin);
	}

	public static void Seek(this BinaryReader binaryRead, int iIntPos, SeekOrigin m_SeekOrigin = SeekOrigin.Begin)
	{
		binaryRead.BaseStream.Seek(iIntPos, m_SeekOrigin);
	}

	public static void Skip(this BinaryReader binaryRead, int count)
	{
		binaryRead.BaseStream.Seek(count, SeekOrigin.Current);
	}

	public static int Tell(this BinaryReader binaryRead)
	{
		return (int)binaryRead.BaseStream.Position;
	}

	public static int ReadInt16At(this BinaryReader binaryRead, int iIntPos)
	{
		binaryRead.BaseStream.Seek(iIntPos, SeekOrigin.Begin);
		return binaryRead.ReadInt16();
	}

	public static int ReadInt32At(this BinaryReader binaryRead, int iIntPos)
	{
		binaryRead.BaseStream.Seek(iIntPos, SeekOrigin.Begin);
		return binaryRead.ReadInt32();
	}

	public static float ReadFloatAt(this BinaryReader binaryRead, int iIntPos)
	{
		binaryRead.BaseStream.Seek(iIntPos, SeekOrigin.Begin);
		return binaryRead.ReadSingle();
	}

	public static int ReadInt32AtBin(this BinaryReader binaryRead, string sBinPos)
	{
		binaryRead.BaseStream.Seek(Convert.ToInt32(sBinPos, 16), SeekOrigin.Begin);
		return binaryRead.ReadInt32();
	}

	public static Vector3 ReadVector3(this BinaryReader binaryRead, bool invert = false)
	{
		Vector3 result = new Vector3(binaryRead.ReadSingle(), binaryRead.ReadSingle(), binaryRead.ReadSingle());
		if (invert)
		{
			result.x = 0f - result.x;
		}
		return result;
	}

	public static Matrix4x4 ReadMatrix4X4(this BinaryReader binaryRead)
	{
		Matrix4x4 result = default(Matrix4x4);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				result[i, j] = binaryRead.ReadFloat();
			}
		}
		return result;
	}

	public static Color32 ReadColorBGRA32(this BinaryReader binaryRead)
	{
		byte b = binaryRead.ReadByte();
		byte g = binaryRead.ReadByte();
		byte r = binaryRead.ReadByte();
		byte a = binaryRead.ReadByte();
		return new Color32(r, g, b, a);
	}

	public static Vector2 ReadVector2(this BinaryReader binaryRead)
	{
		return new Vector2(binaryRead.ReadSingle(), binaryRead.ReadSingle());
	}

	public static bool And(this int flag_1, int flag_2)
	{
		return (flag_1 & flag_2) != 0;
	}

	public static int ReadInt16_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(2);
		Array.Reverse((Array)array);
		return BitConverter.ToInt16(array, 0);
	}

	public static int ReadUInt16_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(2);
		Array.Reverse((Array)array);
		return BitConverter.ToUInt16(array, 0);
	}

	public static int ReadInt32_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array);
		return BitConverter.ToInt32(array, 0);
	}

	public static uint ReadUInt32_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array);
		return BitConverter.ToUInt32(array, 0);
	}

	public static float ReadFloat_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array);
		return BitConverter.ToSingle(array, 0);
	}

	public static float ReadFloat(this BinaryReader binaryRead)
	{
		return binaryRead.ReadSingle();
	}

	public static Vector3 ReadVector3_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array);
		byte[] array2 = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array2);
		byte[] array3 = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array3);
		return new Vector3(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array2, 0), BitConverter.ToSingle(array3, 0));
	}

	public static Quaternion ReadQuaternion_BE(this BinaryReader binaryRead)
	{
		byte[] array = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array);
		byte[] array2 = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array2);
		byte[] array3 = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array3);
		byte[] array4 = binaryRead.ReadBytes(4);
		Array.Reverse((Array)array4);
		return new Quaternion(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array2, 0), BitConverter.ToSingle(array3, 0), BitConverter.ToSingle(array4, 0));
	}

	public static string ReadString_BE(this BinaryReader binaryRead, bool m_FailSafe = false, int m_MaxLength = 256)
	{
		if (m_FailSafe)
		{
			m_MaxLength = (int)Mathf.Min(m_MaxLength, binaryRead.BaseStream.Length - binaryRead.BaseStream.Position);
		}
		byte[] bytes = binaryRead.ReadBytes(m_MaxLength);
		return Encoding.Default.GetString(bytes).Split(default(char))[0];
	}

	public static string ReadString(this BinaryReader binaryRead, int m_MaxLength = 256)
	{
		return Encoding.Default.GetString(binaryRead.ReadBytes(m_MaxLength)).Split(default(char))[0];
	}

	public static string ReadStringFailSafe(this BinaryReader binaryRead, bool m_FailSafe = false, int m_MaxLength = 256)
	{
		if (m_FailSafe)
		{
			m_MaxLength = (int)Mathf.Min(m_MaxLength, binaryRead.BaseStream.Length - binaryRead.BaseStream.Position);
		}
		byte[] bytes = binaryRead.ReadBytes(m_MaxLength);
		return Encoding.Default.GetString(bytes).Split(default(char))[0];
	}

	public static int[] ReadInt32Array(this BinaryReader binaryRead, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = binaryRead.ReadInt32();
		}
		return array;
	}

	public static float[] ReadFloatArray(this BinaryReader binaryRead, int count)
	{
		float[] array = new float[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = binaryRead.ReadSingle();
		}
		return array;
	}

	public static int BinToInt(this string sBinPos)
	{
		return Convert.ToInt32(sBinPos, 16);
	}

	public static string ReadNullTerminatedAsciiString(this BinaryReader binaryRead, int maxLength = int.MaxValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		while (binaryRead.BaseStream.Position != binaryRead.BaseStream.Length)
		{
			byte b = binaryRead.ReadByte();
			if (b == 0)
			{
				break;
			}
			stringBuilder.Append((char)b);
		}
		return stringBuilder.ToString();
	}
}
