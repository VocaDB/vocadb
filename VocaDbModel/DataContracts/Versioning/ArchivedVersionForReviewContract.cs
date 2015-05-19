using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Versioning {

	public class ArchivedVersionForReviewContract {

		public ArchivedVersionForReviewContract(ArchivedObjectVersion archivedVersion, string changeMessage, 
			ContentLanguagePreference languagePreference) {
			
			EditEvent = archivedVersion.EditEvent;
			Entry = EntryBaseHelper.GetEntryWithImageContract(archivedVersion.EntryBase, languagePreference);
			Notes = archivedVersion.Notes;
			Author = archivedVersion.Author != null ? new UserContract(archivedVersion.Author) : null;
			CreateDate = archivedVersion.Created;
			ChangeMessage = changeMessage;
			ArtistString = EntryBaseHelper.GetArtistString(archivedVersion.EntryBase, languagePreference);
			VersionId = archivedVersion.Id;

		}

		public string ArtistString { get; set; }

		public UserContract Author { get; set; }

		public string ChangeMessage { get; set; }

		public DateTime CreateDate { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public EntryWithImageContract Entry { get; set; }

		public string Notes { get; set; }

		public int VersionId { get; set; }

	}

}
