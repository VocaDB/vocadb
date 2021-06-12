import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import TagCreateViewModel from '@ViewModels/Tag/TagCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const TagIndex = (): void => {
	$(function () {
		var tagRepo = repoFactory.tagRepository();
		var viewModel = new TagCreateViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default TagIndex;
