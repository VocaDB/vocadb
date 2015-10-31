using Microsoft.Web.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Code {

	/// <summary>
	/// Generates Gravatar URLs for user profile icons.
	/// </summary>
	public class GravatarUserIconFactory : IUserIconFactory {

		private readonly int size;
		private readonly bool ssl;

		private string GetUrl(IUserWithEmail user, int sizePx) {
			return Gravatar.GetUrl(user.Email, sizePx, scheme: ssl ? "https" : "http");
        }

		public GravatarUserIconFactory(int size = ImageHelper.UserThumbSize, bool ssl = false) {
			this.size = size;
			this.ssl = ssl;
		}

		public string GetIconUrl(IUserWithEmail user) {

			if (string.IsNullOrEmpty(user.Email))
				return string.Empty;

			return GetUrl(user, size);

		}

		public EntryThumbForApiContract GetIcons(IUserWithEmail user, ImageSizes sizes = ImageSizes.All) {

			var contract = new EntryThumbForApiContract();

			if (string.IsNullOrEmpty(user.Email))
				return contract;

			if (sizes.HasFlag(ImageSizes.Thumb)) {
				contract.UrlThumb = GetUrl(user, ImageHelper.UserThumbSize);
			}

			if (sizes.HasFlag(ImageSizes.SmallThumb)) {
				contract.UrlSmallThumb = GetUrl(user, ImageHelper.UserSmallThumbSize);
			}

			if (sizes.HasFlag(ImageSizes.TinyThumb)) {
				contract.UrlTinyThumb = GetUrl(user, ImageHelper.UserTinyThumbSize);
			}

			return contract;
			
		}
	}

}