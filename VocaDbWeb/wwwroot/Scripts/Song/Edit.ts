import SongForEditContract from '../DataContracts/Song/SongForEditContract';
import TranslatedEnumField from '../DataContracts/TranslatedEnumField';
import RepositoryFactory from '../Repositories/RepositoryFactory';
import DialogService from '../Shared/DialogService';
import UrlMapper from '../Shared/UrlMapper';
import SongEditViewModel from '../ViewModels/Song/SongEditViewModel';

function initPage() {
  $('#tabs').tabs();
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#mergeLink').button();
  $('#pvLoader')
    .ajaxStart(function () {
      $(this).show();
    })
    .ajaxStop(function () {
      $(this).hide();
    });

  $('#artistsTableBody a.artistLink').vdbArtistToolTip();
}

const SongEdit = (
  addExtraArtist: string,
  artistRoleJson: { [key: string]: string },
  canBulkDeletePVs: boolean,
  languageNames,
  model: {
    editedSong: SongForEditContract;
    id: number;
    instrumentalTagId: number;
  },
  saveWarning,
  webLinkCategoryJson: TranslatedEnumField[],
) => {
  $(document).ready(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    vdb.resources.entryEdit = {
      saveWarning: saveWarning,
    };

    vdb.resources.song = {
      addExtraArtist: addExtraArtist,
    };

    var editedModel = model.editedSong;
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(
      urlMapper,
      vdb.values.languagePreference,
    );
    var songRepo = repoFactory.songRepository();
    var artistRepo = repoFactory.artistRepository();
    var pvRepo = repoFactory.pvRepository();
    var userRepo = repoFactory.userRepository();
    var instrumentalTagId = model.instrumentalTagId;
    var vm;

    if (editedModel) {
      vm = new SongEditViewModel(
        songRepo,
        artistRepo,
        pvRepo,
        userRepo,
        urlMapper,
        artistRoleJson,
        webLinkCategoryJson,
        editedModel,
        canBulkDeletePVs,
        new DialogService(),
        instrumentalTagId,
        languageNames,
      );
      ko.applyBindings(vm);
    } else {
      songRepo.getForEdit(model.id, function (model) {
        vm = new SongEditViewModel(
          songRepo,
          artistRepo,
          pvRepo,
          userRepo,
          urlMapper,
          artistRoleJson,
          webLinkCategoryJson,
          model,
          canBulkDeletePVs,
          new DialogService(),
          instrumentalTagId,
          languageNames,
        );
        ko.applyBindings(vm);
      });
    }

    initPage();
  });
};

export default SongEdit;
