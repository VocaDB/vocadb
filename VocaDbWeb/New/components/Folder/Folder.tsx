import { DiscussionFolderContract } from '@/types/DataContracts/Discussion/DiscussionFolderContract';
import { Grid, Title } from '@mantine/core';
import useStyles from './Folder.styles';
import { ThemeIcon } from '@mantine/core';
import { IconBook2 } from '@tabler/icons-react';

interface FolderProps {
	folder: DiscussionFolderContract;
}

export function Folder({ folder }: FolderProps) {
	const styles = useStyles();
	return (
        <>
            <Title mt="md" mb="xs" order={5} className={styles.classes.folderName}>
                <ThemeIcon variant="filled" style={{ display: "inline-flex", marginRight: "10px" }}><IconBook2 size="1rem" style={{ display: "inline-flex" }} /></ThemeIcon>{folder.name}
            </Title>
            <Title mt="md" mb="xs" order={6} className={styles.classes.folderDescription}>
                {folder.description}
            </Title>
            <Title mt="md" mb="xs" order={6} className={styles.classes.folderTopics}>
                Topics: {folder.topicCount}
            </Title>
            <Title mt="md" mb="xs" order={6} className={styles.classes.folderLastTopicDate}>
                LastTopic: {new Date(folder.lastTopicDate).toLocaleString()} by {folder.lastTopicAuthor?.name}
            </Title>
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
				<Grid.Col
                    span={4}
                    key={folder.id} className={styles.classes.folderGridList}>
					<Folder key={folder.id} folder={folder} />
				</Grid.Col>
			))}
		</Grid>
	);
}

