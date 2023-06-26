import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import useStyles from './Folder.styles';
import { IconBook2 } from '@tabler/icons-react';
import { Text, Paper, Grid, ThemeIcon } from '@mantine/core';

interface FolderProps {
	folder: DiscussionFolderContract;
}

export function Folder({ folder }: FolderProps) {
	const styles = useStyles();
	return (
		<>
		 	<Text mt="md" mb="xs" className={styles.classes.folderName}>
				<ThemeIcon variant="filled" mr={6}>
					<IconBook2 size="1rem" />
				</ThemeIcon>
				{folder.name}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderDescription}>
				{folder.description}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderTopics}>
				Topics: {folder.topicCount}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderLastTopicDate}>
				Last topic: {new Date(folder.lastTopicDate).toLocaleString()} by{' '}
				{folder.lastTopicAuthor?.name}
			</Text>
		</>
	);
}

interface FoldersProps {
	folders: DiscussionFolderContract[];
}

export function Folders({ folders }: FoldersProps) {
	const styles = useStyles();
	return (
		<Grid className={styles.classes.folderGrid} grow gutter="xl">
			{folders.map((folder) => (
				<Grid.Col span={6} key={folder.id}>
					<Paper shadow="sm" radius="md" p="md" withBorder key={folder.id} className={styles.classes.folderGridList}>
						<Folder key={folder.id} folder={folder} />
					</Paper>
				</Grid.Col>
			))}
		</Grid>
	);
}

