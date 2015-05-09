using System.Data;
using FluentMigrator;

namespace VocaDb.Migrations {

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
