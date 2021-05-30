import AlbumContract from '@DataContracts/Album/AlbumContract';
import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import AlbumRepository from '@Repositories/AlbumRepository';
import vdb from '@Shared/VdbStatic';
import ko from 'knockout';

import ServerSidePagingViewModel from '../ServerSidePagingViewModel';

export default class DeletedAlbumsViewModel {
  constructor(private albumRepo: AlbumRepository) {
    this.updateResults(true);
    this.paging.page.subscribe(() => this.updateResults(false));
    this.paging.pageSize.subscribe(() => this.updateResults(true));
    this.searchTerm.subscribe(() => this.updateResults(true));
  }

  public discTypeName = (discType: string): string => discType;
  public loading = ko.observable(false);
  public page = ko.observableArray<AlbumContract>([]); // Current page of items
  public paging = new ServerSidePagingViewModel(20); // Paging view model
  public ratingStars = (): any[] => [];
  public searchTerm = ko
    .observable('')
    .extend({ rateLimit: { timeout: 300, method: 'notifyWhenChangesStop' } });
  public showTags = ko.observable(false);
  public sort = ko.observable('Name');
  public viewMode = ko.observable('Details');

  private updateResults = (clearResults: boolean): void => {
    if (clearResults) {
      this.paging.page(1);
    }

    var pagingProperties = this.paging.getPagingProperties(clearResults);
    this.albumRepo
      .getList(
        pagingProperties,
        ContentLanguagePreference[vdb.values.languagePreference],
        this.searchTerm(),
        'Name',
        undefined!,
        null!,
        null!,
        null!,
        undefined!,
        undefined!,
        undefined!,
        'AdditionalNames,MainPicture',
        null!,
        true,
        null!,
      )
      .then((result) => {
        this.page(result.items);

        if (pagingProperties.getTotalCount)
          this.paging.totalItems(result.totalCount);
      });
  };
}
