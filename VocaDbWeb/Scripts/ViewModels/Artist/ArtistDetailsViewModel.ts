/// <reference path="../../typings/knockout/knockout.d.ts" /> 
/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.viewModels {

	import cls = vdb.models;
	import dc = dataContracts;
	import rep = vdb.repositories;

	export class ArtistDetailsViewModel {

		constructor(
			repo: rep.ArtistRepository,
			private artistId: number,
			tagUsages: dc.tags.TagUsageForApiContract[],
			emailNotifications: boolean, siteNotifications: boolean,
			hasEnglishDescription: boolean,
			private unknownPictureUrl: string,
			languagePreference: cls.globalization.ContentLanguagePreference,
			private urlMapper: vdb.UrlMapper,
			private albumRepo: rep.AlbumRepository,
			private songRepo: rep.SongRepository,
			private resourceRepo: rep.ResourceRepository,
			private userRepository: rep.UserRepository,
			private cultureCode: string,
			reportTypes: IEntryReportType[],
			private loggedUserId: number,
			canDeleteAllComments: boolean,
			private pvPlayersFactory: pvs.PVPlayersFactory,
			latestComments: dc.CommentContract[]) {

			this.lang = cls.globalization.ContentLanguagePreference[languagePreference];
			this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionViewModel(artistId, emailNotifications, siteNotifications, userRepository);
			this.description = new globalization.EnglishTranslatedStringViewModel((hasEnglishDescription
				&& (languagePreference === cls.globalization.ContentLanguagePreference.English || languagePreference === cls.globalization.ContentLanguagePreference.Romaji)));

			this.comments = new EditableCommentsViewModel(repo, artistId, loggedUserId, canDeleteAllComments, canDeleteAllComments, false, latestComments, true);

			this.tagsEditViewModel = new tags.TagsEditViewModel({
				getTagSelections: callback => userRepository.getArtistTagSelections(artistId, callback),
				saveTagSelections: tags => userRepository.updateArtistTags(artistId, tags, this.tagsUpdated)
			}, callback => repo.getTagSuggestions(this.artistId, callback));

			this.tagUsages = new tags.TagListViewModel(tagUsages);

			this.reportViewModel = new ReportEntryViewModel(reportTypes, (reportType, notes) => {

				repo.createReport(this.artistId, reportType, notes, null);

				vdb.ui.showSuccessMessage(vdb.resources.shared.reportSent);

			});

			this.loadHighcharts();

		}

		public comments: EditableCommentsViewModel;

		customizeSubscriptionDialog: CustomizeArtistSubscriptionViewModel;

		private lang: string;

		private loadHighcharts = () => {
			
			// Delayed load highcharts stuff
			var highchartsPromise = $.getScript(this.urlMapper.mapRelative("scripts/highcharts/4.2.0/highcharts.js"));
			var highchartsHelperPromise = $.getScript(this.urlMapper.mapRelative("/scripts/helpers/HighchartsHelper.js"));
			var songsPerMonthDataPromise = this.songRepo.getOverTime(vdb.models.aggregate.TimeUnit.month, this.artistId);

			$.when(songsPerMonthDataPromise, highchartsPromise, highchartsHelperPromise)
				.done((songsPerMonthData: JQueryPromiseCallback<dataContracts.aggregate.CountPerDayContract[]>) => {

				var points: dataContracts.aggregate.CountPerDayContract[] = songsPerMonthData[0];

				// Need at least 2 points because lone point looks weird
				if (points && points.length >= 2) {
					this.songsOverTimeChart(vdb.helpers.HighchartsHelper.dateLineChartWithAverage('Songs per month', null, 'Songs', points));					
				}

			});

		}

		public showAllMembers = ko.observable(false);
		public description: globalization.EnglishTranslatedStringViewModel;
		public songsViewModel: KnockoutObservable<vdb.viewModels.search.SongSearchViewModel> = ko.observable(null);

		public songsOverTimeChart = ko.observable<HighchartsOptions>(null);

		public collaborationAlbumsViewModel: KnockoutObservable<vdb.viewModels.search.AlbumSearchViewModel> = ko.observable(null);
		public mainAlbumsViewModel: KnockoutObservable<vdb.viewModels.search.AlbumSearchViewModel> = ko.observable(null);

		public reportViewModel: ReportEntryViewModel;

		public tagsEditViewModel: tags.TagsEditViewModel;

		public tagUsages: tags.TagListViewModel;

		private tagsUpdated = (usages: dc.tags.TagUsageForApiContract[]) => {

			this.tagUsages.tagUsages(_.sortBy(usages, u => -u.count));

		}

		public initMainAlbums = () => {
			
			if (this.mainAlbumsViewModel())
				return;

			this.mainAlbumsViewModel(new vdb.viewModels.search.AlbumSearchViewModel(null, this.unknownPictureUrl, this.lang,
				this.albumRepo, null, this.resourceRepo, this.cultureCode, null, [ this.artistId ], null, "Unknown", null));
			this.mainAlbumsViewModel().artistFilters.artistParticipationStatus("OnlyMainAlbums");

		};

		public initCollaborationAlbums = () => {

			if (this.collaborationAlbumsViewModel())
				return;

			this.collaborationAlbumsViewModel(new vdb.viewModels.search.AlbumSearchViewModel(null, this.unknownPictureUrl, this.lang,
				this.albumRepo, null, this.resourceRepo, this.cultureCode, null, [ this.artistId ], null, "Unknown", null));
			this.collaborationAlbumsViewModel().artistFilters.artistParticipationStatus("OnlyCollaborations");

		};

		public initSongs = () => {

			if (this.songsViewModel())
				return;

			this.songsViewModel(new vdb.viewModels.search.SongSearchViewModel(null, this.urlMapper, this.lang, this.songRepo, null, this.userRepository, this.resourceRepo,
				this.cultureCode, this.loggedUserId, null, [ this.artistId ], null, null, false, false, null, null, null, null, null, this.pvPlayersFactory));
			this.songsViewModel().updateResults(true);

		}

	}

	export class CustomizeArtistSubscriptionViewModel {
		
		public dialogVisible = ko.observable(false);

		public notificationsMethod: KnockoutObservable<string>;

		constructor(artistId: number, emailNotifications: boolean, siteNotifications: boolean, userRepository: rep.UserRepository) {

			this.notificationsMethod = ko.observable(!siteNotifications ? "Nothing" : (!emailNotifications ? "Site" : "Email"));

			this.notificationsMethod.subscribe(method => {
				userRepository.updateArtistSubscription(artistId, method == "Email", method == "Site" || method == "Email");
			});

		}

		public show = () => {

			this.dialogVisible(true);

		};

	}

}