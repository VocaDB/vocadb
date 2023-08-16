import { createStyles } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	header: {
		display: 'flex',
		justifyContent: 'space-between',
	},

	image: {
		objectFit: 'contain',
		height: '100%',
	},

	rightSectionWrapper: {
		display: 'flex',
		alignItems: 'center',
		height: '100%',
	},

	burger: {
		[theme.fn.largerThan('sm')]: {
			display: 'none',
		},
	},
}));

