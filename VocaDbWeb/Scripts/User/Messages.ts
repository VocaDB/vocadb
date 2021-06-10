import RepositoryFactory from '@Repositories/RepositoryFactory';
import { UserInboxType } from '@Repositories/UserRepository';
import ui from '@Shared/MessagesTyped';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
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

		var repoFactory = container.get(RepositoryFactory);
		var repository = repoFactory.userRepository();
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
