import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryMergeValidationHelper } from '@/Helpers/EntryMergeValidationHelper';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import {
	action,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class ArtistMergeStore {
	@observable submitting = false;
	readonly target: BasicEntryLinkStore<ArtistContract>;
	@observable validationError_targetIsLessComplete = false;
	@observable validationError_targetIsNewer = false;

	constructor(
		values: GlobalValues,
		private readonly artistRepo: ArtistRepository,
		private readonly artist: ArtistContract,
	) {
		makeObservable(this);

		this.target = new BasicEntryLinkStore<ArtistContract>((entryId) =>
			artistRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		reaction(
			() => this.target.entry,
			(entry) => {
				const result = EntryMergeValidationHelper.validateEntry(artist, entry);
				this.validationError_targetIsLessComplete =
					result.validationError_targetIsLessComplete;
				this.validationError_targetIsNewer =
					result.validationError_targetIsNewer;
			},
		);
	}

	@action submit = async (
		requestToken: string,
		targetArtistId: number,
	): Promise<void> => {
		try {
			this.submitting = true;

			await this.artistRepo.merge(requestToken, {
				id: this.artist.id,
				targetArtistId: targetArtistId,
			});
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
