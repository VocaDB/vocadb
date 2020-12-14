#nullable disable

namespace VocaDb.Model.Service.Security
{
	public static class PasswordHashAlgorithms
	{
		/// <summary>
		/// Default (recommended) password hashing algorithm.
		/// To be used for all new passwords.
		/// </summary>
		public static IPasswordHashAlgorithm Default => Get(PasswordHashAlgorithmType.HMACSHA1);

		public static IPasswordHashAlgorithm Get(PasswordHashAlgorithmType algorithm)
		{
			switch (algorithm)
			{
				case PasswordHashAlgorithmType.SHA1:
					return new SHA1PasswordHashAlgorithm();
				case PasswordHashAlgorithmType.HMACSHA1:
					return new HMICSHA1PasswordHashAlgorithm();
			}
			return null;
		}
	}

	public enum PasswordHashAlgorithmType
	{
		Nothing,
		SHA1,
		HMACSHA1
	}
}
