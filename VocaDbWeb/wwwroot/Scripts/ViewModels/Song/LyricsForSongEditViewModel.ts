import BasicListEditViewModel from '../BasicListEditViewModel';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import LyricsForSongContract from '@DataContracts/Song/LyricsForSongContract';
import TranslationType from '@Models/Globalization/TranslationType';
import WebLinkMatcher from '@Shared/WebLinkMatcher';

export default class LyricsForSongEditViewModel {
  constructor(contract?: LyricsForSongContract) {
    if (contract) {
      this.id = ko.observable(contract.id);
      this.cultureCode = ko.observable(contract.cultureCode);
      this.language = ko.observable(contract.language);
      this.source = ko.observable(contract.source);
      this.translationType = ko.observable(contract.translationType);
      this.url = ko.observable(contract.url);
      this.value = ko.observable(contract.value);
    } else {
      this.id = ko.observable(0);
      this.cultureCode = ko.observable('');
      this.language = ko.observable(
        ContentLanguageSelection[ContentLanguageSelection.Unspecified],
      );
      this.source = ko.observable('');
      this.translationType = ko.observable(
        TranslationType[TranslationType.Translation],
      );
      this.url = ko.observable('');
      this.value = ko.observable('');
    }

    this.url.subscribe((url) => {
      if (!this.source()) {
        var matcher = WebLinkMatcher.matchWebLink(url);

        if (matcher) {
          this.source(matcher.desc);
        }
      }
    });

    this.isNew = contract == null;
  }

  public toggleAccordion = (vm: any, event: JQueryEventObject): void => {
    var elem = $(event.target)
      .closest('.accordion-group')
      .find('.accordion-body') as any;
    elem.collapse('toggle');
  };

  public cultureCode: KnockoutObservable<string>;

  public id: KnockoutObservable<number>;

  public isNew: boolean;

  public language: KnockoutObservable<string>;

  public showLanguageSelection = (): boolean =>
    this.translationType() !== TranslationType[TranslationType.Romanized];

  public source: KnockoutObservable<string>;

  public translationType: KnockoutObservable<string>;

  public url: KnockoutObservable<string>;

  public value: KnockoutObservable<string>;
}

export class LyricsForSongListEditViewModel extends BasicListEditViewModel<
  LyricsForSongEditViewModel,
  LyricsForSongContract
> {
  private find = (translationType: string): LyricsForSongEditViewModel => {
    var vm = _.find(
      this.items(),
      (i) => i.translationType() === translationType,
    );
    if (vm) _.remove(this.items(), vm);
    else {
      vm = new LyricsForSongEditViewModel({ translationType: translationType });
    }
    return vm;
  };

  constructor(contracts: LyricsForSongContract[]) {
    super(LyricsForSongEditViewModel, contracts);
    this.original = this.find('Original');
    this.romanized = this.find('Romanized');
  }

  public changeToOriginal = (lyrics: LyricsForSongEditViewModel): void => {
    this.original.id(lyrics.id());
    this.original.value(lyrics.value());
    this.original.cultureCode(lyrics.cultureCode());
    this.original.source(lyrics.source());
    this.original.url(lyrics.url());
    this.items.remove(lyrics);
  };

  public changeToTranslation = (lyrics: LyricsForSongEditViewModel): void => {
    if (lyrics === this.original) {
      var newLyrics = new LyricsForSongEditViewModel({
        id: this.original.id(),
        cultureCode: this.original.cultureCode(),
        source: this.original.source(),
        url: this.original.url(),
        value: this.original.value(),
        translationType: TranslationType[TranslationType.Translation],
      });

      this.items.push(newLyrics);

      this.original.id(0);
      this.original.value('');
      this.original.cultureCode('');
      this.original.source('');
      this.original.url('');
    } else {
      lyrics.translationType(TranslationType[TranslationType.Translation]);
    }
  };

  public original: LyricsForSongEditViewModel;
  public romanized: LyricsForSongEditViewModel;

  public toContracts: () => LyricsForSongContract[] = () => {
    var result = ko.toJS(
      _.chain([this.original, this.romanized])
        .concat(this.items())
        .filter((i) => i.value())
        .value(),
    );
    return result;
  };
}
