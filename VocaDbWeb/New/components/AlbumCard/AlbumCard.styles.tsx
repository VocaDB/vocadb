import { createStyles } from '@mantine/core';

export default createStyles((theme) => ({
	cardContent: {
		pointerEvents: 'none',
		position: 'absolute',
		display: 'flex',
		flexDirection: 'column',
		justifyContent: 'flex-end',
		top: 0,
		width: '250px',
		height: '250px',
		background: 'linear-gradient(to top, rgba(0, 0, 0, 0.70), transparent)',
	},
	image: {
		transition: 'all 0.5s ease',
		display: 'block',
		marginLeft: 'auto',
		marginRight: 'auto',
		':hover': {
			transform: 'scale(1.1)',
		},
	},
}));

