using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Code
{
	public class VocaDbUserIconFactory : IUserIconFactory
	{
		private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
		private readonly IUserPermissionContext _permissionContext;

		public VocaDbUserIconFactory(IAggregatedEntryImageUrlFactory thumbPersister, IUserPermissionContext permissionContext)
		{
			_thumbPersister = thumbPersister;
			_permissionContext = permissionContext;
		}

		public EntryThumbForApiContract? GetIcons(IEntryImageInformation user, ImageSizes sizes = ImageSizes.All)
		{
			return EntryThumbForApiContract.Create(user, _thumbPersister, sizes);
		}

		public EntryThumbForApiContract? GetUserIcons(User user, ImageSizes sizes = ImageSizes.All)
		{
			// Hide disabled users' avatars from those who don't have the DisableUsers permission.
			if (!user.Active && !_permissionContext.HasPermission(PermissionToken.DisableUsers))
				return null;

			return GetIcons(user, sizes);
		}
	}
}
