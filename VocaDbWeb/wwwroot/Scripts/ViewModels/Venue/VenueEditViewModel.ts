import DeleteEntryViewModel from '../DeleteEntryViewModel';
import EntryType from '../../Models/EntryType';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import NameMatchMode from '../../Models/NameMatchMode';
import NamesEditViewModel from '../Globalization/NamesEditViewModel';
import OptionalGeoPointContract from '../../DataContracts/OptionalGeoPointContract';
import UrlMapper from '../../Shared/UrlMapper';
import UserRepository from '../../Repositories/UserRepository';
import VenueForEditContract from '../../DataContracts/Venue/VenueForEditContract';
import VenueRepository from '../../Repositories/VenueRepository';
import WebLinksEditViewModel from '../WebLinksEditViewModel';

export default class VenueEditViewModel {
  constructor(
    private readonly repo: VenueRepository,
    userRepository: UserRepository,
    private readonly urlMapper: UrlMapper,
    contract: VenueForEditContract,
  ) {
    this.address = ko.observable(contract.address);
    this.addressCountryCode = ko.observable(contract.addressCountryCode);
    this.defaultNameLanguage = ko.observable(contract.defaultNameLanguage);
    this.id = contract.id;
    this.latitude = ko.observable(contract.coordinates?.latitude ?? null);
    this.longitude = ko.observable(contract.coordinates?.longitude ?? null);
    this.names = NamesEditViewModel.fromContracts(contract.names);
    this.webLinks = new WebLinksEditViewModel(contract.webLinks);

    this.coordinates = ko.computed(() => {
      if (!this.latitude() || !this.longitude()) return null;

      return {
        latitude: !isNaN(this.latitude()) ? this.latitude() : null,
        longitude: !isNaN(this.longitude()) ? this.longitude() : null,
      };
    });

    if (contract.id) {
      window.setInterval(
        () => userRepository.refreshEntryEdit(EntryType.Venue, contract.id),
        10000,
      );
    } else {
      _.forEach(
        [
          this.names.originalName,
          this.names.romajiName,
          this.names.englishName,
        ],
        (name) => {
          ko.computed(() => name.value())
            .extend({ rateLimit: 500 })
            .subscribe(this.checkName);
        },
      );
    }
  }

  public address: KnockoutObservable<string>;
  public addressCountryCode: KnockoutObservable<string>;

  private checkName = (value: string) => {
    if (!value) {
      this.duplicateName(null);
      return;
    }

    this.repo.getList(value, NameMatchMode.Exact, 1).then((result) => {
      this.duplicateName(result.items.length ? value : null);
    });
  };

  public coordinates: KnockoutComputed<OptionalGeoPointContract>;

  public defaultNameLanguage: KnockoutObservable<string>;

  public deleteViewModel = new DeleteEntryViewModel((notes) => {
    this.repo.delete(this.id, notes, false).then(this.redirectToDetails);
  });

  public description = ko.observable<string>();
  public duplicateName = ko.observable<string>();
  private id: number;
  public latitude: KnockoutObservable<number>;
  public longitude: KnockoutObservable<number>;
  public names: NamesEditViewModel;

  private redirectToDetails = () => {
    window.location.href = this.urlMapper.mapRelative(
      EntryUrlMapper.details(EntryType.Venue, this.id),
    );
  };

  private redirectToRoot = () => {
    window.location.href = this.urlMapper.mapRelative('Event');
  };

  public submitting = ko.observable(false);
  public webLinks: WebLinksEditViewModel;

  public submit = () => {
    this.submitting(true);
    return true;
  };

  public trashViewModel = new DeleteEntryViewModel((notes) => {
    this.repo.delete(this.id, notes, true).then(this.redirectToRoot);
  });
}
