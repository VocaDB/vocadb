import TagRepository from '@Repositories/TagRepository';
import HttpClient from '@Shared/HttpClient';
import TagCreateViewModel from '@ViewModels/Tag/TagCreateViewModel';
import $ from 'jquery';
import ko from 'knockout';

const TagIndex = (): void => {
	$(function () {
		const httpClient = new HttpClient();
		var tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
		var viewModel = new TagCreateViewModel(tagRepo);
		ko.applyBindings(viewModel);
	});
};

export default TagIndex;
