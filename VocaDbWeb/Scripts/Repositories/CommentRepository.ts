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
		private entryType: EntryType,
	) {
		super();
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
		return this.httpClient.post<CommentContract>(
			UrlMapper.buildUrl(`api/comments/${EntryType[this.entryType]}-comments`),
			contract,
		);
	};

	public deleteComment = (commentId: number): Promise<void> => {
		return this.httpClient.delete<void>(
			UrlMapper.buildUrl(
				`api/comments/${EntryType[this.entryType]}-comments/`,
				commentId.toString(),
			),
		);
	};

	public getComments = async (listId: number): Promise<CommentContract[]> => {
		const result = await this.httpClient.get<
			PartialFindResultContract<CommentContract>
		>(
			UrlMapper.buildUrl(`api/comments/${EntryType[this.entryType]}-comments/`),
			{ entryId: listId },
		);
		return result.items;
	};

	public updateComment = (
		commentId: number,
		contract: CommentContract,
	): Promise<void> => {
		return this.httpClient.post<void>(
			UrlMapper.buildUrl(
				`api/comments/${EntryType[this.entryType]}-comments/`,
				commentId.toString(),
			),
			contract,
		);
	};
}
