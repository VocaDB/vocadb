import LocalizedStringWithIdContract from '@DataContracts/Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '@DataContracts/WebLinkContract';
import ReleaseEventRepository from '@Repositories/ReleaseEventRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ReleaseEventSeriesEditViewModel from '@ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const EventEditSeries = (model: {
  defaultNameLanguage: string;
  id: number;
  names: LocalizedStringWithIdContract[];
  webLinks: WebLinkContract[];
}): void => {
  $(function () {
    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
    var userRepo = new UserRepository(httpClient, urlMapper);

    var vm = new ReleaseEventSeriesEditViewModel(
      eventRepo,
      userRepo,
      urlMapper,
      model.id,
      model.defaultNameLanguage,
      model.names,
      model.webLinks,
    );
    ko.applyBindings(vm);

    initPage();
  });
};

export default EventEditSeries;
