import { DiscussionTopicContract } from '@/types/DataContracts/Discussion/DiscussionTopicContract';
import { Grid, Title } from '@mantine/core';
import useStyles from './RecentTopic.styles';

interface RecentTopicProps {
	topic: DiscussionTopicContract;
}

export function RecentTopic({ topic }: RecentTopicProps) {
	const styles = useStyles();
	return (
        <>
            <Title mt="md" mb="xs" order={5} className={styles.classes.topicName}>
                {topic.name}
            </Title>
            <Title mt="md" mb="xs" order={6} className={styles.classes.topicDate}>
                {new Date(topic.created).toLocaleString()} by {topic.author?.name}
            </Title>
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
				<Grid.Col
                    span={3}
                    key={topic.id} className={styles.classes.folderGridList}>
					<RecentTopic key={topic.id} topic={topic} />
				</Grid.Col>
			))}
		</Grid>
	);
}

