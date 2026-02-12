import { CommentContract } from '@/DataContracts/CommentContract';
import { DiscussionFolderContract } from '@/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/DataContracts/Discussion/DiscussionTopicContract';
import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { ICommentRepository } from '@/Repositories/ICommentRepository';
import { httpClient, HttpClient } from '@/Shared/HttpClient';
import { urlMapper, UrlMapper } from '@/Shared/UrlMapper';

export class DiscussionRepository implements ICommentRepository {
	constructor(
		private readonly httpClient: HttpClient,
		private readonly urlMapper: UrlMapper,
	) {}

	private mapUrl = (relative: string): string => {
		return this.urlMapper.mapRelative(
			UrlMapper.mergeUrls('/api/discussions', relative),
		);
	};

	createComment = ({
		entryId: topicId,
		contract,
	}: {
		entryId: number;
		contract: CommentContract;
	}): Promise<CommentContract> => {
		return this.httpClient.post<CommentContract>(
			this.mapUrl(`topics/${topicId}/comments`),
			contract,
		);
	};

	createTopic = ({
		folderId,
		contract,
	}: {
		folderId: number;
		contract: DiscussionTopicContract;
	}): Promise<DiscussionTopicContract> => {
		return this.httpClient.post<DiscussionTopicContract>(
			this.mapUrl(`folders/${folderId}/topics`),
			contract,
		);
	};

	deleteComment = ({ commentId }: { commentId: number }): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`comments/${commentId}`));
	};

	deleteTopic = ({ topicId }: { topicId: number }): Promise<void> => {
		return this.httpClient.delete<void>(this.mapUrl(`topics/${topicId}`));
	};

	getComments = ({
		entryId: topicId,
	}: {
		entryId: number;
	}): Promise<CommentContract[]> => {
		// Not supported
		return Promise.resolve<CommentContract[]>([]);
	};

	// eslint-disable-next-line no-empty-pattern
	getFolders = ({}: {}): Promise<DiscussionFolderContract[]> => {
		return this.httpClient.get<DiscussionFolderContract[]>(
			this.mapUrl('folders'),
			{
				fields: 'LastTopic,TopicCount',
			},
		);
	};

	getTopic = ({
		topicId,
	}: {
		topicId: number;
	}): Promise<DiscussionTopicContract> => {
		return this.httpClient.get<DiscussionTopicContract>(
			this.mapUrl(`topics/${topicId}`),
			{ fields: 'All' },
		);
	};

	// eslint-disable-next-line no-empty-pattern
	getTopics = ({}: {}): Promise<
		PartialFindResultContract<DiscussionTopicContract>
	> => {
		return this.httpClient.get<
			PartialFindResultContract<DiscussionTopicContract>
		>(this.mapUrl('topics'), { fields: 'CommentCount', maxResults: 5 });
	};

	getTopicsForFolder = ({
		folderId,
		paging,
	}: {
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

	updateComment = ({
		commentId,
		contract,
	}: {
		commentId: number;
		contract: CommentContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`comments/${commentId}`),
			contract,
		);
	};

	updateTopic = ({
		topicId,
		contract,
	}: {
		topicId: number;
		contract: DiscussionTopicContract;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.mapUrl(`topics/${topicId}`),
			contract,
		);
	};

	setCommentsLocked = ({
		entryId,
		locked,
	}: {
		entryId: number;
		locked: boolean;
	}): Promise<void> => {
		return this.httpClient.post<void>(
			this.urlMapper.mapRelative(
				`/api/comments/DiscussionTopic-comments/${entryId}/locked`,
			),
			locked,
		);
	};
}

export const discussionRepo = new DiscussionRepository(httpClient, urlMapper);
