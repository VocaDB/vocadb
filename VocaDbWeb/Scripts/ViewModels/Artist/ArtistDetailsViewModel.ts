/// <reference path="../../typings/knockout/knockout.d.ts" /> 
/// <reference path="../../Repositories/UserRepository.ts" />

module vdb.viewModels {

	import rep = vdb.repositories;

	export class ArtistDetailsViewModel {

		customizeSubscriptionDialog: CustomizeArtistSubscriptionViewModel;

		constructor(artistId: number, emailNotifications: boolean, siteNotifications: boolean, userRepository: rep.UserRepository) {

			this.customizeSubscriptionDialog = new CustomizeArtistSubscriptionViewModel(artistId, emailNotifications, siteNotifications, userRepository);

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