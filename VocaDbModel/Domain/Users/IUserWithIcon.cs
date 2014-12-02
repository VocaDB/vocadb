namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Interface for <see cref="User"/> with profile icon URL.
	/// Safe to be sent to client.
	/// </summary> 
	public interface IUserWithIcon : IUser {

		/// <summary>
		/// Full URL to user profile icon. 
		/// Can be null or empty if there is no profile icon.
		/// </summary>
		string IconUrl { get; }

	}

}
