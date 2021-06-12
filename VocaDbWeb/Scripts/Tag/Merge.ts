import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import { container } from '@Shared/inversify.config';
import TagMergeViewModel from '@ViewModels/Tag/TagMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const repoFactory = container.get(RepositoryFactory);

const TagMerge = (model: TagBaseContract): void => {
	$(function () {
		var repo = repoFactory.tagRepository();
		var data = model;
		var vm = new TagMergeViewModel(repo, data);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the tags?');
		});
	});
};

export default TagMerge;
