import SongContract from '@DataContracts/Song/SongContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import SongMergeViewModel from '@ViewModels/Song/SongMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const SongMerge = (model: SongContract): void => {
	$(function () {
		var repoFactory = container.get(RepositoryFactory);
		var repo = repoFactory.songRepository();
		var data = model;
		var vm = new SongMergeViewModel(repo, data);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the songs?');
		});
	});
};

export default SongMerge;
