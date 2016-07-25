using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models.Artist {

	[PropertyModelBinder]
	public class ArtistEditViewModel {

		public ArtistEditViewModel() {

			AllArtistTypes = Translate.ArtistTypeNames.GetValuesAndNamesStrings(AppConfig.ArtistTypes);

		}

		public ArtistEditViewModel(ArtistContract artist, IUserPermissionContext permissionContext,
			bool canDelete,
			ArtistForEditContract editedArtist = null)
			: this() {

			ParamIs.NotNull(() => artist);

			Artist = artist;
			CanDelete = canDelete;
			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext, artist).ToArray();
			EditedArtist = editedArtist;

		}

		public Dictionary<string, string> AllArtistTypes { get; set; }

		public ArtistContract Artist { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		private IEnumerable<ArtistLinkType> AllowedLinkTypes => EnumVal<ArtistLinkType>.Values.Where(t => t != ArtistLinkType.Group && t != ArtistLinkType.VoiceProvider);

		public Dictionary<string, string> AssociatedArtistTypes => Translate.ArtistLinkTypeNames.GetValuesAndNamesStrings(AllowedLinkTypes);

		public bool CanDelete { get; set; }

		[FromJson]
		public ArtistForEditContract EditedArtist { get; set; }

		public void CheckModel() {
			
			if (EditedArtist == null)
				throw new InvalidFormException("Model was null");

			if (EditedArtist.Names == null)
				throw new InvalidFormException("Names list was null"); // Shouldn't be null

			if (EditedArtist.Pictures == null)
				throw new InvalidFormException("Pictures list was null"); // Shouldn't be null

			if (EditedArtist.WebLinks == null)
				throw new InvalidFormException("Weblinks list was null"); // Shouldn't be null

		}

	}

}