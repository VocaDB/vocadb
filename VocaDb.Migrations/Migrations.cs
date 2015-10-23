using System.Data;
using FluentMigrator;

namespace VocaDb.Migrations {

	[Migration(201510232200)]
	public class ArchivedTagNotes : AutoReversingMigration {

		public override void Up() {

			Create.Column("[Notes]").OnTable(TableNames.ArchivedTagVersions).AsString(200).NotNullable().WithDefaultValue(string.Empty);
			
		}

	}

	[Migration(201510102223)]
	public class AlbumDiscProperties : AutoReversingMigration {

		public override void Up() {

			Create.Table(TableNames.AlbumDiscProperties)
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Album").AsInt32().NotNullable()
					.ForeignKey("FK_AlbumDiscProperties_Albums", TableNames.Albums, "Id").OnDelete(Rule.Cascade)					
				.WithColumn("DiscNumber").AsInt32().NotNullable()
				.WithColumn("MediaType").AsString(20).NotNullable()
				.WithColumn("[Name]").AsString(200).NotNullable();

			Create.Index("IX_AlbumDiscProperties_Album_DiscNumber").OnTable(TableNames.AlbumDiscProperties)
				.OnColumn("Album").Ascending().OnColumn("DiscNumber").Ascending().WithOptions().Unique();
		}

	}

	[Migration(201509172250)]
	public class UnreadNotificationsToKeepForUser : AutoReversingMigration {

		public override void Up() {

			Create.Column("UnreadNotificationsToKeep").OnTable(TableNames.UserOptions).AsInt32().NotNullable().WithDefaultValue(10);

		}

	}

	[Migration(201509131540)]
	public class TagComments : AutoReversingMigration {

