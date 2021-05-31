import VenueRepository from '@Repositories/VenueRepository';
import HttpClient from '@Shared/HttpClient';
import ArchivedEntryViewModel from '@ViewModels/ArchivedEntryViewModel';
import $ from 'jquery';
import ko from 'knockout';

const VenueViewVersion = (model: {
  entry: {
    archivedVersion: {
      version: number;
    };
    venue: {
      id: number;
    };
  };
}): void => {
  $(function () {
    $('#downloadXmlLink').button({
      icons: { primary: 'ui-icon-arrowthickstop-1-s' },
    });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
    $('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

    const httpClient = new HttpClient();
    var rep = new VenueRepository(httpClient);
    var viewModel = new ArchivedEntryViewModel(
      model.entry.venue.id,
      model.entry.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default VenueViewVersion;
