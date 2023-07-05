import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import useStyles from './[id].styles';
import { IconBook2 } from '@tabler/icons-react';
import { Text, Anchor, ThemeIcon, Pagination, Table } from '@mantine/core';

export default function FolderPage({
	folder,
	start,
	page,
	totalPage,
	topics,
}: InferGetServerSidePropsType<typeof getServerSideProps>) {
	const styles = useStyles();
	const setPage = (page: number) => {
		window.location.href = `/discussion/folders/${folder.id}?page=${page}`;
	};
	return (
		<>
			<Anchor href="/discussion" className={styles.classes.folderBackButton}>
				<Text size="sm" variant="link">
					&lt; Back to discussion index page
				</Text>
			</Anchor>
			<Text mt="md" mb="xs" className={styles.classes.folderHeaderName}>
				<ThemeIcon variant="filled" mr={6}>
					<IconBook2 size="1rem" />
				</ThemeIcon>
				{folder.name}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderHeaderDescription}>
				{folder.description}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderHeaderItem}>
				{start + 1}-{start + topics.length} of {folder.topicCount} topics
			</Text>
			<Pagination
				value={page}
				total={totalPage}
				mb={20}
				onChange={setPage}
				className={styles.classes.folderPagination}
			/>
			<Table
				withBorder
				withColumnBorders
				className={styles.classes.folderTable}
			>
				<thead>
					<tr>
						<th>Topic</th>
						<th>Author</th>
						<th>Comments</th>
						<th>Last Post</th>
						<th>Last Comment</th>
						<th>Last Update</th>
					</tr>
				</thead>
				<tbody>
					{topics.map((topic) => (
						<tr key={topic.id}>
							<td>
								<Anchor href={`/discussion/topics/${topic.id}`}>
									{topic.name}
								</Anchor>
							</td>
							<td>
								<Anchor href={`/Profile/${topic.author.name}`}>
									<img
										src={
											topic.author.mainPicture?.urlTinyThumb ?? '/unknown.png'
										}
									/>
									<br />
									{topic.author?.name}
								</Anchor>
							</td>
							<td>{topic.commentCount}</td>
							<td>{new Date(topic.created).toLocaleString()}</td>
							<td>
								{topic.comments.length > 0
									? new Date(
											topic.comments[topic.comments.length - 1]?.created
									  ).toLocaleString()
									: ''}
							</td>
							<td>
								{topic.comments.length > 0
									? new Date(
											topic.comments[topic.comments.length - 1]?.created
									  ).toLocaleString()
									: new Date(topic.created).toLocaleString()}
							</td>
						</tr>
					))}
				</tbody>
			</Table>
			<Pagination
				value={page}
				total={totalPage}
				mt={20}
				onChange={setPage}
				className={styles.classes.folderPagination}
			/>
		</>
	);
}

export const getServerSideProps: GetServerSideProps<{
	folder: DiscussionFolderContract;
	start: number;
	page: number;
	totalPage: number;
	topics: DiscussionTopicContract[];
}> = async ({ params, query, res }) => {
	let id: number;

	if (params?.id) {
		id = parseInt(params.id as string);
	} else {
		return { notFound: true };
	}

	const page = query.page ? parseInt(query.page as string) : 1;
	const perPage = 30;

	const start = (page - 1) * perPage;

	res.setHeader('Cache-Control', 'public, s-maxage=30, stale-while-revalidate=300');

	const apiResp = await Promise.all([
		apiFetch('/api/discussions/folders?fields=TopicCount,LastTopic'),
		apiFetch(
			`/api/discussions/topics?folderId=${id}&start=${start}&maxResults=${perPage}&fields=All`
		),
	]);

	const [folders, topics] = await Promise.all([
		apiResp[0].json(),
		apiResp[1].json().then((topics) => topics.items),
	]);

	const folder = folders.find((folder: any) => folder.id === id);
	if (!folder) {
		return { notFound: true };
	}

	const totalPage = Math.ceil(parseFloat(folder.topicCount) / perPage);

	return { props: { folder, start, page, totalPage, topics } };
};
