#nullable disable

using System.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Code
{
	public class EntrySubTypeNameFactory : IEntrySubTypeNameFactory
	{
		public string GetEntrySubTypeName(IEntryBase entryBase, IEnumTranslations enumTranslations, CultureInfo culture) => entryBase switch
		{
			Album a => enumTranslations.Translation(a.DiscType, culture),
			Artist a => enumTranslations.Translation(a.ArtistType, culture),
			Song s => enumTranslations.Translation(s.SongType, culture),
			_ => string.Empty,
		};
	}
}