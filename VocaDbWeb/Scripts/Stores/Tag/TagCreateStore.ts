import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import NameMatchMode from '@Models/NameMatchMode';
import TagRepository from '@Repositories/TagRepository';
import {
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export default class TagCreateStore {
	@observable public dialogVisible = false;
	@observable public duplicateName = false;
	@observable public newTagName = '';

	public constructor(private readonly tagRepo: TagRepository) {
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

	@computed public get isValid(): boolean {
		return !!this.newTagName && !this.duplicateName;
	}

	public createTag = (): Promise<TagBaseContract> => {
		return this.tagRepo.create({ name: this.newTagName });
	};
}
