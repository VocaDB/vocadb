import { CommentContract } from '@/DataContracts/CommentContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { EntryType } from '@/Models/EntryType';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { HeaderNames, HttpClient, MediaTypes } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';

export class EntryCommentRepository implements ICommentRepository {
	private baseUrl: string;

	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
		resourcePath: string,
		private readonly entryType?: EntryType,
	) {
		this.baseUrl = UrlMapper.mergeUrls('/api/', resourcePath);
	}

	createComment = ({
		entryId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(this.baseUrl, entryId.toString(), '/comments'),
		);
		return this.httpClient.post<CommentContract>(url, contract);
	};

	deleteComment = ({ commentId }: { commentId: number }): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(this.baseUrl, '/comments/', commentId.toString()),
		);
		return this.httpClient.delete<void>(url);
	};

	getComments = async ({
		entryId: listId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(this.baseUrl, listId.toString(), '/comments/'),
		);
		const result = await this.httpClient.get<
			PartialFindResultContract<CommentContract>
		>(url);
		return result.items;
	};

	updateComment = ({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(this.baseUrl, '/comments/', commentId.toString()),
		);
		return this.httpClient.post<void>(url, contract);
	};

	setCommentsLocked = ({
		entryId,
		locked,
	}: {
		entryId: number;
		locked: boolean;
	}): Promise<void> => {
		var url = this.urlMapper.mapRelative(
			UrlMapper.buildUrl(
				`/api/comments/${this.entryType}-comments`,
				entryId.toString(),
				'locked',
			),
		);
		return this.httpClient.post<void>(url, locked, {
			headers: { [HeaderNames.ContentType]: MediaTypes.APPLICATION_JSON },
		});
	};
}
