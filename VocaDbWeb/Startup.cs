#nullable disable

using System;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.CacheOutput.Extensions;
using AspNetCore.CacheOutput.InMemory.Extensions;
using Autofac;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Twitter;
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
using VocaDb.ReMikus;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Code.Security;
using VocaDb.Web.Code.WebApi;
using VocaDb.Web.Helpers;
using VocaDb.Web.Middleware;

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
			// Code from: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0
			services.Configure<SmtpSettings>(Configuration.GetSection(SmtpSettings.Smtp));

			services
				.AddControllersWithViews(options =>
				{
					// Code from: https://stackoverflow.com/questions/51411693/null-response-returns-a-204/60858295#60858295
					// remove formatter that turns nulls into 204 - No Content responses
					// this formatter breaks Angular's Http response JSON parsing
					options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();

					options.Filters.Add<VoidAndTaskTo204NoContentFilter>();
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // All properties in camel case
					options.SerializerSettings.Converters.Add(new StringEnumConverter());  // All enums as strings by default
					options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				});

			services.AddInMemoryCacheOutput();

			services.AddSwaggerGen();

			// Code from: https://blogs.lessthandot.com/index.php/webdev/serverprogramming/aspnet/adding-twitter-authentication-to-an-asp-net-core-2-site-w-cosmos-db/
			services
				.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(AuthenticationConstants.ExternalCookie)
				.AddTwitter(options =>
				{
					options.SignInScheme = AuthenticationConstants.ExternalCookie;
					options.ConsumerKey = AppConfig.TwitterConsumerKey;
					options.ConsumerSecret = AppConfig.TwitterConsumerSecret;
					options.Events = new TwitterEvents
					{
						OnCreatingTicket = context =>
						{
							// Code from: https://qiita.com/yu_ka1984/items/3b7be513a67019e71984
							var identity = (ClaimsIdentity)context.Principal.Identity;
							identity.AddClaim(new Claim(TwitterClaimTypes.AccessToken, context.AccessToken));
							return Task.CompletedTask;
						},
						OnAccessDenied = context =>
						{
							context.Response.Redirect("/");
							context.HandleResponse();
							return Task.CompletedTask;
						},
					};
				})
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/User/Login");
					// This is for external services like UTAU Wiki importer.
					// See also: https://github.com/VocaDB/vocadb/pull/783
					options.Cookie.SameSite = SameSiteMode.None;
				});

			services.AddLaravelMix();

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder
						.AllowAnyOrigin()
						.AllowAnyHeader()
						.WithMethods("GET");
				});

				options.AddPolicy(AuthenticationConstants.AuthenticatedCorsApiPolicy, builder =>
				{
					builder
						.AllowAnyHeader()
						.AllowAnyMethod()
						.WithOrigins(AppConfig.AllowedCorsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries))
						.AllowCredentials();
				});
			});
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
			builder.RegisterType<CommentQueries>().AsSelf();
			builder.RegisterType<DiscussionQueries>().AsSelf();
			builder.RegisterType<EntryQueries>().AsSelf();
			builder.RegisterType<EntryReportQueries>().AsSelf();
			builder.RegisterType<EventQueries>().AsSelf();
			builder.RegisterType<PVQueries>().AsSelf();
			builder.RegisterType<SongQueries>().AsSelf();
			builder.RegisterType<SongAggregateQueries>().AsSelf();
			builder.RegisterType<SongListQueries>().AsSelf();
			builder.RegisterType<StatsQueries>().AsSelf();
			builder.RegisterType<TagQueries>().AsSelf();
			builder.RegisterType<UserQueries>().AsSelf();
			builder.RegisterType<UserMessageQueries>().AsSelf();
			builder.RegisterType<VenueQueries>().AsSelf();

			builder.RegisterType<Login>().AsSelf();
			builder.RegisterType<PVHelper>().AsSelf();
			builder.RegisterType<ViewRenderService>().As<IViewRenderService>();

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
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}

			app.UseStatusCodePagesWithRedirects("/Error?code={0}");

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			// Code from: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "VocaDB Web API V1");
			});

			app.UseRouting();

			app.UseRequestLocalization(options =>
			{
				var supportedCultures = InterfaceLanguage.TwoLetterLanguageCodes.ToArray();
				options.AddSupportedCultures(supportedCultures);
				options.AddSupportedUICultures(supportedCultures);
			});

			// Quote from: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-5.0
			// UseCors must be called before UseResponseCaching when using UseResponseCaching.
			app.UseCors();

			// Quote from: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0#authentication-concepts
			// When using endpoint routing, the call to UseAuthentication must go:
			// After UseRouting, so that route information is available for authentication decisions.
			// Before UseEndpoints, so that users are authenticated before accessing the endpoints.
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseVocaDbPrincipal();

			app.UseResponseCaching();

			// `UseCacheOutput` must go before `UseEndpoints`, otherwise `CacheOutput` throws an `System.InvalidOperationException The response headers cannot be modified because the response has already started`.
			app.UseCacheOutput();

			app.UseEndpoints(endpoints =>
			{
				const string Numeric = "[0-9]+";

				// Ignored files
				// TODO: implement routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
				// TODO: implement routes.IgnoreRoute("favicon.ico");

				// Invalid routes - redirects to 404
				endpoints.MapControllerRoute("AlbumDetailsError", "Album/Details/{id}",
					new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
				endpoints.MapControllerRoute("ArtistDetailsError", "Artist/Details/{id}",
					new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });
				endpoints.MapControllerRoute("SongDetailsError", "Song/Details/{id}",
					new { controller = "Error", action = "NotFound" }, new { id = new IdNotNumberConstraint() });

				// Action routes
				endpoints.MapControllerRoute("Album", "Al/{id}/{friendlyName?}", new { controller = "Album", action = "Details" }, new { id = Numeric });
				endpoints.MapControllerRoute("Artist", "Ar/{id}/{friendlyName?}", new { controller = "Artist", action = "Details" }, new { id = Numeric });
				endpoints.MapControllerRoute("ReleaseEvent", "E/{id}/{slug?}", new { controller = "Event", action = "Details" }, new { id = Numeric });
				endpoints.MapControllerRoute("ReleaseEventSeries", "Es/{id}/{slug?}", new { controller = "Event", action = "SeriesDetails" }, new { id = Numeric });

				// Song shortcut, for example /S/393939
				endpoints.MapControllerRoute("Song", "S/{id}/{friendlyName?}", new { controller = "Song", action = "Details" }, new { id = Numeric });

				endpoints.MapControllerRoute("SongList", "L/{id}/{slug?}", new { controller = "SongList", action = "Details" }, new { id = Numeric });

				endpoints.MapControllerRoute("Tag", "T/{id}/{slug?}", new { controller = "Tag", action = "DetailsById" }, new { id = Numeric });

				// User profile route, for example /Profile/riipah
				endpoints.MapControllerRoute("User", "Profile/{id}", new { controller = "User", action = "Profile" });

				endpoints.MapControllerRoute("Discussion", "discussion/{**clientPath}", new { controller = "Discussion", action = "Index" });

				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
