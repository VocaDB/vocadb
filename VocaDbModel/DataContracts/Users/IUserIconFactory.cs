#nullable disable

using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Provides URL to user's profile icon.
	/// 
	/// Gravatar icons are created based on email address, but most of the time we don't
	/// want to send the email address to the client, especially other users' email addresses.
	/// 
	/// This factory can be used to create URL to the profile icon based on user's email address
	/// (or possibly other properties) without revealing the email address.
	/// </summary>
	public interface IUserIconFactory
	{
		/// <summary>
		/// Get multiple icon sizes.
		/// </summary>
		/// <param name="user">User whose icons are to be loaded. Cannot be null.</param>
		/// <param name="sizes">Sizes of icons to get. Note that user icon image sizes are different from other entries.</param>
		/// <returns>User icons. Cannot be null.</returns>
		EntryThumbForApiContract GetIcons(IUserWithEmail user, ImageSizes sizes = ImageSizes.All);
	}
}
