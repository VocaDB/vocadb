using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Code
{
	public class VocaDbUserIconFactory : IUserIconFactory
	{
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;

		public VocaDbUserIconFactory(IAggregatedEntryImageUrlFactory thumbPersister)
		{
			_thumbPersister = thumbPersister;
		}

		public EntryThumbForApiContract? GetIcons(IEntryImageInformation user, ImageSizes sizes = ImageSizes.All)
		{
			return EntryThumbForApiContract.Create(user, _thumbPersister, sizes);
		}
	}
}
