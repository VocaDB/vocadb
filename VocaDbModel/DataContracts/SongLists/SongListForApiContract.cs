using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.SongLists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListForApiContract : SongListBaseContract, ISongList {

		IUser ISongList.Author => Author;

		public SongListForApiContract() { }

		public SongListForApiContract(SongList list, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory, IAggregatedEntryImageUrlFactory imagePersister,
			SongListOptionalFields fields) : base(list) {
			
			ParamIs.NotNull(() => list);

			Author = new UserForApiContract(list.Author, userIconFactory, UserOptionalFields.None);
			Deleted = list.Deleted;
			EventDate = list.EventDate;
			Status = list.Status;

			if (fields.HasFlag(SongListOptionalFields.Description)) {
				Description = list.Description;
			}

			if (fields.HasFlag(SongListOptionalFields.Events)) {
				Events = list.Events.Select(e => new ReleaseEventContract(e, languagePreference)).OrderBy(e => e.Date).ThenBy(e => e.Name).ToArray();
			}

			if (fields.HasFlag(SongListOptionalFields.MainPicture)) {
				MainPicture = list.Thumb != null ? new EntryThumbForApiContract(list.Thumb, imagePersister) : null;
			}

			if (fields.HasFlag(SongListOptionalFields.Tags)) {
				Tags = list.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(u => u.Count).ToArray();
			}

		}

		[DataMember]
		public UserForApiContract Author { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Deleted { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public DateTime? EventDate { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventContract[] Events { get; set; }

		public bool FeaturedList => FeaturedCategory != SongListFeaturedCategory.Nothing;

		[DataMember(EmitDefaultValue = false)]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; set; }

	}

	[Flags]
	public enum SongListOptionalFields {

		None		= 0,
		Description	= 1,
		Events		= 2,
		MainPicture	= 4,
		Tags		= 8

	}

}
