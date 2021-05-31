import ResourceRepository from '@Repositories/ResourceRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import ActivityEntryListViewModel from '@ViewModels/ActivityEntry/ActivityEntryListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const UserEntryEdits = (
	additionsOnly: boolean,
	model: {
		id: number;
	},
): void => {
	$(function () {
		moment.locale(vdb.values.culture);
		ko.punches.enableAll();

		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var resourceRepo = new ResourceRepository(httpClient);
		var lang = vdb.values.languagePreference;
		var cultureCode = vdb.values.uiCulture;
		var userId = model.id;

		var vm = new ActivityEntryListViewModel(
			urlMapper,
			resourceRepo,
			lang,
			cultureCode,
			userId,
			additionsOnly,
		);
		ko.applyBindings(vm);
	});
};

export default UserEntryEdits;
