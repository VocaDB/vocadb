import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import DeletedAlbumsViewModel from '@ViewModels/Album/DeletedAlbumsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const AlbumDeleted = (): void => {
	$(function () {
		var repo = repoFactory.albumRepository();
		var viewModel = new DeletedAlbumsViewModel(repo);
		ko.applyBindings(viewModel);
	});
};

export default AlbumDeleted;
