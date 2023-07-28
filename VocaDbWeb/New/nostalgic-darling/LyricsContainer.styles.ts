import { createStyles } from '@mantine/core';

export const useStyles = createStyles((theme) => ({
	lyricsContainer: {
		display: 'grid',
		justifyContent: 'center',
	},
	lyricsWrapper: {
		maxWidth: 'max-content',
		[theme.fn.smallerThan('sm')]: {
			marginLeft: theme.spacing.sm,
			marginRight: theme.spacing.sm,
		},
	},
	lyricLine: {
		lineHeight: '1.5em',
		fontSize: '2rem',
		fontWeight: 700,
		color: 'white',
		[theme.fn.smallerThan('lg')]: {
			fontSize: '1.5rem',
		},
	},
	divWrapper: {
		position: 'absolute',
		top: 0,
		left: 0,
		height: '100%',
		width: '100%',
		zIndex: 100,
	},
}));

