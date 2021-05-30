import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import AlbumCreateViewModel from '@ViewModels/Album/AlbumCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumCreate = (): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(httpClient);
    var albumRepo = repoFactory.albumRepository();
    var artistRepo = repoFactory.artistRepository();

    ko.applyBindings(new AlbumCreateViewModel(albumRepo, artistRepo));
  });
};

export default AlbumCreate;
