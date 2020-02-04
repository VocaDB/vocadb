/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../Repositories/AdminRepository.ts" />

import AdminRepository from '../../Repositories/AdminRepository';
import AlbumCollectionViewModel from './AlbumCollectionViewModel';
import CommentContract from '../../DataContracts/CommentContract';
import DeleteEntryViewModel from '../DeleteEntryViewModel';
import EditableCommentsViewModel from '../EditableCommentsViewModel';
import FollowedArtistsViewModel from './FollowedArtistsViewModel';
import HighchartsHelper from '../../Helpers/HighchartsHelper';
import RatedSongsSearchViewModel from './RatedSongsSearchViewModel';
import ReleaseEventContract from '../../DataContracts/ReleaseEvents/ReleaseEventContract';
import ResourceRepository from '../../Repositories/ResourceRepository';
import SongListsBaseViewModel from '../SongList/SongListsBaseViewModel';
import TagRepository from '../../Repositories/TagRepository';
import ui from '../../Shared/MessagesTyped';
import UrlMapper from '../../Shared/UrlMapper';
import UserEventRelationshipType from '../../Models/Users/UserEventRelationshipType';
import UserRepository from '../../Repositories/UserRepository';

//module vdb.viewModels.user {

    export default class UserDetailsViewModel {

		private static overview = "Overview";

		public addBan = () => {

			this.adminRepo.addIpToBanList({ address: this.lastLoginAddress, notes: this.name }, result => {
				if (result) {
					ui.showSuccessMessage("Added to ban list");
				} else {
					ui.showErrorMessage("Already in the ban list");
				}
			});

		}

        public checkSFS = () => {

            this.adminRepo.checkSFS(this.lastLoginAddress, html => {

                $("#sfsCheckDialog").html(html);
                $("#sfsCheckDialog").dialog("open");

            });

		};

		public comments: EditableCommentsViewModel;
		private eventsLoaded = false;
		public events = ko.observableArray<ReleaseEventContract>([]);
		public eventsType = ko.observable(UserEventRelationshipType[UserEventRelationshipType.Attending]);

		public limitedUserViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/users/" + this.userId + "/status-limited"), {
				type: 'POST', data: { reason: notes, createReport: true }, success: () => {
					window.location.reload();
				}
			});
		});

		public reportUserViewModel = new DeleteEntryViewModel(notes => {
			$.ajax(this.urlMapper.mapRelative("api/users/" + this.userId + "/reports"), {
                type: 'POST', data: { reason: notes, reportType: 'Spamming' }, success: () => {
                    ui.showSuccessMessage(vdb.resources.shared.reportSent);
                    this.reportUserViewModel.notes("");
				}
			});
		}, true);

		public initComments = () => {

			this.comments.initComments();

		};

		private initEvents = () => {

			if (this.eventsLoaded) {
				return;
			}

			this.loadEvents();
			this.eventsLoaded = true;

		}

		private loadEvents = () => {

			this.userRepo.getEvents(this.userId, UserEventRelationshipType[this.eventsType()], events => {
				this.events(events);
			});

		}

		private name: string;
		public ratingsByGenreChart = ko.observable<HighchartsOptions>(null);

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
				case "Events":
					this.initEvents();
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
	    public setViewEvents = () => this.setView("Events");

		public songLists: UserSongListsViewModel;

		constructor(
			private readonly userId: number,
			cultureCode: string,
			private loggedUserId: number,
			private lastLoginAddress: string,
			private canEditAllComments: boolean,
			private urlMapper: UrlMapper,
			private userRepo: UserRepository,
			private adminRepo: AdminRepository,
			resourceRepo: ResourceRepository,
			tagRepo: TagRepository,
			languageSelection: string,
			public followedArtistsViewModel: FollowedArtistsViewModel,
			public albumCollectionViewModel: AlbumCollectionViewModel,
			public ratedSongsViewModel: RatedSongsSearchViewModel,
			latestComments: CommentContract[]) {

			var canDeleteAllComments = (userId === loggedUserId);

			this.comments = new EditableCommentsViewModel(userRepo, userId, loggedUserId, canDeleteAllComments, canEditAllComments, false, latestComments, true);
			this.songLists = new UserSongListsViewModel(userId, userRepo, resourceRepo, tagRepo, languageSelection, cultureCode);

			window.onhashchange = () => {
				if (window.location.hash && window.location.hash.length >= 1)
					this.setView(window.location.hash.substr(1));
			};

			userRepo.getRatingsByGenre(userId, data => {
				this.ratingsByGenreChart(HighchartsHelper.simplePieChart(null, "Songs", data));
			});

			userRepo.getOne(userId, null, data => {
				this.name = data.name;
			});

			this.eventsType.subscribe(this.loadEvents);

        }

    }

	export class UserSongListsViewModel extends SongListsBaseViewModel {

		constructor(private readonly userId,
			private readonly userRepo: UserRepository,
			resourceRepo: ResourceRepository,
			tagRepo: TagRepository,
			languageSelection: string,
			cultureCode: string) {
			super(resourceRepo, tagRepo, languageSelection, cultureCode, [], true);
		}

		public loadMoreItems = (callback) => {
			this.userRepo.getSongLists(this.userId, this.query(), { start: this.start, maxEntries: 50, getTotalCount: true }, this.tagFilters.tagIds(), this.sort(), 'MainPicture', callback);
		}

	}

//}