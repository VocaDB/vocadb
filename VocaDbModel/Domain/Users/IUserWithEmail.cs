namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// Interface for <see cref="User"/> with email.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary> 
	public interface IUserWithEmail : IUser
	{
		/// <summary>
		/// Email address. Can be empty. Cannot be null.
		/// Sensitive information, must not be sent to other users besides the user himself.
		/// </summary>
		string Email { get; }
	}
}
