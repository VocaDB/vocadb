namespace VocaDb.Model.Service.Security {

	/// <summary>
	/// Algorithm for password hashing.
	/// </summary>
	public interface IPasswordHashAlgorithm {

		/// <summary>
		/// Algorithm type used for identification.
		/// </summary>
		PasswordHashAlgorithmType AlgorithmType { get; }

		/// <summary>
		/// Generates a new salt.
		/// </summary>
		string GenerateSalt();

		/// <summary>
		/// Generates a hash based on a plaintext password.
		/// </summary>
		/// <param name="password">Plaintext password, for example "MikuMiku39".</param>
		/// <param name="salt">Password salt, originally generated with <see cref="GenerateSalt"/>.</param>
		/// <param name="username">Username, for example "Miku".</param>
		/// <returns>Hashed password, ready to be stored in the database.</returns>
		string HashPassword(string password, string salt, string username);
		
	}

}
