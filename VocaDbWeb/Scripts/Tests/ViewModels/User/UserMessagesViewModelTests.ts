/// <reference path="../../../typings/qunit/qunit.d.ts" />
/// <reference path="../../TestSupport/FakeUserRepository.ts" />

module vdb.tests.viewModels {

    import dc = vdb.dataContracts;
    import vm = vdb.viewModels;
    import sup = vdb.tests.testSupport;

    var receiver: dc.UserWithIconContract;
    var data: dc.PartialFindResultContract<dc.UserMessageSummaryContract>;
    var sender: dc.UserWithIconContract;
    var repository: sup.FakeUserRepository;

    var createMessage = (id: number, subject: string, sender?: dc.UserWithIconContract) => {
        return { createdFormatted: "2039.3.9", highPriority: false, id: id, read: false, sender: sender, receiver: receiver, subject: subject }
    };

    var createViewModel = () => {
        return new vm.UserMessagesViewModel(repository, null, repositories.UserInboxType.Received);
    };

    QUnit.module("UserMessagesViewModel", {
        setup: () => {

            receiver = { id: 39, name: "Rin" };
            sender = { id: 39, name: "Miku" };
            data = {
                items: [
                    createMessage(39, "New message!", sender),
                    createMessage(40, "Notification"),
                    createMessage(41, "Sent message", receiver)
                ],
				totalCount: 0
            };

            repository = new sup.FakeUserRepository();
            repository.messageBody = "Message body";

        }
    });

    test("constructor", () => {

        var viewModel = createViewModel();

        equal(viewModel.notifications.items().length, 1, "viewModel.notifications.messages().length");
        equal(viewModel.notifications.items()[0].subject, "Notification", "viewModel.notifications.messages()[0].subject");
        equal(viewModel.receivedMessages.items().length, 1, "viewModel.receivedMessages.messages().length");
        equal(viewModel.receivedMessages.items()[0].subject, "New message!", "viewModel.receivedMessages.messages()[0].subject");
        equal(viewModel.sentMessages.items().length, 1, "viewModel.sentMessages.messages().length");
        equal(viewModel.sentMessages.items()[0].subject, "Sent message", "viewModel.sentMessages.messages()[0].subject");
        equal(viewModel.selectedMessage(), null, "viewModel.selectedMessage()");
        equal(viewModel.selectedMessageBody(), "", "viewModel.selectedMessageBody()");

    });

    test("selectMessage", () => {

        var viewModel = createViewModel();
        var message = viewModel.notifications.items()[0];

        viewModel.selectMessage(message);

        ok(viewModel.selectedMessage(), "viewModel.selectedMessage()");
        equal(viewModel.selectedMessage().subject, "Notification", "viewModel.selectedMessage().subject");
        equal(viewModel.selectedMessageBody(), "Message body", "viewModel.selectedMessageBody()");
        equal(message.selected(), true, "message.selected()");

    });

    test("selectMessage deselectes others", () => {

        var viewModel = createViewModel();
        var message1 = viewModel.notifications.items()[0];
        var message2 = viewModel.receivedMessages.items()[0];

        viewModel.selectMessage(message1);
        viewModel.selectMessage(message2);

        ok(viewModel.selectedMessage(), "viewModel.selectedMessage()");
        equal(viewModel.selectedMessage().subject, "New message!", "viewModel.selectedMessage().subject");
        equal(message1.selected(), false, "message1.selected()");
        equal(message2.selected(), true, "message2.selected()");

    });

}