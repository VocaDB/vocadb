import AlbumForEditContract from '@DataContracts/Album/AlbumForEditContract';
import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import DialogService from '@Shared/DialogService';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import AlbumEditViewModel from '@ViewModels/Album/AlbumEditViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

function initPage(): void {
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#mergeLink').button();
  $('#pvLoader')
    .ajaxStart(function (this: any) {
      $(this).show();
    })
    .ajaxStop(function (this: any) {
      $(this).hide();
    });
}

const AlbumEdit = (
  allowCustomTracks: boolean,
  artistRoleJson: { [key: string]: string },
  canBulkDeletePVs: boolean,
  model: {
    editedAlbum: AlbumForEditContract;
    id: number;
  },
  saveWarning: string,
  webLinkCategoryJson: TranslatedEnumField[],
): void => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    vdb.resources.entryEdit = {
      saveWarning: saveWarning,
    };

    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);

    var repoFactory = new RepositoryFactory(httpClient, urlMapper);
    var repo = repoFactory.albumRepository();
    var songRepo = repoFactory.songRepository();
    var artistRepo = repoFactory.artistRepository();
    var pvRepo = repoFactory.pvRepository();
    var userRepo = repoFactory.userRepository();
    var eventRepo = repoFactory.eventRepository();
    var editedModel = model.editedAlbum;
    var viewModel;

    if (editedModel) {
      viewModel = new AlbumEditViewModel(
        repo,
        songRepo,
        artistRepo,
        pvRepo,
        userRepo,
        eventRepo,
        urlMapper,
        artistRoleJson,
        webLinkCategoryJson,
        editedModel,
        allowCustomTracks,
        canBulkDeletePVs,
        new DialogService(),
      );

      ko.applyBindings(viewModel);
    } else {
      repo.getForEdit(model.id).then(function (model) {
        viewModel = new AlbumEditViewModel(
          repo,
          songRepo,
          artistRepo,
          pvRepo,
          userRepo,
          eventRepo,
          urlMapper,
          artistRoleJson,
          webLinkCategoryJson,
          model,
          allowCustomTracks,
          canBulkDeletePVs,
          new DialogService(),
        );

        ko.applyBindings(viewModel);
      });
    }

    initPage();
  });
};

export default AlbumEdit;
