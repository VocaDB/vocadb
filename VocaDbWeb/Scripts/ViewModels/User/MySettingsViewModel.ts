import ui from '../../Shared/MessagesTyped';
import UserKnownLanguageContract from '../../DataContracts/User/UserKnownLanguageContract';
import UserRepository from '../../Repositories/UserRepository';
import WebLinkContract from '../../DataContracts/WebLinkContract';
import WebLinksEditViewModel from '../WebLinksEditViewModel';

	// User my settings view model
	export default class MySettingsViewModel {

		aboutMe: KnockoutObservable<string>;

		canVerifyEmail: KnockoutComputed<boolean>;

		email: KnockoutObservable<string>;

		emailVerified: KnockoutObservable<boolean>;

		emailVerificationSent = ko.observable(false);

		public knownLanguages: KnockoutObservableArray<UserKnownLanguageEditViewModel>;

		webLinksViewModel: WebLinksEditViewModel;

		constructor(
			private userRepository: UserRepository,
			aboutMe: string, email: string, emailVerified: boolean, webLinkContracts: WebLinkContract[],
			knownLanguages: UserKnownLanguageContract[]) {

			this.aboutMe = ko.observable(aboutMe);
			this.email = ko.observable(email);
			this.emailVerified = ko.observable(emailVerified);
			this.knownLanguages = ko.observableArray(_.map(knownLanguages, l => new UserKnownLanguageEditViewModel(l)));
			this.webLinksViewModel = new WebLinksEditViewModel(webLinkContracts);

			// TODO: support showing the verification button by saving email immediately after it's changed
			this.canVerifyEmail = ko.computed(() => email && !emailVerified && !this.emailVerificationSent());

		}

		public addKnownLanguage = () => {
			this.knownLanguages.push(new UserKnownLanguageEditViewModel());
		}

		verifyEmail = () => {

			this.emailVerificationSent(true);
			this.userRepository.requestEmailVerification(() => {
				ui.showSuccessMessage("Message sent, please check your email");
			});

		}

	}

	export class UserKnownLanguageEditViewModel {

		constructor(contract?: UserKnownLanguageContract) {
			this.cultureCode = ko.observable(contract != null ? contract.cultureCode : "");
			this.proficiency = ko.observable(contract != null ? contract.proficiency : "");
		}

		public cultureCode: KnockoutObservable<string>;

		public proficiency: KnockoutObservable<string>;

	}