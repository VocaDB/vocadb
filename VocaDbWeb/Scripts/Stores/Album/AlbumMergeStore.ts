import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { EntryMergeValidationHelper } from '@/Helpers/EntryMergeValidationHelper';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import {
	action,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class AlbumMergeStore {
	@observable public submitting = false;
	public target: BasicEntryLinkStore<AlbumContract>;
	@observable public validationError_targetIsLessComplete = false;
	@observable public validationError_targetIsNewer = false;

	public constructor(
		values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		private readonly album: AlbumContract,
	) {
		makeObservable(this);

		this.target = new BasicEntryLinkStore<AlbumContract>((entryId) =>
			albumRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		reaction(
			() => this.target.entry,
			(entry) => {
				const result = EntryMergeValidationHelper.validateEntry(album, entry);
				this.validationError_targetIsLessComplete =
					result.validationError_targetIsLessComplete;
				this.validationError_targetIsNewer =
					result.validationError_targetIsNewer;
			},
		);
	}

	@action public submit = async (
		requestToken: string,
		targetAlbumId: number,
	): Promise<void> => {
		try {
			this.submitting = true;

			await this.albumRepo.merge(requestToken, {
				id: this.album.id,
				targetAlbumId: targetAlbumId,
			});
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
