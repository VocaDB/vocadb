#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListContract : SongListBaseContract
	{
		public SongListContract()
		{
			Description = string.Empty;
		}

		public SongListContract(SongList list, IUserPermissionContext permissionContext)
			: base(list)
		{
			ParamIs.NotNull(() => list);

			Author = new UserForApiContract(list.Author);
			CanEdit = EntryPermissionManager.CanEditSongList(permissionContext, list);
			Deleted = list.Deleted;
			Description = list.Description;
			EventDate = list.EventDate;
			Status = list.Status;
			Thumb = (list.Thumb != null ? new EntryThumbContract(list.Thumb) : null);
			Version = list.Version;
		}

		[DataMember]
		public UserForApiContract Author { get; init; }

		[DataMember]
		public bool CanEdit { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public string Description { get; init; }

		[DataMember]
		public DateTime? EventDate { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public EntryThumbContract Thumb { get; init; }

		[DataMember]
		public int Version { get; init; }
	}
}
