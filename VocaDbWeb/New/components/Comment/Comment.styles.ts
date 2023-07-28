import { createStyles, rem } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	comment: {
		padding: `${theme.spacing.lg} ${theme.spacing.xl}`,
	},

	body: {
		paddingLeft: rem(54),
		paddingTop: theme.spacing.sm,
		fontSize: theme.fontSizes.sm,
	},

	content: {
		'& > p:last-child': {
			marginBottom: 0,
		},
	},
}));
