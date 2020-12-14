#nullable disable

using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model;
using VocaDb.Web.Models.Shared;
using VocaDb.Model.Domain.Albums;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Album
{
	public class Versions
	{
		public static ArchivedObjectVersion CreateForAlbum(ArchivedAlbumVersionContract album)
		{
			return new ArchivedObjectVersion(album, GetReasonName(album.Reason, album.Notes),
				GetChangeString(album.ChangedFields), album.Reason != AlbumArchiveReason.PropertiesUpdated || album.ChangedFields != AlbumEditableFields.Nothing);
		}

		public static string GetChangeString(AlbumEditableFields fields)
		{
			if (fields == AlbumEditableFields.Nothing)
				return string.Empty;

			var fieldNames = EnumVal<AlbumEditableFields>.Values.Where(f =>
				f != AlbumEditableFields.Nothing && fields.HasFlag(f)).Select(Translate.AlbumEditableField);

			return string.Join(", ", fieldNames);
		}

		private static string GetReasonName(AlbumArchiveReason reason, string notes)
		{
			if (reason == AlbumArchiveReason.Unknown)
				return notes;

			return Translate.AlbumArchiveReason(reason);
		}

		public Versions() { }

		public Versions(AlbumWithArchivedVersionsContract contract)
		{
			ParamIs.NotNull(() => contract);

			Album = contract;
			ArchivedVersions = contract.ArchivedVersions.Select(CreateForAlbum).ToArray();
		}

		public AlbumContract Album { get; set; }

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }
	}
}