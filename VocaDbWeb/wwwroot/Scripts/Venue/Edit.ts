import VenueForEditContract from '../DataContracts/Venue/VenueForEditContract';
import RepositoryFactory from '../Repositories/RepositoryFactory';
import HttpClient from '../Shared/HttpClient';
import UrlMapper from '../Shared/UrlMapper';
import VenueEditViewModel from '../ViewModels/Venue/VenueEditViewModel';

function initPage(): void {
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const VenueEdit = (model: VenueForEditContract): void => {
  $(function () {
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
    );
    var venueRepo = repoFactory.venueRepository();
    var userRepo = repoFactory.userRepository();
    var contract = model;

    var vm = new VenueEditViewModel(venueRepo, userRepo, urlMapper, contract);
    ko.applyBindings(vm);

    initPage();
  });
};

export default VenueEdit;
