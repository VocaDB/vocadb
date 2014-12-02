using NHibernate;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.BrandableStrings;

namespace VocaDb.Model.Service {

	public class ServiceModel {

		private readonly BrandableStringsManager brandableStringsManager;
		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IUserPermissionContext permissionContext;
		private readonly ISessionFactory sessionFactory;

		public ServiceModel(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory, BrandableStringsManager brandableStringsManager) {
			this.sessionFactory = sessionFactory;
			this.permissionContext = permissionContext;
			this.entryLinkFactory = entryLinkFactory;
			this.brandableStringsManager = brandableStringsManager;
		}

		public ActivityFeedService ActivityFeed {
			get {
				return new ActivityFeedService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public AdminService Admin {
			get {
				return new AdminService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public AlbumService Albums {
			get {
				return new AlbumService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public ArtistService Artists {
			get {
				return new ArtistService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public MikuDbAlbumService MikuDbAlbums {
			get {
				return new MikuDbAlbumService(sessionFactory, permissionContext, entryLinkFactory, this);				
			}
		}

		public NewsEntryService NewsEntry {
			get {
				return new NewsEntryService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public OtherService Other {
			get {
				return new OtherService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public RankingService Rankings {
			get {
				return new RankingService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public ReleaseEventService ReleaseEvents {
			get {
				return new ReleaseEventService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public SongService Songs {
			get {
				return new SongService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public TagService Tags {
			get {
				return new TagService(sessionFactory, permissionContext, entryLinkFactory);
			}
		}

		public UserService Users {
			get {
				return new UserService(sessionFactory, permissionContext, entryLinkFactory, brandableStringsManager);
			}
		}

	}
}
