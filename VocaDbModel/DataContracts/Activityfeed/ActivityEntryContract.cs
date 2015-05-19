using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		private string GetArtistString(IEntryBase entry, ContentLanguagePreference languagePreference) {

			return EntryBaseHelper.GetArtistString(entry, languagePreference);

		}

		private string GetMime(IEntryBase entry) {

			return EntryBaseHelper.GetMime(entry);

		}

		private string GetSongThumbUrl(IEntryBase entry) {
			return EntryBaseHelper.GetSongThumbUrl(entry);
		}

		public ActivityEntryContract(ActivityEntry entry, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => entry);

			ArtistString = GetArtistString(entry.EntryBase, languagePreference);
			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryWithImageContract(entry.EntryBase, GetMime(entry.EntryBase), GetSongThumbUrl(entry.EntryBase), languagePreference);

		}

		public ActivityEntryContract(ActivityEntry entry, ContentLanguagePreference languagePreference, string changeMessage, string entryTypeName,
			bool getArchivedVersion) {

			ParamIs.NotNull(() => entry);

			ArtistString = GetArtistString(entry.EntryBase, languagePreference);
			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryWithImageContract(entry.EntryBase, GetMime(entry.EntryBase), GetSongThumbUrl(entry.EntryBase), languagePreference);
			EntryTypeName = entryTypeName;

			if (getArchivedVersion && entry.ArchivedVersionBase != null)
				ArchivedVersion = new ArchivedVersionForReviewContract(entry.ArchivedVersionBase, changeMessage, languagePreference);

		}

		public ActivityEntryContract(ArchivedObjectVersion entry, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => entry);

			ArtistString = GetArtistString(entry.EntryBase, languagePreference);
			Author = new UserContract(entry.Author);
			CreateDate = entry.Created;
			EditEvent = entry.EditEvent;
			EntryRef = new EntryWithImageContract(entry.EntryBase, GetMime(entry.EntryBase), GetSongThumbUrl(entry.EntryBase), languagePreference);

		}

		public ArchivedVersionForReviewContract ArchivedVersion { get; set; }

		public string ArtistString { get; set; }

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public EntryWithImageContract EntryRef { get; set; }

		public string EntryTypeName { get; set; }

	}

}
