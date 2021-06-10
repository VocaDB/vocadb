import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import AlbumCreateViewModel from '@ViewModels/Album/AlbumCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumCreate = (): void => {
	$(function () {
		var repoFactory = container.get(RepositoryFactory);
		var albumRepo = repoFactory.albumRepository();
		var artistRepo = repoFactory.artistRepository();

		ko.applyBindings(new AlbumCreateViewModel(albumRepo, artistRepo));
	});
};

export default AlbumCreate;
