import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import ArtistCreateViewModel from '@ViewModels/ArtistCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const ArtistCreate = (model: any): void => {
	$(document).ready(function () {
		ko.punches.enableAll();
		var repoFactory = container.get(RepositoryFactory);
		var repo = repoFactory.artistRepository();
		var tagRepo = repoFactory.tagRepository();
		var json = model;
		ko.applyBindings(new ArtistCreateViewModel(repo, tagRepo, json));
	});
};

export default ArtistCreate;
