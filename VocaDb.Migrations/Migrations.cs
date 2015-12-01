using System;
using System.Data;
using FluentMigrator;

namespace VocaDb.Migrations {

	[Migration(201511302100)]
	public class TagIdReferences : Migration {

		private void CopyTagNameToId(string tableName, string foreignKeyColumnName, bool nullable = false) {

			var tagNameColumn = string.Format("{0}Name", foreignKeyColumnName);

			Rename.Column(foreignKeyColumnName).OnTable(tableName).To(tagNameColumn);
			Create.Column(foreignKeyColumnName).OnTable(tableName).AsInt32().Nullable();

			// Ex. UPDATE SongTagUsages SET SongTagUsages.Tag = Tags.Id FROM SongTagUsages INNER JOIN Tags ON (SongTagUsages.TagName = Tags.Name)
			Execute.Sql(string.Format("UPDATE {0} SET {0}.{1} = SourceTable.Id FROM {0} INNER JOIN {2} SourceTable ON ({0}.{3} = SourceTable.Name)", 
				tableName, foreignKeyColumnName, TableNames.Tags, tagNameColumn));

			if (!nullable)
				Alter.Column(foreignKeyColumnName).OnTable(tableName).AsInt32().NotNullable();

			Delete.Column(tagNameColumn).FromTable(tableName);

		}

		private void MigrateUsagesTable(string usagesTableName, string entryColumnName) {

			var primaryIndexName = string.Format("IX_{0}", usagesTableName);
			var secondaryIndexname = string.Format("IX_{0}_1", usagesTableName);
			var foreignKeyName = string.Format("FK_{0}_Tags", usagesTableName);

			Delete.Index(primaryIndexName).OnTable(usagesTableName);

			if (Schema.Table(usagesTableName).Index(secondaryIndexname).Exists()) {
				Delete.Index(secondaryIndexname).OnTable(usagesTableName);
			}

			if (Schema.Table(usagesTableName).Constraint(foreignKeyName).Exists()) {
				Delete.ForeignKey(foreignKeyName).OnTable(usagesTableName);
			}

			CopyTagNameToId(usagesTableName, "Tag");

			Create.Index(primaryIndexName)
				.OnTable(usagesTableName).OnColumn(entryColumnName).Ascending()
				.OnColumn("Tag").Ascending()
				.WithOptions().Unique();
			Create.Index(secondaryIndexname)
				.OnTable(usagesTableName).OnColumn("Tag").Ascending();

			Create.ForeignKey(foreignKeyName)
				.FromTable(usagesTableName).ForeignColumn("Tag")
				.ToTable(TableNames.Tags).PrimaryColumn("Id")
				.OnDelete(Rule.Cascade);

		}

		public override void Up() {

			// Tag usages
			MigrateUsagesTable(TableNames.SongTagUsages, "Song");
			MigrateUsagesTable(TableNames.AlbumTagUsages, "Album");
			MigrateUsagesTable(TableNames.ArtistTagUsages, "Artist");

			// Archived versions
			Delete.ForeignKey("FK_ArchivedTagVersions_Tags").OnTable(TableNames.ArchivedTagVersions);
			CopyTagNameToId(TableNames.ArchivedTagVersions, "Tag");
			Create.ForeignKey(string.Format("FK_{0}_Tags", TableNames.ArchivedTagVersions))
				.FromTable(TableNames.ArchivedTagVersions).ForeignColumn("Tag")
				.ToTable(TableNames.Tags).PrimaryColumn("Id")
				.OnDelete(Rule.Cascade);

			// AliasedTo + Parent
			Delete.ForeignKey("FK_Tags_Tags").OnTable(TableNames.Tags);
			Delete.ForeignKey("FK_Tags_Tags1").OnTable(TableNames.Tags);
			CopyTagNameToId(TableNames.Tags, "AliasedTo", true);
			CopyTagNameToId(TableNames.Tags, "Parent", true);
			Create.ForeignKey("FK_Tags_Tags").FromTable(TableNames.Tags).ForeignColumn("AliasedTo").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_Tags_Tags1").FromTable(TableNames.Tags).ForeignColumn("Parent").ToTable(TableNames.Tags).PrimaryColumn("Id");

			// Activity entries
			Delete.ForeignKey("FK_ActivityEntries_Tags").OnTable(TableNames.ActivityEntries);
			CopyTagNameToId(TableNames.ActivityEntries, "Tag", true);
			Create.ForeignKey("FK_ActivityEntries_Tags").FromTable(TableNames.ActivityEntries).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");

		}

		public override void Down() {
			// Sorry
		}

	}

	[Migration(201511261900)]
	public class TagEnglishName : Migration {

		public override void Up() {

			Create.Column("EnglishName").OnTable(TableNames.Tags).AsString(100).NotNullable().WithDefaultValue(string.Empty);
			Execute.Sql(string.Format("UPDATE {0} SET EnglishName = Name", TableNames.Tags));
			Create.Index("IX_Tags_EnglishName").OnTable(TableNames.Tags).OnColumn("EnglishName").Unique();

		}

		public override void Down() {
			Delete.Column("EnglishName").FromTable(TableNames.Tags);
		}

	}

	/// <summary>
	/// Replace index in tag usages tables with a unique index of entry + tag pair (since that combination is unique).
	/// </summary>
	[Migration(201511232100)]
	public class TagUsagesUniqueIndexes : Migration {

		private void CreateIndex(string table, string indexName, string entityColumn) {
			Delete.Index(indexName).OnTable(table);
			Create.Index(indexName).OnTable(table).OnColumn(entityColumn).Ascending().OnColumn("Tag").Ascending().WithOptions().Unique();
		}

		private void RevertIndex(string table, string indexName, string entityColumn) {
			Delete.Index(indexName).OnTable(table);
			Create.Index(indexName).OnTable(table).OnColumn(entityColumn).Ascending();
		}

		public override void Up() {

			CreateIndex(TableNames.SongTagUsages, "IX_SongTagUsages", "Song");
			CreateIndex(TableNames.AlbumTagUsages, "IX_AlbumTagUsages", "Album");
			CreateIndex(TableNames.ArtistTagUsages, "IX_ArtistTagUsages", "Artist");

		}

		public override void Down() {

			RevertIndex(TableNames.SongTagUsages, "IX_SongTagUsages", "Song");
			RevertIndex(TableNames.AlbumTagUsages, "IX_AlbumTagUsages", "Album");
			RevertIndex(TableNames.ArtistTagUsages, "IX_ArtistTagUsages", "Artist");

		}

	}

	[Migration(201511151730)]
	public class ReleaseEventSeriesPicture : AutoReversingMigration {

		public override void Up() {

			Create.Column("PictureMime").OnTable(TableNames.AlbumReleaseEventSeries).AsString(32).Nullable();

		}

	}

	[Migration(201511022300)]
	public class UserMessagesIndexReceiver : AutoReversingMigration {

		public override void Up() {

			// Used for checking unread messages
			Create.Index("IX_UserMessages_User").OnTable(TableNames.UserMessages)
				.OnColumn("[User]").Ascending()
				.OnColumn("[Inbox]").Ascending()
				.OnColumn("[Read]").Ascending();

		}

	}

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
