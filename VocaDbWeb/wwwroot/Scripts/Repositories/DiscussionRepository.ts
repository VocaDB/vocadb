import CommentContract from '../DataContracts/CommentContract';
import DiscussionFolderContract from '../DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '../DataContracts/Discussion/DiscussionTopicContract';
import ICommentRepository from './ICommentRepository';
import PagingProperties from '../DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import UrlMapper from '../Shared/UrlMapper';
import HttpClient from '../Shared/HttpClient';

export default class DiscussionRepository implements ICommentRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
  ) {}

  private mapUrl = (relative: string) => {
    return this.urlMapper.mapRelative(
      UrlMapper.mergeUrls('/api/discussions', relative),
    );
  };

  public createComment = (
    topicId: number,
    contract: CommentContract,
    callback: (contract: CommentContract) => void,
  ) => {
    $.postJSON(
      this.mapUrl(`topics/${topicId}/comments`),
      contract,
      callback,
      'json',
    );
  };

  public createTopic = (
    folderId: number,
    contract: DiscussionTopicContract,
    callback: (contract: DiscussionTopicContract) => void,
  ) => {
    $.postJSON(
      this.mapUrl(`folders/${folderId}/topics`),
      contract,
      callback,
      'json',
    );
  };

  public deleteComment = (commentId: number, callback?: () => void) => {
    $.ajax(this.mapUrl(`comments/${commentId}`), {
      type: 'DELETE',
      success: callback,
    });
  };

  public deleteTopic = (topicId: number, callback?: () => void) => {
    $.ajax(this.mapUrl(`topics/${topicId}`), {
      type: 'DELETE',
      success: callback,
    });
  };

  public getComments = (topicId: number): Promise<CommentContract[]> => {
    // Not supported
    return Promise.resolve<CommentContract[]>([]);
  };

  public getFolders = (): Promise<DiscussionFolderContract[]> => {
    return this.httpClient.get<DiscussionFolderContract[]>(
      this.mapUrl('folders'),
      {
        fields: 'LastTopic,TopicCount',
      },
    );
  };

  public getTopic = (topicId: number): Promise<DiscussionTopicContract> => {
    return this.httpClient.get<DiscussionTopicContract>(
      this.mapUrl(`topics/${topicId}`),
      { fields: 'All' },
    );
  };

  public getTopics = (): Promise<
    PartialFindResultContract<DiscussionTopicContract>
  > => {
    return this.httpClient.get<
      PartialFindResultContract<DiscussionTopicContract>
    >(this.mapUrl('topics'), { fields: 'CommentCount', maxResults: 5 });
  };

  public getTopicsForFolder = (
    folderId: number,
    paging: PagingProperties,
  ): Promise<PartialFindResultContract<DiscussionTopicContract>> => {
    return this.httpClient.get<
      PartialFindResultContract<DiscussionTopicContract>
    >(this.mapUrl('topics'), {
      folderId: folderId,
      fields: 'CommentCount,LastComment',
      start: paging.start,
      maxResults: paging.maxEntries,
      getTotalCount: paging.getTotalCount,
    });
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
    callback?: () => void,
  ) => {
    $.postJSON(
      this.mapUrl(`comments/${commentId}`),
      contract,
      callback,
      'json',
    );
  };

  public updateTopic = (
    topicId: number,
    contract: DiscussionTopicContract,
    callback?: () => void,
  ) => {
    $.postJSON(this.mapUrl(`topics/${topicId}`), contract, callback, 'json');
  };
}
