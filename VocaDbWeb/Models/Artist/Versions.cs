using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Artist
{
	public class Versions
	{
		public static ArchivedObjectVersion CreateForArtist(ArchivedArtistVersionContract artist)
		{
			return new ArchivedObjectVersion(artist, GetReasonName(artist.Reason, artist.Notes),
				GetChangeString(artist.ChangedFields), artist.Reason != ArtistArchiveReason.PropertiesUpdated || artist.ChangedFields != ArtistEditableFields.Nothing);
		}

		public static string GetChangeString(ArtistEditableFields fields)
		{
			if (fields == ArtistEditableFields.Nothing)
				return string.Empty;

			var fieldNames = EnumVal<ArtistEditableFields>.Values.Where(f =>
				f != ArtistEditableFields.Nothing && fields.HasFlag(f)).Select(Translate.ArtistEditableField);

			return string.Join(", ", fieldNames);
		}

		private static string GetReasonName(ArtistArchiveReason reason, string notes)
		{
			if (reason == ArtistArchiveReason.Unknown)
				return notes;

			return Translate.ArtistArchiveReason(reason);
		}

		public Versions() { }

		public Versions(ArtistWithArchivedVersionsContract contract)
		{
			ParamIs.NotNull(() => contract);

			Artist = contract;
			ArchivedVersions = contract.ArchivedVersions.Select(CreateForArtist).ToArray();
		}

		public ArtistContract Artist { get; set; }

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }
	}
}