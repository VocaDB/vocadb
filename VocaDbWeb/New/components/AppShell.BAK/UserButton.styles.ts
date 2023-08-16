import { createStyles } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	user: {
		display: 'block',
		width: '100%',
		padding: theme.spacing.md,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},
}));

