import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import { Title } from '@mantine/core';
import { Folders } from '@/components/Folder/Folder';
import { RecentTopics } from '@/components/RecentTopic/RecentTopic';

export default function DiscussionPage({
	discussionFolders,
	recentTopics,
}: InferGetServerSidePropsType<typeof getServerSideProps>) {
	return (
		<>
			<Title order={2} mb="xs" mt="md">
				Discussions
			</Title>
			<Folders folders={discussionFolders} />
			<Title mt="md" mb="xs" order={2}>
				Recent Topics
			</Title>
			<RecentTopics topics={recentTopics} />
		</>
	);
}

export const getServerSideProps: GetServerSideProps<{
	discussionFolders: DiscussionFolderContract[];
	recentTopics: DiscussionTopicContract[];
}> = async ({ res }) => {
	res.setHeader('Cache-Control', 'public, s-maxage=30, stale-while-revalidate=300');

	const apiResp = await Promise.all([
		apiFetch('/api/discussions/folders?fields=TopicCount,LastTopic'),
		apiFetch('/api/discussions/topics?sort=DateCreated'),
	]);

	const [discussionFolders, recentTopics] = await Promise.all([
		apiResp[0].json(),
		apiResp[1].json().then((topics) => topics.items),
	]);

	recentTopics.splice(6);

	return { props: { discussionFolders, recentTopics } };
};

