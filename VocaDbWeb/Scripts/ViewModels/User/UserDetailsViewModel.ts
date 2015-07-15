/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../Repositories/AdminRepository.ts" />

module vdb.viewModels.user {

	import dc = vdb.dataContracts;
    import rep = vdb.repositories;

    export class UserDetailsViewModel {

		private static overview = "Overview";

        public checkSFS = (ip: string) => {

            this.adminRepo.checkSFS(ip, html => {

                $("#sfsCheckDialog").html(html);
                $("#sfsCheckDialog").dialog("open");

            });

		};

		public comments: EditableCommentsViewModel;

		public getRatingsByGenre = (callback: (data: HighchartsOptions) => void) => {

			var url = this.urlMapper.mapRelative('/User/SongsPerGenre/' + this.userId);
			$.getJSON(url, data => {
				callback(vdb.helpers.HighchartsHelper.simplePieChart(null, "Songs", data));
			});

		}

		public initComments = () => {

			this.comments.initComments();

		};

		public view = ko.observable(UserDetailsViewModel.overview);

		private initializeView = (viewName: string) => {

			switch (viewName) {
				case "Albums":
					this.albumCollectionViewModel.init();
					break;
				case "Artists":
					this.followedArtistsViewModel.init();
					break;
				case "Comments":
					this.initComments();
					break;
				case "CustomLists":
					this.songLists.init();
					break;
				case "Songs":
					this.ratedSongsViewModel.init();
					break;
			}
			
		}

		public setView = (viewName: string) => {

			if (!viewName)
				viewName = UserDetailsViewModel.overview;

			this.initializeView(viewName);

			window.scrollTo(0, 0);
			window.location.hash = viewName != UserDetailsViewModel.overview ? viewName : "";
			this.view(viewName);		

		}

		public setOverview = () => this.setView("Overview");
		public setViewAlbums = () => this.setView("Albums");
		public setViewArtists = () => this.setView("Artists");
		public setComments = () => this.setView("Comments");
		public setCustomLists = () => this.setView("CustomLists");
		public setViewSongs = () => this.setView("Songs");

		public songLists: UserSongListsViewModel;

		constructor(
			private userId: number,
			private loggedUserId: number,
			private canEditAllComments: boolean,
			private urlMapper: UrlMapper,
			private userRepo: rep.UserRepository,
			private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel,
			latestComments: dc.CommentContract[]) {

			var canDeleteAllComments = (userId === loggedUserId);

			this.comments = new EditableCommentsViewModel(userRepo, userId, loggedUserId, canDeleteAllComments, canEditAllComments, false, latestComments, true);
			this.songLists = new UserSongListsViewModel(userId, userRepo);

			window.onhashchange = () => {
				if (window.location.hash && window.location.hash.length >= 1)
					this.setView(window.location.hash.substr(1));
			};

        }

    }

	export class UserSongListsViewModel {
		
		constructor(private userId, private userRepo: rep.UserRepository) {
			
			this.sort.subscribe(this.reset);

		}

		public hasMore = ko.observable(false);

		public reset = () => {
			this.start = 0;
			this.lists([]);
			this.loadMore();
		}

		public init = () => {
			
			if (this.isInit)
				return;

			this.loadMore();

			this.isInit = true;

		}

		private isInit = false;

		public lists = ko.observableArray<dc.SongListContract>([]);

		public loadMore = () => {
			
			this.userRepo.getSongLists(this.userId, { start: this.start, maxEntries: 50, getTotalCount: true }, this.sort(), 'MainPicture', result => {
				ko.utils.arrayPushAll(this.lists, result.items);
				this.start += result.items.length;
				this.hasMore(this.start < result.totalCount);
			});

		}

		private start = 0;

		public sort = ko.observable("Name");

	}

}