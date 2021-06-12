import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import CommentListViewModel from '@ViewModels/Comment/CommentListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const CommentCommentsByUser = (model: { id: number }): void => {
	$(function () {
		moment.locale(vdb.values.culture);
		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		const repoFactory = container.get(RepositoryFactory);
		var resourceRepo = repoFactory.resourceRepository();
		var lang = vdb.values.languagePreference;
		var cultureCode = vdb.values.uiCulture;
		var userId = model.id;

		var vm = new CommentListViewModel(
			urlMapper,
			resourceRepo,
			lang,
			cultureCode,
			userId,
		);
		ko.applyBindings(vm);
	});
};

export default CommentCommentsByUser;
