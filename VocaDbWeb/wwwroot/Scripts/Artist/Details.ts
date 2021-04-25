import ArtistDetailsViewModel from '../ViewModels/Artist/ArtistDetailsViewModel';
import SongRepository from '../Repositories/SongRepository';

export function initPage(
  artistId: number,
  saveStr,
  urlMapper,
  viewModel: ArtistDetailsViewModel,
  songRepo: SongRepository,
) {
  $('#addToUserLink').button({
    disabled: $('#addToUserLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-heart' },
  });
  $('#removeFromUserLink')
    .button({
      disabled: $('#removeFromUserLink').hasClass('disabled'),
      icons: { primary: 'ui-icon-close' },
    })
    .next()
    .button({ text: false, icons: { primary: 'ui-icon-triangle-1-s' } })
    .parent()
    .buttonset();
  $('#editArtistLink').button({
    disabled: $('#editArtistLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-wrench' },
  });
  $('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
  $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
  $('#manageTags').button({ icons: { primary: 'ui-icon-wrench' } });
  $('#viewCommentsLink').click(function () {
    $('#tabs').tabs('option', 'active', 1);
    return false;
  });

  $('#tabs').tabs({
    load: function (event, ui) {
      vdb.functions.disableTabReload(ui.tab);
    },
    activate: function (event, ui) {
      if (ui.newTab.data('tab') === 'Discussion') {
        viewModel.comments.initComments();
      }
    },
  });

  if (window.location.hash == '#mainAlbumsTab') {
    viewModel.initMainAlbums();
  }
  if (window.location.hash == '#collaborationAlbumsTab') {
    viewModel.initCollaborationAlbums();
  }
  if (window.location.hash == '#songsTab') {
    viewModel.initSongs();
  }

  $('#newAlbums img').vdbAlbumToolTip();
  $('#topAlbums img').vdbAlbumToolTip();
  $('#baseVoicebank a').vdbArtistToolTip();
  $('#childVoicebanks a').vdbArtistToolTip();
  $('#groups a').vdbArtistToolTip();
  $('.artistLink').vdbArtistToolTip();
}
