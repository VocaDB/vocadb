import EntryReportRepository from '@Repositories/EntryReportRepository';
import UserRepository from '@Repositories/UserRepository';
import TopBarViewModel from '@ViewModels/TopBarViewModel';

import HttpClient from './HttpClient';
import ui from './MessagesTyped';
import UrlMapper from './UrlMapper';

const SharedLayoutScripts = (model: {
	entryTypeTranslations: any;
	getNewReportCount: boolean;
	hasUnreadMessages: boolean;
	reportSent: any;
	rootPath: string;
	thanksForRating: string;
	topBarData: any;
	unreadMessagesCount: number;
}): void => {
	const httpClient = new HttpClient();
	var urlMapper = new UrlMapper(model.rootPath);

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
