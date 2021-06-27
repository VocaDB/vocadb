import AlbumRepository from '@Repositories/AlbumRepository';
import HttpClient from '@Shared/HttpClient';
import DeletedAlbumsViewModel from '@ViewModels/Album/DeletedAlbumsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumDeleted = (): void => {
	$(function () {
		const httpClient = new HttpClient();
		var repo = new AlbumRepository(httpClient, vdb.values.baseAddress);
		var viewModel = new DeletedAlbumsViewModel(vdb.values, repo);
		ko.applyBindings(viewModel);
	});
};

export default AlbumDeleted;
