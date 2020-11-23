using Autofac;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Tests.DatabaseTests;

namespace VocaDb.Tests.TestSupport
{

	/// <summary>
	/// Creates AutoFac dependency injection container for database tests.
	/// </summary>
	public static class TestContainerFactory
	{

		private static ISessionFactory BuildTestSessionFactory()
		{
			return new TestDatabaseFactory().BuildTestSessionFactory();
		}

		public static IContainer BuildContainer()
		{

			var builder = new ContainerBuilder();

			builder.Register(x => BuildTestSessionFactory()).SingleInstance();
			builder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).InstancePerLifetimeScope();
			builder.RegisterType<TestDatabase>().AsSelf();
			builder.Register(x => new FakePermissionContext { Name = "Miku" }).As<IUserPermissionContext>();
			builder.RegisterType<EntryUrlParser>().As<IEntryUrlParser>().SingleInstance();
			builder.Register(x => new NHibernateDatabaseContext(x.Resolve<ISession>(), x.Resolve<IUserPermissionContext>())).As<IDatabaseContext>();

			builder.RegisterType<AlbumNHibernateRepository>().As<IAlbumRepository>();
			builder.RegisterType<ArtistNHibernateRepository>().As<IArtistRepository>();
			builder.RegisterType<EntryReportNHibernateRepository>().As<IEntryReportRepository>();
			builder.RegisterType<EventNHibernateRepository>().As<IEventRepository>();
			builder.RegisterType<SongNHibernateRepository>().As<ISongRepository>();
			builder.RegisterType<SongListNHibernateRepository>().As<ISongListRepository>();
			builder.RegisterType<TagNHibernateSessionPerRequestRepository>().As<ITagRepository>().InstancePerLifetimeScope();
			builder.RegisterType<UserNHibernateRepository>().As<IUserRepository>();
			builder.RegisterType<UserMessageNHibernateRepository>().As<IUserMessageRepository>();

			var container = builder.Build();
			return container;

		}

	}

}
