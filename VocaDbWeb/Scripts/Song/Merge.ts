import SongContract from '@DataContracts/Song/SongContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import SongMergeViewModel from '@ViewModels/Song/SongMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const SongMerge = (model: SongContract): void => {
	$(function () {
		var repo = repoFactory.songRepository();
		var data = model;
		var vm = new SongMergeViewModel(vocaDbContext, repo, data);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the songs?');
		});
	});
};

export default SongMerge;
