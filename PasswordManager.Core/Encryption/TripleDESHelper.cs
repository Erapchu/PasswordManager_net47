using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Encryption
{
	public static class TripleDESHelper
	{
		#region Fields
		private static readonly string Key = "569488476345975095007442";
		private static readonly string IV = "89045003";
		private const int iLen = 24;
		#endregion

		#region Encryption Methods
		public static string EncryptString(string input)
		{
			try
			{
				TripleDES threedes = new TripleDESCryptoServiceProvider();
				threedes.Key = StringToByte(Key, iLen);
				threedes.IV = StringToByte(IV);
				byte[] key = threedes.Key;
				byte[] iv = threedes.IV;
				ICryptoTransform encryptor = threedes.CreateEncryptor(key, iv);
				MemoryStream msEncrypt = new MemoryStream();
				CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

				byte[] decrypted = StringToByte(input);

				csEncrypt.Write(decrypted, 0, decrypted.Length);
				csEncrypt.FlushFinalBlock();

				byte[] encrypted = msEncrypt.ToArray();
				return ByteToString(encrypted);
			}
			catch (Exception)
			{
				return input;
			}

		}
		public static string DecryptString(string text)
		{
			try
			{
				TripleDES threedes = new TripleDESCryptoServiceProvider();

				threedes.Key = StringToByte(Key, iLen); // convert to 24 characters - 192 bits
				threedes.IV = StringToByte(IV);
				byte[] key = threedes.Key;
				byte[] iv = threedes.IV;

				ICryptoTransform decryptor = threedes.CreateDecryptor(key, iv);

				byte[] encrypted = HexStringToByte(text);

				MemoryStream msDecrypt = new MemoryStream(encrypted);
				CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

				return ByteToString(csDecrypt);
			}
			catch (Exception)
			{
				return text;
			}
		}
		#endregion

		#region Helpers
		private static string ByteToString(CryptoStream csDecrypt)
		{
			string binary = "";
			int b = 0;
			//while (b != -1)
			//{
			//	b = csDecrypt.ReadByte();
			//	binary += (char)b;
			//}
			do
			{
				b = csDecrypt.ReadByte();
				if (b != -1) binary += ((char)b);

			} while (b != -1);
			return binary;
		}

		private static string ByteToString(byte[] encrypted)
		{
			string binary = "";
			for (int i = 0; i < encrypted.Length; i++)
			{
				binary += encrypted[i].ToString("X2");
			}
			return binary;
		}

		private static byte[] HexStringToByte(string input)
		{
			char[] chars = input.ToCharArray();
			byte[] bytes = new byte[chars.Length / 2];
			for (int i = 0, j = 0; i < chars.Length; i += 2, j++)
			{
				bytes[j] = Convert.ToByte("" + chars[i] + chars[i + 1], 16);
			}
			return bytes;
		}
		private static byte[] StringToByte(string input)
		{

			char[] chars = input.ToCharArray();
			byte[] bytes = new byte[chars.Length];
			for (int i = 0; i < chars.Length; i++)
			{
				bytes[i] = Convert.ToByte(chars[i]);
			}
			return bytes;
		}
		private static byte[] StringToByte(string input, int length)
		{
			char[] chars = input.ToCharArray();
			byte[] bytes = new byte[length];
			for (int i = 0; i < chars.Length; i++)
			{
				bytes[i] = Convert.ToByte(chars[i]);
			}
			return bytes;
		}
		#endregion
	}
}
