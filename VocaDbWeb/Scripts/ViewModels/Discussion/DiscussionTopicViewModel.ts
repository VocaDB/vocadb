
module vdb.viewModels.discussions {
	
	import dc = vdb.dataContracts;

	export class DiscussionTopicViewModel {

		constructor(private repo: rep.DiscussionRepository, private loggedUserId: number,
			canDeleteAllComments: boolean,
			contract: dc.discussions.DiscussionTopicContract) {

			this.contract = ko.observable(contract);

			this.comments = new EditableCommentsViewModel(repo, contract.id, loggedUserId, canDeleteAllComments, canDeleteAllComments, true, contract.comments);

		}

		public beginEditTopic = () => {
			this.editModel(new DiscussionTopicEditViewModel(this.loggedUserId, this.contract()));
		}

		public comments: EditableCommentsViewModel;

		public cancelEdit = () => {
			this.editModel(null);
		}

		public contract: KnockoutObservable<dc.discussions.DiscussionTopicContract>;

		public editModel = ko.observable<DiscussionTopicEditViewModel>(null);

		public isBeingEdited = ko.computed(() => this.editModel() !== null);

		public saveEditedTopic = () => {

			if (!this.isBeingEdited())
				return;

			var editedContract = this.editModel().toContract();

			this.repo.updateTopic(this.contract().id, editedContract,() => {

				editedContract.id = this.contract().id;
				editedContract.created = this.contract().created;
				editedContract.canBeDeleted = this.contract().canBeDeleted;
				editedContract.canBeEdited = this.contract().canBeEdited;

				this.contract(editedContract);
				this.editModel(null);

			});

		}

	}

	export class DiscussionTopicEditViewModel {

		constructor(userId: number, contract?: dc.discussions.DiscussionTopicContract) {

			this.author = { id: userId, name: '' };

			if (contract) {
				this.author = contract.author;
				this.content(contract.content);
				this.name(contract.name);
			}

		}

		public author: dc.UserWithIconContract;

		public content = ko.observable("");

		public name = ko.observable("");

		public toContract = (): dc.discussions.DiscussionTopicContract => {
			return ko.toJS(this);
		}

	}

} 