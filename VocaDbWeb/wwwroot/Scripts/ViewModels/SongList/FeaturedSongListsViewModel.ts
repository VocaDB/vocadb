import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import SongListContract from '@DataContracts/Song/SongListContract';
import ResourceRepository from '@Repositories/ResourceRepository';
import SongListRepository from '@Repositories/SongListRepository';
import TagRepository from '@Repositories/TagRepository';
import _ from 'lodash';

import SongListsBaseViewModel from './SongListsBaseViewModel';

export default class FeaturedSongListsViewModel {
  constructor(
    listRepo: SongListRepository,
    resourceRepo: ResourceRepository,
    tagRepo: TagRepository,
    languageSelection: string,
    cultureCode: string,
    tagIds: number[],
    categoryNames: string[],
  ) {
    _.forEach(categoryNames, (categoryName) => {
      this.categories[categoryName] = new FeaturedSongListCategoryViewModel(
        listRepo,
        resourceRepo,
        tagRepo,
        languageSelection,
        cultureCode,
        tagIds,
        categoryName,
      );
    });

    window.onhashchange = (): void => {
      if (window.location.hash && window.location.hash.length >= 1)
        this.setCategory(window.location.hash.substr(1));
    };
  }

  public categories: {
    [index: string]: FeaturedSongListCategoryViewModel;
  } = {};

  public category = ko.observable('Concerts');

  public setCategory = (categoryName: string): void => {
    if (!categoryName) categoryName = 'Concerts';

    window.scrollTo(0, 0);
    window.location.hash = categoryName;
    this.category(categoryName);
  };
}

export class FeaturedSongListCategoryViewModel extends SongListsBaseViewModel {
  constructor(
    private listRepo: SongListRepository,
    resourceRepo: ResourceRepository,
    tagRepo: TagRepository,
    languageSelection: string,
    cultureCode: string,
    tagIds: number[],
    private category: string,
  ) {
    // Should figure out a better way for this.
    super(
      resourceRepo,
      tagRepo,
      languageSelection,
      cultureCode,
      tagIds,
      category === 'Concerts' || category === 'VocaloidRanking',
    );
  }

  public loadMoreItems = (
    callback: (result: PartialFindResultContract<SongListContract>) => void,
  ): void => {
    this.listRepo
      .getFeatured(
        this.query(),
        this.category,
        { start: this.start, maxEntries: 50, getTotalCount: true },
        this.tagFilters.tagIds(),
        this.fields(),
        this.sort(),
      )
      .then(callback);
  };
}
