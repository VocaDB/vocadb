using System.Configuration;
using System.Data.SqlClient;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using VocaDb.Model.Service;

namespace VocaDb.Tests.TestSupport {

	public class TestDatabaseFactory {

		private void RunSql(string connectionStringName, string sql) {

			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

			using (var connection = new SqlConnection(connectionString)) {

				connection.Open();

				var command = new SqlCommand(sql, connection);
				command.ExecuteNonQuery();

			}

		}

		/// <summary>
		/// Creates additional required database schemas.
		/// NHibernate schema export doesn't create any schemas, so only the dbo schema is created by default.
		/// </summary>
		private void CreateSchemas(string connectionString) {

			// SQL from http://stackoverflow.com/a/521271 (might be T-SQL specific)
			RunSql(connectionString, @"
				IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'mikudb')
				BEGIN
				EXEC('CREATE SCHEMA [mikudb]');
				END
				IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'discussions')
				BEGIN
				EXEC('CREATE SCHEMA [discussions]');
				END
			");

		}

		private void RecreateSchema(NHibernate.Cfg.Configuration cfg, string connectionStringName) {

			#if !DEBUG
			return;
			#endif

			// NH schema export does not correctly drop all constraints
			// SQL from http://stackoverflow.com/a/26348027
			RunSql(connectionStringName, @"
				exec sp_MSforeachtable ""declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name"";
			");

			var export = new SchemaExport(cfg);
			//export.SetOutputFile(@"C:\Temp\vdb.sql");
			export.Drop(false, true);
			export.Create(false, true);

		}

		public ISessionFactory BuildTestSessionFactory() {


			var testDatabaseConnectionString = "LocalDB";
			var config = DatabaseConfiguration.Configure(testDatabaseConnectionString);

			/* 
			 * Need to comment these out when not needed because session factory can only be created once.
			 * Database schemas need to be created BEFORE NHibernate schema export.
			 * This needs to be run only once.
			*/
			/*
			var fac = DatabaseConfiguration.BuildSessionFactory(config);

			CreateSchemas(fac);*/

			// Drop old database if any, create new schema
			config.ExposeConfiguration(cfg => RecreateSchema(cfg, testDatabaseConnectionString));

			var fac = DatabaseConfiguration.BuildSessionFactory(config);
			return fac;

		}

	}

}
