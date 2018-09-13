using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.SongLists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListForApiContract : SongListBaseContract {

		public SongListForApiContract() {}

		public SongListForApiContract(SongList list, IUserIconFactory userIconFactory, IEntryImagePersister imagePersister,
			SongListOptionalFields fields) : base(list) {
			
			ParamIs.NotNull(() => list);

			Author = new UserForApiContract(list.Author, userIconFactory, UserOptionalFields.None);
			EventDate = list.EventDate;

			if (fields.HasFlag(SongListOptionalFields.MainPicture)) {
				MainPicture = (list.Thumb != null ? new EntryThumbForApiContract(list.Thumb, imagePersister) : null);				
			}

		}

		[DataMember]
		public UserForApiContract Author { get; set; }

		[DataMember]
		public DateTime? EventDate { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

	}

	[Flags]
	public enum SongListOptionalFields {

		None		= 0,
		Description = 1,
		MainPicture = 2,

	}

}
