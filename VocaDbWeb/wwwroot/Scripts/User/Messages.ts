import UserRepository, { UserInboxType } from '../Repositories/UserRepository';
import ui from '../Shared/MessagesTyped';
import UrlMapper from '../Shared/UrlMapper';
import UserMessagesViewModel from '../ViewModels/User/UserMessagesViewModel';

const UserMessages = (
  message: string,
  model: {
    inbox: UserInboxType;
    receiverName: string;
    selectedMessageId: number;
  },
) => {
  $(function () {
    $('#tabs').tabs();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var repository = new UserRepository(urlMapper);
    var receiverName = model.receiverName;
    var viewModel = new UserMessagesViewModel(
      repository,
      vdb.values.loggedUserId,
      model.inbox,
      model.selectedMessageId,
      receiverName,
    );
    viewModel.messageSent = function () {
      ui.showSuccessMessage(message);
    };
    ko.applyBindings(viewModel);
  });
};

export default UserMessages;
