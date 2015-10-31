using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport {

	public class FakeUserIconFactory : IUserIconFactory {

		public string GetIconUrl(IUserWithEmail user) {
			return string.Empty;
		}

		public EntryThumbForApiContract GetIcons(IUserWithEmail user, ImageSizes sizes = ImageSizes.All) {
			return new EntryThumbForApiContract();
		}

	}
}
