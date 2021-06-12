import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import AlbumMergeViewModel from '@ViewModels/Album/AlbumMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const AlbumMerge = (model: { id: number }): void => {
	$(function () {
		var repo = repoFactory.albumRepository();
		var vm = new AlbumMergeViewModel(vocaDbContext, repo, model.id);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the albums?');
		});
	});
};

export default AlbumMerge;
