using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumContract {

		private static void DoIfExists(ArchivedAlbumVersion version, AlbumEditableFields field, XmlCache<ArchivedAlbumContract> xmlCache, Action<ArchivedAlbumContract> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}

		}

		private static void DoIfExists(ArchivedAlbumVersion version, AlbumEditableFields field, XmlCache<ArchivedAlbumContract> xmlCache, Action<ArchivedAlbumContract, XDocument> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data, versionWithField.Data);
			}

		}

		private static void SetArtists(ArchivedAlbumContract data, ArchivedAlbumContract serialized, XDocument doc) {

			if (serialized.Artists != null && serialized.Artists.Any())
				data.Artists = serialized.Artists;
			else {

				// For backwards compatibility
				var artistElems = doc.XPathSelectElements("//ArchivedAlbumContract/Artists/ObjectRefContract"); //.Elements("Artists").Elements("ObjectRefContract"));

				data.Artists = artistElems.Select(e => new ArchivedArtistForAlbumContract {
					Id = int.Parse(e.Element("Id").Value), NameHint = e.Element("NameHint").Value 
				}).ToArray();

			}

		}

		public static ArchivedAlbumContract GetAllProperties(ArchivedAlbumVersion version) {

			var data = new ArchivedAlbumContract();
			var xmlCache = new XmlCache<ArchivedAlbumContract>();
			var thisVersion = xmlCache.Deserialize(version.Version, version.Data);

			data.DiscType = thisVersion.DiscType;
			data.Id = thisVersion.Id;
			data.MainPictureMime = thisVersion.MainPictureMime;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, AlbumEditableFields.Artists, xmlCache, (v, doc) => SetArtists(data, v, doc));
			DoIfExists(version, AlbumEditableFields.Description, xmlCache, v => data.Description = v.Description);
			DoIfExists(version, AlbumEditableFields.Identifiers, xmlCache, v => data.Identifiers = v.Identifiers);
			DoIfExists(version, AlbumEditableFields.OriginalRelease, xmlCache, v => data.OriginalRelease = v.OriginalRelease);
			DoIfExists(version, AlbumEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, AlbumEditableFields.Pictures, xmlCache, v => data.Pictures = v.Pictures);
			DoIfExists(version, AlbumEditableFields.PVs, xmlCache, v => data.PVs = v.PVs);
			DoIfExists(version, AlbumEditableFields.Tracks, xmlCache, v => data.Songs = v.Songs);
			DoIfExists(version, AlbumEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;

		}

		public ArchivedAlbumContract() { }

		public ArchivedAlbumContract(Album album, AlbumDiff diff) {

			ParamIs.NotNull(() => album);
			ParamIs.NotNull(() => diff);

			Artists = (diff.IncludeArtists ? album.Artists.Select(a => new ArchivedArtistForAlbumContract(a)).ToArray() : null);
			Description = (diff.IncludeDescription ? album.Description : null);
			DiscType = album.DiscType;
			Id = album.Id;
			Identifiers = album.Identifiers.Select(i => new AlbumIdentifierContract(i)).ToArray();
			MainPictureMime = album.CoverPictureMime;
			OriginalRelease = (album.OriginalRelease != null && !album.OriginalRelease.IsEmpty ? new ArchivedAlbumReleaseContract(album.OriginalRelease) : null);
			Pictures = (diff.IncludePictures ? album.Pictures.Select(p => new ArchivedEntryPictureFileContract(p)).ToArray() : null);
			PVs = (diff.IncludePVs ? album.PVs.Select(p => new ArchivedPVContract(p)).ToArray() : null);
			Names = (diff.IncludeNames ? album.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null);
			Songs = (diff.IncludeTracks ? album.Songs.Select(s => new SongInAlbumRefContract(s)).ToArray() : null);
			TranslatedName = new TranslatedStringContract(album.TranslatedName);
			WebLinks = (diff.IncludeWebLinks ? album.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null);

		}

		[DataMember]
		public ArchivedArtistForAlbumContract[] Artists { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DiscType DiscType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public AlbumIdentifierContract[] Identifiers { get; set; }

		[DataMember]
		public string MainPictureMime { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ArchivedAlbumReleaseContract OriginalRelease { get; set; }

		[DataMember]
		public ArchivedEntryPictureFileContract[] Pictures { get; set; }

		[DataMember]
		public ArchivedPVContract[] PVs { get; set; }

		[DataMember]
		public SongInAlbumRefContract[] Songs { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
