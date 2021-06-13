import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import DiscussionIndexViewModel from '@ViewModels/Discussion/DiscussionIndexViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const DiscussionIndex = (canDeleteAllComments: boolean): void => {
	$(function () {
		moment.locale(vocaDbContext.culture);

		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vocaDbContext.baseAddress);
		var repo = repoFactory.discussionRepository();
		ko.applyBindings(
			new DiscussionIndexViewModel(
				vocaDbContext,
				repo,
				urlMapper,
				canDeleteAllComments,
			),
		);
	});
};

export default DiscussionIndex;
