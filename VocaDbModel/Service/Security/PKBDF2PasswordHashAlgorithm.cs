#nullable disable

using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VocaDb.Model.Service.Security;

/// <summary>
/// Password hashing using HMAC-SHA1.
/// </summary>
/// <remarks>
/// HMIC = HMAC with 3939 iterations.
/// </remarks>
public class PKBDF2PasswordHashAlgorithm : IPasswordHashAlgorithm
{
	public PasswordHashAlgorithmType AlgorithmType => PasswordHashAlgorithmType.PKBDF2;

	public string GenerateSalt()
	{
		byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
		return Convert.ToBase64String(salt);
	}

	public string HashPassword(string password, string salt)
	{
		var saltBytes = Convert.FromBase64String(salt);

		var hashed = KeyDerivation.Pbkdf2(
			password: password,
			salt: saltBytes,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 600000,
			numBytesRequested: 256 / 8
		);

		return Convert.ToBase64String(hashed);
	}

	public string HashPassword(string password, string salt, string username)
	{
		return HashPassword(password, salt);
	}
}
