/// <reference path="../../typings/knockout/knockout.d.ts" /> 
/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.viewModels {

	import rep = vdb.repositories;

	export class ArtistDetailsViewModel {

		customizeSubscriptionDialog: CustomizeArtistSubscriptionViewModel;

		constructor(
			private artistId: number, emailNotifications: boolean, siteNotifications: boolean,
			private unknownPictureUrl: string,
			private lang: string,
			private urlMapper: vdb.UrlMapper,
			private albumRepo: rep.AlbumRepository,
			private songRepo: rep.SongRepository,
			private resourceRepo: rep.ResourceRepository,
			private userRepository: rep.UserRepository,
			private cultureCode: string,
			private pvPlayerWrapperElement: HTMLElement) {

			this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionViewModel(artistId, emailNotifications, siteNotifications, userRepository);

		}

		public songsViewModel: KnockoutObservable<vdb.viewModels.search.SongSearchViewModel> = ko.observable(null);
		public collaborationAlbumsViewModel: KnockoutObservable<vdb.viewModels.search.AlbumSearchViewModel> = ko.observable(null);
		public mainAlbumsViewModel: KnockoutObservable<vdb.viewModels.search.AlbumSearchViewModel> = ko.observable(null);

		public initMainAlbums = () => {
			
			if (this.mainAlbumsViewModel())
				return;

			this.mainAlbumsViewModel(new vdb.viewModels.search.AlbumSearchViewModel(null, this.unknownPictureUrl, this.lang, this.albumRepo, null, this.resourceRepo, this.cultureCode, null, this.artistId, "Unknown"));
			this.mainAlbumsViewModel().artistParticipationStatus("OnlyMainAlbums");

		};

		public initCollaborationAlbums = () => {

			if (this.collaborationAlbumsViewModel())
				return;

			this.collaborationAlbumsViewModel(new vdb.viewModels.search.AlbumSearchViewModel(null, this.unknownPictureUrl, this.lang, this.albumRepo, null, this.resourceRepo, this.cultureCode, null, this.artistId, "Unknown"));
			this.collaborationAlbumsViewModel().artistParticipationStatus("OnlyCollaborations");

		};

		public initSongs = () => {

			if (this.songsViewModel())
				return;

			this.songsViewModel(new vdb.viewModels.search.SongSearchViewModel(null, this.urlMapper, this.lang, this.songRepo, null, this.userRepository, this.resourceRepo,
				this.cultureCode, null, null, this.artistId, null, false, this.pvPlayerWrapperElement));
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