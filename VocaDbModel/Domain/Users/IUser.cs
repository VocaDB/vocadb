namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// Interface for <see cref="User"/> with minimal information.
	/// Contains no sensitive information.
	/// </summary>
	public interface IUser {

		int Id { get; set; }

		string Name { get; set; }

	}
}
