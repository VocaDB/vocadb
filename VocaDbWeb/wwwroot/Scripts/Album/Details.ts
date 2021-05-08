import AlbumDetailsViewModel, {
  AlbumDetailsAjax,
} from '../ViewModels/Album/AlbumDetailsViewModel';
import ui from '../Shared/MessagesTyped';
import UrlMapper from '../Shared/UrlMapper';
import RepositoryFactory from '../Repositories/RepositoryFactory';
import { IEntryReportType } from '../ViewModels/ReportEntryViewModel';
import HttpClient from '../Shared/HttpClient';

function initAlbumDetailsPage(
  albumId: number,
  collectionRating: number,
  saveStr: string,
  urlMapper: UrlMapper,
  viewModel: AlbumDetailsViewModel,
): void {
  $('#addAlbumLink').button({
    disabled: $('#addAlbumLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-star' },
  });
  $('#updateAlbumLink').button({
    disabled: $('#updateAlbumLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-wrench' },
  });
  $('#editAlbumLink').button({
    disabled: $('#editAlbumLink').hasClass('disabled'),
    icons: { primary: 'ui-icon-wrench' },
  });
  $('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
  $('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
  $('#downloadTags')
    .button({ icons: { primary: 'ui-icon-arrowthickstop-1-s' } })
    .next()
    .button({ text: false, icons: { primary: 'ui-icon-triangle-1-s' } })
    .parent()
    .buttonset();
  $('#manageTags').button({ icons: { primary: 'ui-icon-wrench' } });
  $('#viewCommentsLink').click(function () {
    $('#tabs').tabs('option', 'active', 1);
    return false;
  });
  $('#viewReviewsLink').click(function () {
    $('#tabs').tabs('option', 'active', 2);
    return false;
  });

  $('#picCarousel').carousel({ interval: false });

  $('#collectionRating').jqxRating();

  if (collectionRating != 0) {
    $('#collectionRating').jqxRating({ value: collectionRating });
  }

  $('#removeRating').click(function () {
    $('#collectionRating').jqxRating('setValue', 0);

    return false;
  });

  $('#tabs').tabs({
    load: function (event, ui) {
      vdb.functions.disableTabReload(ui.tab);
    },
    activate: function (event, ui) {
      switch (ui.newTab.data('tab')) {
        case 'Discussion':
          viewModel.comments.initComments();
          break;
        case 'Reviews':
          viewModel.reviewsViewModel.loadReviews();
          break;
      }
    },
  });

  $('#editCollectionDialog').dialog({
    autoOpen: false,
    width: 320,
    modal: false,
    buttons: [
      ({
        text: saveStr,
        click: function () {
          $('#editCollectionDialog').dialog('close');

          var status = $('#collectionStatusSelect').val();
          var mediaType = $('#collectionMediaSelect').val();
          var rating = $('#collectionRating').jqxRating('getValue');

          $.post(
            urlMapper.mapRelative('/User/UpdateAlbumForUser'),
            {
              albumId: albumId,
              collectionStatus: status,
              mediaType: mediaType,
              rating: rating,
            },
            null!,
          );

          if (status == 'Nothing') {
            $('#updateAlbumLink').hide();
            $('#addAlbumLink').show();
          } else {
            $('#addAlbumLink').hide();
            $('#updateAlbumLink').show();
          }

          ui.showSuccessMessage(vdb.resources.album.addedToCollection);
        },
      } as unknown) as JQueryUI.ButtonOptions,
    ],
  });

  var addAlbumLink;
  if ($('#addAlbumLink').is(':visible')) addAlbumLink = $('#addAlbumLink');
  else addAlbumLink = $('#updateAlbumLink');

  $('#editCollectionDialog').dialog('option', 'position', {
    my: 'left top',
    at: 'left bottom',
    of: addAlbumLink,
  });

  $('#addAlbumLink').click(function () {
    $('#editCollectionDialog').dialog('open');
    return false;
  });

  $('#updateAlbumLink').click(function () {
    $('#editCollectionDialog').dialog('open');
    return false;
  });

  $('td.artistList a').vdbArtistToolTip();

  $('#userCollectionsPopup').dialog({
    autoOpen: false,
    width: 400,
    position: { my: 'left top', at: 'left bottom', of: $('#statsLink') },
  });
}

const AlbumDetails = (
  addedToCollection: string,
  albumDetails: typeof vdb.resources.albumDetails,
  canDeleteAllComments: boolean,
  formatString: string,
  model: {
    collectionRating: number;
    id: number;
    jsonModel: AlbumDetailsAjax;
  },
  reportTypes: IEntryReportType[],
  saveStr: string,
  showTranslatedDescription: boolean,
): void => {
  $(document).ready(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
      vdb.values.loggedUserId,
    );
    var albumRepo = repoFactory.albumRepository();
    var userRepo = repoFactory.userRepository();
    var artistRepo = repoFactory.artistRepository();

    vdb.resources.album = {
      addedToCollection: addedToCollection,
    };
    vdb.resources.albumDetails = albumDetails;

    var jsonModel = model.jsonModel;
    var viewModel = new AlbumDetailsViewModel(
      albumRepo,
      userRepo,
      artistRepo,
      jsonModel,
      reportTypes,
      vdb.values.loggedUserId,
      vdb.values.languagePreference,
      canDeleteAllComments,
      formatString,
      showTranslatedDescription,
    );
    ko.applyBindings(viewModel);

    initAlbumDetailsPage(
      model.id,
      model.collectionRating,
      saveStr,
      urlMapper,
      viewModel,
    );
  });
};

export default AlbumDetails;
