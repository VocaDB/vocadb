import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import CommentListViewModel from '@ViewModels/Comment/CommentListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const CommentCommentsByUser = (model: { id: number }): void => {
	$(function () {
		moment.locale(vocaDbContext.culture);
		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vocaDbContext.baseAddress);
		var resourceRepo = repoFactory.resourceRepository();
		var userId = model.id;

		var vm = new CommentListViewModel(
			vocaDbContext,
			urlMapper,
			resourceRepo,
			userId,
		);
		ko.applyBindings(vm);
	});
};

export default CommentCommentsByUser;
