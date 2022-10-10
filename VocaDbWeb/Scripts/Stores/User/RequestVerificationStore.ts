import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { action, makeObservable, observable, runInAction } from 'mobx';

export default class RequestVerificationStore {
	@observable public linkToProof = '';
	@observable public message = '';
	@observable public privateMessage = false;
	@observable public selectedArtist?: ArtistContract;
	@observable public submitting = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	public clearArtist = (): void => {
		this.selectedArtist = undefined;
	};

	public setArtist = async (targetArtistId?: number): Promise<void> => {
		const artist = await this.artistRepo.getOne({
			id: targetArtistId!,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.selectedArtist = artist;
		});
	};

	@action public submit = async (requestToken: string): Promise<void> => {
		if (!this.selectedArtist) return;

		try {
			this.submitting = true;

			await this.artistRepo.requestVerification(requestToken, {
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
