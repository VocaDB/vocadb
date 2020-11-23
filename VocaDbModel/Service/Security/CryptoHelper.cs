using System;
using System.Security.Cryptography;
using System.Text;

namespace VocaDb.Model.Service.Security
{
	public static class CryptoHelper
	{
		public const string MD5 = "MD5";
		public const string SHA1 = "SHA1";

		/// <summary>
		/// From http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
		/// </summary>
		/// <param name="ba"></param>
		/// <returns></returns>
		private static string ByteArrayToString(byte[] ba)
		{
			string hex = BitConverter.ToString(ba);
			return hex.Replace("-", "");
		}

		public static string HashSHA1(string str)
		{
			return HashString(str, SHA1);
		}

		/// <summary>
		/// From http://carson63000-tech.blogspot.com/2010/08/dont-use-formsauthenticationhashpasswor_11.html
		/// </summary>
		/// <param name="inputString">Input string. Cannot be null.</param>
		/// <param name="hashName">Hash algorithm.</param>
		/// <returns>Hashed string. Cannot be null.</returns>
		public static string HashString(string inputString, string hashName)
		{
			ParamIs.NotNull(() => inputString);

			var algorithm = HashAlgorithm.Create(hashName);
			if (algorithm == null)
			{
				throw new ArgumentException("Unrecognized hash name", "hashName");
			}

			byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
			return ByteArrayToString(hash);
		}
	}
}
