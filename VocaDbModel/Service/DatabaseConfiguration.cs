#nullable disable

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
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

		private static string ConnectionStringName => ConfigurationManager.AppSettings["ConnectionStringName"];

		private static string GetConnectionString(string connectionStringName)
		{
			return ConfigurationManager.ConnectionStrings[connectionStringName]?.ConnectionString
				?? throw new ArgumentException("Connection string not found: " + connectionStringName);
		}

		public static FluentConfiguration Configure(string connectionStringName = null, bool useSysCache = true)
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
				.Mappings(m => m
					.FluentMappings.AddFromAssemblyOf<SongMap>()
					.Conventions.AddFromAssemblyOf<ClassConventions>()
				)
				/*.Diagnostics(d => d
					.Enable()
					.OutputToFile("C:\\Temp\\Fluent.txt")
				)*/
				;

			if (useSysCache)
			{
				config
					.Cache(c => c
						.ProviderClass<NHibernate.Caches.SysCache2.SysCacheProvider>()
						.UseSecondLevelCache()
						.UseQueryCache()
					);
			}

			return config;
		}

		public static ISessionFactory BuildSessionFactory(string connectionStringName = null, bool useSysCache = true)
		{
			return BuildSessionFactory(Configure(connectionStringName, useSysCache));
		}

		public static ISessionFactory BuildSessionFactory(FluentConfiguration config)
		{
			try
			{
				return config.BuildSessionFactory();
			}
			catch (ArgumentException x)
			{
				s_log.Fatal(x, "Error while building session factory");
				throw;
			}
			catch (FluentConfigurationException x)
			{
				s_log.Fatal(x, "Error while building session factory");
				throw;
			}
		}
	}
}
