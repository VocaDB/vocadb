import type { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { action, makeObservable, observable, runInAction } from 'mobx';

export default class RequestVerificationStore {
	@observable linkToProof = '';
	@observable message = '';
	@observable privateMessage = false;
	@observable selectedArtist?: ArtistContract;
	@observable submitting = false;

	constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	clearArtist = (): void => {
		this.selectedArtist = undefined;
	};

	setArtist = async (targetArtistId?: number): Promise<void> => {
		const artist = await this.artistRepo.getOne({
			id: targetArtistId!,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.selectedArtist = artist;
		});
	};

	@action submit = async (): Promise<void> => {
		if (!this.selectedArtist) return;

		try {
			this.submitting = true;

			await this.artistRepo.requestVerification({
				artistId: this.selectedArtist.id,
				message: this.message,
				linkToProof: this.linkToProof,
				privateMessage: this.privateMessage,
			});
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
