#nullable disable

namespace VocaDb.Model.Service.Security;

public static class PasswordHashAlgorithms
{
	/// <summary>
	/// Default (recommended) password hashing algorithm.
	/// To be used for all new passwords.
	/// </summary>
	public static IPasswordHashAlgorithm Default => Get(PasswordHashAlgorithmType.PKBDF2);

	public static IPasswordHashAlgorithm Get(PasswordHashAlgorithmType algorithm) => algorithm switch
	{
		PasswordHashAlgorithmType.SHA1 => new SHA1PasswordHashAlgorithm(),
		PasswordHashAlgorithmType.HMACSHA1 => new HMICSHA1PasswordHashAlgorithm(),
		PasswordHashAlgorithmType.PKBDF2 => new PKBDF2PasswordHashAlgorithm(),
		_ => null,
	};
}

public enum PasswordHashAlgorithmType
{
	Nothing,
	SHA1,
	HMACSHA1,
	PKBDF2
}
