import { UserForMySettingsContract } from '@/DataContracts/User/UserForMySettingsContract';
import {
	UserKnownLanguageContract,
	UserLanguageProficiency,
} from '@/DataContracts/User/UserKnownLanguageContract';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { UserEmailOptions } from '@/Models/Users/UserEmailOptions';
import { UserRepository } from '@/Repositories/UserRepository';
import { WebLinksEditStore } from '@/Stores/WebLinksEditStore';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

class UserKnownLanguageEditStore {
	@observable public cultureCode: string;
	@observable public proficiency: UserLanguageProficiency;

	public constructor(contract?: UserKnownLanguageContract) {
		makeObservable(this);

		this.cultureCode = contract?.cultureCode ?? '';
		this.proficiency = contract?.proficiency ?? UserLanguageProficiency.Nothing;
	}
}

export class MySettingsStore {
	@observable public aboutMe: string;
	@observable public cultureSelection: string;
	@observable public defaultLanguageSelection: ContentLanguagePreference;
	@observable public email: string;
	@observable public emailOptions: UserEmailOptions;
	@observable public emailVerified: boolean;
	@observable public emailVerificationSent = false;
	@observable public errors?: Record<string, string[]>;
	@observable public interfaceLanguageSelection: string;
	@observable public knownLanguages: UserKnownLanguageEditStore[];
	@observable public location: string;
	@observable public newPass = '';
	@observable public newPassAgain = '';
	@observable public oldPass = '';
	@observable public preferredVideoService: string /* TODO: enum */;
	@observable public publicAlbumCollection: boolean;
	@observable public publicRatings: boolean;
	@observable public showActivity: boolean;
	@observable public showChatbox: boolean;
	@observable public stylesheet: string;
	@observable public submitting = false;
	@observable public unreadNotificationsToKeep: string;
	@observable public username: string;
	public readonly webLinksStore: WebLinksEditStore;

	public constructor(
		private readonly userRepo: UserRepository,
		public readonly contract: UserForMySettingsContract,
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
		this.unreadNotificationsToKeep = contract.unreadNotificationsToKeep.toString();
		this.username = contract.name;
		this.webLinksStore = new WebLinksEditStore(contract.webLinks);
	}

	// TODO: support showing the verification button by saving email immediately after it's changed
	@computed public get canVerifyEmail(): boolean {
		return !!this.email && !this.emailVerified && !this.emailVerificationSent;
	}

	@action public addKnownLanguage = (): void => {
		this.knownLanguages.push(new UserKnownLanguageEditStore());
	};

	@action public removeKnownLanguage = (
		knownLanguage: UserKnownLanguageEditStore,
	): void => {
		_.pull(this.knownLanguages, knownLanguage);
	};

	@action public verifyEmail = async (): Promise<void> => {
		this.emailVerificationSent = true;
		await this.userRepo.requestEmailVerification({});
	};

	@action public submit = async (
		requestToken: string,
		pictureUpload: File | undefined,
	): Promise<string> => {
		this.submitting = true;

		try {
			const name = await this.userRepo.updateMySettings(
				requestToken,
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
