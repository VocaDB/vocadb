#nullable disable

using VocaDb.Model.Service.BrandableStrings.Collections;

namespace VocaDb.Model.Service.BrandableStrings
{
	public interface IBrandedStringsAssemblyHeader
	{
		AlbumStrings Album { get; }
		ArtistStrings Artist { get; }
		HomeStrings Home { get; }
		LayoutStrings Layout { get; }
		SongStrings Song { get; }
		UserStrings User { get; }
	}
}
