import { createStyles } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	box: {
		padding: theme.spacing.md,
		position: 'relative',
		height: 'calc(100vh - 70px - 64px)',
		overflowY: 'scroll',
		[theme.fn.smallerThan('sm')]: {
			height: 'calc(100vh - 50px - 64px)',
		},
	},
}));

