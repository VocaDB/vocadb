import { createStyles } from '@mantine/core';

export default createStyles((theme) => ({
	folderBackButton: {
		width: '100%',
		textAlign: 'left',
	},
	folderHeaderName: {
		width: '100%',
		height: '40px',
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		fontWeight: 'bold',
        fontSize: '1.2rem',
	},
	folderHeaderDescription: {
		width: '100%',
		textOverflow: 'ellipsis',
		whiteSpace: 'normal',
	},
    folderHeaderItem: {
        width: '100%',
    },
    folderPagination: {
        width: '100%',
        display: 'flex',
        justifyContent: 'center',
    },
    folderTable: {
        width: '100%',
		margin: '0 auto',
		maxWidth: '1024px',
		tableLayout: 'fixed',
		wordBreak: 'break-all',
		wordWrap: 'break-word',

		'& th': {
			width: '100%',
			backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[2],

			'&:first-child': {
				width: '39%',
				[theme.fn.smallerThan('md')]: {
					width: '40%',
				},
				textAlign: 'center',
			},

			'&:nth-child(2)': {
				width: '19%',
				[theme.fn.smallerThan('md')]: {
					width: '30%',
				},
				maxWidth: '100px',
				textAlign: 'center',
			},

			'&:nth-child(3)': {
				width: '12%',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
				textAlign: 'center',
			},

			'&:nth-child(4)': {
				width: '15%',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
				textAlign: 'center',
			},
			'&:nth-child(5)': {
				width: '15%',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
				textAlign: 'center',
			},
			'&:nth-child(6)': {
				width: '30%',
				display: 'none',
				[theme.fn.smallerThan('md')]: {
					display: 'table-cell',
				},
				textAlign: 'center',
			},
		},

		'& tr': {
			width: '100%',
			
			'&:nth-child(even)': {
				backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[5] : theme.colors.gray[1],
			},
			'&:hover': {
				backgroundColor: theme.colorScheme === 'dark' ? theme.colors.dark[6] : theme.colors.gray[2],
			},
		},

		'& td': {
			width: '100%',
			wordBreak: 'break-word',

			'&:first-child': {
				width: '39%',
				[theme.fn.smallerThan('md')]: {
					width: '40%',
				},
			},

			'&:nth-child(2)': {
				width: '19%',
				[theme.fn.smallerThan('md')]: {
					width: '30%',
				},
				textAlign: 'center',
				maxWidth: '100px',
				fontSize: '0.8rem',

				'& img': {
					width: '50px',
					height: '50px',
					borderRadius: '50%',
				},
			},

			'&:nth-child(3)': {
				width: '12%',
				textAlign: 'center',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
			},

			'&:nth-child(4)': {
				width: '15%',
				textAlign: 'center',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
			},

			'&:nth-child(5)': {
				width: '15%',
				textAlign: 'center',
				[theme.fn.smallerThan('md')]: {
					display: 'none',
				},
			},

			'&:nth-child(6)': {
				width: '30%',
				display: 'none',
				textAlign: 'center',
				[theme.fn.smallerThan('md')]: {
					display: 'table-cell',
					wordBreak: 'break-word',
				},
			},

		},
    },
}));