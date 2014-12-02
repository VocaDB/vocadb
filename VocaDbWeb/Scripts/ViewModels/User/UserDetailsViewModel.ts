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

		public createComment = () => {

			var comment = this.newComment();

			if (!comment)
				return;

			this.newComment("");

			var url = this.urlMapper.mapRelative("/User/CreateComment");
			$.post(url, { entryId: this.userId, message: comment }, (result: dc.CommentContract) => {
				this.processComment(result);
				this.comments.unshift(result);
			});


		}

		public deleteComment = (profileComment: dc.CommentContract) => {

			this.userRepo.deleteComment(profileComment.id, () => {
				this.comments.remove(profileComment);
			});

		};

		public getRatingsByGenre = (callback: (data: HighchartsOptions) => void) => {

			var url = this.urlMapper.mapRelative('/User/SongsPerGenre/' + this.userId);
			$.getJSON(url, data => {
				callback(vdb.helpers.HighchartsHelper.simplePieChart(null, "Songs", data));
			});

		}

		public initComments = () => {

			if (this.comments().length)
				return;

			this.userRepo.getProfileComments(this.userId, { start: 0, maxEntries: 300, getTotalCount: false }, (result: dc.PartialFindResultContract<dc.CommentContract>) => {

				_.forEach(result.items, this.processComment);

				this.comments(result.items);

			});

		};

		public comments = ko.observableArray<dc.CommentContract>();
		public newComment = ko.observable("");
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
				case "Songs":
					this.ratedSongsViewModel.init();
					break;
			}
			
		}

		private processComment = (comment: dc.CommentContract) => {

			var commentAny: any = comment;
			commentAny.canBeDeleted = (this.canDeleteComments || this.userId == this.loggedUserId || (comment.author && comment.author.id == this.loggedUserId));

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

		constructor(
			private userId: number,
			private loggedUserId: number,
			private canDeleteComments: boolean,
			private urlMapper: UrlMapper,
			private userRepo: rep.UserRepository,
			private adminRepo: rep.AdminRepository,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel) {

			window.onhashchange = () => {
				if (window.location.hash && window.location.hash.length >= 1)
					this.setView(window.location.hash.substr(1));
			};

        }

    }

}