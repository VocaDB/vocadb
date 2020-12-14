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

			Author = new UserWithEmailContract(list.Author);
			CanEdit = EntryPermissionManager.CanEditSongList(permissionContext, list);
			Deleted = list.Deleted;
			Description = list.Description;
			EventDate = list.EventDate;
			Status = list.Status;
			Thumb = (list.Thumb != null ? new EntryThumbContract(list.Thumb) : null);
			Version = list.Version;
		}

		[DataMember]
		public UserWithEmailContract Author { get; set; }

		[DataMember]
		public bool CanEdit { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime? EventDate { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

		[DataMember]
		public int Version { get; set; }
	}
}
