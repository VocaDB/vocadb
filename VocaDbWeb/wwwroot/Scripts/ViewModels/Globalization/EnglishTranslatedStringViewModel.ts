export default class EnglishTranslatedStringViewModel {
  constructor(showTranslatedDescription: boolean) {
    this.showTranslatedDescription = ko.observable(showTranslatedDescription);
  }

  public isFullDescriptionShown: KnockoutObservable<boolean> = ko.observable(
    false,
  );

  public showFullDescription = () => {
    this.isFullDescriptionShown(true);
  };

  public showTranslatedDescription: KnockoutObservable<boolean>;
}
