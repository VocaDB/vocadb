import { createStyles } from '@mantine/core';

export default createStyles((theme) => ({
	folderGrid: {
		display: 'flex',
		flexDirection: 'row',
		flexWrap: 'wrap',
		justifyContent: 'space-around',
		overflow: 'hidden',
		backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[8] : theme.colors.gray[0],
	},
	folderGridList: {
		width: '100%',
		height: '100%',
		backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[6] : 'white',
		'&:hover': {
			backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[5] : theme.colors.gray[2],
		},
		cursor: 'pointer',
	},
	folderName: {
		width: '100%',
		height: '40px',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		textDecoration: 'underline',
		fontWeight: 'bold',
	},
	folderDescription: {
		width: '100%',
		textOverflow: 'initial',
		whiteSpace: 'normal',
		color: theme.colorScheme === 'dark' ? theme.colors.dark[1] : theme.colors.dark[3],
	},
	folderTopics: {
		width: '100%',
		height: '20px',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		color: theme.colorScheme === 'dark' ? theme.colors.dark[1] : theme.colors.dark[3],
	},
	folderLastTopicDate: {
		width: '100%',
		height: '20px',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		color: theme.colorScheme === 'dark' ? theme.colors.dark[1] : theme.colors.dark[3],
	},
}));

