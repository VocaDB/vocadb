import ReleaseEventRepository from '../Repositories/ReleaseEventRepository';
import UrlMapper from '../Shared/UrlMapper';
import ArchivedEntryViewModel from '../ViewModels/ArchivedEntryViewModel';

const EventViewVersion = (model: {
  entry: {
    archivedVersion: {
      version: number;
    };
    releaseEvent: {
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

    var rep = new ReleaseEventRepository(new UrlMapper(vdb.values.baseAddress));
    var viewModel = new ArchivedEntryViewModel(
      model.entry.releaseEvent.id,
      model.entry.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default EventViewVersion;
