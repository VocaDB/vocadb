using System.Collections.Generic;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{
	public enum ShowRolesMode
	{
		Never,
		/// <summary>
		/// Show roles if role is not "Default"
		/// </summary>
		IfNotDefault,
		/// <summary>
		/// Show roles if role is not "Default" or "Vocalist"
		/// </summary>
		IfNotVocalist
	}

	public class ArtistListViewModel
	{
		public ArtistListViewModel(IEnumerable<IArtistLinkContract> artists, ShowRolesMode showRoles = ShowRolesMode.Never, bool showType = false)
		{
			Artists = artists;
			ShowRoles = showRoles;
			ShowType = showType;
		}

		public IEnumerable<IArtistLinkContract> Artists { get; set; }

		public ShowRolesMode ShowRoles { get; set; }

		public bool ShowType { get; set; }
	}
}