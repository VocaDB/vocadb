import ContentLanguageSelection from '../../Models/Globalization/ContentLanguageSelection';
import KnockoutHelper from '../../Helpers/KnockoutHelper';
import LocalizedStringWithIdContract from '../../DataContracts/Globalization/LocalizedStringWithIdContract';

export default class LocalizedStringWithIdEditViewModel {
  public id: number;
  public language = ko.observable(ContentLanguageSelection.Unspecified); //: KnockoutObservable<cls.globalization.ContentLanguageSelection>;
  public languageStr = KnockoutHelper.stringEnum(
    this.language,
    ContentLanguageSelection,
  );
  public value: KnockoutObservable<string>;

  public static fromContract(
    contract: LocalizedStringWithIdContract,
  ): LocalizedStringWithIdEditViewModel {
    return new LocalizedStringWithIdEditViewModel(
      ContentLanguageSelection[
        contract.language as keyof typeof ContentLanguageSelection
      ],
      contract.value,
      contract.id,
    );
  }

  constructor(
    language: ContentLanguageSelection = ContentLanguageSelection.Unspecified,
    value: string = null,
    id: number = 0,
  ) {
    this.language(language);
    this.value = ko.observable(value);
    this.id = id;
  }
}
