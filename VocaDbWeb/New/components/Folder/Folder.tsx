import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import useStyles from './Folder.styles';
import { IconBook2 } from '@tabler/icons-react';
import { Text, Paper, Grid, ThemeIcon } from '@mantine/core';
import Link from 'next/link';

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
			<Text mt="md" mb="xs" className={styles.classes.folderItem}>
				Topics: {folder.topicCount}
			</Text>
			<Text mt="md" mb="xs" className={styles.classes.folderItem}>
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
				<Grid.Col span={6} key={folder.id} style={{ maxWidth: '100%' }}>
					<Paper
						component={Link}
						href={`/discussion/folders/${folder.id}`}
						shadow="sm"
						radius="md"
						p="md"
						withBorder
						className={styles.classes.folderGridList}
					>
						<Folder folder={folder} />
					</Paper>
				</Grid.Col>
			))}
		</Grid>
	);
}

