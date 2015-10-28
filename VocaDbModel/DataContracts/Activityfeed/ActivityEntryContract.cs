using System;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class ActivityEntryContract {

		public ActivityEntryContract(ActivityEntry entry, ContentLanguagePreference languagePreference,
			IEntryThumbPersister entryThumbPersister, IEntryImagePersisterOld entryImagePersisterOld,
			bool ssl) {

			ParamIs.NotNull(() => entry);

			Author = new UserContract(entry.Author);
			CreateDate = entry.CreateDate;
			EditEvent = entry.EditEvent;
			EntryRef = EntryForApiContract.Create(entry.EntryBase, languagePreference, entryThumbPersister, entryImagePersisterOld, ssl, EntryOptionalFields.AdditionalNames | EntryOptionalFields.MainPicture);

		}

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public EntryEditEvent EditEvent { get; set; }

		public EntryForApiContract EntryRef { get; set; }

		public string EntryTypeName { get; set; }

	}

}
