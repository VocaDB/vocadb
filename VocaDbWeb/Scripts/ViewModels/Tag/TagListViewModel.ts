import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import ko, { Computed, ObservableArray } from 'knockout';
import _ from 'lodash';

export default class TagListViewModel {
  private static maxDisplayedTags = 4;

  constructor(tagUsages: TagUsageForApiContract[]) {
    this.tagUsages = ko.observableArray<TagUsageForApiContract>([]);
    this.updateTagUsages(tagUsages);

    if (tagUsages.length <= TagListViewModel.maxDisplayedTags + 1)
      this.expanded(true);

    this.displayedTagUsages = ko.computed(() =>
      this.expanded()
        ? this.tagUsages()
        : _.take(this.tagUsages(), TagListViewModel.maxDisplayedTags),
    );

    this.tagUsagesByCategories = ko.computed(() => {
      var tags = _.chain(this.tagUsages())
        .orderBy((tagUsage) => tagUsage.tag.categoryName)
        .groupBy((tagUsage) => tagUsage.tag.categoryName)
        .map((tagUsages: TagUsageForApiContract[], categoryName: string) => ({
          categoryName,
          tagUsages,
        }));

      var genres = tags.filter((c) => c.categoryName === 'Genres').value();
      var empty = tags.filter((c) => c.categoryName === '').value();

      return _.chain(genres)
        .concat(
          tags
            .filter((c) => c.categoryName !== 'Genres' && c.categoryName !== '')
            .value(),
        )
        .concat(empty)
        .value();
    });
  }

  public displayedTagUsages: Computed<TagUsageForApiContract[]>;

  public getTagUrl = (tag: TagUsageForApiContract): string => {
    return EntryUrlMapper.details_tag(tag.tag.id, tag.tag.urlSlug);
  };

  public expanded = ko.observable(false);

  public tagUsages: ObservableArray<TagUsageForApiContract>;

  public updateTagUsages = (usages: TagUsageForApiContract[]): void => {
    this.tagUsages(
      _.chain(usages)
        .sortBy((u) => u.tag.name.toLowerCase())
        .sortBy((u) => -u.count)
        .value(),
    );
  };

  public tagUsagesByCategories: Computed<
    { categoryName: string; tagUsages: TagUsageForApiContract[] }[]
  >;
}
