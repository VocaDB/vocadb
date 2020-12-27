#nullable disable

using System.Linq;
using System.Runtime.Caching;
using AspNetCore.CacheOutput.Extensions;
using AspNetCore.CacheOutput.InMemory.Extensions;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Database.Repositories.NHibernate;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Web;
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
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;

namespace VocaDb.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddControllersWithViews(options =>
				{
					// Code from: https://stackoverflow.com/questions/51411693/null-response-returns-a-204/60858295#60858295
					// remove formatter that turns nulls into 204 - No Content responses
					// this formatter breaks Angular's Http response JSON parsing
					options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

					options.Filters.Add<VoidAndTaskTo204NoContentFilter>();
				})
				.ConfigureApiBehaviorOptions(options =>
				{
					// Code from: https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-5.0#apicontroller-attribute
					// Disable automatic 400 response. This prevents API controllers from returning an BadRequestObjectResult when an invalid Enum value is passed.
					options.SuppressModelStateInvalidFilter = true;
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // All properties in camel case
					options.SerializerSettings.Converters.Add(new StringEnumConverter());  // All enums as strings by default
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				});

			services.AddInMemoryCacheOutput();
		}

		private static string[] LoadBlockedIPs(IComponentContext componentContext) => componentContext.Resolve<IRepository>().HandleQuery(q => q.Query<IPRule>().Select(i => i.Address).ToArray());

		public void ConfigureContainer(ContainerBuilder builder)
		{
			// Register services.
			//builder.RegisterControllers(typeof(MvcApplication).Assembly);
			//builder.RegisterType<QueryService>();
			//builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			//builder.RegisterModule(new AutofacWebTypesModule());

			builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
			builder.RegisterType<ActionContextAccessor>().As<IActionContextAccessor>().SingleInstance();
			builder.Register(x => new UrlHelperFactory().GetUrlHelper(x.Resolve<IActionContextAccessor>().ActionContext));

			// TODO: re-enable cache
			builder.Register(x => DatabaseConfiguration.BuildSessionFactory(useSysCache: false)).SingleInstance();

			// Other dependencies (for repositories mostly)
			builder.RegisterType<AspNetCoreHttpContext>().As<IHttpContext>();
			builder.RegisterType<LoginManager>().AsSelf().As<IUserPermissionContext>();
			builder.Register(x => new EntryAnchorFactory(AppConfig.HostAddress)).As<IEntryLinkFactory>();
			builder.RegisterType<UserMessageMailer>().As<IUserMessageMailer>();
			builder.RegisterType<StopForumSpamClient>().As<IStopForumSpamClient>();
			builder.RegisterType<PVParser>().As<IPVParser>();
			builder.RegisterType<DynamicImageUrlFactory>().As<IDynamicImageUrlFactory>();
			builder.RegisterType<ServerEntryThumbPersister>().As<IEntryThumbPersister>().As<IEntryPictureFilePersister>().SingleInstance();
			builder.RegisterType<ServerEntryThumbPersister>().As<IEntryThumbPersister>().SingleInstance();
			builder.RegisterType<ServerEntryImageFactoryAggregator>().As<IAggregatedEntryImageUrlFactory>();
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
			builder.RegisterType<ActivityEntryQueries>().AsSelf();
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

			builder.RegisterType<LaravelMixHelper>().AsSelf();
			builder.RegisterType<Login>().AsSelf();
			builder.RegisterType<PVHelper>().AsSelf();

			// Enable DI for action filters
			//builder.Register(c => new RestrictBlockedIPAttribute(c.Resolve<IPRuleManager>()))
			//	.AsActionFilterFor<Controller>().InstancePerRequest();

			//builder.RegisterFilterProvider();

			// Build container.
			//var container = builder.Build();

			// Set ASP.NET MVC dependency resolver.
			//DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); // For ASP.NET MVC
			//GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container); // For Web API
			//AutofacHostFactory.Container = container; // For WCF
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseCacheOutput();
		}
	}
}
