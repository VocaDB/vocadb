/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../WebLinksEditViewModel.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	// User my settings view model
	export class MySettingsViewModel {

		aboutMe: KnockoutObservable<string>;

		canVerifyEmail: KnockoutComputed<boolean>;

		email: KnockoutObservable<string>;

		emailVerified: KnockoutObservable<boolean>;

		emailVerificationSent = ko.observable(false);

		public knownLanguages: KnockoutObservableArray<UserKnownLanguageEditViewModel>;

		webLinksViewModel: WebLinksEditViewModel;

		constructor(
			private userRepository: rep.UserRepository,
			aboutMe: string, email: string, emailVerified: boolean, webLinkContracts: dc.WebLinkContract[],
			knownLanguages: dc.UserKnownLanguageContract[]) {

			this.aboutMe = ko.observable(aboutMe);
			this.email = ko.observable(email);
			this.emailVerified = ko.observable(emailVerified);
			this.knownLanguages = ko.observableArray(_.map(knownLanguages, l => new UserKnownLanguageEditViewModel(l)));
			this.webLinksViewModel = new WebLinksEditViewModel(webLinkContracts);

			// TODO: support showing the verification button by saving email immediately after it's changed
			this.canVerifyEmail = ko.computed(() => email && !emailVerified && !this.emailVerificationSent());

			/*
			this.canVerifyEmail = ko.computed(() => this.email() && !this.emailVerified());

			this.email.subscribe(() => {
				this.emailVerified(false);
			});*/

		}

		public addKnownLanguage = () => {
			this.knownLanguages.push(new UserKnownLanguageEditViewModel());
		}

		verifyEmail = () => {

			this.emailVerificationSent(true);
			this.userRepository.requestEmailVerification(() => {
				vdb.ui.showSuccessMessage("Message sent, please check your email");
			});

		}

	}

	export class UserKnownLanguageEditViewModel {

		constructor(contract?: dc.UserKnownLanguageContract) {
			this.cultureCode = ko.observable(contract != null ? contract.cultureCode : "");
			this.proficiency = ko.observable(contract != null ? contract.proficiency : "");
		}

		public cultureCode: KnockoutObservable<string>;

		public proficiency: KnockoutObservable<string>;

	}

}