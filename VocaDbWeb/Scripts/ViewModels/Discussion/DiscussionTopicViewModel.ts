import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import UserApiContract from '@DataContracts/User/UserApiContract';
import DiscussionRepository from '@Repositories/DiscussionRepository';
import ko, { Observable } from 'knockout';

import EditableCommentsViewModel from '../EditableCommentsViewModel';

export default class DiscussionTopicViewModel {
  constructor(
    private repo: DiscussionRepository,
    private loggedUserId: number,
    canDeleteAllComments: boolean,
    contract: DiscussionTopicContract,
    private folders: DiscussionFolderContract[],
  ) {
    this.contract = ko.observable(contract);

    this.comments = new EditableCommentsViewModel(
      repo,
      contract.id,
      loggedUserId,
      canDeleteAllComments,
      canDeleteAllComments,
      true,
      contract.comments,
    );
  }

  public beginEditTopic = (): void => {
    this.editModel(
      new DiscussionTopicEditViewModel(
        this.loggedUserId,
        this.folders,
        this.contract(),
      ),
    );
  };

  public comments: EditableCommentsViewModel;

  public cancelEdit = (): void => {
    this.editModel(null!);
  };

  public contract: Observable<DiscussionTopicContract>;

  public editModel = ko.observable<DiscussionTopicEditViewModel>(null!);

  public isBeingEdited = ko.computed(() => this.editModel() !== null);

  public saveEditedTopic = (): void => {
    if (!this.isBeingEdited()) return;

    var editedContract = this.editModel()!.toContract();

    this.repo.updateTopic(this.contract().id, editedContract).then(() => {
      editedContract.id = this.contract().id;
      editedContract.created = this.contract().created;
      editedContract.canBeDeleted = this.contract().canBeDeleted;
      editedContract.canBeEdited = this.contract().canBeEdited;

      this.contract(editedContract);
      this.editModel(null!);
    });
  };
}

export class DiscussionTopicEditViewModel {
  constructor(
    userId: number,
    public folders: DiscussionFolderContract[],
    contract?: DiscussionTopicContract,
  ) {
    this.author = { id: userId, name: '' };

    if (contract) {
      this.author = contract.author;
      this.content(contract.content);
      this.folderId(contract.folderId);
      this.locked(contract.locked);
      this.name(contract.name);
    }
  }

  public author: UserApiContract;

  public content = ko.observable('');

  public folderId = ko.observable<number>(null!);

  public locked = ko.observable(false);

  public name = ko.observable('');

  public toContract = (): DiscussionTopicContract => {
    return ko.toJS(this) as DiscussionTopicContract;
  };
}
