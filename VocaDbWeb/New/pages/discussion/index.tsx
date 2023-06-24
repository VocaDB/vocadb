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
				Discussion
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
}
> = async ({ res }) => {
	res.setHeader('Cache-Control', 'public, s-maxage=30, stale-while-revalidate=300');

	let apiResp = await apiFetch('/api/discussions/folders?fields=TopicCount,LastTopic');
	const discussionFolders = await apiResp.json();

	apiResp = await apiFetch('/api/discussions/topics?sort=DateCreated');
	const recentTopics = await apiResp.json().then((topics) => topics.items);
	recentTopics.splice(6);

	return { props: { discussionFolders, recentTopics } };
};

