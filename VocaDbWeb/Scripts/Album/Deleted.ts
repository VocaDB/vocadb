import AlbumRepository from '@Repositories/AlbumRepository';
import HttpClient from '@Shared/HttpClient';
import DeletedAlbumsViewModel from '@ViewModels/Album/DeletedAlbumsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumDeleted = (): void => {
  $(function () {
    const httpClient = new HttpClient();
    var repo = new AlbumRepository(httpClient);
    var viewModel = new DeletedAlbumsViewModel(repo);
    ko.applyBindings(viewModel);
  });
};

export default AlbumDeleted;
