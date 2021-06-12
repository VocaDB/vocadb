import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import ArtistCreateViewModel from '@ViewModels/ArtistCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const ArtistCreate = (model: any): void => {
	$(document).ready(function () {
		ko.punches.enableAll();
		var repo = repoFactory.artistRepository();
		var tagRepo = repoFactory.tagRepository();
		var json = model;
		ko.applyBindings(
			new ArtistCreateViewModel(vocaDbContext, repo, tagRepo, json),
		);
	});
};

export default ArtistCreate;
