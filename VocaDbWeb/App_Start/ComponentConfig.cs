using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using Autofac.Integration.WebApi;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.ExtSites;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Model.Service.Translations;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Helpers;
using VocaDb.Web.Services;

namespace VocaDb.Web.App_Start {

	/// <summary>
	/// Configures AutoFac dependency injection.
	/// </summary>
	public static class ComponentConfig {

		private static string[] LoadBlockedIPs(IComponentContext componentContext) {

			return componentContext.Resolve<OtherService>().GetIPRules().Select(i => i.Address).ToArray();

		}

		public static void RegisterComponent() {

			var builder = new ContainerBuilder();

			// Register services.
			builder.RegisterControllers(typeof(MvcApplication).Assembly);
			builder.RegisterType<QueryService>();
			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			builder.RegisterModule(new AutofacWebTypesModule());

			builder.Register(x => DatabaseConfiguration.BuildSessionFactory()).SingleInstance();

			// Other dependencies (for repositories mostly)
			builder.RegisterType<LoginManager>().As<IUserPermissionContext>();
			builder.Register(x => new EntryAnchorFactory(AppConfig.HostAddress)).As<IEntryLinkFactory>();
			builder.RegisterType<UserMessageMailer>().As<IUserMessageMailer>();
			builder.RegisterType<StopForumSpamClient>().As<IStopForumSpamClient>();
			builder.RegisterType<PVParser>().As<IPVParser>();
			builder.RegisterType<ServerEntryImagePersisterOld>().As<IEntryImagePersisterOld>().As<IEntryPictureFilePersister>();
			builder.RegisterType<ServerEntryThumbPersister>().As<IEntryThumbPersister>();
			builder.RegisterType<NTextCatLibLanguageDetector>().As<ILanguageDetector>();
			builder.RegisterType<BrandableStringsManager>().AsSelf().SingleInstance();
			builder.RegisterType<VdbConfigManager>().AsSelf().SingleInstance();
			builder.RegisterType<GravatarUserIconFactory>().As<IUserIconFactory>();
			builder.RegisterType<EntryUrlParser>().As<IEntryUrlParser>().SingleInstance();
			builder.RegisterType<AlbumDescriptionGenerator>().AsSelf().SingleInstance();
			builder.RegisterType<MarkdownParser>().AsSelf();
			builder.Register(x => new IPRuleManager(LoadBlockedIPs(x))).AsSelf().SingleInstance();
			builder.Register(_ => MemoryCache.Default).As<ObjectCache>().ExternallyOwned(); // Disable dispose
			builder.RegisterType<EntryForApiContractFactory>().AsSelf();
			builder.RegisterType<EnumTranslations>().As<IEnumTranslations>();
			builder.RegisterType<EntrySubTypeNameFactory>().As<IEntrySubTypeNameFactory>();
			builder.RegisterType<FollowedArtistNotifier>().As<IFollowedArtistNotifier>();

			// Legacy services
			builder.RegisterType<ActivityFeedService>().AsSelf();
			builder.RegisterType<AdminService>().AsSelf();
			builder.RegisterType<AlbumService>().AsSelf();
			builder.RegisterType<ArtistService>().AsSelf();
			builder.RegisterType<MikuDbAlbumService>().AsSelf();
			builder.RegisterType<OtherService>().AsSelf();
			builder.RegisterType<ReleaseEventService>().AsSelf();
			builder.RegisterType<SongService>().AsSelf();
			builder.RegisterType<UserService>().AsSelf();

			// Repositories
			builder.RegisterType<NHibernateRepository>().As<IRepository>();
			builder.RegisterType<AlbumNHibernateRepository>().As<IAlbumRepository>();
			builder.RegisterType<ArtistNHibernateRepository>().As<IArtistRepository>();
			builder.RegisterType<DiscussionFolderNHibernateRepository>().As<IDiscussionFolderRepository>();
			builder.RegisterType<EntryReportNHibernateRepository>().As<IEntryReportRepository>();
			builder.RegisterType<EventNHibernateRepository>().As<IEventRepository>();
			builder.RegisterType<SongNHibernateRepository>().As<ISongRepository>();
			builder.RegisterType<SongListNHibernateRepository>().As<ISongListRepository>();
			builder.RegisterType<TagNHibernateRepository>().As<ITagRepository>();
			builder.RegisterType<UserNHibernateRepository>().As<IUserRepository>();
			builder.RegisterType<UserMessageNHibernateRepository>().As<IUserMessageRepository>();
			builder.RegisterType<VenueNHibernateRepository>().As<IVenueRepository>();
			builder.RegisterType<AlbumQueries>().AsSelf();
			builder.RegisterType<ArtistQueries>().AsSelf();
			builder.RegisterType<DiscussionQueries>().AsSelf();
			builder.RegisterType<EntryQueries>().AsSelf();
			builder.RegisterType<EntryReportQueries>().AsSelf();
			builder.RegisterType<EventQueries>().AsSelf();
			builder.RegisterType<SongQueries>().AsSelf();
			builder.RegisterType<SongAggregateQueries>().AsSelf();
			builder.RegisterType<SongListQueries>().AsSelf();
			builder.RegisterType<TagQueries>().AsSelf();
			builder.RegisterType<UserQueries>().AsSelf();
			builder.RegisterType<UserMessageQueries>().AsSelf();
			builder.RegisterType<VenueQueries>().AsSelf();

			// Enable DI for action filters
			builder.Register(c => new RestrictBlockedIPAttribute(c.Resolve<IPRuleManager>()))
                .AsActionFilterFor<Controller>().InstancePerRequest();

			builder.RegisterFilterProvider();

			// Build container.
			var container = builder.Build();

			// Set ASP.NET MVC dependency resolver.			
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); // For ASP.NET MVC			
			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container); // For Web API			
			AutofacHostFactory.Container = container; // For WCF

		}

	}

}