import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import AlbumCreateViewModel from '@ViewModels/Album/AlbumCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumCreate = (): void => {
	$(function () {
		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var albumRepo = repoFactory.albumRepository();
		var artistRepo = repoFactory.artistRepository();

		ko.applyBindings(
			new AlbumCreateViewModel(vdb.values, albumRepo, artistRepo),
		);
	});
};

export default AlbumCreate;
