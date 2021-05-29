import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { IEntryReportType } from '@ViewModels/ReportEntryViewModel';
import VenueDetailsViewModel from '@ViewModels/Venue/VenueDetailsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const VenueDetails = (
  model: {
    id: number;
  },
  reportTypes: IEntryReportType[],
): void => {
  $(function () {
    $('#editVenueLink').button({
      disabled: $('#editVenueLink').hasClass('disabled'),
      icons: { primary: 'ui-icon-wrench' },
    });
    $('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
    $('#createEventLink').button({ icons: { primary: 'ui-icon-plus' } });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });

    var loggedUserId = vdb.values.loggedUserId;
    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
    );
    var venueRepo = repoFactory.venueRepository();

    var vm = new VenueDetailsViewModel(
      venueRepo,
      reportTypes,
      loggedUserId,
      model.id,
    );
    ko.applyBindings(vm);
  });
};

export default VenueDetails;
