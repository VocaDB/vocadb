#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases;

public class EntryForPictureDisplayContract
{
	public static EntryForPictureDisplayContract Create(IEntryWithNames entry, string mime, byte[] bytes, ContentLanguagePreference languagePreference)
	{
		var name = entry.Names.SortNames[languagePreference];
		var pic = (bytes != null ? new PictureContract(bytes, mime) : null);

		return new EntryForPictureDisplayContract(entry.EntryType, entry.Id, name, entry.Version, pic);
	}

#nullable enable
	public static EntryForPictureDisplayContract Create(Album album, ContentLanguagePreference languagePreference)
	{
		ParamIs.NotNull(() => album);

		var name = album.TranslatedName[languagePreference];
		var pic = (album.CoverPictureData != null ? new PictureContract(album.CoverPictureData, album.CoverPictureMime) : null);

		return new EntryForPictureDisplayContract(EntryType.Album, album.Id, name, album.Version, pic);
	}

	public static EntryForPictureDisplayContract Create(ArchivedAlbumVersion archivedVersion,
		ContentLanguagePreference languagePreference)
	{
		ParamIs.NotNull(() => archivedVersion);

		var name = archivedVersion.Album.TranslatedName[languagePreference];
		var versionWithPic = archivedVersion.GetLatestVersionWithField(AlbumEditableFields.Cover);
		PictureContract? pic = null;

		if (versionWithPic != null && versionWithPic.CoverPicture != null)
			pic = new PictureContract(versionWithPic.CoverPicture, versionWithPic.CoverPictureMime);

		return new EntryForPictureDisplayContract(
			EntryType.Album, archivedVersion.Album.Id, name, archivedVersion.Version, pic);
	}

	public static EntryForPictureDisplayContract Create(Artist artist, ContentLanguagePreference languagePreference)
	{
		ParamIs.NotNull(() => artist);

		var name = artist.TranslatedName[languagePreference];
		var pic = (artist.Picture != null ? new PictureContract(artist.Picture, artist.PictureMime) : null);

		return new EntryForPictureDisplayContract(EntryType.Artist, artist.Id, name, artist.Version, pic);
	}

	public static EntryForPictureDisplayContract Create(ArchivedArtistVersion archivedVersion,
		ContentLanguagePreference languagePreference)
	{
		ParamIs.NotNull(() => archivedVersion);

		var name = archivedVersion.Artist.TranslatedName[languagePreference];
		var versionWithPic = archivedVersion.GetLatestVersionWithField(ArtistEditableFields.Picture);
		PictureContract? pic = null;

		if (versionWithPic != null && versionWithPic.Picture != null)
			pic = new PictureContract(versionWithPic.Picture, versionWithPic.PictureMime);

		return new EntryForPictureDisplayContract(EntryType.Artist, archivedVersion.Artist.Id, name, archivedVersion.Version, pic);
	}
#nullable disable

	public EntryForPictureDisplayContract(EntryType entryType, int entryId, string name, int version, PictureContract pictureContract)
	{
		EntryType = entryType;
		EntryId = entryId;
		Name = name;
		Version = version;
		Picture = pictureContract;
	}

	public int EntryId { get; init; }

	public EntryType EntryType { get; init; }

	public string Name { get; init; }

	public PictureContract Picture { get; init; }

	public int Version { get; init; }
}
