using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NLog;
using NHibernate;
using VocaDb.Model.Mapping;
using VocaDb.Model.Mapping.Songs;
using System.Configuration;

namespace VocaDb.Model.Service
{
	public static class DatabaseConfiguration
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static string ConnectionStringName
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionStringName"];
			}
		}

		private static string GetConnectionString(string connectionStringName)
		{
			return ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString
				?? throw new ArgumentException("Connection string not found: " + connectionStringName);
		}

		public static FluentConfiguration Configure(string connectionStringName = null)
		{
			var config = Fluently.Configure()
				.Database(
					MsSqlConfiguration.MsSql2012
						.ConnectionString(c => c.Is(GetConnectionString(connectionStringName ?? ConnectionStringName)))
						.MaxFetchDepth(1)
#if !DEBUG
					.UseReflectionOptimizer()
#endif
				)
				.Cache(c => c
					.ProviderClass<NHibernate.Caches.SysCache2.SysCacheProvider>()
					.UseSecondLevelCache()
					.UseQueryCache()
				)
				.Mappings(m => m
					.FluentMappings.AddFromAssemblyOf<SongMap>()
					.Conventions.AddFromAssemblyOf<ClassConventions>()
				)
				/*.Diagnostics(d => d
					.Enable()
					.OutputToFile("C:\\Temp\\Fluent.txt")
				)*/
				;

			return config;
		}

		public static ISessionFactory BuildSessionFactory(string connectionStringName = null)
		{
			return BuildSessionFactory(Configure(connectionStringName));
		}

		public static ISessionFactory BuildSessionFactory(FluentConfiguration config)
		{
			try
			{
				return config.BuildSessionFactory();
			}
			catch (ArgumentException x)
			{
				log.Fatal(x, "Error while building session factory");
				throw;
			}
			catch (FluentConfigurationException x)
			{
				log.Fatal(x, "Error while building session factory");
				throw;
			}
		}
	}
}
