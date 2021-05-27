import ko, { Observable } from 'knockout';

export default class EnglishTranslatedStringViewModel {
  constructor(showTranslatedDescription: boolean) {
    this.showTranslatedDescription = ko.observable(showTranslatedDescription);
  }

  public isFullDescriptionShown: Observable<boolean> = ko.observable(false);

  public showFullDescription = (): void => {
    this.isFullDescriptionShown(true);
  };

  public showTranslatedDescription: Observable<boolean>;
}
