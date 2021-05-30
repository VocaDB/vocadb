import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import AlbumCollectionViewModel from '@ViewModels/User/AlbumCollectionViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserAlbumCollection = (
  model: {
    user: {
      id: number;
    };
  },
  publicCollection: boolean,
): void => {
  $(document).ready(function () {
    var cultureCode = vdb.values.uiCulture;
    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    var loggedUserId = model.user.id;

    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(httpClient, urlMapper);
    var userRepo = repoFactory.userRepository();
    var artistRepo = repoFactory.artistRepository();
    var resourceRepo = repoFactory.resourceRepository();

    var vm = new AlbumCollectionViewModel(
      userRepo,
      artistRepo,
      resourceRepo,
      languageSelection,
      loggedUserId,
      cultureCode,
      publicCollection,
    );
    ko.applyBindings(vm);
  });
};

export default UserAlbumCollection;
