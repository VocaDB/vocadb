import VenueRepository from '../Repositories/VenueRepository';
import UrlMapper from '../Shared/UrlMapper';
import ArchivedEntryViewModel from '../ViewModels/ArchivedEntryViewModel';

const VenueViewVersion = (model: {
  entry: {
    archivedVersion: {
      version: number;
    };
    venue: {
      id: number;
    };
  };
}) => {
  $(function () {
    $('#downloadXmlLink').button({
      icons: { primary: 'ui-icon-arrowthickstop-1-s' },
    });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
    $('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

    var rep = new VenueRepository(new UrlMapper(vdb.values.baseAddress));
    var viewModel = new ArchivedEntryViewModel(
      model.entry.venue.id,
      model.entry.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default VenueViewVersion;
