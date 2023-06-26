import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { Text, Paper, Grid } from '@mantine/core';
import useStyles from './RecentTopic.styles';

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
				<Grid.Col span={3} key={topic.id}>
					<Paper shadow="sm" radius="md" p="md" withBorder key={topic.id} className={styles.classes.folderGridList}>
						<RecentTopic key={topic.id} topic={topic} />
					</Paper>
				</Grid.Col>
			))}
		</Grid>
	);
}

