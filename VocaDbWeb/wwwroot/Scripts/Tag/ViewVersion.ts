import TagRepository from '../Repositories/TagRepository';
import HttpClient from '../Shared/HttpClient';
import ArchivedEntryViewModel from '../ViewModels/ArchivedEntryViewModel';

const TagViewVersion = (model: {
  entry: {
    archivedVersion: {
      version: number;
    };
    tag: {
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
    var rep = new TagRepository(httpClient, vdb.values.baseAddress);
    var viewModel = new ArchivedEntryViewModel(
      model.entry.tag.id,
      model.entry.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default TagViewVersion;
