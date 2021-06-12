import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import AlbumCreateViewModel from '@ViewModels/Album/AlbumCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const AlbumCreate = (): void => {
	$(function () {
		var albumRepo = repoFactory.albumRepository();
		var artistRepo = repoFactory.artistRepository();

		ko.applyBindings(
			new AlbumCreateViewModel(vocaDbContext, albumRepo, artistRepo),
		);
	});
};

export default AlbumCreate;
