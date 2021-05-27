import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import ko from 'knockout';

export default class TagFilter {
  public static fromContract = (tag: TagBaseContract): TagFilter => {
    return new TagFilter(tag.id, tag.name, tag.urlSlug);
  };

  constructor(public id: number, name?: string, urlSlug?: string) {
    this.name(name!);
    this.urlSlug(urlSlug!);
  }

  public name = ko.observable<string>(null!);

  public urlSlug = ko.observable<string>(null!);
}
