import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import ArtistMergeViewModel from '@ViewModels/Artist/ArtistMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const ArtistMerge = (model: { id: number }): void => {
	$(function () {
		var repo = repoFactory.artistRepository();
		var vm = new ArtistMergeViewModel(vocaDbContext, repo, model.id);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the artists?');
		});
	});
};

export default ArtistMerge;
