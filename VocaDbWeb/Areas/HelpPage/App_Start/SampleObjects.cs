using System;
using System.Collections.Generic;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Web.Areas.HelpPage.App_Start {

	public static class SampleObjects {

		private static readonly ArtistContract producer = new ArtistContract {
			Id = 10, Name = "Tripshots", ArtistType = ArtistType.Producer, Status = EntryStatus.Approved, Version = 1, PictureMime = "image/jpeg"
		};

		private static readonly ArtistContract vocalist = new ArtistContract {
			Id = 1, Name = "Hatsune Miku", ArtistType = ArtistType.Vocaloid, Status = EntryStatus.Approved, Version = 1, PictureMime = "image/png"
		};

	    private static readonly AlbumForApiContract sampleAlbum = new AlbumForApiContract {
			Id = 1,
  			DefaultName = "Synthesis",
			DefaultNameLanguage = ContentLanguageSelection.English,
			CreateDate = new DateTime(2011, 1, 16),
			DiscType = DiscType.Album,
			RatingAverage = 3.9d,
			RatingCount = 39,
			AdditionalNames = string.Empty,
			ArtistString = "Tripshots feat. Hatsune Miku",
			Name = "Synthesis",
			Status = EntryStatus.Finished,
			Version = 1,
			Artists = new [] {
				new ArtistForAlbumForApiContract {
					Artist = producer, 
					Categories = ArtistCategories.Producer
				},
				new ArtistForAlbumForApiContract {
					Artist = vocalist, 
					Categories = ArtistCategories.Vocalist
				}
			},
			Names = new[] {
				new LocalizedStringContract("Synthesis", ContentLanguageSelection.English)
			},
			Tags = new [] {
				new TagUsageForApiContract { Tag = new TagBaseContract { Name = "electronic" } }
			},
			WebLinks = new[] {
				new WebLinkForApiContract(new WebLink("KarenT", "http://karent.jp/album/29", WebLinkCategory.Official)), 
			}
	    };

		private static readonly ArtistForApiContract sampleArtist = new ArtistForApiContract {
			Id = 10,
			DefaultName = "Tripshots",
			DefaultNameLanguage = ContentLanguageSelection.English,
			CreateDate = new DateTime(2011, 1, 15),
			Description = "A producer who is well-known for animating all his 3D PVs himself. Computer graphics artist by profession. His most well-known song/PV is Nebula.",
			AdditionalNames = string.Empty,
			ArtistType = ArtistType.Producer,
			Name = "Tripshots",
			PictureMime = "image/jpeg",
			Status = EntryStatus.Approved,
			Version = 1,
			Names = new [] {
				new LocalizedStringContract("Tripshots", ContentLanguageSelection.English)				
			},
			Tags = new[] {
				new TagUsageForApiContract { Tag = new TagBaseContract { Name = "electronic" }, Count = 39 }
			},
			WebLinks = new[] {
				new WebLinkForApiContract { Category = WebLinkCategory.Official, Url = "http://tripshots.net/", Description = "Official website" },
			}
		};

		private static readonly SongForApiContract sampleSong = new SongForApiContract {
			Id = 121,
			DefaultName = "Nebula",
			DefaultNameLanguage = ContentLanguageSelection.English,
			CreateDate = new DateTime(2011, 1, 15),
			SongType = SongType.Original,
			PVServices = PVServices.NicoNicoDouga | PVServices.Youtube,
			AdditionalNames = string.Empty,
			ArtistString = "Tripshots feat. Hatsune Miku",
			FavoritedTimes = 39,
			Name = "Nebula",
			RatingScore = 39,
			Status = EntryStatus.Finished,
			ThumbUrl = "http://i1.ytimg.com/vi/hoLu7c2XZYU/default.jpg",
			LengthSeconds = 290,
			Version = 1,
			Artists = new [] {
				new ArtistForSongContract {
					Artist = producer, 
					Categories = ArtistCategories.Producer
				},
				new ArtistForSongContract {
					Artist = vocalist, 
					Categories = ArtistCategories.Vocalist
				}				
			},
			Albums = new [] {
				new AlbumContract {
					Id = 1, Name = "Synthesis", DiscType = DiscType.Album, Version = 1, Status = EntryStatus.Approved,
					CreateDate = new DateTime(2011, 1, 16)
				}, 
			},
			PVs = new [] {
				new PVContract {
					Name = "Nebula ft. Hatsune Miku by Tripshots", Author = "Tripshots", PVType = PVType.Original, Length = 290, 
					Service = PVService.Youtube, PVId = "hoLu7c2XZYU",
					ThumbUrl = "http://i1.ytimg.com/vi/hoLu7c2XZYU/default.jpg", Url = "http://youtu.be/hoLu7c2XZYU"
				}, 
			}
		};

		private static readonly SongInAlbumContract[] sampleSongsInAlbums = new [] {
			new SongInAlbumContract {
				DiscNumber = 1, TrackNumber = 1, 
				Song = new SongContract {
					Id = 1766, Name = "anger [Extend-RMX]", ArtistString = "Tripshots feat. Hatsune Miku", 
					LengthSeconds = 338, PVServices = PVServices.Youtube, SongType = SongType.Remix,
					FavoritedTimes = 39, Version = 1, CreateDate = new DateTime(2011, 2, 11), Status = EntryStatus.Finished
				}
			}, 
			new SongInAlbumContract {
				DiscNumber = 1, TrackNumber = 2, 
				Song = new SongContract {
					Id = 1767, Name = "Extended [Sirius RMX]", ArtistString = "Tripshots feat. Hatsune Miku", 
					LengthSeconds = 231, PVServices = PVServices.Youtube, SongType = SongType.Remix,
					FavoritedTimes = 39, Version = 1, CreateDate = new DateTime(2011, 2, 11), Status = EntryStatus.Finished
				}
			}
		};

		public static Dictionary<Type, object> Samples {
			get {
				return new Dictionary<Type, object> {
					{typeof(ArtistForApiContract), sampleArtist},
					{typeof(PartialFindResult<ArtistForApiContract>), new PartialFindResult<ArtistForApiContract>(new [] { sampleArtist}, 1)},
					{typeof(AlbumForApiContract), sampleAlbum},
					{typeof(SongForApiContract), sampleSong},
					{typeof(SongInAlbumContract[]), sampleSongsInAlbums},
				};
			}
		}

	}
}