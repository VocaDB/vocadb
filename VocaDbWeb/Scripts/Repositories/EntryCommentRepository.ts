import CommentContract from '@DataContracts/CommentContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import ICommentRepository from './ICommentRepository';

export default class EntryCommentRepository implements ICommentRepository {
  private baseUrl: string;

  constructor(private readonly httpClient: HttpClient, resourcePath: string) {
    this.baseUrl = UrlMapper.mergeUrls('/api/', resourcePath);
  }

  public createComment = (
    entryId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    return this.httpClient.post<CommentContract>(
      UrlMapper.buildUrl(this.baseUrl, entryId.toString(), '/comments'),
      contract,
    );
  };

  public deleteComment = (commentId: number): Promise<void> => {
    return this.httpClient.delete<void>(
      UrlMapper.buildUrl(this.baseUrl, '/comments/', commentId.toString()),
    );
  };

  public getComments = async (listId: number): Promise<CommentContract[]> => {
    const result = await this.httpClient.get<
      PartialFindResultContract<CommentContract>
    >(UrlMapper.buildUrl(this.baseUrl, listId.toString(), '/comments/'));
    return result.items;
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    return this.httpClient.post<void>(
      UrlMapper.buildUrl(this.baseUrl, '/comments/', commentId.toString()),
      contract,
    );
  };
}
