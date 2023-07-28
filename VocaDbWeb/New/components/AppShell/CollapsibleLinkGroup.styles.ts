import { createStyles, rem } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	control: {
		display: 'block',
		width: '100%',
		padding: `${theme.spacing.xs} `,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.black,
		fontSize: theme.fontSizes.sm,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},

	link: {
		display: 'block',
		padding: `${theme.spacing.xs}`,
		paddingLeft: rem(31),
		marginLeft: rem(30),
		fontSize: theme.fontSizes.sm,
		color: theme.colorScheme === 'dark' ? theme.colors.dark[0] : theme.colors.gray[7],
		borderLeft: `${rem(1)} solid ${
			theme.colorScheme === 'dark' ? theme.colors.dark[4] : theme.colors.gray[3]
		}`,

		'&:hover': {
			backgroundColor:
				theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[0],
		},
	},
	iconWrapper: {
		display: 'flex',
		alignItems: 'center',
	},
}));