		public override void Up() {

			Create.Table("TagComments")
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Author").AsInt32().NotNullable().ForeignKey("FK_TagComments_Users", TableNames.Users, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Created").AsDateTime().NotNullable()
				.WithColumn("Message").AsString(4000).NotNullable()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey("FK_TagComments_Tags", TableNames.Tags, "Id").OnDelete(Rule.Cascade).Indexed("IX_TagComments_Tag");

		}

	}

	[Migration(201509062115)]
	public class SongListComments : AutoReversingMigration {

		public override void Up() {

			Create.Table("SongListComments")
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Author").AsInt32().NotNullable().ForeignKey("FK_SongListComments_Users", TableNames.Users, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Created").AsDateTime().NotNullable()
				.WithColumn("Message").AsString(4000).NotNullable()
				.WithColumn("SongList").AsInt32().NotNullable().ForeignKey("FK_SongListComments_SongLists", TableNames.SongLists, "Id").OnDelete(Rule.Cascade);
				
		}

	}

	[Migration(201509032103)]
	public class VerifiedArtistForUsers : Migration {

		private const string col = "VerifiedArtist";

		public override void Up() {

			Create.Column(col).OnTable(TableNames.Users).AsBoolean().NotNullable().WithDefaultValue(false);

			Execute.Sql("UPDATE usr SET usr.VerifiedArtist = 1 FROM [Users] usr WHERE usr.Id IN (SELECT DISTINCT [User] FROM OwnedArtistsForUsers)");
				
		}

		public override void Down() {
			Delete.Column(col).FromTable(TableNames.Users);
		}

	}

	[Migration(201508222100)]
	public class CreateInboxesForUserMessages : AutoReversingMigration {

		public override void Up() {

			Create.Column("[User]").OnTable(TableNames.UserMessages).AsInt32().Nullable()
				.ForeignKey("FK_UserMessages_Users2", TableNames.Users, "Id").OnDelete(Rule.Cascade);

			Create.Column("Inbox").OnTable(TableNames.UserMessages).AsString(16).Nullable();

		}

	}

	[Migration(201507261300)]
	public class CreateDateToSongLists : AutoReversingMigration {

		public override void Up() {

			Create.Column("[CreateDate]").OnTable(TableNames.SongLists).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

		}

	}

	[Migration(201507221700)]
    public class AllowCustomTracks : AutoReversingMigration {

        public override void Up() {

			Alter.Column("[Song]").OnTable(TableNames.SongsInAlbums).AsInt32().Nullable();

        }

    }

    [Migration(201507121800)]
	public class EventDateToSongLists : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("[EventDate]").OnTable(TableNames.SongLists).AsDate().Nullable();

		}

	}

	[Migration(201507091400)]
	public class DateToRatedSongs : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("[Date]").OnTable(TableNames.FavoriteSongsForUsers).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

		}

	}

	/// <summary>
	/// Add Index to 'Deleted' column on 'Albums' table.
	/// There's lots of count lookups to albums table that filter by deletion.
	/// For example from user stats.
	/// </summary>
	[Migration(201507040000)]
	public class IndexDeletedToAlbums : AutoReversingMigration {

		public override void Up() {

			Create.Index("IX_Albums_Deleted").OnTable(TableNames.Albums)
				.OnColumn("[Deleted]").Ascending().OnColumn("[Id]").Ascending();

		}

	}

	[Migration(201506272320)]
	public class ArchivedSongListVersionNotes : AutoReversingMigration {

		public override void Up() {
			
			Create.Column("[Status]").OnTable(TableNames.SongLists).AsString(10).NotNullable().WithDefaultValue("Finished");
			Create.Column("[Notes]").OnTable(TableNames.ArchivedSongListVersions).AsString(200).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201505301700)]
	public class VersionNumbers : AutoReversingMigration {

		public override void Up() {
			
			Create.Column("[Version]").OnTable(TableNames.AlbumReleaseEvents).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("[Version]").OnTable(TableNames.SongLists).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("[Version]").OnTable(TableNames.Tags).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("[Version]").OnTable(TableNames.ArchivedEventVersions).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("[Version]").OnTable(TableNames.ArchivedSongListVersions).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("[Version]").OnTable(TableNames.ArchivedTagVersions).AsInt32().NotNullable().WithDefaultValue(0);

		}

	}

	[Migration(201505301600)]
	public class ActivityEntryReleaseEvent : AutoReversingMigration {

		public override void Up() {
			
			Create.Column("ReleaseEvent").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_AlbumReleaseEvents", TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade);

			Create.Column("ArchivedReleaseEventVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedEventVersions", TableNames.ArchivedEventVersions, "Id").OnDelete(Rule.None);

		}

	}

	[Migration(201505252132)]
	public class ActivityEntryTagsAndSongLists : AutoReversingMigration {

		public override void Up() {

			Create.Column("SongList").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_SongLists", TableNames.SongLists, "Id").OnDelete(Rule.Cascade);

			Create.Column("Tag").OnTable(TableNames.ActivityEntries).AsString(30).Nullable()
				.ForeignKey("FK_ActivityEntries_Tags", TableNames.Tags, "Name").OnDelete(Rule.Cascade);

			Create.Column("ArchivedSongListVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedSongListVersions", TableNames.ArchivedSongListVersions, "Id").OnDelete(Rule.None);
		
			Create.Column("ArchivedTagVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedTagVersions", TableNames.ArchivedTagVersions, "Id").OnDelete(Rule.None);
			
		}

	}


	[Migration(201505182200)]
	public class ActivityEntryArchivedEntryIds : AutoReversingMigration {

		public override void Up() {

			Create.Column("ArchivedAlbumVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedAlbumVersions", TableNames.ArchivedAlbumVersions, "Id").OnDelete(Rule.None);

			Create.Column("ArchivedArtistVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedArtistVersions", TableNames.ArchivedArtistVersions, "Id").OnDelete(Rule.None);
		
			Create.Column("ArchivedSongVersion").OnTable(TableNames.ActivityEntries).AsInt32().Nullable()
				.ForeignKey("FK_ActivityEntries_ArchivedSongVersions", TableNames.ArchivedSongVersions, "Id").OnDelete(Rule.None);
			
		}

	}

	[Migration(201505142300)]
	public class SongThumbUrl : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("ThumbUrl").OnTable(TableNames.Songs).AsString(255).Nullable();
				
		}

	}

	[Migration(201505101800)]
	public class SongPublishDate : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("PublishDate").OnTable(TableNames.Songs).AsDate().Nullable();
				
		}

	}

	[Migration(201505092300)]
	public class SongPVPublishDate : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("PublishDate").OnTable(TableNames.PVsForSongs).AsDate().Nullable();
				
		}

	}

	[Migration(201504302045)]
	public class UserLastLoginCulture : AutoReversingMigration {

		public override void Up() {
		
			Create.Column("LastLoginCulture").OnTable(TableNames.UserOptions).AsString(20).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201504111700)]
	public class EntryReportVersionNumber : AutoReversingMigration {

		public override void Up() {
		
			if (Schema.Table(TableNames.EntryReports).Column("VersionNumber").Exists())
				return;

			Create.Column("VersionNumber").OnTable(TableNames.EntryReports).AsInt32().Nullable();

		}

	}

	[Migration(201504012300)]
	public class ArtistBaseVoicebankIndex : AutoReversingMigration {

		public override void Up() {

			if (Schema.Table(TableNames.Artists).Index("IX_Artists_BaseVoicebank").Exists())
				return;

			Create.Index("IX_Artists_BaseVoicebank").OnTable(TableNames.Artists).OnColumn("BaseVoicebank").Ascending();

		}

	}

	[Migration(201502131812)]
	public class ExtendCommentTextLength : Migration {

		private readonly string[] tables = { "AlbumComments", "ArtistComments", "SongComments", "UserComments" };

		public override void Up() {

			foreach (var table in tables) {
	
				Alter.Column("Message").OnTable(table).AsString(4000);
			
			}

			Alter.Column("Message").OnTable("DiscussionComments").InSchema("discussions").AsString(4000);

		}

		public override void Down() {

			foreach (var table in tables) {
	
				Alter.Column("Message").OnTable(table).AsString(800);
			
			}

			Alter.Column("Message").OnTable("DiscussionComments").InSchema("discussions").AsString(800);

		}

	}

	[Migration(201501271800)]
	public class AddDiscussionFolders : AutoReversingMigration {

		public override void Up() {
			
			var schema = "discussions";

			if (Schema.Schema(schema).Table("DiscussionFolders").Exists())
				return;

			Create.Schema(schema);

			Create.Table("DiscussionFolders").InSchema(schema)
				.WithColumn("[Id]").AsInt32().PrimaryKey("PK_DiscussionFolders").Identity().NotNullable()
				.WithColumn("Deleted").AsBoolean().NotNullable().WithDefaultValue(false)
				.WithColumn("Description").AsString(int.MaxValue).NotNullable().WithDefaultValue(string.Empty)
				.WithColumn("Name").AsString(200).NotNullable()
				.WithColumn("SortIndex").AsInt32().NotNullable().WithDefaultValue(0);

			Create.Table("DiscussionTopics").InSchema(schema)
				.WithColumn("[Id]").AsInt32().PrimaryKey("PK_DiscussionTopics").Identity().NotNullable()
				.WithColumn("Content").AsString(int.MaxValue).NotNullable()
				.WithColumn("[Created]").AsDateTime().NotNullable()
				.WithColumn("Deleted").AsBoolean().NotNullable().WithDefaultValue(false)
				.WithColumn("Locked").AsBoolean().NotNullable().WithDefaultValue(false)
				.WithColumn("Pinned").AsBoolean().NotNullable().WithDefaultValue(false)
				.WithColumn("Name").AsString(200).NotNullable()
				.WithColumn("[Author]").AsInt32().NotNullable().ForeignKey("FK_DiscussionTopics_Users", "dbo", "[Users]", "[Id]").OnDelete(Rule.None)
				.WithColumn("[Folder]").AsInt32().NotNullable().ForeignKey("FK_DiscussionTopics_DiscussionFolders", schema, "[DiscussionFolders]", "[Id]").OnDelete(Rule.Cascade);

			Create.Table("DiscussionComments").InSchema(schema)
				.WithColumn("[Id]").AsInt32().PrimaryKey("PK_DiscussionComments").Identity().NotNullable()
				.WithColumn("AuthorName").AsString(100).NotNullable()
				.WithColumn("[Created]").AsDateTime().NotNullable()
				.WithColumn("Message").AsString(int.MaxValue).NotNullable()
				.WithColumn("[Author]").AsInt32().Nullable().ForeignKey("FK_DiscussionComments_Users", "dbo", "[Users]", "[Id]").OnDelete(Rule.SetNull)
				.WithColumn("[Topic]").AsInt32().NotNullable().ForeignKey("FK_DiscussionComments_DiscussionTopics", schema, "[DiscussionTopics]", "[Id]").OnDelete(Rule.Cascade);

		}

	}


	/// <summary>
	/// #11 add English translated description field.
	/// </summary>
	[Migration(201501192000)]
	public class AddEnglishTranslatedDescriptions : AutoReversingMigration {

		public override void Up() {

			if (Schema.Table("Artists").Column("DescriptionEng").Exists())
				return;

			Create.Column("DescriptionEng").OnTable("Artists").AsString(int.MaxValue).WithDefaultValue(string.Empty);
			Create.Column("DescriptionEng").OnTable("Albums").AsString(int.MaxValue).WithDefaultValue(string.Empty);
			Create.Column("NotesEng").OnTable("Songs").AsString(int.MaxValue).WithDefaultValue(string.Empty);

		}
	}

	/// <summary>
	/// Add ShowChatbox column to UserOptions table
	/// </summary>
	[Migration(201501232300)]
	public class AddShowChatboxForUser : AutoReversingMigration {

		public override void Up() {
			
			if (Schema.Table("UserOptions").Column("ShowChatbox").Exists())
				return;

			Create.Column("ShowChatbox").OnTable("UserOptions").AsBoolean().WithDefaultValue(true);

		}

	}

}
