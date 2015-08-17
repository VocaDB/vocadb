
module vdb.viewModels {

	import dc = vdb.dataContracts;

	export class UserMessagesViewModel {

        constructor(private userRepository: vdb.repositories.UserRepository, userId: number, selectedMessageId?: number) {

            this.notifications = new UserMessageFolderViewModel(userRepository, "Notifications", userId);
            this.receivedMessages = new UserMessageFolderViewModel(userRepository, "Received", userId);
            this.sentMessages = new UserMessageFolderViewModel(userRepository, "Sent", userId);

			this.receivedMessages.init(() => {
				if (selectedMessageId != null) {
					this.selectMessageById(selectedMessageId);
				}
			});

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

            var message = _.find(this.notifications.items(), msg => msg.id === messageId);

            if (message) {
                this.selectTab("#notificationsTab");
                this.selectMessage(message);
                return;
            }

            message = _.find(this.receivedMessages.items(), msg => msg.id === messageId);

            if (message) {
                this.selectTab("#receivedTab");
                this.selectMessage(message);
                return;
            }

            message = _.find(this.sentMessages.items(), msg => msg.id === messageId);

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

    export class UserMessageFolderViewModel extends PagedItemsViewModel<UserMessageViewModel> {

        constructor(private userRepo: rep.UserRepository, private inbox: string, private userId) {

			super();
            
            this.unread = ko.computed(() => _.size(_.filter(this.items(), msg => !msg.read())));

			this.selectAll.subscribe(selected => {
				_.forEach(this.items(), m => m.checked(selected));
			});

        }

        private deleteMessage = (message: UserMessageViewModel) => {

			this.userRepo.deleteMessage(message.id);
            this.items.remove(message);

        };

		public deleteSelected = () => {

			var selected = _.chain(this.items()).filter(m => m.checked());
			var selectedIds = selected.map(m => m.id).value();

			if (selectedIds.length === 0)
				return;

			this.userRepo.deleteMessages(this.userId, selectedIds);
            this.items.removeAll(selected.value());

		}

		public loadMoreItems = (callback) => {
			this.userRepo.getMessageSummaries(this.userId, this.inbox, { start: this.start, maxEntries: 100, getTotalCount: true }, false, 40, (result) => {
				var messageViewModels = _.map(result.items, msg => new UserMessageViewModel(msg));
				callback({ items: messageViewModels, totalCount: result.totalCount });
			});
		}

		selectAll = ko.observable(false);

        selectMessage = (message: UserMessageViewModel) => {

            _.each(this.items(), msg => {
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