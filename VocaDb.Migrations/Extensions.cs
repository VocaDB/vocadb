using FluentMigrator.Builders.Execute;

namespace VocaDb.Migrations {

	public static class Extensions {

		public static void SqlFormat(this IExecuteExpressionRoot execute, string format, params object[] args) {
			execute.Sql(string.Format(format, args));
		}

	}

}
