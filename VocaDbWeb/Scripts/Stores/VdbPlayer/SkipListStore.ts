import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { ArtistFilters } from '@/Stores/Search/ArtistFilters';
import { TagFilters } from '@/Stores/Search/TagFilters';
import { action, makeObservable, observable } from 'mobx';

export class SkipListStore {
	@observable public dialogVisible = false;
	public readonly artistFilters: ArtistFilters;
	public readonly tagFilters: TagFilters;

	public constructor(
		values: GlobalValues,
		artistRepo: ArtistRepository,
		tagRepo: TagRepository,
	) {
		makeObservable(this);

		this.artistFilters = new ArtistFilters(values, artistRepo);
		this.tagFilters = new TagFilters(values, tagRepo);
	}

	@action public showDialog = (): void => {
		this.dialogVisible = true;
	};

	@action public hideDialog = (): void => {
		this.dialogVisible = false;
	};
}
