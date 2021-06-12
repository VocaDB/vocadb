import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import AlbumMergeViewModel from '@ViewModels/Album/AlbumMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumMerge = (model: { id: number }): void => {
	$(function () {
		const repoFactory = container.get(RepositoryFactory);
		var repo = repoFactory.albumRepository();
		var vm = new AlbumMergeViewModel(repo, model.id);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the albums?');
		});
	});
};

export default AlbumMerge;
