
module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class UserMessagesViewModel {

        constructor(private userRepository: vdb.repositories.UserRepository, data: dc.UserMessagesContract, userId: number, selectedMessageId?: number) {

            this.notifications = new UserMessageFolderViewModel(userRepository, _.filter(data.receivedMessages, m => m.sender == null), userId);
            this.receivedMessages = new UserMessageFolderViewModel(userRepository, _.filter(data.receivedMessages, m => m.sender != null), userId);
            this.sentMessages = new UserMessageFolderViewModel(userRepository, data.sentMessages, userId);

            if (selectedMessageId != null) {
                this.selectMessageById(selectedMessageId);
            }

        }

		newMessageViewModel = new NewMessageViewModel();

        notifications: UserMessageFolderViewModel;

        receivedMessages: UserMessageFolderViewModel;

        sentMessages: UserMessageFolderViewModel;

		reply = () => {

			if (!this.selectedMessage())
				throw Error("No message selected");

			var msg = this.selectedMessage();
			$("#receiverName").val(msg.sender.name);

			$("#newMessageSubject").val("Re: " + msg.subject);

            this.selectTab("#composeTab");

		};

        selectedMessage = ko.observable<UserMessageViewModel>();

        selectedMessageBody: KnockoutObservable<string> = ko.observable("");

        selectMessageById = (messageId: number) => {

            var message = _.find(this.notifications.messages(), msg => msg.id == messageId);

            if (message) {
                this.selectTab("#notificationsTab");
                this.selectMessage(message);
                return;
            }

            message = _.find(this.receivedMessages.messages(), msg => msg.id == messageId);

            if (message) {
                this.selectTab("#receivedTab");
                this.selectMessage(message);
                return;
            }

            message = _.find(this.sentMessages.messages(), msg => msg.id == messageId);

            if (message) {
                this.selectTab("#sentTab");
                this.selectMessage(message);
            }

        };

		selectMessage = (message: UserMessageViewModel) => {

            this.userRepository.getMessageBody(message.id, body => {
                this.selectedMessageBody(body);
            });

            this.receivedMessages.selectMessage(message);
            this.sentMessages.selectMessage(message);
            this.notifications.selectMessage(message);

            message.selected(true);
            message.read(true);
			this.selectedMessage(message);

        };

        selectTab = (tabName: string) => {
            var index = $('#tabs > ul > li > a').index($(tabName));
            $("#tabs").tabs("option", "active", index);
        };

    }

    export class UserMessageFolderViewModel {

        constructor(private userRepo: rep.UserRepository, messages: dc.UserMessageSummaryContract[], private userId) {

            var messageViewModels = _.map(messages, msg => new UserMessageViewModel(msg));
            this.messages(messageViewModels);
            this.unread = ko.computed(() => _.size(_.filter(messageViewModels, msg => !msg.read())));

			this.selectAll.subscribe(selected => {
				_.forEach(this.messages(), m => m.checked(selected));
			});

        }

        private deleteMessage = (message: UserMessageViewModel) => {

			this.userRepo.deleteMessage(message.id);
            this.messages.remove(message);

        };

		public deleteSelected = () => {

			var selected = _.chain(this.messages()).filter(m => m.checked());
			var selectedIds = selected.map(m => m.id).value();
			this.userRepo.deleteMessages(this.userId, selectedIds);
            this.messages.removeAll(selected.value());

		}

        messages = ko.observableArray<UserMessageViewModel>([]);

		selectAll = ko.observable(false);

        selectMessage = (message: UserMessageViewModel) => {

            _.each(this.messages(), msg => {
                if (msg != message)
                    msg.selected(false);
            });

        };

        unread: KnockoutComputed<number>;

    }

	export class UserMessageViewModel {

		constructor(data: dc.UserMessageSummaryContract) {
            this.created = data.createdFormatted;
            this.highPriority = data.highPriority;
            this.id = data.id;
            this.read = ko.observable(data.read);
            this.receiver = data.receiver;
			this.sender = data.sender;
			this.subject = data.subject;
		}

		checked = ko.observable(false);

		created: string;

        highPriority: boolean;

		id: number;

        read: KnockoutObservable<boolean>;

        receiver: vdb.dataContracts.UserWithIconContract;

		selected = ko.observable(false);

		sender: vdb.dataContracts.UserWithIconContract;

		subject: string;

	}

	export class NewMessageViewModel {

		body = ko.observable<string>();

	}

}