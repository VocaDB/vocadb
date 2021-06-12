import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import SongListEditViewModel from '@ViewModels/SongList/SongListEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

function initPage(
	repoFactory: RepositoryFactory,
	urlMapper: UrlMapper,
	listId: number,
): void {
	$('#tabs').tabs();
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });

	var songListRepo = repoFactory.songListRepository();
	var songRepo = repoFactory.songRepository();

	var viewModel = new SongListEditViewModel(
		songListRepo,
		songRepo,
		urlMapper,
		listId,
	);
	viewModel.init(function () {
		ko.applyBindings(viewModel);
	});
}

const SongListEdit = (model: { id: number }): void => {
	$(document).ready(function () {
		var urlMapper = new UrlMapper(vocaDbContext.baseAddress);
		initPage(repoFactory, urlMapper, model.id);
	});
};

export default SongListEdit;
