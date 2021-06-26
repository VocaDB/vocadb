import ResourceRepository from '@Repositories/ResourceRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import CommentListViewModel from '@ViewModels/Comment/CommentListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const CommentCommentsByUser = (model: { id: number }): void => {
	$(function () {
		moment.locale(vdb.values.culture);
		ko.punches.enableAll();

		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var resourceRepo = new ResourceRepository(
			httpClient,
			vdb.values.baseAddress,
		);
		var cultureCode = vdb.values.uiCulture;
		var userId = model.id;

		var vm = new CommentListViewModel(
			urlMapper,
			resourceRepo,
			cultureCode,
			userId,
		);
		ko.applyBindings(vm);
	});
};

export default CommentCommentsByUser;
