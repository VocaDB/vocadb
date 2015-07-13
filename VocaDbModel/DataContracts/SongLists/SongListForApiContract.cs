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
			bool ssl) : base(list) {
			
			ParamIs.NotNull(() => list);

			Author = new UserForApiContract(list.Author, userIconFactory, UserOptionalFields.None);
			EventDate = list.EventDate;
			FeaturedCategory = list.FeaturedCategory;
			MainPicture = (list.Thumb != null ? new EntryThumbForApiContract(list.Thumb, imagePersister, ssl) : null);

		}

		[DataMember]
		public UserForApiContract Author { get; set; }

		[DataMember]
		public DateTime? EventDate { get; set; }

		[DataMember]
		public SongListFeaturedCategory FeaturedCategory { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

	}

}
