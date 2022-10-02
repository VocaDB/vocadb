import { SongContract } from '@/DataContracts/Song/SongContract';
import { EntryMergeValidationHelper } from '@/Helpers/EntryMergeValidationHelper';
import { SongRepository } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import {
	action,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class SongMergeStore {
	@observable public submitting = false;
	public readonly target: BasicEntryLinkStore<SongContract>;
	@observable public validationError_targetIsLessComplete = false;
	@observable public validationError_targetIsNewer = false;

	public constructor(
		values: GlobalValues,
		private readonly songRepo: SongRepository,
		private readonly song: SongContract,
	) {
		makeObservable(this);

		this.target = new BasicEntryLinkStore<SongContract>((entryId) =>
			songRepo.getOne({ id: entryId, lang: values.languagePreference }),
		);

		reaction(
			() => this.target.entry,
			(entry) => {
				const result = EntryMergeValidationHelper.validateEntry(song, entry);
				this.validationError_targetIsLessComplete =
					result.validationError_targetIsLessComplete;
				this.validationError_targetIsNewer =
					result.validationError_targetIsNewer;
			},
		);
	}

	@action public submit = async (
		requestToken: string,
		targetSongId: number,
	): Promise<void> => {
		try {
			this.submitting = true;

			await this.songRepo.merge(requestToken, {
				id: this.song.id,
				targetSongId: targetSongId,
			});
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
