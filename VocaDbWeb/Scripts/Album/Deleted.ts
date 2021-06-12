import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import DeletedAlbumsViewModel from '@ViewModels/Album/DeletedAlbumsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const AlbumDeleted = (): void => {
	$(function () {
		var repo = repoFactory.albumRepository();
		var viewModel = new DeletedAlbumsViewModel(vocaDbContext, repo);
		ko.applyBindings(viewModel);
	});
};

export default AlbumDeleted;
