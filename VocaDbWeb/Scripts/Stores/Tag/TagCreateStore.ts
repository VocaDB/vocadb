import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { NameMatchMode } from '@/Models/NameMatchMode';
import { TagRepository } from '@/Repositories/TagRepository';
import {
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class TagCreateStore {
	@observable dialogVisible = false;
	@observable duplicateName = false;
	@observable newTagName = '';

	constructor(private readonly tagRepo: TagRepository) {
		makeObservable(this);

		reaction(
			() => this.newTagName,
			(val) => {
				if (!val) {
					this.duplicateName = false;
					return;
				}

				tagRepo
					.getList({
						queryParams: {
							start: 0,
							maxResults: 1,
							getTotalCount: false,
							query: val,
							nameMatchMode: NameMatchMode.Exact,
							allowAliases: true,
						},
					})
					.then((result) => {
						runInAction(() => {
							this.duplicateName = result.items.length > 0;
						});
					});
			},
		);
	}

	@computed get isValid(): boolean {
		return !!this.newTagName && !this.duplicateName;
	}

	createTag = async (): Promise<TagBaseContract> => {
		const tag = await this.tagRepo.create({ name: this.newTagName });

		runInAction(() => {
			this.newTagName = '';
			this.dialogVisible = false;
		});

		return tag;
	};
}
