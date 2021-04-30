import ArtistRepository from '../Repositories/ArtistRepository';
import ArchivedArtistViewModel from '../ViewModels/Artist/ArchivedArtistViewModel';

const ArtistViewVersion = (model: {
  archivedVersion: {
    version: number;
  };
  artist: {
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

    var rep = new ArtistRepository(
      vdb.values.baseAddress,
      vdb.values.languagePreference,
    );

    var viewModel = new ArchivedArtistViewModel(
      model.artist.id,
      model.archivedVersion.version,
      rep,
    );
    ko.applyBindings(viewModel);
  });
};

export default ArtistViewVersion;
