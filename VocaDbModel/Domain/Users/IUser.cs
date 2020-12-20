#nullable disable

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// Interface for <see cref="User"/> with minimal information.
	/// Contains no sensitive information.
	/// </summary>
	public interface IUser
	{
		int Id { get; set; }

		string Name { get; set; }
	}

	public static class IUserExtensions
	{
		public static bool IsTheSameUser(this IUser left, IUser right)
		{
			ParamIs.NotNull(() => left);

			if (right == null)
				return false;

			return left.Id == right.Id;
		}
	}
}
