import { createStyles, rem } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	navbarSection: {
		borderTop: `${rem(1)} solid ${
			theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]
		}`,
	},

	mainLinkButton: {
		display: 'block',
		width: '100%',
		padding: theme.spacing.xs,
		fontSize: theme.fontSizes.sm,
		borderRadius: theme.radius.sm,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},
}));

