import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import UserApiContract from '@DataContracts/User/UserApiContract';
import UserMessageSummaryContract from '@DataContracts/User/UserMessageSummaryContract';
import { UserInboxType } from '@Repositories/UserRepository';
import UserMessagesViewModel from '@ViewModels/User/UserMessagesViewModel';

import FakeUserRepository from '../../TestSupport/FakeUserRepository';

var receiver: UserApiContract;
var data: PartialFindResultContract<UserMessageSummaryContract>;
var sender: UserApiContract;
var repository: FakeUserRepository;

var createMessage = (
  id: number,
  subject: string,
  sender?: UserApiContract,
): UserMessageSummaryContract => {
  return {
    createdFormatted: '2039.3.9',
    highPriority: false,
    id: id,
    inbox: 'Received',
    read: false,
    sender: sender,
    receiver: receiver,
    subject: subject,
  };
};

var createViewModel = (): UserMessagesViewModel => {
  return new UserMessagesViewModel(repository, null!, UserInboxType.Received);
};

QUnit.module('UserMessagesViewModel', {
  setup: () => {
    receiver = { id: 39, name: 'Rin' };
    sender = { id: 39, name: 'Miku' };
    data = {
      items: [
        createMessage(39, 'New message!', sender),
        createMessage(40, 'Another message!', sender),
      ],
      totalCount: 0,
    };

    repository = new FakeUserRepository();
    repository.message = {
      body: 'Message body',
      createdFormatted: null!,
      highPriority: false,
      id: 39,
      inbox: 'Received',
      read: false,
      receiver: null!,
      sender: null!,
      subject: 'New message',
    };
    repository.messages = data.items;
  },
});

test('constructor', () => {
  var viewModel = createViewModel();

  equal(
    viewModel.receivedMessages.items().length,
    2,
    'viewModel.receivedMessages.messages().length',
  );
  equal(
    viewModel.receivedMessages.items()[0].subject,
    'New message!',
    'viewModel.receivedMessages.messages()[0].subject',
  );
  equal(viewModel.selectedMessage(), null, 'viewModel.selectedMessage()');
  equal(viewModel.selectedMessageBody(), '', 'viewModel.selectedMessageBody()');
});

test('selectMessage', () => {
  var viewModel = createViewModel();
  var message = viewModel.receivedMessages.items()[0];

  ok(message, 'message');

  viewModel.selectMessage(message);

  ok(viewModel.selectedMessage(), 'viewModel.selectedMessage()');
  equal(
    viewModel.selectedMessage().subject,
    'New message!',
    'viewModel.selectedMessage().subject',
  );
  equal(
    viewModel.selectedMessageBody(),
    'Message body',
    'viewModel.selectedMessageBody()',
  );
  equal(message.selected(), true, 'message.selected()');
});

test('selectMessage deselectes others', () => {
  var viewModel = createViewModel();
  var message1 = viewModel.receivedMessages.items()[0];
  var message2 = viewModel.receivedMessages.items()[1];

  ok(message1, 'message1');
  ok(message2, 'message2');

  viewModel.selectMessage(message1);
  viewModel.selectMessage(message2);

  ok(viewModel.selectedMessage(), 'viewModel.selectedMessage()');
  equal(
    viewModel.selectedMessage().subject,
    'Another message!',
    'viewModel.selectedMessage().subject',
  );
  equal(message1.selected(), false, 'message1.selected()');
  equal(message2.selected(), true, 'message2.selected()');
});
