import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import TopBarViewModel from '@ViewModels/TopBarViewModel';

import HttpClient from './HttpClient';
import ui from './MessagesTyped';
import UrlMapper from './UrlMapper';
import vdb from './VdbStatic';

const SharedLayoutScripts = (model: {
	culture: string;
	entryTypeTranslations: any;
	getNewReportCount: boolean;
	hasUnreadMessages: boolean;
	isLoggedIn: boolean;
	languagePreference: number;
	loggedUserId: number;
	reportSent: any;
	rootPath: string;
	thanksForRating: string;
	topBarData: any;
	uiCulture: string;
	unreadMessagesCount: number;
}): void => {
	var baseAddress = model.rootPath;

	vdb.values = vdb.values || {};
	vdb.values.baseAddress = baseAddress;
	vdb.values.languagePreference = model.languagePreference;
	vdb.values.isLoggedIn = model.isLoggedIn;
	vdb.values.loggedUserId = model.loggedUserId;
	vdb.values.culture = model.culture;
	vdb.values.uiCulture = model.uiCulture;
	vdb.resources = vdb.resources || {};
	const httpClient = new HttpClient();
	var urlMapper = new UrlMapper(baseAddress);

	vdb.resources.shared = {
		reportSent: model.reportSent,
	};

	ui.initAll(model.thanksForRating);

	var entryReportRepository = new EntryReportRepository(httpClient, urlMapper);
	var userRepository = new UserRepository(httpClient, urlMapper);

	var entryTypeTranslations = model.entryTypeTranslations;
	var topBarData = model.topBarData;
	var unreadMessagesCount = model.unreadMessagesCount;
	var getNewReportCount = model.getNewReportCount;

	var topBarViewModel = new TopBarViewModel(
		entryTypeTranslations,
		topBarData.searchObjectType,
		'',
		unreadMessagesCount,
		getNewReportCount,
		entryReportRepository,
		userRepository,
	);

	var topBar = $('#topBar')[0];
	if (topBar) {
		ko.applyBindings(topBarViewModel, $('#topBar')[0]);
	}
};

export default SharedLayoutScripts;
