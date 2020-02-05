using System.Data;
using FluentMigrator;

namespace VocaDb.Migrations {

	// Migration version format: YYYY_MM_DD_HHmm

	[Migration(2020_02_05_1900)]
	public class EventDescriptionLength : Migration {

		public override void Up() {
			Delete.DefaultConstraint().OnTable(TableNames.AlbumReleaseEvents).OnColumn("Description");
			Alter.Column("Description").OnTable(TableNames.AlbumReleaseEvents).AsString(int.MaxValue).NotNullable().WithDefaultValue(string.Empty);
			Delete.DefaultConstraint().OnTable(TableNames.AlbumReleaseEventSeries).OnColumn("Description");
			Alter.Column("Description").OnTable(TableNames.AlbumReleaseEventSeries).AsString(int.MaxValue).NotNullable().WithDefaultValue(string.Empty);
		}

		public override void Down() {}

	}

	[Migration(2020_01_05_1600)]
	public class TagRelatedEntries : AutoReversingMigration {
		public override void Up() {
			var tableName = "EntryTypeToTagMappings";
			Create.Table(tableName)
				.WithColumn(ColumnNames.Id).AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("EntryType").AsString(20).NotNullable()
				.WithColumn("SubType").AsString(30).NotNullable()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, ColumnNames.Id).OnDelete(Rule.Cascade);

			Create.Index("UX_EntryTypeToTagMappings_EntryType").OnTable(tableName)
				.OnColumn("EntryType").Ascending()
				.OnColumn("SubType").Ascending()
				.WithOptions().Unique();

