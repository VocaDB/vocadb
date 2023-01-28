using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Helpers;

public static class UserHelper
{
	public static int GetLevel(int power)
	{
		if (power <= 0)
			return 0;

		return (int)Math.Log(power, Math.E);
	}

	public static int GetPower(ServerOnlyUserDetailsContract detailsContract, int ownedAlbumCount, int albumRatingCount, int songListCount)
	{
		ParamIs.NotNull(() => detailsContract);

		var power =
			detailsContract.EditCount / 2
			+ detailsContract.SubmitCount / 2
			+ detailsContract.TagVotes * 2
			+ detailsContract.AlbumCollectionCount * 2
			+ ownedAlbumCount * 2
			+ albumRatingCount * 3
			+ detailsContract.FavoriteSongCount
			+ detailsContract.CommentCount * 5
			+ songListCount * 5
			+ (detailsContract.EmailVerified ? 100 : 0);

		return power;
	}

	public static int GetPower(UserDetailsForApiContract contract, int ownedAlbumCount, int albumRatingCount, int songListCount)
	{
		ParamIs.NotNull(() => contract);

		var power =
			contract.EditCount / 2
			+ contract.SubmitCount / 2
			+ contract.TagVotes * 2
			+ contract.AlbumCollectionCount * 2
			+ ownedAlbumCount * 2
			+ albumRatingCount * 3
			+ contract.FavoriteSongCount
			+ contract.CommentCount * 5
			+ songListCount * 5
			+ (contract.EmailVerified ? 100 : 0);

		return power;
	}

	public static bool IsVeteran(ServerOnlyUserDetailsContract details)
	{
		var timeOnSite = DateTime.Now - details.CreateDate;
		return
			details.Active &&
			details.GroupId >= UserGroupId.Regular &&
			timeOnSite.TotalDays > 365 &&
			details.EditCount > 1000;
	}

	public static bool IsVeteran(UserDetailsForApiContract contract)
	{
		var timeOnSite = DateTime.Now - contract.CreateDate;
		return
			contract.Active &&
			contract.GroupId >= UserGroupId.Regular &&
			timeOnSite.TotalDays > 365 &&
			contract.EditCount > 1000;
	}
}
