import TagRepository from '../Repositories/TagRepository';
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
}) => {
  $(function () {
    $('#downloadXmlLink').button({
      icons: { primary: 'ui-icon-arrowthickstop-1-s' },
    });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
    $('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

    var rep = new TagRepository(vdb.values.baseAddress);
    var viewModel = new ArchivedEntryViewModel(
      model.entry.tag.id,
      model.entry.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default TagViewVersion;
