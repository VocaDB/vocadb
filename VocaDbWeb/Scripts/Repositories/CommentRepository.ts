import CommentContract from '@DataContracts/CommentContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import EntryType from '@Models/EntryType';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import BaseRepository from './BaseRepository';
import ICommentRepository from './ICommentRepository';

export default class CommentRepository
	extends BaseRepository
	implements ICommentRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
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
			name: null!,
		};
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(`api/comments/${EntryType[this.entryType]}-comments`),
		);
		return this.httpClient.post<CommentContract>(url, contract);
	};

	public deleteComment = (commentId: number): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(
				`api/comments/${EntryType[this.entryType]}-comments/`,
				commentId.toString(),
			),
		);
		return this.httpClient.delete<void>(url);
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
