using System;
using System.Security.Cryptography;

namespace VocaDb.Model.Service.Security {

	public class HMACSHA1PasswordHashAlgorithm : IPasswordHashAlgorithm {

		public PasswordHashAlgorithmType AlgorithmType => PasswordHashAlgorithmType.HMACSHA1;

		public string GenerateSalt() {
			var bytes = new byte[32];
			new RNGCryptoServiceProvider().GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		public string HashPassword(string password, string salt, string username) {

			var saltBytes = Convert.FromBase64String(salt);
			var algo = new Rfc2898DeriveBytes(password, saltBytes);
			var hashed = algo.GetBytes(20);

			return Convert.ToBase64String(hashed);

		}

	}
}
