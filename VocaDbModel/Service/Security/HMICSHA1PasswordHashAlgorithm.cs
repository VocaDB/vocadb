using System;
using System.Security.Cryptography;

namespace VocaDb.Model.Service.Security {

	/// <summary>
	/// Password hashing using HMAC-SHA1.
	/// </summary>
	/// <remarks>
	/// HMIC = HMAC with 3939 iterations.
	/// </remarks>
	public class HMICSHA1PasswordHashAlgorithm : IPasswordHashAlgorithm {

		public PasswordHashAlgorithmType AlgorithmType => PasswordHashAlgorithmType.HMACSHA1;

		public string GenerateSalt() {
			var bytes = new byte[39];
			new RNGCryptoServiceProvider().GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		public string HashPassword(string password, string salt) {

			var saltBytes = Convert.FromBase64String(salt);
			var algo = new Rfc2898DeriveBytes(password, saltBytes, 3939);
			var hashed = algo.GetBytes(20);

			return Convert.ToBase64String(hashed);

		}

		public string HashPassword(string password, string salt, string username) {
			return HashPassword(password, salt);
		}

	}
}
