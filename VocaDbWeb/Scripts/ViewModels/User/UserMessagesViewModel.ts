
module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

	export class UserMessagesViewModel {

        constructor(private userRepository: vdb.repositories.UserRepository, userId: number, inboxType: rep.UserInboxType, selectedMessageId?: number) {

            this.notifications = new UserMessageFolderViewModel(userRepository, rep.UserInboxType.Notifications, userId, inboxType !== rep.UserInboxType.Notifications);
            this.receivedMessages = new UserMessageFolderViewModel(userRepository, rep.UserInboxType.Received, userId, inboxType !== rep.UserInboxType.Received);
            this.sentMessages = new UserMessageFolderViewModel(userRepository, rep.UserInboxType.Sent, userId, false);

			this.inboxes = [this.receivedMessages, this.notifications, this.sentMessages];

			var inbox = _.find(this.inboxes, i => i.inbox === inboxType);

			inbox.init(() => {
				if (selectedMessageId != null) {
					this.selectMessageById(selectedMessageId, inbox);
				}
			});

        }

		private getInboxTabName = (inbox: rep.UserInboxType) => {
			
			switch (inbox) {
				case rep.UserInboxType.Received:
					return "#receivedTab";
				case rep.UserInboxType.Notifications:
					return "#notificationsTab";
				case rep.UserInboxType.Sent:
					return "#sentTab";
			}

			return null;

		}

		private inboxes: UserMessageFolderViewModel[];

		newMessageViewModel = new NewMessageViewModel();

        notifications: UserMessageFolderViewModel;

        receivedMessages: UserMessageFolderViewModel;

        sentMessages: UserMessageFolderViewModel;

		reply = () => {

			if (!this.selectedMessage())
				throw Error("No message selected");

			var msg = this.selectedMessage();
			$("#receiverName").val(msg.sender.name);

			$("#newMessageSubject").val(msg.subject && msg.subject.indexOf("Re:") === 0 ? msg.subject : "Re: " + msg.subject);

            this.selectTab("#composeTab");

		};

        selectedMessage = ko.observable<UserMessageViewModel>();

        selectedMessageBody: KnockoutObservable<string> = ko.observable("");

        selectMessageById = (messageId: number, inbox: UserMessageFolderViewModel) => {

            var message = _.find(inbox.items(), msg => msg.id === messageId);

            if (message) {
                this.selectInbox(inbox.inbox);
                this.selectMessage(message);
            }

        };

		selectMessage = (message: UserMessageViewModel) => {

            this.userRepository.getMessage(message.id, message => {
                this.selectedMessageBody(message.body);
            });

            this.receivedMessages.selectMessage(message);
            this.sentMessages.selectMessage(message);
            this.notifications.selectMessage(message);

            message.selected(true);
            message.read(true);
			this.selectedMessage(message);

        };

		private selectInbox = (inbox: rep.UserInboxType) => {
			this.selectTab(this.getInboxTabName(inbox));
		}

        selectTab = (tabName: string) => {
            var index = $('#tabs > ul > li > a').index($(tabName));
            $("#tabs").tabs("option", "active", index);
        };

    }

    export class UserMessageFolderViewModel extends PagedItemsViewModel<UserMessageViewModel> {

        constructor(private userRepo: rep.UserRepository, public inbox: rep.UserInboxType, private userId,
			getMessageCount: boolean) {

			super();
            
            this.unread = ko.computed(() => {
				return this.items().length ? _.size(_.filter(this.items(), msg => !msg.read())) : this.unreadOnServer();
            });

			if (getMessageCount) {
				this.userRepo.getMessageSummaries(userId, inbox, { start: 0, maxEntries: 0, getTotalCount: true }, true, null,
					result => this.unreadOnServer(result.totalCount));
			}

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

		private unreadOnServer = ko.observable(null);

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