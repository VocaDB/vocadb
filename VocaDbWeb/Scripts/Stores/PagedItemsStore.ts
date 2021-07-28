import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import { action, makeObservable, observable } from 'mobx';

// Generic store that supports simple paging by loading more items
export default abstract class PagedItemsStore<TModel> {
	@observable public hasMore = false;
	private isInit = false;
	@observable public items: TModel[] = [];
	public start = 0;

	protected constructor() {
		makeObservable(this);
	}

	public abstract loadMoreItems: () => Promise<
		PartialFindResultContract<TModel>
	>;

	@action public itemsLoaded = (
		result: PartialFindResultContract<TModel>,
	): void => {
		this.items.push(...result.items);
		this.start += result.items.length;
		this.hasMore = result.totalCount > this.start;
	};

	public loadMore = (): Promise<PartialFindResultContract<TModel>> => {
		return this.loadMoreItems().then((result) => {
			this.itemsLoaded(result);
			return result;
		});
	};

	@action public clear = (): void => {
		this.items = [];
		this.start = 0;
		this.loadMore();
	};

	public init = (
		callback?: (result: PartialFindResultContract<TModel>) => void,
	): void => {
		if (this.isInit) return;

		this.loadMore().then(callback);
		this.isInit = true;
	};
}
