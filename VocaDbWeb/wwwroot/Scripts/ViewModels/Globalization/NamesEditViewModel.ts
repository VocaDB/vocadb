import LocalizedStringWithIdContract from '@DataContracts/Globalization/LocalizedStringWithIdContract';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';

import LocalizedStringWithIdEditViewModel from './LocalizedStringWithIdEditViewModel';

export default class NamesEditViewModel {
  public aliases: KnockoutObservableArray<LocalizedStringWithIdEditViewModel>;
  public englishName: LocalizedStringWithIdEditViewModel;
  public originalName: LocalizedStringWithIdEditViewModel;
  public romajiName: LocalizedStringWithIdEditViewModel;

  public createAlias = (): void => {
    this.aliases.push(new LocalizedStringWithIdEditViewModel());
  };

  public deleteAlias = (alias: LocalizedStringWithIdEditViewModel): void => {
    this.aliases.remove(alias);
  };

  public getAllNames = (): LocalizedStringWithIdEditViewModel[] => {
    return _.filter(
      this.getAllPrimaryNames().concat(this.aliases()),
      (name) => name && name.value && name.value(),
    );
  };

  private getAllPrimaryNames: () => LocalizedStringWithIdEditViewModel[] = () => {
    return [this.originalName, this.romajiName, this.englishName];
  };

  public getPrimaryNames = (): LocalizedStringWithIdEditViewModel[] =>
    _.filter(this.getAllPrimaryNames(), (n) => n && n.value && n.value());

  // Whether the primary name is specified (in any language). This excludes aliases.
  public hasPrimaryName = (): boolean => {
    return _.some(
      this.getPrimaryNames(),
      (name) => name && name.value && name.value(),
    );
  };

  public toContracts = (): LocalizedStringWithIdContract[] => {
    return _.map(this.getAllNames(), (name) => {
      var contract: LocalizedStringWithIdContract = {
        id: name.id,
        language: name.languageStr(),
        value: name.value(),
      };

      return contract;
    });
  };

  public static fromContracts(
    contracts: LocalizedStringWithIdContract[],
  ): NamesEditViewModel {
    return new NamesEditViewModel(
      _.map(contracts, (contract) =>
        LocalizedStringWithIdEditViewModel.fromContract(contract),
      ),
    );
  }

  private static nameOrEmpty(
    names: LocalizedStringWithIdEditViewModel[],
    lang: ContentLanguageSelection,
  ): LocalizedStringWithIdEditViewModel {
    const name = _.find(names, (n) => n.language() === lang);
    return name || new LocalizedStringWithIdEditViewModel(lang, '');
  }

  constructor(names: LocalizedStringWithIdEditViewModel[] = []) {
    this.englishName = NamesEditViewModel.nameOrEmpty(
      names,
      ContentLanguageSelection.English,
    );
    this.originalName = NamesEditViewModel.nameOrEmpty(
      names,
      ContentLanguageSelection.Japanese,
    );
    this.romajiName = NamesEditViewModel.nameOrEmpty(
      names,
      ContentLanguageSelection.Romaji,
    );

    this.aliases = ko.observableArray(
      _.filter(
        names,
        (n) =>
          n.id !== this.englishName.id &&
          n.id !== this.originalName.id &&
          n.id !== this.romajiName.id,
      ),
    );
  }
}
