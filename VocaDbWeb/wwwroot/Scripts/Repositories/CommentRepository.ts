import BaseRepository from './BaseRepository';
import CommentContract from '../DataContracts/CommentContract';
import EntryType from '../Models/EntryType';
import ICommentRepository from './ICommentRepository';
import UrlMapper from '../Shared/UrlMapper';
import PartialFindResultContract from '../DataContracts/PartialFindResultContract';
import HttpClient from '../Shared/HttpClient';

export default class CommentRepository
  extends BaseRepository
  implements ICommentRepository {
  constructor(
    private readonly httpClient: HttpClient,
    private urlMapper: UrlMapper,
    private entryType: EntryType,
  ) {
    super(urlMapper.baseUrl);
  }

  public createComment = (
    entryId: number,
    contract: CommentContract,
  ): Promise<CommentContract> => {
    contract.entry = {
      entryType: EntryType[this.entryType],
      id: entryId,
      name: null,
    };
    var url = this.urlMapper.mapRelative(
      UrlMapper.buildUrl(`api/comments/${EntryType[this.entryType]}-comments`),
    );
    return this.httpClient.post<CommentContract>(url, contract);
  };

  public deleteComment = (commentId: number, callback?: () => void) => {
    var url = this.urlMapper.mapRelative(
      UrlMapper.buildUrl(
        `api/comments/${EntryType[this.entryType]}-comments/`,
        commentId.toString(),
      ),
    );
    $.ajax(url, { type: 'DELETE', success: callback });
  };

  public getComments = async (listId: number): Promise<CommentContract[]> => {
    var url = this.urlMapper.mapRelative(
      UrlMapper.buildUrl(`api/comments/${EntryType[this.entryType]}-comments/`),
    );
    const result = await this.httpClient.get<
      PartialFindResultContract<CommentContract>
    >(url, { entryId: listId });
    return result.items;
  };

  public updateComment = (
    commentId: number,
    contract: CommentContract,
  ): Promise<void> => {
    var url = this.urlMapper.mapRelative(
      UrlMapper.buildUrl(
        `api/comments/${EntryType[this.entryType]}-comments/`,
        commentId.toString(),
      ),
    );
    return this.httpClient.post<void>(url, contract);
  };
}
