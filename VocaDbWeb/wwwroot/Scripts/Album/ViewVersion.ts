import AlbumRepository from '../Repositories/AlbumRepository';
import ArchivedAlbumViewModel from '../ViewModels/Album/ArchivedAlbumViewModel';

const AlbumViewVersion = (model: {
  album: {
    id: number;
  };
  archivedVersion: {
    version: number;
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

    var rep = new AlbumRepository(
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );

    var viewModel = new ArchivedAlbumViewModel(
      model.album.id,
      model.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default AlbumViewVersion;
