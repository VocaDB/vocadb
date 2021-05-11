import PartialFindResultContract from '@DataContracts/PartialFindResultContract';

// Generic viewmodel that supports simple paging by loading more items
export default class PagedItemsViewModel<TModel> {
  public clear = (): void => {
    this.items([]);
    this.start = 0;
    this.loadMore();
  };

  public hasMore = ko.observable(false);

  private isInit = false;

  public init = (callback?: Function): void => {
    if (this.isInit) return;

    this.loadMore(callback);
    this.isInit = true;
  };

  public items = ko.observableArray<TModel>([]);

  public loadMore = (callback?: Function): void => {
    this.loadMoreItems((result) => {
      this.itemsLoaded(result);
      if (callback) callback();
    });
  };

  public loadMoreItems = (
    callback: (result: PartialFindResultContract<TModel>) => void,
  ): void => {};

  public itemsLoaded = (result: PartialFindResultContract<TModel>): void => {
    ko.utils.arrayPushAll(this.items, result.items);
    this.start = this.start + result.items.length;
    this.hasMore(result.totalCount > this.start);
  };

  public start = 0;
}
