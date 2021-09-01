import AlbumContract from '@DataContracts/Album/AlbumContract';
import AlbumRepository from '@Repositories/AlbumRepository';
import GlobalValues from '@Shared/GlobalValues';
import { AlbumSortRule } from '@Stores/Search/AlbumSearchStore';
import ServerSidePagingStore from '@Stores/ServerSidePagingStore';
import { makeObservable, observable, reaction, runInAction } from 'mobx';

export default class DeletedAlbumsStore {
	@observable public loading = false;
	@observable public page: AlbumContract[] = []; // Current page of items
	public readonly paging = new ServerSidePagingStore(20); // Paging view model
	@observable public searchTerm = '';
	@observable public showTags = false;
	@observable public sort = AlbumSortRule.Name;
	@observable public viewMode = 'Details' /* TODO: enum */;

	public constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
	) {
		makeObservable(this);

		this.updateResults(true);
		reaction(
			() => this.paging.page,
			() => this.updateResults(false),
		);
		reaction(
			() => this.paging.pageSize,
			() => this.updateResults(true),
		);
		reaction(
			() => this.searchTerm,
			() => this.updateResults(true),
		);
	}

	private updateResults = (clearResults: boolean): void => {
		if (clearResults) {
			this.paging.goToFirstPage();
		}

		const pagingProperties = this.paging.getPagingProperties(clearResults);
		this.albumRepo
			.getList({
				paging: pagingProperties,
				lang: this.values.languagePreference,
				query: this.searchTerm,
				sort: AlbumSortRule.Name,
				discTypes: undefined,
				tags: undefined,
				childTags: undefined,
				artistIds: undefined,
				artistParticipationStatus: undefined,
				childVoicebanks: undefined,
				includeMembers: undefined,
				fields: 'AdditionalNames,MainPicture',
				status: undefined,
				deleted: true,
				advancedFilters: undefined,
			})
			.then((result) => {
				runInAction(() => {
					this.page = result.items;

					if (pagingProperties.getTotalCount)
						this.paging.totalItems = result.totalCount;
				});
			});
	};
}
