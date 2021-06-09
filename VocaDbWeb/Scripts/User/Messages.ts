import UserRepository, { UserInboxType } from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import ui from '@Shared/MessagesTyped';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import UserMessagesViewModel from '@ViewModels/User/UserMessagesViewModel';
import $ from 'jquery';
import ko from 'knockout';

const UserMessages = (
	message: string,
	model: {
		inbox: UserInboxType;
		receiverName: string;
		selectedMessageId: number;
	},
): void => {
	$(function () {
		$('#tabs').tabs();

		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var repository = new UserRepository(httpClient, urlMapper);
		var receiverName = model.receiverName;
		var viewModel = new UserMessagesViewModel(
			repository,
			vdb.values.loggedUserId,
			model.inbox,
			model.selectedMessageId,
			receiverName,
		);
		viewModel.messageSent = function (): void {
			ui.showSuccessMessage(message);
		};
		ko.applyBindings(viewModel);
	});
};

export default UserMessages;
