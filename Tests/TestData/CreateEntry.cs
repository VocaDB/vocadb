using System;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.TestData {

	public static class CreateEntry {

		public static Album Album(int id = 0, string name = "Synthesis") {
			return new Album(new LocalizedString(name, ContentLanguageSelection.Unspecified)) { Id = id };
		}

		public static Artist Artist(ArtistType artistType, int id = 0, string name = "Artist") {
			return new Artist(TranslatedString.Create(name)) { Id = id, ArtistType = artistType };
		}

		public static ReleaseEventSeries EventSeries(string name) {
			return new ReleaseEventSeries(name, string.Empty, new string[0]);
		}

		public static Artist Producer(int id = 0, string name = "Tripshots") {
			return new Artist(TranslatedString.Create(name ?? "Tripshots")) { Id = id, ArtistType = ArtistType.Producer };
		}

		public static PVContract PVContract(int id = 0, string pvId = null, PVType pvType = PVType.Original, DateTime? publishDate = null) {
			return new PVContract { Id = id, Service = PVService.Youtube, PVId = pvId, Name = "Nebula", PVType = pvType, PublishDate = publishDate };
		}

		public static ReleaseEvent ReleaseEvent(string name) {
			return new ReleaseEvent(string.Empty, null, name);
		}

		public static Song Song(int id = 0, string name = "Nebula") {
			return new Song(TranslatedString.Create(name ?? "Nebula")) { Id = id };
		}

		public static SongTagUsage SongTagUsage(Song song, Tag tag, User vote = null) {
			
			var usage = new SongTagUsage(song, tag);
			song.Tags.Usages.Add(usage);
			usage.CreateVote(vote);
			return usage;

		}

		public static Tag Tag(string name, int id = 0) {
			return new Tag(name) { Id = id, TagName = name };
		}

		public static User User(int id = 0, string name = "Miku", UserGroupId group = UserGroupId.Regular, string email = "") {
			return new User(name, "123", email, 0) { GroupId = group, Id = id };
		}

		public static UserMessage UserMessageReceived(int id = 0, User sender = null, User receiver = null, 
			string subject = "Hello world", string body = "Message body", bool highPriority = false, 
			bool read = false) {

			return new UserMessage(receiver, UserInboxType.Received, sender, receiver, subject, body, highPriority) { Id = id, Read = read };

		}

		public static UserMessage UserMessageSent(int id = 0, User sender = null, User receiver = null,
			string subject = "Hello world", string body = "Message body", bool highPriority = false,
			bool read = false) {

			return new UserMessage(sender, UserInboxType.Sent, sender, receiver, subject, body, highPriority) { Id = id, Read = read };

		}

		public static VideoUrlParseResult VideoUrlParseResultWithTitle(
			string url = "http://nicovideo.jp/watch/sm1234567", 
			PVService service = PVService.NicoNicoDouga, 
			string id = "sm1234567",
			string title = "Resistance",
			string author = "tripshots",
			string thumbUrl = "",
			int? length = null,
			string[] tags = null) {
			
			return VideoUrlParseResult.CreateOk(url, service, id, VideoTitleParseResult.CreateSuccess(title, author, thumbUrl, length, tags));

		}

		public static Artist Vocalist(int id = 0, string name = null, ArtistType artistType = ArtistType.Vocaloid) {
			return new Artist(TranslatedString.Create(name ?? "Hatsune Miku")) { Id = id, ArtistType = artistType };			
		}

	}

}
