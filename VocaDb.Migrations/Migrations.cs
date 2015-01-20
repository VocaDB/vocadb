using FluentMigrator;

namespace VocaDb.Migrations {

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

}
