import CommentContract from '@DataContracts/CommentContract';
import DiscussionFolderContract from '@DataContracts/Discussion/DiscussionFolderContract';
import DiscussionTopicContract from '@DataContracts/Discussion/DiscussionTopicContract';
import PagingProperties from '@DataContracts/PagingPropertiesContract';
import PartialFindResultContract from '@DataContracts/PartialFindResultContract';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';

import ICommentRepository from './ICommentRepository';

export default class DiscussionRepository implements ICommentRepository {
	constructor(private readonly httpClient: HttpClient) {}

	private mapUrl = (relative: string): string => {
		return UrlMapper.mergeUrls('/api/discussions', relative);
	};

	public createComment = (
		topicId: number,
		contract: CommentContract,
	): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.mapUrl(`topics/${topicId}/comments`),
			contract,
		);
	};

	public createTopic = (
		folderId: number,
		contract: DiscussionTopicContract,
	): Promise<DiscussionTopicContract> => {
		return this.httpClient.post<DiscussionTopicContract>(
			this.mapUrl(`folders/${folderId}/topics`),
			contract,
		);
	};

	public deleteComment = (commentId: number): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`comments/${commentId}`));
	};

	public deleteTopic = (topicId: number): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`topics/${topicId}`));
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
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`comments/${commentId}`),
			contract,
		);
	};

	public updateTopic = (
		topicId: number,
		contract: DiscussionTopicContract,
	): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`topics/${topicId}`),
			contract,
		);
	};
}
