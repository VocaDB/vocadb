import { UserForMySettingsContract } from '@/DataContracts/User/UserForMySettingsContract';
import {
	UserKnownLanguageContract,
	UserLanguageProficiency,
} from '@/DataContracts/User/UserKnownLanguageContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { UserEmailOptions } from '@/Models/Users/UserEmailOptions';
import { UserRepository } from '@/Repositories/UserRepository';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

class UserKnownLanguageEditStore {
	@observable cultureCode: string;
	@observable proficiency: UserLanguageProficiency;

	constructor(contract?: UserKnownLanguageContract) {
		makeObservable(this);

		this.cultureCode = contract?.cultureCode ?? '';
		this.proficiency = contract?.proficiency ?? UserLanguageProficiency.Nothing;
	}
}

export class MySettingsStore {
	@observable aboutMe: string;
	@observable cultureSelection: string;
	@observable defaultLanguageSelection: ContentLanguagePreference;
	@observable email: string;
	@observable emailOptions: UserEmailOptions;
	@observable emailVerified: boolean;
	@observable emailVerificationSent = false;
	@observable errors?: Record<string, string[]>;
	@observable interfaceLanguageSelection: string;
	@observable knownLanguages: UserKnownLanguageEditStore[];
	@observable location: string;
	@observable newPass = '';
	@observable newPassAgain = '';
	@observable oldPass = '';
	@observable preferredVideoService: string /* TODO: enum */;
	@observable publicAlbumCollection: boolean;
	@observable publicRatings: boolean;
	@observable showActivity: boolean;
	@observable showChatbox: boolean;
	@observable stylesheet: string;
	@observable submitting = false;
	@observable unreadNotificationsToKeep: string;
	@observable username: string;
	@observable extendLanguages = false;
	readonly webLinksStore: WebLinksEditStore;

	constructor(
		private readonly userRepo: UserRepository,
		readonly contract: UserForMySettingsContract,
	) {
		makeObservable(this);

		this.aboutMe = contract.aboutMe;
		this.cultureSelection = contract.culture;
		this.defaultLanguageSelection = contract.defaultLanguageSelection;
		this.email = contract.email;
		this.emailOptions = contract.emailOptions;
		this.emailVerified = contract.emailVerified;
		this.interfaceLanguageSelection = contract.language;
		this.knownLanguages = contract.knownLanguages.map(
			(l) => new UserKnownLanguageEditStore(l),
		);
		this.location = contract.location;
		this.preferredVideoService = contract.preferredVideoService;
		this.publicAlbumCollection = contract.publicAlbumCollection;
		this.publicRatings = contract.publicRatings;
		this.showActivity = !contract.anonymousActivity;
		this.showChatbox = contract.showChatbox;
		this.stylesheet = contract.stylesheet ?? '';
		this.unreadNotificationsToKeep =
			contract.unreadNotificationsToKeep.toString();
		this.username = contract.name;
		this.webLinksStore = new WebLinksEditStore(contract.webLinks);

		if (
			contract.knownLanguages.filter((lang) => lang.cultureCode.length > 2)
				.length > 0
		) {
			this.extendLanguages = true;
		}
	}

	// TODO: support showing the verification button by saving email immediately after it's changed
	@computed get canVerifyEmail(): boolean {
		return !!this.email && !this.emailVerified && !this.emailVerificationSent;
	}

	@action addKnownLanguage = (): void => {
		this.knownLanguages.push(new UserKnownLanguageEditStore());
	};

	@action removeKnownLanguage = (
		knownLanguage: UserKnownLanguageEditStore,
	): void => {
		pull(this.knownLanguages, knownLanguage);
	};

	@action verifyEmail = async (): Promise<void> => {
		this.emailVerificationSent = true;
		await this.userRepo.requestEmailVerification({});
	};

	@action submit = async (pictureUpload: File | undefined): Promise<string> => {
		this.submitting = true;

		try {
			const name = await this.userRepo.updateMySettings(
				{
					aboutMe: this.aboutMe,
					anonymousActivity: !this.showActivity,
					culture: this.cultureSelection,
					defaultLanguageSelection: this.defaultLanguageSelection,
					email: this.email,
					emailOptions: this.emailOptions,
					id: this.contract.id,
					knownLanguages: this.knownLanguages,
					language: this.interfaceLanguageSelection,
					location: this.location,
					name: this.username,
					newPass: this.newPass,
					newPassAgain: this.newPassAgain,
					oldPass: this.oldPass,
					preferredVideoService: this.preferredVideoService,
					publicAlbumCollection: this.publicAlbumCollection,
					publicRatings: this.publicRatings,
					showChatbox: this.showChatbox,
					stylesheet: this.stylesheet,
					unreadNotificationsToKeep: Number(this.unreadNotificationsToKeep),
					webLinks: this.webLinksStore.toContracts(),
				},
				pictureUpload,
			);

			return name;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
