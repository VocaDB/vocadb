import { GetServerSideProps, InferGetServerSidePropsType } from 'next';
import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { apiFetch } from '@/Helpers/FetchApiHelper';
import useStyles from './[id].styles';
import { IconBook2 } from '@tabler/icons-react';
import { Text, Box, ThemeIcon, Pagination, Table, useMantineTheme } from '@mantine/core';
import Link from 'next/link';
import CustomImage from '@/components/Image/Image';

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
	const theme = useMantineTheme();
	return (
		<>
			<Text size="sm" component={Link} href="/discussion" variant="link" color={theme.primaryColor}>
				&lt; Back to discussion index page
			</Text>
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
			<Table withBorder withColumnBorders className={styles.classes.folderTable}>
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
								<Text component={Link} href={`/discussion/topics/${topic.id}`} color={theme.primaryColor}>
									{topic.name}
								</Text>
							</td>
							<td>
								<Box component={Link} href={`/Profile/${topic.author.name}`} style={{ textDecoration: 'none' }}>
									<CustomImage
										src={topic.author.mainPicture?.urlTinyThumb ?? ''}
										height={50}
										width={50}
										alt={topic.author?.name ?? 'Unknown'}
									/>
									<br />
									<Text size="sm" color={theme.primaryColor}>
										{topic.author?.name}
									</Text>
								</Box>
							</td>
							<td>{topic.commentCount}</td>
							<td>{new Date(topic.created).toLocaleString()}</td>
							<td>
								{topic.comments.length > 0
									? new Date(topic.comments.at(-1)?.created!).toLocaleString()
									: ''}
							</td>
							<td>
								{topic.comments.length > 0
									? new Date(topic.comments.at(-1)?.created!).toLocaleString()
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

	const [folders, topics]: [DiscussionFolderContract[], DiscussionTopicContract[]] =
		await Promise.all([apiResp[0].json(), apiResp[1].json().then((topics) => topics.items)]);

	const folder = folders.find((folder) => folder.id === id);
	if (!folder) {
		return { notFound: true };
	}

	const totalPage = Math.ceil(folder.topicCount / perPage);

	return { props: { folder, start, page, totalPage, topics } };
};

