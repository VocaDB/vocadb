import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { EntryMergeValidationHelper } from '@/Helpers/EntryMergeValidationHelper';
import { TagRepository } from '@/Repositories/TagRepository';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import {
	action,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class TagMergeStore {
	@observable submitting = false;
	readonly target: BasicEntryLinkStore<TagBaseContract>;
	@observable validationError_targetIsLessComplete = false;
	@observable validationError_targetIsNewer = false;

	constructor(
		private readonly tagRepo: TagRepository,
		private readonly tag: TagBaseContract,
	) {
		makeObservable(this);

		this.target = new BasicEntryLinkStore<TagBaseContract>((entryId) =>
			tagRepo.getById({ id: entryId, fields: undefined, lang: undefined }),
		);

		reaction(
			() => this.target.entry,
			(entry) => {
				const result = EntryMergeValidationHelper.validateEntry(
					this.tag,
					entry,
				);
				this.validationError_targetIsLessComplete =
					result.validationError_targetIsLessComplete;
				this.validationError_targetIsNewer =
					result.validationError_targetIsNewer;
			},
		);
	}

	tagFilter = (tag: TagApiContract): boolean => {
		return tag.id !== this.tag.id;
	};

	@action submit = async (
		requestToken: string,
		targetTagId: number,
	): Promise<void> => {
		try {
			this.submitting = true;

			await this.tagRepo.merge(requestToken, {
				id: this.tag.id,
				targetTagId: targetTagId,
			});
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
