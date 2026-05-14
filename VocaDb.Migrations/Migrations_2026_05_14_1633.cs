using FluentMigrator;

namespace VocaDb.Migrations
{
	[Migration(2026_05_14_1633)]
	public class ChangeRatingColumnsToDecimal : Migration
	{
		public override void Up()
		{
			// Per-user rating: allow one decimal place (e.g. 0.5)
			Alter.Column("Rating").OnTable(TableNames.AlbumsForUsers).AsDecimal(3, 1).NotNullable().WithDefaultValue(0.0m);

			// Album aggregate total: allow one decimal place and larger range for totals
			Alter.Column("RatingTotal").OnTable(TableNames.Albums).AsDecimal(9, 1).NotNullable().WithDefaultValue(0.0m);
		}

		public override void Down()
		{
			// Revert to previous integer columns
			Alter.Column("Rating").OnTable(TableNames.AlbumsForUsers).AsInt32().NotNullable().WithDefaultValue(0);
			Alter.Column("RatingTotal").OnTable(TableNames.Albums).AsInt32().NotNullable().WithDefaultValue(0);
		}
	}
}
