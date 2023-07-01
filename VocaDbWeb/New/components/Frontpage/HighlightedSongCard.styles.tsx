import { createStyles } from '@mantine/core';

export default createStyles(() => ({
	card: {
		height: '100%',
		display: 'flex',
		flexDirection: 'column',
	},
	image: {
		height: '100%',
		width: '100%',
		aspectRatio: '16/9',
		objectFit: 'cover',
	},
	contentStack: {
		justifyContent: 'space-between',
		flex: 2,
	},
}));

