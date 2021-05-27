import SongRepository from '@Repositories/SongRepository';
import HttpClient from '@Shared/HttpClient';
import ArchivedSongViewModel from '@ViewModels/Song/ArchivedSongViewModel';
import $ from 'jquery';
import ko from 'knockout';

const SongViewVersion = (model: {
  archivedVersion: {
    version: number;
  };
  song: {
    id: number;
  };
}): void => {
  $(function () {
    $('#revertLink').button({ icons: { primary: 'ui-icon-arrowrefresh-1-w' } });
    $('#downloadXmlLink').button({
      icons: { primary: 'ui-icon-arrowthickstop-1-s' },
    });
    $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
    $('#showLink').button({ icons: { primary: 'ui-icon-unlocked' } });
    $('#hideLink').button({ icons: { primary: 'ui-icon-locked' } });

    const httpClient = new HttpClient();
    var rep = new SongRepository(
      httpClient,
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
