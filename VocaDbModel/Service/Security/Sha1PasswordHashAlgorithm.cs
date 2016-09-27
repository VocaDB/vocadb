using System;

namespace VocaDb.Model.Service.Security {

	public class SHA1PasswordHashAlgorithm : IPasswordHashAlgorithm {

		public PasswordHashAlgorithmType AlgorithmType => PasswordHashAlgorithmType.SHA1;

		public string GenerateSalt() {
			return new Random().Next().ToString();
		}

		public string HashPassword(string password, string salt, string username) {
			return CryptoHelper.HashSHA1(username + password + salt);
		}

	}

}
