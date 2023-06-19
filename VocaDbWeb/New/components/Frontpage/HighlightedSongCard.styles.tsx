import { createStyles } from '@mantine/core';

export default createStyles((theme) => ({
	card: {
		height: '100%',
		display: 'flex',
		flexDirection: 'column',
	},
	image: {
		width: '100%',
		objectFit: 'cover',
	},
	contentStack: {
		justifyContent: 'space-between',
		flex: 2,
	},
}));
