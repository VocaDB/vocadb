import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { Text, Paper, Grid } from '@mantine/core';
import useStyles from './RecentTopic.styles';
import Link from 'next/link';

interface RecentTopicProps {
	topic: DiscussionTopicContract;
}

export function RecentTopic({ topic }: RecentTopicProps) {
	const styles = useStyles();
	return (
		<>
			<Text mt="md" mb="xs" className={styles.classes.topicName}>
				{topic.name}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.topicDate}>
				{new Date(topic.created).toLocaleString()} by {topic.author?.name}
			</Text>
		</>
	);
}

interface RecentTopicsProps {
	topics: DiscussionTopicContract[];
}

export function RecentTopics({ topics }: RecentTopicsProps) {
	const styles = useStyles();
	return (
		<Grid className={styles.classes.folderGrid} grow gutter="xl">
			{topics.map((topic) => (
				<Grid.Col span={3} key={topic.id} style={{ maxWidth: '100%' }}>
					<Paper
						component={Link}
						href={`/discussion/topics/${topic.id}`}
						shadow="sm"
						radius="md"
						p="md"
						withBorder
						className={styles.classes.folderGridList}
					>
						<RecentTopic topic={topic} />
					</Paper>
				</Grid.Col>
			))}
		</Grid>
	);
}

