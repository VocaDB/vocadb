import DeleteEntryViewModel from './DeleteEntryViewModel';
import EnglishTranslatedStringEditViewModel from './Globalization/EnglishTranslatedStringEditViewModel';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import NamesEditViewModel from './Globalization/NamesEditViewModel';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import UrlMapper from '@Shared/UrlMapper';
import UserRepository from '@Repositories/UserRepository';
import WebLinksEditViewModel from './WebLinksEditViewModel';

export default class TagEditViewModel {
  // Bitmask for all possible entry types (all bits 1)
  public static readonly allEntryTypes = 1073741823;

  constructor(
    private readonly urlMapper: UrlMapper,
    userRepository: UserRepository,
    contract: TagApiContract,
  ) {
    this.categoryName = ko.observable(contract.categoryName);
    this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
    this.description = new EnglishTranslatedStringEditViewModel(
      contract.translatedDescription!,
    );
    this.id = contract.id;
    this.names = NamesEditViewModel.fromContracts(contract.names);
    this.parent = ko.observable(contract.parent);
    this.relatedTags = ko.observableArray(contract.relatedTags);
    this.targets = ko.observable(contract.targets);
    this.webLinks = new WebLinksEditViewModel(contract.webLinks);

    this.validationError_needDescription = ko.computed(
      () => !this.description.original() && _.isEmpty(this.webLinks.items()),
    );

    this.parentName = ko.computed(() =>
      this.parent() ? this.parent().name : null!,
    );

    this.hasValidationErrors = ko.computed(() =>
      this.validationError_needDescription(),
    );

    window.setInterval(
      () => userRepository.refreshEntryEdit(EntryType.Tag, contract.id),
      10000,
    );
  }

  public categoryName: KnockoutObservable<string>;
  public defaultNameLanguage: KnockoutObservable<string>;
  public description: EnglishTranslatedStringEditViewModel;
  public hasValidationErrors: KnockoutComputed<boolean>;
  private id: number;
  public names: NamesEditViewModel;
  public parent: KnockoutObservable<TagBaseContract>;
  public parentName: KnockoutComputed<string>;
  public relatedTags: KnockoutObservableArray<TagBaseContract>;
  public submitting = ko.observable(false);
  public targets: KnockoutObservable<EntryType>;
  public validationExpanded = ko.observable(false);
  public validationError_needDescription: KnockoutComputed<boolean>;
  public webLinks: WebLinksEditViewModel;

  public addRelatedTag = (tag: TagBaseContract): void =>
    this.relatedTags.push(tag);

  public allowRelatedTag = (tag: TagBaseContract): boolean =>
    this.denySelf(tag) && _.every(this.relatedTags(), (t) => t.id !== tag.id);

  public deleteViewModel = new DeleteEntryViewModel((notes) => {
    $.ajax(
      this.urlMapper.mapRelative(
        'api/tags/' +
          this.id +
          '?hardDelete=false&notes=' +
          encodeURIComponent(notes),
      ),
      {
        type: 'DELETE',
        success: () => {
          window.location.href = EntryUrlMapper.details_tag(this.id);
        },
      },
    );
  });

  public denySelf = (tag: TagBaseContract): boolean =>
    tag && tag.id !== this.id;

  public submit = (): boolean => {
    this.submitting(true);
    return true;
  };

  public hasTargetType = (target: EntryType): KnockoutComputed<boolean> => {
    const hasFlag = (t: EntryType): boolean => (this.targets() & t) === t;
    const checkFlags = (): void => {
      const types = [
        EntryType.Album,
        EntryType.Artist,
        EntryType.ReleaseEvent,
        EntryType.Song,
      ];
      if (this.targets() === _.sum(types)) {
        this.targets(TagEditViewModel.allEntryTypes);
      } else {
        this.targets(
          _.chain(types)
            .filter((t) => hasFlag(t))
            .sum()
            .value(),
        );
      }
    };
    const addFlag = (): void => {
      this.targets(this.targets() | target);
      checkFlags();
    };
    const removeFlag = (): void => {
      if (hasFlag(target)) {
        this.targets(this.targets() - target);
        checkFlags();
      }
    };
    return ko.computed<boolean>({
      read: () => hasFlag(target),
      write: (flag) => (flag ? addFlag() : removeFlag()),
    });
  };

  public trashViewModel = new DeleteEntryViewModel((notes) => {
    $.ajax(
      this.urlMapper.mapRelative(
        'api/tags/' +
          this.id +
          '?hardDelete=true&notes=' +
          encodeURIComponent(notes),
      ),
      {
        type: 'DELETE',
        success: () => {
          window.location.href = this.urlMapper.mapRelative('/Tag');
        },
      },
    );
  });
}