			Create.Index("UX_EntryTypeToTagMappings_Tag").OnTable(tableName)
				.OnColumn("Tag").Ascending()
				.WithOptions().Unique();
		}
	}

	[Migration(2019_11_17_0100)]
	public class SongListTags : AutoReversingMigration {

		public override void Up() {
			Create.Table("SongListTagUsages")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Count").AsInt32().NotNullable()
				.WithColumn("SongList").AsInt32().NotNullable().ForeignKey(TableNames.SongLists, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id")
				.WithColumn("Date").AsDateTime().NotNullable();
			Create.Table("SongListTagVotes")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Usage").AsInt64().NotNullable().ForeignKey("SongListTagUsages", "Id").OnDelete(Rule.Cascade)
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id");
			Create.Index("UX_SongListTagUsages").OnTable("SongListTagUsages").OnColumn("SongList").Ascending()
				.OnColumn("Tag").Ascending().WithOptions().Unique();
			Create.Index("IX_SongListTagUsages_Tag").OnTable("SongListTagUsages").OnColumn("Tag").Ascending();
		}

	}

	[Migration(2019_04_14_1300)]
	public class ArchivedEntryVersionChangedFieldsLength : AutoReversingMigration {
		public override void Up() {
			Alter.Column("ChangedFields").OnTable(TableNames.ArchivedAlbumVersions).AsAnsiString(1000).NotNullable().WithDefaultValue(string.Empty);
			Alter.Column("ChangedFields").OnTable(TableNames.ArchivedArtistVersions).AsAnsiString(1000).NotNullable().WithDefaultValue(string.Empty);
			Alter.Column("ChangedFields").OnTable(TableNames.ArchivedSongVersions).AsAnsiString(1000).NotNullable().WithDefaultValue(string.Empty);
		}
	}

	[Migration(2019_03_12_2100)]
	public class SongNameIndex : AutoReversingMigration {
		public override void Up() {
			if (!Schema.Table(TableNames.SongNames).Index("IX_SongNames").Exists()) {
				Create.Index("IX_SongNames").OnTable(TableNames.SongNames).OnColumn("Song").Ascending();
			}
		}
	}

	[Migration(2019_01_27_1700)]
	public class AlbumReviews : AutoReversingMigration {
		public override void Up() {
			Create.Table(TableNames.AlbumReviews)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Album").AsInt32().NotNullable().ForeignKey(TableNames.Albums, "Id").OnDelete(Rule.Cascade)
				.WithColumn("[Date]").AsDateTime().NotNullable()
				.WithColumn("LanguageCode").AsString(8).NotNullable()
				.WithColumn("Text").AsString(4000).NotNullable()
				.WithColumn("Title").AsString(200).NotNullable()
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id").OnDelete(Rule.Cascade);
			Create.Index("UX_AlbumReviews").OnTable(TableNames.AlbumReviews)
				.OnColumn("Album").Ascending()
				.OnColumn("[User]").Ascending()
				.OnColumn("LanguageCode").Ascending()
				.WithOptions().Unique();			
		}
	}

	[Migration(2019_01_17_2200)]
	public class AuditLogEntryEntryLink : AutoReversingMigration {
		public override void Up() {
			Create.Column("EntryId").OnTable(TableNames.AuditLogEntries).AsInt32().NotNullable().WithDefaultValue(0);
			Create.Column("EntryType").OnTable(TableNames.AuditLogEntries).AsString(20).Nullable();
		}
	}

	[Migration(2018_11_03_2000)]
	public class EventNameExtend : AutoReversingMigration {
		public override void Up() {
			Alter.Column("EnglishName").OnTable(TableNames.AlbumReleaseEvents).AsString(255).NotNullable();
			Alter.Column("EnglishName").OnTable(TableNames.AlbumReleaseEventSeries).AsString(255).NotNullable();
		}
	}

	[Migration(2018_07_19_1900)]
	public class LyricsUrlExtend : AutoReversingMigration {
		public override void Up() {
			Alter.Column("URL").OnTable(TableNames.LyricsForSongs).AsString(500).NotNullable();
		}
	}

	[Migration(2018_07_18_1900)]
	public class EntryReportCloseDate : AutoReversingMigration {
		public override void Up() {
			Create.Column("ClosedAt").OnTable(TableNames.EntryReports).AsDateTime().Nullable();
		}
	}

	[Migration(2017_12_10_1900)]
	public class PublishDateForAllPVs : AutoReversingMigration {
		public override void Up() {
			Create.Column("PublishDate").OnTable(TableNames.PVsForAlbums).AsDateTime().Nullable();
			Create.Column("PublishDate").OnTable(TableNames.PVsForEvents).AsDateTime().Nullable();
		}
	}

	[Migration(2017_11_12_2100)]
	public class TagMappingCreateDate : AutoReversingMigration {
		public override void Up() {
			Create.Column("CreateDate").OnTable(TableNames.TagMappings).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
		}
	}

	[Migration(2017_10_01_1400)]
	public class UserStandAlone : AutoReversingMigration {
		public override void Up() {
			Create.Column("Standalone").OnTable(TableNames.UserOptions).AsBoolean().NotNullable().WithDefaultValue(false);
		}
	}

	[Migration(2017_09_17_2200)]
	public class SongListDeleted : AutoReversingMigration {
		public override void Up() {
			Create.Column("Deleted").OnTable(TableNames.SongLists).AsBoolean().NotNullable().WithDefaultValue(false);
		}
	}

	[Migration(2017_07_24_1500)]
	public class PVForSongDisabled : AutoReversingMigration {
		public override void Up() {
			Create.Column("Disabled").OnTable(TableNames.PVsForSongs).AsBoolean().NotNullable().WithDefaultValue(false);
		}
	}

	[Migration(2017_07_15_2300)]
	public class EventEndDate : AutoReversingMigration {
		public override void Up() {
			Create.Column("EndDate").OnTable(TableNames.AlbumReleaseEvents).AsDateTime().Nullable();
		}
	}

	[Migration(2017_07_10_2300)]
	public class EventSeriesTags : AutoReversingMigration {
		public override void Up() {
			Create.Table("EventSeriesTagUsages")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Count").AsInt32().NotNullable()
				.WithColumn("EventSeries").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEventSeries, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id")
				.WithColumn("Date").AsDateTime().NotNullable();
			Create.Table("EventSeriesTagVotes")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Usage").AsInt64().NotNullable().ForeignKey("EventSeriesTagUsages", "Id").OnDelete(Rule.Cascade)
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id");
			Create.Index("UX_EventSeriesTagUsages").OnTable("EventSeriesTagUsages").OnColumn("EventSeries").Ascending()
				.OnColumn("Tag").Ascending().WithOptions().Unique();
			Create.Index("IX_EventSeriesTagUsages_Tag").OnTable("EventSeriesTagUsages").OnColumn("Tag").Ascending();
		}
	}

	[Migration(2017_07_10_2100)]
	public class EntryReportStatus : AutoReversingMigration {
		public override void Up() {
			Create.Column("Status").OnTable(TableNames.EntryReports).AsString(50).NotNullable().WithDefaultValue("Open");
			Create.Column("ClosedBy").OnTable(TableNames.EntryReports).AsInt32().Nullable().ForeignKey(TableNames.Users, "Id").OnDelete(Rule.SetNull);
		}
	}

	[Migration(2017_07_07_2100)]
	public class ReplaceEventIndex : Migration {
		public override void Up() {
			Delete.Index("IX_AlbumReleaseEvents_Name").OnTable(TableNames.AlbumReleaseEvents);
			Create.Index("IX_EventNames_Value").OnTable(TableNames.EventNames).OnColumn("Value").Unique();
		}

		public override void Down() {
			Delete.Index("IX_EventNames_Value").OnTable(TableNames.EventNames);
			Create.Index("IX_AlbumReleaseEvents_Name").OnTable(TableNames.AlbumReleaseEvents).OnColumn("EnglishName").Unique();
		}
	}

	[Migration(2017_06_19_2000)]
	public class ArtistsForEvents : AutoReversingMigration {
		public override void Up() {
			Create.Table("ArtistsForEvents")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Artist").AsInt32().Nullable().ForeignKey(TableNames.Artists, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Event").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade).Indexed("IX_ArtistsForEvents_Event")
				.WithColumn("Name").AsString(250).Nullable()
				.WithColumn("Roles").AsInt32().NotNullable();
		}
	}

	[Migration(2017_06_15_2100)]
	public class EventReports : AutoReversingMigration {
		public override void Up() {
			Alter.Table(TableNames.EntryReports).AddColumn("Event").AsInt32().Nullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade);
		}
	}

	[Migration(2017_06_13_2200)]
	public class PVCreatedBy : AutoReversingMigration {
		public override void Up() {
			Create.Column("CreatedBy").OnTable(TableNames.PVsForSongs).AsInt32().Nullable().ForeignKey(TableNames.Users, "Id").OnDelete(Rule.SetNull);
		}
	}

	[Migration(2017_06_05_1800)]
	public class EventTags : AutoReversingMigration {
		public override void Up() {
			Create.Table("EventTagUsages")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Count").AsInt32().NotNullable()
				.WithColumn("[Event]").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id")
				.WithColumn("Date").AsDateTime().NotNullable();
			Create.Table("EventTagVotes")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Usage").AsInt64().NotNullable().ForeignKey("EventTagUsages", "Id")
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id");
		}
	}

	[Migration(2017_05_01_1700)]
	public class PVsForEvents : AutoReversingMigration {
		public override void Up() {
			Create.Table(TableNames.PVsForEvents)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Author").AsString(100).NotNullable()
				.WithColumn("[Event]").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").Indexed()
				.WithColumn("Name").AsString(200).NotNullable()
				.WithColumn("PVId").AsString(255).NotNullable()
				.WithColumn("PVType").AsString(20).NotNullable()
				.WithColumn("Service").AsString(20).NotNullable()
				.WithColumn("ExtendedMetadataJson").AsString(int.MaxValue).Nullable();
		}
	}

	[Migration(2017_04_30_2000)]
	public class PVExtendedMetadata : AutoReversingMigration {
		public override void Up() {
			Create.Column("ExtendedMetadataJson").OnTable(TableNames.PVsForSongs).AsString(int.MaxValue).Nullable();
			Create.Column("ExtendedMetadataJson").OnTable(TableNames.PVsForAlbums).AsString(int.MaxValue).Nullable();
		}
	}

	[Migration(2017_04_30_1600)]
	public class ReleaseEventCreateDate : AutoReversingMigration {
		public override void Up() {
			Create.Column("CreateDate").OnTable(TableNames.AlbumReleaseEvents).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
		}
	}

	[Migration(2017_04_20_2000)]
	public class TranslatedEventName : Migration {

		public override void Up() {

			// Event series
			Rename.Table("AlbumReleaseEventSeriesAliases").To(TableNames.EventSeriesNames);
			Rename.Column("[Name]").OnTable(TableNames.EventSeriesNames).To("Value"); // Note: must NOT use brackets in column name here
			
			Alter.Table(TableNames.EventSeriesNames)
				.AlterColumn("[Value]").AsString(255).NotNullable()
				.AddColumn("Language").AsString(16).NotNullable().WithDefaultValue("Unspecified"); // Assume existing aliases are unspecified language, can be changed

			Rename.Column("[Name]").OnTable(TableNames.AlbumReleaseEventSeries).To("EnglishName"); // Assume existing names are English, can be changed
			Alter.Table(TableNames.AlbumReleaseEventSeries)
				.AddColumn("DefaultNameLanguage").AsString(20).NotNullable().WithDefaultValue("English")
				.AddColumn("JapaneseName").AsString(255).NotNullable().WithDefaultValue(string.Empty)
				.AddColumn("RomajiName").AsString(255).NotNullable().WithDefaultValue(string.Empty)
				.AddColumn("AdditionalNamesString").AsString(1024).NotNullable().WithDefaultValue(string.Empty);

			Execute.Sql(string.Format("UPDATE {0} SET JapaneseName = EnglishName, RomajiName = EnglishName", TableNames.AlbumReleaseEventSeries));
			Execute.Sql(string.Format("INSERT INTO {0} (Series, Language, Value) SELECT Id, 'English', EnglishName FROM {1}", TableNames.EventSeriesNames, TableNames.AlbumReleaseEventSeries));

			// Events
			Create.Table(TableNames.EventNames)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("[Event]").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Language").AsString(16).NotNullable()
				.WithColumn("Value").AsString(255).NotNullable();

			Rename.Column("[Name]").OnTable(TableNames.AlbumReleaseEvents).To("EnglishName");
			Alter.Table(TableNames.AlbumReleaseEvents)
				.AddColumn("DefaultNameLanguage").AsString(20).NotNullable().WithDefaultValue("English")
				.AddColumn("JapaneseName").AsString(255).NotNullable().WithDefaultValue(string.Empty)
				.AddColumn("RomajiName").AsString(255).NotNullable().WithDefaultValue(string.Empty)
				.AddColumn("AdditionalNamesString").AsString(1024).NotNullable().WithDefaultValue(string.Empty);

			Execute.Sql(string.Format("UPDATE {0} SET JapaneseName = EnglishName, RomajiName = EnglishName", TableNames.AlbumReleaseEvents));
			Execute.Sql(string.Format("INSERT INTO {0} ([Event], Language, Value) SELECT Id, 'English', EnglishName FROM {1}", TableNames.EventNames, TableNames.AlbumReleaseEvents));

		}

		public override void Down() {
		}

	}

	[Migration(2017_04_15_2100)]
	public class EventSeriesStatus : AutoReversingMigration {
		public override void Up() {
			Create.Column("Status").OnTable(TableNames.AlbumReleaseEventSeries).AsString(10).NotNullable().WithDefaultValue("Finished");
		}
	}

	[Migration(2017_04_13_2200)]
	public class EventDeletionAndSeriesCategory : AutoReversingMigration {
		public override void Up() {
			Create.Column("Category").OnTable(TableNames.AlbumReleaseEventSeries).AsString(30).NotNullable().WithDefaultValue("Unspecified");
			Create.Column("Deleted").OnTable(TableNames.AlbumReleaseEventSeries).AsBoolean().NotNullable().WithDefaultValue(false);
			Create.Column("Deleted").OnTable(TableNames.AlbumReleaseEvents).AsBoolean().NotNullable().WithDefaultValue(false);
		}
	}

	[Migration(2017_04_13_2100)]
	public class EventStatusAndCategory : AutoReversingMigration {
		public override void Up() {
			Create.Column("Status").OnTable(TableNames.AlbumReleaseEvents).AsString(10).NotNullable().WithDefaultValue("Finished");
			Create.Column("Category").OnTable(TableNames.AlbumReleaseEvents).AsString(30).NotNullable().WithDefaultValue("Unspecified");
		}
	}

	[Migration(2017_04_10_2100)]
	public class TagTargetTypes : AutoReversingMigration {
		public override void Up() {
			Create.Column("Targets").OnTable(TableNames.Tags).AsInt32().NotNullable().WithDefaultValue(1073741823);
		}
	}

	[Migration(2017_04_02_2100)]
	public class UserOptionStylesheet : AutoReversingMigration {
		public override void Up() {
			Create.Column("Stylesheet").OnTable(TableNames.UserOptions).AsString(50).Nullable();
		}
	}

	[Migration(2017_03_29_2100)]
	public class EventsForUsers : AutoReversingMigration {
		public override void Up() {

			Create.Table("EventsForUsers")
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("RelationshipType").AsString(50).NotNullable()
				.WithColumn("ReleaseEvent").AsInt32().NotNullable().ForeignKey("FK_EventsForUsers_ReleaseEvents", TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade)
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey("FK_EventsForUsers_Users", TableNames.Users, "Id").OnDelete(Rule.Cascade);

			Create.UniqueConstraint("UX_EventsForUsers_ReleaseEvents_Users")
				.OnTable("EventsForUsers")
				.Columns("ReleaseEvent", "[User]");

		}
	}

	[Migration(2017_03_28_2100)]
	public class ReleaseEventComments : AutoReversingMigration {
		public override void Up() {
			Create.Table("ReleaseEventComments")
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Author").AsInt32().NotNullable().ForeignKey("FK_ReleaseEventComments_Users", TableNames.Users, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Created").AsDateTime().NotNullable()
				.WithColumn("Message").AsString(4000).NotNullable()
				.WithColumn("ReleaseEvent").AsInt32().NotNullable().ForeignKey("FK_ReleaseEventComments_ReleaseEvents", TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade).Indexed("IX_ReleaseEventComments_ReleaseEvent");
		}
	}

	[Migration(2017_03_19_2000)]
	public class ReleaseEventPictureAndVenue : AutoReversingMigration {
		public override void Up() {
			Create.Column("PictureMime").OnTable(TableNames.AlbumReleaseEvents).AsString(32).Nullable();
			Create.Column("Venue").OnTable(TableNames.AlbumReleaseEvents).AsString(1000).Nullable();
		}
	}

	[Migration(2017_03_18_2000)]
	public class ReleaseEventSongList : AutoReversingMigration {
		public override void Up() {
			Create.Column("SongList").OnTable(TableNames.AlbumReleaseEvents).AsInt32().Nullable().ForeignKey(TableNames.SongLists, "Id").OnDelete(Rule.SetNull);
		}
	}

	[Migration(2017_02_23_2100)]
	public class AlbumPersonalDescription : AutoReversingMigration {

		public override void Up() {

			Create.Column("PersonalDescriptionText").OnTable(TableNames.Albums).AsString(2000).Nullable();
			Create.Column("PersonalDescriptionAuthor").OnTable(TableNames.Albums).AsInt32().Nullable().ForeignKey(TableNames.Artists, "Id");

		}

	}

	[Migration(201702192100)]
	public class SongPersonalDescription : AutoReversingMigration {

		public override void Up() {

			Create.Column("PersonalDescriptionText").OnTable(TableNames.Songs).AsString(2000).Nullable();
			Create.Column("PersonalDescriptionAuthor").OnTable(TableNames.Songs).AsInt32().Nullable().ForeignKey(TableNames.Artists, "Id");

		}

	}

	[Migration(201701122000)]
	public class ArtistHit : AutoReversingMigration {

		public override void Up() {

			Create.Table("ArtistHits")
				.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity()
				.WithColumn("Artist").AsInt32().NotNullable().ForeignKey(TableNames.Artists, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Agent").AsInt32().NotNullable()
				.WithColumn("[Date]").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

			Create.UniqueConstraint("UX_Artist_Agent").OnTable("ArtistHits").Columns("Artist", "Agent");

		}

	}

	[Migration(201701041900)]
	public class TagForUser : AutoReversingMigration {
		public override void Up() {

			Create.Table(TableNames.TagsForUsers)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id")
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id");

			Create.UniqueConstraint("UX_TagsForUsers_Tag_User").OnTable(TableNames.TagsForUsers)
				.Columns("Tag", "[User]");

		}
	}

	[Migration(201611131900)]
	public class VoicebankReleaseDate : AutoReversingMigration {

		public override void Up() {

			Create.Column("ReleaseDate").OnTable(TableNames.Artists).AsDate().Nullable();
				
		}

	}

	[Migration(201611052100)]
	public class TagMapping : Migration {

		public override void Up() {

			Create.Table(TableNames.TagMappings)
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id")
				.WithColumn("SourceTag").AsString(200).NotNullable();

			Create.Index("IX_TagMappings_Tag_SourceTag").OnTable(TableNames.TagMappings)
				.OnColumn("Tag").Ascending()
				.OnColumn("SourceTag").Ascending()
				.WithOptions().Unique();

		}

		public override void Down() {
			Delete.Table(TableNames.TagMappings);
		}

	}

	[Migration(201610032100)]
	public class TagRemoveAliasedTo : Migration {

		public override void Up() {
			Delete.ForeignKey("FK_Tags_Tags").OnTable(TableNames.Tags);
			Delete.Column("AliasedTo").FromTable(TableNames.Tags);
		}

		public override void Down() {
			Create.Column("AliasedTo").OnTable(TableNames.Tags).AsInt32().Nullable().ForeignKey(TableNames.Tags, "Id");
		}

	}

	[Migration(201609272100)]
	public class UserNewCryptoAlgo : Migration {

		public override void Up() {

			Alter.Table(TableNames.Users).AlterColumn("Salt").AsString(100).NotNullable();
			Create.Column("PasswordHashAlgorithm").OnTable(TableNames.Users).AsString(20).NotNullable().WithDefaultValue("SHA1");

		}

		public override void Down() {

			Alter.Table(TableNames.Users).AlterColumn("Salt").AsInt32().NotNullable();

		}

	}

	[Migration(201609122300)]
	public class LyricsURL : Migration {

		public override void Up() {

			Create.Column("URL").OnTable(TableNames.LyricsForSongs).AsString(255).NotNullable().WithDefaultValue(string.Empty);

			Execute.SqlFormat("UPDATE {0} SET URL = [Source] WHERE [Source] LIKE 'http%' OR [Source] LIKE 'www%'", TableNames.LyricsForSongs);
			Execute.SqlFormat("UPDATE {0} SET [Source] = '' WHERE [Source] LIKE 'http%' OR [Source] LIKE 'www%'", TableNames.LyricsForSongs);

		}

		public override void Down() {
			Delete.Column("URL").FromTable(TableNames.LyricsForSongs);
		}

	}

	[Migration(201609092300)]
	public class RemoveLanguageFromLyrics : Migration {

		public override void Up() {
			Delete.Column("Language").FromTable(TableNames.LyricsForSongs);
		}

		public override void Down() {
			Create.Column("Language").OnTable(TableNames.LyricsForSongs).AsString(20).NotNullable().WithDefaultValue(string.Empty);
		}

	}

	[Migration(201609071900)]
	public class LyricsLanguage : Migration {

		public override void Up() {

			Alter.Table(TableNames.LyricsForSongs).AddColumn("CultureCode").AsString(10).NotNullable().WithDefaultValue(string.Empty);
			Alter.Table(TableNames.LyricsForSongs).AddColumn("TranslationType").AsString(20).NotNullable().WithDefaultValue(string.Empty);

			Execute.SqlFormat("UPDATE {0} SET CultureCode = 'en' WHERE Language = 'English'",
				TableNames.LyricsForSongs);

			Execute.SqlFormat("UPDATE {0} SET TranslationType = CASE WHEN Language = 'Japanese' THEN 'Original' WHEN Language = 'Romaji' THEN 'Romanized' ELSE 'Translation' END",
				TableNames.LyricsForSongs);

		}

		public override void Down() {
			
		}

	}

	[Migration(201608292120)]
	public class TagDescriptionLength : Migration {

		public override void Up() {
			Delete.DefaultConstraint().OnTable(TableNames.Tags).OnColumn("Description");
			Alter.Column("Description").OnTable(TableNames.Tags).AsString(int.MaxValue).NotNullable().WithDefaultValue(string.Empty);
			Delete.DefaultConstraint().OnTable(TableNames.Tags).OnColumn("DescriptionEng");
			Alter.Column("DescriptionEng").OnTable(TableNames.Tags).AsString(int.MaxValue).NotNullable().WithDefaultValue(string.Empty);
		}

		public override void Down() {}

	}

	[Migration(201608231900)]
	public class UserKnownLanguages : AutoReversingMigration {

		public override void Up() {

			Create.Table("UserKnownLanguages")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("CultureCode").AsString(10).NotNullable()
				.WithColumn("Proficiency").AsString(15).NotNullable()
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id");
				
		}

	}

	[Migration(201608082300)]
	public class ArchivedReleaseEventRemoveRedundantFields : Migration {

		public override void Up() {
			Delete.Column("Date").FromTable(TableNames.ArchivedEventVersions);
			Delete.Column("Description").FromTable(TableNames.ArchivedEventVersions);
			Delete.Column("Name").FromTable(TableNames.ArchivedEventVersions);
			Delete.ForeignKey("FK_ArchivedEventVersions_AlbumReleaseEventSeries").OnTable(TableNames.ArchivedEventVersions);
			Delete.Column("Series").FromTable(TableNames.ArchivedEventVersions);
			Delete.Column("SeriesNumber").FromTable(TableNames.ArchivedEventVersions);
		}

		public override void Down() {
			
		}

	}

	[Migration(201608082200)]
	public class ReleaseEventSeriesVersionHistory : AutoReversingMigration {

		public override void Up() {

			Create.Column("Version").OnTable(TableNames.AlbumReleaseEventSeries).AsInt32().NotNullable().WithDefaultValue(0);

			Create.Table("ArchivedEventSeriesVersions")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Author").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id")
				.WithColumn("ChangedFields").AsAnsiString(100).NotNullable()
				.WithColumn("CommonEditEvent").AsAnsiString(30).NotNullable()
				.WithColumn("Created").AsDateTime().NotNullable()
				.WithColumn("Data").AsXml().NotNullable()
				.WithColumn("Series").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEventSeries, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Notes").AsString(200).NotNullable()
				.WithColumn("Version").AsInt32().NotNullable();
						
		}
	}

	[Migration(201608052200)]
	public class AlbumReleaseEventId : Migration {

		public override void Up() {

			Create.Column("ReleaseEvent").OnTable(TableNames.Albums).AsInt32().Nullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.SetNull);

			Execute.Sql(string.Format("UPDATE {0} SET {0}.ReleaseEvent = re.Id FROM {0} INNER JOIN {1} re ON ({0}.ReleaseEventName = re.Name)", 
				TableNames.Albums, TableNames.AlbumReleaseEvents));

			Delete.Column("ReleaseEventName").FromTable(TableNames.Albums);

		}

		public override void Down() {

			Create.Column("ReleaseEventName").OnTable(TableNames.Albums).AsString(50).Nullable().ForeignKey(TableNames.AlbumReleaseEvents, "Name");
			Delete.Column("ReleaseEvent").FromTable(TableNames.Albums);

		}
	}

	[Migration(201608022200)]
	public class ArchivedReleaseEventNotesAndData : AutoReversingMigration {

		public override void Up() {

			Create.Column("[Data]").OnTable(TableNames.ArchivedEventVersions).AsXml().Nullable();
			Create.Column("Notes").OnTable(TableNames.ArchivedEventVersions).AsString(200).NotNullable().WithDefaultValue(string.Empty);
				
		}

	}

	[Migration(201607231900)]
	public class ReleaseEventsForSongs : AutoReversingMigration {

		public override void Up() {

			Create.Column("ReleaseEvent").OnTable(TableNames.Songs).AsInt32().Nullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id");

		}

	}

	[Migration(201607072100)]
	public class ReleaseEventWebLinks : AutoReversingMigration {

		public override void Up() {

			Create.Table("ReleaseEventWebLinks")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Category").AsAnsiString(20).NotNullable()
				.WithColumn("Description").AsString(512).NotNullable()
				.WithColumn("ReleaseEvent").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEvents, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Url").AsString(512).NotNullable();

			Create.Table("ReleaseEventSeriesWebLinks")
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Category").AsAnsiString(20).NotNullable()
				.WithColumn("Description").AsString(512).NotNullable()
				.WithColumn("ReleaseEventSeries").AsInt32().NotNullable().ForeignKey(TableNames.AlbumReleaseEventSeries, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Url").AsString(512).NotNullable();

		}

	}

	[Migration(201607061900)]
	public class SongListDescriptionExtend : AutoReversingMigration {

		public override void Up() {

			Alter.Column("Description").OnTable(TableNames.SongLists).AsString(4000).NotNullable();

		}

	}

	[Migration(201607041700)]
	public class ArtistForArtistLinkType : AutoReversingMigration {

		public override void Up() {

			Create.Column("LinkType").OnTable(TableNames.GroupsForArtists).AsString(20).NotNullable().WithDefaultValue("Group");

		}

	}

	[Migration(201606232200)]
	public class ReleaseEventNameUniqueIndex : AutoReversingMigration {

		public override void Up() {

			Create.Index("IX_AlbumReleaseEvents_Name").OnTable(TableNames.AlbumReleaseEvents).OnColumn("[Name]").Unique();

		}

	}

	[Migration(201606132100)]
	public class TagsHiddenFromSuggestions : AutoReversingMigration {

		public override void Up() {

			Create.Column("HideFromSuggestions").OnTable(TableNames.Tags).AsBoolean().NotNullable().WithDefaultValue(false);

		}

	}

	[Migration(201606041900)]
	public class TrashedEntryNotes : AutoReversingMigration {

		public override void Up() {

			Create.Column("Notes").OnTable(TableNames.TrashedEntries).AsString(200).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201605291900)]
	public class OldUserNames : AutoReversingMigration {

		public override void Up() {

			Create.Table(TableNames.OldUsernames)
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("[Date]").AsDateTime().NotNullable()
				.WithColumn("[User]").AsInt32().NotNullable().ForeignKey(TableNames.Users, "Id")
				.WithColumn("OldName").AsString(400).NotNullable();

		}

	}

	[Migration(201605252100)]
	public class TagMergeRecords : AutoReversingMigration {

		public override void Up() {
			
			Create.Table(TableNames.TagMergeRecords)
				.WithColumn("Id").AsInt32().NotNullable().Identity().PrimaryKey()
				.WithColumn("Source").AsInt32().NotNullable()
				.WithColumn("Target").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id");

		}

	}

	[Migration(201605242300)]
	public class TagUsageDate : AutoReversingMigration {

		public override void Up() {
			Create.Column("[Date]").OnTable(TableNames.AlbumTagUsages).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
			Create.Column("[Date]").OnTable(TableNames.ArtistTagUsages).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
			Create.Column("[Date]").OnTable(TableNames.SongTagUsages).AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
		}

	}

	[Migration(201605231800)]
	public class TagSoftDeleted : AutoReversingMigration {

		public override void Up() {
			Create.Column("Deleted").OnTable(TableNames.Tags).AsBoolean().NotNullable().WithDefaultValue(false);
		}

	}

	[Migration(201605172100)]
	public class EventCustomName : AutoReversingMigration {

		public override void Up() {
			Create.Column("CustomName").OnTable(TableNames.AlbumReleaseEvents).AsBoolean().NotNullable().WithDefaultValue(false);
		}

	}

	[Migration(201604062130)]
	public class SongNotesExtend : Migration {

		public override void Up() {
			Delete.DefaultConstraint().OnTable(TableNames.Songs).OnColumn("Notes");
			Alter.Table(TableNames.Songs).AlterColumn("Notes").AsString(2000).NotNullable().WithDefaultValue("");
		}

		public override void Down() {
			Delete.DefaultConstraint().OnTable(TableNames.Songs).OnColumn("Notes");
			Alter.Table(TableNames.Songs).AlterColumn("Notes").AsString(800).NotNullable().WithDefaultValue("");
		}

	}

	[Migration(201603092000)]
	public class UserLanguageCodeExtend : AutoReversingMigration {

		public override void Up() {

			Alter.Table(TableNames.Users).AlterColumn("Language").AsString(10).NotNullable().WithDefaultValue("");
			Alter.Table(TableNames.Users).AlterColumn("[Culture]").AsAnsiString(10).NotNullable();

		}

	}

	[Migration(201603062100)]
	public class TrashedEntryId : AutoReversingMigration {

		public override void Up() {

			Alter.Table(TableNames.TrashedEntries).AddColumn("EntryId").AsInt32().NotNullable().WithDefaultValue(0);

		}

	}

	[Migration(201603061800)]
	public class TagReports : AutoReversingMigration {

		public override void Up() {

			Alter.Table(TableNames.EntryReports).AddColumn("Tag").AsInt32().Nullable().ForeignKey(TableNames.Tags, "Id");

		}

	}

	[Migration(201603031900)]
	public class TagCreateDate : AutoReversingMigration {

		public override void Up() {

			Alter.Table(TableNames.Tags).AddColumn("CreateDate").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);

		}

	}

	[Migration(201603031800)]
	public class TagAdditionalNamesString : AutoReversingMigration {

		public override void Up() {

			Alter.Table(TableNames.Tags).AddColumn("AdditionalNamesString").AsString(1024).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201602292300)]
	public class PVForAlbumPvIdColumnLength : Migration {

		public override void Up() {

			Alter.Table(TableNames.PVsForAlbums).AlterColumn("PVId").AsString(255).NotNullable();
				
		}

		public override void Down() {

			Alter.Table(TableNames.PVsForAlbums).AlterColumn("PVId").AsString(100).NotNullable();

		}

	}

	[Migration(201602151700)]
	public class TagWebLinks : AutoReversingMigration {

		public override void Up() {

			Create.Table(TableNames.TagWebLinks)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id").OnDelete(Rule.Cascade).Indexed("IX_TagWebLinks")
				.WithColumn("Description").AsString(512).NotNullable()
				.WithColumn("Url").AsString(512).NotNullable();

		}

	}

	[Migration(201602140100)]
	public class SupporterUser : AutoReversingMigration {

		public override void Up() {

			Create.Column("Supporter").OnTable(TableNames.UserOptions).AsBoolean().NotNullable().WithDefaultValue(false);

		}

	}

	[Migration(201602082130)]
	public class RelatedTags : AutoReversingMigration {

		public override void Up() {

			Create.Table(TableNames.RelatedTags)
				.WithColumn("Id").AsInt32().Identity().PrimaryKey()
				.WithColumn("OwnerTag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id").OnDelete(Rule.Cascade)
				.WithColumn("LinkedTag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id");

			Create.Index("IX_RelatedTags_Tag1_Tag2").OnTable(TableNames.RelatedTags)
				.OnColumn("OwnerTag").Ascending()
				.OnColumn("LinkedTag").Unique();

		}

	}

	[Migration(201601231630)]
	public class RemoveInlineFieldsFromArchivedTags : Migration {

		public override void Up() {

			Delete.Column("CategoryName").FromTable(TableNames.ArchivedTagVersions);
			Delete.Column("Description").FromTable(TableNames.ArchivedTagVersions);

		}

		public override void Down() {

			Create.Column("CategoryName").OnTable(TableNames.ArchivedTagVersions).AsString(30).NotNullable().WithDefaultValue(string.Empty);
			Create.Column("Description").OnTable(TableNames.ArchivedTagVersions).AsString(1000).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201601231230)]
	public class TranslatedTagDescription : AutoReversingMigration {

		public override void Up() {

			Create.Column("DescriptionEng").OnTable(TableNames.Tags).AsString(1000).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	/// <summary>
	/// Add index to ActivityEntries table, Author column. This is used especially on the user profile page.
	/// Add unique key between songs and users, in the FavoriteSongsForUsers table. This is a performance as well as integrity improvement.
	/// </summary>
	[Migration(201601212000)]
	public class AddIndicesMigration : Migration {

		public override void Up() {

			Create.Index("IX_ActivityEntries_Author_EditEvent").OnTable(TableNames.ActivityEntries)
				.OnColumn("Author").Ascending()
				.OnColumn("EditEvent").Ascending(); // Include EditEvent column because it's used for filtering

			if (Schema.Table(TableNames.FavoriteSongsForUsers).Index("IX_FavoriteSongsForUsers_3").Exists()) {
				Delete.Index("IX_FavoriteSongsForUsers_3").OnTable(TableNames.FavoriteSongsForUsers);
			}

			Create.Index("IX_FavoriteSongsForUsers_3").OnTable(TableNames.FavoriteSongsForUsers)
				.OnColumn("[User]").Ascending()
				.OnColumn("Song").Ascending()
				.WithOptions().Unique();

		}

		public override void Down() {

			Delete.Index("IX_ActivityEntries_Author_EditEvent").OnTable(TableNames.ActivityEntries);
			Delete.Index("IX_FavoriteSongsForUsers_3").OnTable(TableNames.FavoriteSongsForUsers);
			Create.Index("IX_FavoriteSongsForUsers_3").OnTable(TableNames.FavoriteSongsForUsers)
				.OnColumn("[User]").Ascending()
				.OnColumn("Song").Ascending();

		}

	}

	/// <summary>
	/// Add unique index for AlbumsForUsers table (User, Album). This is used especially on the user profile page.
	/// </summary>
	[Migration(201601202130)]
	public class AlbumForUserUniqueIndex : Migration {

		public override void Up() {

			if (Schema.Table(TableNames.AlbumsForUsers).Index("IX_AlbumsForUsers").Exists())
				Delete.Index("IX_AlbumsForUsers").OnTable(TableNames.AlbumsForUsers);

			Create.Index("IX_AlbumsForUsers").OnTable(TableNames.AlbumsForUsers)
				.OnColumn("[User]").Ascending()
				.OnColumn("Album").Ascending()
				.WithOptions().Unique();

		}

		public override void Down() {

			Delete.Index("IX_AlbumsForUsers").OnTable(TableNames.AlbumsForUsers);

			Create.Index("IX_AlbumsForUsers").OnTable(TableNames.AlbumsForUsers).OnColumn("[User]").Ascending();

		}

	}

	[Migration(201601161800)]
	public class TagUsageCount : Migration {

		public override void Up() {

			Create.Column("UsageCount").OnTable(TableNames.Tags).AsInt32().NotNullable().WithDefaultValue(0);

			Execute.SqlFormat(@"UPDATE {0} SET UsageCount = 
				(SELECT COUNT(*) FROM {1} WHERE Tag = {0}.Id) + (SELECT COUNT(*) FROM {2} WHERE Tag = {0}.Id) + (SELECT COUNT(*) FROM {3} WHERE Tag = {0}.Id)", 
				TableNames.Tags, TableNames.AlbumTagUsages, TableNames.ArtistTagUsages, TableNames.SongTagUsages);

		}

		public override void Down() {
			Delete.Column("UsageCount").FromTable(TableNames.Tags);
		}

	}

	[Migration(201601101900)]
	public class CreateDataForArchivedTagVersion : AutoReversingMigration {

		public override void Up() {

			Create.Column("Data").OnTable(TableNames.ArchivedTagVersions).AsXml().Nullable();
				
		}

	}

	[Migration(201512182300)]
	public class CreateTranslatedTagName : Migration {

		public override void Up() {

			Create.Table(TableNames.TagNames)
				.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
				.WithColumn("Tag").AsInt32().NotNullable().ForeignKey(TableNames.Tags, "Id").OnDelete(Rule.Cascade)
				.WithColumn("Language").AsString(16).NotNullable()
				.WithColumn("Value").AsString(255).NotNullable().Unique();

			Alter.Table(TableNames.Tags)
				.AddColumn("DefaultNameLanguage").AsString(20).NotNullable().WithDefaultValue("English")
				.AddColumn("JapaneseName").AsString(255).NotNullable().WithDefaultValue(string.Empty)
				.AddColumn("RomajiName").AsString(255).NotNullable().WithDefaultValue(string.Empty);

			Execute.Sql(string.Format("UPDATE {0} SET JapaneseName = EnglishName, RomajiName = EnglishName", TableNames.Tags));
			Execute.Sql(string.Format("INSERT INTO {0} (Tag, Language, Value) SELECT Id, 'English', EnglishName FROM {1}", TableNames.TagNames, TableNames.Tags));

		}

		public override void Down() {
			Delete.Table(TableNames.TagNames);
			Delete.Column("DefaultNameLanguage").FromTable(TableNames.Tags);
			Delete.Column("JapaneseName").FromTable(TableNames.Tags);
			Delete.Column("RomajiName").FromTable(TableNames.Tags);
		}

	}

	[Migration(201512072000)]
	public class RemoveTagName : Migration {

		public override void Up() {

			Delete.Column("[Name]").FromTable(TableNames.Tags);

		}

		public override void Down() {

			Create.Column("[Name]").OnTable(TableNames.Tags).AsString(30).NotNullable().WithDefaultValue(string.Empty);

		}

	}

	[Migration(201512011830)]
	public class TagPrimaryKey : Migration {

		private const string pkName = "PK_Tags";
		private const string ixName = "IX_Tags";

		public override void Up() {

			Delete.PrimaryKey(pkName).FromTable(TableNames.Tags);
			Create.PrimaryKey(pkName).OnTable(TableNames.Tags).Column("Id");

			Delete.ForeignKey("FK_Tags_Tags").OnTable(TableNames.Tags);
			Delete.ForeignKey("FK_Tags_Tags1").OnTable(TableNames.Tags);
			Delete.ForeignKey("FK_ActivityEntries_Tags").OnTable(TableNames.ActivityEntries);
			Delete.ForeignKey("FK_ArchivedTagVersions_Tags").OnTable(TableNames.ArchivedTagVersions);
			Delete.ForeignKey("FK_ArtistTagUsages_Tags").OnTable(TableNames.ArtistTagUsages);
			Delete.ForeignKey("FK_AlbumTagUsages_Tags").OnTable(TableNames.AlbumTagUsages);
			Delete.ForeignKey("FK_SongTagUsages_Tags").OnTable(TableNames.SongTagUsages);
			Delete.ForeignKey("FK_TagComments_Tags").OnTable(TableNames.TagComments);

			Delete.UniqueConstraint(ixName).FromTable(TableNames.Tags);

			Create.ForeignKey("FK_Tags_Tags").FromTable(TableNames.Tags).ForeignColumn("AliasedTo").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_Tags_Tags1").FromTable(TableNames.Tags).ForeignColumn("Parent").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_ActivityEntries_Tags").FromTable(TableNames.ActivityEntries).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_ArchivedTagVersions_Tags").FromTable(TableNames.ArchivedTagVersions).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_ArtistTagUsages_Tags").FromTable(TableNames.ArtistTagUsages).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_AlbumTagUsages_Tags").FromTable(TableNames.AlbumTagUsages).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_SongTagUsages_Tags").FromTable(TableNames.SongTagUsages).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");
			Create.ForeignKey("FK_TagComments_Tags").FromTable(TableNames.TagComments).ForeignColumn("Tag").ToTable(TableNames.Tags).PrimaryColumn("Id");

		}

		public override void Down() {

		}

	}

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
			if (Schema.Table(table).Index(indexName).Exists())
				Delete.Index(indexName).OnTable(table);
			Create.Index(indexName).OnTable(table).OnColumn(entityColumn).Ascending().OnColumn("Tag").Ascending().WithOptions().Unique();
		}

		private void RevertIndex(string table, string indexName, string entityColumn) {
			if (Schema.Table(table).Index(indexName).Exists())
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
