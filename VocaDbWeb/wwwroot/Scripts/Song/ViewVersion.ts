import SongRepository from '../Repositories/SongRepository';
import ArchivedSongViewModel from '../ViewModels/Song/ArchivedSongViewModel';

const SongViewVersion = (model: {
  archivedVersion: {
    version: number;
  };
  song: {
    id: number;
  };
}) => {
  $(function () {
    $('#revertLink').button({ icons: { primary: 'ui-icon-arrowrefresh-1-w' } });
    $('#downloadXmlLink').button({
      icons: { primary: 'ui-icon-arrowthickstop-1-s' },
    });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
    $('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

    var rep = new SongRepository(
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );

    var viewModel = new ArchivedSongViewModel(
      model.song.id,
      model.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default SongViewVersion;
