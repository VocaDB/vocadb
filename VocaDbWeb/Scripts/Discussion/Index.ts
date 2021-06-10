import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import DiscussionIndexViewModel from '@ViewModels/Discussion/DiscussionIndexViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const DiscussionIndex = (canDeleteAllComments: boolean): void => {
	$(function () {
		moment.locale(vdb.values.culture);

		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var repoFactory = container.get(RepositoryFactory);
		var repo = repoFactory.discussionRepository();
		ko.applyBindings(
			new DiscussionIndexViewModel(
				repo,
				urlMapper,
				canDeleteAllComments,
				vdb.values.loggedUserId,
			),
		);
	});
};

export default DiscussionIndex;
