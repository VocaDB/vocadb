using System;
using System.Configuration;
using System.Data.SqlClient;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using VocaDb.Model.Service;

namespace VocaDb.Tests.TestSupport {

	public class TestDatabaseFactory {

		private void RunSql(string connectionStringName, Action<SqlConnection> func) {

			var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;

			using (var connection = new SqlConnection(connectionString)) {

				connection.Open();

				func(connection);

			}

		}

		/// <summary>
		/// Creates additional required database schemas.
		/// NHibernate schema export doesn't create any schemas, so only the dbo schema is created by default.
		/// </summary>
		private void CreateSchemas(string connectionString) {

			// SQL from http://stackoverflow.com/a/521271 (might be T-SQL specific)
			RunSql(connectionString, connection => {

				new SqlCommand(@"
					IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'mikudb')
					BEGIN
					EXEC('CREATE SCHEMA [mikudb]');
					END
					IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'discussions')
					BEGIN
					EXEC('CREATE SCHEMA [discussions]');
					END
				", connection).ExecuteNonQuery();

			});

		}

		// Drop old database if any, create new schema
		private void RecreateSchema(NHibernate.Cfg.Configuration cfg, string connectionStringName) {

			#if !DEBUG
			return;
			#endif

			RunSql(connectionStringName, connection => {

				// NH schema export does not correctly drop all constraints
				// SQL from http://stackoverflow.com/a/26348027
				new SqlCommand(@"
					exec sp_MSforeachtable ""declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name"";
				", connection).ExecuteNonQuery();

				var export = new SchemaExport(cfg);
				//export.SetOutputFile(@"C:\Temp\vdb.sql");
				export.Execute(false, true, false, connection, null);

			});

		}

		public ISessionFactory BuildTestSessionFactory() {


			var testDatabaseConnectionString = "LocalDB";
			var config = DatabaseConfiguration.Configure(testDatabaseConnectionString);

			/* 
			 * Database schemas need to be created BEFORE NHibernate schema export.
			 * This needs to be run only once.
			*/			
			//CreateSchemas(testDatabaseConnectionString);

			config.ExposeConfiguration(cfg => RecreateSchema(cfg, testDatabaseConnectionString));

			var fac = DatabaseConfiguration.BuildSessionFactory(config);
			return fac;

		}

	}

}
