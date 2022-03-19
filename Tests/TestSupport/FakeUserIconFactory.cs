using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Tests.TestSupport
{
	public class FakeUserIconFactory : IUserIconFactory
	{
		public EntryThumbForApiContract GetIcons(IEntryImageInformation user, ImageSizes sizes = ImageSizes.All)
		{
			return new EntryThumbForApiContract();
		}
	}
}
