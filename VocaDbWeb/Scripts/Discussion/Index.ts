import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import DiscussionIndexViewModel from '@ViewModels/Discussion/DiscussionIndexViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const DiscussionIndex = (canDeleteAllComments: boolean): void => {
	$(function () {
		moment.locale(vdb.values.culture);

		ko.punches.enableAll();

		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var repo = repoFactory.discussionRepository();
		ko.applyBindings(
			new DiscussionIndexViewModel(repo, urlMapper, canDeleteAllComments),
		);
	});
};

export default DiscussionIndex;
