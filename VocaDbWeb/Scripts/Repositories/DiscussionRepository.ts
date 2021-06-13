import CommentContract from '@DataContracts/CommentContract';
import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import ICommentRepository from './ICommentRepository';
import RepositoryParams from './RepositoryParams';

export default class DiscussionRepository implements ICommentRepository {
	public constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	private mapUrl = (relative: string): string => {
		return this.urlMapper.mapRelative(
			UrlMapper.mergeUrls('/api/discussions', relative),
		);
	};

	public createComment = ({
		baseUrl,
		entryId: topicId,
		contract,
	}: RepositoryParams & {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.mapUrl(`topics/${topicId}/comments`),
			contract,
		);
	};

	public createTopic = ({
		baseUrl,
		folderId,
		contract,
	}: RepositoryParams & {
		folderId: number;
		contract: DiscussionTopicContract;
	}): Promise<DiscussionTopicContract> => {
		return this.httpClient.post<DiscussionTopicContract>(
			this.mapUrl(`folders/${folderId}/topics`),
			contract,
		);
	};

	public deleteComment = ({
		baseUrl,
		commentId,
	}: RepositoryParams & { commentId: number }): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`comments/${commentId}`));
	};

	public deleteTopic = ({
		baseUrl,
		topicId,
	}: RepositoryParams & {
		topicId: number;
	}): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`topics/${topicId}`));
	};

	public getComments = ({
		baseUrl,
		entryId: topicId,
	}: RepositoryParams & { entryId: number }): Promise<CommentContract[]> => {
		// Not supported
		return Promise.resolve<CommentContract[]>([]);
	};

	public getFolders = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<DiscussionFolderContract[]> => {
		return this.httpClient.get<DiscussionFolderContract[]>(
			this.mapUrl('folders'),
			{
				fields: 'LastTopic,TopicCount',
			},
		);
	};

	public getTopic = ({
		baseUrl,
		topicId,
	}: RepositoryParams & {
		topicId: number;
	}): Promise<DiscussionTopicContract> => {
		return this.httpClient.get<DiscussionTopicContract>(
			this.mapUrl(`topics/${topicId}`),
			{ fields: 'All' },
		);
	};

	public getTopics = ({
		baseUrl,
	}: RepositoryParams & {}): Promise<
		PartialFindResultContract<DiscussionTopicContract>
	> => {
		return this.httpClient.get<
			PartialFindResultContract<DiscussionTopicContract>
		>(this.mapUrl('topics'), { fields: 'CommentCount', maxResults: 5 });
	};

	public getTopicsForFolder = ({
		baseUrl,
		folderId,
		paging,
	}: RepositoryParams & {
		folderId: number;
		paging: PagingProperties;
	}): Promise<PartialFindResultContract<DiscussionTopicContract>> => {
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

	public updateComment = ({
		baseUrl,
		commentId,
		contract,
	}: RepositoryParams & {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`comments/${commentId}`),
			contract,
		);
	};

	public updateTopic = ({
		baseUrl,
		topicId,
		contract,
	}: RepositoryParams & {
		topicId: number;
		contract: DiscussionTopicContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`topics/${topicId}`),
			contract,
		);
	};
}
